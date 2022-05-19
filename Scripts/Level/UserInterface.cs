using Godot;
using System;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;

public class UserInterface : CanvasLayer
{

    [Signal] public delegate void StartGame();

    private Button _playButton;
    private Label _messageLabel;
    private Label _scoreLabel;
    private Timer _messageTimer;
    
    public override void _Ready()
    { 
        _playButton = GetNode<Button>("Play_Button");
        _messageTimer = GetNode<Timer>("Message_Timer");
        _messageLabel = GetNode<Label>("Message");
        _scoreLabel = GetNode<Label>("Score_Label");
        
        _scoreLabel.Hide();
        _messageTimer.Connect("timeout", this, "OnMessageTimerTimeout");
        _playButton.Connect("pressed", this, "OnPlayButtonPressed");
    }

    public void ShowMessage(string text)
    {
        _messageLabel.Text = text;
        _messageLabel.Show();
        _messageTimer.Start();
    }

    public async void GameOver()
    {
        ShowMessage("Game Over");
        await ToSignal(_messageTimer, "timeout"); // Wait for message timer timeout signal
        _playButton.Show();
        _messageLabel.Text = "Asteroids";
        _messageLabel.Show();
        _scoreLabel.Hide();
    }

    public void UpdateScore(int points)
    {
        _scoreLabel.Text = points.ToString();
    }

    private void OnMessageTimerTimeout()
    {
        _messageLabel.Hide();
    }

    private void OnPlayButtonPressed()
    {
        _playButton.Hide();
        _scoreLabel.Show();
        EmitSignal("StartGame");
    }
}
