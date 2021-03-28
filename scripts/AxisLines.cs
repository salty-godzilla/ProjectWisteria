using Godot;

namespace ProjectWisteria
{
    public class AxisLines : ImmediateGeometry
    {
        private readonly Color _xOriginColor = Color.Color8(255, 0, 0);
        private readonly Color _xPosColor = Color.Color8(255, 50, 50);

        private readonly Color _yOriginColor = Color.Color8(0, 30, 0);
        private readonly Color _yPosColor = Color.Color8(0, 150, 0);

        private readonly Color _zOriginColor = Color.Color8(0, 0, 255);
        private readonly Color _zPosColor = Color.Color8(50, 50, 255);

        private readonly Vector3 _drawOrigin = new(0, 0, -10f);

        public override void _Process(float delta)
        {
            var xPos = _drawOrigin + GlobalTransform.basis.XformInv(Vector3.Right);
            var yPos = _drawOrigin + GlobalTransform.basis.XformInv(Vector3.Up);
            var zPos = _drawOrigin + GlobalTransform.basis.XformInv(Vector3.Back);

            Clear();
            Begin(Mesh.PrimitiveType.Lines);

            // X axis
            SetColor(_xOriginColor);
            AddVertex(_drawOrigin);

            SetColor(_xPosColor);
            AddVertex(xPos);

            // Y axis
            SetColor(_yOriginColor);
            AddVertex(_drawOrigin);

            SetColor(_yPosColor);
            AddVertex(yPos);

            // Z axis
            SetColor(_zOriginColor);
            AddVertex(_drawOrigin);

            SetColor(_zPosColor);
            AddVertex(zPos);

            End();
        }
    }
}
