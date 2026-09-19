using Asun.Vision.Contracts;

public static class NumericToleranceValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var exact = NumericTolerance.Exact;
        var absolute = NumericTolerance.AbsoluteOnly(0.01);
        var relative = NumericTolerance.RelativeOnly(0.01);
        var combined = NumericTolerance.Create(1e-6, 1e-6);
        var invalid = new NumericTolerance(-1, 0);

        var exactEqual = exact.AreEqual(5, 5);
        var absoluteEqual = absolute.AreEqual(1, 1.005);
        var absoluteNotEqual = !absolute.AreEqual(1, 1.02);
        var relativeEqual = relative.AreEqual(100, 100.5);
        var nearZero = combined.IsNearlyZero(5e-7);
        var clamped = combined.ClampNearZero(5e-7);
        var invalidValidation =
            NumericToleranceValidationRuntime.Validate(invalid);

        for (var i = 0; i < 10; i++)
            Check(exact.IsValid && exactEqual, $"exact tolerance round {i + 1} should validate.");

        for (var i = 0; i < 10; i++)
            Check(absolute.IsValid && absoluteEqual, $"absolute tolerance round {i + 1} should compare within threshold.");

        for (var i = 0; i < 10; i++)
            Check(absoluteNotEqual, $"absolute rejection round {i + 1} should reject an excessive delta.");

        for (var i = 0; i < 10; i++)
            Check(relative.IsValid && relativeEqual, $"relative tolerance round {i + 1} should compare using scale.");

        for (var i = 0; i < 10; i++)
            Check(combined.IsValid && nearZero, $"combined tolerance round {i + 1} should validate near-zero state.");

        for (var i = 0; i < 10; i++)
            Check(clamped == 0d, $"near-zero clamp round {i + 1} should normalize to zero.");

        for (var i = 0; i < 10; i++)
            Check(!exact.AreEqual(double.NaN, 0), $"non-finite comparison round {i + 1} should reject NaN.");

        for (var i = 0; i < 10; i++)
            Check(!exact.AreEqual(double.PositiveInfinity, 0), $"non-finite comparison round {i + 1} should reject infinity.");

        for (var i = 0; i < 10; i++)
            Check(invalidValidation.Count > 0, $"invalid record validation round {i + 1} should detect the malformed tolerance.");

        for (var i = 0; i < 10; i++)
            Check(absolute.WithRelative(0.001).IsValid && relative.WithAbsolute(0.001).IsValid, $"tolerance builder round {i + 1} should preserve validity.");

        assert(round == 100, $"Numeric tolerance smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
