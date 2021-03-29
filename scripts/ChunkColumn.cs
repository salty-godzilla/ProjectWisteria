using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class ChunkColumn
    {
        public readonly ChunkSection[] Sections = new ChunkSection[ChunkSections];

        public ChunkColumn()
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
