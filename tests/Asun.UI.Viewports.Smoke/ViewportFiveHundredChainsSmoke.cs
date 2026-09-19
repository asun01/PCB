using Asun.UI.Viewports;

public static class ViewportFiveHundredChainsSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var context = BulkChainContext.CreateDefault();
        var outcomes = ViewportFiveHundredChains.RunAll(context);

        assert(
            outcomes.Count == 500,
            "The five-hundred-chain runner should execute exactly 500 chains.");

        assert(
            ViewportFiveHundredChains.AllSucceeded(outcomes),
            "All five hundred chain paths should produce finite successful results.");

        var domains = outcomes
            .Select(x => x.Domain)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        assert(
            domains.Length == 10,
            "The five hundred chains should cover all ten execution domains.");

        assert(
            outcomes.Min(x => x.Metric) >= 0 &&
            outcomes.Max(x => x.Metric) > 0,
            "The chain results should contain deterministic non-trivial metrics.");
    }
}
