using Godot;

namespace ProjectWisteria
{
    public static class WorldRayCast
    {
        public static bool GetRayHitCoord(Vector3 origin, Vector3 dir, World world,
            out int x, out int y, out int z,
            out int frontX, out int frontY, out int frontZ)
        {
            var lastCheckedBlockX = 0;
            var lastCheckedBlockY = 0;
            var lastCheckedBlockZ = 0;

            x = 0;
            y = 0;
            z = 0;
            frontX = 0;
            frontY = 0;
            frontZ = 0;

            var currentPoint = origin;

            const float step = 0.05f;
            const int stepIteration = 200;

            var stepDir = dir * step;

            for (var i = 0; i < stepIteration; i++)
            {
                // Get block position
                var blockX = Mathf.FloorToInt(currentPoint.x);
                var blockY = Mathf.FloorToInt(currentPoint.y);
                var blockZ = Mathf.FloorToInt(currentPoint.z);

                if (i == 0 ||
                    !(lastCheckedBlockX == blockX && lastCheckedBlockY == blockY && lastCheckedBlockZ == blockZ))
                {
                    // Moved to different block
                    var block = world.GetBlock(blockX, blockY, blockZ);

                    if (block != BlockType.Air)
                    {
                        x = blockX;
                        y = blockY;
                        z = blockZ;

                        frontX = lastCheckedBlockX;
                        frontY = lastCheckedBlockY;
                        frontZ = lastCheckedBlockZ;

                        return true;
                    }
                }

                lastCheckedBlockX = blockX;
                lastCheckedBlockY = blockY;
                lastCheckedBlockZ = blockZ;

                currentPoint += stepDir;
            }

            return false;
        }
    }
}
