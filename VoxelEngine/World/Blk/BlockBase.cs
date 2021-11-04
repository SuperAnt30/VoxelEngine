using System;
using VoxelEngine.Gen.Group;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.Vxl;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World.Blk
{
    /// <summary>
    /// Объект Блока
    /// </summary>
    public class BlockBase
    {
        /// <summary>
        /// Коробки
        /// </summary>
        public Box[] Boxes { get; protected set; } = new Box[] { new Box() };
        /// <summary>
        /// Хит бокс блока
        /// </summary>
        public Box HitBox { get; protected set; } = new Box();
        /// <summary>
        /// Вся ли прорисовка, аналог кактус, забор...
        /// </summary>
        public bool AllDrawing { get; protected set; } = false;
        /// <summary>
        /// Альфа блок, вода, стекло...
        /// </summary>
        public bool IsAlphe { get; protected set; } = false;
        /// <summary>
        /// Есть ли столкновение
        /// </summary>
        public bool IsCollision { get; protected set; } = true;
        /// <summary>
        /// Можно ли выбирать блок
        /// </summary>
        protected bool IsAction { get; set; } = true;
        /// <summary>
        /// Вода ли это
        /// </summary>
        public bool IsWater { get; protected set; } = false;
        /// <summary>
        /// Листва ли это
        /// </summary>
        public bool IsLeaves { get; protected set; } = false;
        /// <summary>
        /// Трава ли это
        /// </summary>
        public bool IsGrass { get; protected set; } = false;
        /// <summary>
        /// Только на траве
        /// </summary>
        public bool IsOnGrass { get; protected set; } = false;
        /// <summary>
        /// Является ли объект кубом
        /// </summary>
        public bool IsCube { get; protected set; } = true;
        /// <summary>
        /// Явлыется ли блок небом
        /// </summary>
        public bool IsAir { get { return EBlock == EnumBlock.Air; } }
        /// <summary>
        /// Освещение от себя, типа травы
        /// </summary>
        public bool LightingYourself { get; protected set; } = false;
        /// <summary>
        /// Получить тип блока
        /// </summary>
        public EnumBlock EBlock { get; protected set; }
        /// <summary>
        /// Количество излучаемого света (плафон)
        /// </summary>
        public int LightValue { get; protected set; } = 0;
        /// <summary>
        /// Дополнительный параметр блока 4 бита
        /// </summary>
        public byte Properties { get; protected set; } = 0;
        /// <summary>
        /// Относится ли блок к групповой модели
        /// </summary>
        public bool IsGroupModel { get; protected set; } = false;
        /// <summary>
        /// Объект группы блоков
        /// </summary>
        public GroupBase Group { get; protected set; }

        /// <summary>
        /// Звук сломанного блока
        /// </summary>
        protected string soundBreak = "dig.stone";
        /// <summary>
        /// Количество разных звуков сломанного блока
        /// </summary>
        protected int soundBreakCount = 4;
        /// <summary>
        /// Звук поставленного блока
        /// </summary>
        protected string soundPut = "dig.stone";
        /// <summary>
        /// Количество разных звуков поставленного блока
        /// </summary>
        protected int soundPutCount = 4;
        /// <summary>
        /// Звук ходьбы по блоку
        /// </summary>
        protected string soundStep = "step.stone";
        /// <summary>
        /// Количество разных звуков ходьбы по блок
        /// </summary>
        protected int soundStepCount = 4;

        /// <summary>
        /// Строка
        /// </summary>
        public override string ToString()
        {
            return EBlock.ToString() + " " + Position.ToString();
        }

        public BlockBase() { }

        public Voxel Voxel { get; protected set; }

        public void SetVoxel(Voxel voxel)
        {
            Voxel = voxel;
            EBlock = voxel.GetEBlock();
            SetProperties(voxel.GetParam4bit());
        }

        /// <summary>
        /// Задать дополнительный параметр
        /// </summary>
        public void SetProperties(byte properties)
        {
            Properties = properties;
            if (IsGroupModel && Group != null)
            {
                Group.CollisionRefrash(this);
            }
        }

        /// <summary>
        /// Обновить Коробку
        /// </summary>
        public virtual void BoxRefrash() { }

        /// <summary>
        /// Позиция блока в мире
        /// </summary>
        public BlockPos Position { get; protected set; } = new BlockPos();
        /// <summary>
        /// Задать позицию блока
        /// </summary>
        public void SetPosition(BlockPos pos)
        {
            Position = pos;
        }

        /// <summary>
        /// Сколько света вычитается для прохождения света Air = 0
        /// </summary>
        public byte GetBlockLightOpacity()
        {
            return Blocks.GetBlockLightOpacity(EBlock);
        }

        /// <summary>
        /// Звук сломанного блока
        /// </summary>
        public string SoundBreak() => Sound(soundBreakCount, soundBreak);
        /// <summary>
        /// Звук поставленного блока
        /// </summary>
        public string SoundPut() => Sound(soundPutCount, soundPut);
        /// <summary>
        /// Звук поставленного блока
        /// </summary>
        public string SoundStep() => Sound(soundStepCount, soundStep);

        protected string Sound(int count, string sound)
        {
            if (count > 0)
            {
                Random random = new Random();
                return sound + (random.Next(count) + 1);
            }
            return sound;
        }

        /// <summary>
        /// Проверить колизию блока на пересечение луча
        /// </summary>
        /// <param name="pos">точка от куда идёт лучь</param>
        /// <param name="dir">вектор луча</param>
        /// <param name="maxDist">максимальная дистания</param>
        public bool CollisionRayTrace(vec3 pos, vec3 dir, float maxDist)
        {
            if (IsAction)
            {
                if (HitBox.IsHitBoxAll) return true;

                // Если блок не полный, обрабатываем хитбокс блока
                RayCross ray = new RayCross(pos, dir, maxDist);
                vec3 bpos = Position.ToVec3();
                return ray.CrossLineToRectangle(HitBox.From + bpos, HitBox.To + bpos);
            }
            return false;
        }

        /// <summary>
        /// Изменить колизию в блоке
        /// </summary>
        public void SetIsCollision(bool isCollision) => IsCollision = isCollision;

        /// <summary>
        /// Изменить хит бокс блока
        /// </summary>
        public void SetHitBox(Box hitBox) => HitBox = hitBox;

        /// <summary>
        /// Задать группу блоков
        /// </summary>
        public void SetGroup(GroupBase group) => Group = group;
    }
}
