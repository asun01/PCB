using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Production.Runtime;

namespace Asun.Platform.PcbProductionIntegration;

public static class PcbProductionQualityProvenanceBundleValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbAssemblySnapshot assembly,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        QualityInspectionRun qualityRun,
        PcbProductionQualityProvenanceBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(bundle);

        var errors=new List<string>();

        if(!PcbAssemblySnapshotValidationRuntime.IsValid(assembly))
            errors.Add("PCB assembly snapshot is invalid.");

        if(!ProductionSessionValidationRuntime.IsValid(definition,productionReport))
            errors.Add("Production session is invalid.");

        if(!ProductionFrameProvenanceRuntime.IsValid(productionReport,provenance))
            errors.Add("Production frame provenance is invalid.");

        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            errors.Add("Quality inspection run is invalid.");

        if(bundle.AssemblyFingerprint!=assembly.Fingerprint)
            errors.Add("Bundle assembly fingerprint must match the assembly.");

        if(bundle.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Bundle production session id must match the report.");

        if(bundle.ProductionFingerprint!=productionReport.Fingerprint)
            errors.Add("Bundle production fingerprint must match the report.");

        if(bundle.QualityRunId!=qualityRun.RunId)
            errors.Add("Bundle Quality run id must match the run.");

        if(bundle.FrameCount!=productionReport.FrameCount ||
           bundle.FrameCount!=provenance.Count ||
           bundle.FrameCount!=qualityRun.ResultCount)
        {
            errors.Add("Bundle frame count must match production, provenance, and Quality counts.");
        }

        if(bundle.ComponentCount!=assembly.Components.Count)
            errors.Add("Bundle component count must match the assembly.");

        if(bundle.Fingerprint.Length!=64 ||
           !bundle.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Bundle fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=PcbProductionQualityProvenanceBundleRuntime.CreateFingerprint(
            assembly,
            productionReport,
            provenance,
            qualityRun);

        if(expected!=bundle.Fingerprint)
            errors.Add("Bundle fingerprint does not match its canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbAssemblySnapshot assembly,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        QualityInspectionRun qualityRun,
        PcbProductionQualityProvenanceBundle bundle)=>
        Validate(assembly,definition,productionReport,provenance,qualityRun,bundle).Count==0;
}
