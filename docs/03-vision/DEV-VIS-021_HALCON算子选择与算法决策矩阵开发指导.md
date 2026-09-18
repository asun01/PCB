# HALCON算子选择与算法决策矩阵开发指导
- 文档 ID：`DEV-VIS-021`
- 版本：`1.0.0-prep`
- 状态：`ImplementationSpecificationReady`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 适用范围：HALCON 算子候选、鲁棒性、性能、Fallback、版本验证
- 直接依赖：`DEV-GOV-006`
- 主要输出：Operator Decision Matrix + Golden Dataset + Benchmark

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

## 1. 通用算法选择流程

```text
问题定义
 → 可观察边界
 → 输入质量检查
 → 候选算法集合
 → 默认算法
 → 参数初始化
 → 结果质量评分
 → Fallback / Reject
 → 性能 Benchmark
 → Golden Sample 验证
```

任何算法输出都必须带最小质量元数据，例如有效像素比例、残差、score/置信度、异常计数、输入质量等级和算法版本。

## 2. HALCON 使用规则

- HALCON 仅在 Vision Contract 边界内出现；领域代码不得持有 `HObject`/HALCON 原生 handle。
- 使用当前目标版本 Reference Manual 核对算子、参数、线程属性和内存语义。
- 优先验证 MVTec 官方示例，再形成本项目 Benchmark。
- 需要长期缓存的 handle 必须有明确生命周期，不在全局静态对象中隐藏持有。

## 3. 鲁棒性要求

针对照明、反光、污渍、噪声、局部缺损、轻微变形、批次差异分别建立 Golden 变体。算法必须定义“无法可靠判断”的拒识路径，而不是无限调参数。

## 4. 性能要求

默认优先：ROI 限制 → 降采样/金字塔 → 低成本候选 → 高成本精测 → 仅必要时全分辨率。把昂贵算法放在候选集缩减之后。大图采用 tile/pyramid/LOD；3D 重建限制有效区域。

## 5. 已核对的 HALCON 24.11 算子证据（不是最终参数标准）

- `find_shape_model`：二维形状模型匹配；目标版本官方参考 https://www.mvtec.com/doc/halcon/2411/en/find_shape_model.html。
- `edges_sub_pix`：亚像素边缘提取；MVTec 24.11 官方示例在 `fit_ellipse_contour_xld` 页面中直接展示了其典型用法 https://www.mvtec.com/doc/halcon/2411/en/fit_ellipse_contour_xld.html。
- `fit_ellipse_contour_xld`：XLD 椭圆拟合；参数和示例必须按目标版本文档核对 https://www.mvtec.com/doc/halcon/2411/en/fit_ellipse_contour_xld.html。
- `dyn_threshold`：局部阈值分割；作为局部亮暗不均场景的候选路线，需与全局 threshold 等方案用 Golden Dataset Benchmark 决策 https://www.mvtec.com/doc/halcon/2411/en/dyn_threshold.html。
- `xyz_to_object_model_3d`：从 X/Y/Z 图像构建 3D object model；24.11 Reference 明确其输入图像大小需一致且只使用三者交集域 https://www.mvtec.com/doc/halcon/2411/en/xyz_to_object_model_3d.html。
- `reconstruct_surface_stereo`：24.11 多视图立体重建；官方说明强调校准 setup 是精确重建的主要前提，并建议用紧 bounding box 限制重建范围以降低运行时间 https://www.mvtec.com/doc/halcon/2411/en/toc_3dreconstruction_multiviewstereo.html。
- `register_object_model_3d_global`：基于重叠区域优化多个 3D object models 的相对位置；必须同时记录 registration score/residual https://www.mvtec.com/doc/halcon/2411/de/register_object_model_3d_global.html。

以上只证明“算子存在和用途”，不证明它在某一具体产品上最优。最终选择必须由设备样本和 Benchmark 关闭。


## 实施级关联功能

本指导的具体功能以以下实施规范为准：FS-007。每个 FS-xxx 都包含输入/输出、算法、鲁棒性、性能、UI、测试、验收和禁止实现。
