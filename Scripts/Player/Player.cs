using Godot;
using System;

public class Player : Area2D
{
	[Export] private float _speed = 500;
	[Signal] public delegate void HitSignal();
	
	private Vector2 _velocity = new Vector2();
	private CollisionShape2D _collisionShape;
	private Rect2 _mapLimit;
	private AnimatedSprite _playerAnimatedSprite;
	private Sprite _burstSprite;
	private AnimationPlayer _burstAnimationPlayer;

	[Export] private float _rotationSpeed = 4.5f;
	private int _rotationDirection = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mapLimit = GetViewportRect();
		_playerAnimatedSprite = GetNode<AnimatedSprite>("Player_AnimSprite");
		_burstAnimationPlayer = GetNode<AnimationPlayer>("Burst_AnimPlayer");
		_burstSprite = GetNode<Sprite>("Burst_Sprite");
		_collisionShape = GetNode<CollisionShape2D>("Collision");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		HandlePlayerInput();
		HandlePlayerMovement(delta);
	}

	public override void _Process(float delta)
	{
	  
	  HandlePlayerAnimation();
	}

	private void HandlePlayerInput()
	{
		_rotationDirection = 0;
		_velocity = new Vector2();
	  
		if (Input.IsActionPressed("ui_right"))
		{
			_rotationDirection += 1;
		}
		else if (Input.IsActionPressed(("ui_left")))
		{
			_rotationDirection -= 1;
		}
		else if (Input.IsActionPressed(("ui_down")))
		{
			_velocity = new Vector2(_speed, 0).Rotated(Rotation);
		}
		else if (Input.IsActionPressed(("ui_up")))
		{
			_velocity = new Vector2(-_speed, 0).Rotated(Rotation);
		}

		_velocity = _velocity.Normalized() * _speed;
	}

	private void HandlePlayerMovement(float delta)
	{
		Rotation += _rotationDirection * _rotationSpeed * delta;
		
		float xLimit = _mapLimit.End.x;
		float yLimit = _mapLimit.End.y;

		Position += _velocity * delta;
		Position = new Vector2(Mathf.Clamp(Position.x, 0, xLimit), Mathf.Clamp(Position.y, 0, yLimit));
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
		  _collisionShape.Disabled = true;
	  }

	  private void RestartPosition(Vector2 newPosition)
	  {
		  Show();
		  this.Position = newPosition;
		  _collisionShape.Disabled = false;
	  }
	}


