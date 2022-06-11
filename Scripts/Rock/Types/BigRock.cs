using Asteroids.Scripts.Rock.Interface;
using Godot;

namespace Asteroids.Scripts.Rock.Types
{
    public class BigRock : Interface.Rock
    {
        [Export] public override int MinSpeed { get; set; } = 150;
        [Export] public override int MaxSpeed { get; set; } = 250;
        
        [Export] private PackedScene _smallRockScene;
        [Export] private int _amountMediumRocks = 3;

        /*
        * Create a certain amount of medium rocks with random impulse when
        * player's bullet hit this rock
        */
        public override void DestroyRock()
        {
            for (uint i = 0; i < _amountMediumRocks; i++)
            {
                if(!(_smallRockScene?.Instance() is global::Asteroids.Scripts.Rock.Types.MediumRock smallRockInstance)) return;
                GetTree().Root.CallDeferred("add_child", smallRockInstance); // Delay the AddChild method until engine is ready to execute it
            
                double rockRotation = GD.RandRange(-Mathf.Pi, Mathf.Pi);
            
                smallRockInstance.GlobalPosition = Position;
                smallRockInstance.Rotation = (float) rockRotation;
                smallRockInstance.ApplyImpulse((float) rockRotation);
            }

            base.DestroyRock();
        }
    }
}
