using Godot;

namespace Asteroids.Player.Controllers.Movement
{
    public class PlayerRotation : Node
    {
        private const float RotationSpeed = 6.5f;

        public float CalculateRotation(float delta, Vector2 inputVelocity)
        {
            return inputVelocity.x * RotationSpeed * delta;
        }
    }
}
