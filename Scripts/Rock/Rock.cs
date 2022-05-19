using Godot;
using System;

public class Rock : RigidBody2D
{

	[Export] public int MinSpeed { get; private set; } = 150;
	[Export] public int MaxSpeed { get; private set; } = 250;

	private const float BigRockScale = 1.6f;
	private readonly string[] _rockType = { "Big", "Small" };
	
	private AnimatedSprite _rockSprite;
	private CollisionShape2D _collisionShape;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();
		_collisionShape = GetNode<CollisionShape2D>("Collision");
		_rockSprite = GetNode<AnimatedSprite>("Sprite");
		_rockSprite.Animation = _rockType[GD.Randi() % _rockType.Length];

		if (_rockSprite.Animation == "Big")
		{
			_collisionShape.Scale = new Vector2(BigRockScale, BigRockScale);
		}
	}

	private void _on_Visibility_screen_exited()
	{
		QueueFree();
	}
}
