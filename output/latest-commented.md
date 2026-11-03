# 游戏开发技术雷达 · 2026-11-03（LLM 点评版）

> 采集于 2026-11-03 10:24 (UTC+8)。reddit r/Unity3D、r/gamedev 与 Unity 论坛照常 403 被拒，已跳过，不影响主榜。

**今日看点**

1. **coop-anything 破 1800（1823★）**，BRUTAL 2349★、blender-skills 672★ 继续稳涨——主线三板斧未见疲态。
2. **两张次新面孔同步翻倍**：CardboardXR 一天 80→130★，LGUI 49→79★——低成本 XR 原型与"AI agent 专用 UI 框架"都在加速兑现热度。
3. **rigby2d 121★、oni-moono 90★ 持续爬升**；dump-tool 连续两日未回榜，il2cpp 逆向工具热度暂歇。

![候选评分 Top10](chart-score.png)

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 536.2 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 1823★ / 17天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，17 天破 1800★；存量玩法改造思路启发极大，商用注意版权边界。 |
| 389.07 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 672★ / 19天 | Professional Blender 5.2 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，DCC 管线 AI 化样板；TA/资产管线同学强推，skill 分层设计值得抄作业。 |
| 162.5 | [brenpoly/CardboardXR](https://github.com/brenpoly/CardboardXR) | github | 130★ / 4天 | Google Cardboard with 6DoF head tracking, passthrough and ha | 用 ARCore/ARKit 给 Cardboard 补 6DoF/透视/手势的 Unity 插件，一天翻倍；做 XR 玩法验证不用再等 Quest，5 分钟值回票价。 |
| 123.63 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2349★ / 19天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染）；ONI 实战背书后生态持续升温，.NET 底座价值持续兑现。 |
| 72.38 | [LucasJRamos/spritekiln](https://github.com/LucasJRamos/spritekiln) | github | 79★ / 12天 | Animated pixel-art sprites (8-direction top-down or side vie | YAML 写程序化 3D 模型、烘出 8 方向像素精灵动画，专为 coding AI 设计且原生导出 Unity；做像素风卡牌/SLG 的强推。 |
| 65.67 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 215★ / 18天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 51.42 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 121★ / 20天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；涨势不减，暂缺 Unity runtime 是硬伤，继续观察。 |
| 44.77 | [Zengkailin/parametric-topdown-pixel-architecture](https://github.com/Zengkailin/parametric-topdown-pixel-architecture) | github | 57★ / 7天 | 参数化2D topdown像素建筑 - 把参数化 3D 建筑渲染成 2D 俯视像素图的本地网页工具（Godot hd2d | 参数化 3D 建筑一键烘成 2D 俯视像素素材的网页工具；引擎无关、本地即用，做 topdown 场景的 5 分钟稳赚。 |
| 41.85 | [LumioGames/LGUI](https://github.com/LumioGames/LGUI) | github | 79★ / 17天 | Game UI framework for AI agents, made by LumioGames. Build s | 给 AI agent 用的游戏 UI 框架：浏览器搭界面/逻辑/测试再导出 Unity，FairyGUI/Puerts 技术栈；星数翻倍，做 UI 自动化的抓紧试玩。 |
| 39.45 | [zoran123456/lola-retro](https://github.com/zoran123456/lola-retro) | github | 150★ / 19天 | CRT shader for retroarch, unity, kodi, amiberry and 86box | 一份 CRT 滤镜 shader 同时适配 Unity 与多个模拟器平台；做复古像素风手游的直接接入后处理链即可。 |
| 37.8 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 63★ / 15天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |
| 31.35 | [kanaworksai-hue/effekseer-vfx-skill](https://github.com/kanaworksai-hue/effekseer-vfx-skill) | github | 76★ / 23天 | Agent skills that make real-time game VFX: procedural Effeks | 把 Effekseer 实时特效变成"特效即代码"的 agent skill；官方验证基于 Godot 但 Effekseer 有 Unity runtime，特效管线 AI 化可接。 |
| 25.76 | [WouterApts/openik-unity](https://github.com/WouterApts/openik-unity) | github | 92★ / 25天 | Inverse Kinematics package for Unity, featuring multiple sol | Unity 多求解器 IK 包（CCD/FABRIK 等），真工具无水分；动画/TA 向强推，角色接地、瞄准姿态场景直接可用。 |
| 25.0 | [tigin-backwards/oni-moono](https://github.com/tigin-backwards/oni-moono) | github | 90★ / 18天 | Oxygen Not Included's Unity layer on the BRUTAL framework, . | ONI 的 Unity 层迁到 BRUTAL + .NET 10 CoreCLR，后期殖民地约 8× 帧率；做性能优化/Mono→CoreCLR 迁移的活教材，强推一读。 |
| 21.0 | [Scaling Scritchy Scratchy across platforms](https://unity.com/blog/scaling-scritchy-scratchy-across-platforms) | Unity Blog | 官方发布 | Learn how Scritchy Scratchy used Unity to port the game acro | Unity 官方小团队跨平台移植实战案例；做多端发行的值得看官方总结的坑位清单。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
