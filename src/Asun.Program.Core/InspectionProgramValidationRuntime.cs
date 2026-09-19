namespace Asun.Program.Core;

public static class InspectionProgramValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        InspectionProgram program)
    {
        ArgumentNullException.ThrowIfNull(program);

        var errors=new List<string>();

        if(program.ProgramId==Guid.Empty)
            errors.Add("Program id cannot be empty.");
        if(string.IsNullOrWhiteSpace(program.Name))
            errors.Add("Program name cannot be blank.");
        if(program.Version is null)
            errors.Add("Program version cannot be null.");

        var stepIds=new HashSet<Guid>();
        var orders=new HashSet<int>();

        foreach(var step in program.Steps)
        {
            if(step is null)
            {
                errors.Add("Program steps cannot contain null entries.");
                continue;
            }

            if(!stepIds.Add(step.StepId))
                errors.Add("Program step ids must be unique.");

            if(!orders.Add(step.Order))
                errors.Add("Program step orders must be unique.");

            if(!step.IsValid)
                errors.Add($"Program step {step.Order} is invalid.");

            var parameterKeys=new HashSet<string>(StringComparer.Ordinal);

            foreach(var parameter in step.Parameters)
            {
                if(!parameterKeys.Add(parameter.Key))
                    errors.Add($"Program step {step.Order} parameter keys must be unique.");
            }
        }

        if(!program.Steps.SequenceEqual(
            program.Steps.OrderBy(step=>step.Order).ThenBy(step=>step.StepId)))
        {
            errors.Add("Program steps must use canonical order.");
        }

        return errors;
    }

    public static bool IsValid(
        InspectionProgram program)=>
        Validate(program).Count==0;
}
