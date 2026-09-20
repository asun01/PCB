using System.Security.Cryptography;
using System.Text;
using Asun.Device.Impl;
using Asun.Platform.Evidence;
using Asun.Production.Runtime;

namespace Asun.Platform.CaptureEvidenceIntegration;

public sealed record ProductionCaptureSessionEvidenceClosure(
    Guid SessionId,
    int FrameCount,
    string SessionReconciliationFingerprint,
    string EvidenceProjectionFingerprint,
    string ClosureFingerprint);

public static class ProductionCaptureSessionEvidenceClosureRuntime
{
    public static ProductionCaptureSessionEvidenceClosure Create(
        ProductionSessionReport productionReport,
        CaptureSessionSnapshot captureSession,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionEvidenceFrameReference> evidenceFrames)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(captureSession);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(evidenceFrames);

        var errors=Validate(productionReport,captureSession,provenance,evidenceFrames);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var reconciliation=ProductionCaptureSessionReconciliationRuntime.Create(
            productionReport,
            captureSession);
        var references=ProductionCaptureEvidenceProjectionRuntime.Create(
            productionReport,
            provenance,
            evidenceFrames);
        var projectionFingerprint=ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references);
        var canonical=string.Join("|",
            productionReport.SessionId,
            productionReport.FrameCount,
            reconciliation.ReconciliationFingerprint,
            projectionFingerprint);
        var closureFingerprint=Hash(canonical);

        return new ProductionCaptureSessionEvidenceClosure(
            productionReport.SessionId,
            productionReport.FrameCount,
            reconciliation.ReconciliationFingerprint,
            projectionFingerprint,
            closureFingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        CaptureSessionSnapshot captureSession,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionEvidenceFrameReference> evidenceFrames)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(captureSession);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(evidenceFrames);

        var errors=new List<string>();
        errors.AddRange(
            ProductionCaptureSessionReconciliationRuntime.Validate(
                productionReport,
                captureSession));

        errors.AddRange(
            ProductionCaptureEvidenceProjectionValidationRuntime.Validate(
                productionReport,
                evidenceFrames));

        if(provenance.Count!=productionReport.FrameCount)
            errors.Add("Capture provenance count must match production frame count.");

        if(provenance.Count==productionReport.FrameCount)
        {
            try
            {
                ProductionCaptureEvidenceProjectionRuntime.Create(
                    productionReport,
                    provenance,
                    evidenceFrames);
            }
            catch(Exception exception) when(exception is ArgumentException or InvalidOperationException)
            {
                errors.Add(exception.Message);
            }
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        CaptureSessionSnapshot captureSession,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionEvidenceFrameReference> evidenceFrames)=>
        Validate(
            productionReport,
            captureSession,
            provenance,
            evidenceFrames).Count==0;

    public static bool IsEquivalent(
        ProductionCaptureSessionEvidenceClosure left,
        ProductionCaptureSessionEvidenceClosure right)=>
        left.SessionId==right.SessionId &&
        left.FrameCount==right.FrameCount &&
        left.SessionReconciliationFingerprint==right.SessionReconciliationFingerprint &&
        left.EvidenceProjectionFingerprint==right.EvidenceProjectionFingerprint &&
        left.ClosureFingerprint==right.ClosureFingerprint;

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
