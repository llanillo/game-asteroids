using System;
using Asteroids.Manager;

namespace Asteroids.Rock.Interface
{
    public abstract class Rock : RigidBody2D
    {
        [Export] protected PackedScene _explosionScene;

        private VisibilityNotifier2D _visibilityNotifier;
        private GameManager _gameManager;
        private Rect2 _viewportRect;
        
        private const float ScreenWrapOffset = 50.0f;
        
        public abstract int MinSpeed { get; set; }
        public abstract int MaxSpeed { get; set; }
        public abstract int DestroyedScore { get; set; }
        
        public override void _Ready()
        {
            _viewportRect = GetViewportRect();

            _gameManager = GetNode<GameManager>("/root/World");
            _visibilityNotifier = GetNode<VisibilityNotifier2D>("VisibilityNotifier");
            _visibilityNotifier.Connect("screen_exited", this, "OnScreenExited");
        }
        
        /*
        * Called when player's bullet hit this rock
        */
        public virtual void DestroyRock()
        {
            if (!(_explosionScene?.Instance() is Explosion explosionInstance)) return;
		
            _gameManager.UpdateScore(DestroyedScore);
            GetTree().Root.AddChild(explosionInstance);
            explosionInstance.EmitsExplosion(GlobalPosition);
            QueueFree();
        }

        /*
        * Apply a central impulse to the rock's rigidbody with the given rotation
        */
        public void ApplyImpulse(float rotation)
        {
            ApplyCentralImpulse(new Vector2((float) GD.RandRange(MinSpeed, MaxSpeed), 0).Rotated(rotation));
        }
        
        /*
        * Called on screen exited from visibility notifier node
        */
        private void OnScreenExited()
        {
            switch (GameManager.GameStatus)
            {
                case GameStatus.Active:
                    ScreenWrap();
                    break;
                case GameStatus.Stop:
                    QueueFree();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /*
         * Wrap this object position to the screen bounds.
         * If the object goes offscreen it will come from the opposite side
         */
        private void ScreenWrap()
        {
            float wrappedX = Mathf.Wrap(Position.x, -ScreenWrapOffset, _viewportRect.Size.x + ScreenWrapOffset);
            float wrappedY = Mathf.Wrap(Position.y, -ScreenWrapOffset, _viewportRect.Size.y + ScreenWrapOffset);
            Position = new Vector2(wrappedX, wrappedY);
        }

    }
}