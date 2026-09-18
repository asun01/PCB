# AI确定性实施与来源锁定标准
- 文档 ID：`DEV-GOV-015`
- 版本：`1.0.0`
- 状态：`Normative`
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`
- 目的：在现有禁止推断规则之上，进一步防止 AI 在代码实施时自行补全、改写、降级或扩张既定设计。

## 1. 唯一实施来源链

AI 每个任务只能从以下链路获取实施事实：

```text
Current Manifest / Registry
→ Current Architecture / Implementation Binding
→ Contract / Schema
→ State / Owner / Authority
→ Development Guide
→ FunctionSpec / PageContract
→ Code Task
→ Validator / Test / Golden / Benchmark
```

历史审计、旧版本 Master、外部 GitHub、模型记忆、搜索结果和“常见做法”只能作为研究输入，不能覆盖当前权威。

## 2. 决策四值模型

每个关键实施决策必须处于四种状态之一：

- `FROZEN`：当前权威已经明确规定；AI 必须照做；
- `ALLOWED_SET`：权威给出有限候选集；AI 只能在候选集内选择，并记录选择依据；
- `UNVERIFIED`：外部 API/目标环境/资格证据尚未验证；AI 可以实现隔离的接口、Mock、Adapter skeleton，但不得假定具体实现事实；
- `BLOCKED`：缺少语义、Contract、Owner、字段、单位、坐标、阈值、状态、权限或必要证据；不得继续进入该决定所依赖的代码。

## 3. 禁止的“看似合理”行为

AI 不得：

1. 将编译错误解释为需求错误并擅自改需求；
2. 将测试失败解释为验收标准错误并擅自降低测试；
3. 因字段未定义而添加同名/近义字段；
4. 因参数缺失而写默认常数；
5. 因 API 名称不确定而猜算子、猜枚举、猜 SDK 方法；
6. 因 UI 不方便而绕过 Application Port、Owner 或权限；
7. 因性能不足而擅自改变算法语义、采样口径、分母或判定规则；
8. 因异常难处理而把 `Unknown/Invalid/NotQualified/Blocked` 转成 `OK`；
9. 因模块复用方便而把领域语义上移到共享层；
10. 因某个任务“顺手”修改另一功能的 Contract/Schema/State/Owner。

## 4. 变更协议

发现当前规范不足时，AI 必须先生成“变更候选”，说明：原规则、问题证据、影响范围、建议新规则、受影响 FS/Page/Schema/Test，然后停止当前超范围实现。只有获批变更进入当前 Registry 后，代码任务才重新开放。

## 5. 输出要求

每次 AI 代码任务结束必须报告：TaskId、FunctionSpecId、实际变更文件、遵循的权威资产、未改动的约束、测试结果、Benchmark 状态、未验证项、Open Gate 和 `ScopeDrift`。


## 来源优先级（不可自行改变）

1. 已批准 Contract/Schema/State/Owner 权威；
2. 当前 Development Guide / FunctionSpec / PageContract；
3. 当前代码仓库的已验证实现（用于识别现状，不得覆盖规范）；
4. 官方 HALCON/DevExpress/AsunImage 目标版本资料；
5. 固定 commit/tag 的外部研究证据；
6. 一般工程知识只能解释，不得填补项目事实。

当两个来源冲突时，Agent 必须停止并登记 Conflict/Change Request；不得自行选择“更合理”的一方。

## 测试与验收保护

AI 不得仅因为实现失败而修改测试断言、AcceptanceProfile、阈值、采样、覆盖、分母、Owner 或错误语义。任何修改都必须通过变更控制重新生成受影响的 FS/Schema/Test/Manifest 并重新审计。
