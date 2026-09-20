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

    private async void RunSimulationButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        RunSimulationButton.IsEnabled=false;
        SimulationStatus.Text="Running deterministic simulation...";

        try
        {
            var definition=ClientSimulationSessionFactory.CreateDefinition();
            _workspace.Load(definition);
            RefreshWorkspaceStatus();

            var report=await _workspace.StartAsync(
                ClientSimulationSessionFactory.CreateSource());

            SimulationStatus.Text=$"Completed · {report.FrameCount} frames · {report.Fingerprint[..12]}...";
            RefreshWorkspaceStatus();
        }
        catch(OperationCanceledException)
        {
            SimulationStatus.Text="Cancelled.";
            RefreshWorkspaceStatus();
        }
        catch(Exception exception)
        {
            SimulationStatus.Text=$"Failed · {exception.Message}";
            RefreshWorkspaceStatus();
        }
        finally
        {
            RunSimulationButton.IsEnabled=true;
        }
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
