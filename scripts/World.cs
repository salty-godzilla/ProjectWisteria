using System.Collections.Generic;
using Godot;
using ProjectWisteria.Coord;
using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class World : Node
    {
        private readonly Dictionary<ChunkCoord, Chunk> _chunks = new Dictionary<ChunkCoord, Chunk>();

        private readonly Dictionary<ChunkSectionGlobalCoord, Node> _chunkNodes =
            new Dictionary<ChunkSectionGlobalCoord, Node>();

        private readonly Queue<ChunkSectionGlobalCoord> _needRenderUpdateChunkSections =
            new Queue<ChunkSectionGlobalCoord>();

        public override void _Ready()
        {
            for (var chunkX = (int) WorldSizeX / 2 - (int) WorldSizeX; chunkX <= WorldSizeX / 2 - 1; chunkX++)
            {
                for (var chunkZ = (int) WorldSizeZ / 2 - (int) WorldSizeZ;
                    chunkZ <= WorldSizeZ / 2 - 1;
                    chunkZ++)
                {
                    var chunk = new Chunk();

                    for (var chunkSectionY = -(int) ChunkNegativeSections;
                        chunkSectionY < ChunkPositiveSections;
                        chunkSectionY++)
                    {
                        var chunkSectionNode = new Spatial
                        {
                            Translation = new Vector3(
                                chunkX * ChunkSectionSize,
                                chunkSectionY * ChunkSectionSize,
                                chunkZ * ChunkSectionSize
                            ),
                            Name = $"ChunkSection ({chunkX}, {chunkZ}) [{chunkSectionY}]"
                        };

                        AddChild(chunkSectionNode);

                        var chunkSectionGlobalCoord = new ChunkSectionGlobalCoord(chunkX, chunkSectionY, chunkZ);

                        _chunkNodes[chunkSectionGlobalCoord] = chunkSectionNode;
                        _needRenderUpdateChunkSections.Enqueue(chunkSectionGlobalCoord);
                    }

                    var chunkCoord = new ChunkCoord(chunkX, chunkZ);
                    _chunks.Add(chunkCoord, chunk);
                }
            }
        }
    }
}
