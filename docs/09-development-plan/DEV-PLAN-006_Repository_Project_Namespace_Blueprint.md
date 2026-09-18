# Repository Project / Namespace Blueprint
- 文档 ID：`DEV-PLAN-006`
- 版本：`1.1.0`
- 状态：`ImplementationSpecificationReady`
- 技术基线：C# + .NET 10 + WPF + DevExpress + HALCON 24.11 + AsunImage

## 1. 总体依赖方向

```text
Apps / Product Shells
  → Application / Orchestration
  → Domain
  → Platform Contracts / Facts / Evidence
  → Vision / Metrology / Device Ports
  → AsunImage Adapter
  → HALCON / Vendor SDK
```

UI 只能向下依赖 Application Ports / DTO / Render Contracts，不允许 UI → Domain Store 直连。

## 2. 建议程序集边界

| 程序集/项目 | 责任 | 禁止依赖 |
|---|---|---|
| Asun.Platform.Contracts | Schema-generated/contract DTO | UI/HALCON |
| Asun.Platform.Core | lifecycle/identity/common semantics | WPF/HALCON |
| Asun.Platform.Evidence | Evidence/CAS/receipt | UI |
| Asun.Vision.Contracts | vision request/result/quality | WPF |
| Asun.Vision.Halcon | HALCON adapter/implementation | Domain UI |
| Asun.Metrology.Core | measurement/uncertainty/coverage | WPF/HALCON native |
| Asun.Device.Contracts | device ports/capabilities | WPF |
| Asun.Device.Impl | camera/light/motion/PLC adapters | Domain semantics |
| Asun.UI.DesignSystem | styles/shared controls | Domain stores |
| Asun.UI.Viewports | 2D/3D render backends | HALCON native handles |
| Asun.Domain.Pcb | PCB domain | other domain internals |
| Asun.Domain.Hdi | HDI domain | other domain internals |
| Asun.Domain.Fpc | FPC/R2R domain | other domain internals |
| Asun.Domain.RigidFlex | rigid/flex composition | other domain internals |
| Asun.Domain.Stencil | stencil domain | other domain internals |
| Asun.Domain.Spi | SPI domain | AOI/PCB internals |
| Asun.Domain.Aoi | AOI domain | SPI internals |
| Asun.Domain.Quality | cross-process projections/analysis | product private stores |
| Asun.App.* | product shells/workspaces | vendor SDK direct |

具体是否拆成独立 csproj 必须按 `DEV-GOV-005` 的真实消费者/运行/发布边界确认；逻辑模块与进程边界不是一回事。

## 3. Namespace 规则

命名空间反映责任域，不反映 UI 页面。算法命名空间不能包含 `ViewModel`。HALCON 类型只能出现在 `Asun.Vision.Halcon` 等适配层及批准的测试夹具中。

## 4. 代码任务落点

每个 FS-xxx 必须登记：目标项目、入口接口、实现类、Validator、Tests、PageContract（如适用）、Benchmark。没有这些映射不能进入 ImplementationReady。
