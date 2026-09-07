# 游戏开发技术雷达 · 2026-09-06（LLM 点评版 · 补录）

> 基于当日 raw.json 补生成，原始脚本版见同目录 daily.md。

**今日看点**
1. 榜单几乎与昨日同源，但 opdsh/unity-plugin（CLI 控制 Unity Editor）一天从 30★ 涨到 42★，AI 驱动编辑器这条线热度在上行。
2. bot-crossing 持续霸榜（167★/4天），"给 AI 玩的游戏"已成独立赛道，fork 数 58 说明动手玩的人不少。
3. Causa: Into the Dusk 的 Unity 6.3 移动端移植视频，是少见的 PC→手游适配实战分享。

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 125.25 | [jarrenrocks/bot-crossing](https://github.com/jarrenrocks/bot-crossing) | github | 167★ / 4天 | A video game for AI agents. | 给 AI agent 玩的游戏，持续爆发且 fork 高达 58；看它的交互接口设计，对做 agent 评测或游戏 AI 都有启发。 |
| 2 | 50.25 | [GamePhanesStudio/GamePhanes](https://github.com/GamePhanesStudio/GamePhanes) | github | 536★ / 16天 | An open-source game coding agent environment and benchmark for Godot. | "AI 做游戏"的开源评测环境（Godot 向）；作为 AI 编码能力标尺保持趋势级关注。 |
| 3 | 41.12 | [GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-](https://github.com/GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-) | github | 185★ / 36天 | Miku is an open-source material conversion pipeline that translates Blender 5.2 Shader Nodes into editable Unity 6 URP Shader Graph assets. | Blender→URP Shader Graph 材质转换管线，稳步涨星；TA/管线向强烈推荐，材质跨 DCC 的痛点它解得很认真。 |
| 4 | 30.17 | [tettethu/VibeGame](https://github.com/tettethu/VibeGame) | github | 207★ / 24天 | VibeGame: Vibe Your Dream Game -- self-evolving multi-agent framework with an AI-Native game engine. | 自然语言生成 2D 网页游戏的多智能体框架；概念热度还在，看趋势即可。 |
| 5 | 28.0 | [tantaneity/constellation-dice](https://github.com/tantaneity/constellation-dice) | github | 20★ / 5天 | Astral dice in Unity URP: nebula, stars and per-face constellations in a single raymarch pass. | URP 单 pass raymarch 星座骰子；小而美的 shader 练习，写体积渲染的值得拆。 |
| 6 | 27.33 | [Innate-Labs/Noobi.ai](https://github.com/Innate-Labs/Noobi.ai) | github | 246★ / 27天 | Local-first desktop agent that turns a prompt into a reviewed, playable browser game. | 带评审闭环的本地游戏生成 agent；AI 工作流关注者可看它的 review 设计。 |
| 7 | 24.6 | [PudinKiller/VFXMeshLab](https://github.com/PudinKiller/VFXMeshLab) | github | 101★ / 41天 | Unity 6 URP editor tool for procedural VFX mesh authoring, modifiers, diagnostics. | URP 程序化特效网格工具，稳稳破百星；做技能特效的值得 clone 备用。 |
| 8 | 22.0 | [tantaneity/procedural-galaxy](https://github.com/tantaneity/procedural-galaxy) | github | 4★ / 2天 | Raymarched procedural spiral galaxy in Unity URP, 24 ms at 2916x1640. | URP 可飞入的程序化银河；性能不适合手游直接用，但 raymarch 结构可读。 |
| 9 | 21.0 | [opdsh/unity-plugin](https://github.com/opdsh/unity-plugin) | github | 42★ / 8天 | DeepSeek Harness plugin: control the Unity Editor through the unity CLI. | CLI 驱动 Unity Editor 的 agent 插件，日涨 12★ 升温中；AI 工作流重点跟踪对象。 |
| 10 | 21.0 | [gary149/h3-game-sprites](https://github.com/gary149/h3-game-sprites) | github | 114★ / 19天 | Agent Skill: turn AI-generated video into 2D game sprite sheets. | AI 视频转 2D 序列帧的 Agent Skill；2D 美术管线可借鉴的邪道打法。 |
| 11 | 20.43 | [thrixel/build-world](https://github.com/thrixel/build-world) | github | 71★ / 33天 | Build interactive 3D worlds with high-quality assets from Thrixel and your AI agent. | AI agent 搭 3D 世界的素材管线；原型向，趋势级关注。 |
| 12 | 16.5 | [Backyard Baseball 3D 关卡与 VFX 复盘](https://unity.com/blog/reimagining-backyard-baseball-3d-level-design-and-environment-art) | Unity Blog | 官方发布 | Mega Cat Studios 的 3D 化关卡设计、decal、灯光与 VFX。 | 官方美术向案例，decal+灯光的可读性设计对移动端场景有参考价值。 |
| 13 | 16.5 | [权力的游戏 Dragonfire 移动端优化](https://unity.com/blog/building-westeros-for-mobile-in-game-of-thrones-dragonfire) | Unity Blog | 官方发布 | WB Games Boston 的移动端多人与加载优化。 | 大 IP 手游优化复盘，商业手游开发者对口，值得读。 |
| 14 | 15.95 | [nuskey8/SpriteAfterimage](https://github.com/nuskey8/SpriteAfterimage) | github | 29★ / 10天 | High-performance afterimage effect for Unity 2D using GPU instancing. | GPU instancing 的 2D 残影；卡牌/动作游戏打击感的低成本增强件。 |
| 15 | 15.0 | [Sente 六人策略的数据驱动棋盘](https://unity.com/blog/data-driven-board-six-player-strategy-sente) | Unity Blog | 官方发布 | Timeline + 数据驱动棋盘的六人同时回合策略。 | 数据驱动棋盘架构，战棋/卡牌项目可参考。 |
| 16 | 15.0 | [Causa: Into the Dusk 的 Unity 6.3 移动端移植](https://unity.com/blog/adapting-causa-into-the-dusk-for-mobile) | Unity Blog | 官方发布 | Niebla Games 把 2021 年的 PC 游戏搬上 Google Play Pass 的实战视频。 | 少见的 PC→手游移植实战，做移植或多平台的值得看团队踩坑。 |

---
*由 GameDevRadar 生成 · 点评由 LLM 按 profile.json 画像撰写 · 原始榜单见 daily.md*
