# 全量研发工作包与并行依赖 DAG

- 文档 ID：`DEV-PLAN-014`
- 版本：`1.0.0`
- 状态：`ImplementationSpecificationReady`
- 直接依赖：`DEV-PLAN-001`, `DEV-PLAN-002`, `DEV-PLAN-003`, `DEV-PLAN-013`, `DEV-GOV-013`, `DEV-GOV-015`, `GATE-001`

## 1. 目标

把 69 个 Function Specification、平台能力、UI、Qualification 和真实代码仓库任务组织成**可并行但不互相越权**的开发 DAG。此表是开发调度规则，不替代 Contract/Schema/Owner 权威。

## 2. 工作包定义

| WP | 工作包 | 主要范围 | 前置 | 可并行 | 必须串行化的共享入口 | 主要产物 | DoR | DoD |
|---|---|---|---|---|---|---|---|---|
| WP-01 | Governance/Repo Bootstrap | 仓库结构、Registry、Task Traceability、Authority Gate；Primary FS：无 Primary FS；平台/治理/资格基础任务 | GATE-001 | WP-02..WP-22（仅不改同一共享权威） | Registry/Manifest/Authority | repo skeleton, CI checks | 权威已解析 | 仓库可复现、门禁可跑 |
| WP-02 | Contract/Schema | C-01..C-08、Schema/Validator/Test；Primary FS：FS-005 | WP-01 | 各领域研究 | 同一 Schema Major | Schema, Validator, Contract tests | Owner+consumer明确 | Contracted |
| WP-03 | Identity/BoardRun | FS-001..010相关；Primary FS：FS-001, FS-006 | WP-02 | WP-04,05,06,08 | DEV-ARC-005 | identity, run, retry/recovery | State/Owner已解析 | 回放+失败恢复通过 |
| WP-04 | Effective Manifest/Recipe | FS-002, recipe/runtime；Primary FS：FS-002 | WP-02 | WP-03,05,08 | Recipe Manifest | resolver, manifest validator | Schema已解析 | 冻结绑定可回放 |
| WP-05 | Evidence/Commit | FS-011..等证据/提交链；Primary FS：FS-004 | WP-02 | WP-03,06,08 | Evidence schema/commit owner | evidence service | commit owner已解析 | commit/retry/idempotency通过 |
| WP-06 | Pipeline/Attempt | FS-010..等 Pipeline/Attempt；Primary FS：FS-003 | WP-02 | WP-03,05,07 | pipeline contract | DAG, cancellation, recovery | PF已绑定 | 五终结路径通过 |
| WP-07 | Coordinate/Transform | FS/vision coordinate相关；Primary FS：FS-015 | WP-02 | WP-08..11 | coordinate schema | transforms, golden vectors | units/frame declared | golden vectors pass |
| WP-08 | Calibration/Metrology | metrology/calibration；Primary FS：FS-016, FS-017, FS-018, FS-019 | WP-02,07 | WP-09..15 | qualification artifact | calibration/metrology | capability domain declared | qualification evidence |
| WP-09 | Vision/HALCON Foundation | FS-020..等视觉基础；Primary FS：FS-007, FS-008, FS-009, FS-010, FS-011, FS-012 | WP-02,07 | WP-10..15 | Vision Contract | backend adapters | actual HALCON target verified | benchmark harness |
| WP-10 | HALCON Benchmark | candidate algorithms, robustness challenges；Primary FS：FS-013, FS-014 | WP-09 | domain work | benchmark profile | candidate matrix, benchmark evidence | Golden dataset | approved route per capability |
| WP-11 | Viewport/Image | FS vision/UI image display；Primary FS：无 Primary FS；平台/治理/资格基础任务 | WP-02,07 | WP-09 | viewport contract | WPF viewport | rendering contract | UI/HMI benchmark |
| WP-12 | UI Design System | WPF+DevExpress shell/workspaces；Primary FS：无 Primary FS；平台/治理/资格基础任务 | WP-02 | WP-03..10 | PageContract catalog | design system | PageContract IDs fixed | UIA/DPI/keyboard pass |
| WP-13 | Program/Recipe UI | programming workbench；Primary FS：无 Primary FS；平台/治理/资格基础任务 | WP-04,12 | domain UI | Program/Recipe commands | workspace | authority resolved | no direct fact writes |
| WP-14 | Device/Acquisition adapters | camera/light/motion/protocol adapter shells；Primary FS：无 Primary FS；平台/治理/资格基础任务 | WP-02,11 | WP-07..12 | device port only | adapter interfaces | hardware SDK gate stated | simulation/replay path |
| WP-15 | PCB domain | FS-030..etc；Primary FS：FS-020, FS-021, FS-022, FS-023, FS-024, FS-025, FS-026, FS-027, FS-028 | WP-02,07,09 | SPI/Stencil/AOI | domain contracts | PCB domain | targets defined | domain golden/replay |
| WP-16 | FPC/Rigid-Flex/HDI | corresponding domains；Primary FS：FS-029, FS-030, FS-031, FS-032, FS-033, FS-034, FS-035, FS-036, FS-037, FS-038 | WP-02,07,08,09 | PCB/SPI/Stencil/AOI | domain contracts | FPC/HDI modules | applicability matrix | domain qualification |
| WP-17 | Stencil | stencil FS；Primary FS：FS-039, FS-040, FS-041, FS-042, FS-043 | WP-02,07,08,09 | SPI | stencil contracts | stencil inspection | geometry/height gates | golden/replay |
| WP-18 | SPI | SPI FS + printer feedback；Primary FS：FS-044, FS-045, FS-046, FS-047, FS-048, FS-049, FS-050, FS-051, FS-052, FS-053, FS-054, FS-055 | WP-02,07,08,09,10,12 | PCB/Stencil/AOI | SPI contracts | end-to-end SPI | 3D capability qualified | NPI→run→SPC loop |
| WP-19 | AOI | AOI FS + review/rework；Primary FS：FS-056, FS-057, FS-058, FS-059, FS-060, FS-061, FS-062, FS-063 | WP-02,07,09,12 | SPI/PCB | Decision authority | AOI | component/pin/solder semantics | review/rework loop |
| WP-20 | QualityControl | SPC/correlation/disposition；Primary FS：FS-064, FS-065, FS-066 | WP-03,05,08,15..19 | non-writing analytics | quality contracts | Quality Hub | denominator + evidence rules | read-only facts + controlled events |
| WP-21 | AI/ProgramSynthesis | dataset, replay, synthesis, assist；Primary FS：FS-067, FS-068, FS-069 | WP-05,08,10,13,15..20 | domain implementation | AI lifecycle gates | AI modules | usage tier + evidence | no production authority leak |
| WP-22 | Qualification/Production Gate | benchmark/HIL/acceptance；Primary FS：无 Primary FS；平台/治理/资格基础任务 | WP-08..21 | documentation | qualification owner | qualification pack | real samples/env | gate closed by evidence |

## 3. FS 分配原则

每个 FS 必须且只能有一个 Primary Work Package；允许多个 Consumer Work Packages。具体映射以 `function-implementation-registry-v2.csv`、`DEV-PLAN-008` 与 FS 内 Canonical Implementation Contract 为准。工作包不得根据任务名称自行修改 FS 归属。

## 4. 并行规则

可并行 ≠ 可共享写入。多个 Agent 可以并行开发不同模块，但以下共享入口必须串行修改：Schema Major、AuthoritySource、Owner/State、Manifest schema、Event catalog、AcceptanceProfile、PageContract catalog、Term registry、Module registry。

## 5. 每个工作包的统一生命周期

```text
DoR → Scope Freeze → Implement → Unit/Contract Test → Integration → Failure/Recovery → Benchmark → Review → Audit → DoD
```

## 6. 冲突与抢占

发现跨工作包依赖变化、共享 Contract 修改、Authority 冲突、Schema Major 变化、Acceptance 改动或性能基线变化时：停止受影响任务，重新计算 DAG；不得继续“先写完再说”。

## 3.1 FS → Primary Work Package 完整映射

以下是机器可检查的主归属；一个 FS 只能有一个 Primary Work Package。跨 WP 消费者不改变 Primary Owner。

| FS | Primary Work Package |
|---|---|
| `FS-001` | `WP-03` |
| `FS-002` | `WP-04` |
| `FS-003` | `WP-06` |
| `FS-004` | `WP-05` |
| `FS-005` | `WP-02` |
| `FS-006` | `WP-03` |
| `FS-007` | `WP-09` |
| `FS-008` | `WP-09` |
| `FS-009` | `WP-09` |
| `FS-010` | `WP-09` |
| `FS-011` | `WP-09` |
| `FS-012` | `WP-09` |
| `FS-013` | `WP-10` |
| `FS-014` | `WP-10` |
| `FS-015` | `WP-07` |
| `FS-016` | `WP-08` |
| `FS-017` | `WP-08` |
| `FS-018` | `WP-08` |
| `FS-019` | `WP-08` |
| `FS-020` | `WP-15` |
| `FS-021` | `WP-15` |
| `FS-022` | `WP-15` |
| `FS-023` | `WP-15` |
| `FS-024` | `WP-15` |
| `FS-025` | `WP-15` |
| `FS-026` | `WP-15` |
| `FS-027` | `WP-15` |
| `FS-028` | `WP-15` |
| `FS-029` | `WP-16` |
| `FS-030` | `WP-16` |
| `FS-031` | `WP-16` |
| `FS-032` | `WP-16` |
| `FS-033` | `WP-16` |
| `FS-034` | `WP-16` |
| `FS-035` | `WP-16` |
| `FS-036` | `WP-16` |
| `FS-037` | `WP-16` |
| `FS-038` | `WP-16` |
| `FS-039` | `WP-17` |
| `FS-040` | `WP-17` |
| `FS-041` | `WP-17` |
| `FS-042` | `WP-17` |
| `FS-043` | `WP-17` |
| `FS-044` | `WP-18` |
| `FS-045` | `WP-18` |
| `FS-046` | `WP-18` |
| `FS-047` | `WP-18` |
| `FS-048` | `WP-18` |
| `FS-049` | `WP-18` |
| `FS-050` | `WP-18` |
| `FS-051` | `WP-18` |
| `FS-052` | `WP-18` |
| `FS-053` | `WP-18` |
| `FS-054` | `WP-18` |
| `FS-055` | `WP-18` |
| `FS-056` | `WP-19` |
| `FS-057` | `WP-19` |
| `FS-058` | `WP-19` |
| `FS-059` | `WP-19` |
| `FS-060` | `WP-19` |
| `FS-061` | `WP-19` |
| `FS-062` | `WP-19` |
| `FS-063` | `WP-19` |
| `FS-064` | `WP-20` |
| `FS-065` | `WP-20` |
| `FS-066` | `WP-20` |
| `FS-067` | `WP-21` |
| `FS-068` | `WP-21` |
| `FS-069` | `WP-21` |

## 7. Blocker 与验收门禁

### 7.1 Blocker

以下任一项出现，工作包立即停止：

- Contract/Schema/Owner/State/AuthorityScope 未冻结或出现冲突；
- 共享权威资产存在两个写入者；
- FunctionSpec / PageContract / Registry / CodeTask 的绑定不一致；
- 目标 HALCON/DevExpress/AsunImage API 未在实际目标版本资料中确认；
- PerformanceProfile、Golden/Replay、AcceptanceProfile 缺失；
- 并行工作包需要同时修改同一个 Schema Major、Owner、AuthoritySource 或 PageContract catalog；
- 测试失败但修改 Acceptance 口径未经过变更控制。

### 7.2 验收

每个工作包必须完成：Scope Freeze → Contract/Schema 验证 → 实现 → Unit/Contract → Integration → Failure/Recovery → Replay/Golden → Benchmark → Review → Audit → DoD。任何一个阶段未通过，工作包不能声明完成，也不能解除其下游 Blocker。
