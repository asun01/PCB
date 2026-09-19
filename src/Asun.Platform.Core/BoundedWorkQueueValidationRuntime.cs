namespace Asun.Platform.Core;

public static class BoundedWorkQueueValidationRuntime
{
    public static IReadOnlyList<string> Validate<T>(
        BoundedWorkQueue<T> queue)
    {
        ArgumentNullException.ThrowIfNull(queue);

        var errors = new List<string>();

        if (queue.Capacity <= 0)
            errors.Add("Queue capacity must be positive.");

        if (queue.IsCompleted && queue.CanWrite)
            errors.Add("A completed queue cannot remain writable.");

        if (queue.IsCompleted && queue.Completion.IsCanceled)
            errors.Add("Completion state must be observable through the completion task.");

        return errors;
    }

    public static bool IsValid<T>(BoundedWorkQueue<T> queue) =>
        Validate(queue).Count == 0;
}
