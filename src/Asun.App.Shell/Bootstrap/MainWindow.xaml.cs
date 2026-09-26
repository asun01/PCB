using System.Numerics;
using System.Windows.Media.Imaging;
using Asun.Device.Contracts;
using Asun.Platform.ClientIntegration;
using Asun.Platform.SimulationIntegration;
using Asun.UI.Viewports;

namespace Asun.App.Shell.Bootstrap;

public partial class MainWindow : System.Windows.Window
{
    private readonly ClientInspectionWorkspace _client;
    private readonly ClientWorkspaceRuntime _workspaceRuntime;
    private readonly ClientWorkspaceClientProjection _clientProjection;
    private ClientRunHistorySelection _runHistorySelection=
        ClientRunHistorySelectionRuntime.CreateInitial();
    private ClientQualityFindingSelection _qualityFindingSelection=
        ClientQualityFindingSelectionRuntime.CreateInitial();
    private WpfRoiInputAdapter _roiInputAdapter;
    private bool _isSynchronizingClientControls;

    public MainWindow()
    {
        InitializeComponent();

        _client=new ClientInspectionWorkspace(
            new Vector2(100,100),
            new Vector2(1,1),
            historyCapacity:20);

        _workspaceRuntime=new ClientWorkspaceRuntime();
        _clientProjection=new ClientWorkspaceClientProjection(
            _workspaceRuntime,
            _client,
            CreateRouting);
        _clientProjection.Changed+=OnClientProjectionChanged;
        _clientProjection.ExecutionChanged+=OnClientExecutionChanged;
        _clientProjection.RoiPulseChanged+=OnClientRoiPulseChanged;
        _client.AcquisitionCatalog.Register(
            ClientSimulationSessionFactory.CreateSourceDefinition());

        _client.QualityProviderCatalog.Register(
            ClientSimulationQualityRunProvider.CreateDefinition());

        QualityProviderComboBox.ItemsSource=_client.QualityProviderCatalog.Providers;
        QualityProviderComboBox.SelectedIndex=0;
        AcquisitionSourceComboBox.ItemsSource=_client.AcquisitionCatalog.Sources;
        AcquisitionSourceComboBox.SelectedIndex=0;
        _roiInputAdapter=new WpfRoiInputAdapter(_client,RoiSurface);

        RefreshWorkspaceStatus();
        RefreshProgramStatus();
        RefreshResultStatus();
        ApplyQualityProjection(_clientProjection.Snapshot.Content.Quality);
        RefreshRunHistoryStatus();
        ApplyWorkspaceView(_workspaceRuntime.Current.Workspace);
        RefreshCommandAvailability();
    }

    private void Window_Closed(
        object? sender,
        System.EventArgs e)
    {
        _clientProjection.Changed-=OnClientProjectionChanged;
        _clientProjection.ExecutionChanged-=OnClientExecutionChanged;
        _clientProjection.RoiPulseChanged-=OnClientRoiPulseChanged;
        _clientProjection.Dispose();
        _workspaceRuntime.Dispose();
        _client.Dispose();
    }

    private void OnClientProjectionChanged(
        ClientWorkspaceClientSnapshot snapshot)
    {
        if(!Dispatcher.CheckAccess())
        {
            _=Dispatcher.InvokeAsync(() => OnClientProjectionChanged(snapshot));
            return;
        }

        WorkspaceNavigationStatus.Text=$"Workspace: {snapshot.Selection.Workspace}";
        ApplyWorkspaceView(snapshot.Selection.Workspace);
        ApplyHomeProjection(snapshot.Content.Home);
        ApplyProgramProjection(snapshot.Content.Program);
        ApplyInspectionProjection(snapshot.Content.Inspection);
        ApplyResultsProjection(
            snapshot.Content.Results,
            snapshot.History);
        ApplyQualityProjection(snapshot.Content.Quality);
        RefreshWorkspaceStatus();
        RefreshCommandAvailability(snapshot);
    }

    private void ApplyHomeProjection(ClientHomePresentationSnapshot home)
    {
        HomeProgramStatus.Text=$"Program: {home.ProgramStatus}";
        HomeAcquisitionStatus.Text=$"Acquisition: {home.AcquisitionStatus}";
        HomeProductionStatus.Text=$"Production: {home.ProductionStatus}";
        HomeQualityStatus.Text=$"Quality: {home.QualityStatus}";
        HomeResultsStatus.Text=$"Results: {home.ResultsStatus}";
        HomeFingerprint.Text=$"Overview: {home.Fingerprint[..12]}...";
    }

    private void ApplyProgramProjection(ClientProgramSurface program)
    {
        _isSynchronizingClientControls=true;
        try
        {
            ProgramStepList.ItemsSource=program.Items;
            ProgramStepList.SelectedItem=program.SelectedItem;
        }
        finally
        {
            _isSynchronizingClientControls=false;
        }

        ProgramStatus.Text=program.Snapshot.Status==ClientProgramLoadStatus.Ready
            ? $"Program: {program.Snapshot.Name} · v{program.Snapshot.Version} · {program.Items.Count} step(s)."
            : program.Snapshot.Status==ClientProgramLoadStatus.Invalid
                ? "Program: invalid."
                : "Program: none loaded.";

        ProgramStepDetails.Text=program.SelectedItem is null
            ? program.SelectionText
            : $"Step {program.SelectedItem.Order} · {program.SelectedItem.Name} · {program.SelectedItem.Kind}\n" +
              (string.IsNullOrWhiteSpace(program.SelectedItem.ParameterSummary)
                  ? "Parameters: none"
                  : $"Parameters: {program.SelectedItem.ParameterSummary}");
    }

    private void ApplyInspectionProjection(ClientInspectionExecutionSurface inspection)
    {
        AcquisitionStatus.Text=inspection.Presentation.AcquisitionText;
        AcquisitionPreviewStatus.Text=inspection.Presentation.AcquisitionPreviewText;
        RunReadinessStatus.Text=inspection.RunReadinessText;
        RenderPreview(inspection.AcquisitionPreview);
    }

    private void ApplyResultsProjection(
        ClientResultsSurface results,
        ClientProductionRunHistorySnapshot history)
    {
        ResultStatus.Text=$"Status: {results.Current.Status}";
        ResultCurrentAuthorityStatus.Text=results.CurrentAuthorityText;
        ResultSession.Text=$"{results.Current.SessionText} · {results.Current.FrameCount} frame(s)";
        ResultQuality.Text=results.Current.QualityText;
        ResultReplay.Text=results.Current.ReplayText;
        ResultRelease.Text=results.Current.ReleaseText;
        var releaseReplay=results.ReleaseReplay;
        ResultReleaseReplayQuality.Text=releaseReplay.QualityFingerprint.Length==64
            ? $"Quality authority: {releaseReplay.QualityFingerprint[..12]}..."
            : "Quality authority: pending";
        ResultReleaseReplayReplay.Text=releaseReplay.ReplayAvailable &&
            releaseReplay.ReplayFingerprint.Length==64
            ? $"Replay authority: {releaseReplay.ReplayFingerprint[..12]}..."
            : "Replay authority: unavailable";
        ResultReleaseReplayRelease.Text=releaseReplay.ReleaseAvailable
            ? releaseReplay.ReleaseReady
                ? $"Release authority: Ready · {releaseReplay.ReleaseArtifactPath}"
                : "Release authority: Not Ready"
            : "Release authority: unavailable";

        _isSynchronizingClientControls=true;
        try
        {
            RunHistoryList.ItemsSource=results.History;
            RunHistoryList.SelectedItem=results.SelectedHistoryItem;
        }
        finally
        {
            _isSynchronizingClientControls=false;
        }

        _runHistorySelection=results.SelectedHistoryItem is { } selected
            ? new ClientRunHistorySelection(
                selected.Ordinal,
                _runHistorySelection.SelectionSequence,
                results.SelectionText)
            : ClientRunHistorySelectionRuntime.CreateInitial();

        RunHistorySelectionStatus.Text=results.SelectionText;
        RunHistoryAuthorityStatus.Text=results.SelectionAuthorityText;
        RunHistoryStatus.Text=$"History: {history.Entries.Count} runs · dropped {history.DroppedCount}.";
    }

    private void OnClientExecutionChanged(
        ClientWorkspaceClientExecutionPulse pulse)
    {
        if(!Dispatcher.CheckAccess())
        {
            _=Dispatcher.InvokeAsync(() => OnClientExecutionChanged(pulse));
            return;
        }

        WorkspaceNavigationStatus.Text=$"Workspace: {pulse.Selection.Workspace}";
        WorkspaceStatus.Text=pulse.Progress.StatusText;
        var target=pulse.Progress.TargetFrameCount;
        ExecutionProgress.Maximum=Math.Max(1,target);
        ExecutionProgress.Value=Math.Clamp(pulse.Progress.FramesProcessed,0,Math.Max(1,target));
        RefreshCommandAvailability();
    }

    private void OnClientRoiPulseChanged(
        ClientInspectionRoiPulse pulse)
    {
        if(!Dispatcher.CheckAccess())
        {
            _=Dispatcher.InvokeAsync(() => OnClientRoiPulseChanged(pulse));
            return;
        }

        RefreshRoiSurface();
    }

    private async void PreviewAcquisitionButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        var routing=CreateRouting(ClientWorkspaceKind.Inspection);
        if(!routing.CanPreviewAcquisition)
        {
            AcquisitionStatus.Text="Acquisition preview unavailable in the current client state.";
            RefreshCommandAvailability();
            return;
        }

        var snapshot=_client.Capture();
        try
        {
            var preview=await ClientInspectionAcquisitionCommandRuntime
                .Preview(
                    _client,
                    ClientInspectionExecutionSurfaceRuntime.Create(
                        snapshot,
                        routing));
            RefreshDiagnosticStatus();
        }
        catch(OperationCanceledException)
        {
            AcquisitionStatus.Text="Acquisition preview cancelled.";
        }
        catch(Exception exception)
        {
            AcquisitionStatus.Text=$"Acquisition preview failed · {exception.Message}";
            RefreshDiagnosticStatus();
        }
        finally
        {
            RefreshCommandAvailability();
        }
    }

    private void BindAcquisitionButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        var routing=CreateRouting(ClientWorkspaceKind.Inspection);
        if(!routing.CanBindAcquisition)
        {
            AcquisitionStatus.Text="Acquisition: binding unavailable in the current client state.";
            RefreshCommandAvailability();
            return;
        }

        if(AcquisitionSourceComboBox.SelectedItem
            is ClientAcquisitionSourceDefinition definition &&
           ClientInspectionAcquisitionCommandRuntime.BindSource(
               _client,
               routing,
               definition.Descriptor.SourceId))
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

    private void QualityFindingList_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if(_isSynchronizingClientControls ||
           e.AddedItems.Count==0 ||
           e.AddedItems[0] is not ClientQualityFindingDisplayItem item)
            return;

        var filter=new ClientQualityFilter(
            ReadComboValue(QualityOutcomeFilter),
            ReadComboValue(QualitySeverityFilter));

        if(!ClientQualityCommandRuntime.SelectVisibleFinding(
            _client,
            CreateRouting(ClientWorkspaceKind.Quality),
            filter,
            item.FindingId))
            return;

        var filtered=ClientQualityFilterRuntime.Apply(
            _clientProjection.Snapshot.Content.Quality.Snapshot,
            filter);

        _qualityFindingSelection=ClientQualityFindingSelectionRuntime.Select(
            filtered,
            item.FindingId,
            _qualityFindingSelection.SelectionSequence);

        ApplyQualityProjection(_clientProjection.Snapshot.Content.Quality);
    }

    private void QualityFilter_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if(!IsInitialized)
            return;

        ApplyQualityProjection(_clientProjection.Snapshot.Content.Quality);
    }

    private static string ReadComboValue(
        System.Windows.Controls.ComboBox combo)
    {
        return (combo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString()
            ?? "All";
    }

    private void RunQualityButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        try
        {
            if(QualityProviderComboBox.SelectedItem
                is not ClientQualityProviderDefinition definition)
            {
                throw new InvalidOperationException("No Quality provider is selected.");
            }

            ClientQualityCommandRuntime.EvaluateProvider(
                _client,
                CreateRouting(ClientWorkspaceKind.Quality),
                definition.Descriptor.ProviderId);
            ApplyQualityProjection(_clientProjection.Snapshot.Content.Quality);
            RefreshHomeStatus();
            RefreshDiagnosticStatus();
            RefreshCommandAvailability();
        }
        catch(Exception exception)
        {
            QualityStatus.Text=$"Quality evaluation failed · {exception.Message}";
            RefreshHomeStatus();
            RefreshDiagnosticStatus();
            RefreshCommandAvailability();
        }
    }

    private void ProgramStepList_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if(_isSynchronizingClientControls ||
           e.AddedItems.Count==0)
            return;

        if(e.AddedItems[0] is ClientProgramDisplayItem item)
        {
            ClientProgramCommandRuntime.SelectStep(
                _client,
                CreateRouting(ClientWorkspaceKind.Program),
                item.StepId);
            ProgramStepDetails.Text=
                $"Step {item.Order} · {item.Name} · {item.Kind}\n" +
                (string.IsNullOrWhiteSpace(item.ParameterSummary)
                    ? "Parameters: none"
                    : $"Parameters: {item.ParameterSummary}");
        }
    }

    private ClientWorkspaceCommandRouting CreateRouting(
        ClientWorkspaceSelection selection)
    {
        var availability=ClientCommandAvailabilityRuntime.Create(
            _client.Capture());

        return ClientWorkspaceCommandRoutingRuntime.Create(
            selection,
            availability);
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
        HomeWorkspace.Visibility =
            workspace==ClientWorkspaceKind.Home
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;

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
            _workspaceRuntime.TryNavigate(ClientWorkspaceKind.Inspection);
            var definition=ClientSimulationSessionFactory.CreateDefinition();
            ClientInspectionExecutionCommandRuntime.LoadProgram(
                _client,
                ClientInspectionExecutionSurfaceRuntime.Create(
                    _client.Capture(),
                    CreateRouting(ClientWorkspaceKind.Inspection)),
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                definition.SessionId,
                definition.FrameCount);
            ClientInspectionAcquisitionCommandRuntime.BindSource(
                _client,
                CreateRouting(ClientWorkspaceKind.Inspection),
                "simulation");
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
        if(!_clientProjection.Snapshot.Content.Inspection.HasCompletedResult)
        {
            RoiStatus.Text="ROI: run a completed client session first.";
            return;
        }

        ClientInspectionExecutionCommandRuntime.SetRoiMode(
            _client,
            ClientInspectionExecutionSurfaceRuntime.Create(
                _client.Capture(),
                CreateRouting(ClientWorkspaceKind.Inspection)),
            RoiEditorMode.Select);
        RefreshCommandAvailability();
        RoiSurface.Focus();
        RoiStatus.Text="ROI: Select mode.";
    }

    private void UndoRoiButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if(ClientInspectionExecutionCommandRuntime.UndoRoi(
               _client,
               ClientInspectionExecutionSurfaceRuntime.Create(
                   _client.Capture(),
                   CreateRouting(ClientWorkspaceKind.Inspection))))
            RefreshRoiSurface();
    }

    private void RedoRoiButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if(ClientInspectionExecutionCommandRuntime.RedoRoi(
               _client,
               ClientInspectionExecutionSurfaceRuntime.Create(
                   _client.Capture(),
                   CreateRouting(ClientWorkspaceKind.Inspection))))
            RefreshRoiSurface();
    }

    private void CreateRoiButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        if(!_clientProjection.Snapshot.Content.Inspection.HasCompletedResult)
        {
            RoiStatus.Text="ROI: run a completed client session first.";
            return;
        }

        ClientInspectionExecutionCommandRuntime.SetRoiMode(
            _client,
            ClientInspectionExecutionSurfaceRuntime.Create(
                _client.Capture(),
                CreateRouting(ClientWorkspaceKind.Inspection)),
            RoiEditorMode.CreateRectangle);
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
            _workspaceRuntime.TryNavigate(ClientWorkspaceKind.Inspection);
            var definition=ClientSimulationSessionFactory.CreateDefinition();
            ClientInspectionExecutionCommandRuntime.LoadProgram(
                _client,
                ClientInspectionExecutionSurfaceRuntime.Create(
                    _client.Capture(),
                    CreateRouting(ClientWorkspaceKind.Inspection)),
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                definition.SessionId,
                definition.FrameCount);
            ClientInspectionAcquisitionCommandRuntime.BindSource(
                _client,
                CreateRouting(ClientWorkspaceKind.Inspection),
                "simulation");
            RefreshWorkspaceStatus();
            RefreshProgramStatus();
            RefreshCommandAvailability();

            var execution=ClientInspectionExecutionCommandRuntime.ExecuteAsync(
                _client,
                ClientInspectionExecutionSurfaceRuntime.Create(
                    _client.Capture(),
                    CreateRouting(ClientWorkspaceKind.Inspection)),
                ClientSimulationSessionFactory.CreateReleaseManifest());

            RefreshWorkspaceStatus();
            RefreshCommandAvailability();
            var report=await execution;

            if(QualityProviderComboBox.SelectedItem is not ClientQualityProviderDefinition qualityDefinition)
                throw new InvalidOperationException("No Quality provider is selected for the completed simulation.");

            ClientQualityCommandRuntime.EvaluateProvider(
                _client,
                CreateRouting(ClientWorkspaceKind.Quality),
                qualityDefinition.Descriptor.ProviderId);

            var snapshot=_client.Capture();
            var diagnostic=ClientInspectionDiagnosticsRuntime.Analyze(snapshot);
            var replay=snapshot.Replay
                ?? throw new InvalidOperationException("Quality evaluation did not finalize Replay evidence.");
            var release=snapshot.Release
                ?? throw new InvalidOperationException("Quality evaluation did not finalize Release evidence.");

            SimulationStatus.Text=$"Completed · {report.FrameCount} frames · Quality {snapshot.Quality.Fingerprint![..12]}... · replay {replay.ReplayFingerprint[..12]}...";
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
        ClientInspectionExecutionCommandRuntime.Cancel(
            _client,
            ClientInspectionExecutionSurfaceRuntime.Create(
                _client.Capture(),
                CreateRouting(ClientWorkspaceKind.Inspection)));
        SimulationStatus.Text="Cancellation requested...";
    }

    private void ResetSessionButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        ClientInspectionExecutionCommandRuntime.Reset(
            _client,
            ClientInspectionExecutionSurfaceRuntime.Create(
                _client.Capture(),
                CreateRouting(ClientWorkspaceKind.Inspection)));
        ReleaseStatus.Text="Release: not evaluated.";
        RefreshDiagnosticStatus();
        RefreshCommandAvailability();
        SimulationStatus.Text="Ready.";
        RefreshWorkspaceStatus();
        RefreshProgramStatus();
        RefreshAcquisitionStatus();
        RefreshHomeStatus();
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
        if(!_clientProjection.Snapshot.Content.Inspection.HasCompletedResult)
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
        if(!_clientProjection.Snapshot.Content.Inspection.HasCompletedResult)
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
        if(!_clientProjection.Snapshot.Content.Inspection.HasCompletedResult)
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
        if(!_clientProjection.Snapshot.Content.Inspection.HasCompletedResult)
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
        if(!_clientProjection.Snapshot.Content.Inspection.HasCompletedResult)
            return;

        if(e.Key==System.Windows.Input.Key.Escape && _roiInputAdapter.Escape())
        {
            e.Handled=true;
            RefreshRoiSurface();
        }
    }

    private void RenderPreview(ClientAcquisitionPreviewSnapshot? preview)
    {
        if(preview is null)
        {
            FramePreviewImage.Source=null;
            return;
        }

        if((preview.PixelFormat.Equals("Gray8",StringComparison.OrdinalIgnoreCase) ||
            preview.PixelFormat.Equals("Mono8",StringComparison.OrdinalIgnoreCase)) &&
           preview.Width>0 &&
           preview.Height>0 &&
           preview.Width<=int.MaxValue &&
           preview.Height<=int.MaxValue &&
           preview.Payload.LongLength==preview.Width*preview.Height)
        {
            var bitmap=new WriteableBitmap(
                (int)preview.Width,
                (int)preview.Height,
                96,
                96,
                System.Windows.Media.PixelFormats.Gray8,
                null);

            var stride=(int)preview.Width;
            bitmap.WritePixels(
                new System.Windows.Int32Rect(
                    0,
                    0,
                    (int)preview.Width,
                    (int)preview.Height),
                preview.Payload,
                stride,
                0);

            FramePreviewImage.Source=bitmap;
        }
        else
        {
            FramePreviewImage.Source=null;
        }
    }

    private void RefreshRoiSurface()
    {
        RoiSurface.Children.Clear();

        if(!_clientProjection.Snapshot.Content.Inspection.HasCompletedResult)
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

    private void RefreshCommandAvailability(
        ClientInspectionWorkspaceSnapshot? snapshot=null)
    {
        var clientSnapshot=snapshot ?? _client.Capture();
        var availability=ClientCommandAvailabilityRuntime.Create(clientSnapshot);
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
        BindAcquisitionButton.IsEnabled=routing.CanBindAcquisition;
        PreviewAcquisitionButton.IsEnabled=routing.CanPreviewAcquisition;
        RunQualityButton.IsEnabled=routing.CanEvaluateSimulationQuality;
    }

    private void ApplyQualityProjection(ClientQualitySurface quality)
    {
        var filter=new ClientQualityFilter(
            ReadComboValue(QualityOutcomeFilter),
            ReadComboValue(QualitySeverityFilter));
        var projected=ClientQualitySurfaceRuntime.Create(quality.Snapshot,filter);

        QualityStatus.Text=$"{projected.AuthorityText} {projected.SummaryText} {projected.ProviderText}";
        _isSynchronizingClientControls=true;
        try
        {
            QualityFindingList.ItemsSource=projected.VisibleFindings;
            QualityFindingList.SelectedItem=projected.SelectedFinding;
        }
        finally
        {
            _isSynchronizingClientControls=false;
        }

        QualityFindingSelectionStatus.Text=projected.SelectionText;
        QualityFindingDetails.Text=projected.SelectedFinding is { } selected
            ? $"{selected.RuleCode} · {selected.Outcome} · {selected.Severity}\\n" +
              $"Evidence links: {selected.EvidenceCount}\\n{selected.Message}"
            : projected.SelectionText;
    }

    private void RefreshAcquisitionStatus()
    {
        AcquisitionStatus.Text=
            _clientProjection.Snapshot.Content.Inspection.Presentation.AcquisitionText;
    }

    private void RefreshProgramStatus()
    {
        var program=_clientProjection.Snapshot.Content.Program;
        var snapshot=program.Snapshot;

        ProgramStatus.Text=snapshot.Status==ClientProgramLoadStatus.Ready
            ? $"Program: {snapshot.Name} · v{snapshot.Version} · {program.Items.Count} step(s)."
            : snapshot.Status==ClientProgramLoadStatus.Invalid
                ? "Program: invalid."
                : "Program: none loaded.";

        _isSynchronizingClientControls=true;
        try
        {
            ProgramStepList.ItemsSource=program.Items;
            ProgramStepList.SelectedItem=program.SelectedItem;
        }
        finally
        {
            _isSynchronizingClientControls=false;
        }

        ProgramStepDetails.Text=program.SelectionText;
    }

    private void RefreshRunHistoryStatus()
    {
        var results=_clientProjection.Snapshot.Content.Results;
        var history=results.History;
        RunHistoryStatus.Text=$"History: {history.Count} runs.";
        var items=results.History;

        _isSynchronizingClientControls=true;
        try
        {
            RunHistoryList.ItemsSource=items;
            RunHistoryList.SelectedItem=results.SelectedHistoryItem;
        }
        finally
        {
            _isSynchronizingClientControls=false;
        }
    }

    private void RefreshHomeStatus()
    {
        var snapshot=_clientProjection.Snapshot;
        var home=snapshot.Content.Home;
        var workflow=snapshot.Content.Workflow;

        HomeProgramStatus.Text=$"Program: {home.ProgramStatus}";
        HomeAcquisitionStatus.Text=$"Acquisition: {home.AcquisitionStatus}";
        HomeProductionStatus.Text=$"Production: {home.ProductionStatus}";
        HomeQualityStatus.Text=$"Quality: {home.QualityStatus}";
        HomeResultsStatus.Text=$"Results: {home.ResultsStatus}";
        HomeFingerprint.Text=$"Overview: {home.Fingerprint[..12]}...";
        HomeWorkflowStatus.Text=$"Workflow: {workflow.Step}";
        HomeWorkflowMessage.Text=workflow.Message;
    }

    private void RunHistoryList_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if(_isSynchronizingClientControls ||
           RunHistoryList.SelectedItem is not ClientRunHistoryDisplayItem item)
            return;

        if(!ClientResultsCommandRuntime.SelectHistory(
            _client,
            CreateRouting(ClientWorkspaceKind.Results),
            item.Ordinal))
            return;

        var history=_clientProjection.Snapshot.Content.Results.History;
        _runHistorySelection=ClientRunHistorySelectionRuntime.Select(
            history,
            item.Ordinal,
            _runHistorySelection.SelectionSequence);

        RunHistorySelectionStatus.Text=_runHistorySelection.StatusText;
        RefreshResultStatus();
    }

    private void RefreshResultStatus()
    {
        var snapshot=_clientProjection.Snapshot;
        ApplyResultsProjection(
            snapshot.Content.Results,
            snapshot.Content.Results.History);
    }

    private void RefreshWorkspaceStatus()
    {
        var inspection=_clientProjection.Snapshot.Content.Inspection;
        WorkspaceStatus.Text=inspection.Presentation.ExecutionStatus;
        RefreshProgressPresentation(inspection.Presentation.ProgressText);
    }

}
