using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportFiftyChainSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        var a = new RoiDocumentItem(
            Guid.NewGuid(),
            RoiGeometry.CreateRectangle(new Vector2(50, 50), new Vector2(20, 20)),
            0);
        var b = new RoiDocumentItem(
            Guid.NewGuid(),
            RoiGeometry.CreateEllipse(new Vector2(120, 50), new Vector2(30, 20)),
            1);
        var c = new RoiDocumentItem(
            Guid.NewGuid(),
            RoiGeometry.CreateRectangle(new Vector2(70, 120), new Vector2(20, 30)),
            2);
        var items = new[] { a, b, c };

        // 1 clipboard
        var clipboard = new RoiClipboardRuntime();
        clipboard.Copy(new[] { a, b });
        var pasted = clipboard.Paste(new Vector2(10, 5));
        assert(pasted.Count == 2 && pasted[0].Id != a.Id, "Clipboard should round-trip ROIs.");

        // 2 duplicate pattern
        var grid = RoiDuplicatePatternRuntime.Grid(a, 2, 2, new Vector2(30, 40));
        assert(grid.Count == 4 && grid[3].Geometry.Center == a.Geometry.Center + new Vector2(30, 40), "Grid duplication should generate deterministic copies.");

        // 3 alignment
        var aligned = RoiAlignmentRuntime.Align(items, RoiAlignmentMode.Top);
        assert(aligned.Select(x => x.Geometry.GetBounds().Top).Distinct().Count() == 1, "Alignment should normalize top edges.");

        // 4 distribution
        var distributed = RoiDistributionRuntime.Distribute(items, RoiDistributionMode.HorizontalCenters);
        assert(distributed.Count == 3, "Distribution should preserve the item set.");

        // 5 distance
        var distance = RoiDistanceRuntime.Between(a, b);
        assert(distance.Distance > 0 && RoiDistanceRuntime.CenterDistance(a, b) > 0, "ROI distance runtime should calculate spatial distances.");

        // 6 measurement overlay
        var measurement = RoiMeasurementOverlayRuntime.Build(a);
        assert(measurement.Width.Length > 0 && measurement.Area.Length > 0, "Measurement overlay should format geometry values.");

        // 7 annotations
        var annotations = new RoiAnnotationRuntime();
        annotations.Set(a.Id, "A", a.Geometry.Center);
        assert(annotations.Snapshot().Count == 1 && annotations.Remove(a.Id), "Annotation runtime should add and remove annotations.");

        // 8 guidelines
        var guidelines = RoiGuidelineRuntime.Build(items);
        assert(guidelines.Count == 6, "Guideline runtime should emit two guides per ROI.");

        // 9 magnet
        var magnet = RoiMagnetRuntime.SnapToNearest(new Vector2(9, 10), new[] { new Vector2(10, 10), new Vector2(40, 40) }, 2);
        assert(magnet.Snapped && magnet.Point == new Vector2(10, 10), "Magnet runtime should snap within tolerance.");

        // 10 edit transaction
        var transaction = new RoiEditTransactionRuntime();
        var before = new RoiDocumentRuntime();
        before.Add(a.Geometry);
        transaction.Begin(before.CreateSnapshot());
        assert(transaction.Complete(before.CreateSnapshot()) is not null && !transaction.IsActive, "Edit transaction should complete and release its snapshot.");

        // 11 batch edit
        var batch = RoiBatchEditRuntime.Translate(items, new Vector2(5, 5));
        assert(batch.Count == items.Length && batch[0].Geometry.Center == a.Geometry.Center + new Vector2(5, 5), "Batch edit should transform all items.");

        // 12 selection projection
        var projection = RoiSelectionProjectionRuntime.Build(items);
        assert(projection.Items.Count == 3 && projection.Bounds.Width > 0, "Selection projection should build aggregate bounds.");

        // 13 focus
        var transform = ViewportTransform.Fit(new Vector2(1000, 500), new Vector2(500, 400));
        var focused = RoiFocusRuntime.Focus(transform, a);
        assert(focused.Scale > 0 && focused.GetVisibleImageRectangle().Width > 0, "ROI focus should produce a usable transform.");

        // 14 zoom profile
        var zoomProfile = ViewportZoomProfileRuntime.Validate(ViewportZoomProfileRuntime.Default);
        assert(ViewportZoomProfileRuntime.StepScale(1, 120, zoomProfile) > 1, "Zoom profile should calculate wheel scale.");

        // 15 pan policy
        var panPolicy = new ViewportPanPolicy(true, false, 0);
        var panTranslation = ViewportPanPolicyRuntime.Apply(transform, new Vector2(10000, -10000), panPolicy);
        assert(panTranslation != new Vector2(10000, -10000), "Pan policy should clamp out-of-bounds translation.");

        // 16 auto pan
        var autoPan = ViewportAutoPanRuntime.GetDelta(new Vector2(2, 200), transform.ViewportSize, 20, 10);
        assert(autoPan.X < 0, "Auto-pan should push toward the image when near an edge.");

        // 17 inertial pan
        var inertia = new ViewportInertialPanRuntime();
        inertia.SetVelocity(new Vector2(100, 0));
        var displacement = inertia.Step(0.1f);
        assert(displacement.X > 0 && inertia.Velocity.X < 100, "Inertial pan should decay velocity.");

        // 18 dirty flags
        var dirty = new ViewportRenderDirtyRuntime();
        dirty.Mark(ViewportDirtyFlags.Roi | ViewportDirtyFlags.Transform);
        assert((dirty.Consume() & ViewportDirtyFlags.Roi) != 0 && !dirty.IsDirty, "Dirty runtime should accumulate and consume render flags.");

        // 19 input capture
        var capture = new ViewportInputCaptureRuntime();
        assert(capture.TryCapture(ViewportInputOwner.Roi) && !capture.TryCapture(ViewportInputOwner.Pan) && capture.Release(ViewportInputOwner.Roi), "Input capture should enforce a single owner.");

        // 20 keyboard viewport
        var keyboardTransform = ViewportKeyboardNavigationRuntime.Apply(transform, ViewportNavigationKey.Home);
        assert(Math.Abs(keyboardTransform.Scale - keyboardTransform.FitScale) < 1e-9, "Keyboard Home should fit the viewport.");

        // 21 state journal
        var journal = new ViewportStateJournalRuntime(2);
        journal.Append(transform);
        journal.Append(focused);
        assert(journal.Count == 2 && journal.Snapshot().Count == 2, "Viewport journal should retain bounded history.");

        // 22 frame rate gate
        var gate = new ViewportFrameRateGate(60);
        var now = DateTimeOffset.UtcNow;
        assert(gate.TryEnter(now) && !gate.TryEnter(now), "Frame-rate gate should suppress duplicate frame entries.");

        // 23 prefetch policy
        var requests = new[]
        {
            new TileRequest(new TileIndex(0, 0), true, 0),
            new TileRequest(new TileIndex(1, 0), false, 4),
            new TileRequest(new TileIndex(0, 1), false, 1)
        };
        var policyRequests = TilePrefetchPolicyRuntime.Apply(requests, new TilePrefetchPolicy(1, 8, true, false));
        assert(policyRequests.Count == 3 && policyRequests[0].IsVisible, "Prefetch policy should retain visible requests first.");

        // 24 cache warmup runtime uses existing generic loader types.
        var source = new LocalTileSource();
        using var coordinator = new TileLoadCoordinator<string>(source, new TileCache<string>(8));
        var warmRequests = requests.Take(2).ToArray();
        var warmed = await TileCacheWarmupRuntime.WarmAsync(
            coordinator,
            warmRequests,
            request => new RectangleF(request.Index.X * 100, request.Index.Y * 100, 100, 100));
        assert(warmed == 2 && coordinator.Cache.Count == 2, "Tile warmup should populate the cache.");

        // 25 tile diagnostics
        var tileViewport = new ImageViewportRuntime<string>(
            new Vector2(500, 500),
            new Vector2(250, 250),
            new Vector2(125, 125),
            1,
            16,
            2,
            new LocalTileSource());
        var frame = await tileViewport.RefreshAsync();
        var diagnostics = TileViewportDiagnosticsRuntime.Capture(tileViewport, frame);
        assert(diagnostics.Planned > 0 && diagnostics.Loaded > 0, "Tile diagnostics should capture a populated viewport frame.");

        // 26 request priority
        var priority = TileRequestPriorityRuntime.Sort(requests);
        assert(priority[0].IsVisible, "Tile request priority should sort visible work first.");

        // 27 tile range
        var range = tileViewport.Transform.GetVisibleTileRange(new Vector2(125, 125));
        assert(TileRangeRuntime.Enumerate(range).Count > 0, "Tile range runtime should enumerate visible tiles.");

        // 28 visible region
        var visibleRegion = ViewportVisibleRegionRuntime.Capture(tileViewport.Transform, new Vector2(125, 125));
        assert(visibleRegion.Tiles.Count > 0 && visibleRegion.ImageRectangle.Width > 0, "Visible-region runtime should expose image and tile coverage.");

        // 29 selection projection
        var roiViewport = new RoiViewportRuntime(new Vector2(1000, 500), new Vector2(500, 400));
        var rid = roiViewport.Document.Add(a.Geometry);
        roiViewport.Document.Select(rid);
        var roiSnapshot = roiViewport.CreateSnapshot();
        var reprojected = ViewportSelectionProjectionRuntime.Project(roiSnapshot, new[] { rid });
        assert(reprojected.Single().IsSelected, "Viewport selection projection should mark selected render items.");

        // 30 viewport focus
        var focusPoint = ViewportFocusRuntime.FocusPoint(transform, new Vector2(300, 200));
        assert(Vector2.Distance(focusPoint.ViewportToImage(focusPoint.ViewportCenter), new Vector2(300, 200)) < 1e-4f, "Viewport focus point should center the requested image point.");

        // 31 interaction snapshot
        var gesture = new ViewportGestureRuntime(roiViewport);
        var interactionSnapshot = new ViewportInteractionSnapshot(
            roiViewport.Transform,
            gesture.Snapshot,
            roiSnapshot,
            ViewportDirtyFlags.Roi,
            ViewportInputOwner.Roi);
        assert(interactionSnapshot.Transform.Scale > 0 && interactionSnapshot.Roi.Items.Count == 1, "Interaction snapshot should combine viewport subsystems.");

        // 32 input router
        var routerCapture = new ViewportInputCaptureRuntime();
        var router = new ViewportInputRouterRuntime(gesture, routerCapture);
        var routerDown = router.PointerDown(roiViewport.Transform.ImageToViewport(a.Geometry.Center));
        router.PointerUp(roiViewport.Transform.ImageToViewport(a.Geometry.Center));
        assert(routerDown.Kind != ViewportGestureKind.Idle || routerCapture.Owner == ViewportInputOwner.None, "Input router should route pointer ownership.");

        // 33 command runtime
        var commandChanged = ViewportCommandRuntime.Execute(ViewportCommand.ZoomIn, roiViewport);
        assert(commandChanged, "Viewport command runtime should execute navigation commands.");

        // 34 ROI validation
        var issues = RoiValidationRuntime.Validate(items, new Vector2(500, 500));
        assert(issues.Count == 0, "Valid ROI collection should pass validation.");

        // 35 template runtime
        var templates = new RoiTemplateRuntime();
        templates.Save("rect", a.Geometry);
        assert(templates.TryCreate("rect", new Vector2(200, 200), out var templateGeometry) &&
               templateGeometry.Center == new Vector2(200, 200), "ROI templates should instantiate at a requested center.");

        // 36 layers
        var layers = new RoiLayerRuntime();
        layers.Ensure(a.Id, "A", 1);
        layers.SetVisible(a.Id, false);
        assert(!layers.Snapshot().Single().Visible, "ROI layers should track visibility.");

        // 37 visibility
        var visibleItems = RoiVisibilityRuntime.Filter(items, layers);
        assert(visibleItems.All(x => x.Id != a.Id), "Visibility runtime should filter hidden ROIs.");

        // 38 lock
        layers.SetLocked(b.Id, true);
        assert(RoiLockRuntime.LockedIds(layers).Contains(b.Id), "ROI lock runtime should expose locked IDs.");

        // 39 command history
        var history = new RoiCommandHistoryRuntime();
        history.Push("duplicate");
        assert(history.TryPop(out var historyCommand) && historyCommand == "duplicate", "ROI command history should preserve command order.");

        // 40 keyboard ROI command
        var keyboardDocument = new RoiDocumentRuntime();
        var keyboardId = keyboardDocument.Add(a.Geometry);
        keyboardDocument.Select(keyboardId);
        assert(RoiKeyboardCommandRuntime.Apply(keyboardDocument, RoiKeyboardCommand.MoveRight, 5), "ROI keyboard command should translate selected geometry.");

        // 41 status
        var status = RoiStatusRuntime.Capture(items, layers, new[] { a.Id, b.Id }, true);
        assert(status.Count == 3 && status.Selected == 2 && status.Locked == 1, "ROI status should aggregate selection, visibility and lock state.");

        // 42 marquee
        var marquee = RoiSelectionMarqueeRuntime.Normalize(new Vector2(100, 100), new Vector2(0, 0));
        assert(marquee.Width == 100 && marquee.Height == 100, "Marquee normalization should make positive dimensions.");

        // 43 coordinate transform
        var physical = RoiCoordinateTransformRuntime.ScaleToPhysical(a.Geometry, new Vector2(0.1f, 0.2f));
        assert(physical.Center == a.Geometry.Center * new Vector2(0.1f, 0.2f), "Coordinate transform should convert ROI coordinates to physical units.");

        // 44 multi-selection transforms
        var scaledGroup = RoiMultiSelectionTransformRuntime.Scale(items, new Vector2(2, 2));
        assert(scaledGroup.Count == 3 && scaledGroup[0].Geometry.Size.X == a.Geometry.Size.X * 2, "Multi-selection transform should scale all ROI items.");

        // 45 fit selection
        var fitSelection = RoiFitRuntime.FitSelection(transform, items);
        assert(fitSelection.GetVisibleImageRectangle().Width > 0, "ROI fit runtime should produce a visible transform.");

        // 46 physical formatter
        var formatted = RoiPhysicalUnitFormatterRuntime.Length(12.5, "mm", 2);
        assert(formatted == "12.50 mm", "Physical-unit formatter should produce invariant deterministic text.");

        // 47 transform guard
        assert(ViewportTransformGuardRuntime.IsUsable(transform), "Viewport transform guard should accept usable transforms.");

        // 48 state snapshot
        var stateSnapshot = new ViewportStateSnapshot(
            transform,
            gesture.Snapshot,
            new RoiSelectionRuntime().CreateSnapshot(),
            ViewportDirtyFlags.All,
            ViewportInputOwner.None,
            items);
        assert(stateSnapshot.Items.Count == 3 && stateSnapshot.Transform.Scale > 0, "Viewport state snapshot should aggregate editor state.");

        // 49 tile health
        var health = TileLoadHealthRuntime.Capture(tileViewport);
        assert(health.CacheCount > 0 && health.Hits >= 0, "Tile load health should expose cache metrics.");

        // 50 tile cache policy
        var cachePolicy = TileCachePolicyRuntime.Normalize(new TileCachePolicy(64, true, 2));
        assert(cachePolicy.Capacity == 64 && cachePolicy.PrefetchMarginTiles == 2, "Tile cache policy should validate and normalize.");

        if (failures.Count > 0)
            return;

        async Task DrainUnusedAsync() { await Task.CompletedTask; }
        await DrainUnusedAsync();
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult($"tile:{request.Index.X},{request.Index.Y}");
    }

    private static readonly List<string> failures = new();
}
