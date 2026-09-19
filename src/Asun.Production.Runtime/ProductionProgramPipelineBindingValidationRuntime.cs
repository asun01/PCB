using Asun.Device.Contracts;
using Asun.Platform.Pipeline;

namespace Asun.Production.Runtime;

public static class ProductionProgramPipelineBindingValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProgramExecutionPlan plan,
        PipelineDefinition<CapturedFrame> pipeline,
        ProductionProgramPipelineBinding binding)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(pipeline);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=new List<string>();

        if(binding.ProgramId!=plan.ProgramId)
            errors.Add("Program-pipeline binding program id must match the plan.");

        if(binding.PlanFingerprint!=plan.Fingerprint)
            errors.Add("Program-pipeline binding plan fingerprint must match the plan.");

        if(binding.Stages.Count!=plan.Steps.Count ||
           binding.Stages.Count!=pipeline.Stages.Count)
        {
            errors.Add("Program-pipeline binding stage count must match both plan and pipeline.");
        }

        var steps=plan.Steps
            .OrderBy(step=>step.Order)
            .ThenBy(step=>step.StepId)
            .ToArray();
        var stages=pipeline.Stages
            .OrderBy(stage=>stage.Order)
            .ThenBy(stage=>stage.Name,StringComparer.Ordinal)
            .ToArray();

        var count=Math.Min(
            binding.Stages.Count,
            Math.Min(steps.Length,stages.Length));

        for(var index=0;index<count;index++)
        {
            var actual=binding.Stages[index];
            var step=steps[index];
            var stage=stages[index];

            if(actual.ProgramStepId!=step.StepId)
                errors.Add($"Binding stage {index} program step id mismatch.");

            if(actual.ProgramStepKind!=step.Kind)
                errors.Add($"Binding stage {index} program step kind mismatch.");

            if(actual.ProgramOrder!=step.Order)
                errors.Add($"Binding stage {index} program order mismatch.");

            if(!string.Equals(actual.ProgramName,step.Name,StringComparison.Ordinal))
                errors.Add($"Binding stage {index} program name mismatch.");

            if(actual.PipelineOrder!=stage.Order)
                errors.Add($"Binding stage {index} pipeline order mismatch.");

            if(!string.Equals(actual.PipelineName,stage.Name,StringComparison.Ordinal))
                errors.Add($"Binding stage {index} pipeline name mismatch.");
        }

        if(binding.Fingerprint.Length!=64 ||
           !binding.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Program-pipeline binding fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count==0)
        {
            var expected=ProductionProgramPipelineBindingRuntime.CreateFingerprint(
                plan.ProgramId,
                plan.Fingerprint,
                binding.Stages);

            if(expected!=binding.Fingerprint)
                errors.Add("Program-pipeline binding fingerprint does not match its canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProgramExecutionPlan plan,
        PipelineDefinition<CapturedFrame> pipeline,
        ProductionProgramPipelineBinding binding)=>
        Validate(plan,pipeline,binding).Count==0;
}