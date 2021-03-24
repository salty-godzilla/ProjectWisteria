using ProjectWisteria.Coord;
using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class TerrainGenerator
    {
        public void Generate(ChunkSection section, ChunkSectionGlobalCoord coord)
        {
            for (byte y = 0; y < ChunkSectionSize; y++)
            {
                for (byte z = 0; z < ChunkSectionSize; z++)
                {
                    for (byte x = 0; x < ChunkSectionSize; x++)
                    {
                        if (coord.Y < 0)
                        {
                            section.SetBlock(x, y, z, BlockType.Dirt);
                        }
                    }
                }
            }
        }
    }
}
