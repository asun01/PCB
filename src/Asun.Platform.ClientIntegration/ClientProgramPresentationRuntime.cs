using Asun.Program.Core;

namespace Asun.Platform.ClientIntegration;

public sealed record ClientProgramDisplayItem(
    int Order,
    string Name,
    string Kind,
    int ParameterCount)
{
    public Guid StepId { get; init; }
    public string ParameterSummary { get; init; }="";
};

public static class ClientProgramPresentationRuntime
{
    public static IReadOnlyList<ClientProgramDisplayItem> CreateItems(
        InspectionProgram? program)
    {
        if(program is null)
            return Array.Empty<ClientProgramDisplayItem>();

        var errors=InspectionProgramValidationRuntime.Validate(program);
        if(errors.Count>0)
            throw new ArgumentException(
                string.Join(" ",errors),
                nameof(program));

        return program.Steps
            .OrderBy(step=>step.Order)
            .ThenBy(step=>step.StepId)
            .Select(step=>new ClientProgramDisplayItem(
                step.Order,
                step.Name,
                step.Kind.ToString(),
                step.Parameters.Count)
            {
                StepId=step.StepId,
                ParameterSummary=string.Join(
                    ", ",
                    step.Parameters
                        .OrderBy(parameter=>parameter.Key,StringComparer.Ordinal)
                        .Select(parameter=>$"{parameter.Key}={parameter.Value}"))
            })
            .ToArray();
    }

    public static bool IsValid(
        InspectionProgram? program,
        IReadOnlyList<ClientProgramDisplayItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        try
        {
            return items.SequenceEqual(CreateItems(program));
        }
        catch(ArgumentException)
        {
            return false;
        }
    }
}
