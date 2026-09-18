# ContractCandidate 详细实施基线
- 文档 ID：`DEV-CONTRACT-BASE-001`
- 版本：`1.1.0`
- 状态：`ImplementationSpecificationReady`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`

本文不是第二套 schema；它是 C-01~C-08 的**字段语义、边界、验证规则和消费者映射**。正式代码必须把这些语义绑定到仓库唯一权威 Schema。

## C-01 身份与状态

覆盖：Product/ProductRevision/WorkOrder/Lot/Board/Panel/PanelUnit/Layer/Side/Roll/RollSegment/Carrier/StationInstance/ProcessAttempt/InspectionAttempt/BoardRun，以及 Scope、Parent、Causality、Epoch、Owner。

### 必须验证
- ID 唯一；版本和 hash 对应同一内容；Parent 不形成循环；Scope 不交叉越权。
- 状态必须满足正交状态约束；不能把质量状态、执行状态、设备状态压成一个枚举。
- 重复事件幂等；乱序按批准策略处理；时间域/epoch 不匹配拒绝。

## C-02 Program/Recipe/Manifest

覆盖 ProductRevision、ManufacturingGeometrySnapshot、SourceSetManifest、LayerStack/BuildUp/ViaDefinition、InspectionProgram、Recipe、MachineProfile、Calibration、Library 与 EffectiveRuntimeManifest。

### 必须验证
- 所有生产依赖明确 `id + version + sha256`。
- `latest`、隐式 default、同 version 异 hash、循环依赖均拒绝。
- ManufacturingGeometrySnapshot 必须引用 approved BoardModel，不重复建 Geometry 主库。

## C-03 Coordinate/3D/Metrology

覆盖 CoordinateGraph/FOV/Stitching/HeightField/Rigid/Affine/NonRigid/WebCoordinate、MeasurementDefinition/Quality/Uncertainty/Qualification。

### 必须验证
- 单位、方向、参考坐标和有效域显式；domain/out-of-domain 必须拒绝。
- Raw/Canonical/Display 三层分离。
- MeasurementPairing、ReferenceFrame、Direction、Finite/Infinite support 显式。

## C-04 Pipeline/Decision/Commit

覆盖 Stage/MeasurementSet/CoverageResult/InspectionFact/RuleEvaluation/InspectionDecision/MachineDecision/Evidence/Receipt/Feedback。

### 必须验证
- CalculationIdentity 与 CommitIdentity 分离。
- 只有批准 Producer/CommitOwner/ActivationOwner 能执行相应生产副作用。
- 重算/重试不生成重复事实；ACK 不等价 Applied/Committed。

## C-05 WPF product workspace

覆盖 PageContract/ApplicationPort/Query/Command/Permission/AutomationId/八态/Shortcut/Localization/DPI/UIA。

### 必须验证
- 页面不得持有 HALCON/AsunImage 原生对象。
- Command 必须映射到受控 Application Port；权限校验不能只在 UI。
- 八态和异常恢复必须可自动化测试。

## C-06 Deployment/Security/Plugin/Adapter

覆盖进程 Owner、IPC、签名、证书、PluginManifest、Adapter capability/version/permission/resource。

### 必须验证
- SDK 类型不得穿透领域层。
- 未签名/篡改/能力不兼容/证书失效必须阻断。
- 卸载插件后状态可解释地进入 NotReady，而不是崩溃/静默降级。

## C-07 Performance/Recovery

覆盖 PerformanceProfile、ContinuousInspectionProfile、queue、backpressure、resource、HIL、long-run、RPO/RTO。

### 必须验证
- 每条生产链都有 P50/P95/P99、内存峰值、队列上限、取消、恢复指标。
- 备份存在不等于可恢复；恢复演练需要独立证据。

## C-08 Product assembly/route/coverage/cross-site/external test

覆盖 ProductAssembly/ProcessRoute/InspectionCoverage/Station/CrossSiteEvent/ExternalTestRegistration/QualityPlan/RequiredEvidenceSet/RouteAssessmentContext。

### 必须验证
- RouteAssessment 只能消费已提交事实与明确水位；不直接读取当前 UI/原图/临时 Recipe。
- Coverage 缺口、迟到、未知条件必须可见。
- 外部测试结果必须有来源、适用范围、版本、时间和 commit semantics。
