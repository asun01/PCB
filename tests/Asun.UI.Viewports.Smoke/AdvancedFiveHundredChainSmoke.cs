using System.Reflection;
using Asun.UI.Viewports.AdvancedChains;

public static class AdvancedFiveHundredChainSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var assembly = typeof(AdvancedChainResult).Assembly;
        var types = assembly
            .GetTypes()
            .Where(type =>
                type.IsClass &&
                type.IsAbstract &&
                type.IsSealed &&
                type.Namespace?.StartsWith(
                    "Asun.UI.Viewports.AdvancedChains.",
                    StringComparison.Ordinal) == true &&
                type.Name.EndsWith("Chain" + type.Name[^3..], StringComparison.Ordinal))
            .Where(type => type.Name.Length >= 8)
            .ToArray();

        var chainTypes = types
            .Where(type => type.GetMethod(
                "Execute",
                BindingFlags.Public | BindingFlags.Static) is not null)
            .ToArray();

        assert(
            chainTypes.Length == 500,
            $"Expected 500 executable chain types, found {chainTypes.Length}.");

        var executed = 0;

        foreach (var type in chainTypes.OrderBy(type => type.FullName))
        {
            var method = type.GetMethod(
                "Execute",
                BindingFlags.Public | BindingFlags.Static)!;

            var value = ((double)((executed % 37) - 18)) + 0.25d;
            var result = (AdvancedChainResult)method.Invoke(null, new object[] { value })!;

            assert(
                result.Valid &&
                result.Steps == 5 &&
                double.IsFinite(result.Value),
                $"{type.FullName} returned an invalid chain result.");

            executed++;
        }

        assert(
            executed == 500,
            $"Expected to execute 500 chains, executed {executed}.");
    }
}
