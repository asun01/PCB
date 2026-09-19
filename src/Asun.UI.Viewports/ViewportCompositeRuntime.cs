using System.Collections.Immutable;
using System.Numerics;

namespace Asun.UI.Viewports;

public sealed class ViewportCompositeFrame<TTile>
{
    internal ViewportCompositeFrame(
        ViewportTileFrame<TTile> tiles,
        RoiViewportSnapshot roi,
        IReadOnlyList<RoiRenderCommand> roiCommands,
        IReadOnlyList<ViewportSceneCommand> sceneCommands,
        ViewportDirtyFlags dirtyFlags,
        long generation)
    {
        Tiles = tiles;
        Roi = roi;
        RoiCommands = roiCommands.ToImmutableArray();
        SceneCommands = sceneCommands.ToImmutableArray();
        DirtyFlags = dirtyFlags;
        Generation = generation;
    }

    public ViewportTileFrame<TTile> Tiles { get; }

    public RoiViewportSnapshot Roi { get; }

    public IReadOnlyList<RoiRenderCommand> RoiCommands { get; }

    public IReadOnlyList<ViewportSceneCommand> SceneCommands { get; }

    public ViewportDirtyFlags DirtyFlags { get; }

    public long Generation { get; }

    public bool IsReady =>
        Tiles.IsCompleteForVisible &&
        Tiles.LoadedCount > 0;

    public int LoadedTileCount => Tiles.LoadedCount;
}

public sealed class ViewportCompositeRuntime<TTile> : IDisposable
{
    private readonly object _sync = new();
    private readonly ImageViewportRuntime<TTile> _image;
    private readonly RoiViewportRuntime _roi;
    private readonly ViewportRenderDirtyRuntime _dirty = new();

    private long _generation;
    private int _disposed;

    public ViewportCompositeRuntime(
        Vector2 imageSize,
        Vector2 viewportSize,
        Vector2 tileSize,
        int prefetchMarginTiles,
        int cacheCapacity,
        int maxConcurrency,
        ITileSource<TTile> tileSource,
        RoiEditorMode roiMode = RoiEditorMode.Select)
    {
        _image = new ImageViewportRuntime<TTile>(
            imageSize,
            viewportSize,
            tileSize,
            prefetchMarginTiles,
            cacheCapacity,
            maxConcurrency,
            tileSource);

        _roi = new RoiViewportRuntime(
            imageSize,
            viewportSize,
            roiMode);

        _roi.SetTransform(_image.Transform);
        _dirty.Mark(ViewportDirtyFlags.All);
    }

    public ImageViewportRuntime<TTile> ImageRuntime => _image;

    public RoiViewportRuntime RoiRuntime => _roi;

    public ViewportRenderDirtyRuntime DirtyRuntime => _dirty;

    public ViewportTransform Transform
    {
        get
        {
            lock (_sync)
                return _image.Transform;
        }
    }

    public long Generation =>
        Interlocked.Read(ref _generation);

    public void FitToViewport()
    {
        ExecuteNavigation(
            _image.FitToViewport,
            ViewportDirtyFlags.Image | ViewportDirtyFlags.Transform | ViewportDirtyFlags.Roi);
    }

    public void ResizeViewport(Vector2 viewportSize)
    {
        ExecuteNavigation(
            () => _image.ResizeViewport(viewportSize),
            ViewportDirtyFlags.Image | ViewportDirtyFlags.Transform | ViewportDirtyFlags.Roi);
    }

    public void PanBy(Vector2 delta, bool clamp = true)
    {
        ExecuteNavigation(
            () =>
            {
                if (clamp)
                    _image.PanByClamped(delta);
                else
                    _image.PanBy(delta);
            },
            ViewportDirtyFlags.Image | ViewportDirtyFlags.Transform | ViewportDirtyFlags.Roi);
    }

    public void ZoomAt(
        double factor,
        double minScale,
        double maxScale,
        Vector2 viewportAnchor)
    {
        ExecuteNavigation(
            () => _image.ZoomFactor(
                factor,
                minScale,
                maxScale,
                viewportAnchor),
            ViewportDirtyFlags.Image | ViewportDirtyFlags.Transform | ViewportDirtyFlags.Roi);
    }

    public void CenterOnImagePoint(Vector2 imagePoint)
    {
        ExecuteNavigation(
            () => _image.CenterOnImagePoint(imagePoint),
            ViewportDirtyFlags.Image | ViewportDirtyFlags.Transform | ViewportDirtyFlags.Roi);
    }

    public RoiViewportPointerEvent PointerDown(
        Vector2 viewportPoint,
        float handleTolerancePixels = 8f,
        float bodyTolerancePixels = 0f)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            SyncRoiTransformUnsafe();
            var result = _roi.PointerDown(
                viewportPoint,
                handleTolerancePixels,
                bodyTolerancePixels);
            _dirty.Mark(
                ViewportDirtyFlags.Roi |
                (result.DocumentEvent.SelectionChanged
                    ? ViewportDirtyFlags.Selection
                    : ViewportDirtyFlags.None));
            Interlocked.Increment(ref _generation);
            return result;
        }
    }

    public RoiViewportPointerEvent PointerMove(
        Vector2 viewportPoint,
        float handleTolerancePixels = 8f)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            SyncRoiTransformUnsafe();
            var result = _roi.PointerMove(
                viewportPoint,
                handleTolerancePixels);
            _dirty.Mark(ViewportDirtyFlags.Roi);
            Interlocked.Increment(ref _generation);
            return result;
        }
    }

    public RoiViewportPointerEvent PointerUp(Vector2 viewportPoint)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            SyncRoiTransformUnsafe();
            var result = _roi.PointerUp(viewportPoint);
            _dirty.Mark(ViewportDirtyFlags.Roi);
            Interlocked.Increment(ref _generation);
            return result;
        }
    }

    public RoiViewportPointerEvent CancelPointer(Vector2 viewportPoint)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            SyncRoiTransformUnsafe();
            var result = _roi.Cancel(viewportPoint);
            _dirty.Mark(ViewportDirtyFlags.Roi);
            Interlocked.Increment(ref _generation);
            return result;
        }
    }

    public Guid AddRoi(RoiGeometry geometry, Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(geometry);

        lock (_sync)
        {
            ThrowIfDisposed();
            SyncRoiTransformUnsafe();
            var result = _roi.Document.Add(geometry, id);
            _dirty.Mark(ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection);
            Interlocked.Increment(ref _generation);
            return result;
        }
    }

    public bool SelectRoi(Guid? id)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            var changed = _roi.Document.Select(id);
            if (changed)
            {
                _dirty.Mark(ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection);
                Interlocked.Increment(ref _generation);
            }

            return changed;
        }
    }

    public bool TranslateSelected(Vector2 delta)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            var changed = _roi.Document.TranslateSelected(delta);
            if (changed)
            {
                _dirty.Mark(ViewportDirtyFlags.Roi);
                Interlocked.Increment(ref _generation);
            }

            return changed;
        }
    }

    public Guid? DuplicateSelected(Vector2 offset = default)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            var id = _roi.Document.DuplicateSelected(offset);
            if (id is not null)
            {
                _dirty.Mark(ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection);
                Interlocked.Increment(ref _generation);
            }

            return id;
        }
    }

    public bool DeleteSelected()
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            var changed = _roi.Document.DeleteSelected();
            if (changed)
            {
                _dirty.Mark(ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection);
                Interlocked.Increment(ref _generation);
            }

            return changed;
        }
    }

    public bool Undo()
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            var changed = _roi.Undo();
            if (changed)
            {
                _dirty.Mark(ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection);
                Interlocked.Increment(ref _generation);
            }

            return changed;
        }
    }

    public bool Redo()
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            var changed = _roi.Redo();
            if (changed)
            {
                _dirty.Mark(ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection);
                Interlocked.Increment(ref _generation);
            }

            return changed;
        }
    }

    public ViewportCompositeFrame<TTile> CreateCachedFrame()
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            SyncRoiTransformUnsafe();
            return BuildFrameUnsafe(
                _image.CreateCachedFrame(),
                _roi.CreateSnapshot(),
                _dirty.Flags);
        }
    }

    public async ValueTask<ViewportCompositeFrame<TTile>> RefreshAsync(
        bool includePrefetch = false,
        CancellationToken cancellationToken = default)
    {
        ViewportTransform transform;
        RoiViewportSnapshot roiSnapshot;
        ViewportDirtyFlags dirtyAtStart;

        lock (_sync)
        {
            ThrowIfDisposed();
            SyncRoiTransformUnsafe();
            transform = _image.Transform;
            roiSnapshot = _roi.CreateSnapshot();
            dirtyAtStart = _dirty.Flags;
        }

        var tileFrame = includePrefetch
            ? await _image.RefreshAndPrefetchAsync(cancellationToken)
                .ConfigureAwait(false)
            : await _image.RefreshAsync(cancellationToken)
                .ConfigureAwait(false);

        if (tileFrame.Transform != transform)
            throw new InvalidOperationException(
                "The tile frame transform changed during a composite refresh.");

        lock (_sync)
        {
            ThrowIfDisposed();
            return BuildFrameUnsafe(
                tileFrame,
                roiSnapshot,
                dirtyAtStart);
        }
    }

    public ViewportCompositeFrame<TTile> CreateFrame(
        ViewportTileFrame<TTile> tileFrame,
        RoiViewportSnapshot roiSnapshot,
        ViewportDirtyFlags dirtyFlags = ViewportDirtyFlags.All)
    {
        ArgumentNullException.ThrowIfNull(tileFrame);
        ArgumentNullException.ThrowIfNull(roiSnapshot);

        if (tileFrame.Transform != roiSnapshot.Transform)
        {
            throw new ArgumentException(
                "Tile and ROI snapshots must use the same viewport transform.");
        }

        lock (_sync)
        {
            ThrowIfDisposed();
            return BuildFrameUnsafe(
                tileFrame,
                roiSnapshot,
                dirtyFlags);
        }
    }

    public ViewportDirtyFlags ConsumeDirtyFlags() =>
        _dirty.Consume();

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        _image.Dispose();
    }

    private void ExecuteNavigation(
        Action navigation,
        ViewportDirtyFlags flags)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            navigation();
            SyncRoiTransformUnsafe();
            _dirty.Mark(flags);
            Interlocked.Increment(ref _generation);
        }
    }

    private ViewportCompositeFrame<TTile> BuildFrameUnsafe(
        ViewportTileFrame<TTile> tileFrame,
        RoiViewportSnapshot roiSnapshot,
        ViewportDirtyFlags dirtyFlags)
    {
        var roiCommands = RoiRenderCommandBuilder.Build(roiSnapshot);
        var scene = ViewportSceneRuntime.Build(roiSnapshot);

        return new ViewportCompositeFrame<TTile>(
            tileFrame,
            roiSnapshot,
            roiCommands,
            scene.Commands,
            dirtyFlags,
            Generation);
    }

    private void SyncRoiTransformUnsafe() =>
        _roi.SetTransform(_image.Transform);

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
