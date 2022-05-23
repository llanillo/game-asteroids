using Godot;
using System;

public class BigRock : Rock
{

    [Export] private PackedScene _smallRockScene;
    [Export] private int _amountSmallRocks = 3;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("body_entered", this, "OnBigRockBodyEntered");
    }
    
    private void OnBigRockBodyEntered(Node body)
    {
        if (!(body is Bullet)) return;
        
        for (uint i = 0; i < _amountSmallRocks; i++)
        {
            Rock smallRockInstance = _smallRockScene.Instance() as Rock;
            double rockRotation = GD.RandRange(-Mathf.Pi, Mathf.Pi);
            
            smallRockInstance.Position = Position;
            smallRockInstance.Rotation = (float) rockRotation;
            smallRockInstance.AssignLinearVelocity((float) rockRotation);    
        }
        
    }

}
