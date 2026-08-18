# GameDevRadar — 个人向游戏开发技术雷达

每天自动采集 GitHub / 博客 / 论坛的新项目与新话题，按**你的个人画像**评分过滤，
生成一份只服务于你一个人的技术热榜。不是 HelloGitHub 那种"面向所有人的周刊"，
是"面向你一个人的过滤器"。

零依赖（Python ≥ 3.9 标准库），零服务成本。

## 快速开始

```bash
python radar.py              # 生成 output/daily-<今天>.md + output/latest.md
python radar.py --dry-run    # 只打印不写文件
python radar.py --top 20 --days 30
```

可选：设置 `GITHUB_TOKEN` 环境变量提高 GitHub API 限额。

## 核心：画像就是过滤器（profile.json）

榜单不准时**改画像，别忍着**：

| 配置 | 作用 |
|---|---|
| `boost_keywords` / `boost_weight` | 命中加分（URP、热更、AI 工作流……） |
| `weak_keywords` / `weak_weight` | 弱相关保留（其他引擎、DCC 工具……） |
| `block_keywords` | 直接踢掉（SEO 垃圾仓、破解、赌博……） |
| `github_queries` | 采集哪些 GitHub 搜索词 |
| `reddit_subs` / `rss_feeds` | 其他信息源 |

评分公式：`热度增速（星数 ÷ 存活天数）× 画像相关度权重`。
增速而不是总量——防止老项目霸榜，让"三天冒出来的新东西"浮上来。

## 每日自动运行（两选一或都用）

### 方案 A：GitHub Actions（云端，推荐长期用）

仓库里已带 `.github/workflows/daily.yml`：每天定时跑 + 把榜单 commit 回仓库。
在 Actions 页手动触发一次即可验证。无需任何密钥（公开 API）。
Reddit / Unity 论坛在部分网络环境会被 403，GitHub Actions 的 IP 段通常正常。

### 方案 B：Kimi 定时任务（本机）

Kimi Work 自带定时任务（Blueprint Automation）：每天固定时间在本机唤醒 agent，
跑脚本 + 用 LLM 做"软判断"（读描述踢垃圾、写一句话点评）。
比方案 A 多一层 LLM 智能点评；要求电脑当时开机。

## 分层设计

```
radar.py        采集 + 硬信号评分 + 画像过滤 + 生成 Markdown（无 LLM, 哪里都能跑）
raw-*.json      原始候选, 留给 LLM 点评层消费
LLM 点评层      (可选, 如 Kimi 定时任务) 读 raw json → 踢垃圾 → 写点评 → 出终榜
```

脚本层保证"没有 LLM 也能出榜"；LLM 层负责"判断哪些是 SEO 垃圾、
哪些一句话说清为什么值得关注"。

## 输出

- `output/daily-YYYY-MM-DD.md` — 每日榜单（带日期，可积累成时间序列）
- `output/latest.md` — 永远是最新一期
- `output/raw-YYYY-MM-DD.json` — 原始候选数据

积累几周后回看时间序列：**持续多天在榜 = 真趋势，一日游 = 热闹**。

## License

MIT
