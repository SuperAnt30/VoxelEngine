using System;
using VoxelEngine.Glm;

namespace VoxelEngine.Util
{
    /// <summary>
    /// Позиция блока
    /// </summary>
    public class BlockPos
    {
        public int X { get; protected set; } = 0;
        public int Y { get; protected set; } = 0;
        public int Z { get; protected set; } = 0;

        public BlockPos()
        {
        }

        public BlockPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public BlockPos(float x, float y, float z)
        {

            X = Mth.Floor(x);
            Y = Mth.Floor(y);
            Z = Mth.Floor(z);
        }

        public BlockPos(vec3 v)
        {

            X = Mth.Floor(v.x);
            Y = Mth.Floor(v.y);
            Z = Mth.Floor(v.z);
        }

        public BlockPos(vec3i v)
        {
            X = v.x;
            Y = v.y;
            Z = v.z;
        }

        public BlockPos Add(float x, float y, float z)
        {
            return new BlockPos((float)X + x, (float)Y + y, (float)Z + z);
        }

        public BlockPos Add(int x, int y, int z)
        {
            return new BlockPos(X + x, Y + y, Z + z);
        }

        public BlockPos Add(vec3i v)
        {
            return new BlockPos(X + v.x, Y + v.y, Z + v.z);
        }

        /// <summary>
        /// Позиция соседнего блока
        /// </summary>
        public BlockPos Offset(Pole pole)
        {
            return new BlockPos(ToVec3i() + EnumFacing.DirectionVec(pole));
        }

        /// <summary>
        /// Позиция блока снизу
        /// </summary>
        public BlockPos OffsetDown()
        {
            return new BlockPos(ToVec3i() + EnumFacing.DirectionVec(Pole.Down));
        }

        /// <summary>
        /// Позиция блока сверху
        /// </summary>
        public BlockPos OffsetUp()
        {
            return new BlockPos(ToVec3i() + EnumFacing.DirectionVec(Pole.Up));
        }
        /// <summary>
        /// Позиция блока сверху
        /// </summary>
        public BlockPos OffsetUp(int i)
        {
            return new BlockPos(ToVec3i() + (EnumFacing.DirectionVec(Pole.Up) * i));
        }

        public vec3i ToVec3i()
        {
            return new vec3i(X, Y, Z);
        }

        #region ToString support

        public override string ToString()
        {
            return String.Format("{0}; {1}; {2}", X, Y, Z);
        }

        #endregion
    }
}
