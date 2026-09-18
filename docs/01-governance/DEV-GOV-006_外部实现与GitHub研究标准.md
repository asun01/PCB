# 外部资料、GitHub 与第三方实现研究标准
- 文档 ID：`DEV-GOV-006`
- 版本：`1.0.0-prep`
- 状态：`ImplementationSpecificationReady`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 适用范围：所有重要公共能力和新算法选型
- 直接依赖：`ARCH-PCBA-VISION-001`
- 主要输出：研究流程、许可证、安全、维护性与吸收规则

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

## 1. 研究优先级

```text
官方技术文档/Reference Manual
 → 官方示例
 → 行业/论文
 → GitHub/开源工程
 → 二手文章
```

GitHub 只能提供实现证据和工程经验，不能把其领域模型、性能和测试结论直接变成本项目事实。许可证必须核对；公开代码是否可用取决于许可证以及项目的分发模式。参考：GitHub 关于复用他人代码的说明 https://docs.github.com/en/get-started/learning-to-code/reusing-other-peoples-code-in-your-projects。

## 2. 研究记录格式

| 项 | 内容 |
|---|---|
| Repository / URL | 精确定位 |
| License | 原始许可证 |
| Activity | 最近维护情况 |
| Architecture | 可借鉴结构 |
| Algorithm | 可借鉴算法/参数 |
| Tests | 是否有可复用测试思想 |
| Risks | 许可证/安全/维护风险 |
| Decision | 采用/借鉴/拒绝 |

## 3. 当前基线参考

- OpenPnP：值得研究其 Board/Panel/Job/Part/Package 分层、CAD 数据导入与操作员工作流；其文档把 Board 定义为独立于机器的设计/装配语义，并支持从 CAD 导入 placement 数据。见 https://github.com/openpnp/openpnp/wiki/User-Manual。
- OpenPnP 校准文档：可借鉴“先验证设备本身，再进入软件校准”的工程分层思想。见 https://github.com/openpnp/openpnp/wiki/Setup-and-Calibration_Before-You-Start。
- DevExpress WPF Demos：重点研究 Docking、Grid、Layout、MVVM、Editors、Navigation 等真实示例；见 https://github.com/DevExpress/demos-wpf。
- DevExpress Docking 示例：可用于高保真工作台布局研究，不得直接复制为 Asun 领域模型。见 https://github.com/DevExpress-Examples/wpf-docklayoutmanager-create-a-simple-layout-of-dock-panes。


## 实施级补充

本包中的 `12-function-specs/FS-*.md` 是本治理规则的实施执行层。开发人员/AI 不得只阅读本治理文件就开始编码，必须继续读取对应 FS、ContractCandidate、State/Owner 和测试规范。
