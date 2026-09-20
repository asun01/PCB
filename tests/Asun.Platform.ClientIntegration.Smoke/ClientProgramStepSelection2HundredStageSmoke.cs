using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProgramStepSelection2HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var program=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram();
            workspace.Load(program);
            var step=program.Steps.Last();
            var selected=workspace.SelectStep(step.StepId);
            Check(selected &&
                  workspace.Snapshot.SelectedStepId==step.StepId &&
                  workspace.SelectedStep?.StepId==step.StepId,
                  "Program step selection must change the selected step context");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
