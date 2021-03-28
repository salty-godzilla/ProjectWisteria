using Godot;

namespace ProjectWisteria
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public class PlayerMovement : Spatial
    {
        private const float FloorMaxAngleDeg = 50;

        private const float JumpForce = 0.2f;
        private const float GravityForce = 0.5f;
        private const float MoveSpeed = 10f;
        private const float Acceleration = 10f;

        private readonly float _mouseSensitivty = 0.2f;

        private Camera _camera;

        private Vector2 _inputVector = Vector2.Zero;
        private bool _isMouseCaptured;

        public Vector3 CharacterVelocity;

        private EntityCollision _entityCollision;

        public override void _Ready()
        {
            _camera = GetNode<Camera>("./Camera");
            _camera.Current = true;

            _entityCollision = GetNode<EntityCollision>("./CollisionDetecter");

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
            targetHorizontalCharacterVelocity *= MoveSpeed * delta;

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

            var y = _entityCollision.Move(CharacterVelocity.x, CharacterVelocity.y, CharacterVelocity.z);

            if (Transform.origin.y.Equals(y))
            {
                CharacterVelocity.y = 0;
            }

            var pos = Transform;

            pos.origin.x += CharacterVelocity.x;
            pos.origin.y = y;
            pos.origin.z += CharacterVelocity.z;

            Transform = pos;
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
