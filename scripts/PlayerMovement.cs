using Godot;

namespace ProjectWisteria
{
    public class PlayerMovement : KinematicBody
    {
        private const float FloorMaxAngleDeg = 50;

        private const float JumpForce = 6f;
        private const float GravityForce = 9.8f;
        private const float MoveForce = 10f;
        private const float Acceleration = 10f;

        private readonly float _mouseSensitivty = 0.2f;

        private Camera _camera;

        private Vector2 _inputVector = Vector2.Zero;
        private bool _isMouseCaptured;

        public Vector3 CharacterVelocity;

        public override void _Ready()
        {
            _camera = GetNode<Camera>("./Camera");
            _camera.Current = true;

            Input.SetMouseMode(Input.MouseMode.Captured);
            _isMouseCaptured = true;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                Input.SetMouseMode(_isMouseCaptured ? Input.MouseMode.Visible : Input.MouseMode.Captured);
                _isMouseCaptured ^= true;
            }

            if (true)
            {
                _inputVector = Vector2.Zero;
                if (Input.IsActionPressed("move_forward"))
                {
                    _inputVector.y += 1;
                }

                if (Input.IsActionPressed("move_backward"))
                {
                    _inputVector.y -= 1;
                }

                if (Input.IsActionPressed("move_left"))
                {
                    _inputVector.x -= 1;
                }

                if (Input.IsActionPressed("move_right"))
                {
                    _inputVector.x += 1;
                }
            }

            var jump = Input.IsActionJustPressed("move_jump");

            var targetHorizontalCharacterVelocity =
                -GlobalTransform.basis.z * _inputVector.y + GlobalTransform.basis.x * _inputVector.x;
            targetHorizontalCharacterVelocity.Normalized();
            targetHorizontalCharacterVelocity *= MoveForce;

            if (jump)
            {
                CharacterVelocity.y = JumpForce;
            }

            CharacterVelocity.y -= GravityForce * delta;

            var horizontalCharacterVelocity = CharacterVelocity;
            horizontalCharacterVelocity.y = 0;

            horizontalCharacterVelocity =
                horizontalCharacterVelocity.LinearInterpolate(targetHorizontalCharacterVelocity, Acceleration * delta);

            CharacterVelocity.x = horizontalCharacterVelocity.x;
            CharacterVelocity.z = horizontalCharacterVelocity.z;

            CharacterVelocity = MoveAndSlide(CharacterVelocity, Vector3.Up, true, 4, Mathf.Deg2Rad(FloorMaxAngleDeg),
                false);

            UpdateTransform(Translation, Rotation);
        }

        private void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            Translation = position;
            Rotation = rotation;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion motion && Input.GetMouseMode() == Input.MouseMode.Captured)
            {
                // horizontal character rotation 
                RotateY(Mathf.Deg2Rad(-motion.Relative.x) * _mouseSensitivty);

                // vertical camera rotation
                _camera.RotateX(Mathf.Deg2Rad(-motion.Relative.y) * _mouseSensitivty);
                _camera.Rotation = new Vector3(Mathf.Deg2Rad(Mathf.Clamp(_camera.RotationDegrees.x, -95f, 95f)), 0, 0);
            }
        }
    }
}
