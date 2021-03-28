using System;
using Godot;

namespace ProjectWisteria
{
    public class EntityCollision : Spatial
    {
        private AABB _aabb;

        private readonly float _width = 1f;
        private readonly float _height = 2f;
        private readonly float _depth = 1f;

        private World _world;

        public bool Hit;

        private float offsetX;
        private float offsetZ;

        public override void _Ready()
        {
            _world = GetNode("/root/Root/World") as World;

            offsetX = _width / 2f;
            offsetZ = _depth / 2f;

            var startPosition = GlobalTransform.origin;
            startPosition.x -= offsetX;
            startPosition.z -= offsetZ;

            _aabb.Position = startPosition;
            _aabb.Size = new Vector3(_width, _height, _depth);
        }

        public float Move(float dx, float dy, float dz)
        {
            var startPosition = GlobalTransform.origin;
            startPosition.x -= offsetX;
            startPosition.z -= offsetZ;

            _aabb.Position = startPosition + new Vector3(dx, dy, dz);

            _aabb.Size = new Vector3(_width, _height, _depth);

            var worldAabbs = _world.GetAabb(_aabb);

            var yMax = _aabb.Position.y;

            foreach (var worldAabb in worldAabbs)
            {
                if (_aabb.Position.y < worldAabb.End.y)
                {
                    // hit Y
                    yMax = Math.Max(yMax, worldAabb.End.y);
                }
            }

            //GD.Print(yMax);

            return yMax;
        }
    }
}
