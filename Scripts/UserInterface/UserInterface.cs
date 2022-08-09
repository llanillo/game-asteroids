using System;

namespace Asteroids.UserInterface
{
    public class UserInterface : CanvasLayer
    {

        [Signal] public delegate void StartGame();

        private VBoxContainer _creditsVBox;
        private Button _playButton;
        private Label _messageLabel;
        private Label _scoreLabel;
        private Timer _messageTimer;
    
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _creditsVBox = GetNode<VBoxContainer>("CreditsVBox") ?? throw new ArgumentNullException(nameof(_creditsVBox));
            _playButton = GetNode<Button>("PlayButton") ?? throw new ArgumentNullException(nameof(_playButton));
            _messageTimer = GetNode<Timer>("MessageTimer") ?? throw new ArgumentNullException(nameof(_messageTimer));
            _messageLabel = GetNode<Label>("Message") ?? throw new ArgumentNullException(nameof(_messageLabel));
            _scoreLabel = GetNode<Label>("ScoreLabel") ?? throw new ArgumentNullException(nameof(_scoreLabel));
        
        
            _scoreLabel.Hide();
            _messageTimer.Connect("timeout", this, "OnMessageTimerTimeout");
            _playButton.Connect("pressed", this, "OnPlayButtonPressed");
        }

        /*
     * Show the given message in the game GUI
     */
        public void ShowMessage(string text)
        {
            _messageLabel.Text = text;
            _messageLabel.Show();
            _messageTimer.Start();
        }

        /*
     * Must be called when player dies. Resets and show label's messages
     */
        public async void GameOver()
        {
            ShowMessage("Game Over");
            await ToSignal(_messageTimer, "timeout"); // Wait for message timer timeout signal before continue
            _creditsVBox.Show();
            _playButton.Show();
            _messageLabel.Text = "Asteroids";
            _messageLabel.Show();
            _scoreLabel.Hide();
        }

        /*
     * Updates label score message
     */
        public void UpdateScore(int points)
        {
            _scoreLabel.Text = points.ToString();
        }

        /*
     * Called on message timer timeout signal
     */
        private void OnMessageTimerTimeout()
        {
            _messageLabel.Hide();
        }

        /*
     * Called on play button pressed signal
     */
        private void OnPlayButtonPressed()
        {
            _creditsVBox.Hide();
            _playButton.Hide();
            _scoreLabel.Show();
            EmitSignal("StartGame");
        }
    }
}
