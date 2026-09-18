# HALCON 与外部资料反幻觉审计
- 文档 ID：`AUD-065`
- 版本：`1.0.0`
- 状态：`PASS_WITH_EXTERNAL_GATES`
- 审计基线：`ARCH-PCBA-VISION-001 v0.5.3` + `Asun Vision Development Standard v2.1.5`
- 范围：除硬件具体 SDK 外的全量开发准备文档、开发步骤、FunctionSpec、PageContract、工具、权威绑定和语义链路

## 1. HALCON 规则

当前标准固定目标 HALCON 24.11 family；官方 Reference 只用于确认 Operator/API 的存在、签名、输入输出和版本语义。默认/备选路线必须由 Golden Dataset + Benchmark 决定，不允许把官方存在性等同于“本项目最优”。

## 2. 已建立的官方入口

`find_shape_model`、`fit_ellipse_contour_xld`、`dyn_threshold`、`xyz_to_object_model_3d`、`reconstruct_surface_stereo`、`register_object_model_3d_global` 均在 `REF-003` 与 HALCON Decision Matrix 有登记；目标机器安装版本不同则必须重新验证。

## 3. 外部代码

GitHub、论文和第三方项目均为 Research Evidence；必须记录固定提交/版本、许可证、路径、借鉴点、安全风险和不复制原因。外部实现不能改变 Asun Contract、Owner、State、Acceptance。

## 4. API 不确定性

任何没有官方目标版本文档或真实仓库 API 证据支持的名称、参数、类型都必须标为 `Unverified`；Agent 不得编造。
