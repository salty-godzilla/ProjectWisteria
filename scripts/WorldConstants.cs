namespace ProjectWisteria
{
    public static class WorldConstants
    {
        public const uint ChunkSectionSize = 16;
        public const uint ChunkSectionSizeSquared = ChunkSectionSize * ChunkSectionSize;
        public const uint ChunkSectionSizeCubed = ChunkSectionSize * ChunkSectionSize * ChunkSectionSize;

        public const uint ChunkHeightPositive = 128; // 0 to 127
        public const uint ChunkHeightNegative = 64; // -1 to -64
        public const uint ChunkHeight = ChunkHeightPositive + ChunkHeightNegative;

        public const uint ChunkPositiveSections = ChunkHeightPositive / ChunkSectionSize;
        public const uint ChunkNegativeSections = ChunkHeightNegative / ChunkSectionSize;
        public const uint ChunkSections = ChunkPositiveSections + ChunkNegativeSections;

        public const uint WorldSizeX = 16;
        public const uint WorldSizeZ = 16;
    }
}
