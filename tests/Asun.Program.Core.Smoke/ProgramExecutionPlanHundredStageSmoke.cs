using Asun.Program.Core;

public static class ProgramExecutionPlanHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var program=CreateProgram();
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var tampered=plan with {Fingerprint=new string('a',64)};
        var reversed=plan with {Steps=plan.Steps.Reverse().ToArray()};

        for(var i=0;i<10;i++) Check(plan.ProgramId==program.ProgramId,"Execution plan should retain program identity.");
        for(var i=0;i<10;i++) Check(plan.Version==program.Version,"Execution plan should retain program version.");
        for(var i=0;i<10;i++) Check(plan.Steps.Count==3,"Execution plan should retain three steps.");
        for(var i=0;i<10;i++) Check(ProgramExecutionPlanValidationRuntime.IsValid(program,plan),"Execution plan should validate.");
        for(var i=0;i<10;i++) Check(!ProgramExecutionPlanValidationRuntime.IsValid(program,tampered),"Tampered fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!ProgramExecutionPlanValidationRuntime.IsValid(program,reversed),"Reversed steps should be rejected.");
        for(var i=0;i<10;i++) Check(plan.Fingerprint.Length==64,"Execution plan fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(plan.Fingerprint.All(Uri.IsHexDigit),"Execution plan fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(ProgramExecutionPlanRuntime.Create(program)==plan,"Execution plan creation should be deterministic.");
        for(var i=0;i<10;i++) Check(InspectionProgramValidationRuntime.IsValid(program),"Source program should remain valid.");

        assert(round==100,$"Program execution plan smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private static InspectionProgram CreateProgram()=>
        new(
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            "PCB-Inspection",
            new Version(1,2,0),
            new[]{
                new ProgramStep(Guid.Parse("20000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",new[]{new ProgramParameter("camera","primary")}),
                new ProgramStep(Guid.Parse("20000000-0000-0000-0000-000000000002"),2,ProgramStepKind.Measure,"Measure",new[]{new ProgramParameter("unit","mm")}),
                new ProgramStep(Guid.Parse("20000000-0000-0000-0000-000000000003"),3,ProgramStepKind.Inspect,"Inspect",new[]{new ProgramParameter("recipe","default")})
            });
}
