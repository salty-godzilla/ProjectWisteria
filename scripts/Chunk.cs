using System;
using System.Runtime.CompilerServices;
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

        public static bool IsValidBlockPos(int x, int y, int z)
        {
            var invalid = x < 0 || x >= ChunkSize
                                || y < 0 || y >= ChunkSize
                                || z < 0 || z >= ChunkSize;

            return !invalid;
        }
    }
}
