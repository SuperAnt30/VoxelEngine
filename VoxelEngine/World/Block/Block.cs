﻿using VoxelEngine.Glm;
using System.Collections.Generic;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Объект Блока
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Коробки
        /// </summary>
        public Box[] Boxes { get; protected set; } = new Box[] { new Box() };
        /// <summary>
        /// Проходит ли свет, прозрачность, 0 не прозрачный, 15 прозрачный полностью
        /// </summary>
        //public int LightOpacity { get; protected set; } = 0;
        /// <summary>
        /// Вся ли прорисовка, аналог кактус, забор...
        /// </summary>
        public bool AllDrawing { get; protected set; } = false;
        /// <summary>
        /// Альфа блок, вода, стекло...
        /// </summary>
        public bool IsAlphe { get; protected set; } = false;
        /// <summary>
        /// Цвет блока
        /// </summary>
        public vec4 Color { get; protected set; } = new vec4(1f, 1f, 1f, 1f);
        
        /// <summary>
        /// Индекс блока
        /// </summary>
        public byte Id { get; protected set; } = 0;

        /// <summary>
        /// Получить тип блока
        /// </summary>
        public EnumBlock EBlock { get { return (EnumBlock)Id; } }

        /// <summary>
        /// Количество излучаемого света (плафон)
        /// </summary>
        public byte LightValue { get; protected set; } = 0;

        /// <summary>
        /// Дополнительный параметр блока 4 бита
        /// </summary>
        public byte Properties { get; set; } = 0;

        /// <summary>
        /// Строка
        /// </summary>
        public override string ToString()
        {
            return base.ToString();
        }

        public Block() { }

        public Voxel Voxel { get; protected set; }

        public void SetVoxel(Voxel voxel)
        {
            Voxel = voxel;
            Id = voxel.GetId();
            Properties = voxel.GetParam4bit();
            //LightBlock = voxel..B1;
        }

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
        /// Второй вариант для прорисовки
        /// </summary>
        public virtual void BoxesTwo() { }

        /// <summary>
        /// Сколько света вычитается для прохождения этого блока
        /// </summary>
        public byte GetBlockLightOpacity()
        {
            return Blocks.GetBlockLightOpacity(Id);
        }



    }
}