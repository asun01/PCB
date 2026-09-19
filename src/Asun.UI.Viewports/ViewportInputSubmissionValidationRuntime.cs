namespace Asun.UI.Viewports;

public static class ViewportInputSubmissionValidationRuntime
{
    public static bool IsValid(
        ViewportInputSubmissionSnapshot snapshot)
    {
        if (snapshot.Pending < 0 ||
            snapshot.Submitted < 0 ||
            snapshot.Coalesced < 0 ||
            snapshot.Coalesced > snapshot.Submitted ||
            snapshot.Pending > snapshot.Submitted)
            return false;

        if (snapshot.IsCancelled && !snapshot.IsCompleted)
            return false;

        return true;
    }

    public static bool IsTerminal(
        ViewportInputSubmissionSnapshot snapshot) =>
        snapshot.IsCompleted &&
        snapshot.Pending == 0 &&
        (!snapshot.IsCancelled || snapshot.IsCancelled);
}
