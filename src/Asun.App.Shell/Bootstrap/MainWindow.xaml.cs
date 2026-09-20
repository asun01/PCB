using System.Numerics;
using Asun.Platform.ClientIntegration;
using Asun.Platform.SimulationIntegration;
using Asun.UI.Viewports;

namespace Asun.App.Shell.Bootstrap;

public partial class MainWindow : System.Windows.Window
{
    private readonly ClientProductionWorkspace _workspace=new();
    private readonly ClientProductionRunHistory _runHistory=new(20);
    private ClientRoiInteractionWorkspace? _roiWorkspace;
    private WpfRoiInputAdapter? _roiInputAdapter;

    public MainWindow()
    {
        InitializeComponent();
        RefreshWorkspaceStatus();
        RefreshRunHistoryStatus();
    }

    private void LoadSimulationButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        try
        {
            EnsureRoiWorkspace();
            var definition=ClientSimulationSessionFactory.CreateDefinition();
            _workspace.Load(definition);
            ReleaseStatus.Text="Release: not evaluated.";
            SimulationStatus.Text="Simulation session loaded.";
            RefreshWorkspaceStatus();
            RefreshRoiSurface();
        }
        catch(Exception exception)
        {
            SimulationStatus.Text=$"Load failed · {exception.Message}";
            ReleaseStatus.Text="Release: not evaluated.";
            RefreshWorkspaceStatus();
        }
    }

    private void SelectRoiButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if(_roiWorkspace is null)
        {
            RoiStatus.Text="ROI: load and run a client session first.";
            return;
        }

        _roiWorkspace.Mode=RoiEditorMode.Select;
        RoiSurface.Focus();
        RoiStatus.Text="ROI: Select mode.";
    }

    private void CreateRoiButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if(_roiWorkspace is null)
        {
            RoiStatus.Text="ROI: load and run a client session first.";
            return;
        }

        _roiWorkspace.Mode=RoiEditorMode.CreateRectangle;
        RoiSurface.Focus();
        RoiStatus.Text="ROI: Create Rectangle mode.";
    }

    private async void RunSimulationButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        LoadSimulationButton.IsEnabled=false;
        RunSimulationButton.IsEnabled=false;
        ResetSessionButton.IsEnabled=false;
        SelectRoiButton.IsEnabled=false;
        CreateRoiButton.IsEnabled=false;
        SimulationStatus.Text="Running deterministic simulation...";

        try
        {
            var definition=ClientSimulationSessionFactory.CreateDefinition();
            _workspace.Load(definition);
            EnsureRoiWorkspace();

            var report=await _workspace.StartAsync(
                ClientSimulationSessionFactory.CreateSource());

            _roiWorkspace!.BindProductionReport(report);
            var replay=ClientProductionReplaySnapshotRuntime.Create(
                _workspace.Snapshot,
                report);
            var release=ClientReleaseProjectionRuntime.Create(
                replay,
                ClientSimulationSessionFactory.CreateReleaseManifest());

            _runHistory.Append(replay,release);
            SimulationStatus.Text=$"Completed · {report.FrameCount} frames · replay {replay.ReplayFingerprint[..12]}...";
            ReleaseStatus.Text=release.ReleaseReady
                ? $"Release: Ready · {release.ArtifactPath}"
                : "Release: Not ready.";
            RefreshWorkspaceStatus();
            RefreshRunHistoryStatus();
            RefreshRoiSurface();
        }
        catch(OperationCanceledException)
        {
            SimulationStatus.Text="Cancelled.";
            ReleaseStatus.Text="Release: not evaluated.";
            RefreshWorkspaceStatus();
        }
        catch(Exception exception)
        {
            SimulationStatus.Text=$"Failed · {exception.Message}";
            ReleaseStatus.Text="Release: not evaluated.";
            RefreshWorkspaceStatus();
        }
        finally
        {
            LoadSimulationButton.IsEnabled=true;
            RunSimulationButton.IsEnabled=true;
            ResetSessionButton.IsEnabled=true;
            SelectRoiButton.IsEnabled=true;
            CreateRoiButton.IsEnabled=true;
        }
    }

    private void ResetSessionButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        _workspace.Reset();
        _roiWorkspace?.Reset();
        ReleaseStatus.Text="Release: not evaluated.";
        SimulationStatus.Text="Ready.";
        RefreshWorkspaceStatus();
        RefreshRunHistoryStatus();
        RefreshRoiSurface();
    }

    private void RoiSurface_SizeChanged(
        object sender,
        System.Windows.SizeChangedEventArgs e)
    {
        if(_roiWorkspace is null || e.NewSize.Width<=0 || e.NewSize.Height<=0)
            return;

        _roiWorkspace.ResizeViewport(
            new Vector2((float)e.NewSize.Width,(float)e.NewSize.Height));
        RefreshRoiSurface();
    }

    private void RoiSurface_MouseDown(
        object sender,
        System.Windows.Input.MouseButtonEventArgs e)
    {
        if(_roiInputAdapter?.MouseDown(e)==true)
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RoiSurface_MouseMove(
        object sender,
        System.Windows.Input.MouseEventArgs e)
    {
        if(_roiInputAdapter?.MouseMove(e)==true)
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RoiSurface_MouseUp(
        object sender,
        System.Windows.Input.MouseButtonEventArgs e)
    {
        if(_roiInputAdapter?.MouseUp(e)==true)
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RoiSurface_MouseWheel(
        object sender,
        System.Windows.Input.MouseWheelEventArgs e)
    {
        if(_roiInputAdapter?.MouseWheel(e)==true)
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RoiSurface_KeyDown(
        object sender,
        System.Windows.Input.KeyEventArgs e)
    {
        if(e.Key==System.Windows.Input.Key.Escape &&
           _roiInputAdapter is not null &&
           _roiInputAdapter.Escape())
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void EnsureRoiWorkspace()
    {
        if(_roiWorkspace is not null)
            return;

        _roiWorkspace=new ClientRoiInteractionWorkspace(
            new Vector2(100,100),
            new Vector2(
                Math.Max(1d,RoiSurface.ActualWidth),
                Math.Max(1d,RoiSurface.ActualHeight)));

        _roiInputAdapter=new WpfRoiInputAdapter(
            _roiWorkspace,
            RoiSurface);
    }

    private void RefreshRoiSurface()
    {
        RoiSurface.Children.Clear();

        if(_roiWorkspace is null)
        {
            RoiStatus.Text="ROI: workspace not bound.";
            return;
        }

        var snapshot=_roiWorkspace.CaptureViewportSnapshot();
        foreach(var item in snapshot.Items)
        {
            var bounds=item.Geometry.GetBounds();
            var rectangle=new System.Windows.Shapes.Rectangle
            {
                Width=Math.Max(1d,bounds.Width),
                Height=Math.Max(1d,bounds.Height),
                Stroke=item.IsSelected
                    ? SystemColors.HighlightBrush
                    : SystemColors.InactiveSelectionHighlightBrush,
                StrokeThickness=item.IsSelected ? 2d : 1d,
                Fill=System.Windows.Media.Brushes.Transparent,
                IsHitTestVisible=false
            };

            Canvas.SetLeft(rectangle,bounds.Left);
            Canvas.SetTop(rectangle,bounds.Top);
            RoiSurface.Children.Add(rectangle);
        }

        if(snapshot.Items.Count==0)
        {
            RoiSurface.Children.Add(RoiSurfaceHint);
            RoiStatus.Text="ROI: 0 items.";
            return;
        }

        RoiStatus.Text=$"ROI: {snapshot.Items.Count} items · selected {snapshot.Document.SelectedId}.";
    }

    private void RefreshRunHistoryStatus()
    {
        var history=_runHistory.Capture();
        RunHistoryStatus.Text=$"History: {history.Entries.Count} runs · dropped {history.DroppedCount}.";
    }

    private void RefreshWorkspaceStatus()
    {
        var snapshot=_workspace.Snapshot;
        WorkspaceStatus.Text=snapshot.Status switch
        {
            ClientExecutionStatus.Idle=>"Idle — no production session loaded.",
            ClientExecutionStatus.Ready=>$"Ready — Program {snapshot.ProgramId} · Session {snapshot.ActiveSessionId}.",
            ClientExecutionStatus.Running=>$"Running — Session {snapshot.ActiveSessionId}.",
            ClientExecutionStatus.Completed=>$"Completed — {snapshot.LastFrameCount} frames; report fingerprint is available.",
            ClientExecutionStatus.Cancelled=>"Cancelled — client session execution was cancelled.",
            ClientExecutionStatus.Failed=>$"Failed — {snapshot.LastError}",
            _=>"Unknown client workspace state."
        };
    }
}
