using System.Collections.Generic;
using Godot;
using ProjectWisteria.Coord;
using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class World : Node
    {
        private readonly Dictionary<ChunkCoord, Chunk> _chunks = new Dictionary<ChunkCoord, Chunk>();

        private readonly Dictionary<ChunkSectionGlobalCoord, Node> _sectionNodes =
            new Dictionary<ChunkSectionGlobalCoord, Node>();

        private readonly Queue<ChunkSectionGlobalCoord> _needRenderUpdateChunkSections =
            new Queue<ChunkSectionGlobalCoord>();

        private TerrainGenerator _terrainGenerator;

        private ChunkSectionMeshGenerator _chunkSectionMeshGenerator;

        public override void _Ready()
        {
            _terrainGenerator = new TerrainGenerator();

            _chunkSectionMeshGenerator = new ChunkSectionMeshGenerator();

            for (var chunkX = -(int) WorldSizeX / 2; chunkX <= (WorldSizeX - 1) / 2; chunkX++)
            {
                for (var chunkZ = -(int) WorldSizeZ / 2; chunkZ <= (WorldSizeZ - 1) / 2; chunkZ++)
                {
                    var chunk = new Chunk();

                    for (var chunkSectionY = -(int) ChunkNegativeSections;
                        chunkSectionY < ChunkPositiveSections;
                        chunkSectionY++)
                    {
                        var chunkSectionGlobalCoord = new ChunkSectionGlobalCoord(chunkX, chunkSectionY, chunkZ);

                        _terrainGenerator.Generate(chunk.Sections[Chunk.GetChunkSectionArrayIndex(chunkSectionY)],
                            chunkSectionGlobalCoord);

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

                        var chunkSectionMeshNode = new MeshInstance
                        {
                            Name = "BlockMesh"
                        };
                        chunkSectionNode.AddChild(chunkSectionMeshNode);

                        _sectionNodes[chunkSectionGlobalCoord] = chunkSectionNode;
                        _needRenderUpdateChunkSections.Enqueue(chunkSectionGlobalCoord);
                    }

                    var chunkCoord = new ChunkCoord(chunkX, chunkZ);
                    _chunks.Add(chunkCoord, chunk);
                }
            }
        }

        public override void _Process(float delta)
        {
            if (_needRenderUpdateChunkSections.Count > 0)
            {
                var globalCoord = _needRenderUpdateChunkSections.Dequeue();
                var chunkCoord = new ChunkCoord(globalCoord.X, globalCoord.Z);

                var chunk = _chunks[chunkCoord];
                var chunkSection = chunk.GetChunkSection(globalCoord.Y);

                _chunkSectionMeshGenerator.Generate(out var mesh, chunkSection);

                ((MeshInstance) _sectionNodes[globalCoord].GetChild(0)).Mesh = mesh;
            }
        }
    }
}
