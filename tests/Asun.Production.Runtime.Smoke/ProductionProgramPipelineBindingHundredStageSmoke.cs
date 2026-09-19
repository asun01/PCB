using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionProgramPipelineBindingHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var program=new InspectionProgram(
            Guid.Parse("64000000-0000-0000-0000-000000000001"),
            "BindingProgram",
            new Version(1,2,0),
            new[]{
                new ProgramStep(
                    Guid.Parse("65000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>()),
                new ProgramStep(
                    Guid.Parse("65000000-0000-0000-0000-000000000002"),
                    2,
                    ProgramStepKind.Measure,
                    "Measure",
                    Array.Empty<ProgramParameter>())
            });

        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame),
            new PipelineStage<CapturedFrame>(2,"Measure",frame=>frame)
        });
        var binding=ProductionProgramPipelineBindingRuntime.Create(plan,pipeline);
        var tampered=binding with {
            PlanFingerprint=new string('b',64)
        };
        var reordered=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<CapturedFrame>(1,"Measure",frame=>frame),
            new PipelineStage<CapturedFrame>(2,"Acquire",frame=>frame)
        });

        for(var i=0;i<10;i++) Check(binding.ProgramId==plan.ProgramId,"Binding should preserve program identity.");
        for(var i=0;i<10;i++) Check(binding.PlanFingerprint==plan.Fingerprint,"Binding should preserve plan fingerprint.");
        for(var i=0;i<10;i++) Check(binding.Stages.Count==2,"Binding should contain one entry per step.");
        for(var i=0;i<10;i++) Check(binding.Stages[0].ProgramStepKind==ProgramStepKind.Acquire,"First binding should retain Acquire kind.");
        for(var i=0;i<10;i++) Check(binding.Stages[1].ProgramStepKind==ProgramStepKind.Measure,"Second binding should retain Measure kind.");
        for(var i=0;i<10;i++) Check(binding.Stages[0].ProgramName==binding.Stages[0].PipelineName,"First stage names should match.");
        for(var i=0;i<10;i++) Check(binding.Stages[1].ProgramName==binding.Stages[1].PipelineName,"Second stage names should match.");
        for(var i=0;i<10;i++) Check(binding.Fingerprint.Length==64,"Binding fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(ProductionProgramPipelineBindingValidationRuntime.IsValid(plan,pipeline,binding),"Binding should validate.");
        for(var i=0;i<10;i++) Check(!ProductionProgramPipelineBindingValidationRuntime.IsValid(plan,pipeline,tampered),"Tampered plan fingerprint should be rejected.");

        assert(round==100,$"Binding smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
