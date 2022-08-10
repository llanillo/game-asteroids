using System;

namespace Asteroids
{
    public class Bullet : RigidBody2D
    {

        private VisibilityNotifier2D _visibilityNotifier;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _visibilityNotifier = GetNode<VisibilityNotifier2D>("VisibilityNotifier") ?? throw new ArgumentNullException(nameof(_visibilityNotifier));
            _visibilityNotifier.Connect(SignalUtil.ScreenExited, this, nameof(OnScreenExited));

            Connect(SignalUtil.BodyEntered, this, nameof(OnBulletBodyEntered));
        }

        /*
         * Called on body entered signal from the rigidbody node
         */
        private void OnBulletBodyEntered(Node body)
        {
            if (!(body is Asteroids.Rock.Interface.Rock rock)) return;

            rock.DestroyRock();
            QueueFree();
        }

        /*
         * Called on screen exited from visibility notifier node
         */
        private void OnScreenExited()
        {
            QueueFree();
        }
    }
}
