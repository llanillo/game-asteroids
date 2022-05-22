using Godot;
using System;

public class Rock : RigidBody2D
{
	[Export] public int MinSpeed { get; private set; } = 150;
	[Export] public int MaxSpeed { get; private set; } = 250;
	
	private VisibilityNotifier2D _visibilityNotifier;
	private AnimatedSprite _rockAnimatedSprite;

	private readonly string[] _rockTypes = { "Type 1", "Type 2", "Type 3" };
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rockAnimatedSprite = GetNode<AnimatedSprite>("AnimSprite");
		_visibilityNotifier = GetNode<VisibilityNotifier2D>("VisibilityNotifier");
		_visibilityNotifier.Connect("screen_exited", this, "OnScreenExited");
		
		// Set random rock sprite
		GD.Randomize();
		uint randomValue = GD.Randi() % 3;
		_rockAnimatedSprite.Animation = _rockTypes[randomValue];
	}

	public void AssignLinearVelocity(float rotation)
	{
		LinearVelocity = new Vector2((float) GD.RandRange(MinSpeed, MaxSpeed), 0).Rotated(rotation);
	}
	
	private void OnScreenExited()
	{
		QueueFree();
	}
}
