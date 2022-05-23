using Godot;
using System;

public class MainGame : Node
{

	[Export] private PackedScene _smallRockScene;
	[Export] private PackedScene _bigRockScene;

	private UserInterface _userInterface;
	private Position2D _startPosition;
	private Player _player;
	
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
		_userInterface = GetNode<UserInterface>("Canvas");
		_rockPathFollow = GetNode<PathFollow2D>("Rock_Path/Rock_PathFollow");
		
		_startTimer.Connect("timeout", this, "OnStartTimerTimeout");
		_scoreTimer.Connect("timeout", this, "OnScoreTimerTimeout");
		_rockTimer.Connect("timeout", this, "OnRockTimerTimeout");
		_userInterface.Connect("StartGame", this, "RestartGame"); // Connects play button pressed to restart game
		_player.Connect("HitSignal", this, "GameOver"); // Connects player hitting rock signal to game over
	}

	private void RestartGame()
	{
		_score = 0;
		_player.RestartPosition(_startPosition.Position);
		_startTimer.Start();
		_userInterface.ShowMessage("Get Ready");
		_userInterface.UpdateScore(_score);
	}

	private void GameOver()
	{
		_scoreTimer.Stop();
		_rockTimer.Stop();
		_userInterface.GameOver();
	}

	private void OnStartTimerTimeout()
	{
		_rockTimer.Start();
		_scoreTimer.Start();
	}

	private void OnScoreTimerTimeout()
	{
		_score += 1;
		_userInterface.UpdateScore(_score);
	}

	private void OnRockTimerTimeout()
	{
		// The final rock rotation is the path rotation plus an offset angle
		double rockFinalRotation = _rockPathFollow.Rotation + Mathf.Pi / 2;
		rockFinalRotation += GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		
		// Assign random  rock path follow
		_rockPathFollow.Offset = GD.Randi();

		// Null propagation syntax and pattern matching
		if (_bigRockScene is null || _smallRockScene is null) return;
		if (!(_smallRockScene?. Instance() is Rock rockInstance) && !(_bigRockScene?.Instance() is BigRock bigRockInstance)) return;

		BigRock bigRockInstance = _bigRockScene.Instance() as BigRock;
		Rock smallRockInstance = _smallRockScene.Instance() as Rock;
		AddChild(rockInstance);
		
		float minSpeed = rockInstance.MinSpeed;
		float maxSpeed = rockInstance.MaxSpeed;
		
		rockInstance.Position = _rockPathFollow.Position;
		rockInstance.Rotation = (float) rockFinalRotation;
		
		// Rotated at the end makes the rock instance points the same direction we rotated it
		rockInstance.LinearVelocity = new Vector2((float) GD.RandRange(minSpeed, maxSpeed), 0).Rotated((float) rockFinalRotation);
		rockInstance.AssignLinearVelocity((float) rockFinalRotation);
	}
}
