# 游戏开发技术雷达 · 2026-10-08（LLM 点评版 · 补录）

> 本日为补录（当日缺席），配图从简，数据取自当日 raw.json。

**今日看点**
1. ai-game-modding-guides 持续爆发：218★→314★（仅 4 天），AI agent 做游戏 mod 的指南集成本周 GitHub 最大黑马——AI 工作流信号持续走强。
2. 新面孔 InfiniteGarden：Manifold Garden 风格无限建筑的 Unity 6 URP 技术复原（SDF 描边 + GPU 实例化 + Render Graph）——2★ 超早期，TA/渲染向值得盯。
3. 灰色仓 ninja-ripper-2.10 三日 106★ 零增长——刷量嫌疑坐实，维持剔除；SEO 味新仓 game-slop-purge 同日剔除。

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 78.5 | [trevaintdead/ai-game-modding-guides](https://github.com/trevaintdead/ai-game-modding-guides) | github | 314★ / 4天 | Guides for building game mods with AI coding agents: passthrough mods, Rust rewrites, mod loaders, prompting, and troubleshooting. | 用 AI 编码 agent 做游戏 mod 的实战指南集，四日 314★ 持续爆发，含 prompting 与排错；做 AI 工作流的值得翻，纯文档仓。 |
| 2 | 65.07 | [Station-Sciences/bot-crossing](https://github.com/Station-Sciences/bot-crossing) | github | 781★ / 36天 | A video game for AI agents. Created by Jarren Rocks | "AI agent 玩的游戏"品类样本，781★ 热度稳定；做 AI 玩法/AI 游戏的值得看它怎么把 agent 当玩家设计。 |
| 3 | 43.32 | [TheGreatSimon/AnvilCSG](https://github.com/TheGreatSimon/AnvilCSG) | github | 166★ / 23天 | Anvil CSG Beta 2: a free legacy preview of brush-based level design inside Unity. Hammer-inspired workflow, Built-in and URP. | Hammer 式笔刷关卡设计工具，Built-in/URP 双支持；Unity 关卡白盒刚需，做关卡/TA 的强烈建议试。 |
| 4 | 27.81 | [Yokino337088/Revolution](https://github.com/Yokino337088/Revolution) | github | 102★ / 11天 | Revolution——具有革命性的Unity客户端框架。 | 自称革命性的 Unity 客户端框架，连续五日零增长且无实质文档；宣传水分坐实，不建议花时间。 |
| 5 | 22.62 | [frrazer/roblox-game-boilerplate](https://github.com/frrazer/roblox-game-boilerplate) | github | 98★ / 13天 | Roblox game boilerplate for AI agents: Rojo, Wally, typed Packet networking, ProfileStore data, strict Luau and an AGENTS.md | 专为 AI agent 协作设计的 Roblox 工程模板（自带 AGENTS.md）；"AI 友好型工程结构"参考样本，对 Unity 项目的 AI 协作改造有借鉴意义。 |
| 6 | 21.0 | [Foxy Dumplings 全平台移植复盘](https://unity.com/blog/scaling-scritchy-scratchy-across-platforms) | Unity Blog | 官方发布 | Soft Crunch Games 如何把 Foxy Dumplings 做上全平台。 | 官方跨平台移植案例（标题更新，同一篇），多平台适配的实战参考。 |
| 7 | 19.5 | [ruccho/YAUI](https://github.com/ruccho/YAUI) | github | 78★ / 12天 | Yet Another Unity UI: a fast, Flexbox-based UI system on GameObjects. | 直接在 GameObject 上做 Flexbox 布局的高性能 UI 系统；做卡牌/复杂 UI 的重点关注，手游 UI 适配友好。 |
| 8 | 16.5 | [Backyard Baseball 3D 关卡与 VFX 复盘](https://unity.com/blog/reimagining-backyard-baseball-3d-level-design-and-environment-art) | Unity Blog | 官方发布 | Mega Cat Studios 用可读性关卡设计、decal、灯光和 VFX 把 Backyard Baseball 2026 做成 3D。 | 官方案例：decal + 灯光的可读性关卡设计，对移动端场景表现力有参考价值。 |
| 9 | 15.0 | [matbcontrol/InfiniteGarden](https://github.com/matbcontrol/InfiniteGarden) | github | 2★ / 1天 | Manifold Garden-inspired infinite architecture in Unity 6 URP: SDF outlines, an instanced world that tiles in every direction. | Manifold Garden 式无限建筑的 URP 技术复原：SDF 描边 + GPU 实例化平铺 + Render Graph；2★ 超早期，做 TA/渲染的值得盯一眼实现思路。 |
| 10 | 15.0 | [Content directories：超越 AssetBundle](https://unity.com/blog/content-directories-beyond-the-assetbundle) | Unity Blog | 官方发布 | Unity 6.6 引入 content directories，更快更细粒度的 AssetBundle 替代方案，支持远程分发。 | 资源系统换代 + 远程分发内置，做热更/资源管理的一线开发者必看。 |
| 11 | 15.0 | [Deep Rock Galactic: Survivor 手游优化复盘](https://unity.com/blog/optimizing-deep-rock-galactic-survivor-for-mobile) | Unity Blog | 官方发布 | Piktiv 如何把 Deep Rock Galactic: Survivor 移植到移动端。 | 幸存者 like 爆款的手游移植优化复盘，海量同屏单位的性能处理对商业手游直接对口。 |
| 12 | 15.0 | [Unity 官方 Codex 插件](https://unity.com/blog/unity-plugin-codex) | Unity Blog | 官方发布 | Unity's official plugin for Codex. | 官方 Codex 插件官宣，Unity 官方 AI 编码助手成型；做 AI 工作流的必看。 |
| 13 | 15.0 | [Sente 六人策略的数据驱动棋盘](https://unity.com/blog/data-driven-board-six-player-strategy-sente) | Unity Blog | 官方发布 | Oxobox 用 Timeline + 数据驱动棋盘做六人同时回合制策略游戏。 | 数据驱动棋盘 + Timeline 表现层，做战棋/卡牌的架构可参考。 |

---
*由 GameDevRadar 生成 · 点评由 LLM 按 profile.json 画像撰写 · 原始榜单见 daily.md*
