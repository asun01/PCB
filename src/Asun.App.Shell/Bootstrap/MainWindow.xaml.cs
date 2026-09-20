using Asun.Platform.ClientIntegration;
using Asun.Platform.SimulationIntegration;

namespace Asun.App.Shell.Bootstrap;

public partial class MainWindow : System.Windows.Window
{
    private readonly ClientProductionWorkspace _workspace=new();

    public MainWindow()
    {
        InitializeComponent();
        RefreshWorkspaceStatus();
    }

    private void LoadSimulationButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        try
        {
            var definition=ClientSimulationSessionFactory.CreateDefinition();
            _workspace.Load(definition);
            SimulationStatus.Text="Simulation session loaded.";
            ReleaseStatus.Text="Release: not evaluated.";
            RefreshWorkspaceStatus();
        }
        catch(Exception exception)
        {
            SimulationStatus.Text=$"Load failed · {exception.Message}";
            ReleaseStatus.Text="Release: not evaluated.";
            RefreshWorkspaceStatus();
        }
    }

    private async void RunSimulationButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        LoadSimulationButton.IsEnabled=false;
        RunSimulationButton.IsEnabled=false;
        ResetSessionButton.IsEnabled=false;
        SimulationStatus.Text="Running deterministic simulation...";

        try
        {
            if(_workspace.Snapshot.Status==ClientExecutionStatus.Idle)
            {
                _workspace.Load(ClientSimulationSessionFactory.CreateDefinition());
                RefreshWorkspaceStatus();
            }

            var report=await _workspace.StartAsync(
                ClientSimulationSessionFactory.CreateSource());

            var replay=ClientProductionReplaySnapshotRuntime.Create(
                _workspace.Snapshot,
                report);
            var release=ClientReleaseProjectionRuntime.Create(
                replay,
                ClientSimulationSessionFactory.CreateReleaseManifest());

            SimulationStatus.Text=$"Completed · {report.FrameCount} frames · replay {replay.ReplayFingerprint[..12]}...";
            ReleaseStatus.Text=release.ReleaseReady
                ? $"Release: Ready · {release.ArtifactPath}"
                : "Release: Not ready.";
            RefreshWorkspaceStatus();
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
        }
    }

    private void ResetSessionButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        _workspace.Reset();
        SimulationStatus.Text="Ready.";
        ReleaseStatus.Text="Release: not evaluated.";
        RefreshWorkspaceStatus();
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
