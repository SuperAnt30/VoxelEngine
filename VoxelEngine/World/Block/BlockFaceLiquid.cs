using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    /// <summary>
    /// Построение жидкого блока с разной стороны
    /// </summary>
    public class BlockFaceLiquid
    {
        public BlockFaceUV BlockFace { get; protected set; }
        /// <summary>
        /// Объект блока
        /// </summary>
        public Block Blk { get; protected set; }
        /// <summary>
        /// Уровни высот каждого угла
        ///    x-----w------> X
        ///    |     |
        ///    | up  |
        ///    y-----z
        ///    |
        ///    V Z
        /// </summary>
        protected vec4 level;
        /// <summary>
        /// Нулевые координаты текстуры
        /// </summary>
        protected vec2 uv;
        /// <summary>
        /// Текущая коробка
        /// </summary>
        protected Box box;
        /// <summary>
        /// вспомогательный
        /// </summary>
        protected float y;

        public BlockFaceLiquid(BlockFaceUV blockFace, Block block, Box box, vec4 level, vec2 uv)
        {
            BlockFace = blockFace;
            Blk = block;
            y = Blk.Position.Y + 1f;
            this.box = box;
            this.level = level;
            this.uv = uv;
        }

        public void RenderMeshSide(Pole pole)
        {
            switch(pole)
            {
                case Pole.East: East(); break;
                case Pole.West: West(); break;
                case Pole.North: North(); break;
                case Pole.South: South(); break;
                case Pole.Up: Up(); break;
            }
        }

        protected void Up()
        {
            BlockFace.SetVec(
                new vec3(Blk.Position.X + box.From.x, y - level.x, Blk.Position.Z + box.From.z),
                new vec3(Blk.Position.X + box.From.x, y - level.y, Blk.Position.Z + box.To.z),
                new vec3(Blk.Position.X + box.To.x, y - level.z, Blk.Position.Z + box.To.z),
                new vec3(Blk.Position.X + box.To.x, y - level.w, Blk.Position.Z + box.From.z)
            );
            // Определяем направление течения

            float w = level.x + level.y;
            float s = level.y + level.z;
            float e = level.z + level.w;
            float n = level.w + level.x;

            //if (level.x > level.y && level.w > level.z)
            //{
            //    if (level.x == level.w) FlowSouth();
            //    else if (level.x > level.w) FlowSouthEast();
            //    else FlowSouthWest();
            //}

            if (w > e && w >= s && w >= n)
            {
                FlowWest();
            }
            else if(s >= w && s >= e && s > n)
            {
                FlowSouth();
            }
            else if (e > w && e >= s && e >= n)
            {
                FlowEast();
            }
            else
            {
                FlowNorth();
            }
        }

        protected void South()
        {
            BlockFace.SetVec(
                new vec3(Blk.Position.X + box.To.x, Blk.Position.Y + box.From.y, Blk.Position.Z + box.To.z),
                new vec3(Blk.Position.X + box.To.x, y - level.z, Blk.Position.Z + box.To.z),
                new vec3(Blk.Position.X + box.From.x, y - level.y, Blk.Position.Z + box.To.z),
                new vec3(Blk.Position.X + box.From.x, Blk.Position.Y + box.From.y, Blk.Position.Z + box.To.z)
            );
            UV(level.z, level.y);
        }

        protected void North()
        {
            BlockFace.SetVec(
                new vec3(Blk.Position.X + box.From.x, Blk.Position.Y + box.From.y, Blk.Position.Z + box.From.z),
                new vec3(Blk.Position.X + box.From.x, y - level.x, Blk.Position.Z + box.From.z),
                new vec3(Blk.Position.X + box.To.x, y - level.w, Blk.Position.Z + box.From.z),
                new vec3(Blk.Position.X + box.To.x, Blk.Position.Y + box.From.y, Blk.Position.Z + box.From.z)
            );
            UV(level.x, level.w);
        }

        protected void East()
        {
            BlockFace.SetVec(
                new vec3(Blk.Position.X + box.To.x, Blk.Position.Y + box.From.y, Blk.Position.Z + box.From.z),
                new vec3(Blk.Position.X + box.To.x, y - level.w, Blk.Position.Z + box.From.z),
                new vec3(Blk.Position.X + box.To.x, y - level.z, Blk.Position.Z + box.To.z),
                new vec3(Blk.Position.X + box.To.x, Blk.Position.Y + box.From.y, Blk.Position.Z + box.To.z)
            );
            UV(level.w, level.z);
        }

        protected void West()
        {
            BlockFace.SetVec(
                new vec3(Blk.Position.X + box.From.x, Blk.Position.Y + box.From.y, Blk.Position.Z + box.To.z),
                new vec3(Blk.Position.X + box.From.x, y - level.y, Blk.Position.Z + box.To.z),
                new vec3(Blk.Position.X + box.From.x, y - level.x, Blk.Position.Z + box.From.z),
                new vec3(Blk.Position.X + box.From.x, Blk.Position.Y + box.From.y, Blk.Position.Z + box.From.z)
            );
            UV(level.y, level.x);
        }

        /// <summary>
        /// Текстура стороны
        /// </summary>
        /// <param name="l">верхний уровень слево</param>
        /// <param name="r">верхний уровень справа</param>
        protected void UV(float l, float r)
        {
            BlockFace.SetUV(
                new vec2(uv.x, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y + VE.UV_SIZE * l),
                new vec2(uv.x + VE.UV_SIZE, uv.y + VE.UV_SIZE * r),
                new vec2(uv.x + VE.UV_SIZE, uv.y + VE.UV_SIZE)
            );
        }

        /// <summary>
        /// Течение на North
        /// </summary>
        protected void FlowNorth()
        {
            BlockFace.SetUV(
                new vec2(uv.x, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y),
                new vec2(uv.x + VE.UV_SIZE, uv.y),
                new vec2(uv.x + VE.UV_SIZE, uv.y + VE.UV_SIZE)
            );
        }

        /// <summary>
        /// Течение на West
        /// </summary>
        protected void FlowWest()
        {
            BlockFace.SetUV(
                new vec2(uv.x + VE.UV_SIZE, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y),
                new vec2(uv.x + VE.UV_SIZE, uv.y)
            );
        }

        /// <summary>
        /// Течение на South
        /// </summary>
        protected void FlowSouth()
        {
            BlockFace.SetUV(
                new vec2(uv.x + VE.UV_SIZE, uv.y),
                new vec2(uv.x + VE.UV_SIZE, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y)
            );
        }

        /// <summary>
        /// Течение на East
        /// </summary>
        protected void FlowEast()
        {
            BlockFace.SetUV(
                new vec2(uv.x, uv.y),
                new vec2(uv.x + VE.UV_SIZE, uv.y),
                new vec2(uv.x + VE.UV_SIZE, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y + VE.UV_SIZE)
            );
        }

        /// <summary>
        /// Течение на SouthEast
        /// </summary>
        protected void FlowSouthEast()
        {
            float f0 = VE.UV_SIZE * .5f;
            float f1 = VE.UV_SIZE - f0;
            BlockFace.SetUV(
                new vec2(uv.x + f1, uv.y),
                new vec2(uv.x + VE.UV_SIZE, uv.y + f1),
                new vec2(uv.x + f0, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y + f0)
            );
        }

        /// <summary>
        /// Течение на NorthEast
        /// </summary>
        protected void FlowNorthEast()
        {
            float f0 = VE.UV_SIZE * .5f;
            float f1 = VE.UV_SIZE - f0;
            BlockFace.SetUV(
                new vec2(uv.x, uv.y + f0),
                new vec2(uv.x + f1, uv.y),
                new vec2(uv.x + VE.UV_SIZE, uv.y + f1),
                new vec2(uv.x + f0, uv.y + VE.UV_SIZE)
            );
        }

        /// <summary>
        /// Течение на NorthWest
        /// </summary>
        protected void FlowNorthWest()
        {
            float f0 = VE.UV_SIZE * .5f;
            float f1 = VE.UV_SIZE - f0;
            BlockFace.SetUV(
                new vec2(uv.x + f0, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y + f0),
                new vec2(uv.x + f1, uv.y),
                new vec2(uv.x + VE.UV_SIZE, uv.y + f1)
            );
        }

        /// <summary>
        /// Течение на SouthWest
        /// </summary>
        protected void FlowSouthWest()
        {
            float f0 = VE.UV_SIZE * .5f;
            float f1 = VE.UV_SIZE - f0;
            BlockFace.SetUV(
                new vec2(uv.x + VE.UV_SIZE, uv.y + f1),
                new vec2(uv.x + f0, uv.y + VE.UV_SIZE),
                new vec2(uv.x, uv.y + f0),
                new vec2(uv.x + f1, uv.y)
            );
        }

    }
}
