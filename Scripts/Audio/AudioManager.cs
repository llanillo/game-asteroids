using Godot;
using System;
using Object = Godot.Object;

public class AudioManager : Node2D
{

    private const int MusicVolume = -15;
    private const float FadeOutTransitionDuration = 3.0f;
    private const float FadeInTransitionDuration = 2.0f;
    
    private AudioStreamPlayer _audioStreamPlayer;
    private SpectrumAnalyzer _spectrumAnalyzer;

    private FadeOutAudio _fadeOutAudio;
    private FadeInAudio _fadeInAudio;
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("Music_AudioStream");
        _spectrumAnalyzer = GetNode<SpectrumAnalyzer>("/root/World/Spectrum_Analyzer");
        _fadeOutAudio = GetNode<FadeOutAudio>("FadeOut_Tween");
        _fadeInAudio = GetNode<FadeInAudio>("FadeIn_Tween");
        
        _spectrumAnalyzer.SetAudioStreamPlayer(_audioStreamPlayer);
    }
    
    /*
     * Play the audio stream player instantly
     */
    public void PlayAudioStreamPlayer()
    {
        _audioStreamPlayer.VolumeDb = MusicVolume;
        _audioStreamPlayer.Play();
    }

    /*
     * Stops the audio stream player instantly
     */
    public void StopAudioStreamPlayer()
    {
        _audioStreamPlayer.Stop();
    }
    
    /*
     * Plays a fade out effect to the audio stream player
     * with the given transition duration
     */
    public void FadeOutAudio()
    {
        _fadeOutAudio.FadeOut(FadeOutTransitionDuration, _audioStreamPlayer);
    }

    /*
     * Plays a fade in effect to the audio stream player
     * with the given transition duration
     */
    public void FadeInAudio()
    {
        _fadeInAudio.FadeIn(MusicVolume, FadeInTransitionDuration, _audioStreamPlayer);
    }
}
