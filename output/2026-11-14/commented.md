# 游戏开发技术雷达 · 2026-11-14（LLM 点评版 · 补录）

> 补录于 2026-11-17（原始采集 2026-11-14 15:01 UTC+8），点评基于当日 raw 数据，当日配图从略。

**今日看点**

1. **guizang-game-skill 325★ 连续两日霸榜**，blender-skills 1072★、coop-anything 2113★ 紧随其后——AI skill 三强格局成型。
2. **新面孔 construct（123★/20天）**：给 AI agent 的持久可编辑对象系统，逐部件研究 + 来源溯源，可带入游戏/影视/CAD/3D 打印——"agent 可操作世界"的基础设施尝试。
3. **MetahumanURP 23★ 缓爬**；CardboardXR 308★、genex-desktop 869★ 稳步上行。

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 568.75 | [op7418/guizang-game-skill](https://github.com/op7418/guizang-game-skill) | github | 325★ / 2天 | 歸藏 Game Skill：让 AI 从零做出能在浏览器里玩的 3D 动作游戏，附可玩演示《雾锁山门》。Agent sk | 歸藏出品的 agent skill 两日 325★ 连庄：three.js 单 HTML + Tripo AI 素材直出可玩 3D 动作游戏；"从零到可玩"的 skill 编排是 AI 工作流最强样板，必拆。 |
| 393.03 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 1072★ / 30天 | Professional Blender 5.3 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，破千后继续涨；TA/资产管线同学强推，skill 分层设计值得抄作业。 |
| 377.3 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 2113★ / 28天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，稳在 2100+；存量玩法改造思路启发极大，商用注意版权边界。 |
| 206.9 | [genex-games/genex-desktop](https://github.com/genex-games/genex-desktop) | github | 869★ / 42天 | Desktop app for game dev with AI: make games in Unreal Engin | AI 桌面应用：agent 自己装引擎、搭游戏、跑自测，支持 Unity/Unreal/Godot/浏览器；赛道第一梯队，值得看它的能力边界。 |
| 143.71 | [brenpoly/CardboardXR](https://github.com/brenpoly/CardboardXR) | github | 308★ / 15天 | Mobile VR with 6DoF head tracking, passthrough and hand trac | 用 ARCore/ARKit 给手机 VR 补 6DoF/透视/手势的 Unity 插件，破 300；做 XR 玩法验证不用再等 Quest。 |
| 82.77 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2483★ / 30天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染）；.NET 底座价值持续兑现，架构思路值得跟。 |
| 77.44 | [LucasJRamos/spritekiln](https://github.com/LucasJRamos/spritekiln) | github | 162★ / 23天 | Animated pixel-art sprites (8-direction top-down or side vie | YAML 写程序化 3D 模型、烘出 8 方向像素精灵动画，专为 coding AI 设计且原生导出 Unity；做像素风卡牌/SLG 的强推。 |
| 76.88 | [rtmandreyev/construct](https://github.com/rtmandreyev/construct) | github | 123★ / 20天 | Persistent, editable objects for AI agents: researched part  | 给 AI agent 的持久可编辑对象系统：逐部件研究、来源溯源，可带入游戏/影视/CAD/3D 打印（Blender addon + MCP）；agent 基础设施新思路，TA/程序化管线值得看。 |
| 66.89 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 244★ / 31天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；稳涨，暂缺 Unity runtime 是硬伤，继续观察。 |
| 55.6 | [Zengkailin/parametric-topdown-pixel-architecture](https://github.com/Zengkailin/parametric-topdown-pixel-architecture) | github | 192★ / 19天 | 参数化2D topdown像素建筑 - 把参数化 3D 建筑渲染成 2D 俯视像素图的本地网页工具（Godot hd2d | 参数化 3D 建筑一键烘成 2D 俯视像素素材的网页工具；引擎无关、本地即用，做 topdown 场景的 5 分钟稳赚。 |
| 46.07 | [kanaworksai-hue/effekseer-vfx-skill](https://github.com/kanaworksai-hue/effekseer-vfx-skill) | github | 165★ / 34天 | Agent skills that make real-time game VFX: procedural Effeks | "特效即代码" agent skill 稳步上行；官方验证基于 Godot 但 Effekseer 有 Unity runtime，特效管线 AI 化值得跟进。 |
| 43.45 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 229★ / 29天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 42.39 | [LumioGames/LGUI](https://github.com/LumioGames/LGUI) | github | 132★ / 28天 | Game UI framework for AI agents, made by LumioGames. Build s | 给 AI agent 用的游戏 UI 框架：浏览器搭界面/逻辑/测试再导出 Unity，FairyGUI/Puerts 技术栈；做 UI 自动化的抓紧试玩。 |
| 38.35 | [lofihashbloom/MetahumanURP](https://github.com/lofihashbloom/MetahumanURP) | github | 23★ / 3天 | Package to help import Epic's Metahuman into Unity 6 URP | 把 Epic Metahuman 导入 Unity 6 URP 的工具包；星数缓爬但直击 URP 数字人管线痛点，做写实角色的建议立刻试。 |
| 35.15 | [tigin-backwards/oni-moono](https://github.com/tigin-backwards/oni-moono) | github | 204★ / 29天 | Oxygen Not Included's Unity layer on the BRUTAL framework, . | ONI 的 Unity 层迁到 BRUTAL + .NET 10 CoreCLR，后期殖民地约 8× 帧率；破 200★，做性能优化/CoreCLR 迁移的活教材，强推一读。 |
| 32.22 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 93★ / 26天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |
| 32.09 | [FrogCosmonaut/cosmo_tree_gen](https://github.com/FrogCosmonaut/cosmo_tree_gen) | github | 165★ / 18天 | Procedural low poly tree generator in Godot. | Godot 程序化低模树生成器；引擎不对口，但程序化植被参数化思路对 URP 植被工具链有参照价值，趋势级。 |

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
