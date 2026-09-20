using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asun.Platform.ClientIntegration;
using Asun.UI.Viewports;

namespace Asun.App.Shell.Bootstrap;

public sealed class WpfRoiInputAdapter
{
    private readonly ClientInspectionWorkspace _workspace;
    private readonly FrameworkElement _host;

    public WpfRoiInputAdapter(
        ClientInspectionWorkspace workspace,
        FrameworkElement host)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(host);
        _workspace=workspace;
        _host=host;
    }

    public bool MouseDown(MouseButtonEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);
        _host.Focus();

        var point=ToVector(args.GetPosition(_host));
        var button=args.ChangedButton==MouseButton.Right
            ? ViewportMouseButton.Right
            : ViewportMouseButton.Left;

        return _workspace.SubmitRoiInput(
            ViewportInputEventKind.PointerDown,
            point,
            button:button);
    }

    public bool MouseMove(MouseEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);
        var point=ToVector(args.GetPosition(_host));
        return _workspace.SubmitRoiInput(
            ViewportInputEventKind.PointerMove,
            point);
    }

    public bool MouseUp(MouseButtonEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);
        var point=ToVector(args.GetPosition(_host));
        return _workspace.SubmitRoiInput(
            ViewportInputEventKind.PointerUp,
            point);
    }

    public bool MouseWheel(MouseWheelEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);
        var point=ToVector(args.GetPosition(_host));
        return _workspace.SubmitRoiInput(
            ViewportInputEventKind.Wheel,
            point,
            args.Delta);
    }

    public bool Escape()
    {
        var point=new Vector2(
            (float)(_host.ActualWidth/2d),
            (float)(_host.ActualHeight/2d));

        return _workspace.SubmitRoiInput(
            ViewportInputEventKind.Escape,
            point);
    }

    private static Vector2 ToVector(Point point)=>
        new((float)point.X,(float)point.Y);
}
