using VoxelEngine.Util;

namespace VoxelEngine.World
{
    /// <summary>
    /// Перечень блоков
    /// </summary>
    public class Blocks
    {
        public static Block GetBlock(Voxel voxel)
        {
            if (voxel.IsEmpty) return new Block();
            Block block = new Block();
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
            }
            
            // тут нада id voxel-я, так-как не успевает замениться тип блока, нет света корректного
            //voxel.SetBlockLightOpacity(GetBlockLightOpacity(id));// block.EBlock));
            //block.LightBlock = (float)voxel.GetLightFor(EnumSkyBlock.Sky) / 15f;
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
                case EnumBlock.Cactus: return 3;
                case EnumBlock.Water: return 3;
                case EnumBlock.WaterFlowing: return 3;
                case EnumBlock.Leaves: return 3;
                case EnumBlock.Sapling: return 0;
                case EnumBlock.LeavesApple: return 4;
            }
            return 15;
        }

        public static Block GetBlock(Voxel voxel, BlockPos pos)
        {
            Block block = GetBlock(voxel);
            block.SetPosition(pos);
            return block;
        }

        public static Block GetBlock(EnumBlock eBlock, BlockPos pos)
        {
            Voxel voxel = new Voxel();
            voxel.SetEBlock(eBlock);
            Block block = GetBlock(voxel);
            block.SetPosition(pos);
            return block;
        }

        /// <summary>
        /// Получить блок воздуха
        /// </summary>
        /// <param name="pos"></param>
        public static Block GetAir(BlockPos pos)
        {
            Block block = new BlockAir();
            block.SetPosition(pos);
            return block;
        }
    }
}
