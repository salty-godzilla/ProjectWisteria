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

            public override int GetHashCode()
            {
                return (X, Z).GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is ChunkColumnCoord a && a.X == X && a.Z == Z;
            }
        }

        public struct ChunkCoord
        {
            public int Y { get; set; }

            public ChunkCoord(int y)
            {
                Y = y;
            }

            public override int GetHashCode()
            {
                return Y;
            }

            public override bool Equals(object obj)
            {
                return obj is ChunkCoord a && a.Y == Y;
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

            public override int GetHashCode()
            {
                return (X, Y, Z).GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is ChunkGlobalCoord a && a.X == X && a.Y == Y && a.Z == Z;
            }
        }
    }
}
