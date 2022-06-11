using Asteroids.Scripts.Player.Controllers.Animation;
using Asteroids.Scripts.Player.Controllers.Attack;
using Asteroids.Scripts.Player.Controllers.Input;
using Godot;

namespace Asteroids.Scripts.Player.Controllers.Manager
{
    public class PlayerManager : Area2D
    {
        [Signal] public delegate void HitSignal();
    
        private PlayerInput _playerInput;
        private PlayerMovement _playerMovement;
        private PlayerRotation _playerRotation;
    
        private PlayerSpecialAttack _playerSpecialAttack;
        private PlayerAttack _playerAttack;

        private PlayerAnimation _playerAnimation;
    
        private CollisionPolygon2D _collisionPolygon;
        private Rect2 _viewportRect;
    
        public override void _Ready()
        {
            Hide();
            _viewportRect = GetViewportRect();
        
            _playerInput = GetNode<PlayerInput>("Controllers/PlayerInput");
            _playerMovement = GetNode<PlayerMovement>("Controllers/PlayerMovement");
            _playerRotation = GetNode<PlayerRotation>("Controllers/PlayerRotation");
        
            _playerAttack = GetNode<PlayerAttack>("Controllers/PlayerAttack");
            _playerSpecialAttack = GetNode<PlayerSpecialAttack>("Controllers/PlayerSpecialAttack");
        
            _playerAnimation = GetNode<PlayerAnimation>("Controllers/PlayerAnimation");
            _collisionPolygon = GetNode<CollisionPolygon2D>("CollisionPolygon");
        
            Connect("body_entered", this, "OnPlayerBodyEntered");
        }
    
        public override void _PhysicsProcess(float delta)
        {
            // Movement and rotation
            Vector2 inputVelocity = _playerInput.OnMovementInput();
            Rotation += _playerRotation.CalculateRotation(delta, inputVelocity);
            Position += _playerMovement.GetLinearMovement(Rotation, delta, inputVelocity);
            Position = _playerMovement.ClampPosition(Position, _viewportRect.End);
        
            // Player shoot and special
            _playerAttack.Shoot(_playerInput.IsShooting, -Transform.x.Normalized());
            _playerSpecialAttack.ShootSpecial(_playerInput.IsShootingSpecial);
        }

        public override void _Process(float delta)
        {
            _playerAnimation.PlayBurstAnimation(_playerMovement.Velocity);
        }
    
        /*
        * Restart player position
        */
        public void RestartPosition(Vector2 newPosition)
        {
            Show();
            Position = newPosition;
            _collisionPolygon.Disabled = false;
        }
    
        /*
        * Called on player body entered signal
        */
        private void OnPlayerBodyEntered(object body)
        {
            Hide();
            EmitSignal("HitSignal");
            _collisionPolygon.SetDeferred("disabled", false);
        }    
    }
}
