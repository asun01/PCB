using System.Numerics;

namespace Asun.UI.Viewports;

public sealed class ViewportInertialPanRuntime
{
    private Vector2 _velocity;

    public Vector2 Velocity => _velocity;

    public void SetVelocity(Vector2 velocity)
    {
        Validate(velocity);
        _velocity = velocity;
    }

    public Vector2 Step(float deltaSeconds, float damping = 8f)
    {
        if (!float.IsFinite(deltaSeconds) || deltaSeconds < 0 ||
            !float.IsFinite(damping) || damping < 0)
            throw new ArgumentOutOfRangeException();

        var displacement = _velocity * deltaSeconds;
        var factor = MathF.Exp(-damping * deltaSeconds);
        _velocity *= factor;
        if (_velocity.LengthSquared() < 1e-6f) _velocity = Vector2.Zero;
        return displacement;
    }

    public void Stop() => _velocity = Vector2.Zero;

    private static void Validate(Vector2 v)
    {
        if (!float.IsFinite(v.X) || !float.IsFinite(v.Y)) throw new ArgumentOutOfRangeException(nameof(v));
    }
}
