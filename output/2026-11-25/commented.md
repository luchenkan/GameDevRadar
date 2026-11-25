# 游戏开发技术雷达 · 2026-11-25（LLM 点评版）

> 采集于 2026-11-25 10:24 (UTC+8)。今日 Unity 论坛/GameDeveloper/PocketGamer 403、GamesIndustry 超时，原始候选仅 130 条，主榜已跳过失败源，GitHub 侧不受影响。

**今日看点**

1. **blender-skills 2859★ 继续领涨、逼近 2900**；同作者 godot-skills 51★/5天 稳步起量——EL4CTEO 的 DCC/引擎 skill 化矩阵成型。
2. **rigby2d 349★、oni-moono 278★、CardboardXR 357★ 缓涨不断更**；中期工具（BRUTAL/spritekiln/parametric-topdown）横盘但持续有人看。
3. **刷量与灰色仓回潮**：BioSignal-Guided-LLM-RL 单日 141★ 疑似刷量且与游戏开发无关，nier-reincarnation-pc 商业手游非官方移植、TotK 重编译、GFLFC 解密换号重发，均已剔除。

![候选评分 Top10](chart-score.png)

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 767.03 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 2859★ / 41天 | Professional Blender 5.3 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，逼近 2900★ 持续领涨；DCC 管线 AI 化头号样板，TA/资产管线必看。 |
| 402.5 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 2204★ / 39天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，横盘第九日热度不减；存量玩法改造思路启发极大，商用注意版权边界。 |
| 132.2 | [op7418/guizang-game-skill](https://github.com/op7418/guizang-game-skill) | github | 491★ / 13天 | 歸藏 Game Skill：让 AI 从零做出能在浏览器里玩的 3D 动作游戏，附可玩演示《雾锁山门》。Agent sk | 歸藏出品的 agent skill：three.js 单 HTML + AI 素材直出可玩 3D 动作游戏，马上破 500★；"从零到可玩"的 skill 编排是 AI 工作流最强样板，必拆。 |
| 96.11 | [brenpoly/CardboardXR](https://github.com/brenpoly/CardboardXR) | github | 357★ / 26天 | Mobile VR with 6DoF head tracking, passthrough and hand trac | 用 ARCore/ARKit 给手机 VR 补 6DoF/透视/手势的 Unity 插件；做 XR 玩法验证不用再等 Quest，移动端原型利器。 |
| 71.0 | [rtmandreyev/construct](https://github.com/rtmandreyev/construct) | github | 176★ / 31天 | Persistent, editable objects for AI agents: researched part  | 给 AI agent 的持久可编辑对象系统：逐部件研究、来源溯源，可带入游戏/影视/CAD；agent 基础设施新思路，TA/程序化管线值得看。 |
| 70.64 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 349★ / 42天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；稳步缓涨，暂缺 Unity runtime 是硬伤，继续观察。 |
| 62.27 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2553★ / 41天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染）；.NET 底座价值持续兑现，架构思路值得跟。 |
| 57.64 | [LucasJRamos/spritekiln](https://github.com/LucasJRamos/spritekiln) | github | 178★ / 34天 | Animated pixel-art sprites (8-direction top-down or side vie | YAML 写程序化 3D 模型、烘出 8 方向像素精灵动画，专为 coding AI 设计且原生导出 Unity；做像素风卡牌/SLG 的强推。 |
| 56.1 | [EL4CTEO/godot-skills](https://github.com/EL4CTEO/godot-skills) | github | 51★ / 5天 | Godot 4.7 Agent Skills for AI coding agents, based on Godot  | blender-skills 作者的 Godot 版：精确 API 查询 + 安全场景编辑 + 编译检查；引擎不对口但作者实力已验证，趋势级关注其 skill 工程方法。 |
| 46.64 | [Zengkailin/parametric-topdown-pixel-architecture](https://github.com/Zengkailin/parametric-topdown-pixel-architecture) | github | 246★ / 29天 | 参数化2D topdown像素建筑 - 把参数化 3D 建筑渲染成 2D 俯视像素图的本地网页工具（Godot hd2d | 参数化 3D 建筑一键烘成 2D 俯视像素素材的网页工具；引擎无关、本地即用，做 topdown 场景的 5 分钟稳赚。 |
| 36.27 | [LumioGames/LGUI](https://github.com/LumioGames/LGUI) | github | 157★ / 39天 | Game UI framework for AI agents, made by LumioGames. Build s | 给 AI agent 用的游戏 UI 框架：浏览器搭界面/逻辑/测试再导出 Unity，FairyGUI/Puerts 技术栈；做 UI 自动化的抓紧试玩。 |
| 34.75 | [tigin-backwards/oni-moono](https://github.com/tigin-backwards/oni-moono) | github | 278★ / 40天 | Oxygen Not Included's Unity layer on the BRUTAL framework, . | ONI 的 Unity 层迁到 BRUTAL + CoreCLR，后期殖民地约 8× 帧率；做性能优化/CoreCLR 迁移的活教材，强推一读。 |
| 32.17 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 234★ / 40天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 30.75 | [Wendystar0628/AI-All-Round-Bench](https://github.com/Wendystar0628/AI-All-Round-Bench) | github | 127★ / 31天 | AI全能测评 \| AI All-Round Bench: timed challenge benchmarks for | AI coding agent 限时挑战基准（含游戏 AI、3D/web 应用等 10 类任务）；非游戏工具，但评估手头 agent 实战能力时值得对照，趋势级。 |
| 29.43 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 121★ / 37天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
