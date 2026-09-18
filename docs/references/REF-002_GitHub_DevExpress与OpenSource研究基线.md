# GitHub / DevExpress / Open Source 研究基线
- 文档 ID：`REF-002`
- 版本：`2.0.0`
- 状态：`Reference`
- 日期：2026-09-18
- 用途：实施设计参考，不是本项目权威

## DevExpress WPF

1. DevExpress WPF demos：`https://github.com/DevExpress/demos-wpf`
2. DockLayoutManager + MVVM / IMVVMDockingProperties：`https://github.com/DevExpress-Examples/wpf-docklayoutmanager-use-imvvmdockingproperties-to-build-dock-ui-with-mvvm`
3. LayoutAdapter：`https://github.com/DevExpress-Examples/wpf-docklayoutmanager-use-layoutadapter-to-build-dock-ui-with-mvvm`
4. DocumentManagerService：`https://github.com/DevExpress-Examples/wpf-docklayoutmanager-use-services-that-implement-the-idocumentmanagerservice`
5. 复杂 Dock 布局：`https://github.com/DevExpress-Examples/wpf-docklayoutmanager-create-a-complex-dock-ui`
6. Layout persistence / upgrade：`https://github.com/DevExpress-Examples/wpf-dock-layout-manager-save-and-restore-the-dock-layout-managers-layout`、`https://github.com/DevExpress-Examples/wpf-dock-layout-manager-upgrade-layouts-between-versions`

**吸收原则：** 借鉴工作区/Docking/MVVM/Layout persistence 的工程方法，不复制业务对象；具体 API 必须按实际 DevExpress 版本验证。

## PCB/AOI 开源参考

- `https://github.com/apertus-open-source-cinema/pcb-aoi`：展示了基于参考板图像进行 PCB AOI 原型化的思路；仓库本身为原型、GPL-3.0，不作为本项目代码依赖。
- `https://github.com/PLD-Projects/AOI`：展示 AOI、Qt UI、视觉模型、多线程和外部运动控制的原型组合；用于对照架构关注点，不作为生产实现来源。

## 许可证规则

第三方代码在引入前必须记录 SPDX/许可证、作者/版权要求、二次分发限制、依赖树和安全状态。研究价值不能覆盖 license obligation。
## Research Evidence

所有 GitHub/开源研究结果必须保存 Research Evidence：固定仓库、commit/tag、路径、访问日期、借鉴点与不采用原因。License/许可证、SPDX、版权声明、依赖树和安全状态是引入前的强制门禁。


## Evidence Audit

本页中的每一项外部结论都是研究证据，而非 Asun 的生产标准。进入实现前必须保留证据记录：固定仓库/URL、commit/tag 或文档版本、源码/章节路径、访问日期、许可证/SPDX、依赖和安全状态、借鉴点、未采用原因以及“证据”与“本项目事实”的边界。


边界：外部研究内容**不作为本项目生产实现权威**，不得覆盖内部 Contract/Schema/Qualification 规则。
