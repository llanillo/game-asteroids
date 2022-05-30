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

    /*
     * Called on body entered signal from the rigidbody node
     */
    private void OnBulletBodyEntered(Node body)
    {
        if (!(body is Rock rock)) return;
        
        rock.DestroyRock();
        QueueFree();
    }
    
    /*
     * Called on screen exited from visibility notifier node
     */
    private void OnScreenExited()
    {
        QueueFree();
    }
}
