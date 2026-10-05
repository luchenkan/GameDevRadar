# 游戏开发技术雷达 · 2026-10-05（LLM 点评版）

**今日看点**
1. 新面孔 trevaintdead/ai-game-modding-guides：首日 30★，教用 AI 编码 agent 做游戏 mod 的指南集（passthrough mod、Rust 重写、mod loader、prompting 技巧）——AI 工作流的新样本，做 AI 辅助开发的可翻。
2. 被剔除的旁观察：Godot 的 procedural-pixel-creatures 三连冠（156★/3天，评分 234.0 继续全场最高）——Godot 程序化生成工具热度不减，连续三日趋势记录。
3. Revolution 满一周涨星停滞（102★/8天，分数回落）——描述信息量仍为零，验货窗口期，谨慎对待。

![候选评分 Top10](chart-score.png)

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 69.9 | [Station-Sciences/bot-crossing](https://github.com/Station-Sciences/bot-crossing) | github | 769★ / 33天 | A video game for AI agents. Created by Jarren Rocks | "AI agent 玩的游戏"品类样本，769★ 热度稳定；做 AI 玩法/AI 游戏的值得看它怎么把 agent 当玩家设计。 |
| 2 | 49.8 | [TheGreatSimon/AnvilCSG](https://github.com/TheGreatSimon/AnvilCSG) | github | 166★ / 20天 | Anvil CSG Beta 2: a free legacy preview of brush-based level design inside Unity. Hammer-inspired workflow, Built-in and URP. | Hammer 式笔刷关卡设计工具，Built-in/URP 双支持；Unity 关卡白盒刚需，做关卡/TA 的强烈建议试。 |
| 3 | 38.25 | [Yokino337088/Revolution](https://github.com/Yokino337088/Revolution) | github | 102★ / 8天 | Revolution——具有革命性的Unity客户端框架。 | 自称革命性的 Unity 客户端框架，满一周涨星停滞且 README 无实质内容；宣传水分嫌疑增大，除非好奇否则可跳过。 |
| 4 | 30.0 | [trevaintdead/ai-game-modding-guides](https://github.com/trevaintdead/ai-game-modding-guides) | github | 30★ / 1天 | Guides for building game mods with AI coding agents: passthrough mods, Rust rewrites, mod loaders, prompting, and troubleshooting. | 用 AI 编码 agent 做游戏 mod 的实战指南集，含 prompting 与排错；做 AI 工作流/工具链的值得翻，纯文档仓无代码框架。 |
| 5 | 27.9 | [frrazer/roblox-game-boilerplate](https://github.com/frrazer/roblox-game-boilerplate) | github | 93★ / 10天 | Roblox game boilerplate for AI agents: Rojo, Wally, typed Packet networking, ProfileStore data, strict Luau and an AGENTS.md | 专为 AI agent 协作设计的 Roblox 工程模板（自带 AGENTS.md）；"AI 友好型工程结构"参考样本，对 Unity 项目的 AI 协作改造有借鉴意义。 |
| 6 | 25.32 | [ruccho/YAUI](https://github.com/ruccho/YAUI) | github | 76★ / 9天 | Yet Another Unity UI: a fast, Flexbox-based UI system on GameObjects. | 直接在 GameObject 上做 Flexbox 布局的高性能 UI 系统；做卡牌/复杂 UI 的重点关注，手游 UI 适配友好。 |
| 7 | 21.0 | [Scaling Scritchy Scratchy across platforms](https://unity.com/blog/scaling-scritchy-scratchy-across-platforms) | Unity Blog | 官方发布 | Learn how Scritchy Scratchy used Unity to port the game across platforms. | 官方跨平台移植案例，多平台适配的实战参考。 |
| 8 | 16.62 | [UpstandPlatform/OpenRive](https://github.com/UpstandPlatform/OpenRive) | github | 21★ / 12天 | Build interactive UIs, motion, and game experiences in the OpenRive Editor, or let an AI agent generate them through the CLI. | 开源 Rive 替代品，亮点是 AI agent 可用 CLI 直接生成动画 UI；游戏 UI 动效管线 + AI 工作流方向值得跟踪。 |
| 9 | 16.5 | [Backyard Baseball 3D 关卡与 VFX 复盘](https://unity.com/blog/reimagining-backyard-baseball-3d-level-design-and-environment-art) | Unity Blog | 官方发布 | Mega Cat Studios 用可读性关卡设计、decal、灯光和 VFX 把 Backyard Baseball 2026 做成 3D。 | 官方案例：decal + 灯光的可读性关卡设计，对移动端场景表现力有参考价值。 |
| 10 | 15.0 | [Content directories：超越 AssetBundle](https://unity.com/blog/content-directories-beyond-the-assetbundle) | Unity Blog | 官方发布 | Unity 6.6 引入 content directories，更快更细粒度的 AssetBundle 替代方案，支持远程分发。 | 资源系统换代 + 远程分发内置，做热更/资源管理的一线开发者必看。 |
| 11 | 15.0 | [Deep Rock Galactic: Survivor 手游优化复盘](https://unity.com/blog/optimizing-deep-rock-galactic-survivor-for-mobile) | Unity Blog | 官方发布 | Piktiv 如何把 Deep Rock Galactic: Survivor 移植到移动端。 | 幸存者 like 爆款的手游移植优化复盘，海量同屏单位的性能处理对商业手游直接对口。 |
| 12 | 15.0 | [Unity 官方 Codex 插件](https://unity.com/blog/unity-plugin-codex) | Unity Blog | 官方发布 | Unity's official plugin for Codex. | 官方 Codex 插件官宣，Unity 官方 AI 编码助手成型；做 AI 工作流的必看。 |
| 13 | 15.0 | [Sente 六人策略的数据驱动棋盘](https://unity.com/blog/data-driven-board-six-player-strategy-sente) | Unity Blog | 官方发布 | Oxobox 用 Timeline + 数据驱动棋盘做六人同时回合制策略游戏。 | 数据驱动棋盘 + Timeline 表现层，做战棋/卡牌的架构可参考。 |
| 14 | 15.0 | [Causa: Into the Dusk 的 Unity 6.3 移动端移植](https://unity.com/blog/adapting-causa-into-the-dusk-for-mobile) | Unity Blog | 官方发布 | Niebla Games 把 2021 年的 PC 游戏搬上 Google Play Pass 的实战视频。 | 少见的 PC→手游移植实战分享，做移植或多平台适配的值得看。 |
| 15 | 15.0 | [PocketGamer 一周观点：AppLovin vs Unity、Supercell 未遂交易、Mario Kart Tour 停运](https://www.pocketgamer.biz/mobile-tracker-claims-the-supercell-deal-that-never-happened-mario-kart-tours-closure-and-applovin-vs-unity-week-in-views/) | PocketGamer.biz | 官方发布 | 手游行业一周争议与大事汇总。 | 行业侧汇总：AppLovin 与 Unity 的争端直接关系买量变现生态，做商业化的值得读。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 点评由 LLM 按 profile.json 画像撰写 · 原始榜单见 daily.md*
