using Asun.Program.Core;

public static class ProgramDefinitionHundredStageSmoke
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
        var invalid=program with
        {
            Steps=new[]{
                program.Steps[0],
                program.Steps[0]
            }
        };

        for(var i=0;i<10;i++) Check(InspectionProgramValidationRuntime.IsValid(program),"Inspection program should validate.");
        for(var i=0;i<10;i++) Check(program.Steps.Count==3,"Program should preserve three steps.");
        for(var i=0;i<10;i++) Check(program.Steps[0].Order==1 && program.Steps[1].Order==2 && program.Steps[2].Order==3,"Program steps should remain ordered.");
        for(var i=0;i<10;i++) Check(program.Steps[0].Kind==ProgramStepKind.Acquire,"First step should acquire.");
        for(var i=0;i<10;i++) Check(program.Steps[1].Kind==ProgramStepKind.Measure,"Second step should measure.");
        for(var i=0;i<10;i++) Check(program.Steps[2].Kind==ProgramStepKind.Inspect,"Third step should inspect.");
        for(var i=0;i<10;i++) Check(!InspectionProgramValidationRuntime.IsValid(invalid),"Duplicate step order/id should be rejected.");
        for(var i=0;i<10;i++) Check(program.Steps.All(step=>step.IsValid),"Every defined program step should validate.");
        for(var i=0;i<10;i++) Check(program.Steps.SelectMany(step=>step.Parameters).All(parameter=>parameter.IsValid),"Every program parameter should validate.");
        for(var i=0;i<10;i++) Check(program.Version==new Version(1,2,0),"Program version should remain deterministic.");

        assert(round==100,$"Program definition smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private static InspectionProgram CreateProgram()=>
        new(
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            "PCB-Inspection",
            new Version(1,2,0),
            new[]{
                new ProgramStep(
                    Guid.Parse("20000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    new[]{new ProgramParameter("camera","primary")}),
                new ProgramStep(
                    Guid.Parse("20000000-0000-0000-0000-000000000002"),
                    2,
                    ProgramStepKind.Measure,
                    "Measure",
                    new[]{new ProgramParameter("unit","mm")}),
                new ProgramStep(
                    Guid.Parse("20000000-0000-0000-0000-000000000003"),
                    3,
                    ProgramStepKind.Inspect,
                    "Inspect",
                    new[]{new ProgramParameter("recipe","default")})
            });
}
