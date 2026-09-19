namespace Asun.UI.Viewports;

public static class ViewportPresentationQueueValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ViewportPresentationQueueStatistics statistics)
    {
        var errors = new List<string>();

        if (statistics.Enqueued < 0 ||
            statistics.Dequeued < 0 ||
            statistics.Presented < 0 ||
            statistics.Dropped < 0 ||
            statistics.StaleRejected < 0 ||
            statistics.Cancelled < 0 ||
            statistics.Pending < 0 ||
            statistics.LatestSubmissionSequence < 0)
        {
            errors.Add("Queue counters must be non-negative.");
        }

        if (statistics.Enqueued !=
            statistics.Dequeued +
            statistics.Pending +
            statistics.Dropped)
        {
            errors.Add("Queue accounting must satisfy enqueued = dequeued + pending + dropped.");
        }

        if (statistics.Presented > statistics.Dequeued ||
            statistics.Cancelled > statistics.Dequeued)
        {
            errors.Add("Queue terminal outcomes cannot exceed dequeued packets.");
        }

        if (statistics.Presented + statistics.Cancelled >
            statistics.Dequeued)
        {
            errors.Add("Presented and cancelled packets cannot exceed dequeued packets.");
        }

        if (statistics.PresentedGeneration is null !=
            (statistics.PresentedSequence is null))
        {
            errors.Add("Presented generation and sequence must be paired.");
        }

        if (statistics.InFlightGeneration is null !=
            (statistics.InFlightSequence is null))
        {
            errors.Add("In-flight generation and sequence must be paired.");
        }

        if (statistics.CommitInProgress)
        {
            if (statistics.CommittingGeneration is null ||
                statistics.CommittingSequence is null)
            {
                errors.Add("Commit-in-progress requires a committing token.");
            }

            if (statistics.InFlightGeneration is null ||
                statistics.InFlightSequence is null)
            {
                errors.Add("Commit-in-progress requires an in-flight token.");
            }

            if (statistics.CommittingSequence is not null &&
                statistics.InFlightSequence is not null &&
                statistics.CommittingSequence != statistics.InFlightSequence)
            {
                errors.Add("Committing sequence must match the in-flight sequence.");
            }
        }
        else if (statistics.CommittingGeneration is not null ||
                 statistics.CommittingSequence is not null)
        {
            errors.Add("A non-committing queue cannot retain a committing token.");
        }

        if (statistics.PresentedSequence is long presentedSequence &&
            presentedSequence > statistics.LatestSubmissionSequence)
        {
            errors.Add("Presented sequence cannot exceed the latest submission sequence.");
        }

        if (statistics.InFlightSequence is long inFlightSequence &&
            inFlightSequence > statistics.LatestSubmissionSequence)
        {
            errors.Add("In-flight sequence cannot exceed the latest submission sequence.");
        }

        if (statistics.Pending == 0 &&
            statistics.InFlightSequence is null &&
            statistics.CommitInProgress)
        {
            errors.Add("Commit-in-progress requires an active in-flight packet.");
        }

        return errors;
    }

    public static bool IsValid(ViewportPresentationQueueStatistics statistics) =>
        Validate(statistics).Count == 0;
}
