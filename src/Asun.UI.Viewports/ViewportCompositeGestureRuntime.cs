using System.Numerics;

namespace Asun.UI.Viewports;

public sealed class ViewportCompositeGestureRuntime
{
    private readonly object _sync = new();
    private readonly ViewportCompositeRuntime<object> _unused;
}
