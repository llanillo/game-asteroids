using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export] private float _rotationSpeed = 4.5f;
	[Export] private float _speed = 500;
	[Signal] public delegate void HitSignal();
	
	private CollisionShape2D _collisionShape;
	private Vector2 _velocity = Vector2.Zero;
	private Area2D _area2D;
	private Rect2 _mapLimit;
	
	private AnimationPlayer _burstAnimationPlayer;
	private AnimatedSprite _playerAnimatedSprite;
	private Sprite _burstSprite;

	private int _rotationDirection = 0;

	private const float Acceleration = 0.2f;
	private const float Friction = 0.02f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Hide();
		_mapLimit = GetViewportRect();
		_playerAnimatedSprite = GetNode<AnimatedSprite>("Player_AnimSprite");
		_burstAnimationPlayer = GetNode<AnimationPlayer>("Burst_AnimPlayer");
		_burstSprite = GetNode<Sprite>("Burst_Sprite");
		_collisionShape = GetNode<CollisionShape2D>("Area2D/Collision");
		_area2D = GetNode<Area2D>("Area2D");

		_area2D.Connect("body_entered", this, "OnPlayerBodyEntered");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		Vector2 inputVelocity = HandlePlayerInput();
		HandlePlayerMovement(delta, inputVelocity);
	}

	public override void _Process(float delta)
	{
		HandlePlayerAnimation();
	}

	private Vector2 HandlePlayerInput()
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
			inputVelocity = new Vector2(0, _speed).Rotated(Rotation);
		}
		if (Input.IsActionPressed(("ui_up")))
		{
			inputVelocity = new Vector2(0, -_speed).Rotated(Rotation);
		}

		return inputVelocity.Normalized() * _speed;
	}

	private void HandlePlayerMovement(float delta, Vector2 inputVelocity)
	{
		Rotation += _rotationDirection * _rotationSpeed * delta;

		_velocity = inputVelocity.Length() > 0 ? _velocity.LinearInterpolate(inputVelocity, Acceleration) : _velocity.LinearInterpolate(Vector2.Zero, Friction);
		_velocity = MoveAndSlide(_velocity);
		
		// Limits player position to map boundaries with an offset
		float xLimit = _mapLimit.End.x;
		float yLimit = _mapLimit.End.y;
		float offset = 20.0f;
		Position = new Vector2(Mathf.Clamp(Position.x, offset, xLimit - offset), Mathf.Clamp(Position.y, offset, yLimit - offset));
	}
	
	private void HandlePlayerAnimation()
	{
		if (_velocity != Vector2.Zero)
		{
			_playerAnimatedSprite.Animation = "Moving";
			_burstAnimationPlayer.Play("Fire Blink");
		}
		else
		{
			_playerAnimatedSprite.Animation = "Still";
			_burstSprite.Visible = false;
			_burstAnimationPlayer.Stop();
		}
	}

	private void OnPlayerBodyEntered(object body)
	{
	  Hide();
	  EmitSignal("HitSignal");
	  _collisionShape.SetDeferred("Disabled", false);
	}

	public void RestartPosition(Vector2 newPosition)
	{
	  Show();
	  this.Position = newPosition;
	  _collisionShape.Disabled = false;
	}
}


