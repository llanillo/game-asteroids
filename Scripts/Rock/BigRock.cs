using Godot;

public class BigRock : Rock
{

    [Export] private PackedScene _smallRockScene;
    [Export] private int _amountSmallRocks = 3;
    
    /*
     * Create a certain amount of small rocks with random impulse when
     * player's bullet hit this rock
     */
    public override void DestroyRock()
    {
        for (uint i = 0; i < _amountSmallRocks; i++)
        {
            if(!(_smallRockScene?.Instance() is Rock smallRockInstance)) return;
            GetTree().Root.CallDeferred("add_child", smallRockInstance); // Delay the AddChild method until engine is ready to execute it
            
            double rockRotation = GD.RandRange(-Mathf.Pi, Mathf.Pi);
            
            smallRockInstance.GlobalPosition = Position;
            smallRockInstance.Rotation = (float) rockRotation;
            smallRockInstance.ApplyImpulse((float) rockRotation);
        }

        base.DestroyRock();
    }
}
