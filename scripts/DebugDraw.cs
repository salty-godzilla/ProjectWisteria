using Godot;

namespace ProjectWisteria
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public class DebugDraw : Node
    {
        public override void _Ready()
        {
            VisualServer.SetDebugGenerateWireframes(true);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey {Pressed: true, Scancode: (uint) KeyList.F4})
            {
                var vp = GetViewport()!;
                vp.DebugDraw =
                    vp.DebugDraw == Viewport.DebugDrawEnum.Disabled
                        ? Viewport.DebugDrawEnum.Wireframe
                        : Viewport.DebugDrawEnum.Disabled;
            }
        }
    }
}
