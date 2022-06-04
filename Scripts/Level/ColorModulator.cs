using Godot;
using System;
using System.Linq;

public class ColorModulator : Node2D
{
    private const int RedFrequency = 0;
    private const int BlueFrequency = 2;
    private const int GreenFrequency = 4;
    
    private const float ModulateAcceleration = 0.2f;
    private static readonly Random _random = new Random();
    
    private CanvasModulate _canvasModulate;

    private float[] _frequenciesLoudness;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();
        _canvasModulate = GetNode<CanvasModulate>("CanvasModulate");
        _frequenciesLoudness = GetNode<SpectrumAnalyzer>("Spectrum_Analyzer").FrequenciesLoudness;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
      float red = _frequenciesLoudness[RedFrequency];
      float blue = _frequenciesLoudness[BlueFrequency];
      float green = _frequenciesLoudness[GreenFrequency];
      
      var colors = new[] {red, green, blue};
      var randomColors = colors.OrderBy(x => _random.Next()).ToArray();
      
      Color modulateColor = new Color(randomColors[0], randomColors[1], randomColors[2]);
      _canvasModulate.Color = _canvasModulate.Color.LinearInterpolate(modulateColor, ModulateAcceleration);
    }
    
}
