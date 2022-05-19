using Godot;
using System;

public class MainGame : Node
{

    [Export] private PackedScene _rockScene;

    private Player _player;
    private Position2D _startPosition;
    
    private Timer _startTimer;
    private Timer _scoreTimer;
    private Timer _rockTimer;

    private PathFollow2D _rockPathFollow;
    
    private int _score = 0;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();

        _player = GetNode<Player>("Player");
        _startPosition = GetNode<Position2D>("Start_Position");
        _startTimer = GetNode<Timer>("Start_Timer");
        _scoreTimer = GetNode<Timer>("Score_Timer");
        _rockTimer = GetNode<Timer>("Rock_Timer");
        _rockPathFollow = GetNode<PathFollow2D>("Rock_Path/Rock_PathFollow");
        
        _startTimer.Connect("timeout", this, "OnStartTimerTimeout");
        _scoreTimer.Connect("timeout", this, "OnScoreTimerTimeout");
        _rockTimer.Connect("timeout", this, "OnRockTimerTimeout");
    }

    private void RestartGame()
    {
        _score = 0;
        _player.RestartPosition(_startPosition.Position);
        _startTimer.Start();
    }

    private void GameOver()
    {
        _scoreTimer.Stop();
        _rockTimer.Stop();
    }

    private void OnStartTimerTimeout()
    {
        _rockTimer.Start();
        _scoreTimer.Start();
    }

    private void OnScoreTimerTimeout()
    {
        _score += 1;
    }

    private void OnRockTimerTimeout()
    {
        // The final rock rotation is the path rotation plus an offset angle
        double rockFinalRotation = _rockPathFollow.Rotation + Mathf.Pi / 2;
        rockFinalRotation += GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        
        // Assign random  rock path follow
        _rockPathFollow.Offset = GD.Randi();

        // Null propagation syntax and pattern matching
        if (!(_rockScene?. Instance() is Rock rockInstance)) return;
        
        AddChild(rockInstance);

        float minSpeed = rockInstance.MinSpeed;
        float maxSpeed = rockInstance.MaxSpeed;
        
        rockInstance.Position = _rockPathFollow.Position;
        rockInstance.Rotation = (float) rockFinalRotation;
        
        // Rotated at the end makes the rock instance points the same direction we rotated it
        rockInstance.LinearVelocity = new Vector2((float) GD.RandRange(minSpeed, maxSpeed), 0).Rotated((float) rockFinalRotation);
    }
}
