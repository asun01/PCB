namespace Asun.Domain.Quality;

public sealed record QualityInspectionSequenceRelation(long Delta)
{
    public bool IsForwardOrEqual => Delta >= 0;

    public bool IsBackward => Delta < 0;

    public bool IsConsecutive => Delta == 1;
}
