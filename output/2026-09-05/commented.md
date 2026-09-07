# 游戏开发技术雷达 · 2026-09-05（LLM 点评版 · 补录）

> 基于当日 raw.json 补生成，原始脚本版见同目录 daily.md。

**今日看点**
1. bot-crossing 三天冲到 156★——"给 AI agent 玩的游戏"这个新品类正在爆发，做 AI 工作流的人值得看它怎么定义 agent 交互接口。
2. tantaneity 连发两个 URP raymarching 小项目（星座骰子 + 程序化银河），单 pass 体积渲染的技术密度很高，写 shader 的直接抄思路。
3. 新面孔 opdsh/unity-plugin：通过 CLI 控制 Unity Editor 的 DeepSeek Harness 插件，AI 直接操作编辑器这条线又多一个实现。

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 156.0 | [jarrenrocks/bot-crossing](https://github.com/jarrenrocks/bot-crossing) | github | 156★ / 3天 | A video game for AI agents. | 专为 AI agent 设计的游戏，三天 156★ 爆发中；值得花 5 分钟看它的 agent 交互协议，"AI 玩游戏"是评测和游戏 AI 的新基建。 |
| 2 | 53.59 | [GamePhanesStudio/GamePhanes](https://github.com/GamePhanesStudio/GamePhanes) | github | 536★ / 15天 | An open-source game coding agent environment and benchmark for Godot. | 开源的"AI 做游戏"评测环境与 benchmark（基于 Godot）；虽是 Godot 向，但作为 AI 编码能力标尺必看，趋势级关注。 |
| 3 | 41.6 | [GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-](https://github.com/GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-) | github | 182★ / 35天 | Miku is an open-source material conversion pipeline that translates Blender 5.2 Shader Nodes into editable Unity 6 URP Shader Graph assets. | Blender Shader Nodes 转 Unity 6 URP Shader Graph 的材质管线，带中间表示和诊断；做 TA/管线的强烈推荐，DCC 到引擎的材质断层它真在补。 |
| 4 | 35.0 | [tantaneity/constellation-dice](https://github.com/tantaneity/constellation-dice) | github | 20★ / 4天 | Astral dice in Unity URP: nebula, stars and per-face constellations in a single raymarch pass on the default cube. | URP 单 raymarch pass 在默认 cube 上画出星云+逐面星座的骰子；技术密度高的小品，写体积 shader 的值得拆。 |
| 5 | 33.0 | [tantaneity/procedural-galaxy](https://github.com/tantaneity/procedural-galaxy) | github | 3★ / 1天 | Raymarched procedural spiral galaxy in Unity URP. Single volume pass you can fly into, 24 ms at 2916x1640 from inside the disk. | 同一作者的 URP 程序化螺旋银河，可飞入的单体积 pass；24ms 偏高不适合手游直接上，但 raymarch 结构值得读。 |
| 6 | 30.45 | [tettethu/VibeGame](https://github.com/tettethu/VibeGame) | github | 200★ / 23天 | VibeGame: Vibe Your Dream Game -- An open-source self-evolving multi-agent framework with an AI-Native game engine. | 基于 Claude Code/Codex 的多智能体"自然语言生成 2D 网页游戏"框架；自进化概念新，看趋势即可，离商业手游管线还远。 |
| 7 | 28.38 | [Innate-Labs/Noobi.ai](https://github.com/Innate-Labs/Noobi.ai) | github | 246★ / 26天 | Local-first desktop agent that turns a prompt into a reviewed, playable browser game with Codex App Server. | 本地优先的游戏生成 agent，亮点是生成后带 review 环节；关注 AI 工作流的可以看它的评审闭环怎么设计。 |
| 8 | 25.0 | [PudinKiller/VFXMeshLab](https://github.com/PudinKiller/VFXMeshLab) | github | 100★ / 40天 | Unity 6 URP editor tool for procedural VFX mesh authoring, modifiers, diagnostics, and safe reference-preserving updates. | Unity 6 URP 程序化特效网格编辑器工具，带修改器和引用安全更新；做技能特效的直接 clone，省掉手写特效 mesh 的时间。 |
| 9 | 21.98 | [gary149/h3-game-sprites](https://github.com/gary149/h3-game-sprites) | github | 113★ / 18天 | Agent Skill: turn AI-generated video into 2D game sprite sheets (the Mortal Kombat method, with MiniMax H3 as the actor). | 把 AI 生成视频切成 2D 序列帧的 Agent Skill（真人快打式管线）；2D 项目美术管线可借鉴，AI 工作流关注者必看。 |
| 10 | 21.09 | [thrixel/build-world](https://github.com/thrixel/build-world) | github | 71★ / 32天 | Build interactive 3D worlds with high-quality assets from Thrixel and your AI agent of choice. | AI agent + 高质量素材库搭 3D 世界（three.js 向，topics 带 unity）；原型阶段可试，趋势级关注。 |
| 11 | 17.71 | [nuskey8/SpriteAfterimage](https://github.com/nuskey8/SpriteAfterimage) | github | 29★ / 9天 | High-performance afterimage effect for Unity 2D using GPU instancing. | GPU instancing 实现的 Unity 2D 残影效果；卡牌/动作手游做打击感的低成本方案，值得 5 分钟。 |
| 12 | 17.16 | [opdsh/unity-plugin](https://github.com/opdsh/unity-plugin) | github | 30★ / 7天 | DeepSeek Harness plugin: control the Unity Editor through the unity CLI. | 通过 CLI 让 DeepSeek Harness 操控 Unity Editor 的插件；AI 直接驱动编辑器是热方向，Unity 开发者值得跟踪这类自动化接口。 |
| 13 | 16.5 | [Backyard Baseball 3D 关卡与 VFX 复盘](https://unity.com/blog/reimagining-backyard-baseball-3d-level-design-and-environment-art) | Unity Blog | 官方发布 | Mega Cat Studios 用可读性关卡设计、decal、灯光和 VFX 把 Backyard Baseball 2026 做成 3D。 | 官方案例：decal + 灯光的可读性关卡设计思路，对移动端场景表现力有参考价值。 |
| 14 | 16.5 | [权力的游戏 Dragonfire 移动端优化](https://unity.com/blog/building-westeros-for-mobile-in-game-of-thrones-dragonfire) | Unity Blog | 官方发布 | WB Games Boston 如何为移动端优化可扩展多人、加载速度与 Unity 工具链。 | 大 IP 手游的加载与多人优化复盘，商业手游开发者直接对口，值得读。 |
| 15 | 15.0 | [Sente 六人策略的数据驱动棋盘](https://unity.com/blog/data-driven-board-six-player-strategy-sente) | Unity Blog | 官方发布 | Oxobox 用 Timeline + 数据驱动棋盘做六人同时回合制策略游戏。 | 数据驱动棋盘 + Timeline 驱动表现层，做战棋/卡牌的架构可参考。 |

---
*由 GameDevRadar 生成 · 点评由 LLM 按 profile.json 画像撰写 · 原始榜单见 daily.md*
