namespace ProjectWisteria
{
    namespace Coord
    {
        public struct ChunkColumnCoord
        {
            public int X { get; set; }
            public int Z { get; set; }

            public ChunkColumnCoord(int x, int z)
            {
                X = x;
                Z = z;
            }
        }

        public struct ChunkCoord
        {
            public int Y { get; set; }

            public ChunkCoord(int y)
            {
                Y = y;
            }
        }

        public struct ChunkGlobalCoord
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public ChunkGlobalCoord(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
    }
}
