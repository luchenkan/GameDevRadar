# 游戏开发技术雷达 · 2026-10-29（LLM 点评版）

> 采集于 2026-10-29 10:23 (UTC+8)。reddit r/Unity3D、r/gamedev 与 Unity 论坛照常 403 被拒，已跳过，不影响主榜。

**今日看点**

1. **coop-anything 1548★、blender-skills 571★、BRUTAL 2275★**——三条主线星数齐涨，AI agent 改造存量游戏、DCC 工作流 skill 化、.NET 自建引擎底座热度均无衰减。
2. **唯一新面孔 lola-retro（140★/14天）**：一份 CRT shader 通吃 retroarch/Unity/kodi/86box，做复古/街机风格手游的可以直接接入 URP 后处理链。
3. **"AI + 存量游戏资产"赛道持续发酵**：dump-tool（57★）、offline-game-revival（28★）连日上涨，Jeff-FX-forge、rigby2d 等创作向工具也在稳步爬星。

![候选评分 Top10](chart-score.png)

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 645.0 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 1548★ / 12天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，板块龙头破 1500★ 仍在涨；存量玩法改造思路启发极大，商用注意版权边界。 |
| 448.69 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 571★ / 14天 | Professional Blender 5.2 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，DCC 管线 AI 化样板；TA/资产管线同学强推，skill 分层设计值得抄作业。 |
| 162.5 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2275★ / 14天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染），非 Unity 插件但属 .NET 游戏底座级事件，架构思路值得跟。 |
| 89.7 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 212★ / 13天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 50.0 | [zoran123456/lola-retro](https://github.com/zoran123456/lola-retro) | github | 140★ / 14天 | CRT shader for retroarch, unity, kodi, amiberry and 86box | 一份 CRT 滤镜 shader 同时适配 Unity 与多个模拟器平台；做复古像素风手游的直接接入后处理链即可，5 分钟稳赚。 |
| 37.5 | [ersingvn/ludra](https://github.com/ersingvn/ludra) | github | 20★ / 4天 | The game engine built for AI agents. Games are JSON, errors  | AI-native 引擎：游戏即 JSON、报错自带修复建议、无头自测；星数连续两日停在 20，概念激进但落地待观察。 |
| 33.0 | [wnsrn3436/open-game-works-core](https://github.com/wnsrn3436/open-game-works-core) | github | 22★ / 5天 | A harness that lets AI agents make server-client games of co | 让 AI agent 做服务器-客户端架构游戏的 harness；方向新、成色待验，继续观望。 |
| 32.58 | [Alone9898/SpineSlim](https://github.com/Alone9898/SpineSlim) | github | 47★ / 13天 | Spine 资源优化与批量处理工具，支持版本转换、纹理压缩和 PC / 移动端资源整理。A desktop toolki | Spine 资源版本转换 + 纹理压缩 + 批量整理桌面工具；2D/卡牌项目资源管线刚需，国产工具文档友好。 |
| 31.5 | [WouterApts/openik-unity](https://github.com/WouterApts/openik-unity) | github | 90★ / 20天 | Inverse Kinematics package for Unity, featuring multiple sol | Unity 多求解器 IK 包（CCD/FABRIK 等），真工具无水分；动画/TA 向强推，角色接地、瞄准姿态场景直接可用。 |
| 29.45 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 68★ / 15天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；暂缺 Unity runtime 是硬伤，列入观察名单。 |
| 26.84 | [Smidle/Jeff-FX-forge](https://github.com/Smidle/Jeff-FX-forge) | github | 47★ / 7天 | Jeff FX 1.0.1 — Godot 4.7 pixel VFX workshop. 82 presets, sp | Godot 4.7 像素特效工作台，82 预设 + sprite-sheet 导出；引擎不对口，但"预设库 + 贴图导出"思路对 URP 特效侧有参照价值。 |
| 25.2 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 28★ / 10天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |
| 22.16 | [tcmmm/dump-tool](https://github.com/tcmmm/dump-tool) | github | 57★ / 9天 | il2cpp Dump 翻译工作台 - Unity il2cpp 游戏逆向辅助工具 | il2cpp 逆向分析工作台；正向价值是自查包体暴露面、评估加固强度，使用时注意合规，勿用于他人游戏。 |
| 21.0 | [Scaling Scritchy Scratchy across platforms](https://unity.com/blog/scaling-scritchy-scratchy-across-platforms) | Unity Blog | 官方发布 | Learn how Scritchy Scratchy used Unity to port the game acro | Unity 官方小团队跨平台移植实战案例；做多端发行的值得看官方总结的坑位清单。 |
| 19.2 | [UpstandPlatform/OpenRive](https://github.com/UpstandPlatform/OpenRive) | github | 60★ / 36天 | Build inetractive UIs, motion, and game experiences in the O | 开源 Rive 替代路线 + agent CLI；交互动效 UI 方向，做卡牌/运营活动页可关注其成熟度。 |
| 18.45 | [excifroge/VFX-Essential-Shader](https://github.com/excifroge/VFX-Essential-Shader) | github | 45★ / 22天 | VFX Essential Shader: an all-in-one VFX shader for Unity URP | URP 全家桶特效 shader，溶解/描边/全息一站齐；手游特效偷懒神器，改参数即用。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
