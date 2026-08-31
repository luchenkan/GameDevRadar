#!/usr/bin/env python3
"""GameDevRadar — 每日榜单配图生成器

用法: python charts.py <日期 YYYY-MM-DD>
读取 output/<日期>/raw.json, 在同目录生成两张图:
  chart-score.png    原始候选 Top10 评分横条图
  chart-sources.png  候选来源构成 + 各来源最高分
依赖 Daimon 托管 Python（自带 CJK 字体）。
"""
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(sys.executable).parent.parent.parent))
from daimon_runtime import setup_plot  # noqa: E402

import pandas as pd  # noqa: E402
import seaborn as sns  # noqa: E402
import matplotlib.pyplot as plt  # noqa: E402

ROOT = Path(__file__).resolve().parent


def main():
    day = sys.argv[1] if len(sys.argv) > 1 else None
    if not day:
        sys.exit("usage: python charts.py <YYYY-MM-DD>")
    day_dir = ROOT / "output" / day
    items = json.loads((day_dir / "raw.json").read_text(encoding="utf-8"))
    df = pd.DataFrame(items)
    df["kind"] = df["source"].apply(
        lambda s: "GitHub" if s == "github" else ("Reddit" if s.startswith("r/") else s))

    setup_plot()

    # ---- 图 1: Top10 评分 ----
    top = df.nlargest(10, "score").iloc[::-1]
    top = top.copy()
    top["label"] = top["title"].str.slice(0, 38)
    fig, ax = plt.subplots(figsize=(9, 5.2))
    sns.barplot(data=top, y="label", x="score", hue="kind",
                palette="Set2", dodge=False, ax=ax)
    ax.set_title(f"GameDevRadar {day} · 候选评分 Top10", fontsize=14)
    ax.set_xlabel("评分（热度增速 × 画像相关度）")
    ax.set_ylabel("")
    for i, v in enumerate(top["score"]):
        ax.text(v + 1, i, f"{v:.0f}", va="center", fontsize=9)
    ax.legend(title="来源", loc="lower right", fontsize=9)
    fig.savefig(day_dir / "chart-score.png", dpi=180, bbox_inches="tight")
    plt.close(fig)

    # ---- 图 2: 来源构成 ----
    fig, axes = plt.subplots(1, 2, figsize=(9, 3.8))
    counts = df["kind"].value_counts()
    axes[0].pie(counts, labels=counts.index, autopct="%d%%",
                colors=sns.color_palette("Set2"), startangle=90,
                textprops={"fontsize": 10})
    axes[0].set_title("候选来源构成", fontsize=12)
    best = df.groupby("kind")["score"].max().sort_values()
    sns.barplot(x=best.index, y=best.values, palette="Set2", ax=axes[1])
    axes[1].set_title("各来源最高分", fontsize=12)
    axes[1].set_xlabel("")
    axes[1].set_ylabel("最高分")
    axes[1].tick_params(axis="x", rotation=20)
    fig.savefig(day_dir / "chart-sources.png", dpi=180, bbox_inches="tight")
    plt.close(fig)

    print(f"[ok] {day_dir / 'chart-score.png'}")
    print(f"[ok] {day_dir / 'chart-sources.png'}")


if __name__ == "__main__":
    main()
