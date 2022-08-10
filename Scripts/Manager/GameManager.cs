global using Godot;
global using Asteroids.Player.Controllers.Manager;
global using Asteroids.Rock.Implementation;
global using System.Linq;
global using System.Collections.Generic;
global using Asteroids.Util;
using System;
using Array = Godot.Collections.Array;

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

			_startPosition = GetNode<Position2D>("Start_Position") ??
			                 throw new ArgumentNullException(nameof(_startPosition));
			_startTimer = GetNode<Timer>("Timers/Start_Timer") ?? throw new ArgumentNullException(nameof(_startTimer));
			_scoreTimer = GetNode<Timer>("Timers/Score_Timer") ?? throw new ArgumentNullException(nameof(_scoreTimer));
			_rockTimer = GetNode<Timer>("Timers/Rock_Timer") ?? throw new ArgumentNullException(nameof(_rockTimer));
			_userInterface = GetNode<UserInterface.UserInterface>("Canvas") ??
			                 throw new ArgumentNullException(nameof(_userInterface));
			_audioManager = GetNode<AudioManager>("AudioManager") ??
			                throw new ArgumentNullException(nameof(_audioManager));
			_rockPathFollow = GetNode<PathFollow2D>("Rock_Path/Rock_PathFollow") ??
			                  throw new ArgumentNullException(nameof(_rockPathFollow));

			_startTimer.Connect(SignalUtil.Timeout, this, nameof(OnStartTimerTimeout));
			_scoreTimer.Connect(SignalUtil.Timeout, this, nameof(UpdateScore), new Array{ScorePerSecond});
			_rockTimer.Connect(SignalUtil.Timeout, this, nameof(OnRockTimerTimeout));
			_userInterface.Connect(nameof(UserInterface.UserInterface.StartGame), this,
				nameof(RestartGame));
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
			_player.Connect(nameof(PlayerManager.HitSignal), this, nameof(GameOver));
		}
	}
}