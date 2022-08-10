using System;
using Asteroids.Player.Controllers.Animation;
using Asteroids.Player.Controllers.Attack;
using Asteroids.Player.Controllers.Input;
using Asteroids.Player.Controllers.Movement;

namespace Asteroids.Player.Controllers.Manager
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

            _playerInput = GetNode<PlayerInput>("Controllers/PlayerInput") ??
                           throw new ArgumentNullException(nameof(_playerInput));
            _playerMovement = GetNode<PlayerMovement>("Controllers/PlayerMovement") ?? throw new ArgumentNullException(nameof(_playerMovement));
            _playerRotation = GetNode<PlayerRotation>("Controllers/PlayerRotation") ?? throw new ArgumentNullException(nameof(_playerRotation));
            _playerAttack = GetNode<PlayerAttack>("Controllers/PlayerAttack") ??
                            throw new ArgumentNullException(nameof(_playerAttack));
            _playerSpecialAttack = GetNode<PlayerSpecialAttack>("Controllers/PlayerSpecialAttack") ?? throw new ArgumentNullException(nameof(_playerSpecialAttack));
            _playerAnimation = GetNode<PlayerAnimation>("Controllers/PlayerAnimation") ?? throw new ArgumentNullException(nameof(_playerAnimation));
            _collisionPolygon = GetNode<CollisionPolygon2D>("CollisionPolygon") ?? throw new ArgumentNullException(nameof(_collisionPolygon));
        
            Connect(SignalUtil.BodyEntered, this, nameof(OnPlayerBodyEntered));
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
