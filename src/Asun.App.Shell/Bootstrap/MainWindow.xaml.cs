using System.Numerics;
using Asun.Platform.ClientIntegration;
using Asun.Platform.SimulationIntegration;
using Asun.UI.Viewports;

namespace Asun.App.Shell.Bootstrap;

public partial class MainWindow : System.Windows.Window
{
    private readonly ClientInspectionWorkspace _client;
    private WpfRoiInputAdapter _roiInputAdapter;

    public MainWindow()
    {
        InitializeComponent();

        _client=new ClientInspectionWorkspace(
            new Vector2(100,100),
            new Vector2(1,1),
            historyCapacity:20);
        _roiInputAdapter=new WpfRoiInputAdapter(_client,RoiSurface);

        RefreshWorkspaceStatus();
        RefreshRunHistoryStatus();
    }

    private void Window_Closed(
        object? sender,
        System.EventArgs e)
    {
        _client.Dispose();
    }

    private void LoadSimulationButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        try
        {
            _client.Load(ClientSimulationSessionFactory.CreateDefinition());
            SimulationStatus.Text="Simulation session loaded.";
            ReleaseStatus.Text="Release: not evaluated.";
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
        if(_client.Production.Status!=ClientExecutionStatus.Completed)
        {
            RoiStatus.Text="ROI: run a completed client session first.";
            return;
        }

        _client.SetRoiMode(RoiEditorMode.Select);
        RoiSurface.Focus();
        RoiStatus.Text="ROI: Select mode.";
    }

    private void CreateRoiButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if(_client.Production.Status!=ClientExecutionStatus.Completed)
        {
            RoiStatus.Text="ROI: run a completed client session first.";
            return;
        }

        _client.SetRoiMode(RoiEditorMode.CreateRectangle);
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
            _client.Load(definition);

            var report=await _client.ExecuteAsync(
                ClientSimulationSessionFactory.CreateSource(),
                ClientSimulationSessionFactory.CreateReleaseManifest());

            var snapshot=_client.Capture();
            var replay=snapshot.Replay!;
            var release=snapshot.Release!;

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
        _client.ResetCurrentSession();
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
        if(e.NewSize.Width<=0 || e.NewSize.Height<=0)
            return;

        _client.ResizeRoiViewport(
            new Vector2((float)e.NewSize.Width,(float)e.NewSize.Height));
        RefreshRoiSurface();
    }

    private void RoiSurface_MouseDown(
        object sender,
        System.Windows.Input.MouseButtonEventArgs e)
    {
        if(_client.Production.Status!=ClientExecutionStatus.Completed)
            return;

        if(_roiInputAdapter.MouseDown(e))
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RoiSurface_MouseMove(
        object sender,
        System.Windows.Input.MouseEventArgs e)
    {
        if(_client.Production.Status!=ClientExecutionStatus.Completed)
            return;

        if(_roiInputAdapter.MouseMove(e))
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RoiSurface_MouseUp(
        object sender,
        System.Windows.Input.MouseButtonEventArgs e)
    {
        if(_client.Production.Status!=ClientExecutionStatus.Completed)
            return;

        if(_roiInputAdapter.MouseUp(e))
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RoiSurface_MouseWheel(
        object sender,
        System.Windows.Input.MouseWheelEventArgs e)
    {
        if(_client.Production.Status!=ClientExecutionStatus.Completed)
            return;

        if(_roiInputAdapter.MouseWheel(e))
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RoiSurface_KeyDown(
        object sender,
        System.Windows.Input.KeyEventArgs e)
    {
        if(_client.Production.Status!=ClientExecutionStatus.Completed)
            return;

        if(e.Key==System.Windows.Input.Key.Escape && _roiInputAdapter.Escape())
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RefreshRoiSurface()
    {
        RoiSurface.Children.Clear();

        if(_client.Production.Status!=ClientExecutionStatus.Completed)
        {
            RoiSurface.Children.Add(RoiSurfaceHint);
            RoiStatus.Text="ROI: workspace not bound.";
            return;
        }

        var snapshot=_client.CaptureRoiViewport();
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
        var history=_client.History;
        RunHistoryStatus.Text=$"History: {history.Entries.Count} runs · dropped {history.DroppedCount}.";
    }

    private void RefreshWorkspaceStatus()
    {
        var snapshot=_client.Production;
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
