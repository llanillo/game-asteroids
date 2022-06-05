using Godot;

public class FadeOutAudio : AudioEffect
{
    public override float TransitionDuration => 2.0f;

    private float _lastVolume;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("tween_completed", this, "OnFadeOutTweenCompleted");
    }
    
    public void FadeOut(AudioStreamPlayer audioStreamPlayer)
    {
        _lastVolume = audioStreamPlayer.VolumeDb;
        InterpolateProperty(audioStreamPlayer, "volume_db", _lastVolume,
                    LowestVolume, TransitionDuration, Tween.TransitionType.Linear, EaseType.In);
        Start();
    }

    private void OnFadeOutTweenCompleted(Object signalObject, NodePath key)
    {
        var audioStreamPlayer = (AudioStreamPlayer) signalObject;
        audioStreamPlayer.Stop();
        audioStreamPlayer.VolumeDb = _lastVolume;
    }
}
