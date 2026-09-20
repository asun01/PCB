using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Asun.UI.Viewports;

public readonly record struct ViewportRoiInputRecoverySnapshot(
    ViewportInputRecoverySnapshot Input,
    RoiViewportSnapshot Roi,
    long ProcessedEvents,
    long RejectedEvents,
    string RoiFingerprint,
    string Fingerprint);

public sealed class ViewportRoiInputRecoveryRuntime : IDisposable
{
    private readonly object _sync=new();
    private readonly ViewportInputRecoveryRuntime _inputRecovery;
    private readonly RoiViewportRuntime _roiViewport;
    private long _processedEvents;
    private long _rejectedEvents;
    private int _disposed;

    public ViewportRoiInputRecoveryRuntime(
        Vector2 imageSize,
        Vector2 viewportSize,
        RoiEditorMode mode=RoiEditorMode.Select,
        int capacity=512,
        ViewportInputDropPolicy dropPolicy=ViewportInputDropPolicy.CoalesceMoves)
    {
        _inputRecovery=new ViewportInputRecoveryRuntime(capacity,dropPolicy);
        _roiViewport=new RoiViewportRuntime(imageSize,viewportSize,mode);
    }

    public ViewportInputRecoveryRuntime InputRecovery => _inputRecovery;

    public RoiViewportRuntime RoiViewport => _roiViewport;

    public bool TryStart(out CancellationToken token)
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            return _inputRecovery.TryStart(out token);
        }
    }

    public bool TrySubmitAndProcess(
        ViewportInputEventKind kind,
        Vector2 viewportPoint,
        int wheelDelta=0,
        ViewportMouseButton button=ViewportMouseButton.Left)
    {
        lock(_sync)
        {
            ThrowIfDisposed();

            if(!_inputRecovery.TrySubmit(kind,viewportPoint,wheelDelta,button))
            {
                _rejectedEvents++;
                return false;
            }

            ProcessPendingUnsafe();
            return true;
        }
    }

    public long ProcessPending()
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            return ProcessPendingUnsafe();
        }
    }

    public ViewportRoiInputRecoverySnapshot Capture()
    {
        lock(_sync)
        {
            ThrowIfDisposed();

            var input=_inputRecovery.Capture();
            var roi=_roiViewport.CreateSnapshot();
            var roiFingerprint=CreateRoiFingerprint(roi);
            var fingerprint=CreateFingerprint(
                input,
                _processedEvents,
                _rejectedEvents,
                roiFingerprint);

            return new ViewportRoiInputRecoverySnapshot(
                input,
                roi,
                _processedEvents,
                _rejectedEvents,
                roiFingerprint,
                fingerprint);
        }
    }

    public void Complete(bool cancelPending=false)
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            ProcessPendingUnsafe();
            _inputRecovery.Complete(cancelPending);
        }
    }

    public void Cancel()
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            _inputRecovery.Cancel();
        }
    }

    public void Reset()
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            _inputRecovery.Reset();
            _processedEvents=0;
            _rejectedEvents=0;
        }
    }

    public void Dispose()
    {
        lock(_sync)
        {
            if(Interlocked.Exchange(ref _disposed,1)!=0)
                return;

            _inputRecovery.Dispose();
        }
    }

    private long ProcessPendingUnsafe()
    {
        long processed=0;
        var events=_inputRecovery.DrainPending();

        foreach(var input in events)
        {
            DispatchUnsafe(input);
            _processedEvents++;
            processed++;
        }

        return processed;
    }

    private void DispatchUnsafe(ViewportInputEvent input)
    {
        switch(input.Kind)
        {
            case ViewportInputEventKind.PointerDown:
                _roiViewport.PointerDown(input.Position);
                break;
            case ViewportInputEventKind.PointerMove:
                _roiViewport.PointerMove(input.Position);
                break;
            case ViewportInputEventKind.PointerUp:
                _roiViewport.PointerUp(input.Position);
                break;
            case ViewportInputEventKind.Escape:
                _roiViewport.Cancel(input.Position);
                break;
            case ViewportInputEventKind.Wheel:
            case ViewportInputEventKind.DoubleClick:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(input));
        }
    }

    private static string CreateRoiFingerprint(RoiViewportSnapshot snapshot)
    {
        var canonical=new StringBuilder();
        canonical.Append(snapshot.Transform.Scale.ToString("R",CultureInfo.InvariantCulture)).Append('|')
            .Append(snapshot.Transform.Translation.X.ToString("R",CultureInfo.InvariantCulture)).Append('|')
            .Append(snapshot.Transform.Translation.Y.ToString("R",CultureInfo.InvariantCulture)).Append('|')
            .Append(snapshot.SelectedId).Append('|');

        foreach(var item in snapshot.Items)
        {
            canonical.Append(item.Id).Append('|')
                .Append(item.IsSelected).Append('|')
                .Append(item.ZIndex).Append('|')
                .Append(item.Geometry.Kind).Append('|')
                .Append(item.Geometry.Center.X.ToString("R",CultureInfo.InvariantCulture)).Append('|')
                .Append(item.Geometry.Center.Y.ToString("R",CultureInfo.InvariantCulture)).Append('|')
                .Append(item.Geometry.Size.X.ToString("R",CultureInfo.InvariantCulture)).Append('|')
                .Append(item.Geometry.Size.Y.ToString("R",CultureInfo.InvariantCulture)).Append('|')
                .Append(item.Geometry.RotationRadians.ToString("R",CultureInfo.InvariantCulture)).Append('|');

            foreach(var point in item.Geometry.Vertices)
                canonical.Append(point.X.ToString("R",CultureInfo.InvariantCulture)).Append(',')
                    .Append(point.Y.ToString("R",CultureInfo.InvariantCulture)).Append(';');

            canonical.Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString())))
            .ToLowerInvariant();
    }

    private static string CreateFingerprint(
        ViewportInputRecoverySnapshot input,
        long processedEvents,
        long rejectedEvents,
        string roiFingerprint)
    {
        var canonical=string.Join(
            "|",
            input.PresentationState,
            input.Submission.Submitted,
            input.Submission.Pending,
            input.Backpressure.Accepted,
            input.Backpressure.Dropped,
            processedEvents,
            rejectedEvents,
            roiFingerprint);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed!=0,this);
}
