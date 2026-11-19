# 游戏开发技术雷达 · 2026-11-19（LLM 点评版）

> 采集于 2026-11-19 10:23 (UTC+8)。reddit/Unity 论坛照常 403，已跳过，不影响主榜。

**今日看点**

1. **blender-skills 2228★ 正式超越 coop-anything（2174★）成全榜星数第一**——35 天从 0 到 2200+，DCC skill 化是本季度游戏工具圈最强曲线。
2. **rigby2d 破 300（300★/36天）**，parametric-topdown 226★、oni-moono 232★ 跟进；construct 156★ 稳涨。
3. **两个换号重发仓仍在榜**（GFLFC 解密工具、cat-rpocket-paw 引擎），星数原地不动疑似买星后断供，继续剔除。

![候选评分 Top10](chart-score.png)

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 700.26 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 2228★ / 35天 | Professional Blender 5.3 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，反超成全榜星数第一；DCC 管线 AI 化的头号样板，TA/资产管线同学必看。 |
| 329.4 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 2174★ / 33天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，星数让出第一但基本盘稳；存量玩法改造思路启发极大，商用注意版权边界。 |
| 205.0 | [op7418/guizang-game-skill](https://github.com/op7418/guizang-game-skill) | github | 410★ / 7天 | 歸藏 Game Skill：让 AI 从零做出能在浏览器里玩的 3D 动作游戏，附可玩演示《雾锁山门》。Agent sk | 歸藏出品的 agent skill：three.js 单 HTML + Tripo AI 素材直出可玩 3D 动作游戏；"从零到可玩"的 skill 编排是 AI 工作流最强样板，必拆。 |
| 119.35 | [brenpoly/CardboardXR](https://github.com/brenpoly/CardboardXR) | github | 341★ / 20天 | Mobile VR with 6DoF head tracking, passthrough and hand trac | 用 ARCore/ARKit 给手机 VR 补 6DoF/透视/手势的 Unity 插件；做 XR 玩法验证不用再等 Quest。 |
| 78.0 | [rtmandreyev/construct](https://github.com/rtmandreyev/construct) | github | 156★ / 25天 | Persistent, editable objects for AI agents: researched part  | 给 AI agent 的持久可编辑对象系统：逐部件研究、来源溯源，可带入游戏/影视/CAD/3D 打印；agent 基础设施新思路，TA/程序化管线值得看。 |
| 72.2 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2527★ / 35天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染）；.NET 底座价值持续兑现，架构思路值得跟。 |
| 70.81 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 300★ / 36天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；破 300★，暂缺 Unity runtime 是硬伤，继续观察。 |
| 67.98 | [LucasJRamos/spritekiln](https://github.com/LucasJRamos/spritekiln) | github | 173★ / 28天 | Animated pixel-art sprites (8-direction top-down or side vie | YAML 写程序化 3D 模型、烘出 8 方向像素精灵动画，专为 coding AI 设计且原生导出 Unity；做像素风卡牌/SLG 的强推。 |
| 54.06 | [Zengkailin/parametric-topdown-pixel-architecture](https://github.com/Zengkailin/parametric-topdown-pixel-architecture) | github | 226★ / 23天 | 参数化2D topdown像素建筑 - 把参数化 3D 建筑渲染成 2D 俯视像素图的本地网页工具（Godot hd2d | 参数化 3D 建筑一键烘成 2D 俯视像素素材的网页工具；引擎无关、本地即用，做 topdown 场景的 5 分钟稳赚。 |
| 43.13 | [kanaworksai-hue/effekseer-vfx-skill](https://github.com/kanaworksai-hue/effekseer-vfx-skill) | github | 177★ / 39天 | Agent skills that make real-time game VFX: procedural Effeks | "特效即代码" agent skill 稳步上行；官方验证基于 Godot 但 Effekseer 有 Unity runtime，特效管线 AI 化值得跟进。 |
| 37.62 | [LumioGames/LGUI](https://github.com/LumioGames/LGUI) | github | 138★ / 33天 | Game UI framework for AI agents, made by LumioGames. Build s | 给 AI agent 用的游戏 UI 框架：浏览器搭界面/逻辑/测试再导出 Unity，FairyGUI/Puerts 技术栈；做 UI 自动化的抓紧试玩。 |
| 37.51 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 232★ / 34天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 34.1 | [tigin-backwards/oni-moono](https://github.com/tigin-backwards/oni-moono) | github | 232★ / 34天 | Oxygen Not Included's Unity layer on the BRUTAL framework, . | ONI 的 Unity 层迁到 BRUTAL + .NET 10 CoreCLR，后期殖民地约 8× 帧率；做性能优化/CoreCLR 迁移的活教材，强推一读。 |
| 31.68 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 109★ / 31天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |
| 28.91 | [FrogCosmonaut/cosmo_tree_gen](https://github.com/FrogCosmonaut/cosmo_tree_gen) | github | 190★ / 23天 | Procedural tree generator addon for Godot 4 | Godot 程序化低模树生成器；引擎不对口，但程序化植被参数化思路对 URP 植被工具链有参照价值，趋势级。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
