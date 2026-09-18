# 从零到生产的不可跳步开发主序列
- 文档 ID：`DEV-PLAN-013`
- 版本：`1.0.0`
- 状态：`Normative`
- 目的：把全部平台开发工作转换为可执行、可并行、可停止、可回退的研发 DAG。

## 1. 主关键路径

```text
G0 实施绑定确认
→ Contract/Schema/State/Owner 冻结
→ Repository Skeleton / CI / Validator
→ Identity + RuntimeManifest
→ Pipeline + Evidence/Commit
→ Coordinate/Calibration/Metrology 基础
→ Vision Contract + HALCON adapter
→ UI DesignSystem + Viewport
→ SPI Vertical Slice
→ Golden/Replay/Benchmark
→ PCB/Stencil/HDI/FPC/AOI/Quality 扩展
→ AI/ProgramSynthesis
→ HIL/Qualification
→ Production Gate
```

## 2. 可并行轨道

| Track | 可并行工作 | 串行前提 |
|---|---|---|
| Contract | Schema / Validators / Golden schema | Authority/Owner 已冻结 |
| Platform | Identity / Manifest / Evidence / Pipeline | Contract 基础已冻结 |
| Vision | ImageFrame / ROI / Registration / Algorithm adapters | Vision Contract 已冻结 |
| Metrology | Calibration / Measurement / Coverage | Coordinate/Unit semantics 已冻结 |
| UI | DesignSystem / Workspaces / PageContract implementation | PageContract 与 Application Port 已冻结 |
| Domain | PCB/HDI/FPC/Stencil/SPI/AOI capability implementations | 对应 FunctionSpec + domain Contract 已冻结 |
| Quality/AI | SPC / Correlation / AI shadow/advice | Facts/Evidence watermarks 已冻结 |
| Qualification | Golden / Replay / Benchmark / HIL harness | 被测能力已有 deterministic implementation |

## 3. 绝对禁止的并行组合

- 两个 Agent 同时修改同一 Schema Major；
- 两个 Agent 同时定义同一 Owner/AuthoritySource；
- UI Agent 和 Domain Agent 同时改变同一事实语义；
- 算法 Agent 在未冻结 FunctionSpec 前自行改变 Primary/Fallback/Reject；
- 测试 Agent 不得自行降低 Acceptance 以配合实现；
- AI Agent 不得因为另一个 Agent 的未提交改动而复制其未经审计的结论。

## 4. 每个任务的执行循环

`Task领取 → 读取当前 Registry/FS/Page → 依赖确认 → Task Card → 实施 → Validator → Unit/Integration → Replay → Benchmark → Review → Audit → 提交证据 → 解锁消费者`。

## 5. 最小可交付切片

每个 FunctionSpec 最小代码切片至少包括：Port/Contract binding、Implementation、Validator、Tests、Benchmark instrumentation、Evidence/trace mapping，以及在适用时的 PageContract projection。没有真实硬件 SDK 时，必须使用明确的 Mock/Simulator/Adapter boundary，不得假装完成设备行为。

## 6. 阶段退出

A. Contract Freeze：无冲突 Owner/Schema/Authority。
B. Shared Core：可回放、可失败、可取消、可恢复、可验证。
C. SPI Vertical Slice：单板从输入到 Evidence/Review 闭环。
D. Domain Expansion：每个领域按自己的事实语义和 FunctionSpec 扩展。
E. AI/Quality：AI 只获得 Shadow/Advice/受控 Proposal 能力，除非另有明确 Authority 绑定。
F. Qualification/Production：Golden、Performance、Metrology、HIL、Interop、Production Gate 全部按适用范围关闭。

## 7. 停止条件

任何任务遇到缺失/冲突的 Contract、字段、枚举、Owner、状态、权限、坐标、单位、阈值、算法候选、API 或验收口径，必须停止在该依赖边界，不得绕过。


## 12. 并行实施与纵向切片绑定

- 全量工作包、前置/并行/串行关系唯一依据：`DEV-PLAN-014`。
- 仓库零点启动和第一条可运行纵向切片唯一依据：`DEV-PLAN-015`。
- 本文不得自行增加第二套研发 DAG。


## 8. 阶段与 WorkPackage 精确映射

| 阶段 | Primary WorkPackages | 进入条件 | 退出条件 | Blocker | Scope/变更规则 | Test/验收来源 |
|---|---|---|---|---|---|---|
| G0 实施绑定确认 | WP-01 | GATE-001 权威解析 | Repository/Authority Traceability 可复现 | 权威缺失/冲突即停止 | Scope 只允许门禁解析与注册 | Contract/Registry gate |
| Contract 冻结 | WP-02 | Owner/Consumer/Authority 已明确 | Schema/Validator/Contract Test | Schema/Owner 冲突 | Schema Major 变更需重新计算 DAG | Contract tests / Golden Vector |
| Shared Core | WP-03..WP-08 | Contracted + deterministic skeleton | Identity/Manifest/Pipeline/Evidence/Coordinate/Metrology 可回放 | 缺状态/幂等/单位/坐标 | 不得扩展到领域功能 | Unit/Integration/Replay/Recovery |
| Vision foundation | WP-09..WP-11 | Vision Contract + target API verification | Vision adapter/Viewport/Benchmark harness | HALCON/API 未验证 | 算法路线变更触发 FS 变更 | Golden/Benchmark/UI performance |
| UI foundation | WP-12..WP-13 | PageContract/App Port 冻结 | DesignSystem + Program/Recipe workspaces | PageContract 漂移 | UI 不得改变事实语义 | UIA/DPI/keyboard/localization |
| Device shells | WP-14 | SDK gate explicitly isolated | Simulation/Replay adapter path | SDK 未验证不得伪装完成 | 硬件 Scope 独立 | Contract + simulator |
| Domain expansion | WP-15..WP-20 | 对应 FunctionSpec/Contracted | PCB/FPC/HDI/Stencil/SPI/AOI/Quality capability chains | 缺 Domain Contract / Gate | 每个 FS 只归属一个 Primary WP | Domain Golden/Replay/Performance |
| AI | WP-21 | Facts/Evidence/Review contracts ready | Shadow/Advice/controlled Proposal | 数据/模型/用途未冻结 | 不得升级为 Production Authority | Held-out validation / leakage checks |
| Qualification/Production | WP-22 | 被测实现 deterministic | All applicable gates closed | 任一适用 gate 未关闭 | Production scope 不得自行扩大 | Golden/Calibration/GR&R/HIL/Interop/Acceptance |

任何任务发生 ScopeDrift 时，状态必须进入 Blocked/Change Review；不得以“先实现再补文档”作为绕过。


## Scope Freeze

主序列只允许执行已登记的 WorkPackage、FunctionSpec、Contract 和 Gate。任何新增对象、状态、Owner、接口或算法路线必须先变更评审。

## DoR

进入任一阶段前：前置 Contract/Schema/Owner/State 已解析；输入证据可获得；目标版本 API 已验证；上游 DoD 已满足。
