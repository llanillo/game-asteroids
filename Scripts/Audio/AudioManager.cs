using Godot;
using System;
using Object = Godot.Object;

public class AudioManager : Node2D
{

    private AudioStreamPlayer _audioStreamPlayer;
    private SpectrumAnalyzer _spectrumAnalyzer;

    private AudioEffect _audioEffect;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("Music_AudioStream");
        _spectrumAnalyzer = GetNode<SpectrumAnalyzer>("/root/World/Spectrum_Analyzer");
        _audioEffect = GetNode<AudioEffect>("Audio_Tween");
        
        _spectrumAnalyzer.SetAudioStreamPlayer(_audioStreamPlayer);
    }
    
    public void PlayMusic()
    {
        _audioStreamPlayer.Play();
    }

    public void FadeOutMusic()
    {
        _audioEffect.FadeOutAudio(_audioStreamPlayer);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
