# 游戏开发技术雷达 · 2026-08-23（LLM 点评版 · 周末补录）

> 原始数据生成于 2026-08-23 10:13 (UTC+8)（GitHub Actions 云端运行），点评于 08-24 补写。评分 = 热度增速 × 个人画像相关度。仅供每日速览, 上榜与否不是质量背书。

**今日看点**

1. GamePhanes 继续霸榜且增速加快（107★/天级）：Godot 游戏编码 Agent 基准环境成了本周末最热项目。
2. 周末榜单被 AI 工作流屠版：序列帧生成、3D 世界生成、多 Agent 游戏生成、MCP 直连引擎，五个前排全是这一主题。
3. Unity 侧无重大新消息，6.3 LTS 与官方 AI 工具链（MCP / 3D 生成器）维持热度。

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 160.5 | [GamePhanes/GamePhanes](https://github.com/GamePhanes/GamePhanes) | github | 107★ / 1天 | An open-source game coding agent environment and benchmark f | Godot 游戏编码 Agent 的开源环境 + 基准测试，增速还在加快；「AI 写游戏」进入评测时代，趋势级必看。 |
| 2 | 109.12 | [RykoTheDev/GodotHub](https://github.com/RykoTheDev/GodotHub) | github | 873★ / 32天 | What if Unity Hub and GitHub Desktop had a Baby but its Adop | Godot 版「Unity Hub + GitHub Desktop」项目管理器；趋势级信号，看个方向即可。 |
| 3 | 35.0 | [gary149/h3-game-sprites](https://github.com/gary149/h3-game-sprites) | github | 50★ / 5天 | Agent Skill: turn AI-generated video into 2D game sprite she | AI 视频转 2D 序列帧 sprite sheet 的 Agent Skill；做 2D/卡牌表现的值得 5 分钟。 |
| 4 | 30.5 | [thrixel/build-world](https://github.com/thrixel/build-world) | github | 61★ / 19天 | Build interactive 3D worlds with high-quality assets from Th | 文本/Agent 驱动搭建可交互 3D 世界；原型演示向，看趋势即可。 |
| 5 | 28.5 | [Unity's AI tools in beta: How to get started with MCP](https://unity.com/blog/unity-ai-mcp-how-to-get-started) | Unity Blog | 官方发布 | Unity MCP Server implements the Model Context Protocol to gi | 官方 MCP Server 上手指南，AI Agent 直连运行中的 Unity 工程；没看的补课。 |
| 6 | 26.95 | [tettethu/VibeGame](https://github.com/tettethu/VibeGame) | github | 77★ / 10天 | VibeGame: Vibe Your Dream Game -- An open-source self-evolvi | 自然语言生成可玩 2D 网页游戏的自进化多 Agent 框架；看趋势不看实用。 |
| 7 | 17.57 | [Miisan-png/godot-liquid-ui](https://github.com/Miisan-png/godot-liquid-ui) | github | 82★ / 7天 | code only ui design and feel framework for godot | Godot 纯代码 UI 框架；做卡牌 UI 的可借鉴「代码即设计」思路，不值得上手。 |
| 8 | 16.5 | [Making fire feel alive: Real-time fluid simulation in Ignitement](https://unity.com/blog/real-time-fluid-simulation-fire-vfx-ignitement-breakdown) | Unity Blog | 官方发布 | Solo developer Sørb explains how he uses real-time 2D fluid  | 独游开发者拆解 Unity 实时 2D 流体模拟火焰 VFX；玩法驱动特效的思路有参考价值。 |
| 9 | 15.0 | [How Playrix is growing Township with Unity Ads’ D28 IAP ROAS optimizer](https://unity.com/blog/playrix-township-roas-optimization-vector) | Unity Blog | 官方发布 | Discover how Playrix scaled user acquisition for Township us | Playrix 用 D28 IAP ROAS 优化买量的案例；商业手游团队看长线回收模型可参考。 |
| 10 | 15.0 | [Unity 6.3 LTS is now available](https://unity.com/blog/unity-6-3-lts-is-now-available) | Unity Blog | 官方发布 | Unity 6.3 LTS delivers long-term support and a reliable ecos | Unity 6.3 LTS 发布；版本选型必读，升级前先查热更/插件兼容性。 |
| 11 | 14.82 | [PudinKiller/VFXMeshLab](https://github.com/PudinKiller/VFXMeshLab) | github | 42★ / 27天 | Editor-only procedural VFX mesh generator for Unity 6+ URP. | Unity 6+ URP 编辑器内程序化 VFX 网格生成器；做技能/卡牌特效的值得 clone 试。 |
| 12 | 12.0 | [yurne91/Godot-Secure-Build-Pipeline](https://github.com/yurne91/Godot-Secure-Build-Pipeline) | github | 24★ / 5天 | Build a custom Godot 4.7.1 editor and matching export templa | Godot 自定义编辑器 + 导出模板的安全构建管线；对应 Unity 侧 il2cpp 加固那条线，趋势级参考。 |
| 13 | 12.0 | [Unity's AI tools in beta: Create props with the 3D Object Generator](https://unity.com/blog/unity-ai-3d-object-generator) | Unity Blog | 官方发布 | Unity's AI tools in beta includes a 3D Object Generator capa | 官方 AI 生成静态 3D 道具 prefab；只适合占位/原型，了解边界即可。 |
| 14 | 12.0 | [Monster Prom: Building a dialog system for a multiplayer dating sim](https://unity.com/blog/monster-prom-building-a-dialog-system-for-a-dating-sim) | Unity Blog | 官方发布 | Go behind the scenes of the Monster Prom series. Learn how d | Monster Prom 自研编辑器 + 分支对话系统幕后；做剧情/卡牌叙事模块可抄作业。 |
| 15 | 11.85 | [arloopa/UnitySplats](https://github.com/arloopa/UnitySplats) | github | 32★ / 31天 | Cross-platform Unity 6 package for importing, loading, and r | Unity 6 跨平台 Gaussian Splats 包，支持 URP/HDRP、移动端、WebGL；关注新渲染表现值得 5 分钟。 |
| 16 | 11.64 | [karminski/VibeGamer](https://github.com/karminski/VibeGamer) | github | 128★ / 33天 | 用 AI Agent 自动游玩《Turmoil》（石油大亨）并不断积累经验进化的实验项目. | AI Agent 自动玩经营游戏并自我迭代；「Agent 记忆积累」设计对 NPC/AI 工作流有启发。 |

---
*由 GameDevRadar 生成（LLM 点评层）· 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
