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
            for (var y = 0; y < ChunkSize; y++)
            {
                for (var z = 0; z < ChunkSize; z++)
                {
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        var blockGlobalX = coord.X * ChunkSize + x;
                        var blockGlobalY = coord.Y * ChunkSize + y;
                        var blockGlobalZ = coord.Z * ChunkSize + z;

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
