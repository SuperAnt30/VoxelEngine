﻿using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    /// <summary>
    /// Блок камня
    /// </summary>
    public class BlockStone : BlockBase
    {
        /// <summary>
        /// Блок камня
        /// </summary>
        public BlockStone()
        {
            Boxes = new Box[] { new Box(0) };
        }
    }
}
