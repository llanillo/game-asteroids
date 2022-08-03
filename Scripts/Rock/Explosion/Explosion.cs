using Godot;
using System;

namespace Asteroids
{
    public class Explosion : Node2D
    {

        private const float TimerOffset = 0.5f;

        private CPUParticles2D _explosionParticles;
        private AudioStreamPlayer _explosionStreamPlayer;
        private Timer _queueFreeTimer;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _explosionParticles = GetNode<CPUParticles2D>("CPUParticles2D");
            _explosionStreamPlayer = GetNode<AudioStreamPlayer>("Explosion_AudioStream");
            _queueFreeTimer = GetNode<Timer>("QueueFree_Timer");

            _explosionParticles.Emitting = true;
            _queueFreeTimer.WaitTime = _explosionParticles.Lifetime + TimerOffset;
            _queueFreeTimer.Connect("timeout", this, "OnQueueFreeTimerTimeout");
        }

        /*
         * Emits the explosion audio and particle effect
         */
        public void EmitsExplosion(Vector2 position)
        {
            GlobalPosition = position;
            _explosionStreamPlayer.Play();
            _queueFreeTimer.Start();
        }

        /*
         * Called on queue free timer timeout signal
         */
        private void OnQueueFreeTimerTimeout()
        {
            QueueFree();
        }
    }
}
