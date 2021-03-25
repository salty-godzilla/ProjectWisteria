using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class Chunk
    {
        public ChunkSection[] Sections = new ChunkSection[ChunkSections];

        public Chunk()
        {
            for (var i = 0; i < Sections.Length; i++)
            {
                Sections[i] = new ChunkSection();
            }
        }

        public static int GetChunkSectionArrayIndex(int chunkSectionY)
        {
            return (int) ChunkNegativeSections + chunkSectionY;
        }

        public ChunkSection GetChunkSection(int chunkSectionY)
        {
            return Sections[GetChunkSectionArrayIndex(chunkSectionY)];
        }
    }
}