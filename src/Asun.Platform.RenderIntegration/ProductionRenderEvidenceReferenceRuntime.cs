using Asun.Platform.Evidence;

namespace Asun.Platform.RenderIntegration;

public sealed record ProductionRenderEvidenceReference(
    long Sequence,
    string RenderFingerprint,
    EvidenceHandle EvidenceHandle);

public static class ProductionRenderEvidenceReferenceRuntime
{
    public static IReadOnlyList<ProductionRenderEvidenceReference> Create(
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames,
        IReadOnlyList<EvidenceHandle> handles)
    {
        ArgumentNullException.ThrowIfNull(frames);
        ArgumentNullException.ThrowIfNull(handles);

        if(frames.Count!=handles.Count)
            throw new ArgumentException("Render frame and Evidence handle counts must match.",nameof(handles));

        var ordered=frames.OrderBy(frame=>frame.Sequence).ToArray();
        var result=new List<ProductionRenderEvidenceReference>(ordered.Length);

        for(var index=0;index<ordered.Length;index++)
        {
            var frame=ordered[index];
            var handle=handles[index];
            if(!handle.IsValid)
                throw new ArgumentException($"Evidence handle {index} is invalid.",nameof(handles));
            if(frame.RenderFingerprint.Length!=64)
                throw new ArgumentException($"Render fingerprint {index} is invalid.",nameof(frames));

            result.Add(new ProductionRenderEvidenceReference(
                frame.Sequence,
                frame.RenderFingerprint,
                handle));
        }

        if(result.Select(item=>item.EvidenceHandle).Distinct().Count()!=result.Count)
            throw new ArgumentException("Evidence handles must be unique.",nameof(handles));

        return result;
    }

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames,
        IReadOnlyList<ProductionRenderEvidenceReference> references)
    {
        ArgumentNullException.ThrowIfNull(frames);
        ArgumentNullException.ThrowIfNull(references);

        var errors=new List<string>();
        var orderedFrames=frames.OrderBy(frame=>frame.Sequence).ToArray();
        var orderedReferences=references.OrderBy(reference=>reference.Sequence).ToArray();

        if(orderedFrames.Length!=orderedReferences.Length)
            errors.Add("Render Evidence reference count must match render frame count.");

        var count=Math.Min(orderedFrames.Length,orderedReferences.Length);
        for(var index=0;index<count;index++)
        {
            var frame=orderedFrames[index];
            var reference=orderedReferences[index];

            if(!reference.EvidenceHandle.IsValid)
                errors.Add($"Render Evidence reference {index} has an invalid opaque handle.");
            if(reference.Sequence!=frame.Sequence)
                errors.Add($"Render Evidence reference {index} sequence mismatch.");
            if(reference.RenderFingerprint!=frame.RenderFingerprint)
                errors.Add($"Render Evidence reference {index} render fingerprint mismatch.");
        }

        if(orderedReferences.Select(item=>item.Sequence).Distinct().Count()!=orderedReferences.Length)
            errors.Add("Render Evidence reference sequences must be unique.");
        if(orderedReferences.Select(item=>item.EvidenceHandle).Distinct().Count()!=orderedReferences.Length)
            errors.Add("Render Evidence handles must be unique.");

        return errors;
    }

    public static bool IsValid(
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames,
        IReadOnlyList<ProductionRenderEvidenceReference> references)=>
        Validate(frames,references).Count==0;
}
