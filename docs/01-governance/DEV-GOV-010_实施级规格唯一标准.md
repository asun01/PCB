# 实施级规格唯一标准
- 文档 ID：`DEV-GOV-010`
- 版本：`2.0.0`
- 状态：`Normative`

## 1. 目标

本标准规定：任何进入代码实现的功能必须存在一份可执行的 `Function Implementation Specification (FS-xxx)` 或等价领域专项指导；它是实施语义的唯一入口之一，不能由口头规则、代码注释、AI 临时推断取代。

## 2. 五层权威

```text
Architecture → Contract/Schema → State/Owner → Development Guide / FS → Test/Qualification
```

- Architecture：边界、职责、关系。
- Contract/Schema：数据结构、兼容和传输/持久化形状。
- State/Owner：谁可以读/写/提交/激活。
- Development Guide/FS：算法、流程、参数、UI、性能、异常、代码任务切片。
- Test/Qualification：证明实现满足前述约束。

下层不得反向定义上层事实；发现冲突必须阻断并提交 ADR/Contract 变更。

## 3. Implementation Ready 的硬条件

必须同时具备：FunctionSpec、Guide、ContractCandidate/approved Contract、输入输出语义、失败路径、Owner、代码落点、Test plan、Benchmark plan、UI PageContract（如适用）、Acceptance Criteria、Prohibited Paths。

缺任一项只能是 `PreparationIncomplete`，不得标成 `ImplementationSpecificationReady`。

## 4. “完整”定义

“文档完整”仅表示所有当前架构范围内的可决定工程语义已经被明确；不等价于真实硬件性能、视觉准确率、计量资格、HIL/FAT/SAT 或生产资格。后者必须有外部证据门。
