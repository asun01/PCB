namespace Asun.Program.Core;

public static class ProgramExecutionPlanValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        InspectionProgram program,
        ProgramExecutionPlan plan)
    {
        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(plan);

        var errors=new List<string>(
            InspectionProgramValidationRuntime.Validate(program));

        if(plan.ProgramId!=program.ProgramId)
            errors.Add("Execution plan program id must match the source program.");

        if(plan.Version!=program.Version)
            errors.Add("Execution plan version must match the source program.");

        var expected=ProgramExecutionPlanRuntime.Create(program);

        if(!plan.Steps.SequenceEqual(expected.Steps))
            errors.Add("Execution plan steps do not match the source program.");

        if(plan.Fingerprint.Length!=64 ||
           !plan.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Execution plan fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(plan.Fingerprint!=expected.Fingerprint)
            errors.Add("Execution plan fingerprint does not match the source program.");

        return errors;
    }

    public static bool IsValid(
        InspectionProgram program,
        ProgramExecutionPlan plan)=>
        Validate(program,plan).Count==0;
}
