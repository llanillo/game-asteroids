using Godot;
using System;

public class PlayerRotation : Node
{
    private const float RotationSpeed = 4.5f;

    public float CalculateRotation(float delta, Vector2 inputVelocity)
    {
        return inputVelocity.x * RotationSpeed * delta;
    }
}
