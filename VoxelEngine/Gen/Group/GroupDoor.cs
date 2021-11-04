using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;
using VoxelEngine.World.Chk;

namespace VoxelEngine.Gen.Group
{
    /// <summary>
    /// Дверь
    /// </summary>
    public class GroupDoor : GroupBase
    {
        public GroupDoor(WorldBase world, vec3i pos) : base(world, pos)
        {
            Size = new vec3i(2, 4, 2);
            Type = EnumGroup.Door;
        }

        /// <summary>
        /// Открыта
        /// </summary>
        protected bool isOpen = false;
        /// <summary>
        /// С левой стороны петли 
        /// </summary>
        protected bool isLeft = false;
        /// <summary>
        /// С какой стороны, полюс 4 шт
        /// </summary>
        protected Pole side = Pole.South;

        /// <summary>
        /// Задать параметры
        /// </summary>
        public override void SetProperties(ushort prop)
        {
            base.SetProperties(prop);
            // Из 16 bit получить данные двери
            side = (Pole)((prop & 3) + 2);   // 2 bit
            isLeft = ((prop >> 2) & 1) == 1; // 1 bit
            isOpen = ((prop >> 3) & 1) == 1; // 1 bit

            Generation(false);
        }

        /// <summary>
        /// Получить параметры
        /// </summary>
        public override ushort GetProperties()
        {
            // Из параметров сгенерировать 16 bit параметр
            int l = isLeft ? 1 : 0; // 1 bit
            int o = isOpen ? 1 : 0; // 1 bit
            int s = (int)side - 2;  // 2 bit
            properties = (ushort)(s | l << 2 | o << 3);
            return properties;
        }

        /// <summary>
        /// Поставить дверь
        /// </summary>
        public override bool Put()
        {
            float angle = glm.degrees(World.Entity.RotationYaw);
            side = EnumFacing.FromAngle(angle);
            isLeft = EnumFacing.IsFromAngleLeft(angle, side);

            // Переместить координату в зависимости от положения
            if (Check0(Pole.East, false)) Position += new vec3i(0, 0, -1);
            else if (Check0(Pole.North, true)) Position += new vec3i(0, 0, -1);
            else if (Check0(Pole.North, false)) Position += new vec3i(-1, 0, -1);
            else if (Check0(Pole.West, true)) Position += new vec3i(-1, 0, -1);
            else if (Check0(Pole.West, false)) Position += new vec3i(-1, 0, 0);
            else if (Check0(Pole.South, true)) Position += new vec3i(-1, 0, 0);
            else if (Check0(Pole.South, false)) Position += new vec3i(0, 0, 0);

            if (Check(Position))
            {
                Generation(true);
                ModifiedRender();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Удалить дверь
        /// </summary>
        /// <param name="block">блок клика двери</param>
        public void Remove(BlockBase block)
        {
            // Находим смещение координаты к контрольной точке
            vec3i bias = GroupPropertiesBias242(block.Properties);
            // контрольный блок
            vec3i pos = block.Position.ToVec3i() - bias;
            vec3i max = pos + Size;
            for (int y = pos.y; y < max.y; y++)
            {
                for (int x = pos.x; x < max.x; x++)
                {
                    for (int z = pos.z; z < max.z; z++)
                    {
                        BlockPos blockPos = new BlockPos(x, y, z);
                        SetBlockState(blockPos, EnumBlock.Air);
                        RemoveGroupChunk(blockPos);
                    }
                }
            }
            ModifiedRender();
        }

        /// <summary>
        /// Обновить колизию блока
        /// </summary>
        public override void CollisionRefrash(BlockBase block)
        {
            BoxDoor door = new BoxDoor();
            int b = block.Properties;

            if (CheckS() || CheckSr())
            {
                door = new BoxDoor(2, 0, b);
                if (door.Yes) block.SetHitBox(new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3])));
            }
            else if (CheckN() || CheckNr())
            {
                door = new BoxDoor(1, 3, b);
                if (door.Yes) block.SetHitBox(new Box(new vec3(0, 0, VES.Xy[13]), new vec3(1f)));
            }
            else if (CheckE() || CheckEr())
            {
                door = new BoxDoor(0, 1, b);
                if (door.Yes) block.SetHitBox(new Box(new vec3(0), new vec3(VES.Xy[3], 1f, 1f)));
            }
            else if (CheckW() || CheckWr())
            {
                door = new BoxDoor(3, 2, b);
                if (door.Yes) block.SetHitBox(new Box(new vec3(VES.Xy[13], 0, 0), new vec3(1f)));
            }

            if (door.Yes)
            {
                block.SetIsCollision(true);
            }
            else
            {
                block.SetIsCollision(false);
                block.SetHitBox(new Box(new vec3(0), new vec3(0)));
            }
        }

        /// <summary>
        /// Сгенерировать дверь
        /// </summary>
        /// <param name="addGroup">надо ли добовлять в группу, для первого раза надо</param>
        protected void Generation(bool addGroup)
        {
            vec3i pos = Position;
            vec3i max = pos + Size;

            int pr = 0;
            for (int y = pos.y; y < max.y; y++)
            {
                for (int x = pos.x; x < max.x; x++)
                {
                    for (int z = pos.z; z < max.z; z++)
                    {
                        BlockPos blockPos = new BlockPos(x, y, z);
                        if (IsLight) light.BlockModify(blockPos);
                        if (addGroup) SetGroupChunk(blockPos);
                        // Тут при !addGroup надо просто пометка обновить прорисовку блока
                        SetBlockState(blockPos, EnumBlock.Door, pr);
                        pr++;
                    }
                }
            }
        }

        /// <summary>
        /// Запрос загрузки с генерацией
        /// </summary>
        public override void RequestLoad(ChunkBase chunk)
        {
            LoadGeneration(chunk, World.GetBlock(Position));
        }

        /// <summary>
        /// Сгенерировать дверь при загрузке
        /// </summary>
        /// <param name="chunk">тикущий чанк</param>
        protected void LoadGeneration(ChunkBase chunk, BlockBase block)
        {
            vec3i pos = Position;
            vec3i bias = GroupPropertiesBias242(block.Properties);
            pos = pos - bias;

            vec3i max = pos + Size;
            int pr = 0;
            for (int y = pos.y; y < max.y; y++)
            {
                for (int x = pos.x; x < max.x; x++)
                {
                    for (int z = pos.z; z < max.z; z++)
                    {
                        BlockPos blockPos = new BlockPos(x, y, z);
                        if (x >> 4 == chunk.X && z >> 4 == chunk.Z)
                        {
                            SetGroupChunk(chunk, blockPos);
                        }
                        else
                        {
                            SetGroupChunk(blockPos);
                        }
                        // Тут надо просто пометка обновить прорисовку блока
                        SetBlockState(blockPos, EnumBlock.Door, pr);
                        pr++;
                    }
                }
            }
        }

        /// <summary>
        /// Загрузить с файла
        /// </summary>
        public override void LoadBin(ChunkBase chunk, ushort prop)
        {
            base.LoadBin(chunk, prop);
            LoadGeneration(chunk, chunk.GetBlock(Position));
        }

        /// <summary>
        /// Действие блоков, клик по модельному блоку
        /// </summary>
        public override void Action()
        {
            World.Audio.PlaySound("other." + (isOpen ? "door_close" : "door_open"),
                World.Entity.GetPositionSound(), 1f, 1f);
            isOpen = !isOpen;
            Generation(false);
            ModifiedRender();
        }

        /// <summary>
        /// Получить массив боксов от вокселя
        /// </summary>
        /// <param name="prop">параметры вокселя который прорисовываем</param>
        public override Box[] Box(int prop)
        {
            BoxDoor door = new BoxDoor();
            bool left = isLeft ? !isOpen : isOpen;

            if (CheckS()) door = new BoxDoor(2, 0, 104, 97);
            else if (CheckN()) door = new BoxDoor(1, 3, 100, 109);
            else if (CheckE()) door = new BoxDoor(0, 1, 96, 101);
            else if (CheckW()) door = new BoxDoor(3, 2, 108, 105);
            else if (CheckSr()) door = new BoxDoor(0, 2, 96, 105);
            else if (CheckWr()) door = new BoxDoor(2, 3, 104, 109);
            else if (CheckNr()) door = new BoxDoor(3, 1, 108, 101);
            else if (CheckEr()) door = new BoxDoor(1, 0, 100, 97);

            Box[] boxes = new Box[0];
            if (door.C1.Contains(prop))
            {
                boxes = left ? BoxRefrashL(door.T1 - prop * 4, true, prop > 11, prop < 4)
                    : BoxRefrashR(door.T1 - prop * 4, true, prop > 11, prop < 4);
            }
            else if (door.C2.Contains(prop))
            {
                boxes = left ? BoxRefrashL(door.T2 - prop * 4, false, prop > 11, prop < 4)
                    : BoxRefrashR(door.T2 - prop * 4, false, prop > 11, prop < 4);
            }

            if (boxes.Length > 0)
            {
                if (CheckN() || CheckNr()) Rotate(boxes, glm.pi);
                else if (CheckW() || CheckWr()) Rotate(boxes, -glm.pi90);
                else if (CheckE() || CheckEr()) Rotate(boxes, glm.pi90);
            }
            
            return boxes;
        }

        #region Box 

        /// <summary>
        /// Для левой петли
        /// </summary>
        protected Box[] BoxRefrashL(int texture, bool torec, bool up, bool down)
        {
            if (texture == -1) return new Box[0];

            Box[] boxes = new Box[up || down ? 4 : 3];

            // лицо
            boxes[0] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(0), new vec2(VES.Uv[16]), Pole.North, texture);
            // зад
            boxes[1] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(VES.Uv[16], 0), new vec2(0, VES.Uv[16]), Pole.South, texture);
            if (torec)
            {
                boxes[2] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(0), new vec2(VES.Uv[1], VES.Uv[16]), Pole.East, texture);
            }
            else
            {
                boxes[2] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(VES.Uv[15], 0), new vec2(VES.Uv[16]), Pole.West, texture);
            }
            if (up)
            {
                boxes[3] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(0), new vec2(VES.Uv[16], VES.Uv[1]), Pole.Up, texture);
            }
            if (down)
            {
                boxes[3] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(VES.Uv[16], VES.Uv[15]), new vec2(0, VES.Uv[16]), Pole.Down, texture);
            }
            return boxes;
        }

        /// <summary>
        /// Для правой петли
        /// </summary>
        protected Box[] BoxRefrashR(int texture, bool torec, bool up, bool down)
        {
            if (texture == -1) return new Box[0];

            Box[] boxes = new Box[up || down ? 4 : 3];

            boxes[0] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(VES.Uv[16], 0), new vec2(0, VES.Uv[16]), Pole.North, texture);
            boxes[1] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(0), new vec2(VES.Uv[16]), Pole.South, texture);
            if (torec)
            {
                boxes[2] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(0), new vec2(VES.Uv[1], VES.Uv[16]), Pole.West, texture);
            }
            else
            {
                boxes[2] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(VES.Uv[15], 0), new vec2(VES.Uv[16]), Pole.East, texture);
            }
            if (up)
            {
                boxes[3] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(VES.Uv[16], 0), new vec2(0, VES.Uv[1]), Pole.Up, texture);
            }
            if (down)
            {
                boxes[3] = new Box(new vec3(0), new vec3(1f, 1f, VES.Xy[3]),
                new vec2(0, VES.Uv[15]), new vec2(VES.Uv[16]), Pole.Down, texture);
            }
            return boxes;
        }

        #endregion

        protected void Rotate(Box[] boxes, float yaw)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].RotateYaw = yaw;
            }
        }

        public bool CheckS() => Check(Pole.South, false, true) || Check(Pole.West, true, false);
        public bool CheckN() => Check(Pole.North, false, true) || Check(Pole.East, true, false);
        public bool CheckW() => Check(Pole.West, false, true) || Check(Pole.North, true, false);
        public bool CheckE() => Check(Pole.East, false, true) || Check(Pole.South, true, false);
        public bool CheckSr() => Check(Pole.South, false, false) || Check(Pole.East, true, true);
        public bool CheckNr() => Check(Pole.North, false, false) || Check(Pole.West, true, true);
        public bool CheckWr() => Check(Pole.West, false, false) || Check(Pole.South, true, true);
        public bool CheckEr() => Check(Pole.East, false, false) || Check(Pole.North, true, true);

        public bool Check(Pole s, bool open, bool left)
        {
            return s == side && isOpen == open && isLeft == left;
        }

        public bool Check0(Pole s, bool left)
        {
            return s == side && isLeft == left;
        }

        /// <summary>
        /// Получить смещение групповой модели 2*4*2
        /// </summary>
        protected vec3i GroupPropertiesBias242(byte prop)
        {
            return new vec3i((prop & 2) >> 1, prop >> 2, prop & 1);
        }

        /// <summary>
        /// Вспомогательный объект для определения коробок двери
        /// </summary>
        protected class BoxDoor
        {
            public List<int> C1 { get; protected set; } = new List<int>();
            public List<int> C2 { get; protected set; } = new List<int>();
            public int T1 { get; protected set; } = 0;
            public int T2 { get; protected set; } = 0;
            public bool Yes { get; protected set; } = false;

            public BoxDoor() { }
            public BoxDoor(int c1, int c2, int t1, int t2) 
            {
                for (int i = 0; i < 4; i++)
                {
                    C1.Add(c1 + i * 4);
                    C2.Add(c2 + i * 4);
                }
                T1 = t1;
                T2 = t2;
            }
            public BoxDoor(int c1, int c2, int b)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (c1 + i * 4 == b || c2 + i * 4 == b)
                    {
                        Yes = true;
                        return;
                    }
                }
            }
        }
    }
}
