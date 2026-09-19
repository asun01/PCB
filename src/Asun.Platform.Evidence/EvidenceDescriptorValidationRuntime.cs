namespace Asun.Platform.Evidence;

public static class EvidenceDescriptorValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=new List<string>();

        if(!descriptor.Handle.IsValid)
            errors.Add("Evidence handle must be valid.");

        if(descriptor.Kind is < EvidenceKind.Unknown or > EvidenceKind.Other)
            errors.Add("Evidence kind is invalid.");

        if(string.IsNullOrWhiteSpace(descriptor.MediaType))
            errors.Add("Evidence media type cannot be blank.");

        if(descriptor.ByteLength is < 0)
            errors.Add("Evidence byte length cannot be negative.");

        return errors;
    }

    public static bool IsValid(
        EvidenceDescriptor descriptor)=>
        Validate(descriptor).Count==0;
}
