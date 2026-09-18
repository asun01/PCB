# Asun Vision Platform 当前仓库必需权威资产与解析门禁

- 文档 ID：`GATE-001`
- 版本：`1.0.0`
- 状态：`Normative`
- 直接依赖：`DEV-GOV-002`, `DEV-GOV-004`, `DEV-GOV-015`
- 适用范围：当前开发标准包未复制、但架构明确要求真实仓库提供的权威资产

> **规则：本文件是缺失权威来源的显式门禁，不是第二套 Schema/Owner/State 真相。任何未解析的权威资产均为 Blocker/Unverified；禁止 AI 根据记忆补写。**

## 1. 必须由实际代码仓库解析的权威资产

| 标识 | 预期路径/来源 | 权威职责 | 本包处理方式 | 缺失时动作 |
|---|---|---|---|---|
| `DEV-ARC-005` | `docs/40-development/00-standards/权威状态角色消息与判定所有权规范.md` | BoardRun、Decision、Review、Disposition、Owner、状态、CAS/Receipt | 只引用，不复制 | Blocker；不得自行定义 Owner/State |
| `DEV-PLT-001` | 仓库级共享平台开发标准（实际仓库路径为准） | 平台模块边界、公共能力入口与实现约束 | 只引用 | Blocker；登记 Repository Authority Gap |
| `DEV-PLT-002` | 仓库级模块独立性与复用标准 | 五边界与八维独立性判断 | 只引用 | Blocker；不得自行决定拆分尺度 |
| `DEV-DAT-002` | 仓库级维护健康、备份、升级与恢复指导 | RPO/RTO、密钥、恢复演练、升级/回退 | 只引用 | 对运维/灾备任务为 Blocker |
| `contracts/v1/schema/events-catalog.schema.json` | 实际仓库 | Event Envelope、实体/事件身份等 | 不复制 | 跨模块事件任务 Blocker |
| `contracts/v1/catalog/events.v1.json` | 实际仓库 | 事件枚举/载荷目录 | 不复制 | 事件任务 Blocker |
| `contracts/v1/schema/cross-module-flow-registry.schema.json` | 实际仓库 | 跨模块流程、兼容、去重、乱序、断点/回执 | 不复制 | 跨模块任务 Blocker |
| `contracts/v1/schema/recipe-manifest.schema.json` | 实际仓库 | Recipe/Runtime Manifest 结构 | 不复制 | Recipe/Runtime 任务 Blocker |
| `contracts/v1/schema/evidence-manifest.schema.json` | 实际仓库 | Evidence Manifest / Commit 边界 | 不复制 | Evidence/Commit 任务 Blocker |
| `contracts/v1/schema/plugin-manifest.schema.json` | 实际仓库 | Plugin capability/dependency/permission/signature | 不复制 | Plugin 任务 Blocker |

## 2. 解析顺序

```text
Repository checkout
  → exact path lookup
  → version/hash capture
  → authority/consumer/validator resolution
  → task admission
```

任何 Agent 在代码修改前都必须记录：`AuthorityPath + 版本/Commit + Hash + ReadRange + Applicability`。

## 3. 禁止行为

- 不得新建与上述资产同义的第二套 Schema、State、Owner Registry 或 Event Catalog；
- 不得因为外部仓库暂时缺少文件而创造“临时兼容实现”并让其进入生产语义；
- 编译错误不能授权修改 Authority；
- 测试失败不能授权改变 Acceptance；
- 第三方代码不能成为本项目事实来源。

## 4. Implementation Gate

相关任务进入 `ImplementationReady` 前，必须把所需仓库权威资产解析结果挂入任务 Traceability；解析失败即 `BLOCKED_MISSING_AUTHORITY`。
