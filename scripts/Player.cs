using Godot;

namespace ProjectWisteria
{
    public class Player : Spatial
    {
        private PlayerMovement _playerMovement;
        private World _world;

        public override void _Ready()
        {
            _world = GetNode<World>("/root/Root/World");
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseButton {Pressed: true, ButtonIndex: (int) ButtonList.Left})
            {
                GD.Print("pressed");
                var x = Mathf.FloorToInt(GlobalTransform.origin.x);
                var y = Mathf.FloorToInt(GlobalTransform.origin.y) - 1;
                var z = Mathf.FloorToInt(GlobalTransform.origin.z);
                GD.Print($"{x} {y} {z}");

                _world.SetBlock(x, y, z, BlockType.Air);
            }
            else if (inputEvent is InputEventMouseButton {Pressed: true, ButtonIndex: (int) ButtonList.Right})
            {
                GD.Print("pressed");
                var x = Mathf.FloorToInt(GlobalTransform.origin.x);
                var y = Mathf.FloorToInt(GlobalTransform.origin.y);
                var z = Mathf.FloorToInt(GlobalTransform.origin.z);
                GD.Print($"{x} {y} {z}");

                _world.SetBlock(x, y, z, BlockType.Grass);
            }
        }
    }
}
