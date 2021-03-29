namespace ProjectWisteria
{
    public static class WorldConstants
    {
        public const int ChunkSize = 16;
        public const int ChunkSizeSquared = 256;
        public const int ChunkSizeCubed = 4096;

        public const int ChunkColumnHeightPositive = 128; // 0 to 127
        public const int ChunkColumnHeightNegative = 64; // -1 to -64
        public const int ChunkColumnHeight = ChunkColumnHeightPositive + ChunkColumnHeightNegative;

        public const int ChunksPositiveInColumn = ChunkColumnHeightPositive / ChunkSize;
        public const int ChunksNegativeInColumn = ChunkColumnHeightNegative / ChunkSize;
        public const int ChunksInColumn = ChunksPositiveInColumn + ChunksNegativeInColumn;

        public const int WorldSizeX = 16;
        public const int WorldSizeZ = 16;
    }
}
