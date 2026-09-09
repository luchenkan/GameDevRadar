# 游戏开发技术雷达 · 2026-09-09（LLM 点评版）

**今日看点**
1. bot-crossing 易主到 Station-Sciences 组织账号下，一周 277★——"给 AI agent 玩的游戏"从个人项目升级为团队化运作信号。
2. Miku 材质管线（Blender→URP Shader Graph）突破 200★，TA 圈对 DCC→引擎材质转换的需求被持续验证。
3. 同题对比 GPT-6 Astra vs Gemini 3.8 Flash 造 3D 文明模拟的实验仓继续升温，前沿模型 agentic 编码的横向评测开始有人系统做。

![候选评分 Top10](chart-score.png)

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 118.71 | [Station-Sciences/bot-crossing](https://github.com/Station-Sciences/bot-crossing) | github | 277★ / 7天 | A video game for AI agents. Created by Jarren Rocks. | 给 AI agent 玩的游戏，迁入组织账号、一周 277★；它的 agent 交互协议值得做 AI 工作流/游戏 AI 的人花 5 分钟研究。 |
| 2 | 41.2 | [GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-](https://github.com/GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-) | github | 201★ / 39天 | Miku is an open-source material conversion pipeline that translates Blender 5.2 Shader Nodes into editable Unity 6 URP Shader Graph assets. | Blender Shader Nodes 转 URP Shader Graph 的材质管线，破 200★；TA/管线向强烈推荐，DCC 到引擎的材质断层它真在补。 |
| 3 | 39.66 | [GamePhanesStudio/GamePhanes](https://github.com/GamePhanesStudio/GamePhanes) | github | 476★ / 18天 | An open-source game coding agent environment and benchmark for Godot. | "AI 做游戏"的开源评测环境（Godot 向）；作为 AI 编码能力标尺保持趋势级关注。 |
| 4 | 35.75 | [cagrikacmaz/gpt-6-astra-vs-gemini-3-8-flash](https://github.com/cagrikacmaz/gpt-6-astra-vs-gemini-3-8-flash) | github | 13★ / 2天 | Same-brief agentic build comparison: GPT-6 Astra High vs Gemini 3.8 Flash High building a playable 3D AI civilization simulation game. | 同题对比两大前沿模型 agentic 造 3D 文明模拟游戏；星少但形式新颖，关注 AI 编码工作流的值得看过程差异。 |
| 5 | 28.25 | [tettethu/VibeGame](https://github.com/tettethu/VibeGame) | github | 218★ / 27天 | VibeGame: Vibe Your Dream Game -- self-evolving multi-agent framework with an AI-Native game engine. | 自然语言生成 2D 网页游戏的多智能体框架；热度延续，看趋势即可，离商业手游管线尚远。 |
| 6 | 24.69 | [Innate-Labs/Noobi.ai](https://github.com/Innate-Labs/Noobi.ai) | github | 247★ / 30天 | Local-first desktop agent that turns a prompt into a reviewed, playable browser game. | 本地优先的游戏生成 agent，亮点是生成后带 review 闭环；关注 AI 工作流的可借鉴其评审设计。 |
| 7 | 23.0 | [PudinKiller/VFXMeshLab](https://github.com/PudinKiller/VFXMeshLab) | github | 101★ / 44天 | Unity 6 URP editor tool for procedural VFX mesh authoring, modifiers, diagnostics. | URP 程序化特效网格编辑工具，带修改器和引用安全更新；做技能特效的直接 clone 备用。 |
| 8 | 19.28 | [opdsh/unity-plugin](https://github.com/opdsh/unity-plugin) | github | 53★ / 11天 | DeepSeek Harness plugin: control the Unity Editor through the unity CLI. | 通过 CLI 让 AI 操控 Unity Editor 的插件；增速放缓但方向成立，AI 驱动编辑器工作流继续跟踪。 |
| 9 | 19.0 | [thrixel/build-world](https://github.com/thrixel/build-world) | github | 72★ / 36天 | Build interactive 3D worlds with high-quality assets from Thrixel and your AI agent. | AI agent + 素材库搭 3D 世界（three.js 向）；原型阶段工具，趋势级关注。 |
| 10 | 18.3 | [gary149/h3-game-sprites](https://github.com/gary149/h3-game-sprites) | github | 115★ / 22天 | Agent Skill: turn AI-generated video into 2D game sprite sheets (the Mortal Kombat method). | AI 视频转 2D 序列帧的 Agent Skill；2D 美术管线可借鉴的邪道打法，AI 工作流关注者必看。 |
| 11 | 17.5 | [tantaneity/constellation-dice](https://github.com/tantaneity/constellation-dice) | github | 20★ / 8天 | Astral dice in Unity URP: nebula, stars and per-face constellations in a single raymarch pass. | URP 单 raymarch pass 的星座骰子；小而美、技术密度高，写体积 shader 的值得拆。 |
| 12 | 16.5 | [Backyard Baseball 3D 关卡与 VFX 复盘](https://unity.com/blog/reimagining-backyard-baseball-3d-level-design-and-environment-art) | Unity Blog | 官方发布 | Mega Cat Studios 用可读性关卡设计、decal、灯光和 VFX 把 Backyard Baseball 2026 做成 3D。 | 官方案例：decal + 灯光的可读性关卡设计，对移动端场景表现力有参考价值。 |
| 13 | 16.5 | [权力的游戏 Dragonfire 移动端优化](https://unity.com/blog/building-westeros-for-mobile-in-game-of-thrones-dragonfire) | Unity Blog | 官方发布 | WB Games Boston 如何为移动端优化可扩展多人、加载速度与 Unity 工具链。 | 大 IP 手游的加载与多人优化复盘，商业手游开发者直接对口，值得读。 |
| 14 | 15.0 | [Sente 六人策略的数据驱动棋盘](https://unity.com/blog/data-driven-board-six-player-strategy-sente) | Unity Blog | 官方发布 | Oxobox 用 Timeline + 数据驱动棋盘做六人同时回合制策略游戏。 | 数据驱动棋盘 + Timeline 表现层，做战棋/卡牌的架构可参考。 |
| 15 | 15.0 | [Causa: Into the Dusk 的 Unity 6.3 移动端移植](https://unity.com/blog/adapting-causa-into-the-dusk-for-mobile) | Unity Blog | 官方发布 | Niebla Games 把 2021 年的 PC 游戏搬上 Google Play Pass 的实战视频。 | 少见的 PC→手游移植实战分享，做移植或多平台适配的值得看。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 点评由 LLM 按 profile.json 画像撰写 · 原始榜单见 daily.md*
