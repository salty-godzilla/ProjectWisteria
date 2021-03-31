using System;
using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class ChunkColumn
    {
        private readonly World _world;

        public int ChunkColumnX { get; }
        public int ChunkColumnZ { get; }

        private readonly Chunk[] _chunks = new Chunk[ChunksInColumn];

        public ChunkColumn(World world, int chunkColumnX, int chunkColumnZ)
        {
            _world = world;

            ChunkColumnX = chunkColumnX;
            ChunkColumnZ = chunkColumnZ;

            for (var y = 0; y < _chunks.Length; y++)
            {
                _chunks[y] = new Chunk(_world, chunkColumnX, chunkColumnZ, -ChunksNegativeInColumn + y);
            }
        }

        public static int GetChunksArrayIndex(int chunkY)
        {
            if (!IsValidChunkPos(chunkY)) { throw new ArgumentOutOfRangeException(); }

            return ChunksNegativeInColumn + chunkY;
        }

        public Chunk? GetChunk(int chunkY)
        {
            if (!IsValidChunkPos(chunkY)) { return null; }

            return _chunks[GetChunksArrayIndex(chunkY)];
        }

        public static bool IsValidChunkPos(int y)
        {
            var invalid = ChunksNegativeInColumn + y < 0 || y >= ChunksPositiveInColumn;

            return !invalid;
        }
    }
}
