# 研发实施步骤与并行开发计划
- 文档 ID：`DEV-PLAN-001`
- 版本：`1.0.0-prep`
- 状态：`Normative`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 适用范围：从文档冻结到第一条可运行生产链
- 直接依赖：`DEV-GOV-001`, `DEV-GOV-008`, `ARCH-PCBA-VISION-001`
- 主要输出：阶段、并行 Track、依赖 DAG、Definition of Ready/Done

>
本文均以 `ARCH-PCBA-VISION-001 v0.5.3` 为架构输入。该架构当前仍为候选基线/DocMapped，未因本开发准备包而自动获得架构批准或生产资格。若本包与未来获批架构、既有权威 Schema、状态/Owner 规范或实际仓库实现冲突，以获批权威资产为准，并必须登记冲突后再实现。


## 统一实施规则

1. **单一权威**：架构文档负责关系与边界；Schema 负责数据结构；状态/角色规范负责状态与 Owner；PageContract 负责 UI 行为；本开发指导负责实施语义、禁止路径、算法选择、性能、测试与验收；Validator/Test/Manifest 分别负责机器校验、验证和交付快照。
2. **不得复制第二套事实**：不得在产品目录、`spi/`、ViewModel、数据库或临时 JSON 中建立与既有 Schema/状态机/Owner 平行的定义。
3. **事实与投影分离**：UI、报告、统计、质量中心、AI 解释均不得成为事实的第二写入权威。
4. **生产副作用后移**：能在离线/Replay/Shadow 完成的验证，不要先绑定真实设备。
5. **所有失败可见**：失败、拒识、覆盖不足、资格不足、配置失效都必须形成显式状态或事实，禁止默认降级为 Success/OK。
6. **性能有预算**：算法、UI、IO、缓存和并发必须在设计时给出预算；不得在最后阶段才测性能。
7. **可回放优先**：能进入 Replay 的输入、参数、模型、库快照、标定、环境和算法版本尽量冻结为可引用资产。
8. **AI 不获得隐含权威**：AI 只能在既定 Contract 和 AuthorityScope 内产生建议/候选或受控 revision。
9. **真实硬件证据单独计门**：没有 HIL/实机/Golden Sample 证据时，文档状态只能为“准备完成/软件可验证”，不能写“生产通过”。

## 1. 总体策略

不按“先把平台全部写完，再写产品”推进，也不按“所有产品同时开工”推进。采用一条**最小真实纵向链**验证平台契约，同时并行准备公共能力、UI、数据和领域能力。

## 2. 推荐阶段

### Gate A — Architecture/Contract Freeze
- 处理 v0.5.3 当前 P1：BoardModel 与 ManufacturingGeometrySnapshot 的 authority wording；
- 冻结 ContractCandidate、Authority Matrix、PageContract Catalog、Terminology Registry；
- 建立本包审计脚本并先跑通过。

### Gate B — Shared Core Skeleton
并行：Identity / Pipeline / Evidence / Vision Contract / UI Shell / Test Harness / Performance Harness。

硬依赖：Contract/Schema；不得直接接 HALCON 原生对象到 UI。

### Gate C — SPI Vertical Slice
```text
Identity → Acquisition → HeightField → Reference → Paste Measurement → Rule → Decision → Evidence → Review
```
此阶段用于验证共享能力，不把 SPI 专属对象上提平台。

### Gate D — Product Expansion
并行准备 PCB / Stencil / AOI / HDI / FPC 的领域 Contract 和工作区；按真实设备和样本分别关闭。

### Gate E — AI / QualityControl
Replay → Shadow → Advice；跨工序质量中心最后接入稳定事实。

## 3. 并行开发 Track

| Track | 可并行内容 | 前置 |
|---|---|---|
| A Contract | Schema/Validator/Golden Vector | Architecture wording + ownership |
| B Platform Runtime | Identity/Pipeline/Evidence | A |
| C Vision | Vision Contract/HALCON adapters/benchmark | A |
| D Metrology | Calibration/Measurement | A + coordinate semantics |
| E UI | Shell/Design System/PageContract/Viewport | A |
| F Device | Camera/Light/Motion adapters | Contract + target hardware |
| G Domain | SPI/PCB/AOI/Stencils 等领域 | B/C/D/E 最小契约 |
| H Quality/AI | SPC/Replay/AI | Facts/Evidence 稳定 |
| I Qualification | Golden/HIL/Performance | 各切片可运行 |

## 4. 可并行与不可并行

**可并行**：UI shell 与 Vision adapter；Schema validator 与 UI prototypes；Golden dataset 建设与基础 runtime；算法 benchmark 与 domain UI；部署脚本与测试 harness。

**必须串行**：Authority/Contract 定义 → 依赖的 State/Schema → Production write path；Calibration qualification → Metrology production enablement；HeightField validity → SPI metrology qualification；BoardDataAdapter semantics → NPI program synthesis；Evidence commit → quality release reporting。

## 5. AI 开发循环

```text
读取 Architecture + Guide + Contract
 → 明确改动文件
 → 实现最小切片
 → 编译
 → 单元/契约测试
 → 回放/Benchmark
 → 自动审计
 → 人工审阅
 → Merge
```

## 6. 首个可运行里程碑

不是“UI 出来了”，而是完成一条可回放、可追溯、可恢复的最小检测链：`BoardIdentity → EffectiveRuntimeManifest → AcquisitionFrame → MeasurementSet/Fact → DecisionProposal → EvidenceCommit → ResultCommitReceipt → Review projection`。真实设备 Release 仍受 Qualification Gate 控制。
## 7. 阶段退出条件（不可跳过）

| 阶段 | 必须关闭 | 不得提前进入下一阶段 |
|---|---|---|
| A Contract Freeze | Authority/Schema/State/Owner/Terminology/Implementation Binding | 不得直接进入生产写路径 |
| B Shared Core | Identity/Manifest/Pipeline/Evidence/Validator/Test harness 可回放 | 不得开始跨领域共享事实写入 |
| C SPI Vertical Slice | 从输入到 Evidence/Review 的单板闭环 + Replay + Fault + Benchmark | 不得用 UI 完成替代后端事实 |
| D Domain Expansion | 每个领域自己的 Contract/Guide/FS/Page/Golden | 不得跨域复制内部 Store/State |
| E AI/Quality | Replay/Shadow/Advice 链与统计口径冻结 | 不得直接获得生产 Decision Authority |
| F Qualification | Golden/HIL/Performance/Metrology/Interop/Production Gates | 不得把 `ImplementationSpecificationReady` 当 `Qualified` |

## 8. 并行开发的串行化条件

任何下列共享资源发生变更时，所有消费者先暂停再串行处理：同一 Schema Major、同一 State/Owner、同一 AuthoritySource、同一 Evidence Commit 语义、同一 Runtime Manifest 绑定。只有变更回归通过后，消费者任务才重新开放。

## 9. AI 并行开发隔离

每个 AI Agent 必须领取唯一 `TaskId + FunctionSpecId`；不同 Agent 不允许同时修改同一 Contract/Schema/Owner/Manifest 入口。UI、算法、测试可以并行，但共享 Contract 变更必须通过单一 Owner 队列。
