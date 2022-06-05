using Godot;

public class FadeInAudio : AudioEffect
{
    
    /*
     * Uses a tween node to interpolate the volume property to
     * the audio stream player to simulate the fade in effect
     */
    public void FadeIn(float maximumVolume, float transitionDuration, AudioStreamPlayer audioStreamPlayer)
    {
        InterpolateProperty(audioStreamPlayer, VolumeProperty, LowestVolume,
                    maximumVolume, transitionDuration, TransitionType.Linear, EaseType.In);
        Start();
        audioStreamPlayer.Play();
    }
}
