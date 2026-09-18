# 变更影响分析与追踪标准
- 文档 ID：`DEV-GOV-013`
- 版本：`1.0.0`
- 状态：`Normative`

## 1. 任何变更都必须回答

```text
Change
→ Why
→ Affected Contract / Schema
→ Affected State / Owner
→ Affected FunctionSpecs
→ Affected PageContracts
→ Affected Tests / Golden / Benchmark
→ Affected Qualification
→ Migration / Compatibility
→ Release / Manifest
```

## 2. AI/人工禁止的“顺手修改”

不能因为编译失败、测试失败、UI 绑定不便、性能不足或第三方 API 不熟悉而顺手修改无关 Contract、领域边界、生产状态、数据库结构或 UI 交互语义。

## 3. 文件范围冻结

每个任务必须先生成 `FilesToChange`。实际 diff 超过清单即自动标记 `ScopeDrift`，没有批准的扩展不得合并。

## 4. 兼容策略

Schema/Contract 变更必须明确 Major/Minor/Patch 影响、消费者兼容范围、旧版本读取策略和回滚方式。禁止通过隐式字段、字符串或“向下兼容”口号绕过契约审查。
