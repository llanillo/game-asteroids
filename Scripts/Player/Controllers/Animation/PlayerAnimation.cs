using Godot;

namespace Asteroids.Player.Controllers.Animation
{
    public class PlayerAnimation : Node
    {
        private const int MinVelocityBurstAnimation = 40;
        
        private AnimationPlayer _burstAnimationPlayer;
        private Line2D _burstLine;

        public override void _Ready()
        {
            _burstAnimationPlayer = GetNode<AnimationPlayer>("../../AnimationPlayerBurst");
            _burstLine = GetNode<Line2D>("../../PlayerShape/LineBurst");
        }
        
        /*
        * Handle player's burst animation, this will play only when
        * player's velocity is greater than the minimum velocity for animation
        */
        public void PlayBurstAnimation(Vector2 currentVelocity)
        {
            if (currentVelocity.Length() >= MinVelocityBurstAnimation)
            {
                _burstAnimationPlayer.Play("Burst Blink");
            }
            else
            {
                _burstLine.Visible = false;
                _burstAnimationPlayer.Stop();
            }
        }
    }
}