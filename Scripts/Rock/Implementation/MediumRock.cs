using System;

namespace Asteroids.Rock.Implementation
{
	public class MediumRock : Interface.Rock
	{
		[Export] public override int MinSpeed { get; set; } = 300;
		[Export] public override int MaxSpeed { get; set; } = 370;
		[Export] public override int DestroyedScore { get; set; } = 5;
		
		private AnimatedSprite _rockAnimatedSprite;
		
		private readonly string[] _rockTypes = { "Type 1", "Type 2", "Type 3" };
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			base._Ready();
			GD.Randomize();
			_rockAnimatedSprite = GetNode<AnimatedSprite>("AnimSprite") ?? throw new ArgumentNullException(nameof(_rockAnimatedSprite));
			SetRandomSprite(_rockTypes);
		}
		
		/*
		 * Set a random sprite to the animated sprite node
		 */
		private void SetRandomSprite(string[] rockTypes)
		{
			uint randomValue =  GD.Randi() % 3;
			_rockAnimatedSprite.Animation = rockTypes[randomValue];
		}
		
	}
}
