# DevExpress 高阶能力使用矩阵
- 文档 ID：`DEV-UI-046`
- 版本：`2.0.0`
- 状态：`Normative`
- 技术基线：WPF + DevExpress

## 1. 目标

UI 标准不允许“把 WPF Button 换成 DevExpress Button”作为完成标准。必须从工作台、信息密度、数据操作、布局持久化、MVVM、批量操作和上下文交互层面使用 DevExpress 的强项。

## 2. Capability → UI responsibility

| DevExpress capability | Asun usage | 禁止用途 |
|---|---|---|
| DockLayoutManager | 主工作台 docking/document/panel 管理 | 承载业务状态 |
| LayoutGroup/LayoutPanel/DocumentGroup | 构建工程化 workspace | 用大量绝对坐标堆页面 |
| GridControl/TreeList | 结果、目标树、批量操作、虚拟化数据展示 | 作为事实 Store |
| Editors | 参数编辑、验证状态、单位/范围呈现 | 绕过 Contract 的自由字段 |
| MVVM support | Command/Query/Service wiring | ViewModel 直接写 Store |
| Layout persistence | 用户/角色布局偏好 | 恢复布局改变生产配置 |
| Contextual panels | Inspector/Evidence/Diagnostics | 隐藏关键安全状态 |

DevExpress 当前文档说明 DockLayoutManager 支持 docking、layout 保存/恢复以及 MVVM；官方 GitHub 示例展示 `IMVVMDockingProperties`、`LayoutAdapter`、复杂 Dock UI、浮动/自动隐藏和 Visual Studio 类工作区，可作为实现研究输入。

## 3. UI performance rule

任何高成本图像/3D/数据库计算不得在 UI thread 执行。Grid/TreeList 等大数据展示使用分页/虚拟化/增量查询策略；viewport 与证据加载必须可取消。

## 4. Visual design rule

“炫技”来自信息结构和交互效率：状态可视化、上下文联动、证据追踪、批量命令、异常聚焦、布局持久化和高质量 feedback，而不是颜色、动画或控件数量堆叠。
