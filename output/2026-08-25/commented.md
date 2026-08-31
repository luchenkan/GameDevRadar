# 游戏开发技术雷达 · 2026-08-25（LLM 点评版）

> 自动生成于 2026-08-25 10:23 (UTC+8)。评分 = 热度增速 × 个人画像相关度，点评由 LLM 按画像软判断。仅供每日速览, 上榜与否不是质量背书。

**今日看点**

1. VFXMeshLab 星数一夜从 42 涨到 78（疑似被社区推荐）：URP 程序化 VFX 网格工具正在出圈，做特效的今天最该 clone 一试。
2. GamePhanes 三天 234★ 继续霸榜，Miku（Blender→URP 材质转换）稳居前三，「AI 评测 + DCC 管线」是本周两大实线。
3. OpenGameAgent 连续在榜：C# AI NPC 运行时，Unity 可直接集成，AI 工作流从玩具走向可商用底座。

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 117.0 | [GamePhanes/GamePhanes](https://github.com/GamePhanes/GamePhanes) | github | 234★ / 3天 | An open-source game coding agent environment and benchmark f | Godot 游戏编码 Agent 的开源环境 + 基准测试，三天 234★；「AI 写游戏」评测化趋势确立，趋势级必看。 |
| 2 | 102.96 | [RykoTheDev/GodotHub](https://github.com/RykoTheDev/GodotHub) | github | 875★ / 34天 | What if Unity Hub and GitHub Desktop had a Baby but its Adop | Godot 版「Unity Hub + GitHub Desktop」项目管理器；趋势级信号，看个方向即可。 |
| 3 | 41.36 | [GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-](https://github.com/GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-) | github | 124★ / 24天 | Miku is an open-source material conversion pipeline that tra | Blender 5.2 Shader Nodes → Unity 6 URP Shader Graph 的开源转换管线，材质语义不丢；做 URP 的 TA/美术管线强烈建议花 5 分钟。 |
| 4 | 33.5 | [gary149/h3-game-sprites](https://github.com/gary149/h3-game-sprites) | github | 67★ / 7天 | Agent Skill: turn AI-generated video into 2D game sprite she | AI 视频转 2D 序列帧 sprite sheet 的 Agent Skill；做 2D/卡牌表现的值得 5 分钟。 |
| 5 | 30.34 | [tettethu/VibeGame](https://github.com/tettethu/VibeGame) | github | 104★ / 12天 | VibeGame: Vibe Your Dream Game -- An open-source self-evolvi | 自然语言生成可玩 2D 网页游戏的自进化多 Agent 框架；看趋势不看实用。 |
| 6 | 28.5 | [thrixel/build-world](https://github.com/thrixel/build-world) | github | 63★ / 21天 | Build interactive 3D worlds with high-quality assets from Th | 文本/Agent 驱动搭建可交互 3D 世界；原型演示向，看趋势即可。 |
| 7 | 28.5 | [Unity's AI tools in beta: How to get started with MCP](https://unity.com/blog/unity-ai-mcp-how-to-get-started) | Unity Blog | 官方发布 | Unity MCP Server implements the Model Context Protocol to gi | 官方 MCP Server 上手指南，AI Agent 直连运行中的 Unity 工程；没看的补课。 |
| 8 | 26.9 | [PudinKiller/VFXMeshLab](https://github.com/PudinKiller/VFXMeshLab) | github | 78★ / 29天 | Unity 6 URP editor tool for procedural VFX mesh authoring, m | URP 程序化 VFX 网格工具，星数一夜近翻倍（42→78），社区关注度陡增；做技能/卡牌特效的今天最该 clone 试。 |
| 9 | 16.5 | [Making fire feel alive: Real-time fluid simulation in Ignitement](https://unity.com/blog/real-time-fluid-simulation-fire-vfx-ignitement-breakdown) | Unity Blog | 官方发布 | Solo developer Sørb explains how he uses real-time 2D fluid  | 独游开发者拆解 Unity 实时 2D 流体模拟火焰 VFX；玩法驱动特效的思路有参考价值。 |
| 10 | 15.0 | [How Playrix is growing Township with Unity Ads’ D28 IAP ROAS optimizer](https://unity.com/blog/playrix-township-roas-optimization-vector) | Unity Blog | 官方发布 | Discover how Playrix scaled user acquisition for Township us | Playrix 用 D28 IAP ROAS 优化买量的案例；商业手游团队看长线回收模型可参考。 |
| 11 | 15.0 | [Unity 6.3 LTS is now available](https://unity.com/blog/unity-6-3-lts-is-now-available) | Unity Blog | 官方发布 | Unity 6.3 LTS delivers long-term support and a reliable ecos | Unity 6.3 LTS 发布；版本选型必读，升级前先查热更/插件兼容性。 |
| 12 | 14.34 | [Miisan-png/godot-liquid-ui](https://github.com/Miisan-png/godot-liquid-ui) | github | 86★ / 9天 | code only ui design and feel framework for godot | Godot 纯代码 UI 框架；做卡牌 UI 的可借鉴「代码即设计」思路，不值得上手。 |
| 13 | 12.0 | [Unity's AI tools in beta: Create props with the 3D Object Generator](https://unity.com/blog/unity-ai-3d-object-generator) | Unity Blog | 官方发布 | Unity's AI tools in beta includes a 3D Object Generator capa | 官方 AI 生成静态 3D 道具 prefab；只适合占位/原型，了解边界即可。 |
| 14 | 12.0 | [Monster Prom: Building a dialog system for a multiplayer dating sim](https://unity.com/blog/monster-prom-building-a-dialog-system-for-a-dating-sim) | Unity Blog | 官方发布 | Go behind the scenes of the Monster Prom series. Learn how d | Monster Prom 自研编辑器 + 分支对话系统幕后；做剧情/卡牌叙事模块可抄作业。 |
| 15 | 11.98 | [EricSun0218/OpenGameAgent](https://github.com/EricSun0218/OpenGameAgent) | github | 38★ / 27天 | Open-source C# agent runtime for AI NPCs and in-game agents. | 开源 C# Agent 运行时：结构化上下文 + 工具调用 + 记忆 + 多 NPC 调度，Unity 可直接集成的 AI NPC 底座；做 AI 工作流/NPC 的值得认真看。 |
| 16 | 11.15 | [arloopa/UnitySplats](https://github.com/arloopa/UnitySplats) | github | 32★ / 33天 | Cross-platform Unity 6 package for importing, loading, and r | Unity 6 跨平台 Gaussian Splats 包，支持 URP/HDRP、移动端、WebGL；关注新渲染表现值得 5 分钟。 |

---
*由 GameDevRadar 生成（LLM 点评层）· 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
