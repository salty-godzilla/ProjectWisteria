using System;
using System.Collections.Generic;
using Godot;
using ProjectWisteria.Coord;
using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public class World : Node
    {
        private readonly Dictionary<ChunkCoord, Chunk> _chunks = new();
        private readonly Dictionary<ChunkSectionGlobalCoord, Node> _sectionNodes = new();

        private readonly Queue<ChunkSectionGlobalCoord> _needRenderUpdateChunkSections = new();

        private TerrainGenerator _terrainGenerator = null!;
        private ChunkSectionMeshGenerator _chunkSectionMeshGenerator = null!;

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

        public BlockType GetBlock(int x, int y, int z)
        {
            var chunkSectionX = x >> 4;
            var chunkSectionY = y >> 4;
            var chunkSectionZ = z >> 4;

            var blockLocalPosX = (byte) (x & 0b1111);
            var blockLocalPosY = (byte) (y & 0b1111);
            var blockLocalPosZ = (byte) (z & 0b1111);

            var chunk = _chunks[new ChunkCoord(chunkSectionX, chunkSectionZ)].GetChunkSection(chunkSectionY);

            var block = chunk.GetBlock(blockLocalPosX, blockLocalPosY, blockLocalPosZ);
            return block;
        }

        public List<AABB> GetAabb(AABB colliderAabb)
        {
            var aabbList = new List<AABB>();

            var x0 = (int) Math.Floor(colliderAabb.Position.x);
            var x1 = (int) Math.Floor(colliderAabb.End.x + 1);

            var y0 = (int) Math.Floor(colliderAabb.Position.y);
            var y1 = (int) Math.Floor(colliderAabb.End.y + 1);

            var z0 = (int) Math.Floor(colliderAabb.Position.z);
            var z1 = (int) Math.Floor(colliderAabb.End.z + 1);

            //GD.Print($"{x0} {x1} / {y0} {y1} / {z0} {z1}");

            for (var x = x0; x < x1; x++)
            {
                for (var y = y0; y < y1; y++)
                {
                    for (var z = z0; z < z1; z++)
                    {
                        if (GetBlock(x, y, z) != BlockType.Air)
                        {
                            var aabb = new AABB(x, y, z, 1, 1, 1);
                            aabbList.Add(aabb);
                        }
                    }
                }
            }

            return aabbList;
        }
    }
}
