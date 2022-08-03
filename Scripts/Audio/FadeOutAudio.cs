using Godot;

namespace Asteroids
{
    public class FadeOutAudio : AudioEffect
    {

        private float _lastVolume;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Connect("tween_completed", this, "OnFadeOutTweenCompleted");
        }

        /*
        * Uses a tween node to interpolate the volume property to
        * the audio stream player to simulate the fade out effect
        */
        public void FadeOut(float transitionDuration, AudioStreamPlayer audioStreamPlayer)
        {
            _lastVolume = audioStreamPlayer.VolumeDb;
            InterpolateProperty(audioStreamPlayer, VolumeProperty, _lastVolume,
                        LowestVolume, transitionDuration, Tween.TransitionType.Linear, EaseType.In);
            Start();
        }

        /*
         * Called when fade out tween completed signal is emitted
         */
        private void OnFadeOutTweenCompleted(Object @object, NodePath key)
        {
            var audioStreamPlayer = (AudioStreamPlayer)@object;
            audioStreamPlayer.Stop();
            audioStreamPlayer.VolumeDb = _lastVolume;
        }
    }
}