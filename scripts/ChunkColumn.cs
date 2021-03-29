using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class ChunkColumn
    {
        public readonly Chunk[] Sections = new Chunk[ChunksInColumn];

        public ChunkColumn()
        {
            for (var i = 0; i < Sections.Length; i++)
            {
                Sections[i] = new Chunk();
            }
        }

        public static int GetChunkSectionArrayIndex(int chunkSectionY)
        {
            return ChunksNegativeInColumn + chunkSectionY;
        }

        public Chunk GetChunkSection(int chunkSectionY)
        {
            return Sections[GetChunkSectionArrayIndex(chunkSectionY)];
        }
    }
}
