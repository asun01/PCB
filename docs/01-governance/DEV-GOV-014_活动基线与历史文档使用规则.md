# 活动基线与历史文档使用规则
- 文档 ID：`DEV-GOV-014`
- 版本：`1.0.0`
- 状态：`Normative`
- 适用范围：所有人类/AI研发活动、文档导航、自动审计

## 1. 当前权威活动基线

当前活动文档集合由 `11-tooling/document-registry-v2.json` 定义；当前发布包必须同时满足：活动 Markdown 数量、唯一 Document ID、FunctionSpec、PageContract 与 Manifest 一致。

`98-history/` 下文件只用于审计追溯，不属于当前实施语义权威。AI 不得把历史审计数字、旧版流程、旧版 Registry 或旧版 Manifest 当作当前事实。

## 2. 当前读取优先级

```text
Current Manifest / Registry
→ current Architecture binding
→ current Contract/Schema
→ current State/Owner
→ current Development Guide
→ current FunctionSpec / PageContract
→ current Test/Benchmark
→ historical audit (only for change history)
```

## 3. 版本冲突

当同一主题存在多个版本：使用当前 Registry 标记的活动资产；所有历史版本均视为背景证据。任何无法确定当前版本的任务必须停止并登记 `Blocker`。

## 4. 允许的历史用途

历史文档只允许用于：理解为什么发生变更、回归审计、比较差异、追踪缺陷。不得从历史文档复制字段、状态、算法、路径或阈值到生产实现，除非当前权威资产重新批准。
