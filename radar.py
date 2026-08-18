#!/usr/bin/env python3
"""GameDevRadar — 个人向游戏开发技术热榜采集器

零依赖（仅标准库）。采集 GitHub / Reddit / RSS 信号，按"个人画像"评分过滤，
生成每日 Markdown 热榜。

用法:
    python radar.py                      # 用默认画像跑, 输出到 output/daily-<日期>.md
    python radar.py --days 14 --top 20   # 自定义窗口与榜长
    python radar.py --dry-run            # 只打印不写文件

环境变量:
    GITHUB_TOKEN   可选, 提高 GitHub API 限额 (无 token 时 search 10 次/分钟)
"""
import json
import re
import sys
import time
import urllib.parse
import urllib.request
import xml.etree.ElementTree as ET
from datetime import datetime, timezone, timedelta
from pathlib import Path

ROOT = Path(__file__).resolve().parent
PROFILE_PATH = ROOT / "profile.json"
UA = {"User-Agent": "gamedev-radar/0.1", "Accept": "application/vnd.github+json"}


# ---------------------------------------------------------------- 基础 HTTP
def http_json(url: str, timeout: int = 25):
    req = urllib.request.Request(url, headers=UA)
    with urllib.request.urlopen(req, timeout=timeout) as r:
        return json.loads(r.read().decode("utf-8"))


def http_text(url: str, timeout: int = 25) -> str:
    req = urllib.request.Request(url, headers={"User-Agent": UA["User-Agent"]})
    with urllib.request.urlopen(req, timeout=timeout) as r:
        return r.read().decode("utf-8", errors="replace")


# ---------------------------------------------------------------- 采集: GitHub
def fetch_github(profile: dict, now: datetime) -> list:
    """按画像里的 github_queries 搜索近期新仓库, 计算星数增速。"""
    items = []
    days = profile.get("github_created_within_days", 45)
    since = (now - timedelta(days=days)).date().isoformat()
    for q in profile["github_queries"]:
        query = f"{q} created:>{since}"
        url = ("https://api.github.com/search/repositories?q="
               + urllib.parse.quote(query) + "&sort=stars&order=desc&per_page=20")
        try:
            data = http_json(url)
        except Exception as e:  # 单源失败不拖垮整体
            print(f"[warn] github query {q!r} failed: {e}", file=sys.stderr)
            continue
        for r in data.get("items", []):
            created = datetime.fromisoformat(r["created_at"].replace("Z", "+00:00"))
            age_days = max((now - created).days, 1)
            items.append({
                "source": "github",
                "title": r["full_name"],
                "url": r["html_url"],
                "desc": (r.get("description") or "").strip(),
                "lang": r.get("language") or "",
                "topics": r.get("topics") or [],
                "stars": r["stargazers_count"],
                "forks": r["forks_count"],
                "age_days": age_days,
                "velocity": round(r["stargazers_count"] / age_days, 2),
                "created": r["created_at"][:10],
            })
        time.sleep(2)  # 无 token 时友善限速
    return items


# ---------------------------------------------------------------- 采集: Reddit
def fetch_reddit(profile: dict) -> list:
    items = []
    for sub in profile.get("reddit_subs", []):
        url = f"https://www.reddit.com/r/{sub}/hot.json?limit=15"
        try:
            data = http_json(url)
        except Exception as e:
            print(f"[warn] reddit r/{sub} failed: {e}", file=sys.stderr)
            continue
        for post in data.get("data", {}).get("children", []):
            p = post["data"]
            if p.get("stickied"):
                continue
            items.append({
                "source": f"r/{sub}",
                "title": p.get("title", "").strip(),
                "url": "https://www.reddit.com" + p.get("permalink", ""),
                "desc": (p.get("selftext") or "")[:200].strip(),
                "lang": "", "topics": [p.get("link_flair_text") or ""],
                "stars": p.get("score", 0),
                "forks": p.get("num_comments", 0),
                "age_days": 1,
                "velocity": round(p.get("score", 0) / 7, 2),  # reddit 热度天然短周期
                "created": "",
            })
        time.sleep(2)
    return items


# ---------------------------------------------------------------- 采集: RSS
def fetch_rss(profile: dict, now: datetime) -> list:
    items = []
    window = profile.get("rss_within_days", 7)
    for feed in profile.get("rss_feeds", []):
        try:
            root = ET.fromstring(http_text(feed["url"]))
        except Exception as e:
            print(f"[warn] rss {feed['name']} failed: {e}", file=sys.stderr)
            continue
        for it in root.iter("item"):
            title = (it.findtext("title") or "").strip()
            link = (it.findtext("link") or "").strip()
            desc = re.sub(r"<[^>]+>", "", it.findtext("description") or "")[:200].strip()
            pub = it.findtext("pubDate") or ""
            try:
                dt = datetime.strptime(pub, "%a, %d %b %Y %H:%M:%S %z")
                if (now - dt).days > window:
                    continue
            except ValueError:
                pass
            items.append({
                "source": feed["name"], "title": title, "url": link, "desc": desc,
                "lang": "", "topics": [], "stars": 0, "forks": 0,
                "age_days": 1, "velocity": 3.0,  # 官方博客给基础分, 让它有机会上榜
                "created": pub[:16],
            })
    return items


# ---------------------------------------------------------------- 评分
def relevance(profile: dict, item: dict):
    """返回 (是否拦截, 相关度权重)。个人画像即过滤函数。"""
    text = " ".join([item["title"], item["desc"], item["lang"],
                     " ".join(item["topics"])]).lower()
    for kw in profile["block_keywords"]:
        if kw.lower() in text:
            return True, 0.0
    weight = 1.0
    for kw in profile["boost_keywords"]:
        if kw.lower() in text:
            weight += profile.get("boost_weight", 2.0)
    for kw in profile["weak_keywords"]:
        if kw.lower() in text:
            weight += profile.get("weak_weight", 0.5)
    # 无描述 + 无 topics 的仓库多半是噪音
    if item["source"] == "github" and not item["desc"] and not item["topics"]:
        weight *= 0.3
    return False, round(weight, 2)


def score_items(profile: dict, items: list) -> list:
    seen, out = set(), []
    for it in items:
        key = it["url"].split("?")[0].rstrip("/")
        if key in seen:
            continue
        seen.add(key)
        blocked, weight = relevance(profile, it)
        if blocked:
            continue
        it["relevance"] = weight
        # 总分 = 热度信号(增速) × 相关度权重; reddit/rss 有各自的基础 velocity
        it["score"] = round(it["velocity"] * weight, 2)
        out.append(it)
    out.sort(key=lambda x: x["score"], reverse=True)
    return out


# ---------------------------------------------------------------- 输出
def render_markdown(items: list, now: datetime, top: int) -> str:
    lines = [
        f"# 游戏开发技术雷达 · {now.date().isoformat()}",
        "",
        f"> 自动生成于 {now.strftime('%Y-%m-%d %H:%M')} (UTC+8)。"
        "评分 = 热度增速 × 个人画像相关度。仅供每日速览, 上榜与否不是质量背书。",
        "",
        "| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 |",
        "|---|---|---|---|---|---|",
    ]
    for i, it in enumerate(items[:top], 1):
        signal = (f"{it['stars']}★ / {it['age_days']}天"
                  if it["source"] == "github"
                  else f"{it['stars']} 热度" if it["source"].startswith("r/")
                  else "官方发布")
        desc = it["desc"].replace("|", "\\|")[:60] or "—"
        lines.append(
            f"| {i} | {it['score']} | [{it['title']}]({it['url']}) "
            f"| {it['source']} | {signal} | {desc} |")
    lines += [
        "",
        "---",
        "*由 GameDevRadar 生成 · 画像配置见 profile.json · "
        "觉得哪类多了/少了就改画像, 别忍着*",
    ]
    return "\n".join(lines) + "\n"


def main():
    args = sys.argv[1:]
    top = int(args[args.index("--top") + 1]) if "--top" in args else 15
    dry = "--dry-run" in args
    if "--days" in args:  # 覆盖 github 窗口
        days_override = int(args[args.index("--days") + 1])
    else:
        days_override = None

    profile = json.loads(PROFILE_PATH.read_text(encoding="utf-8"))
    if days_override:
        profile["github_created_within_days"] = days_override

    now = datetime.now(timezone(timedelta(hours=8)))
    items = []
    items += fetch_github(profile, now)
    items += fetch_reddit(profile)
    items += fetch_rss(profile, now)
    print(f"[info] collected {len(items)} raw items", file=sys.stderr)

    ranked = score_items(profile, items)
    md = render_markdown(ranked, now, top)

    (ROOT / "output").mkdir(exist_ok=True)
    raw_path = ROOT / "output" / f"raw-{now.date().isoformat()}.json"
    raw_path.write_text(json.dumps(ranked[:50], ensure_ascii=False, indent=1),
                        encoding="utf-8")
    if dry:
        print(md)
    else:
        out = ROOT / "output" / f"daily-{now.date().isoformat()}.md"
        out.write_text(md, encoding="utf-8")
        print(f"[ok] {out}")
        latest = ROOT / "output" / "latest.md"
        latest.write_text(md, encoding="utf-8")


if __name__ == "__main__":
    main()
