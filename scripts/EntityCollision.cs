using System;
using Godot;

namespace ProjectWisteria
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public class EntityCollision : Spatial
    {
        private AABB _aabb;

        private readonly float _width = 1f;
        private readonly float _height = 2f;
        private readonly float _depth = 1f;

        private World _world = null!;

        public bool Hit;

        private float _offsetX;
        private float _offsetZ;

        public override void _Ready()
        {
            _world = GetNode<World>("/root/Root/World");

            _offsetX = _width / 2f;
            _offsetZ = _depth / 2f;

            var startPosition = GlobalTransform.origin;
            startPosition.x -= _offsetX;
            startPosition.z -= _offsetZ;

            _aabb.Position = startPosition;
            _aabb.Size = new Vector3(_width, _height, _depth);
        }

        public float Move(float dx, float dy, float dz)
        {
            var startPosition = GlobalTransform.origin;
            startPosition.x -= _offsetX;
            startPosition.z -= _offsetZ;

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
