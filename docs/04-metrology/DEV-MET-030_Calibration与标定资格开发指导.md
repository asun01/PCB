# Calibration与标定资格开发指导
- 文档 ID：`DEV-MET-030`
- 版本：`1.0.0-prep`
- 状态：`ImplementationSpecificationReady`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 适用范围：相机/镜头/畸变/比例/3D/运动/环境条件
- 直接依赖：`DEV-VIS-020`, `DEV-GOV-008`
- 主要输出：CalibrationProfile、Artifact、Validity、Qualification

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

## 1. 核心数据链

```text
MeasurementSpec
  → MeasurementPlan（执行投影）
  → RawValue
  → CanonicalMeasurementValue
  → MeasurementQuality / Uncertainty
  → ToleranceEvaluation
  → FeatureMeasurement
  → MeasurementSet
  → Board/Panel Summary
  → Statistical Dataset
```

`DisplayValue` 只服务 UI，不得反向成为计算输入。

## 2. 异常点处理

异常点必须记录原因和策略：未观察、明显污染、遮挡、拟合残差超阈、域失效、信号饱和等。不得用“去掉异常点”作为无条件优化；剔除规则必须预先定义并可回放。

## 3. 不确定度

计量结果必须能解释主要来源：像素量化、标定、温漂、镜头畸变、姿态、拟合残差、重复性、设备差异。参数变化必须能重新计算资格。

## 4. 多 FOV

跨 FOV 结果必须引用每个 FOV 的有效资格和变换质量。任何“拼接看起来连续”都不足以证明计量一致性。


## 实施级关联功能

本指导的具体功能以以下实施规范为准：FS-016。每个 FS-xxx 都包含输入/输出、算法、鲁棒性、性能、UI、测试、验收和禁止实现。
