# 游戏开发技术雷达 · 2026-09-17（LLM 点评版 · 补录）

> 基于当日 raw.json 补生成，原始脚本版见同目录 daily.md。

**今日看点**
1. agent-isles 两天 75★ 继续霸榜——游戏化学 AI 编码的岛屿世界（DeepSeek Harness 驱动），增速压过 bot-crossing。
2. unity-agent-plugin（Unity 官方 agent 接入插件）273★，单日增 20★，官方 AI 基建关注度加速。
3. AnvilCSG（Hammer 式笔刷关卡设计，支持 URP）保持高位，Unity 关卡白盒工具持续被关注。

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 150.0 | [Qiuner/agent-isles](https://github.com/Qiuner/agent-isles) | github | 75★ / 2天 | An explorable island world to learn AI coding and build real projects with AI agents. Powered by DeepSeek Harness. | 游戏化岛屿世界学 AI 编码（Godot 做壳、DeepSeek Harness 驱动）；把"教 agent 工作流"做成游戏，增速凶猛，趋势级关注。 |
| 2 | 117.21 | [Station-Sciences/bot-crossing](https://github.com/Station-Sciences/bot-crossing) | github | 586★ / 15天 | A video game for AI agents. Created by Jarren Rocks. | 给 AI agent 玩的游戏，15 天 586★；做 AI 工作流/游戏 AI 的值得看它的 agent 交互协议设计。 |
| 3 | 48.0 | [TheGreatSimon/AnvilCSG](https://github.com/TheGreatSimon/AnvilCSG) | github | 16★ / 2天 | Anvil CSG Beta 2: a free legacy preview of brush-based level design inside Unity. Hammer-inspired workflow, Built-in and URP. | Hammer 式笔刷关卡设计工具，Unity 内免费用、支持 URP；做关卡白盒的值得立刻试。 |
| 4 | 26.28 | [GamePhanesStudio/GamePhanes](https://github.com/GamePhanesStudio/GamePhanes) | github | 473★ / 27天 | An open-source game coding agent environment and benchmark for Godot. | "AI 做游戏"的开源评测环境（Godot 向）；作为 AI 编码能力标尺保持趋势级关注。 |
| 5 | 24.19 | [tettethu/VibeGame](https://github.com/tettethu/VibeGame) | github | 242★ / 35天 | VibeGame: Vibe Your Dream Game -- self-evolving multi-agent framework with an AI-Native game engine. | 自然语言生成 2D 网页游戏的多智能体框架；热度延续，看趋势即可。 |
| 6 | 21.0 | [PocketGamer 一周热点汇总](https://www.pocketgamer.biz/hot-five-lego-digital-play-acquires-offroad-games-unity-launches-claude-code-plugin-and-pokemon-go-is-top-grossing-mobile-game-of-early-september/) | PocketGamer.biz | 官方发布 | 过去七天手游行业大事：Unity 官方 Claude Code 插件、乐高收购 Offroad、Pokémon Go 登顶。 | 手游行业周报式汇总，5 分钟补齐一周行业动态。 |
| 7 | 19.98 | [Unity-Technologies/unity-agent-plugin](https://github.com/Unity-Technologies/unity-agent-plugin) | github | 273★ / 41天 | Unity plugin for third-party agent platforms. | Unity 官方出品的第三方 agent 平台接入插件，单日增 20★ 加速；AI agent 接入引擎已是官方战略，做 AI 工作流的必看。 |
| 8 | 19.59 | [Innate-Labs/Noobi.ai](https://github.com/Innate-Labs/Noobi.ai) | github | 248★ / 38天 | Local-first desktop agent that turns a prompt into a reviewed, playable browser game. | 本地优先的游戏生成 agent，亮点是生成后带 review 闭环；关注 AI 工作流的可借鉴其评审设计。 |
| 9 | 16.5 | [Backyard Baseball 3D 关卡与 VFX 复盘](https://unity.com/blog/reimagining-backyard-baseball-3d-level-design-and-environment-art) | Unity Blog | 官方发布 | Mega Cat Studios 用可读性关卡设计、decal、灯光和 VFX 把 Backyard Baseball 2026 做成 3D。 | 官方案例：decal + 灯光的可读性关卡设计，对移动端场景表现力有参考价值。 |
| 10 | 16.5 | [权力的游戏 Dragonfire 移动端优化](https://unity.com/blog/building-westeros-for-mobile-in-game-of-thrones-dragonfire) | Unity Blog | 官方发布 | WB Games Boston 如何为移动端优化可扩展多人、加载速度与 Unity 工具链。 | 大 IP 手游的加载与多人优化复盘，商业手游开发者直接对口，值得读。 |
| 11 | 16.5 | [Unity 发布官方 Claude Code 插件，内置 29 个引擎技能](https://www.pocketgamer.biz/unity-launches-official-claude-code-plugin-with-29-built-in-engine-skills/) | PocketGamer.biz | 官方发布 | Unity has launched an official plugin for Claude Code to bring engine skills to the AI coding tool. | Unity 官方拥抱 AI 编码助手；官方 AI 工具链布局已成型，值得花时间评估接入。 |
| 12 | 15.0 | [Sente 六人策略的数据驱动棋盘](https://unity.com/blog/data-driven-board-six-player-strategy-sente) | Unity Blog | 官方发布 | Oxobox 用 Timeline + 数据驱动棋盘做六人同时回合制策略游戏。 | 数据驱动棋盘 + Timeline 表现层，做战棋/卡牌的架构可参考。 |
| 13 | 15.0 | [Causa: Into the Dusk 的 Unity 6.3 移动端移植](https://unity.com/blog/adapting-causa-into-the-dusk-for-mobile) | Unity Blog | 官方发布 | Niebla Games 把 2021 年的 PC 游戏搬上 Google Play Pass 的实战视频。 | 少见的 PC→手游移植实战分享，做移植或多平台适配的值得看。 |
| 14 | 15.0 | [Playrix 用 D28 IAP ROAS 优化 Township 买量](https://unity.com/blog/playrix-township-roas-optimization-vector) | Unity Blog | 官方发布 | Playrix 用 Vector AI 和 D28 IAP ROAS 优化实现长线留存与营收。 | 头部厂商的长线 ROAS 优化实战，做商业化/买量对接的值得一看。 |

---
*由 GameDevRadar 生成 · 点评由 LLM 按 profile.json 画像撰写 · 原始榜单见 daily.md*
