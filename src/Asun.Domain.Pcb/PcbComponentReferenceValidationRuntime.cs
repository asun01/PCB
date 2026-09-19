namespace Asun.Domain.Pcb;

public static class PcbComponentReferenceValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbComponentReference component,
        PcbBoardDefinition? board=null)
    {
        ArgumentNullException.ThrowIfNull(component);

        var errors=new List<string>();

        if(!component.Id.IsValid)
            errors.Add("Component id must be valid.");
        if(string.IsNullOrWhiteSpace(component.Designator))
            errors.Add("Component designator cannot be blank.");
        if(string.IsNullOrWhiteSpace(component.Value))
            errors.Add("Component value cannot be blank.");
        if(string.IsNullOrWhiteSpace(component.PackageName))
            errors.Add("Component package name cannot be blank.");
        if(component.LayerIndex<0)
            errors.Add("Component layer index cannot be negative.");
        if(board is not null && board.IsValid && component.LayerIndex>=board.LayerCount)
            errors.Add("Component layer index must be inside the board layer count.");
        if(!component.Position.IsFinite || !double.IsFinite(component.RotationRadians))
            errors.Add("Component placement must be finite.");

        return errors;
    }

    public static bool IsValid(
        PcbComponentReference component,
        PcbBoardDefinition? board=null)=>
        Validate(component,board).Count==0;
}
