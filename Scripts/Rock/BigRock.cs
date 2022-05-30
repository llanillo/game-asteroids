using Godot;
using System;

public class BigRock : Rock
{

    [Export] private PackedScene _smallRockScene;
    [Export] private int _amountSmallRocks = 3;
    
    public override void DestroyRock()
    {
        for (uint i = 0; i < _amountSmallRocks; i++)
        {
            Rock smallRockInstance = _smallRockScene.Instance() as Rock;
            GetTree().Root.CallDeferred("add_child", smallRockInstance); // Delay the AddChild method until engine is ready to execute it
            
            double rockRotation = GD.RandRange(-Mathf.Pi, Mathf.Pi);
            
            smallRockInstance.Position = Position;
            smallRockInstance.Rotation = (float) rockRotation;
            smallRockInstance.ApplyImpulse((float) rockRotation);
        }
        
        base.DestroyRock();
    }
}
