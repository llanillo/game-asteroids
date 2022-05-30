using Godot;
using System;

public class Bullet : RigidBody2D
{

    private VisibilityNotifier2D _visibilityNotifier;
    
    public override void _Ready()
    {
        _visibilityNotifier = GetNode<VisibilityNotifier2D>("VisibilityNotifier");
        _visibilityNotifier.Connect("screen_exited", this, "OnScreenExited");
        
        Connect("body_entered", this, "OnBulletBodyEntered");
    }

    private void OnBulletBodyEntered(Node body)
    {
        GD.Print("Se lalmo");
        if (body is Rock rock)
        {
            rock.DestroyRock();
        }
    }
    
    private void OnScreenExited()
    {
        QueueFree();
    }
}
