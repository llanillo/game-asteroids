using Godot;

namespace Asteroids.Scripts.Player.Controllers.Attack
{
    public abstract class Attack : Node
    {
        [Export] private float _bulletSpeed = 500.0f;
        [Export] private PackedScene _bulletScene;

        protected float BulletSpeed => _bulletSpeed;
        
        protected Position2D BulletSpawnPosition { get; private set; }


        public override void _Ready()
        {
            BulletSpawnPosition = GetNode<Position2D>("../../BulletPosition");
        }

        /*
        * Spawns a bullet with direction, position and applies to it a central impulse
        */
        protected void SpawnBullet(Position2D spawnPosition, Vector2 direction, float bulletSpeed)
        {
            if(!(_bulletScene?.Instance() is Bullet bulletInstance)) return;
		
            GetTree().Root.AddChild(bulletInstance);
		
            bulletInstance.Position = spawnPosition.GlobalPosition;
            bulletInstance.ApplyCentralImpulse(direction * bulletSpeed);
        }    
    }
}