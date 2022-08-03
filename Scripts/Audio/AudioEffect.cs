using Godot;
using System;

namespace Asteroids.Scripts
{
    public class AudioEffect : Tween
    {
        protected static int LowestVolume { get; } = -80;

        protected static string VolumeProperty { get; } = "volume_db";
    }
}
