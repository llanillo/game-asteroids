namespace Asteroids
{
    public class SpectrumAnalyzer : Node2D
    {

        private struct Range
        {
            public int Minimum { get; }
            public int Maximum { get; }

            public Range(int minimum, int maximum)
            {
                Minimum = minimum;
                Maximum = maximum;
            }

            public int GetRangeDifference()
            {
                return Maximum - Minimum;
            }
        }

        private const int FrequencyAmount = 20;

        /*
         * Audible hertz range for humans
         */
        private const int MaximumFrequency = 20000;
        private const int MinimumFrequency = 20;

        /*
         * Common music decibel range values
         */
        private const int MaximumDecibel = -20;
        private const int MinimumDecibel = -50;

        private AudioEffectSpectrumAnalyzerInstance _audioSpectrum;
        private Range _frequencyRange;
        private Range _dbRange;

        public float[] FrequenciesLoudness { get; } = new float[FrequencyAmount]; // Loudness of each frequency

        public override void _Ready()
        {
            /*
             * Create the spectrum analyzer effect and add it to the master audio server
             */
            var spectrumAnalyzerEffect = new AudioEffectSpectrumAnalyzer();
            AudioServer.AddBusEffect(0, spectrumAnalyzerEffect);

            /*
             * Retrieve the recent created audio spectrum effect as
             * and instance to access all its properties
             */
            _audioSpectrum = (AudioEffectSpectrumAnalyzerInstance)AudioServer.GetBusEffectInstance(0, 0);

            _frequencyRange = new Range(MinimumFrequency, MaximumFrequency);
        }

        public override void _Process(float delta)
        {
            float frequency = _frequencyRange.Minimum;
            var interval = _frequencyRange.GetRangeDifference() / FrequencyAmount;

            for (uint i = 0; i < FrequencyAmount; i++)
            {
                float previousFrequencyScale = GetFrequencyScale(frequency, _frequencyRange);
                frequency += interval;
                float frequencyScale = GetFrequencyScale(frequency, _frequencyRange);

                float frequencyRangeLow = GetFrequencyRange(previousFrequencyScale, _frequencyRange);
                float frequencyRangeHigh = GetFrequencyRange(frequencyScale, _frequencyRange);

                float magnitude = GD.Linear2Db(_audioSpectrum.
                    GetMagnitudeForFrequencyRange(frequencyRangeLow, frequencyRangeHigh).Length());

                /*
                 * Subtract the current magnitude to the minimum decibel and
                 * divide by the difference to get the current volume level
                 */
                magnitude = (magnitude - _dbRange.Minimum) / _dbRange.GetRangeDifference(); // Slope multiplication
                magnitude += 0.3f * frequencyScale;
                magnitude = Mathf.Clamp(magnitude, 0.05f, 1);

                FrequenciesLoudness[i] = /*Mathf.Lerp(FrequenciesLoudness[i], magnitude, 20.0f * delta);*/ magnitude;
            }
        }

        /*
        * Must be called before using the analyzer. Creates the frequency
         * and decibel range the spectrum analyzer is going to use.
        */
        public void SetAudioStreamPlayer(AudioStreamPlayer audioStreamPlayer)
        {
            float currentMusicVolume = audioStreamPlayer.VolumeDb;
            _dbRange = new Range(MinimumDecibel + (int)currentMusicVolume, MaximumDecibel + (int)currentMusicVolume);
        }

        /*
         * Draw an audio visualizer of the current playing music.
         */
        private void DrawAudioVisualizer()
        {
            Vector2 drawPos = Vector2.Zero;
            float interval = 200f / FrequencyAmount;

            for (uint i = 0; i < FrequencyAmount; i++)
            {
                DrawLine(drawPos, drawPos + new Vector2(0, -FrequenciesLoudness[i] * 50), Colors.Chocolate, 4.0f, true);
                drawPos.x += interval;
            }
        }

        /*
         * Returns the interpolated value between the maximum and minimum
         * of the frequency range of four times the received frequency scale
         */
        private static float GetFrequencyRange(float frequencyScale, Range frequencyRange)
        {
            return Mathf.Lerp(frequencyRange.Minimum, frequencyRange.Maximum, Mathf.Pow(frequencyScale, 4));
        }

        /*
         * Return the the scale of given frequency
         */
        private static float GetFrequencyScale(float frequency, Range frequencyRange)
        {
            return (frequency - frequencyRange.Minimum) / frequencyRange.GetRangeDifference();
        }
    }
}
