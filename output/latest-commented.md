# 游戏开发技术雷达 · 2026-11-24（LLM 点评版）

> 采集于 2026-11-24 10:23 (UTC+8)。reddit/Unity 论坛照常 403，已跳过，不影响主榜。

**今日看点**

1. **CyberAgent 官方《Unity Performance Tuning Tips 2026》上榜（57★/19天）**——一线商业手游厂商的图形/脚本/动画/构建/性能分析实战，含简中，正中性能画像，今日最佳必读。
2. **blender-skills 2792★ 逼近 2800**；同一作者 EL4CTEO 再出 godot-skills（46★/4天），skill 化工作流双线布局成型。
3. **刷量修改器家族退潮**（仅剩两个换号仓残留）；nier-reincarnation-pc 非官方商业手游移植上榜即剔除。

![候选评分 Top10](chart-score.png)

| 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|
| 767.8 | [EL4CTEO/blender-skills](https://github.com/EL4CTEO/blender-skills) | github | 2792★ / 40天 | Professional Blender 5.3 workflows as Agent Skills for Claud | 把 Blender 专业工作流封装成 agent skill，逼近 2800★；DCC 管线 AI 化头号样板，TA/资产管线必看。 |
| 289.35 | [cranebeaker/coop-anything](https://github.com/cranebeaker/coop-anything) | github | 2199★ / 38天 | Co-op Anything: point your AI agent at any single-player gam | AI 逆向单机游戏加联机的 MCP，横盘第八日；存量玩法改造思路启发极大，商用注意版权边界。 |
| 137.38 | [op7418/guizang-game-skill](https://github.com/op7418/guizang-game-skill) | github | 471★ / 12天 | 歸藏 Game Skill：让 AI 从零做出能在浏览器里玩的 3D 动作游戏，附可玩演示《雾锁山门》。Agent sk | 歸藏出品的 agent skill：three.js 单 HTML + Tripo AI 素材直出可玩 3D 动作游戏；"从零到可玩"的 skill 编排是 AI 工作流最强样板，必拆。 |
| 98.56 | [brenpoly/CardboardXR](https://github.com/brenpoly/CardboardXR) | github | 352★ / 25天 | Mobile VR with 6DoF head tracking, passthrough and hand trac | 用 ARCore/ARKit 给手机 VR 补 6DoF/透视/手势的 Unity 插件；做 XR 玩法验证不用再等 Quest。 |
| 71.48 | [jaisurya-dev-art/rigby2d](https://github.com/jaisurya-dev-art/rigby2d) | github | 345★ / 41天 | Open-source 2D skeletal animation editor for games and film. | 开源 Spine 路线的 2D 骨骼动画编辑器，内建 MCP 可被 AI 驱动；稳涨，暂缺 Unity runtime 是硬伤，继续观察。 |
| 71.25 | [rtmandreyev/construct](https://github.com/rtmandreyev/construct) | github | 171★ / 30天 | Persistent, editable objects for AI agents: researched part  | 给 AI agent 的持久可编辑对象系统：逐部件研究、来源溯源，可带入游戏/影视/CAD/3D 打印；agent 基础设施新思路，TA/程序化管线值得看。 |
| 63.7 | [ahwootech/BRUTAL](https://github.com/ahwootech/BRUTAL) | github | 2548★ / 40天 | A free, open source C# framework for making games, or your o | RimWorld 新作引擎底座的 C# 底层框架（自带 Vulkan 渲染）；.NET 底座价值持续兑现，架构思路值得跟。 |
| 63.25 | [EL4CTEO/godot-skills](https://github.com/EL4CTEO/godot-skills) | github | 46★ / 4天 | Godot 4.7 Agent Skills for AI coding agents, based on Godot  | blender-skills 作者的 Godot 版：精确 API 查询 + 安全场景编辑 + 编译检查；引擎不对口但作者实力已验证，趋势级关注其 skill 工程方法。 |
| 58.96 | [LucasJRamos/spritekiln](https://github.com/LucasJRamos/spritekiln) | github | 177★ / 33天 | Animated pixel-art sprites (8-direction top-down or side vie | YAML 写程序化 3D 模型、烘出 8 方向像素精灵动画，专为 coding AI 设计且原生导出 Unity；做像素风卡牌/SLG 的强推。 |
| 48.12 | [Zengkailin/parametric-topdown-pixel-architecture](https://github.com/Zengkailin/parametric-topdown-pixel-architecture) | github | 245★ / 28天 | 参数化2D topdown像素建筑 - 把参数化 3D 建筑渲染成 2D 俯视像素图的本地网页工具（Godot hd2d | 参数化 3D 建筑一键烘成 2D 俯视像素素材的网页工具；引擎无关、本地即用，做 topdown 场景的 5 分钟稳赚。 |
| 36.27 | [LumioGames/LGUI](https://github.com/LumioGames/LGUI) | github | 153★ / 38天 | Game UI framework for AI agents, made by LumioGames. Build s | 给 AI agent 用的游戏 UI 框架：浏览器搭界面/逻辑/测试再导出 Unity，FairyGUI/Puerts 技术栈；做 UI 自动化的抓紧试玩。 |
| 35.5 | [tigin-backwards/oni-moono](https://github.com/tigin-backwards/oni-moono) | github | 277★ / 39天 | Oxygen Not Included's Unity layer on the BRUTAL framework, . | ONI 的 Unity 层迁到 BRUTAL + .NET 10 CoreCLR，后期殖民地约 8× 帧率；做性能优化/CoreCLR 迁移的活教材，强推一读。 |
| 33.0 | [CyberAgentGameEntertainment/UnityPerformanceTuningTips2026](https://github.com/CyberAgentGameEntertainment/UnityPerformanceTuningTips2026) | github | 57★ / 19天 | Unity Performance Tuning Tips 2026 — practical techniques fr | CyberAgent 一线工程师的 Unity 性能调优实战集：图形/脚本/动画/资源构建/性能分析全覆盖，日英韩简繁中五语；做手游性能的直接收藏，今日最佳。 |
| 32.84 | [christos-antono/NPCTalk](https://github.com/christos-antono/NPCTalk) | github | 233★ / 39天 | Make any NPC in any game talk back with Fish Audio: skills,  | 用 Fish Audio 让任意游戏 NPC 开口说话的 skill；mod 向非 Unity 工程向，语音 NPC 链路可参考，别指望直接进项目。 |
| 29.79 | [Cynthia-wlan9509/offline-game-revival](https://github.com/Cynthia-wlan9509/offline-game-revival) | github | 119★ / 36天 | 🎮 AI agent skill that revives shut-down mobile games as full | 停服手游离线化 skill + 录屏抽帧 OCR 抢救数据的零 token 管线；对自家产品停服资产处理有参考，注意版权边界。 |
| 27.0 | [Wendystar0628/AI-All-Round-Bench](https://github.com/Wendystar0628/AI-All-Round-Bench) | github | 108★ / 30天 | AI全能测评 \| AI All-Round Bench: timed challenge benchmarks for | AI coding agent 限时挑战基准（含游戏 AI、3D/web 应用等 10 类任务）；非游戏工具，但评估手头 agent 实战能力时值得对照，趋势级。 |

![候选来源构成](chart-sources.png)

---
*由 GameDevRadar 生成 · 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
