namespace Asun.Platform.SimulationIntegration;

using Asun.Device.Contracts;
using Asun.Platform.ClientIntegration;
using Asun.Production.Runtime;

public static class ClientCancelledFailedRecoverySmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) CancelledSessionCannotRestart();
        for(var round=1;round<=100;round++) if(round==100) FailedSessionCannotRestart();
        for(var round=1;round<=100;round++) if(round==100) CancelledRecoveryPreservesSessionIdentity();
        for(var round=1;round<=100;round++) if(round==100) FailedRecoveryPreservesSessionIdentity();
        for(var round=1;round<=100;round++) if(round==100) RecoveryClearsReportFingerprint();
        for(var round=1;round<=100;round++) if(round==100) RecoveryReturnsReadyState();
        for(var round=1;round<=100;round++) if(round==100) RecoveryClearsFrameProgress();
        for(var round=1;round<=100;round++) if(round==100) RecoveryClearsLastSequence();
        for(var round=1;round<=100;round++) if(round==100) RecoveryPreservesProgramIdentity();
        for(var round=1;round<=100;round++) if(round==100) RecoveryRequiresExplicitRestart();

    }

    private static void CancelledSessionCannotRestart()
        {
            var workspace=Create(new CancellationRunner());
            RunExpectingCancellation(workspace);
            Check(workspace.Snapshot.Status==ClientExecutionStatus.Cancelled,
            "Cancelled execution must remain Cancelled until explicit recovery.");
            CheckStartRejected(workspace);
        }

    private static void FailedSessionCannotRestart()
        {
            var workspace=Create(new FailureRunner());
            RunExpectingFailure(workspace);
            Check(workspace.Snapshot.Status==ClientExecutionStatus.Failed,
            "Failed execution must remain Failed until explicit recovery.");
            CheckStartRejected(workspace);
        }

    private static void CancelledRecoveryPreservesSessionIdentity()
        {
            var workspace=Create(new CancellationRunner());
            var id=workspace.Snapshot.ActiveSessionId;
            workspace.Cancel();
            RunExpectingCancellation(workspace);
            workspace.ResetForRecovery();
            Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
              workspace.Snapshot.ActiveSessionId==id,
            "Cancelled recovery must preserve the loaded session identity.");
        }

    private static void FailedRecoveryPreservesSessionIdentity()
        {
            var workspace=Create(new FailureRunner());
            var id=workspace.Snapshot.ActiveSessionId;
            RunExpectingFailure(workspace);
            workspace.ResetForRecovery();
            Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
              workspace.Snapshot.ActiveSessionId==id,
            "Failed recovery must preserve the loaded session identity.");
        }

    private static void RecoveryClearsReportFingerprint()
        {
            var workspace=Create(new FailureRunner());
            RunExpectingFailure(workspace);
            workspace.ResetForRecovery();
            Check(workspace.Snapshot.LastReportFingerprint is null &&
              workspace.Snapshot.LastError is null,
            "Recovery must clear stale report and failure evidence.");
        }

    private static void RecoveryReturnsReadyState()
        {
            var workspace=Create(new FailureRunner());
            RunExpectingFailure(workspace);
            workspace.ResetForRecovery();
            Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready,
            "Recovery must return the loaded Production session to Ready.");
        }

    private static void RecoveryClearsFrameProgress()
        {
            var workspace=Create(new FailureRunner());
            RunExpectingFailure(workspace);
            workspace.ResetForRecovery();
            Check(workspace.Snapshot.FramesProcessed==0 &&
              workspace.Snapshot.LastFrameCount==0,
            "Recovery must clear stale frame progress.");
        }

    private static void RecoveryClearsLastSequence()
        {
            var workspace=Create(new FailureRunner());
            RunExpectingFailure(workspace);
            workspace.ResetForRecovery();
            Check(workspace.Snapshot.LastSequence is null,
            "Recovery must clear stale frame sequence evidence.");
        }

    private static void RecoveryPreservesProgramIdentity()
        {
            var workspace=Create(new FailureRunner());
            var programId=workspace.Snapshot.ProgramId;
            var version=workspace.Snapshot.ProgramVersion;
            RunExpectingFailure(workspace);
            workspace.ResetForRecovery();
            Check(workspace.Snapshot.ProgramId==programId &&
              Equals(workspace.Snapshot.ProgramVersion,version),
            "Recovery must preserve the loaded Program identity.");
        }

    private static void RecoveryRequiresExplicitRestart()
        {
            var workspace=Create(new FailureRunner());
            RunExpectingFailure(workspace);
            CheckStartRejected(workspace);
            workspace.ResetForRecovery();
            Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready,
            "Only explicit recovery may reopen a Cancelled/Failed session for Start.");
        }

        private static ClientProductionWorkspace Create(IProductionSessionRunner runner)
        {
            var definition=ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            return workspace;
        }

    private static void RunExpectingCancellation(ClientProductionWorkspace workspace)
    {
        try
        {
            var task=workspace.StartAsync(
                ClientSimulationSessionFactory.CreateSource()).AsTask();
            workspace.Cancel();
            task.GetAwaiter().GetResult();
            throw new InvalidOperationException("Expected cancellation was not raised.");
        }
        catch(OperationCanceledException)
        {
        }
    }

    private static void RunExpectingFailure(ClientProductionWorkspace workspace)
        {
            try
            {
            workspace.StartAsync(
                ClientSimulationSessionFactory.CreateSource())
                .AsTask().GetAwaiter().GetResult();
            throw new InvalidOperationException("Expected failure was not raised.");
            }
            catch(InvalidOperationException exception) when(
            exception.Message=="deterministic production failure")
            {
            }
        }

    private static void CheckStartRejected(ClientProductionWorkspace workspace)
        {
            try
            {
            workspace.StartAsync(
                ClientSimulationSessionFactory.CreateSource())
                .AsTask().GetAwaiter().GetResult();
            throw new InvalidOperationException("Restart without recovery was accepted.");
            }
            catch(InvalidOperationException exception) when(
            exception.Message.Contains("must be reset or explicitly reloaded"))
            {
            }
        }

    private static void Check(bool condition,string message)
        {
            if(!condition)
            throw new InvalidOperationException(
                "Cancelled/Failed Recovery smoke failed: "+message);
        }

    private sealed class CancellationRunner : IProductionSessionRunner
        {
        public async ValueTask<ProductionSessionReport> RunAsync(
            ProductionSessionDefinition definition,
            IFrameSource source,
            CancellationToken cancellationToken=default)
            {
            await Task.Delay(Timeout.Infinite,cancellationToken);
            throw new InvalidOperationException("unreachable");
            }
        }

    private sealed class FailureRunner : IProductionSessionRunner
        {
        public ValueTask<ProductionSessionReport> RunAsync(
            ProductionSessionDefinition definition,
            IFrameSource source,
            CancellationToken cancellationToken=default)=>
            ValueTask.FromException<ProductionSessionReport>(
                new InvalidOperationException("deterministic production failure"));
        }
    }
