using System.Collections.Generic;
using Godot;
using Directory = System.IO.Directory;
using Path = System.IO.Path;

namespace ProjectWisteria
{
    public class BlockDictionary : Node
    {
        public Dictionary<BlockType, Block> Blocks = new();
        public Dictionary<string, int> Textures = new();

        public TextureArray TextureArray { get; private set; }

        public static BlockDictionary Instance { get; private set; }

        public override void _Ready()
        {
            if (Instance != null && Instance != this)
            {
                GD.PrintErr($"{GetType().Name} is already exists.");
                GetParent().RemoveChild(this);
                return;
            }

            Instance = this;

            RegisterBlock(BlockType.Dirt,
                new List<string> {"dirt.png"},
                new List<int> {0, 0, 0, 0, 0, 0});

            RegisterBlock(BlockType.Grass,
                new List<string> {"dirt.png", "grass.png", "grass_side.png"},
                new List<int> {2, 2, 1, 0, 2, 2});

            GenerateTextureArray();
        }

        public void RegisterBlock(BlockType blockType, List<string> tileImages, List<int> facesTextureIndex)
        {
            var tilesTextureIndex = new List<int>(tileImages.Count);

            // Register tiles texture
            for (var i = 0; i < tileImages.Count; i++)
            {
                if (Textures.TryGetValue(tileImages[i], out var index))
                {
                    // Texture already registered
                    tilesTextureIndex.Add(index);
                }
                else
                {
                    var newIndex = Textures.Count;
                    Textures.Add(tileImages[i], newIndex);
                    tilesTextureIndex.Add(newIndex);
                }
            }

            var block = new Block
            {
                BlockType = blockType,
                XpTextureIndex = tilesTextureIndex[facesTextureIndex[0]],
                XnTextureIndex = tilesTextureIndex[facesTextureIndex[1]],
                YpTextureIndex = tilesTextureIndex[facesTextureIndex[2]],
                YnTextureIndex = tilesTextureIndex[facesTextureIndex[3]],
                ZpTextureIndex = tilesTextureIndex[facesTextureIndex[4]],
                ZnTextureIndex = tilesTextureIndex[facesTextureIndex[5]]
            };

            Blocks.Add(blockType, block);
        }

        public void GenerateTextureArray()
        {
            TextureArray = new TextureArray();
            TextureArray.Create(16, 16, (uint) Textures.Count, Image.Format.Rgba8,
                (uint) Texture.FlagsEnum.Repeat | (uint) Texture.FlagsEnum.Mipmaps);

            var texturePackDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
            if (!OS.HasFeature("standalone"))
            {
                texturePackDir += "exports" + Path.DirectorySeparatorChar;
            }

            texturePackDir += "texture_pack";

            foreach (var texture in Textures)
            {
                var filePath = $"{texturePackDir}/{texture.Key}";

                var image = new Image();
                image.Load(filePath);
                image.GenerateMipmaps();

                TextureArray.SetLayerData(image, texture.Value);
            }
        }
    }
}
