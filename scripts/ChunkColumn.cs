using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class ChunkColumn
    {
        public readonly Chunk[] Sections = new Chunk[ChunkSections];

        public ChunkColumn()
        {
            for (var i = 0; i < Sections.Length; i++)
            {
                Sections[i] = new Chunk();
            }
        }

        public static int GetChunkSectionArrayIndex(int chunkSectionY)
        {
            return (int) ChunkNegativeSections + chunkSectionY;
        }

        public Chunk GetChunkSection(int chunkSectionY)
        {
            return Sections[GetChunkSectionArrayIndex(chunkSectionY)];
        }
    }
}
