global using Godot;
global using Asteroids.Player.Controllers.Manager;
global using Asteroids.Rock.Implementation;
global using System.Linq;
global using System.Collections.Generic;

using Godot.Collections;

namespace Asteroids.Manager
{
	public class GameManager : Node
	{
		public static GameStatus GameStatus = GameStatus.Stop;

		private const float RockRotation = Mathf.Pi / 4;
		private const int ScorePerSecond = 1;

		[Export] protected PackedScene _mediumRockScene;
		[Export] protected PackedScene _bigRockScene;
		[Export] protected PackedScene _playerScene;

		private AudioManager _audioManager;
		private UserInterface.UserInterface _userInterface;
		private Position2D _startPosition;
		private PlayerManager _player;
	
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
			_userInterface = GetNode<UserInterface.UserInterface>("Canvas");
			_audioManager = GetNode<AudioManager>("AudioManager");
			_rockPathFollow = GetNode<PathFollow2D>("Rock_Path/Rock_PathFollow");
		
			_startTimer.Connect("timeout", this, "OnStartTimerTimeout");
			_scoreTimer.Connect("timeout", this, "UpdateScore", new Array{ScorePerSecond});
			_rockTimer.Connect("timeout", this, "OnRockTimerTimeout");
			_userInterface.Connect("StartGame", this, "RestartGame"); // Connects play button pressed to restart game
		}

		/*
	 * Called on start game signal 
	 */
		private void RestartGame()
		{
			GameStatus = GameStatus.Active;
		
			SpawnPlayer(_startPosition);
			_audioManager.FadeInAudio();
		
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
			GameStatus = GameStatus.Stop;
		
			_audioManager.FadeOutAudio();
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
		 * Add score to the total score of the game manager and user interface
		 */
		public void UpdateScore(int score)
		{
			_score += score;
			_userInterface.UpdateScore(_score);
		}
		/*
		* Called on rock timer timeout signal
		*/
		private void OnRockTimerTimeout()
		{
			if (_bigRockScene is null || _mediumRockScene is null) return;
		
			double rockFinalRotation = _rockPathFollow.Rotation + Mathf.Pi / 2;
			rockFinalRotation += GD.RandRange(-RockRotation, RockRotation);
			_rockPathFollow.Offset = GD.Randi();

			Rock.Interface.Rock rockInstance;
			if (GD.Randi() % 3 == 0)
			{
				rockInstance = _bigRockScene.Instance() as BigRock;
			}
			else
			{
				rockInstance = _mediumRockScene.Instance() as MediumRock;
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
			if (_playerScene is null) return;

			_player = _playerScene.Instance() as PlayerManager;
			AddChild(_player);
			_player.RestartPosition(position2D.Position);
			_player.Connect("HitSignal", this, "GameOver"); // Connects player hitting rock signal to game over
		}
	}
}