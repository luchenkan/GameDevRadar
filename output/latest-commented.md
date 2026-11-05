# 游戏开发技术雷达 · 2026-11-05（LLM 点评版）

> 采集于 2026-11-05 10:23 (UTC+8)。reddit/Unity 论坛照常 403，PocketGamer 今日 SSL 失败，已跳过，不影响主榜。

**今日看点**

1. **coop-anything 1892★ 逼近 1900，blender-skills 712★**，genex-desktop 连续两日站稳前三——AI 全栈开发工具热度固化。
2. **effekseer-vfx-skill 一天翻倍（77→149★）**——"特效即代码"赛道突然爆发，是今日最大异动；CardboardXR 六连涨至 205★。
3. **创作工具群全面上行**：rigby2d 139★、oni-moono 122★、LGUI 98★ 逼近破百。

![候选评分 Top10](chart-score.png)

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 497.9 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 1892★ / 19天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，19 天逼近 1900★；存量玩法改造思路启发极大，商用注意版权边界。 |
| 372.9 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 712★ / 21天 | Professional Blender 5.2 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，DCC 管线 AI 化样板；TA/资产管线同学强推，skill 分层设计值得抄作业。 |
| 240.0 | [genex-games/genex-desktop](https://github.com/genex-games/genex-desktop) | github | 792★ / 33天 | Desktop app for game dev with AI: make games in Unreal Engin | AI 桌面应用：agent 自己装引擎、搭游戏、跑自测，支持 Unity/Unreal/Godot/浏览器；赛道第一梯队，值得看它的能力边界。 |
| 170.85 | [brenpoly/CardboardXR](https://github.com/brenpoly/CardboardXR) | github | 205★ / 6天 | Google Cardboard with 6DoF head tracking, passthrough and ha | 用 ARCore/ARKit 给 Cardboard 补 6DoF/透视/手势的 Unity 插件，六连涨破 200；做 XR 玩法验证不用再等 Quest。 |
| 112.48 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2362★ / 21天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染）；.NET 底座价值持续兑现，架构思路值得跟。 |
| 63.69 | [LucasJRamos/spritekiln](https://github.com/LucasJRamos/spritekiln) | github | 81★ / 14天 | Animated pixel-art sprites (8-direction top-down or side vie | YAML 写程序化 3D 模型、烘出 8 方向像素精灵动画，专为 coding AI 设计且原生导出 Unity；做像素风卡牌/SLG 的强推。 |
| 59.67 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 217★ / 20天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 56.62 | [kanaworksai-hue/effekseer-vfx-skill](https://github.com/kanaworksai-hue/effekseer-vfx-skill) | github | 149★ / 25天 | Agent skills that make real-time game VFX: procedural Effeks | "特效即代码" agent skill 一天翻倍，今日最大异动；官方验证基于 Godot 但 Effekseer 有 Unity runtime，特效管线 AI 化值得立刻跟进。 |
| 53.72 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 139★ / 22天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；涨势不减，暂缺 Unity runtime 是硬伤，继续观察。 |
| 46.44 | [LumioGames/LGUI](https://github.com/LumioGames/LGUI) | github | 98★ / 19天 | Game UI framework for AI agents, made by LumioGames. Build s | 给 AI agent 用的游戏 UI 框架：浏览器搭界面/逻辑/测试再导出 Unity，FairyGUI/Puerts 技术栈；逼近破百，做 UI 自动化的抓紧试玩。 |
| 38.55 | [zoran123456/lola-retro](https://github.com/zoran123456/lola-retro) | github | 162★ / 21天 | CRT shader for retroarch, unity, kodi, amiberry and 86box | 一份 CRT 滤镜 shader 同时适配 Unity 与多个模拟器平台；做复古像素风手游的直接接入后处理链即可。 |
| 37.29 | [Zengkailin/parametric-topdown-pixel-architecture](https://github.com/Zengkailin/parametric-topdown-pixel-architecture) | github | 61★ / 9天 | 参数化2D topdown像素建筑 - 把参数化 3D 建筑渲染成 2D 俯视像素图的本地网页工具（Godot hd2d | 参数化 3D 建筑一键烘成 2D 俯视像素素材的网页工具；引擎无关、本地即用，做 topdown 场景的 5 分钟稳赚。 |
| 36.0 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 68★ / 17天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |
| 30.5 | [tigin-backwards/oni-moono](https://github.com/tigin-backwards/oni-moono) | github | 122★ / 20天 | Oxygen Not Included's Unity layer on the BRUTAL framework, . | ONI 的 Unity 层迁到 BRUTAL + .NET 10 CoreCLR，后期殖民地约 8× 帧率；做性能优化/CoreCLR 迁移的活教材，强推一读。 |
| 24.08 | [WouterApts/openik-unity](https://github.com/WouterApts/openik-unity) | github | 93★ / 27天 | Inverse Kinematics package for Unity, featuring multiple sol | Unity 多求解器 IK 包（CCD/FABRIK 等），真工具无水分；动画/TA 向强推，角色接地、瞄准姿态场景直接可用。 |
| 22.95 | [Alone9898/SpineSlim](https://github.com/Alone9898/SpineSlim) | github | 51★ / 20天 | Spine 资源优化与批量处理工具，支持版本转换、纹理压缩和 PC / 移动端资源整理。A desktop toolki | Spine 资源版本转换 + 纹理压缩 + 批量整理桌面工具；2D/卡牌项目资源管线刚需，国产工具文档友好。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
