# Device/Station — PageContract 实施规范
- 文档 ID：`UI-PCS-010`
- 版本：`1.1.0`
- 状态：`ImplementationSpecificationReady`
- 技术路线：WPF + DevExpress
- 架构基线：`ARCH-PCBA-VISION-001 v0.5.3`

## 1. 页面职责

设备。输入：`Capability/DeviceState`；输出/投影：`ready/alarm/lease/health`。页面不是事实权威。

## 2. 工作台布局

`station dashboard + diagnostics + capability matrix`

推荐利用 DevExpress Docking/Layout/Document/TreeList/Grid/Editors/MVVM/Command/Workspace 能力，而不是把 WPF 原生控件逐一替换为同名控件。

## 3. 八态

`Ready / Busy / Empty / Stale / Blocked / RecoverableError / PermissionDenied / Completed` 必须分别定义：显示内容、允许的 Command、恢复动作、AutomationId、审计需求。

## 4. Command 规则

- 所有会改变生产配置/质量状态的 Command 必须进入 Application Port；
- UI 只能发送 Command，不直接修改 Store；
- 批量动作必须显示影响对象数、跳过对象和失败原因；
- 发布/回滚必须显示版本/hash/依赖差异；
- 阻断必须能从 UI 一键跳到证据/原因。

## 5. 高性能视觉交互

- 2D/3D viewport 只消费不可变 frame/overlay DTO；
- 大图使用 Tile/Pyramid/LOD + visible-region cache；
- 叠加层按 z-order 和 selection state 管理；
- 交互线程不执行高成本 HALCON；
- 拖动 ROI 时先本地预览，提交后进入受控 Command；
- 选中缺陷/量测时必须能同步定位到原始证据。

## 6. 键盘与人因

高频任务必须支持键盘完成；Review 支持 next/previous/accept/reject/defer；Program 编辑支持 focus order 和 batch apply；每个快捷键必须有冲突检测。

## 7. UI Automation

关键控件必须有稳定 AutomationId；八态、异常、关键 Command、结果单元格都需要 UIA 验证。三语/DPI/最小窗口尺寸必须在真实 WPF 环境验收。

## 8. 安全

权限校验在 Application/Domain 层重复执行；UI 隐藏按钮不是权限控制。PermissionDenied 页面必须解释“需要什么权限/为何被拒绝”，但不得泄露不应看的客户数据。

## 9. Acceptance

- 首屏能在 3 秒内让受训操作员回答“当前对象/当前状态/结果/下一步”；
- 正常/Busy/Blocked/Error 均有可执行下一动作；
- 视觉/3D操作无明显拖动卡顿，具体帧率/延迟以目标 PerformanceProfile 为准；
- AutomationId、键盘、DPI、本地化、恢复、权限全部有测试证据；
- 不存在 UI 对生产事实的第二写入口。

## 9. Page-specific freeze

**页面角色：** `Device Station`

### 关键 Command

`OpenStation` · `ReadCapability` · `Acquire` · `MoveRequest` · `ResetRequest` · `ShowHealth`

上述 Command 只定义 UI 意图；实际权限、Owner 和副作用必须由 Application Port/State-Owner Contract 决定。

### 关键 Query / 对象

`DeviceCapability` · `StationState` · `Lease` · `Health`

### 页面硬边界

['No direct safety writes; device actions only via approved ports']

### 页面完成条件

- 每个关键 Command 都能映射到 Application Port、Permission 和审计事件；
- 每个关键 Query 都能定位到唯一事实/投影来源；
- 八态都有页面特定的下一动作；
- AutomationId、键盘、DPI、本地化、异常恢复和 UIA 均有可执行测试；
- 页面无法通过视觉绑定、ViewModel 或布局恢复直接修改生产事实。
