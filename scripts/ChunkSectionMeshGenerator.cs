using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;
using static ProjectWisteria.WorldConstants;

namespace ProjectWisteria
{
    public class ChunkSectionMeshGenerator
    {
        private readonly int[] _baseBlockTriangles =
        {
            0, 1, 2, // 0
            0, 2, 3 // 1
        };

        private readonly List<Vector3> _verts = new();
        private readonly List<Vector2> _uvs = new();
        private readonly List<Vector2> _uv2s = new();
        private readonly List<Vector3> _normals = new();
        private readonly List<int> _tris = new();

        private readonly ShaderMaterial _material;

        private const string MaterialPath = "res://materials/block_array.tres";

        public ChunkSectionMeshGenerator()
        {
            _material = ResourceLoader.Load(MaterialPath) as ShaderMaterial;

            _material.SetShaderParam("texture_albedo", BlockDictionary.Instance.TextureArray);
        }

        public void Generate(out ArrayMesh mesh, ChunkSection section)
        {
            if (section.IsOnlyAirs())
            {
                mesh = null;
                return;
            }

            // Create X and Y faces
            for (byte blockY = 0; blockY < ChunkSectionSize; blockY++)
            {
                for (byte blockX = 0; blockX < ChunkSectionSize; blockX++)
                {
                    for (byte blockZ = 0; blockZ < ChunkSectionSize;)
                    {
                        var startBlock = section.GetBlock(blockX, blockY, blockZ);

                        if (startBlock == BlockType.Air)
                        {
                            blockZ++;
                            continue;
                        }

                        var mergedZLength = CreateMergedFaceXy(startBlock, section, blockX, blockY, blockZ);
                        blockZ += mergedZLength;
                    }
                }
            }

            // Create Z faces
            for (byte blockY = 0; blockY < ChunkSectionSize; blockY++)
            {
                for (byte blockZ = 0; blockZ < ChunkSectionSize; blockZ++)
                {
                    for (byte blockX = 0; blockX < ChunkSectionSize;)
                    {
                        var startBlock = section.GetBlock(blockX, blockY, blockZ);

                        if (startBlock == BlockType.Air)
                        {
                            blockX++;
                            continue;
                        }

                        var mergedZLength = CreateMergedFaceZ(startBlock, section, blockX, blockY, blockZ);
                        blockX += mergedZLength;
                    }
                }
            }

            mesh = new ArrayMesh();

            var arrays = new Array();
            arrays.Resize((int) Mesh.ArrayType.Max);
            arrays[(int) Mesh.ArrayType.Vertex] = _verts.ToArray();
            arrays[(int) Mesh.ArrayType.TexUv] = _uvs.ToArray();
            arrays[(int) Mesh.ArrayType.TexUv2] = _uv2s.ToArray();
            arrays[(int) Mesh.ArrayType.Normal] = _normals.ToArray();
            arrays[(int) Mesh.ArrayType.Index] = _tris.ToArray();

            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

            mesh.SurfaceSetMaterial(0, _material);

            _verts.Clear();
            _uvs.Clear();
            _uv2s.Clear();
            _normals.Clear();
            _tris.Clear();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte CreateMergedFaceXy(BlockType startBlock, ChunkSection section, byte startX, byte startY,
            byte startZ)
        {
            var isXpFaceVisible = IsXpFaceVisible(startX, startY, startZ, section);
            var isXnFaceVisible = IsXnFaceVisible(startX, startY, startZ, section);
            var isYpFaceVisible = IsYpFaceVisible(startX, startY, startZ, section);
            var isYnFaceVisible = IsYnFaceVisible(startX, startY, startZ, section);

            if (!isXpFaceVisible && !isXnFaceVisible && !isYpFaceVisible && !isYnFaceVisible)
            {
                return 1;
            }

            var length = (byte) 1;

            // Z-axis scanning
            for (var z = (byte) (startZ + 1); z < ChunkSectionSize; z++)
            {
                if (IsDifferentBlock(section, startX, startY, z, startBlock)) { break; }

                if (IsXpFaceVisible(startX, startY, z, section)) { isXpFaceVisible = true; }

                if (IsXnFaceVisible(startX, startY, z, section)) { isXnFaceVisible = true; }

                if (IsYpFaceVisible(startX, startY, z, section)) { isYpFaceVisible = true; }

                if (IsYnFaceVisible(startX, startY, z, section)) { isYnFaceVisible = true; }

                length++;
            }

            if (isXpFaceVisible)
            {
                AddXpBlockFaceElems(startX, startY, startZ, length, startBlock);
            }

            if (isXnFaceVisible)
            {
                AddXnBlockFaceElems(startX, startY, startZ, length, startBlock);
            }

            if (isYpFaceVisible)
            {
                AddYpBlockFaceElems(startX, startY, startZ, length, startBlock);
            }

            if (isYnFaceVisible)
            {
                AddYnBlockFaceElems(startX, startY, startZ, length, startBlock);
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte CreateMergedFaceZ(BlockType startBlock, ChunkSection section, byte startX, byte startY,
            byte startZ)
        {
            var isZpFaceVisible = IsZpFaceVisible(startX, startY, startZ, section);
            var isZnFaceVisible = IsZnFaceVisible(startX, startY, startZ, section);

            if (!isZpFaceVisible && !isZnFaceVisible)
            {
                return 1;
            }

            var length = (byte) 1;

            // X-axis scanning
            for (var x = (byte) (startX + 1); x < ChunkSectionSize; x++)
            {
                if (IsDifferentBlock(section, x, startY, startZ, startBlock)) { break; }

                if (IsZpFaceVisible(x, startY, startZ, section)) { isZpFaceVisible = true; }

                if (IsZnFaceVisible(x, startY, startZ, section)) { isZnFaceVisible = true; }

                length++;
            }

            if (isZpFaceVisible)
            {
                AddZpBlockFaceElems(startX, startY, startZ, length, startBlock);
            }

            if (isZnFaceVisible)
            {
                AddZnBlockFaceElems(startX, startY, startZ, length, startBlock);
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsXpFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (x < ChunkSectionSize - 1)
            {
                return section.GetBlock((byte) (x + 1), y, z) == BlockType.Air;
            }

            if (section.XpNeighbor == null) { return true; }

            return section.XpNeighbor.GetBlock(0, y, z) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsXnFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (x > 0)
            {
                return section.GetBlock((byte) (x - 1), y, z) == BlockType.Air;
            }

            if (section.XnNeighbor == null) { return true; }

            return section.XnNeighbor.GetBlock((byte) (ChunkSectionSize - 1), y, z) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsZpFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (z < ChunkSectionSize - 1)
            {
                return section.GetBlock(x, y, (byte) (z + 1)) == BlockType.Air;
            }

            if (section.ZpNeighbor == null) { return true; }

            return section.ZpNeighbor.GetBlock(x, y, 0) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsZnFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (z > 0)
            {
                return section.GetBlock(x, y, (byte) (z - 1)) == BlockType.Air;
            }

            if (section.ZnNeighbor == null) { return true; }

            return section.ZnNeighbor.GetBlock(x, y, (byte) (ChunkSectionSize - 1)) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsYpFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (y < ChunkSectionSize - 1)
            {
                return section.GetBlock(x, (byte) (y + 1), z) == BlockType.Air;
            }

            if (section.YpNeighbor == null) { return true; }

            return section.YpNeighbor.GetBlock(x, 0, z) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsYnFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (y > 0)
            {
                return section.GetBlock(x, (byte) (y - 1), z) == BlockType.Air;
            }

            if (section.YnNeighbor == null) { return true; }

            return section.YnNeighbor.GetBlock(x, (byte) (ChunkSectionSize - 1), z) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDifferentBlock(ChunkSection section, byte x, byte y, byte z, BlockType currentBlock)
        {
            return section.GetBlock(x, y, z) != currentBlock;
        }

        private void AddXpBlockFaceElems(byte x, byte y, byte z, byte len, BlockType block)
        {
            _verts.Add(new Vector3(x + 1, y + 1, z + len));
            _verts.Add(new Vector3(x + 1, y + 1, z));
            _verts.Add(new Vector3(x + 1, y, z));
            _verts.Add(new Vector3(x + 1, y, z + len));

            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(len, 0));
            _uvs.Add(new Vector2(len, 1));
            _uvs.Add(new Vector2(0, 1));

            var textureLayer = BlockDictionary.Instance.Blocks[block].XpTextureIndex;

            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddXnBlockFaceElems(byte x, byte y, byte z, byte len, BlockType block)
        {
            _verts.Add(new Vector3(x, y + 1, z));
            _verts.Add(new Vector3(x, y + 1, z + len));
            _verts.Add(new Vector3(x, y, z + len));
            _verts.Add(new Vector3(x, y, z));

            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(len, 0));
            _uvs.Add(new Vector2(len, 1));
            _uvs.Add(new Vector2(0, 1));

            var textureLayer = BlockDictionary.Instance.Blocks[block].XnTextureIndex;

            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddYpBlockFaceElems(byte x, byte y, byte z, byte len, BlockType block)
        {
            _verts.Add(new Vector3(x, y + 1, z));
            _verts.Add(new Vector3(x + 1, y + 1, z));
            _verts.Add(new Vector3(x + 1, y + 1, z + len));
            _verts.Add(new Vector3(x, y + 1, z + len));

            _normals.Add(new Vector3(0, 1, 0));
            _normals.Add(new Vector3(0, 1, 0));
            _normals.Add(new Vector3(0, 1, 0));
            _normals.Add(new Vector3(0, 1, 0));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, len));
            _uvs.Add(new Vector2(0, len));

            var textureLayer = BlockDictionary.Instance.Blocks[block].YpTextureIndex;

            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddYnBlockFaceElems(byte x, byte y, byte z, byte len, BlockType block)
        {
            _verts.Add(new Vector3(x, y, z + len));
            _verts.Add(new Vector3(x + 1, y, z + len));
            _verts.Add(new Vector3(x + 1, y, z));
            _verts.Add(new Vector3(x, y, z));

            _normals.Add(new Vector3(0, -1, 0));
            _normals.Add(new Vector3(0, -1, 0));
            _normals.Add(new Vector3(0, -1, 0));
            _normals.Add(new Vector3(0, -1, 0));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, len));
            _uvs.Add(new Vector2(0, len));

            var textureLayer = BlockDictionary.Instance.Blocks[block].YnTextureIndex;

            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddZpBlockFaceElems(byte x, byte y, byte z, byte len, BlockType block)
        {
            _verts.Add(new Vector3(x, y + 1, z + 1));
            _verts.Add(new Vector3(x + len, y + 1, z + 1));
            _verts.Add(new Vector3(x + len, y, z + 1));
            _verts.Add(new Vector3(x, y, z + 1));

            _normals.Add(new Vector3(0, 0, 1));
            _normals.Add(new Vector3(0, 0, 1));
            _normals.Add(new Vector3(0, 0, 1));
            _normals.Add(new Vector3(0, 0, 1));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(len, 0));
            _uvs.Add(new Vector2(len, 1));
            _uvs.Add(new Vector2(0, 1));

            var textureLayer = BlockDictionary.Instance.Blocks[block].ZpTextureIndex;

            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddZnBlockFaceElems(byte x, byte y, byte z, byte len, BlockType block)
        {
            _verts.Add(new Vector3(x + len, y + 1, z));
            _verts.Add(new Vector3(x, y + 1, z));
            _verts.Add(new Vector3(x, y, z));
            _verts.Add(new Vector3(x + len, y, z));

            _normals.Add(new Vector3(0, 0, -1));
            _normals.Add(new Vector3(0, 0, -1));
            _normals.Add(new Vector3(0, 0, -1));
            _normals.Add(new Vector3(0, 0, -1));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(len, 0));
            _uvs.Add(new Vector2(len, 1));
            _uvs.Add(new Vector2(0, 1));

            var textureLayer = BlockDictionary.Instance.Blocks[block].ZnTextureIndex;

            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));
            _uv2s.Add(new Vector2(textureLayer, 0));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }
    }
}
