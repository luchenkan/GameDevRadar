# 游戏开发技术雷达 · 2026-11-10（LLM 点评版）

> 采集于 2026-11-10 10:23 (UTC+8)。reddit/Unity 论坛照常 403，已跳过，不影响主榜。

**今日看点**

1. **coop-anything 破 2000（2023★/24天）**——板块首个两千星项目；blender-skills 单日 +106 飙至 902★，DCC skill 化开始出圈。
2. **parametric-topdown-pixel-architecture 再涨 75★ 至 176★**，连续两日爆发；rigby2d 192★ 逼近 200，2D 动画/素材管线全面走强。
3. **链上雷达连续五日在榜、地缘政治纯游戏 world-dominion 再现**，均已剔除——引流与不对口条目仍在试探。

![候选评分 Top10](chart-score.png)

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 421.45 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 2023★ / 24天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，24 天破 2000★ 成板块首个两千星；存量玩法改造思路启发极大，商用注意版权边界。 |
| 381.59 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 902★ / 26天 | Professional Blender 5.2 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，单日 +106 破 900；TA/资产管线同学强推，skill 分层设计值得抄作业。 |
| 216.6 | [genex-games/genex-desktop](https://github.com/genex-games/genex-desktop) | github | 823★ / 38天 | Desktop app for game dev with AI: make games in Unreal Engin | AI 桌面应用：agent 自己装引擎、搭游戏、跑自测，支持 Unity/Unreal/Godot/浏览器；赛道第一梯队，值得看它的能力边界。 |
| 177.52 | [brenpoly/CardboardXR](https://github.com/brenpoly/CardboardXR) | github | 279★ / 11天 | Mobile VR with 6DoF head tracking, passthrough and hand trac | 用 ARCore/ARKit 给手机 VR 补 6DoF/透视/手势的 Unity 插件，十连涨逼近 300；做 XR 玩法验证不用再等 Quest。 |
| 93.08 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2420★ / 26天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染）；.NET 底座价值持续兑现，架构思路值得跟。 |
| 82.83 | [LucasJRamos/spritekiln](https://github.com/LucasJRamos/spritekiln) | github | 143★ / 19天 | Animated pixel-art sprites (8-direction top-down or side vie | YAML 写程序化 3D 模型、烘出 8 方向像素精灵动画，专为 coding AI 设计且原生导出 Unity；做像素风卡牌/SLG 的强推。 |
| 69.14 | [Zengkailin/parametric-topdown-pixel-architecture](https://github.com/Zengkailin/parametric-topdown-pixel-architecture) | github | 176★ / 14天 | 参数化2D topdown像素建筑 - 把参数化 3D 建筑渲染成 2D 俯视像素图的本地网页工具（Godot hd2d | 参数化 3D 建筑一键烘成 2D 俯视像素素材的网页工具；两日爆发至 176★，做 topdown 场景的 5 分钟稳赚。 |
| 60.44 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 192★ / 27天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；逼近 200★，暂缺 Unity runtime 是硬伤，继续观察。 |
| 51.02 | [kanaworksai-hue/effekseer-vfx-skill](https://github.com/kanaworksai-hue/effekseer-vfx-skill) | github | 161★ / 30天 | Agent skills that make real-time game VFX: procedural Effeks | "特效即代码" agent skill 稳步上行；官方验证基于 Godot 但 Effekseer 有 Unity runtime，特效管线 AI 化值得跟进。 |
| 50.16 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 228★ / 25天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 45.72 | [LumioGames/LGUI](https://github.com/LumioGames/LGUI) | github | 122★ / 24天 | Game UI framework for AI agents, made by LumioGames. Build s | 给 AI agent 用的游戏 UI 框架：浏览器搭界面/逻辑/测试再导出 Unity，FairyGUI/Puerts 技术栈；做 UI 自动化的抓紧试玩。 |
| 34.74 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 85★ / 22天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |
| 33.8 | [tigin-backwards/oni-moono](https://github.com/tigin-backwards/oni-moono) | github | 169★ / 25天 | Oxygen Not Included's Unity layer on the BRUTAL framework, . | ONI 的 Unity 层迁到 BRUTAL + .NET 10 CoreCLR，后期殖民地约 8× 帧率；做性能优化/CoreCLR 迁移的活教材，强推一读。 |
| 32.1 | [zoran123456/lola-retro](https://github.com/zoran123456/lola-retro) | github | 167★ / 26天 | CRT shader for retroarch, unity, kodi, amiberry and 86box | 一份 CRT 滤镜 shader 同时适配 Unity 与多个模拟器平台；做复古像素风手游的直接接入后处理链即可。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
