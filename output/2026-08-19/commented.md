# 游戏开发技术雷达 · 2026-08-19（LLM 点评版）

> 自动生成于 2026-08-19 10:24 (UTC+8)。评分 = 热度增速 × 个人画像相关度，点评由 LLM 按画像软判断。仅供每日速览, 上榜与否不是质量背书。

**今日看点**

1. 「AI 视频转 2D 序列帧」的 h3-game-sprites 一天冲到榜二（31★/天级增速），AI 资产管线这条线正在快速升温。
2. Unity 官方 MCP 上手指南持续在榜，配合 godot-mcp-go，引擎「Agent 直连工程」已成跨引擎共识方向。
3. 实用工具侧看两个：URP 程序化 VFX 网格生成器 VFXMeshLab 和 Unity 6 跨平台 Gaussian Splats 包 UnitySplats，都可直接上手试。

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 122.28 | [RykoTheDev/GodotHub](https://github.com/RykoTheDev/GodotHub) | github | 856★ / 28天 | What if Unity Hub and GitHub Desktop had a Baby but its Adop | Godot 版「Unity Hub + GitHub Desktop」项目管理器；趋势级信号——Godot 工具链在补齐工程管理短板，Unity 开发者看个方向即可。 |
| 2 | 108.5 | [gary149/h3-game-sprites](https://github.com/gary149/h3-game-sprites) | github | 31★ / 1天 | Agent Skill: turn AI-generated video into 2D game sprite she | 把 AI 生成视频转成 2D 序列帧 sprite sheet 的 Agent Skill（真人快打式做法）；增速翻倍，AI 资产管线的新玩法，做 2D/卡牌表现的值得 5 分钟。 |
| 3 | 39.0 | [Miisan-png/godot-liquid-ui](https://github.com/Miisan-png/godot-liquid-ui) | github | 78★ / 3天 | code only ui design and feel framework for godot | Godot 纯代码 UI 框架；做卡牌 UI 的可以借鉴其「代码即设计」思路，但不值得上手。 |
| 4 | 28.5 | [Unity's AI tools in beta: How to get started with MCP](https://unity.com/blog/unity-ai-mcp-how-to-get-started) | Unity Blog | 官方发布 | Unity MCP Server implements the Model Context Protocol to gi | 官方 MCP Server 上手指南，AI Agent 直接操作运行中的 Unity 工程；还没看的话今天最该补的一篇。 |
| 5 | 26.47 | [thrixel/goal-to-game](https://github.com/thrixel/goal-to-game) | github | 53★ / 15天 | Build beautiful games with high-quality 3D assets using Thri | text-to-3D 资产 + Claude Code 快速原型的工作流模板；偏演示性质，原型期可参考，商用管线别当真。 |
| 6 | 18.66 | [tettethu/VibeGame](https://github.com/tettethu/VibeGame) | github | 32★ / 6天 | VibeGame: Vibe Your Dream Game -- An open-source self-evolvi | 自然语言生成可玩 2D 网页游戏的自进化多 Agent 框架；「AI 原生引擎」的早期样本，看趋势不看实用。 |
| 7 | 16.91 | [PudinKiller/VFXMeshLab](https://github.com/PudinKiller/VFXMeshLab) | github | 41★ / 23天 | Editor-only procedural VFX mesh generator for Unity 6+ URP. | Unity 6+ URP 编辑器内程序化 VFX 网格生成器；正中 URP 特效管线，做技能/卡牌特效的值得 clone 下来试。 |
| 8 | 16.5 | [Making fire feel alive: Real-time fluid simulation in Ignitement](https://unity.com/blog/real-time-fluid-simulation-fire-vfx-ignitement-breakdown) | Unity Blog | 官方发布 | Solo developer Sørb explains how he uses real-time 2D fluid  | 独游开发者拆解 Unity 实时 2D 流体模拟做交互火焰 VFX；玩法驱动特效的思路对技能表现有直接参考价值。 |
| 9 | 15.0 | [How Playrix is growing Township with Unity Ads’ D28 IAP ROAS optimizer](https://unity.com/blog/playrix-township-roas-optimization-vector) | Unity Blog | 官方发布 | Discover how Playrix scaled user acquisition for Township us | Playrix 用 D28 IAP ROAS 优化买量的案例；技术含量为零，但商业手游团队看长线回收模型可以参考。 |
| 10 | 15.0 | [Unity 6.3 LTS is now available](https://unity.com/blog/unity-6-3-lts-is-now-available) | Unity Blog | 官方发布 | Unity 6.3 LTS delivers long-term support and a reliable ecos | Unity 6.3 LTS 正式发布，性能与 QoL 改进；商业项目版本选型必读，升级前先查热更/插件兼容性。 |
| 11 | 13.14 | [karminski/VibeGamer](https://github.com/karminski/VibeGamer) | github | 127★ / 29天 | 用 AI Agent 自动游玩《Turmoil》（石油大亨）并不断积累经验进化的实验项目. | AI Agent 自动玩经营游戏并自我迭代的实验；不是开发工具，但「Agent 积累经验」的记忆设计对 NPC/AI 工作流有启发。 |
| 12 | 12.77 | [arloopa/UnitySplats](https://github.com/arloopa/UnitySplats) | github | 30★ / 27天 | Cross-platform Unity 6 package for importing, loading, and r | Unity 6 跨平台 Gaussian Splats 导入渲染包，支持 URP/HDRP、移动端和 WebGL；关注新渲染表现的值得 5 分钟。 |
| 13 | 12.0 | [Unity's AI tools in beta: Create props with the 3D Object Generator](https://unity.com/blog/unity-ai-3d-object-generator) | Unity Blog | 官方发布 | Unity's AI tools in beta includes a 3D Object Generator capa | 官方 AI 生成静态 3D 道具 prefab；目前只适合占位/原型，商用品质别指望，了解边界即可。 |
| 14 | 12.0 | [Monster Prom: Building a dialog system for a multiplayer dating sim](https://unity.com/blog/monster-prom-building-a-dialog-system-for-a-dating-sim) | Unity Blog | 官方发布 | Go behind the scenes of the Monster Prom series. Learn how d | Monster Prom 团队自研编辑器 + 分支对话系统的幕后；做剧情/卡牌叙事模块的可以抄作业。 |
| 15 | 11.7 | [regiellis/godot-mcp-go](https://github.com/regiellis/godot-mcp-go) | github | 42★ / 27天 | Give AI agents the complete Godot development loop: discover | Godot 版完整 MCP 开发闭环（构建-运行-观察-调试-修复）；和 Unity MCP 对照着看，就知道引擎 AI 工作流在往哪走。 |

---
*由 GameDevRadar 生成（LLM 点评层）· 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
