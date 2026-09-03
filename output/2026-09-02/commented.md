# 游戏开发技术雷达 · 2026-09-02（LLM 点评版 · 补录）

> 原始数据生成于 2026-09-02 13:04 (UTC+8)（GitHub Actions 云端运行），点评于 09-03 补写。评分 = 热度增速 × 个人画像相关度。仅供每日速览, 上榜与否不是质量背书。

**今日看点**

1. 榜首 constellation-dice 是少见的硬核 shader 玩具：Unity URP 单 raymarch pass 在默认立方体上画出星云 + 每面星座的骰子，HLSL/SDF/体积渲染全齐， shader 爱好者必看。
2. 常规军稳定：GodotHub 894★、GamePhanes 540★、Miku 166★，三大长线项目继续缓涨。
3. VFXMeshLab 98★ 即将破百，URP 程序化特效工具的关注度还在爬升。

| # | 评分 | 项目 / 话题 | 来源 | 信号 | 简介 | 点评 |
|---|---|---|---|---|---|---|
| 1 | 112.0 | [tantaneity/constellation-dice](https://github.com/tantaneity/constellation-dice) | github | 16★ / 1天 | Astral dice in Unity URP: nebula, stars and per-face constel | URP 单 pass raymarching 星空骰子 shader（HLSL/SDF/体积渲染）；学 shader 或做特殊材质表现的值得拆开看，技术密度很高。 |
| 2 | 85.16 | [RykoTheDev/GodotHub](https://github.com/RykoTheDev/GodotHub) | github | 894★ / 42天 | What if Unity Hub and GitHub Desktop had a Baby but its Adop | Godot 版「Unity Hub + GitHub Desktop」项目管理器；趋势级信号，看个方向即可。 |
| 3 | 67.5 | [GamePhanesStudio/GamePhanes](https://github.com/GamePhanesStudio/GamePhanes) | github | 540★ / 12天 | An open-source game coding agent environment and benchmark f | Godot 游戏编码 Agent 的开源环境 + 基准测试；「AI 写游戏」评测化趋势确立，趋势级必看。 |
| 4 | 41.52 | [GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-](https://github.com/GenshinmasterJinHang/Miku-Material-Converter-Blender-to-Unity-) | github | 166★ / 32天 | Miku is an open-source material conversion pipeline that tra | Blender 5.2 Shader Nodes → Unity 6 URP Shader Graph 的开源转换管线；做 URP 的 TA/美术管线强烈建议花 5 分钟。 |
| 5 | 26.57 | [nuskey8/SpriteAfterimage](https://github.com/nuskey8/SpriteAfterimage) | github | 29★ / 6天 | High-performance afterimage effect for Unity 2D using GPU in | Unity 2D GPU instancing 残影特效；做 2D/卡牌打击感的值得立刻试。 |
| 6 | 26.5 | [PudinKiller/VFXMeshLab](https://github.com/PudinKiller/VFXMeshLab) | github | 98★ / 37天 | Unity 6 URP editor tool for procedural VFX mesh authoring, m | URP 程序化 VFX 网格工具，98★ 即将破百；做技能/卡牌特效的值得 clone 试。 |
| 7 | 22.9 | [thrixel/build-world](https://github.com/thrixel/build-world) | github | 70★ / 29天 | Build interactive 3D worlds with high-quality assets from Th | 文本/Agent 驱动搭建可交互 3D 世界；原型演示向，看趋势即可。 |
| 8 | 16.5 | [How to reimagine a classic sports game for a new generation with level design, worldbuilding, and VFX](https://unity.com/blog/reimagining-backyard-baseball-3d-level-design-and-environment-art) | Unity Blog | 官方发布 | Learn how Mega Cat Studios used Unity, readable level design | Mega Cat 用 Unity 把 Backyard Baseball 重制成 3D：可读性关卡设计 + 贴花 + 光照 + VFX；美术向案例，闲时翻。 |
| 9 | 16.5 | [Building Westeros for mobile in Game of Thrones: Dragonfire](https://unity.com/blog/building-westeros-for-mobile-in-game-of-thrones-dragonfire) | Unity Blog | 官方发布 | Explore how Warner Bros. Games Boston optimized Game of Thro | WB Boston 手游化权游 IP：多人扩展、加载提速、Unity 工具链优化；一线商业手游性能优化的对口案例，值得 5 分钟。 |
| 10 | 15.0 | [How Oxobox Games built a data-driven board to power Sente’s six-player strategy](https://unity.com/blog/data-driven-board-six-player-strategy-sente) | Unity Blog | 官方发布 | How Oxobox Games used Unity's Timeline and a data-driven boa | 用 Timeline + 数据驱动棋盘做六人同步回合策略；做卡牌/战棋的可以直接参考其棋盘数据结构设计。 |
| 11 | 15.0 | [Adapting Causa: Into the Dusk for mobile with Unity 6.3](https://unity.com/blog/adapting-causa-into-the-dusk-for-mobile) | Unity Blog | 官方发布 | In this video, the Niebla Games team explains how they worke | PC 游戏用 Unity 6.3 移植手游并上 Google Play Pass 的访谈；关注 6.3 移动端实际表现的可看。 |
| 12 | 15.0 | [How Playrix is growing Township with Unity Ads’ D28 IAP ROAS optimizer](https://unity.com/blog/playrix-township-roas-optimization-vector) | Unity Blog | 官方发布 | Discover how Playrix scaled user acquisition for Township us | Playrix 用 D28 IAP ROAS 优化买量的案例；商业手游团队看长线回收模型可参考。 |
| 13 | 13.78 | [zimo-xiao-zheng/godot-ui-integration](https://github.com/zimo-xiao-zheng/godot-ui-integration) | github | 62★ / 9天 | A Codex skill for building Godot UI from approved designs wi | 「设计稿 → Godot UI 场景 + 运行时视觉一致性校验」的 Codex skill；UI 生产 + 回归校验的工作流思路值得做卡牌 UI 的借鉴。 |

---
*由 GameDevRadar 生成（LLM 点评层）· 画像配置见 profile.json · 觉得哪类多了/少了就改画像, 别忍着*
