# SPI Paste Segmentation与Metrology开发指导
- 文档 ID：`DEV-DOM-055C`
- 版本：`1.0.0-prep`
- 状态：`ImplementationSpecificationReady`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 适用范围：锡膏分割、体积、面积、高度、偏移、形状
- 直接依赖：`DEV-DOM-055B`; `DEV-MET-031`; `DEV-VIS-026`
- 主要输出：Paste FeatureMeasurement、MeasurementSet、CoverageResult

> 本文以 `ARCH-PCBA-VISION-001 v0.5.3` 为架构输入。未提供给本任务的真实仓库 Contract/Schema、实际 DevExpress 版本、HALCON 安装、实机与资格证据不会被虚构。

## 实施规则

- 领域负责语义；平台负责稳定技术能力；UI 不成为事实权威。
- 算法必须提供默认路径、Fallback、拒识和质量评分。
- 所有生产相关参数都必须可版本化、可回放、可审计。
- 大图/3D 优先 ROI、Tile/Pyramid/LOD、分层计算；先低成本候选，再高成本精测。
- 任何“自动优化”必须输出 Draft/Proposal，不得静默修改生产权威。

## 验收最小集

正常样本、边界样本、难例/污染样本、拒识样本；P50/P95/P99；失败/恢复；回放一致性；UI Automation；证据闭环。
## 推荐处理层次

1. 输入质量与 ValidMask；2. 基准面；3. ROI 裁剪；4. 候选分割；5. 边界质量；6. 高度/面积/体积积分；7. 中心/偏移；8. 形状；9. 质量评分；10. RuleEvaluation。

分割方法可采用全局阈值、局部阈值、模型/几何约束等候选，但不能仅以单样本通过率选算法。Benchmark 至少覆盖光泽变化、桥连、少锡、污染、反射、边缘阴影。

## 测试与验收

- 正常、边界、难例/污染、拒识样本；
- Replay 前后结果差异可解释；
- 关键参数变更触发必要回归；
- 失败/取消/恢复路径有证据。

## 性能

必须记录典型耗时、P95/P99、峰值内存、并发条件和数据规模；大图/3D 必须标明 ROI、分辨率和中间结果生命周期。

## 禁止实现

不得将领域专有对象降格为通用对象；不得绕过 Contract/Owner；不得将拒识/不可观察状态静默转换为 OK；不得用 AI/经验值修改生产权威。


## 实施级关联功能

本指导的具体功能以以下实施规范为准：FS-047, FS-048, FS-049, FS-050, FS-051, FS-052。每个 FS-xxx 都包含输入/输出、算法、鲁棒性、性能、UI、测试、验收和禁止实现。
