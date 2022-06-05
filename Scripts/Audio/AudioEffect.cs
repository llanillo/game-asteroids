using Godot;
using System;

public class AudioEffect : Tween
{
    private const int _lowestVolume = -80;

    protected static int LowestVolume => _lowestVolume;

    public virtual float TransitionDuration { get; }
}
