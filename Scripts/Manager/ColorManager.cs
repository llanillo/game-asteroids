using System;
using System.Linq;
using Godot;

namespace Asteroids.Manager
{
    public class ColorManager : Node2D
    {
        private const int RedFrequency = 0;
        private const int BlueFrequency = 2;
        private const int GreenFrequency = 4;
    
        private const float ModulateAcceleration = 0.3f;
        private static readonly Random Random = new Random();
    
        private CanvasModulate _canvasModulate;
        private SpectrumAnalyzer _spectrumAnalyzer;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GD.Randomize();
            _canvasModulate = GetNode<CanvasModulate>("CanvasModulate");
            _spectrumAnalyzer = GetNode<SpectrumAnalyzer>("/root/World/Spectrum_Analyzer");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            float[] frequenciesLoudness = _spectrumAnalyzer.FrequenciesLoudness;
            var red = frequenciesLoudness[RedFrequency];
            var blue = frequenciesLoudness[BlueFrequency];
            var green = frequenciesLoudness[GreenFrequency];
      
            var colors = new[] {red, green, blue};
            var randomColors = colors.OrderBy(x => Random.Next()).ToArray();
      
            var modulateColor = new Color(randomColors[0], randomColors[1], randomColors[2]);
            _canvasModulate.Color = _canvasModulate.Color.LinearInterpolate(modulateColor, ModulateAcceleration);
        }
    }
}
