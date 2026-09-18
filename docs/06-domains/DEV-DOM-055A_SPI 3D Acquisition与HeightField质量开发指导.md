# SPI 3D Acquisition与HeightField质量开发指导
- 文档 ID：`DEV-DOM-055A`
- 版本：`1.0.0-prep`
- 状态：`ImplementationSpecificationReady`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 适用范围：3D SPI 采集、相位/高度结果有效性
- 直接依赖：`DEV-DOM-055`; `DEV-VIS-027`; `DEV-PLT-015`
- 主要输出：HeightField + ValidMask + AcquisitionQuality

> 本文以 `ARCH-PCBA-VISION-001 v0.5.3` 为架构输入。未提供给本任务的真实仓库 Contract/Schema、实际 DevExpress 版本、HALCON 安装、实机与资格证据不会被虚构。

## 实施规则

- 领域负责语义；平台负责稳定技术能力；UI 不成为事实权威。
- 算法必须提供默认路径、Fallback、拒识和质量评分。
- 所有生产相关参数都必须可版本化、可回放、可审计。
- 大图/3D 优先 ROI、Tile/Pyramid/LOD、分层计算；先低成本候选，再高成本精测。
- 任何“自动优化”必须输出 Draft/Proposal，不得静默修改生产权威。

## 验收最小集

正常样本、边界样本、难例/污染样本、拒识样本；P50/P95/P99；失败/恢复；回放一致性；UI Automation；证据闭环。
## 采集前
检查设备 ready、预热、曝光/增益、同步、环境和标定有效性。

## 采集后
生成 HeightField、ValidMask、Quality flags。将饱和、解算失败、阴影/遮挡、低信噪区域显式标记。

## 重建
优先只处理有效 ROI；保存必要中间结果以支持 Replay/诊断。

HALCON 24.11 多视图立体路线应以校准相机配置和 bounding box 控制为基本约束，具体算法参数必须实机 Benchmark。

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

本指导的具体功能以以下实施规范为准：FS-044。每个 FS-xxx 都包含输入/输出、算法、鲁棒性、性能、UI、测试、验收和禁止实现。
