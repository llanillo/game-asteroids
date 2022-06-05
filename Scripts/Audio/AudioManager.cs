using Godot;
using System;
using Object = Godot.Object;

public class AudioManager : Node2D
{

    private const int MusicVolume = -15;
    
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
    
    public void PlayMusic()
    {
        _audioStreamPlayer.Play();
    }

    public void FadeOutMusic()
    {
        _fadeOutAudio.FadeOut(_audioStreamPlayer);
    }

    public void FadeInMusic()
    {
        _fadeInAudio.FadeIn(MusicVolume, _audioStreamPlayer);
    }
}
