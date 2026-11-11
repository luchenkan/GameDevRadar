# 游戏开发技术雷达 · 2026-11-11（LLM 点评版）

> 采集于 2026-11-11 10:23 (UTC+8)。reddit/Unity 论坛照常 403，已跳过，不影响主榜。

**今日看点**

1. **blender-skills 单日再 +54 飙至 956★ 逼近千星**，coop-anything 2045★、genex-desktop 832★——AI 开发工具头部加速分化。
2. **新面孔 MetahumanURP（11★/1天）**：把 Epic Metahuman 导入 Unity 6 URP 的稀缺工具包，做数字人/写实角色的 URP 项目建议第一时间试。
3. **rigby2d 破 200（219★）**，parametric-topdown 184★ 连涨三日——2D 素材/动画管线热度不减。

![候选评分 Top10](chart-score.png)

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 409.0 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 2045★ / 25天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，破 2000★ 后涨势放缓；存量玩法改造思路启发极大，商用注意版权边界。 |
| 389.51 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 956★ / 27天 | Professional Blender 5.2 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，逼近千星；TA/资产管线同学强推，skill 分层设计值得抄作业。 |
| 213.3 | [genex-games/genex-desktop](https://github.com/genex-games/genex-desktop) | github | 832★ / 39天 | Desktop app for game dev with AI: make games in Unreal Engin | AI 桌面应用：agent 自己装引擎、搭游戏、跑自测，支持 Unity/Unreal/Godot/浏览器；赛道第一梯队，值得看它的能力边界。 |
| 166.81 | [brenpoly/CardboardXR](https://github.com/brenpoly/CardboardXR) | github | 286★ / 12天 | Mobile VR with 6DoF head tracking, passthrough and hand trac | 用 ARCore/ARKit 给手机 VR 补 6DoF/透视/手势的 Unity 插件，逼近 300；做 XR 玩法验证不用再等 Quest。 |
| 90.15 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2434★ / 27天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染）；.NET 底座价值持续兑现，架构思路值得跟。 |
| 82.5 | [LucasJRamos/spritekiln](https://github.com/LucasJRamos/spritekiln) | github | 150★ / 20天 | Animated pixel-art sprites (8-direction top-down or side vie | YAML 写程序化 3D 模型、烘出 8 方向像素精灵动画，专为 coding AI 设计且原生导出 Unity；做像素风卡牌/SLG 的强推。 |
| 67.48 | [Zengkailin/parametric-topdown-pixel-architecture](https://github.com/Zengkailin/parametric-topdown-pixel-architecture) | github | 184★ / 15天 | 参数化2D topdown像素建筑 - 把参数化 3D 建筑渲染成 2D 俯视像素图的本地网页工具（Godot hd2d | 参数化 3D 建筑一键烘成 2D 俯视像素素材的网页工具；连涨三日至 184★，做 topdown 场景的 5 分钟稳赚。 |
| 66.47 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 219★ / 28天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；破 200★，暂缺 Unity runtime 是硬伤，继续观察。 |
| 55.0 | [lofihashbloom/MetahumanURP](https://github.com/lofihashbloom/MetahumanURP) | github | 11★ / 1天 | Package to help import Epic's Metahuman into Unity 6 URP | 把 Epic Metahuman 导入 Unity 6 URP 的工具包；星数刚起步但直击 URP 数字人管线痛点，做写实角色的建议立刻试。 |
| 50.26 | [kanaworksai-hue/effekseer-vfx-skill](https://github.com/kanaworksai-hue/effekseer-vfx-skill) | github | 164★ / 31天 | Agent skills that make real-time game VFX: procedural Effeks | "特效即代码" agent skill 稳步上行；官方验证基于 Godot 但 Effekseer 有 Unity runtime，特效管线 AI 化值得跟进。 |
| 48.46 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 229★ / 26天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 45.36 | [LumioGames/LGUI](https://github.com/LumioGames/LGUI) | github | 126★ / 25天 | Game UI framework for AI agents, made by LumioGames. Build s | 给 AI agent 用的游戏 UI 框架：浏览器搭界面/逻辑/测试再导出 Unity，FairyGUI/Puerts 技术栈；做 UI 自动化的抓紧试玩。 |
| 34.47 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 88★ / 23天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |
| 34.25 | [tigin-backwards/oni-moono](https://github.com/tigin-backwards/oni-moono) | github | 178★ / 26天 | Oxygen Not Included's Unity layer on the BRUTAL framework, . | ONI 的 Unity 层迁到 BRUTAL + .NET 10 CoreCLR，后期殖民地约 8× 帧率；做性能优化/CoreCLR 迁移的活教材，强推一读。 |
| 31.1 | [zoran123456/lola-retro](https://github.com/zoran123456/lola-retro) | github | 168★ / 27天 | CRT shader for retroarch, unity, kodi, amiberry and 86box | 一份 CRT 滤镜 shader 同时适配 Unity 与多个模拟器平台；做复古像素风手游的直接接入后处理链即可。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
