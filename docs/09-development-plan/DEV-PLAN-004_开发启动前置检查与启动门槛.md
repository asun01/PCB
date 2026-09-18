# 开发启动前置检查与启动门槛
- 文档 ID：`DEV-PLAN-004`
- 版本：`1.0.0-prep`
- 状态：`Normative`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 适用范围：从开发准备包交付到正式代码仓库开始第一个 ImplementationReady 切片
- 直接依赖：`DEV-PLAN-001`, `DEV-PLAN-003`, `DEV-GOV-004`, `DEV-GOV-008`, `DEV-GOV-009`
- 主要输出：启动检查清单、阻断门、首个代码切片准入标准、外部证据接口

> 本文只定义“什么时候可以开始写代码、以什么条件开始写代码”。它不把文档完成等同于产品生产资格。真实仓库、已安装 SDK、设备、HIL、Golden Sample、计量与互操作证据仍必须独立关闭。

## 1. 启动原则

正式代码开发采用 **Contract-first + Vertical-slice + Replay-first + Qualification-later**。

```text
Architecture Candidate
  → Architecture wording gate
  → Contract / Schema / Owner freeze
  → Development Guide freeze
  → Validator / Test harness
  → First vertical slice
  → Replay / Benchmark
  → HIL / Qualification
  → Production Gate
```

除正式架构批准外，代码可在 `ARCH-REVIEW-003` + `ARCH-REVIEW-004` 绑定的当前语义下实施；正式架构批准仍不得伪装成已通过。

## 2. 必须关闭的前置门

| 门 | 内容 | 状态 | 关闭证据 |
|---|---|---|---|
| G0 | ARCH v0.5.3 P1：BoardModel vs ManufacturingGeometrySnapshot | `IMPLEMENTATION_BOUND` | `ARCH-REVIEW-003` + `ARCH-REVIEW-004` |
| G1 | 现有 Contract/Schema/State/Owner/Validator/Test 仓库绑定 | `EXTERNAL_REQUIRED` | 真实文件路径 + hash + validator + test |
| G2 | .NET/VS/DevExpress 目标版本与许可证基线 | `EXTERNAL_REQUIRED` | 实机环境清单 + license evidence |
| G3 | HALCON 目标版本、license、运行环境 | `EXTERNAL_REQUIRED` | 安装信息 + license + sample execution |
| G4 | 目标相机/光源/3D/运动/PLC 设备清单 | `EXTERNAL_REQUIRED` | DeviceProfile + SDK + HIL 接口 |
| G5 | Golden Sample / Replay 数据集 | `EXTERNAL_REQUIRED` | Dataset manifest + hash + ground truth policy |
| G6 | 性能基线 | `EXTERNAL_REQUIRED` | PerformanceProfile + benchmark raw data |
| G7 | 第一条 SPI Vertical Slice 资格 | `PREPARED` | 本包文档 + test/replay harness |

## 3. 首个代码切片

推荐首个真实代码切片仍为 SPI Vertical Slice，但只实现最小稳定链：

```text
BoardIdentity
 → EffectiveRuntimeManifest
 → AcquisitionFrame
 → HeightField / ValidMask
 → Reference Plane
 → Paste FeatureMeasurement
 → MeasurementSet
 → RuleEvaluation
 → Decision Proposal
 → EvidenceCommit
 → ResultCommitReceipt
 → Review Projection
```

不在第一条纵向链里一次性加入：跨站互联、AI 自动生产权威、复杂 SPC、全量 AOI、全量 PCB、所有 3D 算法。

## 4. Definition of Ready

一个代码任务只有全部满足以下条件才能开始：

1. 有唯一 `Document ID` 和开发指导；
2. 输入/输出 Contract 明确；
3. Schema 或现有权威 Schema 已绑定；
4. State/Owner/AuthorityScope 明确；
5. 真实 Consumer 明确；
6. 失败、取消、超时、重试、恢复路径明确；
7. 性能预算存在；
8. UI 有 PageContract（若涉及 UI）；
9. Golden/Replay/Test 数据策略明确；
10. 禁止路径已定义。

## 5. Definition of Done

任务完成必须同时满足：

```text
代码
+ Contract Test
+ Unit/Integration Test
+ Replay/Golden Test
+ Performance Evidence
+ Audit Pass
+ Documentation Status Update
+ Manifest / Version Reference
```

需要真实设备或计量资格的项目还必须附加 HIL/Qualification evidence。

## 6. AI 编码准入

AI 在执行代码任务前必须读取：

```text
Architecture
 → 对应 Development Guide
 → Contract / Schema
 → State / Owner
 → 依赖 Guide
 → PageContract（UI任务）
 → Golden/Test/Benchmark
 → 当前代码仓库实际 API
```

禁止仅凭自然语言任务标题生成代码。遇到目标仓库中不存在的 API、Schema、HALCON operator、DevExpress API 或 AsunImage API，必须停止猜测并要求证据。

## 7. 启动结论

当前开发准备包可以作为**文档/规划/审计基线候选**交付并用于建立代码仓库骨架；正式进入 `ImplementationReady` 前仍必须关闭 G0–G6 中的外部/审批门。任何“准备包 PASS”均不等同于生产可用。

## 8. 实施

开发启动前必须把 G0–G6 转换为可执行的工程任务，并在仓库的 Contract/Schema/Validator/Test/Manifest 中建立对应入口。每一个任务必须回指本文件、具体 Development Guide 和实际代码路径。

## 9. 测试

启动前验证至少包括：文档审计、Contract/Schema 校验器自检、依赖图环检测、Golden/Replay harness 可执行性检查、性能 harness 可执行性检查。真实设备相关测试进入 HIL/Qualification 门，不由本文件静态替代。

## 10. G0 精确定义

G0 现在被定义为“**实施语义已冻结**”，不是“架构文档已正式批准”。任何制造几何相关任务如果缺少 `ARCH-REVIEW-004` 的绑定引用，仍然阻断。
