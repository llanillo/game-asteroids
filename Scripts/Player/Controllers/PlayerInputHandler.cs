using Godot;

public class PlayerInputHandler : Node
{
    
    private Vector2 _movementInput;
    public Vector2 MovementInput { get; }
    
    public bool IsShooting { get; private set; }
    public bool IsShootingSpecial { get; private set; }

    public override void _Input(InputEvent @event)
    {
        OnFireInput(@event);
        OnSpecialInput(@event);
        OnMovementInput(@event);
    }

    private void OnFireInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_select")) return;
        
        IsShooting = true;
        ConsumeRecentInputEvent();
    }

    private void OnSpecialInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_secondary")) return;
        
        IsShootingSpecial = true;
        ConsumeRecentInputEvent();
    }

    private void OnMovementInput(InputEvent @event)
    {
        _movementInput = Vector2.Zero;

        if (@event.IsActionPressed("ui_right"))
        {
            _movementInput.x += 1;
        }
        if (@event.IsActionPressed(("ui_left")))
        {
            _movementInput.x -= 1;
        }
        if (@event.IsActionPressed(("ui_down")))
        {
            _movementInput.y += 1;
        }
        if (@event.IsActionPressed(("ui_up")))
        {
            _movementInput.y -= 1;
        }

        _movementInput = _movementInput.Normalized();
    }

    private void ConsumeRecentInputEvent()
    {
        GetTree().SetInputAsHandled();
    }
}
