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

        private readonly List<Vector3> _verts = new List<Vector3>();
        private readonly List<Vector2> _uvs = new List<Vector2>();
        private readonly List<Vector3> _normals = new List<Vector3>();
        private readonly List<int> _tris = new List<int>();

        public void Generate(out ArrayMesh mesh, ChunkSection section)
        {
            if (section.IsOnlyAirs())
            {
                mesh = null;
                return;
            }

            for (byte blockY = 0; blockY < ChunkSectionSize; blockY++)
            {
                for (byte blockZ = 0; blockZ < ChunkSectionSize; blockZ++)
                {
                    for (byte blockX = 0; blockX < ChunkSectionSize; blockX++)
                    {
                        if (section.GetBlock(blockX, blockY, blockZ) == BlockType.Air) { continue; }

                        if (IsXpFaceVisible(blockX, blockY, blockZ, section))
                        {
                            AddXpBlockFaceElems(blockX, blockY, blockZ);
                        }

                        if (IsXnFaceVisible(blockX, blockY, blockZ, section))
                        {
                            AddXnBlockFaceElems(blockX, blockY, blockZ);
                        }

                        if (IsYpFaceVisible(blockX, blockY, blockZ, section))
                        {
                            AddYpBlockFaceElems(blockX, blockY, blockZ);
                        }

                        if (IsYnFaceVisible(blockX, blockY, blockZ, section))
                        {
                            AddYnBlockFaceElems(blockX, blockY, blockZ);
                        }

                        if (IsZpFaceVisible(blockX, blockY, blockZ, section))
                        {
                            AddZpBlockFaceElems(blockX, blockY, blockZ);
                        }

                        if (IsZnFaceVisible(blockX, blockY, blockZ, section))
                        {
                            AddZnBlockFaceElems(blockX, blockY, blockZ);
                        }
                    }
                }
            }

            mesh = new ArrayMesh();

            var arrays = new Array();
            arrays.Resize((int) Mesh.ArrayType.Max);
            arrays[(int) Mesh.ArrayType.Vertex] = _verts.ToArray();
            arrays[(int) Mesh.ArrayType.TexUv] = _uvs.ToArray();
            arrays[(int) Mesh.ArrayType.Normal] = _normals.ToArray();
            arrays[(int) Mesh.ArrayType.Index] = _tris.ToArray();

            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

            var material = new SpatialMaterial
            {
                AlbedoColor = Color.Color8(255, 255, 255)
            };

            mesh.SurfaceSetMaterial(0, material);

            _verts.Clear();
            _uvs.Clear();
            _normals.Clear();
            _tris.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsXpFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (x < ChunkSectionSize - 1)
            {
                return section.GetBlock((byte) (x + 1), y, z) == BlockType.Air;
            }

            if (section.XpNeighbor == null) { return true; }

            return section.XpNeighbor.GetBlock(0, y, z) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsXnFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (x > 0)
            {
                return section.GetBlock((byte) (x - 1), y, z) == BlockType.Air;
            }

            if (section.XnNeighbor == null) { return true; }

            return section.XnNeighbor.GetBlock((byte) (ChunkSectionSize - 1), y, z) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsZpFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (z < ChunkSectionSize - 1)
            {
                return section.GetBlock(x, y, (byte) (z + 1)) == BlockType.Air;
            }

            if (section.ZpNeighbor == null) { return true; }

            return section.ZpNeighbor.GetBlock(x, y, 0) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsZnFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (z > 0)
            {
                return section.GetBlock(x, y, (byte) (z - 1)) == BlockType.Air;
            }

            if (section.ZnNeighbor == null) { return true; }

            return section.ZnNeighbor.GetBlock(x, y, (byte) (ChunkSectionSize - 1)) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsYpFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (y < ChunkSectionSize - 1)
            {
                return section.GetBlock(x, (byte) (y + 1), z) == BlockType.Air;
            }

            if (section.YpNeighbor == null) { return true; }

            return section.YpNeighbor.GetBlock(x, 0, z) == BlockType.Air;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsYnFaceVisible(byte x, byte y, byte z, ChunkSection section)
        {
            if (y > 0)
            {
                return section.GetBlock(x, (byte) (y - 1), z) == BlockType.Air;
            }

            if (section.YnNeighbor == null) { return true; }

            return section.YnNeighbor.GetBlock(x, (byte) (ChunkSectionSize - 1), z) == BlockType.Air;
        }

        private void AddXpBlockFaceElems(byte x, byte y, byte z)
        {
            _verts.Add(new Vector3(x + 1, y + 1, z + 1));
            _verts.Add(new Vector3(x + 1, y + 1, z));
            _verts.Add(new Vector3(x + 1, y, z));
            _verts.Add(new Vector3(x + 1, y, z + 1));

            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(0, 1));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddXnBlockFaceElems(byte x, byte y, byte z)
        {
            _verts.Add(new Vector3(x, y + 1, z));
            _verts.Add(new Vector3(x, y + 1, z + 1));
            _verts.Add(new Vector3(x, y, z + 1));
            _verts.Add(new Vector3(x, y, z));

            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(0, 1));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddYpBlockFaceElems(byte x, byte y, byte z)
        {
            _verts.Add(new Vector3(x, y + 1, z));
            _verts.Add(new Vector3(x + 1, y + 1, z));
            _verts.Add(new Vector3(x + 1, y + 1, z + 1));
            _verts.Add(new Vector3(x, y + 1, z + 1));

            _normals.Add(new Vector3(0, 1, 0));
            _normals.Add(new Vector3(0, 1, 0));
            _normals.Add(new Vector3(0, 1, 0));
            _normals.Add(new Vector3(0, 1, 0));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(0, 1));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddYnBlockFaceElems(byte x, byte y, byte z)
        {
            _verts.Add(new Vector3(x, y, z + 1));
            _verts.Add(new Vector3(x + 1, y, z + 1));
            _verts.Add(new Vector3(x + 1, y, z));
            _verts.Add(new Vector3(x, y, z));

            _normals.Add(new Vector3(0, -1, 0));
            _normals.Add(new Vector3(0, -1, 0));
            _normals.Add(new Vector3(0, -1, 0));
            _normals.Add(new Vector3(0, -1, 0));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(0, 1));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddZpBlockFaceElems(byte x, byte y, byte z)
        {
            _verts.Add(new Vector3(x, y + 1, z + 1));
            _verts.Add(new Vector3(x + 1, y + 1, z + 1));
            _verts.Add(new Vector3(x + 1, y, z + 1));
            _verts.Add(new Vector3(x, y, z + 1));

            _normals.Add(new Vector3(0, 0, 1));
            _normals.Add(new Vector3(0, 0, 1));
            _normals.Add(new Vector3(0, 0, 1));
            _normals.Add(new Vector3(0, 0, 1));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(0, 1));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }

        private void AddZnBlockFaceElems(byte x, byte y, byte z)
        {
            _verts.Add(new Vector3(x + 1, y + 1, z));
            _verts.Add(new Vector3(x, y + 1, z));
            _verts.Add(new Vector3(x, y, z));
            _verts.Add(new Vector3(x + 1, y, z));

            _normals.Add(new Vector3(0, 0, -1));
            _normals.Add(new Vector3(0, 0, -1));
            _normals.Add(new Vector3(0, 0, -1));
            _normals.Add(new Vector3(0, 0, -1));

            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, 1));
            _uvs.Add(new Vector2(0, 1));

            foreach (var triangle in _baseBlockTriangles)
            {
                _tris.Add(triangle + _verts.Count - 4);
            }
        }
    }
}
