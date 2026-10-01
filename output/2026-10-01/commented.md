# 游戏开发技术雷达 · 2026-10-01（LLM 点评版）

**今日看点**
1. bot-crossing 744★ 重回榜首；QCode 掉出榜单（评分跌破前 20），第一轮 AI 游戏化学习的热度周期走完。
2. YAUI 五天 68★ 稳步上行——Flexbox 式 Unity UI 系统持续被认可，做卡牌/UI 的保持关注。
3. 警示样本：shiedless/unity-il2cpp-esp-tutorial 是 iOS IL2CPP 游戏的 ESP 透视外挂教程，37★ 上榜即被剔除——外挂类内容请勿沾边，做反外挂的倒可以研究其手法。

![候选评分 Top10](chart-score.png)

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 76.98 | [Station-Sciences/bot-crossing](https://github.com/Station-Sciences/bot-crossing) | github | 744★ / 29天 | A video game for AI agents. Created by Jarren Rocks. | 给 AI agent 玩的游戏，29 天 744★ 重回榜首；做 AI 工作流/游戏 AI 的值得看它的 agent 交互协议设计。 |
| 2 | 61.14 | [TheGreatSimon/AnvilCSG](https://github.com/TheGreatSimon/AnvilCSG) | github | 163★ / 16天 | Anvil CSG Beta 2: a free legacy preview of brush-based level design inside Unity. Hammer-inspired workflow, Built-in and URP. | Hammer 式笔刷关卡设计工具；Unity 关卡白盒刚需，做关卡/TA 的强烈建议试。 |
| 3 | 44.49 | [frrazer/roblox-game-boilerplate](https://github.com/frrazer/roblox-game-boilerplate) | github | 89★ / 6天 | Roblox game boilerplate for AI agents: Rojo, Wally, typed Packages. | 专为 AI agent 打造的 Roblox 工程脚手架；"AI 友好模板"品类代表，做游戏 AI 基建的值得看它的结构约定。 |
| 4 | 40.8 | [ruccho/YAUI](https://github.com/ruccho/YAUI) | github | 68★ / 5天 | Yet Another Unity UI: a fast, Flexbox-based UI system on GameObjects. | 直接在 GameObject 上做 Flexbox 布局的高性能 UI 系统；做卡牌/复杂 UI 的重点关注，手游 UI 适配友好。 |
| 5 | 21.0 | [Scritchy Scratchy 跨平台移植复盘](https://unity.com/blog/scaling-scritchy-scratchy-across-platforms) | Unity Blog | 官方发布 | Learn how Scritchy Scratchy used Unity to port the game across platforms. | 官方跨平台移植案例，多平台适配的实战参考。 |
| 6 | 19.5 | [Meta 发布移动端/浏览器 AI 游戏工具，新 VR 眼镜首日支持 Unity](https://www.gamesindustry.biz/meta-unveils-ai-powered-game-tools-for-mobile-devices-and-browsers-announces-vr-glasses-with-day-one-unity-support) | GamesIndustry.biz | 官方发布 | Meta has revealed two AI-powered game tools that let players... | 行业重磅：Meta 大厂入场 AI 游戏工具；新硬件首日支持 Unity，引擎生态位巩固。 |
| 7 | 17.82 | [GamePhanesStudio/GamePhanes](https://github.com/GamePhanesStudio/GamePhanes) | github | 475★ / 40天 | An open-source game coding agent environment and benchmark for Godot. | "AI 做游戏"的开源评测环境（Godot 向）；作为 AI 编码能力标尺保持趋势级关注。 |
| 8 | 16.62 | [UpstandPlatform/OpenRive](https://github.com/UpstandPlatform/OpenRive) | github | 14★ / 8天 | Build interactive UIs, motion, and game experiences in the OpenRive Editor, or let an AI agent generate them through the CLI, running natively across platforms. | 开源 Rive 替代：UI/动效编辑器 + AI agent CLI 生成，全平台原生运行；做 UI 动效/卡牌表现的可关注其成熟度。 |
| 9 | 16.5 | [Backyard Baseball 3D 关卡与 VFX 复盘](https://unity.com/blog/reimagining-backyard-baseball-3d-level-design-and-environment-art) | Unity Blog | 官方发布 | Mega Cat Studios 用可读性关卡设计、decal、灯光和 VFX 把 Backyard Baseball 2026 做成 3D。 | 官方案例：decal + 灯光的可读性关卡设计，对移动端场景表现力有参考价值。 |
| 10 | 15.0 | [Content directories：超越 AssetBundle](https://unity.com/blog/content-directories-beyond-the-assetbundle) | Unity Blog | 官方发布 | Unity 6.6 引入 content directories，更快更细粒度的 AssetBundle 替代方案，支持远程分发。 | 资源系统换代 + 远程分发内置，做热更/资源管理的一线开发者必看。 |
| 11 | 15.0 | [Deep Rock Galactic: Survivor 手游优化复盘](https://unity.com/blog/optimizing-deep-rock-galactic-survivor-for-mobile) | Unity Blog | 官方发布 | Piktiv 如何把 Deep Rock Galactic: Survivor 移植到移动端。 | 幸存者 like 爆款的手游移植优化复盘，海量同屏单位的性能处理对商业手游直接对口。 |
| 12 | 15.0 | [Unity 官方 Codex 插件](https://unity.com/blog/unity-plugin-codex) | Unity Blog | 官方发布 | Unity's official plugin for Codex. | 官方 Codex 插件官宣，官方 AI 编码助手全家桶成型；做 AI 工作流的必看。 |
| 13 | 15.0 | [Sente 六人策略的数据驱动棋盘](https://unity.com/blog/data-driven-board-six-player-strategy-sente) | Unity Blog | 官方发布 | Oxobox 用 Timeline + 数据驱动棋盘做六人同时回合制策略游戏。 | 数据驱动棋盘 + Timeline 表现层，做战棋/卡牌的架构可参考。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 点评由 LLM 按 profile.json 画像撰写 · 原始榜单见 daily.md*
