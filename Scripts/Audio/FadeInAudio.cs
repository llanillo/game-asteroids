using Godot;

public class FadeInAudio : AudioEffect
{
    public override float TransitionDuration => 1.0f;

    public void FadeIn(float maximumVolume, AudioStreamPlayer audioStreamPlayer)
    {
        audioStreamPlayer.Play();
        
        InterpolateProperty(audioStreamPlayer, "volume_db", LowestVolume,
            maximumVolume, TransitionDuration, TransitionType.Linear, EaseType.In);
        Start();
    }
}
