using Godot;
using System;

public class BigRock : Rock
{

    [Export] private PackedScene _rockScene;
    [Export] public int SmallRockMinSpeed { get; private set; }
    [Export] public int SmallRockMaxSpeed { get; private set; }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("body_entered", this, "OnBigRockBodyEntered");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {
    //     
    // }
    
    private void OnBigRockBodyEntered(Node body)
    {
        if (!(body is Bullet)) return;
        
        double rockRotation = GD.RandRange(-Mathf.Pi, Mathf.Pi);
        Rock rockInstance = _rockScene.Instance() as Rock;
        
        rockInstance.Position = Position;
        rockInstance.Rotation = (float) rockRotation;
        rockInstance.AssignLinearVelocity((float) rockRotation);
    }

}
