using ProjectWisteria.Coord;
using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class TerrainGenerator
    {
        private readonly FastNoiseLite _noise;

        public TerrainGenerator()
        {
            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        }

        public void Generate(Chunk section, ChunkSectionGlobalCoord coord)
        {
            for (byte y = 0; y < ChunkSectionSize; y++)
            {
                for (byte z = 0; z < ChunkSectionSize; z++)
                {
                    for (byte x = 0; x < ChunkSectionSize; x++)
                    {
                        var blockGlobalX = coord.X * ChunkSectionSize + x;
                        var blockGlobalY = coord.Y * ChunkSectionSize + y;
                        var blockGlobalZ = coord.Z * ChunkSectionSize + z;

                        var height = (int) (_noise.GetNoise(blockGlobalX * 2f, blockGlobalZ * 2f) * 4
                                            + _noise.GetNoise(blockGlobalX, blockGlobalZ) * 4);

                        if (height == blockGlobalY)
                        {
                            section.SetBlock(x, y, z, BlockType.Grass);
                        }
                        else if (height > blockGlobalY)
                        {
                            section.SetBlock(x, y, z, BlockType.Dirt);
                        }
                    }
                }
            }
        }
    }
}
