namespace Asun.Platform.Evidence;

public static class EvidenceDescriptorQueryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceDescriptorQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var errors=new List<string>();

        if(!query.HasCriteria)
            errors.Add("Evidence descriptor query must contain at least one criterion.");

        if(query.Kind is not null &&
           !Enum.IsDefined(query.Kind.Value))
        {
            errors.Add("Evidence descriptor query kind must be defined.");
        }

        if(query.MediaType is not null &&
           string.IsNullOrWhiteSpace(query.MediaType))
        {
            errors.Add("Evidence descriptor query media type must not be blank.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceDescriptorQuery query)=>
        Validate(query).Count==0;
}
