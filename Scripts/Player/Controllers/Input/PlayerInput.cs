namespace Asteroids.Player.Controllers.Input
{
    public class PlayerInput : Node
    {

        public bool IsShooting { get; private set; }
        public bool IsShootingSpecial { get; private set; }

        public override void _Process(float delta)
        {
            OnFireInput();
            OnSpecialInput();
        }

        private void OnFireInput()
        {
            IsShooting = Godot.Input.IsActionPressed("ui_select");
        }

        private void OnSpecialInput()
        {
            IsShootingSpecial = Godot.Input.IsActionPressed("ui_secondary");
        }

        public Vector2 OnMovementInput()
        {
            Vector2 inputVelocity = Vector2.Zero;

            if (Godot.Input.IsActionPressed("ui_right"))
            {
                inputVelocity.x += 1;
            }
            if (Godot.Input.IsActionPressed(("ui_left")))
            {
                inputVelocity.x -= 1;
            }
            if (Godot.Input.IsActionPressed(("ui_down")))
            {
                inputVelocity.y += 1;
            }
            if (Godot.Input.IsActionPressed(("ui_up")))
            {
                inputVelocity.y -= 1;
            }

            return inputVelocity.Normalized();
        }
    }
}
