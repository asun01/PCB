using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientHomePresentation3HundredStageSmoke
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
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('b',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,2,0,new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,Guid.NewGuid(),Guid.NewGuid(),3,new string('c',64),true,"artifact")
                }))
            {
                Quality=new ClientQualityWorkspaceSnapshot(
                    Guid.NewGuid(),1,2,1,1,0,1,new string('d',64),true,
                    Array.Empty<ClientQualityFindingDisplayItem>()),
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("simulation","Simulation",true),
                    null,
                    true)
            };
            var home=ClientHomePresentationRuntime.Create(snapshot);
            Check(home.ProductionStatus.Contains("Completed") &&
                  home.QualityStatus.Contains("2 finding") &&
                  home.ResultsStatus.Contains("1 historical"),
                  "Home overview must summarize completed Production, Quality, and Results");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
