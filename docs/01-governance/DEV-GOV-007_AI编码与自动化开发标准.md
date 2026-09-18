# AI 编码与自动化开发标准
- 文档 ID：`DEV-GOV-007`
- 版本：`1.0.0-prep`
- 状态：`ImplementationSpecificationReady`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 适用范围：Codex、Copilot、内部 Agent 及人工开发
- 直接依赖：`DEV-GOV-001`, `DEV-GOV-004`
- 主要输出：AI 上下文装载顺序、禁止猜测、验证闭环

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

## 1. AI 开发前必须装载

```text
架构基线
 → 本模块 Development Guide
 → Contract/Schema
 → State/Owner
 → 依赖模块 Development Guide
 → PageContract
 → Golden Sample / Test
 → 当前仓库代码
```

## 2. AI 不得猜测

- 不猜 HALCON 算子名、参数和返回类型；必须查当前目标版本。
- 不猜 AsunImage API；必须使用批准公共边界。
- 不猜 DevExpress API；需要核对已安装版本和本地示例/文档。
- 不创建架构中不存在的 Universal 对象。
- 不因编译失败擅自改变 Contract。

## 3. 每个 AI 任务输出

必须同时给出：修改文件、实现边界、使用的 Contract、测试、剩余风险、未验证项。对于视觉算法任务，附算法选择理由、Fallback 和性能测量计划。


## 实施级补充

本包中的 `12-function-specs/FS-*.md` 是本治理规则的实施执行层。开发人员/AI 不得只阅读本治理文件就开始编码，必须继续读取对应 FS、ContractCandidate、State/Owner 和测试规范。

## 5. 防跑偏硬规则

- AI 的任务边界由 `FunctionSpec + TaskCard + FilesToChange` 冻结；不得主动扩大需求。
- 编译错误不是授权；测试失败不是授权；第三方 API 变化不是授权。任何解决方案都必须回到权威 Contract/Guide 并做影响分析。
- 任何“缺字段/缺参数/缺接口/缺状态”的地方都必须写入 `Blocker`，不得补一个“合理值”。
- AI 不得创建与现有事实模型同义的新类、新表、新状态、新事件或第二个 Producer。
- 任务结束必须逐项证明：实现路径、测试路径、证据路径、性能路径和禁止路径均已闭环。
