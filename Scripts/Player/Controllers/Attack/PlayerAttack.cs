using System;

namespace Asteroids.Player.Controllers.Attack
{
    public class PlayerAttack : Attack
    {
        private AudioStreamPlayer _bulletAudioStream;
        private Timer _bulletTimer;
        
        private bool _canShoot = true;
    
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _bulletTimer = GetNode<Timer>("../../Timers/BulletTimer") ?? throw new ArgumentNullException(nameof(_bulletTimer));
            _bulletAudioStream = GetNode<AudioStreamPlayer>("../../ShootAudioStream") ?? throw new ArgumentNullException(nameof(_bulletAudioStream));

            _bulletTimer.Connect(SignalUtil.Timeout, this, nameof(OnBulletTimerTimeout));
        }

        /*
        * Spawns a bullet and applies central impulse to it with player's current direction
        */
        public void Shoot(bool isShooting, Vector2 direction)
        {
            if (!isShooting || !_canShoot) return;
		
            SpawnBullet(BulletSpawnPosition, direction, BulletSpeed);
		
            _canShoot = false;
            _bulletTimer.Start();
            _bulletAudioStream.Play();
        }

        /*
        * Called on bullet timer timeout signal
        */
        private void OnBulletTimerTimeout()
        {
            _canShoot = true;
        }    
    }
}
