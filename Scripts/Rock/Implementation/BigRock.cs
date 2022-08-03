namespace Asteroids.Rock.Implementation
{
    public class BigRock : Interface.Rock
    {
        [Export] public override int MinSpeed { get; set; } = 250;
        [Export] public override int MaxSpeed { get; set; } = 320;
        [Export] public override int DestroyedScore { get; set; } = 10;
        
        [Export] private PackedScene _mediumRockScene;
        [Export] private int _amountMediumRocks = 3;

        /*
        * Create a certain amount of medium rocks with random impulse when
        * player's bullet hit this rock
        */
        public override void DestroyRock()
        {
            for (uint i = 0; i < _amountMediumRocks; i++)
            {
                if(!(_mediumRockScene?.Instance() is global::Asteroids.Rock.Implementation.MediumRock smallRockInstance)) return;
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
