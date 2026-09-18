# HALCON 版本验证与算法 Benchmark 开发指导
- 文档 ID：`DEV-VIS-022A`
- 版本：`2.0.0`
- 状态：`Normative`
- HALCON baseline：24.11.x（目标运行环境必须在实际开发机确认精确 patch）

## 1. Mandatory rule

所有进入生产代码的 HALCON operator、参数和返回值必须由目标版本官方 Reference Manual 与实际安装 API 双重验证。AI/开发人员不得凭记忆写 operator 名称。

## 2. Verified baseline examples

| Task | Verified operator | Evidence rule |
|---|---|---|
| Shape-based 2D localization | `find_shape_model` / `find_shape_models` | 当前目标版本 Reference + Golden benchmark |
| Subpixel edge | `edges_sub_pix` | 当前目标版本 Reference + repeatability benchmark |
| Ellipse fitting | `fit_ellipse_contour_xld` | 当前目标版本 Reference + residual benchmark |
| Local thresholding | `dyn_threshold` | 当前目标版本 Reference + illumination sweep |
| Camera calibration | `calibrate_cameras` | calibration qualification + independent validation poses |
| Stereo surface | `reconstruct_surface_stereo` | calibrated stereo setup + coverage/height benchmark |
| 3D global registration | `register_object_model_3d_global` | overlap/residual benchmark |

MVTec 24.11 documentation confirms `calibrate_cameras` performs a simultaneous minimization over the calibration data model, `fit_ellipse_contour_xld` fits XLD contours to ellipses/elliptic arcs and exposes robust fitting options, and multi-view stereo reconstruction requires a calibrated stereo setup; `reconstruct_surface_stereo` returns an ObjectModel3D. 

## 3. Algorithm selection procedure

For each problem record: candidate A/B/C, applicability, input-quality assumptions, robustness, parameter sensitivity, CPU/memory cost, multithreading behavior, determinism, diagnostics, fallback and reject conditions.

## 4. HALCON 3D rule

3D object models are not equivalent to metrology qualification. Registration, reconstruction and visualization quality must be separated from calibrated measurement quality. HALCON provides object-model functions including `FindSurfaceModel`, `ReconstructSurfaceStereo`, `RegisterObjectModel3dGlobal`, etc.; selection depends on task and qualification data, not operator popularity. 

## 5. Benchmark record

Every benchmark record must bind: HALCON exact version, CPU/GPU, input dataset hash, operator parameters, thread settings, ROI/bounding box, P50/P95/P99, peak memory, output quality metrics, failure count and reproducibility.


## 6. Implementation requirements

1. HALCON adapter must expose only the approved Vision Contract surface.
2. Operator selection records are versioned artifacts; application code does not embed an undocumented alternative route.
3. Benchmark instrumentation must be part of the implementation path, not a post-hoc manual measurement.
4. Any unverified operator/parameter blocks production implementation of that route; a Mock/Interface skeleton may be implemented without asserting API details.

## 7. Test requirements

- operator existence/signature verification against the target 24.11.x installation;
- nominal/boundary/adversarial Golden samples;
- repeatability and deterministic-output checks where required;
- parameter sweep and sensitivity analysis;
- P50/P95/P99, peak memory, cancellation and concurrency measurements;
- benchmark result hash and environment capture.

## 8. Forbidden implementation

- Do not guess HALCON operator names or parameters from model memory;
- do not silently replace a qualified route with a different operator;
- do not call HALCON from Domain/UI code;
- do not declare an operator “best” without the required benchmark and qualification evidence.
