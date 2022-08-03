using Godot;

namespace Asteroids.Scripts
{
    public class FadeInAudio : AudioEffect
    {

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Connect("tween_started", this, "OnFadeInTweenCompleted");
        }

        /*
         * Uses a tween node to interpolate the volume property to
         * the audio stream player to simulate the fade in effect
         */
        public void FadeIn(float maximumVolume, float transitionDuration, AudioStreamPlayer audioStreamPlayer)
        {
            InterpolateProperty(audioStreamPlayer, VolumeProperty, LowestVolume,
                        maximumVolume, transitionDuration, TransitionType.Linear, EaseType.In);
            Start();
        }

        private void OnFadeInTweenCompleted(Object @object, NodePath key)
        {
            var audioStreamPlayer = (AudioStreamPlayer)@object;
            audioStreamPlayer.Play();
        }
    }
}
