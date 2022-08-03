namespace Asteroids.Player.Controllers.Attack
{
    public class PlayerSpecialAttack : Attack
    {
        [Export] private int _numberSpecials = 3;
        
        private Timer _specialTimer;
        
        private const int AmountSpecialBullets = 50;
        private bool _canShootSpecial = true;

        public override void _Ready()
        {
            base._Ready();
            _specialTimer = GetNode<Timer>("../../Timers/SpecialTimer");
            _specialTimer.Connect("timeout", this, "OnSpecialTimerTimeout");
        }

        /*
        * Spawn a series of bullets that follows a spiral form
        */
        public async void ShootSpecial(bool isShootingSpecial)
        {
            if (!isShootingSpecial || _numberSpecials <= 0 || !_canShootSpecial) return;

            const float specialDegreesFactor = 4.2f;
            const float degreesOffset0 = 0;
            const float degreesOffset90 = 90;
            const float degreesOffset180 = 180;
            const float degreesOffset270 = 270;
		
            _numberSpecials--;
            _canShootSpecial = false;
		
            for (var i = 0; i < AmountSpecialBullets; i++)
            {
                var directions = GetSpecialBulletsDirection(i, specialDegreesFactor,
                    new[] { degreesOffset0, degreesOffset90, degreesOffset180, degreesOffset270 });
			
                for(uint j = 0; j < directions.Length; j++)
                {
                    var bulletDirection = new Vector2(Mathf.Cos(directions[j]), Mathf.Sin(directions[j]));
                    SpawnBullet(BulletSpawnPosition, bulletDirection, BulletSpeed);
                }
			
                await ToSignal(GetTree(), "idle_frame");
            }
		
            _specialTimer.Start();
        }
    
        /*
         * Returns an float array that contains degrees
         * directions used to spawn special abilities bullets
         */
        private static float[] GetSpecialBulletsDirection(int cycleIndex, float degreesFactor, IReadOnlyList<float> offsets)
        {
            var amount = offsets.Count;
            var directions = new float[amount];
            
            for (var i = 0; i < amount; i++)
            {
                directions[i] = Mathf.Deg2Rad(offsets[i] + cycleIndex * degreesFactor);
            }

            return directions;
        }  
        
        /*
        * Called on special ability timer timeout signal
        */
        private void OnSpecialTimerTimeout()
        {
            _canShootSpecial = true;
        }
    }
}