using Godot;

namespace Asteroids.Scripts.Player.Controllers.Movement
{
    public class PlayerMovement : Node
    {
        [Export] private float _speed = 500;
    
        private const float Acceleration = 0.2f;
        private const float Friction = 0.02f;
        private const float WorldMarginOffset = 20.0f;

        public Vector2 Velocity => _velocity;
        private Vector2 _velocity = Vector2.Zero;

        /*
     * Returns player's linear movement given its rotation and input
     */
        public Vector2 GetLinearMovement(float rotation, float delta, Vector2 inputVelocity)
        {
            var newVelocity = new Vector2(inputVelocity.y, 0).Rotated(rotation) * _speed;
            _velocity = newVelocity.Length() > 0
                ? _velocity.LinearInterpolate(newVelocity, Acceleration)
                : _velocity.LinearInterpolate(Vector2.Zero, Friction);
            return _velocity * delta;
        }

        /*
     * Return the player's clamped position to the limits of the map
     */
        public Vector2 ClampPosition(Vector2 position, Vector2 worldMargin)
        {
            float xLimit = worldMargin.x;
            float yLimit = worldMargin.y;
        
            return new Vector2(Mathf.Clamp(position.x, WorldMarginOffset, xLimit - WorldMarginOffset),
                Mathf.Clamp(position.y, WorldMarginOffset, yLimit - WorldMarginOffset));
        }
    }
}
