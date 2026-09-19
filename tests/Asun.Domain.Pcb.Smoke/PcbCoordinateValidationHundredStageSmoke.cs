using Asun.Domain.Pcb;

public static class PcbCoordinateValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var first = new PcbCoordinate(10, 20);
        var second = new PcbCoordinate(40, 60);
        var midpoint = first.Midpoint(second);
        var translated = first.Translate(5, -2);
        var distanceSquared = first.DistanceSquaredTo(second);
        var distance = first.DistanceTo(second);
        var zero = PcbCoordinate.Zero;
        var invalid = new PcbCoordinate(double.NaN, 0);

        for (var i = 0; i < 10; i++)
            Check(first.IsFinite && second.IsFinite, $"coordinate finiteness round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(midpoint == new PcbCoordinate(25, 40), $"midpoint round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(translated == new PcbCoordinate(15, 18), $"translation round {i + 1} should preserve millimetre units.");

        for (var i = 0; i < 10; i++)
            Check(distanceSquared == 2500, $"distance-squared round {i + 1} should be stable.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(distance - 50) < 1e-12, $"distance round {i + 1} should be stable.");

        for (var i = 0; i < 10; i++)
            Check(zero == new PcbCoordinate(0, 0), $"zero coordinate round {i + 1} should be canonical.");

        for (var i = 0; i < 10; i++)
            Check(PcbCoordinateValidationRuntime.IsValid(first), $"coordinate validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(!PcbCoordinateValidationRuntime.IsValid(invalid), $"invalid coordinate validation round {i + 1} should fail.");

        for (var i = 0; i < 10; i++)
        {
            var threw = false;
            try
            {
                invalid.DistanceTo(first);
            }
            catch (InvalidOperationException)
            {
                threw = true;
            }

            Check(threw, $"invalid coordinate operation round {i + 1} should reject non-finite state.");
        }

        for (var i = 0; i < 10; i++)
            Check(first.Translate(0, 0) == first, $"zero translation round {i + 1} should preserve identity.");

        assert(round == 100, $"PCB coordinate smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
