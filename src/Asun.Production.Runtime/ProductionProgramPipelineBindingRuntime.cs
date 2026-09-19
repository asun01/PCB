using System.Security.Cryptography;
using System.Text;
using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Program.Core;

namespace Asun.Production.Runtime;

public static class ProductionProgramPipelineBindingRuntime
{
    public static ProductionProgramPipelineBinding Create(
        ProgramExecutionPlan plan,
        PipelineDefinition<CapturedFrame> pipeline)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(pipeline);

        if(!ProgramExecutionPlanValidationRuntime.IsValid(
            CreateSourceProgram(plan),
            plan))
        {
            throw new ArgumentException(
                "Program execution plan is invalid.",
                nameof(plan));
        }

        if(pipeline.Stages.Count==0)
            throw new ArgumentException(
                "Pipeline must contain at least one stage.",
                nameof(pipeline));

        var steps=plan.Steps
            .OrderBy(step=>step.Order)
            .ThenBy(step=>step.StepId)
            .ToArray();
        var stages=pipeline.Stages
            .OrderBy(stage=>stage.Order)
            .ThenBy(stage=>stage.Name,StringComparer.Ordinal)
            .ToArray();

        if(steps.Length!=stages.Length)
            throw new ArgumentException(
                "Program step count must match pipeline stage count.");

        var bindings=new List<ProductionProgramPipelineStageBinding>(steps.Length);

        for(var index=0;index<steps.Length;index++)
        {
            var step=steps[index];
            var stage=stages[index];

            if(step.Order!=stage.Order ||
               !string.Equals(step.Name,stage.Name,StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    $"Program step '{step.Name}' does not bind to pipeline stage '{stage.Name}'.");
            }

            bindings.Add(
                new ProductionProgramPipelineStageBinding(
                    step.StepId,
                    step.Kind,
                    step.Order,
                    step.Name,
                    stage.Order,
                    stage.Name));
        }

        var fingerprint=CreateFingerprint(
            plan.ProgramId,
            plan.Fingerprint,
            bindings);

        return new ProductionProgramPipelineBinding(
            plan.ProgramId,
            plan.Fingerprint,
            bindings,
            fingerprint);
    }

    internal static string CreateFingerprint(
        Guid programId,
        string planFingerprint,
        IReadOnlyList<ProductionProgramPipelineStageBinding> bindings)
    {
        var builder=new StringBuilder();
        builder.Append(programId).Append('|')
            .Append(planFingerprint).Append('|');

        foreach(var binding in bindings)
        {
            builder.Append(binding.ProgramStepId).Append('|')
                .Append((int)binding.ProgramStepKind).Append('|')
                .Append(binding.ProgramOrder).Append('|')
                .Append(binding.ProgramName.Length).Append(':')
                .Append(binding.ProgramName).Append('|')
                .Append(binding.PipelineOrder).Append('|')
                .Append(binding.PipelineName.Length).Append(':')
                .Append(binding.PipelineName).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }

    private static InspectionProgram CreateSourceProgram(
        ProgramExecutionPlan plan)=>
        new(
            plan.ProgramId,
            "validated-source-program",
            plan.Version,
            plan.Steps);
}