using System.Net;
using Godot;

namespace Asteroids.UserInterface
{
    public class WaveLabel : CanvasLayer
    {
        private Timer _visibleTimer;
        private Label _waveLabel;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _visibleTimer = GetNode<Timer>("VisibleTimer");
            _waveLabel = GetNode<Label>("Label");
            
            _waveLabel.Hide();
            _visibleTimer.Connect("timeout", this, "OnVisibleTimerTimeout");
        }

        /*
         * Set the current wave label and show it
         */
        public void SetWaveLabel(string text)
        {
            _waveLabel.Show();
            _waveLabel.Text = text;
            _visibleTimer.Start();
        }

        /*
        * Called on visible timer timeout signal
        */
        private void OnVisibleTimerTimeout()
        {
            _waveLabel.Hide();
        }
    }
}
