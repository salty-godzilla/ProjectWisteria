using System;
using System.Runtime.CompilerServices;
using ProjectWisteria.Coord;
using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class Chunk
    {
        public World World { get; }

        public int ChunkColumnX { get; }
        public int ChunkColumnZ { get; }
        public int ChunkY { get; }

        private readonly BlockType[] _blocks = new BlockType[ChunkSizeCubed];

        public int BlockCount { get; private set; }

        public bool IsRenderScheduled { get; set; }

        public Chunk(World world, int chunkColumnX, int chunkColumnZ, int chunkY)
        {
            World = world;

            ChunkColumnX = chunkColumnX;
            ChunkColumnZ = chunkColumnZ;
            ChunkY = chunkY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBlockArrayIndex(int x, int y, int z)
        {
            if (!IsValidBlockPos(x, y, z)) { throw new ArgumentOutOfRangeException(); }

            return (y * ChunkSize + z) * ChunkSize + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBlock(int x, int y, int z, BlockType blockType)
        {
            var blockArrayPos = GetBlockArrayIndex(x, y, z);
            var beforeBlockType = _blocks[blockArrayPos];

            if (blockType == beforeBlockType) { return; }

            _blocks[blockArrayPos] = blockType;

            if (beforeBlockType == BlockType.Air)
            {
                BlockCount++;
            }
            else if (blockType == BlockType.Air)
            {
                BlockCount--;
            }

            PendingRender();

            var chunkEdgeX = x switch
            {
                0 => -1,
                ChunkSize - 1 => 1,
                _ => 0
            };

            var chunkEdgeY = y switch
            {
                0 => -1,
                ChunkSize - 1 => 1,
                _ => 0
            };


            var chunkEdgeZ = z switch
            {
                0 => -1,
                ChunkSize - 1 => 1,
                _ => 0
            };

            var coord = new ChunkGlobalCoord(ChunkColumnX, ChunkY, ChunkColumnZ);

            var neighbor1 = new ChunkGlobalCoord(ChunkColumnX + chunkEdgeX, ChunkY, ChunkColumnZ);
            var neighbor2 = new ChunkGlobalCoord(ChunkColumnX, ChunkY + chunkEdgeY, ChunkColumnZ);
            var neighbor3 = new ChunkGlobalCoord(ChunkColumnX, ChunkY, ChunkColumnZ + chunkEdgeZ);
            var neighbor4 = new ChunkGlobalCoord(ChunkColumnX + chunkEdgeX, ChunkY + chunkEdgeY, ChunkColumnZ);
            var neighbor5 = new ChunkGlobalCoord(ChunkColumnX + chunkEdgeX, ChunkY, ChunkColumnZ + chunkEdgeZ);
            var neighbor6 = new ChunkGlobalCoord(ChunkColumnX, ChunkY + chunkEdgeY, ChunkColumnZ + chunkEdgeZ);
            var neighbor7 =
                new ChunkGlobalCoord(ChunkColumnX + chunkEdgeX, ChunkY + chunkEdgeY, ChunkColumnZ + chunkEdgeZ);

            if (!neighbor1.Equals(coord)) { World.GetChunk(neighbor1)?.PendingRender(); }

            if (!neighbor2.Equals(coord)) { World.GetChunk(neighbor2)?.PendingRender(); }

            if (!neighbor3.Equals(coord)) { World.GetChunk(neighbor3)?.PendingRender(); }

            if (!neighbor4.Equals(coord)) { World.GetChunk(neighbor4)?.PendingRender(); }

            if (!neighbor5.Equals(coord)) { World.GetChunk(neighbor5)?.PendingRender(); }

            if (!neighbor6.Equals(coord)) { World.GetChunk(neighbor6)?.PendingRender(); }

            if (!neighbor7.Equals(coord)) { World.GetChunk(neighbor7)?.PendingRender(); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockType GetBlock(int x, int y, int z)
        {
            return _blocks[GetBlockArrayIndex(x, y, z)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockType GetBlock(int blockArrayIndex)
        {
            return _blocks[blockArrayIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsOnlyAir()
        {
            return BlockCount == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidBlockPos(int x, int y, int z)
        {
            var invalid = x < 0 || x >= ChunkSize
                                || y < 0 || y >= ChunkSize
                                || z < 0 || z >= ChunkSize;

            return !invalid;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PendingRender()
        {
            if (IsRenderScheduled) { return; }

            IsRenderScheduled = true;

            World.ScheduleUpdateChunk(this);
        }
    }
}
