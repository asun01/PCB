using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct ViewportInputRecoverySnapshot(
    ViewportPresentationState PresentationState,
    ViewportInputSubmissionSnapshot Submission,
    ViewportInputBackpressureSnapshot Backpressure);

public sealed class ViewportInputRecoveryRuntime : IDisposable
{
    private readonly object _sync=new();
    private readonly ViewportInputSubmissionRuntime _input;
    private readonly ViewportInputBackpressureRuntime _backpressure;
    private readonly ViewportPresentationLifecycleRuntime _lifecycle;
    private int _disposed;

    public ViewportInputRecoveryRuntime(
        int capacity=512,
        ViewportInputDropPolicy dropPolicy=ViewportInputDropPolicy.CoalesceMoves)
    {
        _input=new ViewportInputSubmissionRuntime();
        _backpressure=new ViewportInputBackpressureRuntime(capacity,dropPolicy);
        _lifecycle=new ViewportPresentationLifecycleRuntime();
    }

    public ViewportPresentationState State =>
        _lifecycle.State;

    public bool TryStart(out CancellationToken token)
    {
        lock(_sync)
        {
            ThrowIfDisposed();

            if(State==ViewportPresentationState.Running ||
               State==ViewportPresentationState.Stopping)
            {
                token=default;
                return false;
            }

            _input.ResetLifecycle();
            _backpressure.Reset();
            return _lifecycle.TryStart(out token);
        }
    }

    public bool TrySubmit(
        ViewportInputEventKind kind,
        Vector2 position,
        int wheelDelta=0,
        ViewportMouseButton button=ViewportMouseButton.Left)
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            return _backpressure.TrySubmit(
                _input,
                kind,
                position,
                wheelDelta,
                button);
        }
    }

    public ViewportInputRecoverySnapshot Capture()
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            return new ViewportInputRecoverySnapshot(
                State,
                _input.Snapshot(),
                _backpressure.Capture(_input));
        }
    }

    public IReadOnlyList<ViewportInputEvent> DrainPending(int maxCount=256)
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            return _input.Drain(maxCount);
        }
    }

    public void RequestStop()
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            _backpressure.Complete();
            _input.Complete();
            _lifecycle.RequestStop();
        }
    }

    public void MarkStopped()
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            _lifecycle.MarkStopped();
        }
    }

    public void Complete(bool cancelPending=false)
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            _backpressure.Complete(cancelPending);
            _input.Complete(cancelPending);
            _lifecycle.RequestStop();
            _lifecycle.MarkStopped();
        }
    }

    public void Cancel()
    {
        lock(_sync)
        {
            ThrowIfDisposed();
            _backpressure.Cancel();
            _input.Cancel();
            _lifecycle.RequestStop();
            _lifecycle.MarkStopped();
        }
    }

    public void Reset()
    {
        lock(_sync)
        {
            ThrowIfDisposed();

            if(State==ViewportPresentationState.Running ||
               State==ViewportPresentationState.Stopping)
            {
                throw new InvalidOperationException(
                    "A running input recovery session must stop before reset.");
            }

            _input.ResetLifecycle();
            _backpressure.Reset();
            _lifecycle.Reset();
        }
    }

    public void Dispose()
    {
        lock(_sync)
        {
            if(Interlocked.Exchange(ref _disposed,1)!=0)
                return;

            _backpressure.Dispose();
            _input.Dispose();
            _lifecycle.Dispose();
        }
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed!=0,this);
}
