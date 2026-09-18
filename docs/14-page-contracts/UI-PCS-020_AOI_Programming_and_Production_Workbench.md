# AOI Programming and Production Workbench
- 文档 ID：`UI-PCS-020`
- 版本：`2.0.0`
- 状态：`ImplementationSpecificationReady`
- 技术路线：WPF + DevExpress
- Scope：CAD/PnP target generation, component/pin/polarity/OCR/solder joint and review

## 1. Information architecture

首屏必须同时让操作员回答：当前产品/板/工单、当前状态、当前对象、当前结果、下一动作。页面采用 `DockLayoutManager` + document workspace + contextual inspectors；布局本身不得成为业务状态。

## 2. Primary regions

```text
Command/Ribbon
  → Context Header
  → Main Document/Viewport
  → Target/Result Navigation
  → Inspector / Evidence / Timeline
  → Status / Recovery
```

## 3. DevExpress usage rule

必须优先发挥 DevExpress 的工作台能力：Docking/Document、Layout、Grid/TreeList、Editors、MVVM/commands、layout persistence、contextual panels；禁止仅做原生 WPF 控件同名替换。DevExpress 官方文档明确支持 DockLayoutManager 的 docking、layout 保存恢复与 MVVM；官方示例也展示复杂 Dock UI、LayoutAdapter、MVVM docking 和 Visual Studio 类工作台结构。

## 4. Eight states

页面必须定义：Ready / Busy / Empty / Stale / Blocked / RecoverableError / PermissionDenied / Completed。每种状态都有可执行下一动作、AutomationId、键盘行为和证据入口。

## 5. Command model

所有生产配置/判定/处置动作通过 Application Port；批量操作必须先显示影响范围与跳过对象；危险/不可逆动作必须二次确认并展示 version/hash/authority。

## 6. Visual interaction

2D/3D viewport 不持有 HALCON/AsunImage 原生对象；使用 immutable frame/evidence/overlay DTO。大图采用 tile/pyramid/LOD；ROI 操作必须有 hover/active/committed 三态，拖动过程不触发生产副作用。

## 7. Operator simplification

系统自动带出已知信息、自动定位对象、自动预选可用方法、自动汇总异常；操作员优先处理 exceptions，而不是重复输入系统已知参数。高级参数渐进披露，工程参数与生产常用参数分层。

## 8. Keyboard/UIA/Localization/DPI

定义稳定 AutomationId；Review 至少支持 next/previous/accept/reject/defer；Program/Recipe 编辑支持 focus order 和 batch apply；真实 WPF 环境验证 DPI、三语、最小窗口尺寸和 UIA。

## 9. Acceptance

- 页面不拥有生产事实；
- 所有关键状态均可测试；
- 所有关键 Command 有权限和审计；
- 视觉证据可从结果反查，结果可反查目标；
- 复杂工作台可保存/恢复布局，但布局恢复不能改变业务状态；
- UI 性能必须绑定 PerformanceProfile。

## 9. Page-specific freeze

**页面角色：** `AOI Programming and Production Workbench`

### 关键 Command

`ImportCAD` · `ImportPnP` · `GenerateTargets` · `OpenComponent` · `ConfigureInspection` · `RunBoard` · `ReviewCandidate` · `SubmitApproval`

上述 Command 只定义 UI 意图；实际权限、Owner 和副作用必须由 Application Port/State-Owner Contract 决定。

### 关键 Query / 对象

`PackageLibrary` · `ComponentTarget` · `InspectionProgram` · `ReviewClaim`

### 页面硬边界

['Recipe/Program and review semantics are separate']

### 页面完成条件

- 每个关键 Command 都能映射到 Application Port、Permission 和审计事件；
- 每个关键 Query 都能定位到唯一事实/投影来源；
- 八态都有页面特定的下一动作；
- AutomationId、键盘、DPI、本地化、异常恢复和 UIA 均有可执行测试；
- 页面无法通过视觉绑定、ViewModel 或布局恢复直接修改生产事实。
