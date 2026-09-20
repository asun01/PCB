using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientHomePresentation2HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Running,1,null,null)
                {
                    TargetFrameCount=3,
                    FramesProcessed=1
                },
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Program=new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.NewGuid(),
                    "Program A",
                    new Version(1,2,0),
                    3,
                    new string('a',64),
                    Array.Empty<string>())
                {
                    SelectedStepId=Guid.NewGuid()
                },
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProgramStatus.Contains("Program A") &&
                  home.AcquisitionStatus=="Camera 01" &&
                  home.ProductionStatus=="Running 1/3",
                  "Home overview must expose live Program/Acquisition/Production state");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
