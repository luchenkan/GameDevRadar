# 游戏开发技术雷达 · 2026-08-22（LLM 点评版 · 周末补录）

> 原始数据生成于 2026-08-22 10:04 (UTC+8)（GitHub Actions 云端运行），点评于 08-24 补写。评分 = 热度增速 × 个人画像相关度。仅供每日速览, 上榜与否不是质量背书。

**今日看点**

1. 新面孔 GamePhanes 空降榜首：开源 Godot 游戏编码 Agent 环境 + 基准测试，一天 92★，「Agent 做游戏」开始有了评测标准。
2. AI 生成管线持续霸榜：h3-game-sprites（视频转序列帧）、build-world（文本生成 3D 世界）、VibeGame（多 Agent 生成网页游戏）占据前排。
3. Unity 实用钉子户照常在线：URP 特效网格工具 VFXMeshLab、Gaussian Splats 包 UnitySplats。

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 138.0 | [GamePhanes/GamePhanes](https://github.com/GamePhanes/GamePhanes) | github | 92★ / 1天 | An open-source game coding agent environment and benchmark f | Godot 游戏编码 Agent 的开源环境 + 基准测试；「AI 写游戏」开始卷评测了，趋势级必看，Unity 侧可对照官方 MCP 路线。 |
| 2 | 112.12 | [RykoTheDev/GodotHub](https://github.com/RykoTheDev/GodotHub) | github | 869★ / 31天 | What if Unity Hub and GitHub Desktop had a Baby but its Adop | Godot 版「Unity Hub + GitHub Desktop」项目管理器；趋势级信号，看个方向即可。 |
| 3 | 41.12 | [gary149/h3-game-sprites](https://github.com/gary149/h3-game-sprites) | github | 47★ / 4天 | Agent Skill: turn AI-generated video into 2D game sprite she | AI 视频转 2D 序列帧 sprite sheet 的 Agent Skill；连续高热，做 2D/卡牌表现的值得 5 分钟。 |
| 4 | 30.59 | [thrixel/build-world](https://github.com/thrixel/build-world) | github | 58★ / 18天 | Build interactive 3D worlds with high-quality assets from Th | 文本/Agent 驱动搭建可交互 3D 世界；原型演示向，看趋势即可。 |
| 5 | 28.5 | [Unity's AI tools in beta: How to get started with MCP](https://unity.com/blog/unity-ai-mcp-how-to-get-started) | Unity Blog | 官方发布 | Unity MCP Server implements the Model Context Protocol to gi | 官方 MCP Server 上手指南，AI Agent 直连运行中的 Unity 工程；没看的补课。 |
| 6 | 26.46 | [tettethu/VibeGame](https://github.com/tettethu/VibeGame) | github | 68★ / 9天 | VibeGame: Vibe Your Dream Game -- An open-source self-evolvi | 自然语言生成可玩 2D 网页游戏的自进化多 Agent 框架；看趋势不看实用。 |
| 7 | 20.25 | [Miisan-png/godot-liquid-ui](https://github.com/Miisan-png/godot-liquid-ui) | github | 81★ / 6天 | code only ui design and feel framework for godot | Godot 纯代码 UI 框架；做卡牌 UI 的可借鉴「代码即设计」思路，不值得上手。 |
| 8 | 16.5 | [Making fire feel alive: Real-time fluid simulation in Ignitement](https://unity.com/blog/real-time-fluid-simulation-fire-vfx-ignitement-breakdown) | Unity Blog | 官方发布 | Solo developer Sørb explains how he uses real-time 2D fluid  | 独游开发者拆解 Unity 实时 2D 流体模拟火焰 VFX；玩法驱动特效的思路有参考价值。 |
| 9 | 15.39 | [PudinKiller/VFXMeshLab](https://github.com/PudinKiller/VFXMeshLab) | github | 42★ / 26天 | Editor-only procedural VFX mesh generator for Unity 6+ URP. | Unity 6+ URP 编辑器内程序化 VFX 网格生成器；做技能/卡牌特效的值得 clone 试。 |
| 10 | 15.0 | [yurne91/Godot-Secure-Build-Pipeline](https://github.com/yurne91/Godot-Secure-Build-Pipeline) | github | 24★ / 4天 | Build a custom Godot 4.7.1 editor and matching export templa | Godot 自定义编辑器 + 导出模板的安全构建管线；对应 Unity 侧 il2cpp 加固那条线，趋势级参考。 |
| 11 | 15.0 | [How Playrix is growing Township with Unity Ads’ D28 IAP ROAS optimizer](https://unity.com/blog/playrix-township-roas-optimization-vector) | Unity Blog | 官方发布 | Discover how Playrix scaled user acquisition for Township us | Playrix 用 D28 IAP ROAS 优化买量的案例；商业手游团队看长线回收模型可参考。 |
| 12 | 15.0 | [Unity 6.3 LTS is now available](https://unity.com/blog/unity-6-3-lts-is-now-available) | Unity Blog | 官方发布 | Unity 6.3 LTS delivers long-term support and a reliable ecos | Unity 6.3 LTS 发布；版本选型必读，升级前先查热更/插件兼容性。 |
| 13 | 12.0 | [karminski/VibeGamer](https://github.com/karminski/VibeGamer) | github | 128★ / 32天 | 用 AI Agent 自动游玩《Turmoil》（石油大亨）并不断积累经验进化的实验项目. | AI Agent 自动玩经营游戏并自我迭代；「Agent 记忆积累」设计对 NPC/AI 工作流有启发。 |
| 14 | 12.0 | [Unity's AI tools in beta: Create props with the 3D Object Generator](https://unity.com/blog/unity-ai-3d-object-generator) | Unity Blog | 官方发布 | Unity's AI tools in beta includes a 3D Object Generator capa | 官方 AI 生成静态 3D 道具 prefab；只适合占位/原型，了解边界即可。 |
| 15 | 12.0 | [Monster Prom: Building a dialog system for a multiplayer dating sim](https://unity.com/blog/monster-prom-building-a-dialog-system-for-a-dating-sim) | Unity Blog | 官方发布 | Go behind the scenes of the Monster Prom series. Learn how d | Monster Prom 自研编辑器 + 分支对话系统幕后；做剧情/卡牌叙事模块可抄作业。 |
| 16 | 11.85 | [arloopa/UnitySplats](https://github.com/arloopa/UnitySplats) | github | 31★ / 30天 | Cross-platform Unity 6 package for importing, loading, and r | Unity 6 跨平台 Gaussian Splats 包，支持 URP/HDRP、移动端、WebGL；关注新渲染表现值得 5 分钟。 |

---
*由 GameDevRadar 生成（LLM 点评层）· 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
