# 开发指导体系总则
- 文档 ID：`DEV-GOV-001`
- 版本：`1.0.0-prep`
- 状态：`ImplementationSpecificationReady`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 适用范围：全平台所有实施活动
- 直接依赖：`ARCH-PCBA-VISION-001`
- 主要输出：统一开发入口、文档层级、AI 可执行标准和变更门禁

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

## 1. 目标

本体系把“架构设计”转化为“可执行研发标准”。研发人员或 AI 在开始写代码前，应能从本包定位：功能属于哪个领域、依赖哪个 Contract、由谁拥有、允许哪些实现路径、如何验证、何时可以进入真实设备。

## 2. 文档层级

```text
L0 总体架构
  ↓
L1 开发总则 / 平台标准
  ↓
L2 能力开发指导（一个稳定能力一个文档）
  ↓
L3 产品/领域开发指导
  ↓
L4 PageContract / Schema / Validator / Test / Qualification
```

L2 不按按钮拆文档；按“独立语义 + 独立算法链 + 独立参数 + 独立性能 + 独立验收 + 可复用边界”划分。一个模块如果仍高度依赖某个工艺的特殊默认值，应留在领域层，而不是强行公共化。

## 3. 每份开发指导的最低内容

- Purpose / Scope / Non-goals
- 架构位置与输入输出
- Authority / Owner / Producer / Consumer
- 依赖 Contract/Schema/PageContract
- 正常路径、异常路径、取消、超时、恢复
- 参数及自动初始化规则
- 默认算法、Fallback、拒识条件
- HALCON/API 版本验证点
- 性能预算与 Benchmark 方法
- UI 工作流和高保真交互要求
- 日志、审计、证据与回放
- Unit / Integration / HIL / Qualification
- Acceptance Criteria
- 禁止实现方式
- 外部资料与许可证审查

## 4. Definition of Ready for Coding

只有以下条件全部满足，工作项才能进入 `ImplementationReady`：

| 门 | 必须有 | 不满足时 |
|---|---|---|
| Contract | 已存在或已登记并批准 | 不得实现新持久化语义 |
| Owner | 唯一 State/Commit/Qualification Owner 已解析 | 阻断 |
| Consumer | 至少一个真实消费者或获批测试消费者 | 阻断 |
| Algorithm | 默认/Fallback/拒识有边界 | 阻断 |
| UI | 页面命令、状态、权限和 AutomationId 已定义 | 页面能力阻断 |
| Test | 至少单元+集成验证路径 | 阻断 |
| Performance | Profile 或明确 Pending 条件 | 可以延期实测，但不能无预算编码 |
| Evidence | 运行事实如何提交已有定义 | 阻断生产链 |


## 实施级补充

本包中的 `12-function-specs/FS-*.md` 是本治理规则的实施执行层。开发人员/AI 不得只阅读本治理文件就开始编码，必须继续读取对应 FS、ContractCandidate、State/Owner 和测试规范。
