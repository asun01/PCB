using System.Numerics;
using Asun.Platform.ClientIntegration;
using Asun.Platform.SimulationIntegration;
using Asun.UI.Viewports;

namespace Asun.App.Shell.Bootstrap;

public partial class MainWindow : System.Windows.Window
{
    private readonly ClientInspectionWorkspace _client;
    private readonly ClientWorkspaceRuntime _workspaceRuntime;
    private WpfRoiInputAdapter _roiInputAdapter;

    public MainWindow()
    {
        InitializeComponent();

        _client=new ClientInspectionWorkspace(
            new Vector2(100,100),
            new Vector2(1,1),
            historyCapacity:20);

        _workspaceRuntime=new ClientWorkspaceRuntime();
        _workspaceRuntime.Changed+=OnWorkspaceChanged;
        _client.ProductionChanged+=OnProductionChanged;
        _client.AcquisitionCatalog.Register(
            ClientSimulationSessionFactory.CreateSourceDefinition());
        AcquisitionSourceComboBox.ItemsSource=_client.AcquisitionCatalog.Sources;
        AcquisitionSourceComboBox.SelectedIndex=0;
        _roiInputAdapter=new WpfRoiInputAdapter(_client,RoiSurface);

        RefreshWorkspaceStatus();
        RefreshProgramStatus();
        RefreshResultStatus();
        RefreshQualityStatus();
        RefreshRunHistoryStatus();
        ApplyWorkspaceView(_workspaceRuntime.Current.Workspace);
        RefreshCommandAvailability();
    }

    private void Window_Closed(
        object? sender,
        System.EventArgs e)
    {
        _client.ProductionChanged-=OnProductionChanged;
        _workspaceRuntime.Changed-=OnWorkspaceChanged;
        _workspaceRuntime.Dispose();
        _client.Dispose();
    }

    private void OnWorkspaceChanged(ClientWorkspaceSelection selection)
    {
        WorkspaceNavigationStatus.Text=$"Workspace: {selection.Workspace}";
        ApplyWorkspaceView(selection.Workspace);
        RefreshCommandAvailability();
    }


    private void OnProductionChanged(ClientWorkspaceSnapshot snapshot)
    {
        if(!Dispatcher.HasShutdownStarted)
        {
            _=Dispatcher.BeginInvoke(new Action(() =>
            {
                RefreshWorkspaceStatus();
                RefreshAcquisitionStatus();
                RefreshCommandAvailability();
                RefreshResultStatus();
            }));
        }
    }

    private void BindAcquisitionButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if(AcquisitionSourceComboBox.SelectedItem
            is ClientAcquisitionSourceDefinition definition &&
           _client.BindAcquisitionSource(definition.Descriptor.SourceId))
        {
            AcquisitionStatus.Text=$"Acquisition: {definition.Descriptor.DisplayName}" +
                (definition.Descriptor.IsSimulation ? " · Simulation" : " · External source");
        }
        else
        {
            AcquisitionStatus.Text="Acquisition: source binding failed.";
        }

        RefreshCommandAvailability();
        RefreshDiagnosticStatus();
    }

    private void RunHistoryList_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if(e.AddedItems.Count==0)
            return;

        if(e.AddedItems[0] is ClientRunHistoryDisplayItem item &&
           _client.SelectHistory(item.Ordinal))
        {
            ResultStatus.Text=$"Historical run · #{item.Ordinal}";
            ResultSession.Text=$"{item.SessionText} · {item.FrameText}";
            ResultReplay.Text=item.ReplayText;
            ResultRelease.Text=item.ReleaseText;
        }
    }

    private void QualityFindingList_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if(e.AddedItems.Count==0)
            return;

        if(e.AddedItems[0] is ClientQualityFindingDisplayItem item &&
           _client.SelectQualityFinding(item.FindingId))
        {
            QualityFindingDetails.Text=
                $"{item.RuleCode} · {item.Outcome} · {item.Severity}\n" +
                $"Evidence links: {item.EvidenceCount}\n{item.Message}";
        }
    }

    private void ProgramStepList_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if(e.AddedItems.Count==0)
            return;

        if(e.AddedItems[0] is ClientProgramDisplayItem item)
        {
            _client.SelectProgramStep(item.StepId);
            ProgramStepDetails.Text=
                $"Step {item.Order} · {item.Name} · {item.Kind}\n" +
                (string.IsNullOrWhiteSpace(item.ParameterSummary)
                    ? "Parameters: none"
                    : $"Parameters: {item.ParameterSummary}");
        }
    }

    private void WorkspaceHomeButton_Click(object sender, System.Windows.RoutedEventArgs e) =>
        _workspaceRuntime.TryNavigate(ClientWorkspaceKind.Home);

    private void WorkspaceInspectionButton_Click(object sender, System.Windows.RoutedEventArgs e) =>
        _workspaceRuntime.TryNavigate(ClientWorkspaceKind.Inspection);

    private void WorkspaceProgramButton_Click(object sender, System.Windows.RoutedEventArgs e) =>
        _workspaceRuntime.TryNavigate(ClientWorkspaceKind.Program);

    private void WorkspaceQualityButton_Click(object sender, System.Windows.RoutedEventArgs e) =>
        _workspaceRuntime.TryNavigate(ClientWorkspaceKind.Quality);

    private void WorkspaceResultsButton_Click(object sender, System.Windows.RoutedEventArgs e) =>
        _workspaceRuntime.TryNavigate(ClientWorkspaceKind.Results);

    private void ApplyWorkspaceView(ClientWorkspaceKind workspace)
    {
        ProgramWorkspace.Visibility =
            workspace==ClientWorkspaceKind.Program
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;

        ResultWorkspace.Visibility =
            workspace is ClientWorkspaceKind.Results or ClientWorkspaceKind.Quality
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;

        RoiSurface.Visibility =
            workspace==ClientWorkspaceKind.Inspection
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;

        RoiWorkspaceTitle.Visibility =
            workspace==ClientWorkspaceKind.Inspection
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;

        RoiStatus.Visibility =
            workspace==ClientWorkspaceKind.Inspection
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;

        QualityStatus.Visibility =
            workspace==ClientWorkspaceKind.Quality
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
    }

    private void LoadSimulationButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        try
        {
            var definition=ClientSimulationSessionFactory.CreateDefinition();
            _client.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                definition.SessionId,
                definition.FrameCount);
            _client.BindAcquisitionSource("simulation");
            SimulationStatus.Text="Simulation session loaded.";
            RefreshCommandAvailability();
            ReleaseStatus.Text="Release: not evaluated.";
            DiagnosticStatus.Text="Diagnostic: not evaluated.";
            RefreshWorkspaceStatus();
            RefreshProgramStatus();
            RefreshResultStatus();
            RefreshRunHistoryStatus();
            RefreshRoiSurface();
        }
        catch(Exception exception)
        {
            SimulationStatus.Text=$"Load failed · {exception.Message}";
            ReleaseStatus.Text="Release: not evaluated.";
            RefreshDiagnosticStatus();
            RefreshWorkspaceStatus();
            RefreshProgramStatus();
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
        RefreshCommandAvailability();
        RoiSurface.Focus();
        RoiStatus.Text="ROI: Select mode.";
    }

    private void UndoRoiButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if(_client.UndoRoi())
            RefreshRoiSurface();
    }

    private void RedoRoiButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if(_client.RedoRoi())
            RefreshRoiSurface();
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
        RefreshCommandAvailability();
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
        CancelSimulationButton.IsEnabled=true;
        CreateRoiButton.IsEnabled=false;
        SimulationStatus.Text="Running deterministic simulation...";

        try
        {
            var definition=ClientSimulationSessionFactory.CreateDefinition();
            _client.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                definition.SessionId,
                definition.FrameCount);
            RefreshWorkspaceStatus();
            RefreshProgramStatus();
            RefreshCommandAvailability();

            var execution=_client.ExecuteAsync(
                ClientSimulationSessionFactory.CreateReleaseManifest());

            RefreshWorkspaceStatus();
            RefreshCommandAvailability();
            var report=await execution;

            var snapshot=_client.Capture();
            var diagnostic=ClientInspectionDiagnosticsRuntime.Analyze(snapshot);
            var replay=snapshot.Replay!;
            var release=snapshot.Release!;

            SimulationStatus.Text=$"Completed · {report.FrameCount} frames · replay {replay.ReplayFingerprint[..12]}...";
            ReleaseStatus.Text=release.ReleaseReady
                ? $"Release: Ready · {release.ArtifactPath}"
                : "Release: Not ready.";
            RefreshWorkspaceStatus();
            RefreshProgramStatus();
            RefreshResultStatus();
            RefreshRunHistoryStatus();
            RefreshDiagnosticStatus(diagnostic);
            RefreshRoiSurface();
        }
        catch(OperationCanceledException)
        {
            SimulationStatus.Text="Cancelled.";
            ReleaseStatus.Text="Release: not evaluated.";
            RefreshDiagnosticStatus();
            RefreshWorkspaceStatus();
        }
        catch(Exception exception)
        {
            SimulationStatus.Text=$"Failed · {exception.Message}";
            ReleaseStatus.Text="Release: not evaluated.";
            RefreshDiagnosticStatus();
            RefreshWorkspaceStatus();
        }
        finally
        {
            RefreshCommandAvailability();
        }
    }

    private void CancelSimulationButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        _client.CancelExecution();
        SimulationStatus.Text="Cancellation requested...";
    }

    private void ResetSessionButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        _client.ResetCurrentSession();
        ReleaseStatus.Text="Release: not evaluated.";
        RefreshDiagnosticStatus();
        RefreshCommandAvailability();
        SimulationStatus.Text="Ready.";
        RefreshWorkspaceStatus();
        RefreshProgramStatus();
        RefreshAcquisitionStatus();
        RefreshResultStatus();
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
            RefreshCommandAvailability();
            return;
        }

        RoiStatus.Text=$"ROI: {snapshot.Items.Count} items · selected {snapshot.Document.SelectedId}.";
        RefreshCommandAvailability();
    }

    private void RefreshCommandAvailability()
    {
        var availability=ClientCommandAvailabilityRuntime.Create(_client.Capture());
        var routing=ClientWorkspaceCommandRoutingRuntime.Create(
            _workspaceRuntime.Current,
            availability);

        LoadSimulationButton.IsEnabled=routing.CanLoadProgram;
        RunSimulationButton.IsEnabled=routing.CanRunInspection;
        CancelSimulationButton.IsEnabled=routing.CanCancelInspection;
        ResetSessionButton.IsEnabled=routing.CanResetSession;
        SelectRoiButton.IsEnabled=routing.CanEditRoi && availability.CanSelectRoi;
        CreateRoiButton.IsEnabled=routing.CanEditRoi && availability.CanCreateRoi;
        UndoRoiButton.IsEnabled=routing.CanEditRoi && availability.CanUndoRoi;
        RedoRoiButton.IsEnabled=routing.CanEditRoi && availability.CanRedoRoi;
    }

    private void RefreshAcquisitionStatus()
    {
        var acquisition=_client.Acquisition;
        AcquisitionStatus.Text=acquisition.State switch
        {
            ClientAcquisitionState.Unbound=>"Acquisition: unbound.",
            ClientAcquisitionState.Ready =>
                $"Acquisition: {acquisition.Descriptor?.DisplayName}" +
                (acquisition.Descriptor?.IsSimulation==true ? " · Simulation" : " · External source"),
            ClientAcquisitionState.Faulted =>
                $"Acquisition: Faulted · {acquisition.LastError}",
            _=>"Acquisition: unknown."
        };
    }

    private void RefreshProgramStatus()
    {
        var snapshot=_client.Program;
        if(snapshot.Status!=ClientProgramLoadStatus.Ready || _client.CurrentProgram is null)
        {
            ProgramStatus.Text="Program: none loaded.";
            return;
        }

        var items=ClientProgramPresentationRuntime.CreateItems(_client.CurrentProgram);
        ProgramStepList.ItemsSource=items;

        if(snapshot.SelectedStepId is Guid selected)
        {
            var selectedItem=items.FirstOrDefault(item=>item.StepId==selected);
            ProgramStepList.SelectedItem=selectedItem;
        }

        ProgramStatus.Text=$"Program: {snapshot.Name} · v{snapshot.Version} · {items.Count} step(s).";
    }

    private void RefreshResultStatus()
    {
        var snapshot=_client.Capture();
        var result=ClientResultsPresentationRuntime.CreateCurrent(snapshot);

        ResultStatus.Text=$"Status: {result.Status}";
        ResultSession.Text=$"{result.SessionText} · {result.FrameCount} frame(s)";
        ResultReplay.Text=result.ReplayText;
        ResultRelease.Text=result.ReleaseText;
        RefreshQualityStatus();
    }

    private void RefreshQualityStatus()
    {
        var quality=_client.Quality;

        if(!quality.IsBound)
        {
            QualityStatus.Text=
                "Quality Run: not attached. Quality facts remain outside this client projection until an authoritative Quality run is available.";
            QualityFindingList.ItemsSource=Array.Empty<ClientQualityFindingDisplayItem>();
            QualityFindingDetails.Text="No Quality finding selected.";
            return;
        }

        QualityStatus.Text=
            $"Quality Run {quality.RunId} · Results {quality.ResultCount} · Findings {quality.FindingCount} · Pass {quality.PassCount} · Fail {quality.FailCount} · Review {quality.ReviewCount} · Evidence links {quality.EvidenceLinkCount}.";
        QualityFindingList.ItemsSource=quality.Findings;

        if(quality.SelectedFindingId is string selected)
            QualityFindingList.SelectedItem=quality.Findings
                .FirstOrDefault(item=>item.FindingId==selected);
    }

    private void RefreshDiagnosticStatus(
        ClientInspectionDiagnosticSnapshot? diagnostic=null)
    {
        if(diagnostic is null)
        {
            DiagnosticStatus.Text="Diagnostic: not evaluated.";
            return;
        }

        DiagnosticStatus.Text=diagnostic.IsCoherent
            ? $"Diagnostic: Coherent · {diagnostic.Fingerprint[..12]}..."
            : $"Diagnostic: {diagnostic.Errors.Count} error(s).";
    }

    private void RefreshRunHistoryStatus()
    {
        var history=_client.History;
        RunHistoryStatus.Text=$"History: {history.Entries.Count} runs · dropped {history.DroppedCount}.";
        var items=ClientRunHistoryPresentationRuntime.CreateItems(history,5);
        RunHistoryList.ItemsSource=items;

        if(_client.SelectedHistoryOrdinal is long selected)
            RunHistoryList.SelectedItem=items.FirstOrDefault(item=>item.Ordinal==selected);
    }

    private void RefreshWorkspaceStatus()
    {
        var snapshot=_client.Production;
        WorkspaceStatus.Text=snapshot.Status switch
        {
            ClientExecutionStatus.Idle=>"Idle — no production session loaded.",
            ClientExecutionStatus.Ready=>$"Ready — Program {snapshot.ProgramId} · Session {snapshot.ActiveSessionId}.",
            ClientExecutionStatus.Running=>$"Running — Session {snapshot.ActiveSessionId} · {snapshot.FramesProcessed}/{snapshot.TargetFrameCount} frames.",
            ClientExecutionStatus.Completed=>$"Completed — {snapshot.LastFrameCount} frames; report fingerprint is available.",
            ClientExecutionStatus.Cancelled=>"Cancelled — client session execution was cancelled.",
            ClientExecutionStatus.Failed=>$"Failed — {snapshot.LastError}",
            _=>"Unknown client workspace state."
        };
    }
}
