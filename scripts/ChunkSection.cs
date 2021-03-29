using System.Runtime.CompilerServices;
using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class ChunkSection
    {
        private readonly BlockType[] _blocks = new BlockType[ChunkSectionSizeCubed];

        public ushort BlockCount { get; set; }

        public ChunkSection? XpNeighbor, XnNeighbor;
        public ChunkSection? YpNeighbor, YnNeighbor;
        public ChunkSection? ZpNeighbor, ZnNeighbor;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetBlockArrayIndex(byte x, byte y, byte z)
        {
            return (ushort) ((y * ChunkSectionSize + z) * ChunkSectionSize + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBlock(byte x, byte y, byte z, BlockType blockType)
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
        public BlockType GetBlock(byte x, byte y, byte z)
        {
            return _blocks[GetBlockArrayIndex(x, y, z)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockType GetBlock(ushort blockArrayIndex)
        {
            return _blocks[blockArrayIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockType GetBlock(int x, int y, int z)
        {
            return _blocks[GetBlockArrayIndex((byte) x, (byte) y, (byte) z)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsOnlyAirs()
        {
            return BlockCount == 0;
        }

        public static bool IsValidBlockPos(int x, int y, int z)
        {
            var invalid = x < 0 || x >= ChunkSectionSize
                                || y < 0 || y >= ChunkSectionSize
                                || z < 0 || z >= ChunkSectionSize;

            return !invalid;
        }
    }
}
