# 质疑式反向审计与无已知缺口标准
- 文档 ID：`DEV-GOV-011`
- 版本：`2.0.0`
- 状态：`Normative`

每轮审计必须从以下相反方向重新问一次，而不是只检查“有没有写”：

1. **开发者反问**：如果只有这份 FS 和关联 Contract，我还能猜什么？所有可猜项都应被明确。
2. **AI 反问**：是否存在多个合理实现路径而文档没有规定优先级/选择条件？若存在，必须增加 Decision Rule 或 Candidate Benchmark。
3. **测试反问**：什么情况下实现看起来正常但实际上错误？必须增加 negative/edge/recovery cases。
4. **性能反问**：平均值是否掩盖 P99、内存峰值、队列堆积或 UI 卡顿？必须给出 budget/profile。
5. **质量反问**：Unknown、Invalid、NotApplicable、NoEvidence 是否可能被转换为 OK？必须显式禁止。
6. **权威反问**：有没有第二个 Producer/Store/Owner？必须显式禁止。
7. **操作员反问**：完成任务是否需要重复输入系统已经知道的信息？应自动带出并只让操作员处理例外。
8. **维护者反问**：换算法/换相机/升级库/重启后，旧事实是否仍可解释？必须有 version/hash/replay。

只有连续一轮没有新的**已知文档缺口**，且自动审计为零错误、关键人工复核项已记录，才能标记 `DocumentationReady`。
