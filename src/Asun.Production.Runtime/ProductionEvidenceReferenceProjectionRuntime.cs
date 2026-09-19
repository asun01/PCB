using System.Security.Cryptography;
using System.Text;
using Asun.Platform.Evidence;

namespace Asun.Production.Runtime;

public static class ProductionEvidenceReferenceProjectionRuntime
{
    public static ProductionEvidenceReferenceProjection Create(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionEvidenceFrameReference> frameReferences)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(frameReferences);

        if(productionReport.FrameCount!=frameReferences.Count)
            throw new ArgumentException(
                "Evidence frame-reference count must match the production frame count.",
                nameof(frameReferences));

        var frames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var references=frameReferences
            .OrderBy(item=>item.Sequence)
            .ToArray();

        var links=new List<ProductionEvidenceFrameReference>(references.Count);
        for(var index=0;index<references.Length;index++)
        {
            var frame=frames[index];
            var reference=references[index];

            if(reference.Sequence!=frame.Sequence.Value)
                throw new ArgumentException(
                    $"Evidence reference sequence {reference.Sequence} does not match production sequence {frame.Sequence.Value}.");

            var set=new EvidenceReferenceSet(reference.Handles);
            if(!EvidenceReferenceSetValidationRuntime.IsValid(set))
                throw new ArgumentException(
                    $"Evidence references for sequence {reference.Sequence} are not canonical.");

            links.Add(
                new ProductionEvidenceFrameReference(
                    reference.Sequence,
                    reference.Handles.ToArray()));
        }

        var fingerprint=CreateFingerprint(
            productionReport.SessionId,
            productionReport.Fingerprint,
            links);

        return new ProductionEvidenceReferenceProjection(
            productionReport.SessionId,
            productionReport.Fingerprint,
            links,
            fingerprint);
    }

    internal static string CreateFingerprint(
        Guid productionSessionId,
        string productionFingerprint,
        IReadOnlyList<ProductionEvidenceFrameReference> frames)
    {
        var builder=new StringBuilder();
        builder.Append(productionSessionId).Append('|')
            .Append(productionFingerprint).Append('|');

        foreach(var frame in frames.OrderBy(item=>item.Sequence))
        {
            builder.Append(frame.Sequence).Append('|');
            foreach(var handle in frame.Handles.OrderBy(item=>item.Value,StringComparer.Ordinal))
                builder.Append(handle.Value.Length).Append(':').Append(handle.Value).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
