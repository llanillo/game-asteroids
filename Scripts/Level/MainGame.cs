using Godot;

public class MainGame : Node
{
	[Export] private PackedScene _smallRockScene;
	[Export] private PackedScene _bigRockScene;
	[Export] private PackedScene _playerScene;

	private AudioManager _audioManager;
	private UserInterface _userInterface;
	private Position2D _startPosition;
	private Player _player;
	
	private Timer _startTimer;
	private Timer _scoreTimer;
	private Timer _rockTimer;

	private PathFollow2D _rockPathFollow;
	
	private int _score;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();
		
		_startPosition = GetNode<Position2D>("Start_Position");
		_startTimer = GetNode<Timer>("Timers/Start_Timer");
		_scoreTimer = GetNode<Timer>("Timers/Score_Timer");
		_rockTimer = GetNode<Timer>("Timers/Rock_Timer");
		_userInterface = GetNode<UserInterface>("Canvas");
		_audioManager = GetNode<AudioManager>("AudioManager");
		_rockPathFollow = GetNode<PathFollow2D>("Rock_Path/Rock_PathFollow");
		
		_startTimer.Connect("timeout", this, "OnStartTimerTimeout");
		_scoreTimer.Connect("timeout", this, "OnScoreTimerTimeout");
		_rockTimer.Connect("timeout", this, "OnRockTimerTimeout");
		_userInterface.Connect("StartGame", this, "RestartGame"); // Connects play button pressed to restart game
	}

	/*
	 * Called on start game signal 
	 */
	private void RestartGame()
	{
		SpawnPlayer(_startPosition);
		_audioManager.FadeInMusic();
		
		_score = 0;
		_startTimer.Start();
		_userInterface.ShowMessage("Get Ready");
		_userInterface.UpdateScore(_score);
	}

	/*
	 * Called on player's hit signal. This signal is emitted when any rock
	 * hit the player
	 */
	private void GameOver()
	{
		_audioManager.FadeOutMusic();
		_player.QueueFree();
		_scoreTimer.Stop();
		_rockTimer.Stop();
		_userInterface.GameOver();
	}

	/*
	 * Called on start timer timeout signal
	 */
	private void OnStartTimerTimeout()
	{
		_rockTimer.Start();
		_scoreTimer.Start();
	}

	/*
	 * Called on score timer timeout signal
	 */
	private void OnScoreTimerTimeout()
	{
		_score += 1;
		_userInterface.UpdateScore(_score);
	}

	/*
	 * Called on rock timer timeout signal
	 */
	private void OnRockTimerTimeout()
	{
		if (_bigRockScene is null || _smallRockScene is null) return;
		
		// The final rock rotation is the path rotation plus an offset angle
		double rockFinalRotation = _rockPathFollow.Rotation + Mathf.Pi / 2;
		rockFinalRotation += GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		_rockPathFollow.Offset = GD.Randi();

		Rock rockInstance;
		if (GD.Randi() % 3 == 0)
		{
			rockInstance = _bigRockScene.Instance() as BigRock;
		}
		else
		{
			rockInstance = _smallRockScene.Instance() as Rock;
		}

		if (rockInstance is null) return;
		
		AddChild(rockInstance);
		
		rockInstance.Position = _rockPathFollow.Position;
		rockInstance.Rotation = (float) rockFinalRotation;
		
		// Rotated the rock instance to the same direction we rotated it
		rockInstance.ApplyImpulse((float) rockFinalRotation);
	}

	/*
	 * Spawn player at given position
	 */
	private void SpawnPlayer(Position2D position2D)
	{
		_player = _playerScene.Instance() as Player;
		AddChild(_player);
		_player.RestartPosition(position2D.Position);
		_player.Connect("HitSignal", this, "GameOver"); // Connects player hitting rock signal to game over
	}
}
