# 游戏开发技术雷达 · 2026-10-27（LLM 点评版）

**今日看点**
1. coop-anything 1446★/10天 逼近 1500、blender-skills 530★——agent skill 板块双雄继续领涨。
2. ludra（20★/2天）与 open-game-works-core（17★/3天）两个"AI-native 游戏引擎/harness"新概念同步缓慢起量——方向确立但成色待验，保持观察。
3. mcp-game-asset-forge 连续两日 58★ 零增长——topics 全引擎堆砌 + 空壳文档仓，引流嫌疑坐实，维持剔除。

![候选评分 Top10](chart-score.png)

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 723.0 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 1446★ / 10天 | Co-op Anything: point your AI agent at any single-player game and play it with friends (online and/or splitscreen!). MCP server and playbook to reverse engineer, fix, test, ship and share. | "让 AI agent 把任意单机游戏逆向改造成联机"的 MCP 服务器 + 方法论，十日逼近 1500★；代表 agent 逆向工作流方向，注意版权边界，看技术思路即可。 |
| 2 | 485.87 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 530★ / 12天 | Professional Blender 5.2 workflows as Agent Skills for Claude Code, Codex, Gemini CLI, Copilot and Cursor: modeling, UVs, PBR, baking, rigging, animation, export and QA. | Blender 5.2 专业工作流整体 skill 化（建模/UV/PBR/烘焙/绑定/导出/QA），12 日 530★；做 TA/资产管线的强烈建议立刻试，AI 工作流直接对口。 |
| 3 | 187.75 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2253★ / 12天 | A free, open source C# framework for making games, or your own game engine. Direct access to native APIs and Vulkan, with the low-level libraries around them. The foundation under Kitten Space Agency and RimWorld's Waylith engine. | 开源 C# 底层游戏框架：原生 API + Vulkan 直访，RimWorld 新作引擎底座，12 日 2253★；虽非 Unity 插件，但 .NET 游戏开发底座级事件，做引擎/底层的必看。 |
| 4 | 105.0 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 210★ / 11天 | Make any NPC in any game talk back with Fish Audio: skills, a self-hosted voice server and tools for AI coding agents, with a full GTA V example. | 让任意游戏 NPC 开口说话的 agent skill 套件：Fish Audio TTS + 自托管语音服务 + GTA V 完整示例；做 AI NPC/语音交互的值得看，mod 向而非 Unity 工程向。 |
| 5 | 75.0 | [ersingvn/ludra](https://github.com/ersingvn/ludra) | github | 20★ / 2天 | The game engine built for AI agents. Games are JSON, errors come with fixes, and agents play-test their own games headlessly. | "为 AI agent 打造的游戏引擎"：游戏即 JSON、报错自带修复方案、agent 无头自测——AI-native 引擎新物种，概念激进值得跟踪，星数尚早。 |
| 6 | 42.52 | [wnsrn3436/open-game-works-core](https://github.com/wnsrn3436/open-game-works-core) | github | 17★ / 3天 | Harness that lets AI agents make server-client games of commercial quality on a prebuilt core. | 让 AI agent 在预制底座上开发"商业品质"服务器-客户端游戏的 harness；"AI 全流程做游戏"又一分支，关注但先观望成色。 |
| 7 | 34.58 | [WouterApts/openik-unity](https://github.com/WouterApts/openik-unity) | github | 89★ / 18天 | Inverse Kinematics package for Unity, featuring multiple solvers, joint limits and procedural animation samples. | Unity IK 包：CCD/FABRIK 多求解器 + 关节限制 + 程序化动画示例；做动画/TA/程序化动作的值得立刻试，真工具无水分。 |
| 8 | 31.95 | [Alone9898/SpineSlim](https://github.com/Alone9898/SpineSlim) | github | 39★ / 11天 | Spine 资源优化与批量处理工具，支持版本转换、纹理压缩和 PC / 移动端资源整理。 | Spine 资源优化桌面工具：版本转换 + 纹理压缩 + PC/移动端资源整理一站搞定；做 2D/卡牌手游资源管线的直接对口，建议试。 |
| 9 | 25.48 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 51★ / 13天 | Open-source 2D skeletal animation editor for games and film. | 开源 2D 骨骼动画工具（Spine 路线），内建 MCP server 供 AI 助手调用，导出 .skel 可对接 Spine 管线；暂无 Unity runtime 是硬伤，做 2D/卡牌动画的值得跟踪。 |
| 10 | 22.56 | [TheGreatSimon/AnvilCSG](https://github.com/TheGreatSimon/AnvilCSG) | github | 158★ / 42天 | Anvil CSG Beta 2: a free legacy preview of brush-based level design inside Unity. Hammer-inspired workflow, Built-in and URP. | Hammer 式笔刷关卡设计工具，Built-in/URP 双支持；Unity 关卡白盒刚需，做关卡/TA 的强烈建议试。 |
| 11 | 21.0 | [Scaling Scritchy Scratchy across platforms](https://unity.com/blog/scaling-scritchy-scratchy-across-platforms) | Unity Blog | 官方发布 | Learn how Scritchy Scratchy used Unity to port the game across platforms. | 官方跨平台移植案例，多平台适配的实战参考。 |
| 12 | 19.35 | [excifroge/VFX-Essential-Shader](https://github.com/excifroge/VFX-Essential-Shader) | github | 43★ / 20天 | VFX Essential Shader: an all-in-one VFX shader for Unity URP with a custom material inspector. | URP 全家桶 VFX shader + 自定义材质面板，一个材质应付多种特效需求；做手游特效的值得试，小体量项目尤其省事。 |
| 13 | 18.59 | [kanaworksai-hue/effekseer-vfx-skill](https://github.com/kanaworksai-hue/effekseer-vfx-skill) | github | 54★ / 16天 | Agent skills that make real-time game VFX as code for Effekseer and verify them in real Godot renders (Claude Code, Codex and other Agent Skills hosts). | 让 AI agent 用代码编写 Effekseer 实时特效并在引擎内渲染验证的 agent skill；验证基于 Godot，但 Effekseer 可接 Unity，做 VFX/TA + AI 工作流的值得试。 |
| 14 | 17.25 | [UpstandPlatform/OpenRive](https://github.com/UpstandPlatform/OpenRive) | github | 51★ / 34天 | Build interactive UIs, motion, and game experiences in the OpenRive Editor, or let an AI agent generate them through the CLI. | 开源 Rive 替代品，亮点是 AI agent 可用 CLI 直接生成动画 UI；游戏 UI 动效管线 + AI 工作流方向值得跟踪。 |
| 15 | 16.5 | [Echo Weaver：Unity 6 时间循环 metroidbrainia](https://unity.com/blog/echo-weaver-time-loop-unity) | Unity Blog | 官方发布 | Moonlight Kids 讲解如何用 Unity 6 构建时间循环玩法。 | 官方案例：时间循环 + 银河城式地图状态管理，做关卡/系统设计的值得看拆解。 |
| 16 | 16.5 | [Backyard Baseball 3D 关卡与 VFX 复盘](https://unity.com/blog/reimagining-backyard-baseball-3d-level-design-and-environment-art) | Unity Blog | 官方发布 | Mega Cat Studios 用可读性关卡设计、decal、灯光和 VFX 把 Backyard Baseball 2026 做成 3D。 | 官方案例：decal + 灯光的可读性关卡设计，对移动端场景表现力有参考价值。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 点评由 LLM 按 profile.json 画像撰写 · 原始榜单见 daily.md*
