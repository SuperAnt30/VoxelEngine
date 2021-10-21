using VoxelEngine.Gen.Group;
using VoxelEngine.Util;
using VoxelEngine.Vxl;

namespace VoxelEngine.World.Blk
{
    /// <summary>
    /// Перечень блоков
    /// </summary>
    public class Blocks
    {
        /// <summary>
        /// Количество блоков
        /// </summary>
        public const int BLOCKS_COUNT = 24;

        protected static BlockBase GetBlock(Voxel voxel, GroupBase group)
        {
            if (voxel.IsEmpty) return new BlockBase();
            BlockBase block = new BlockBase();
            EnumBlock eBlock = voxel.GetEBlock();
            switch (eBlock)
            {
                case EnumBlock.Air: block = new BlockAir(); break;
                case EnumBlock.Stone: block = new BlockStone(); break;
                case EnumBlock.Dirt: block = new BlockDirt(); break;
                case EnumBlock.Grass: block = new BlockGrass(); break;            
                case EnumBlock.Sand: block = new BlockSand(); break;
                case EnumBlock.Planks: block = new BlockPlanks(); break;
                case EnumBlock.TileGray: block = new BlockTileGray(); break;
                case EnumBlock.TileDark: block = new BlockTileDark(); break;
                case EnumBlock.TileBrown: block = new BlockTileBrown(); break;
                case EnumBlock.Glass: block = new BlockGlass(); break;
                case EnumBlock.Cactus: block = new BlockCactus(); break;
                case EnumBlock.Brol: block = new BlockBrol(); break;

                case EnumBlock.Water: block = new BlockWater(); break;
                case EnumBlock.WaterFlowing: block = new BlockWaterFlowing(); break;

                case EnumBlock.Log: block = new BlockLog(); break;
                case EnumBlock.Leaves: block = new BlockLeaves(); break;
                case EnumBlock.Sapling: block = new BlockSapling(); break;
                case EnumBlock.LeavesApple: block = new BlockLeavesApple(); break;

                case EnumBlock.Diorite: block = new BlockDiorite(); break;
                case EnumBlock.Bedrock: block = new BlockBedrock(); break;
                case EnumBlock.TallGrass: block = new BlockTallGrass(); break;
                case EnumBlock.Poppy: block = new BlockPoppy(); break;
                case EnumBlock.Dandelion: block = new BlockDandelion(); break;

                case EnumBlock.Torch: block = new BlockTorch(); break;
                case EnumBlock.Door: block = new BlockDoor(); break;

            }

            // тут нада id voxel-я, так-как не успевает замениться тип блока, нет света корректного
            //voxel.SetBlockLightOpacity(GetBlockLightOpacity(id));// block.EBlock));
            //block.LightBlock = (float)voxel.GetLightFor(EnumSkyBlock.Sky) / 15f;
            if (group != null)
            {
                block.SetGroup(group);
            }
            block.SetVoxel(voxel);
            return block;
        }
        /// <summary>
        /// Блок не прозрачный
        /// </summary>
        public static bool IsNotTransparent(EnumBlock eBlock)
        {
            return GetBlockLightOpacity(eBlock) > 13;
        }

        /// <summary>
        /// Блок воды ли
        /// </summary>
        public static bool IsWater(EnumBlock eBlock)
        {
            return eBlock == EnumBlock.Water || eBlock == EnumBlock.WaterFlowing;
        }
        /// <summary>
        /// Сколько света вычитается для прохождения этого блока
        /// </summary>
        public static byte GetBlockLightOpacity(EnumBlock eblock)
        {
            switch (eblock)
            {
                case EnumBlock.Air: return 0;
                case EnumBlock.Glass: return 2;
                case EnumBlock.Cactus: return 0;
                case EnumBlock.Water: return 2;
                case EnumBlock.WaterFlowing: return 2;
                case EnumBlock.Leaves: return 3;
                case EnumBlock.Sapling: return 0;
                case EnumBlock.LeavesApple: return 4;
                case EnumBlock.TallGrass: return 0;
                case EnumBlock.Poppy: return 0;
                case EnumBlock.Dandelion: return 0;
                case EnumBlock.Torch: return 2;
                case EnumBlock.Door: return 1; // 1 вместо 0, чтоб при удалении двери корректно обновлялись блоки под дверью
            }
            return 15;
        }

        public static BlockBase GetBlock(Voxel voxel, BlockPos pos, GroupBase group)
        {
            BlockBase block = GetBlock(voxel, group);
            block.SetPosition(pos);
            return block;
        }

        public static BlockBase GetBlock(EnumBlock eBlock, BlockPos pos)
        {
            Voxel voxel = new Voxel();
            voxel.SetEBlock(eBlock);
            return GetBlock(voxel, pos, null);

            //Voxel voxel = new Voxel();
            //voxel.SetEBlock(eBlock);
            //BlockBase block = GetBlock(voxel);
            //block.SetPosition(pos);
            //return block;
        }

        /// <summary>
        /// Получить блок воздуха
        /// </summary>
        /// <param name="pos"></param>
        public static BlockBase GetAir(BlockPos pos)
        {
            BlockBase block = new BlockAir();
            block.SetPosition(pos);
            return block;
        }

        /// <summary>
        /// Получить базовый блок, как пустышку
        /// </summary>
        public static BlockBase GetEmpty(BlockPos pos)
        {
            BlockBase block = new BlockBase();
            block.SetPosition(pos);
            return block;
        }
    }
}
