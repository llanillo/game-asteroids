using Godot;
using System;

public class Player : Area2D
{
	[Export] private float _speed = 500;
	[Signal] public delegate void HitSignal();
	
	private Vector2 _velocity;
	private CollisionShape2D _collisionShape;
	private Rect2 _mapLimit;
	private AnimatedSprite _playerSprite;
	private Sprite _fireSprite;
	private AnimationPlayer _fireAnimation;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mapLimit = GetViewportRect();
		_playerSprite = GetNode<AnimatedSprite>("Player_Sprite");
		_fireAnimation = GetNode<AnimationPlayer>("Fire_Anim");
		_fireSprite = GetNode<Sprite>("Fire_Sprite");
		_collisionShape = GetNode<CollisionShape2D>("Collision");
	}

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
	  HandlePlayerInput(delta);
	  HandlePlayerRotation();
	  HandlePlayerAnimation();
  }

private void HandlePlayerInput(float Delta)
{
	_velocity = new Vector2();
  
	if (Input.IsActionPressed("ui_right"))
	{
		_velocity.x += 1;
	}
	else if (Input.IsActionPressed(("ui_left")))
	{
		_velocity.x -= 1;
	}
	else if (Input.IsActionPressed(("ui_down")))
	{
		_velocity.y += 1;
	}
	else if (Input.IsActionPressed(("ui_up")))
	{
		_velocity.y -= 1;
	}

	if (_velocity.Length() > 0)
	{
		_velocity = _velocity.Normalized() * _speed;
	}

	Position += _velocity * Delta;

	float xLimit = _mapLimit.End.x;
	float yLimit = _mapLimit.End.y;
  
	Position = new Vector2(Mathf.Clamp(Position.x, 0, xLimit), Mathf.Clamp(Position.y, 0, yLimit));
}

private void HandlePlayerRotation()
{
	if (_velocity.x > 0)
	{
		RotationDegrees = 90;
	}
	else if (_velocity.x < 0)
	{
		RotationDegrees = -90;
	}
	else if (_velocity.y > 0)
	{
		RotationDegrees = -180;
	}
	else if (_velocity.y < 0)
	{
		RotationDegrees = 0;
	}
}
private void HandlePlayerAnimation()
{
	if (_velocity != Vector2.Zero)
	{
		_playerSprite.Animation = "Moving";
		_fireAnimation.Play("Fire Blink");
	}
	else
	{
		_playerSprite.Animation = "Still";
		_fireSprite.Visible = false;
		_fireAnimation.Stop();
	}
}

  private void OnPlayerBodyEntered(object body)
  {
	  Hide();
	  EmitSignal("HitSignal");
	  _collisionShape.Disabled = true;
  }

  private void RestartPosition(Vector2 Position)
  {
	  Show();
	  this.Position = Position;
	  _collisionShape.Disabled = false;
  }
}


