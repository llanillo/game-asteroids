using Godot;

public class Rock : RigidBody2D
{
	[Export] public int MinSpeed { get; private set; } = 150;
	[Export] public int MaxSpeed { get; private set; } = 250;

	[Export] private PackedScene _explosionScene;
	
	private VisibilityNotifier2D _visibilityNotifier;
	private AnimatedSprite _rockAnimatedSprite;

	private readonly string[] _rockTypes = { "Type 1", "Type 2", "Type 3" };
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rockAnimatedSprite = GetNode<AnimatedSprite>("AnimSprite");
		_visibilityNotifier = GetNode<VisibilityNotifier2D>("VisibilityNotifier");
		_visibilityNotifier.Connect("screen_exited", this, "OnScreenExited");
		SetRandomSprite(_rockTypes);
	}

	/*
	 * Set a random sprite to the animated sprite node
	 */
	private void SetRandomSprite(string[] rockTypes)
	{
		GD.Randomize();
		uint randomValue = GD.Randi() % 3;
		_rockAnimatedSprite.Animation = rockTypes[randomValue];
	}
	
	/*
	 * Apply a central impulse to the rock's rigidbody with the given rotation
	 */
	public void ApplyImpulse(float rotation)
	{
		ApplyCentralImpulse(new Vector2((float) GD.RandRange(MinSpeed, MaxSpeed), 0).Rotated(rotation));
	}
	
	/*
	* Called on screen exited from visibility notifier node
	*/
	private void OnScreenExited()
	{
		QueueFree();
	}

	protected void CreateExplosion(Vector2 position)
	{
		if (!(_explosionScene?.Instance() is Explosion explosionInstance)) return;
		
		GetTree().Root.AddChild(explosionInstance);
		explosionInstance.EmitsExplosion(position);
	}
	
	/*
	* Called when player's bullet hit this rock
	*/
	public virtual void DestroyRock()
	{
		CreateExplosion(GlobalPosition);
		QueueFree();
	}
}
