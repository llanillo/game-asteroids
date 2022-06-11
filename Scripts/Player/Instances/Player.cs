using System.Collections.Generic;
using Godot;

namespace Asteroids.Scripts.Player.Instances
{
	public class Player : Area2D
	{
		[Export] private PackedScene _bulletScene;
		[Export] private float _bulletSpeed = 500.0f;
		[Export] private float _rotationSpeed = 4.5f;
		[Export] private float _speed = 500;
		[Export] private int _numberSpecials = 3;
	
		[Signal] public delegate void HitSignal();
	
		private CollisionPolygon2D _collisionPolygon;
		private Vector2 _velocity = Vector2.Zero;
		private Rect2 _viewportRect;
	
		private AnimationPlayer _burstAnimationPlayer;
		private Line2D _burstLine;
	
		private AudioStreamPlayer _bulletAudioStream;
		private Position2D _bulletSpawnPosition;
		private Timer _bulletTimer;
		private Timer _specialTimer;
	
		private const int NumberSpecialBullets = 50;
	
		private const float Acceleration = 0.2f;
		private const float Friction = 0.02f;
	
		private const int MinVelocityBurstAnimation = 40;
	
		private int _rotationDirection;
		private bool _canShoot = true;
		private bool _canShootSpecial = true;
	
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Hide();
			_viewportRect = GetViewportRect();
			_burstAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer_Burst");
			_burstLine = GetNode<Line2D>("Node_Player/Line_Burst");
			_collisionPolygon = GetNode<CollisionPolygon2D>("CollisionPolygon");
		
			_bulletSpawnPosition = GetNode<Position2D>("Bullet_Position");
			_bulletAudioStream = GetNode<AudioStreamPlayer>("Shoot_AudioStream");

			_specialTimer = GetNode<Timer>("Special_Timer");
			_specialTimer.Connect("timeout", this, "OnSpecialTimerTimeout");
			_bulletTimer = GetNode<Timer>("Bullet_Timer");
			_bulletTimer.Connect("timeout", this, "OnBulletTimerTimeout");
		
			Connect("body_entered", this, "OnPlayerBodyEntered");
		}

		public override void _PhysicsProcess(float delta)
		{
			Vector2 inputVelocity = HandlePlayerMovementInput();
			HandlePlayerMovement(delta, inputVelocity);
			HandlePlayerShootingInput();
			HandlePlayerSpecialInput();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(float delta)
		{
			HandlePlayerAnimation();
		}

		/*
	 * Saves the player's input rotation direction.
	 * Return a vector2 with player's direction multiply by speed
	 */
		private Vector2 HandlePlayerMovementInput()
		{
			Vector2 inputVelocity = Vector2.Zero;
			_rotationDirection = 0;
	  
			if (Input.IsActionPressed("ui_right"))
			{
				_rotationDirection += 1;
			}
			if (Input.IsActionPressed(("ui_left")))
			{
				_rotationDirection -= 1;
			}
			if (Input.IsActionPressed(("ui_down")))
			{
				inputVelocity = new Vector2(_speed, 0).Rotated(Rotation);
			}
			if (Input.IsActionPressed(("ui_up")))
			{
				inputVelocity = new Vector2(-_speed, 0).Rotated(Rotation);
			}
			
			return inputVelocity.Normalized() * _speed;
		}

		/*
	 * Spawn a series of bullets that follows a spiral form
	 */
		private async void HandlePlayerSpecialInput()
		{
			if ((_bulletScene is null) || !Input.IsActionPressed("ui_secondary") 
			                           || _numberSpecials <= 0 || !_canShootSpecial) return;

			const float specialDegreesFactor = 4.2f;
			const float degreesOffset0 = 0;
			const float degreesOffset90 = 90;
			const float degreesOffset180 = 180;
			const float degreesOffset270 = 270;
		
			_numberSpecials--;
			_canShootSpecial = false;
		
			for (uint i = 0; i < NumberSpecialBullets; i++)
			{
				float[] directions = GetSpecialBulletsDirection((int) i, specialDegreesFactor, 
					new float[]{degreesOffset0, degreesOffset90, degreesOffset180, degreesOffset270});
			
				for(uint j = 0; j < directions.Length; j++)
				{
					var bulletDirection = new Vector2(Mathf.Cos(directions[j]), Mathf.Sin(directions[j]));
					SpawnBullet(_bulletSpawnPosition, bulletDirection, _bulletSpeed);
				}
			
				await ToSignal(GetTree(), "idle_frame");
			}
		
			_specialTimer.Start();
		}

		/*
	 * Returns an float array that contains degrees
	 * directions used to spawn special abilities bullets
	 */
		private static float[] GetSpecialBulletsDirection(int cycleIndex, float degreesFactor, IReadOnlyList<float> offsets)
		{
			var amount = offsets.Count;
			var directions = new float[amount];
            
			for (var i = 0; i < amount; i++)
			{
				directions[i] = Mathf.Deg2Rad(offsets[i] + cycleIndex * degreesFactor);
			}

			return directions;
		}
	
		/*
	 * Spawns a bullet and applies central impulse to it with player's current direction
	 */
		private void HandlePlayerShootingInput()
		{
			if (!Input.IsActionPressed("ui_select") || !_canShoot) return;
		
			SpawnBullet(_bulletSpawnPosition, - Transform.x.Normalized(), _bulletSpeed);
		
			_canShoot = false;
			_bulletTimer.Start();
			_bulletAudioStream.Play();
		}
	
		/*
	 * Moves and rotates player according to its current direction and input velocity.
	 * Also, limits player position to map boundaries with an offset
	 */
		private void HandlePlayerMovement(float delta, Vector2 inputVelocity)
		{
			Rotation += _rotationDirection * _rotationSpeed * delta;

			_velocity = inputVelocity.Length() > 0 ? _velocity.LinearInterpolate(inputVelocity, Acceleration) : _velocity.LinearInterpolate(Vector2.Zero, Friction);
			Position += _velocity * delta;

			float xLimit = _viewportRect.End.x;
			float yLimit = _viewportRect.End.y;
			float offset = 20.0f;
			Position = new Vector2(Mathf.Clamp(Position.x, offset, xLimit - offset), Mathf.Clamp(Position.y, offset, yLimit - offset));
		}
	
		/*
	 * Handle player's burst animation, this will play only when
	 * player's velocity is greater than the minimum velocity for animation
	 */
		private void HandlePlayerAnimation()
		{
			if (_velocity.Length() >= MinVelocityBurstAnimation)
			{
				_burstAnimationPlayer.Play("Burst Blink");
			}
			else
			{
				_burstLine.Visible = false;
				_burstAnimationPlayer.Stop();
			}
		}

		/*
	 * Spawns a bullet with direction, position and applies to it a central impulse
	 */
		private void SpawnBullet(Position2D spawnPosition, Vector2 direction, float bulletSpeed)
		{
			if(!(_bulletScene?.Instance() is Bullet bulletInstance)) return;
		
			GetTree().Root.AddChild(bulletInstance);
		
			bulletInstance.Position = spawnPosition.GlobalPosition;
			bulletInstance.ApplyCentralImpulse(direction * bulletSpeed);
		}
	
		/*
	 * Called on player body entered signal
	 */
		private void OnPlayerBodyEntered(object body)
		{
			Hide();
			EmitSignal("HitSignal");
			_collisionPolygon.SetDeferred("disabled", false);
		}

		/*
	 * Restart player position
	 */
		public void RestartPosition(Vector2 newPosition)
		{
			Show();
			Position = newPosition;
			_collisionPolygon.Disabled = false;
		}

		/*
	 * Called on special ability timer timeout signal
	 */
		private void OnSpecialTimerTimeout()
		{
			_canShootSpecial = true;
		}
	
		/*
	 * Called on bullet timer timeout signal
	 */
		private void OnBulletTimerTimeout()
		{
			_canShoot = true;
		}
	}
}