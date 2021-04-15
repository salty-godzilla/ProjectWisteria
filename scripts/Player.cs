using Godot;

namespace ProjectWisteria
{
    public class Player : Spatial
    {
        private PlayerMovement _playerMovement;
        private World _world;

        private Camera _camera;

        public override void _Ready()
        {
            _world = GetNode<World>("/root/Root/World");

            _camera = GetNode<Camera>("./../../Camera");
            _camera.Current = true;
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseButton {Pressed: true, ButtonIndex: (int) ButtonList.Left})
            {
                GD.Print("pressed");

                var hit = WorldRayCast.GetRayHitCoord(_camera.GlobalTransform.origin, -_camera.GlobalTransform.basis.z,
                    _world, out var x, out var y, out var z, out _, out _, out _);
                GD.Print($"cam hit: {hit}");

                if (hit)
                {
                    GD.Print($"del {x} {y} {z}");
                    _world.SetBlock(x, y, z, BlockType.Air);
                }
            }
            else if (inputEvent is InputEventMouseButton {Pressed: true, ButtonIndex: (int) ButtonList.Right})
            {
                GD.Print("pressed");

                GD.Print($"cam pos {_camera.GlobalTransform.origin}");
                GD.Print($"cam dir {-_camera.GlobalTransform.basis.z}");

                var hit = WorldRayCast.GetRayHitCoord(
                    _camera.GlobalTransform.origin, -_camera.GlobalTransform.basis.z, _world,
                    out var x, out var y, out var z,
                    out var fx, out var fy, out var fz);
                GD.Print($"cam hit: {hit}");

                if (hit)
                {
                    GD.Print($"put {fx} {fy} {fz}");
                    _world.SetBlock(fx, fy, fz, BlockType.Grass);
                }
            }
        }
    }
}
