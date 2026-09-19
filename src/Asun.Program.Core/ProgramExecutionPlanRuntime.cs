using System.Security.Cryptography;
using System.Text;

namespace Asun.Program.Core;

public static class ProgramExecutionPlanRuntime
{
    public static ProgramExecutionPlan Create(
        InspectionProgram program)
    {
        ArgumentNullException.ThrowIfNull(program);

        if(!InspectionProgramValidationRuntime.IsValid(program))
            throw new ArgumentException(
                "Inspection program is invalid.",
                nameof(program));

        var steps=program.Steps
            .OrderBy(step=>step.Order)
            .ThenBy(step=>step.StepId)
            .ToArray();

        var builder=new StringBuilder();
        builder.Append(program.ProgramId).Append('|')
            .Append(program.Version).Append('|');

        foreach(var step in steps)
        {
            builder.Append(step.StepId).Append('|')
                .Append(step.Order).Append('|')
                .Append((int)step.Kind).Append('|')
                .Append(step.Name.Length).Append(':').Append(step.Name).Append('|');

            foreach(var parameter in step.Parameters.OrderBy(item=>item.Key,StringComparer.Ordinal))
            {
                builder.Append(parameter.Key.Length).Append(':').Append(parameter.Key)
                    .Append('=')
                    .Append(parameter.Value.Length).Append(':').Append(parameter.Value)
                    .Append('|');
            }
        }

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();

        return new ProgramExecutionPlan(
            program.ProgramId,
            program.Version,
            steps,
            fingerprint);
    }
}
