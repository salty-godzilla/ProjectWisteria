namespace ProjectWisteria
{
    namespace Coord
    {
        public struct ChunkCoord
        {
            public int X { get; set; }
            public int Z { get; set; }

            public ChunkCoord(int x, int z)
            {
                X = x;
                Z = z;
            }
        }

        public struct ChunkSectionCoord
        {
            public int Y { get; set; }

            public ChunkSectionCoord(int y)
            {
                Y = y;
            }
        }

        public struct ChunkSectionGlobalCoord
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public ChunkSectionGlobalCoord(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
    }
}
