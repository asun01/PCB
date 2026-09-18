# 从仓库零点启动第一条可运行纵向切片

- 文档 ID：`DEV-PLAN-015`
- 版本：`1.0.0`
- 状态：`ImplementationSpecificationReady`
- 直接依赖：`DEV-PLAN-003`, `DEV-PLAN-006`, `DEV-PLAN-007`, `DEV-PLAN-009`, `DEV-PLAN-010`, `DEV-PLAN-014`, `GATE-001`

## 1. 目标

建立一条最小但真实闭环的“可运行纵向切片”，让团队先验证工程骨架，而不是并行制造大量孤立模块。

## 2. Scope Freeze

进入编码前必须完成范围冻结；任何未注册功能扩展进入 ScopeDrift/Blocker。

## 3. 不可跳步启动顺序

1. 解析 `GATE-001` 中全部适用仓库权威；缺失即阻断。
2. 固定 solution/project/namespace 目录与 Owner。
3. 注册 Contract/Schema/Validator/Test Traceability。
4. 建立 BoardIdentity + EffectiveRuntimeManifest + BoardRun 的最小状态链。
5. 接入一个可 Replay 的视觉/测量计算任务，不接真实硬件 SDK。
6. 形成 Fact → RuleEvaluation → InspectionDecision → Commit → Evidence 的闭环。
7. 建立至少一个 PageContract 驱动的 WPF+DevExpress 页面，并通过八态/UIA/键盘/DPI 验收。
8. 建立失败：缺输入、取消、超时、重启、重复提交、权限拒绝、证据提交失败等路径。
9. 建立 Golden/Replay + Benchmark + Validator 自动门禁。
10. 运行全量独立审计；只有闭环通过后才开始大规模并行开发。

## 4. 纵向切片完成定义

必须证明：

`Observation/Identity → Runtime Manifest → BoardRun → Attempt → Calculation → Fact → RuleEvaluation → InspectionDecision → MachineDecision（仅当 Authority 已获批） → EvidenceCommit/Receipt → Query/UI Projection`

其中任何一步如果属于尚未获批的生产权威，必须停留在 Engineering/Replay/Qualification 用途，不得伪装成 Production。

## 5. AI 工作方式

每个任务先读：Architecture → GATE-001 → Guide → FS → Contract/Schema → State/Owner → PageContract → Test/Benchmark。完成后必须输出：影响文件、接口变化、未知项、验证证据和范围偏移检查。

## 6. 禁止快捷路径

- 不因编译失败降低 Contract；
- 不因测试失败改变 Acceptance；
- 不因 UI 压力绕过 Application/Domain Owner；
- 不用 Stub Result 伪装生产 Fact；
- 不用硬编码默认值替代缺失配置；
- 不在纵向切片成功前建立第二套平行事实/状态/Schema。

## 7. 停止条件

任何缺失的权威资产、字段、Owner、State、AuthorityScope、API、单位、坐标、阈值、测试向量、Golden 数据或验收口径都必须 STOP/Blocker。

停止后只允许做：证据收集、只读分析、ContractCandidate 登记、Mock/Simulator 准备和变更提案；禁止通过临时硬编码、默认值、`latest`、Stub Result 或隐式降级继续生产实现。

## 8. 验收

第一条纵向切片必须完成 Contract → Runtime → Attempt → Calculation → Fact → Decision/Projection → Commit → Evidence → Query/UI 的链路；所有失败/恢复路径有证据；独立审计 PASS 后才能解除大规模并行开发门。

## DoR

GATE-001、Contract/Schema、Owner/State、PageContract、Test/Benchmark 入口必须已解析；目标 API 未验证时只允许只读分析/Mock。

## DoD

纵向切片完成：可回放、可提交、可查询、UI 八态通过、失败/恢复有证据、独立审计 PASS。

## DAG 变化

任何共享 Contract、Owner、Schema Major、Acceptance 或前置变化都触发停止受影响任务并重新计算 DAG。
