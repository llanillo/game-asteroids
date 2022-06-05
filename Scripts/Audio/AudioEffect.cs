using Godot;

public class AudioEffect : Tween
{
    private const float TransitionDuration = 2.0f;
    private const int LowestVolume = -80;

    private float _lastVolume;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("tween_completed", this, "OnMusicTweenCompleted");
    }
    
    public void FadeOutAudio(AudioStreamPlayer audioStreamPlayer)
    {
        _lastVolume = audioStreamPlayer.VolumeDb;
        InterpolateProperty(audioStreamPlayer, "volume_db", _lastVolume,
                    LowestVolume, TransitionDuration, Tween.TransitionType.Linear, EaseType.In);
        Start();
    }

    private void OnMusicTweenCompleted(Object signalObject, NodePath key)
    {
        var audioStreamPlayer = (AudioStreamPlayer) signalObject;
        audioStreamPlayer.Stop();
        audioStreamPlayer.VolumeDb = _lastVolume;
    }
}
