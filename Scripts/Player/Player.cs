using Godot;

public class Player : Area2D
{
	[Export] private PackedScene _bulletScene;
	[Export] private float _bulletSpeed = 400.0f;
	[Export] private float _rotationSpeed = 4.5f;
	[Export] private float _speed = 500;
	
	[Signal] public delegate void HitSignal();
	
	private CollisionPolygon2D _collisionPolygon;
	private Vector2 _velocity = Vector2.Zero;
	private Rect2 _mapLimit;
	
	private AnimationPlayer _burstAnimationPlayer;
	private Line2D _burstLine;
	
	private Position2D _bulletSpawn;
	private Timer _bulletTimer;
	
	private const float Acceleration = 0.2f;
	private const float Friction = 0.02f;
	private const int MinVelocityBurstAnimation = 40;
	
	private int _rotationDirection;

	private bool _canShoot = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		_mapLimit = GetViewportRect();
		_burstAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer_Burst");
		_burstLine = GetNode<Line2D>("Node_Player/Line_Burst");
		_collisionPolygon = GetNode<CollisionPolygon2D>("CollisionPolygon");
		
		_bulletSpawn = GetNode<Position2D>("Bullet_Position");
		_bulletTimer = GetNode<Timer>("Bullet_Timer");
		_bulletTimer.Connect("timeout", this, "OnBulletTimerTimeout");
		Connect("body_entered", this, "OnPlayerBodyEntered");
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 inputVelocity = HandlePlayerMovementInput();
		HandlePlayerMovement(delta, inputVelocity);
		HandlePlayerShootingInput();
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
	 * Spawns a bullet and applies central impulse to it with player's current direction
	 */
	private void HandlePlayerShootingInput()
	{
		if ((_bulletScene is null) || !Input.IsActionPressed("ui_select") || !_canShoot) return;
		
		_canShoot = false;
		
		Bullet bulletInstance = (Bullet) _bulletScene.Instance();
		GetTree().Root.AddChild(bulletInstance);
		
		bulletInstance.Position = _bulletSpawn.GlobalPosition;
		bulletInstance.ApplyCentralImpulse(- Transform.x.Normalized() * _bulletSpeed);
		_bulletTimer.Start();
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

		float xLimit = _mapLimit.End.x;
		float yLimit = _mapLimit.End.y;
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
	 * Called on player body entered signal
	 */
	private void OnPlayerBodyEntered(object body)
	{
	  Hide();
	  EmitSignal("HitSignal");
	  _collisionPolygon.SetDeferred("disabled", false);
	}

	public void RestartPosition(Vector2 newPosition)
	{
	  Show();
	  this.Position = newPosition;
	  _collisionPolygon.Disabled = false;
	}
	
	/*
	 * Called on bullet timer timeout signal
	 */
	private void OnBulletTimerTimeout()
	{
		_canShoot = true;
	}
}