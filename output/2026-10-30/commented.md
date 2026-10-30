# 游戏开发技术雷达 · 2026-10-30（LLM 点评版）

> 采集于 2026-10-30 10:23 (UTC+8)。reddit r/Unity3D、r/gamedev 与 Unity 论坛照常 403 被拒，已跳过，不影响主榜。

**今日看点**

1. **三巨头齐涨**：coop-anything 1590★、BRUTAL 逼近 2300★（2299★）、blender-skills 585★——AI 改造存量游戏、.NET 引擎底座、DCC 工作流 skill 化三条主线继续走强。
2. **新面孔 parametric-topdown-pixel-architecture（52★/3天）**：把参数化 3D 建筑烘焙成 2D 俯视像素图的本地网页工具，程序生成像素素材管线的新样本，topdown/SLG 项目可即取即用。
3. **像素/复古创作工具群整体走强**：rigby2d 79★、lola-retro 142★、effekseer-vfx-skill 66★、Jeff-FX-forge 49★ 齐涨；对照之下 ludra 连续三日停在 20★，开始掉队。

![候选评分 Top10](chart-score.png)

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 611.55 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 1590★ / 13天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，逼近 1600★ 仍在涨；存量玩法改造思路启发极大，商用注意版权边界。 |
| 429.0 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 585★ / 15天 | Professional Blender 5.2 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，DCC 管线 AI 化样板；TA/资产管线同学强推，skill 分层设计值得抄作业。 |
| 153.27 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2299★ / 15天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染），非 Unity 插件但属 .NET 游戏底座级事件，架构思路值得跟。 |
| 95.31 | [Zengkailin/parametric-topdown-pixel-architecture](https://github.com/Zengkailin/parametric-topdown-pixel-architecture) | github | 52★ / 3天 | 参数化2D topdown像素建筑 - 把参数化 3D 建筑渲染成 2D 俯视像素图的本地网页工具（Godot hd2d | 参数化 3D 建筑一键烘成 2D 俯视像素素材的网页工具（Godot 工具移植版）；引擎无关、本地即用，做 topdown/卡牌场景的 5 分钟稳赚。 |
| 83.66 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 213★ / 14天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 47.35 | [zoran123456/lola-retro](https://github.com/zoran123456/lola-retro) | github | 142★ / 15天 | CRT shader for retroarch, unity, kodi, amiberry and 86box | 一份 CRT 滤镜 shader 同时适配 Unity 与多个模拟器平台；做复古像素风手游的直接接入后处理链即可。 |
| 41.99 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 79★ / 16天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；涨速在工具群里领先，暂缺 Unity runtime 是硬伤，继续观察。 |
| 32.97 | [kanaworksai-hue/effekseer-vfx-skill](https://github.com/kanaworksai-hue/effekseer-vfx-skill) | github | 66★ / 19天 | Agent skills that make real-time game VFX: procedural Effeks | 把 Effekseer 实时特效变成"特效即代码"的 agent skill；官方验证基于 Godot 但 Effekseer 有 Unity runtime，特效管线 AI 化可接。 |
| 31.5 | [Alone9898/SpineSlim](https://github.com/Alone9898/SpineSlim) | github | 49★ / 14天 | Spine 资源优化与批量处理工具，支持版本转换、纹理压缩和 PC / 移动端资源整理。A desktop toolki | Spine 资源版本转换 + 纹理压缩 + 批量整理桌面工具；2D/卡牌项目资源管线刚需，国产工具文档友好。 |
| 30.03 | [WouterApts/openik-unity](https://github.com/WouterApts/openik-unity) | github | 90★ / 21天 | Inverse Kinematics package for Unity, featuring multiple sol | Unity 多求解器 IK 包（CCD/FABRIK 等），真工具无水分；动画/TA 向强推，角色接地、瞄准姿态场景直接可用。 |
| 30.0 | [ersingvn/ludra](https://github.com/ersingvn/ludra) | github | 20★ / 5天 | The game engine built for AI agents. Games are JSON, errors  | AI-native 引擎概念（游戏即 JSON、报错带修复、无头自测）；星数连续三日停在 20，概念热度退去，降为低频跟踪。 |
| 28.62 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 35★ / 11天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |
| 27.52 | [wnsrn3436/open-game-works-core](https://github.com/wnsrn3436/open-game-works-core) | github | 22★ / 6天 | A harness that lets AI agents make server-client games of co | 让 AI agent 做服务器-客户端架构游戏的 harness；方向新、成色待验，继续观望。 |
| 24.48 | [Smidle/Jeff-FX-forge](https://github.com/Smidle/Jeff-FX-forge) | github | 49★ / 8天 | Jeff FX 1.0.1 — Godot 4.7 pixel VFX workshop. 82 presets, sp | Godot 4.7 像素特效工作台，82 预设 + sprite-sheet 导出；引擎不对口，"预设库 + 贴图导出"思路对 URP 特效侧有参照价值。 |
| 21.35 | [tcmmm/dump-tool](https://github.com/tcmmm/dump-tool) | github | 61★ / 10天 | il2cpp Dump 翻译工作台 - Unity il2cpp 游戏逆向辅助工具 | il2cpp 逆向分析工作台；正向价值是自查包体暴露面、评估加固强度，使用时注意合规，勿用于他人游戏。 |
| 21.0 | [Scaling Scritchy Scratchy across platforms](https://unity.com/blog/scaling-scritchy-scratchy-across-platforms) | Unity Blog | 官方发布 | Learn how Scritchy Scratchy used Unity to port the game acro | Unity 官方小团队跨平台移植实战案例；做多端发行的值得看官方总结的坑位清单。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
