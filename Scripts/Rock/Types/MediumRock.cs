using System;
using System.Collections.Generic;
using Asteroids.Scripts.Rock.Interface;
using Godot;

namespace Asteroids.Scripts.Rock.Types
{
	public class MediumRock : Interface.Rock
	{
		[Export] public override int MinSpeed { get; set; } = 200;
		[Export] public override int MaxSpeed { get; set; } = 300;
		
		private AnimatedSprite _rockAnimatedSprite;
		
		private readonly string[] _rockTypes = { "Type 1", "Type 2", "Type 3" };
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			base._Ready();
			GD.Randomize();
			_rockAnimatedSprite = GetNode<AnimatedSprite>("AnimSprite");
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
