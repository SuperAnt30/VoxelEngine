using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Перечень блоков
    /// </summary>
    public class Blocks
    {
        public static Block GetBlock(Voxel voxel)
        {
            if (voxel.IsEmpty()) return new Block();
            Block block = new Block();
            byte id = voxel.GetId();
            switch (id)
            {
                case 0: block = new BlockAir(); break;
                case 1: block = new BlockStone(); break;
                case 2: block = new BlockDirt(); break;
                case 3: block = new BlockGrass(); break;            
                case 4: block = new BlockSand(); break;
                case 5: block = new BlockPlanks(); break;
                case 6: block = new BlockTileGray(); break;
                case 7: block = new BlockTileDark(); break;
                case 8: block = new BlockTileBrown(); break;
                case 9: block = new BlockGlass(); break;
                case 10: block = new BlockCactus(); break;
                case 12: block = new BlockBrol(); break;

                case 11: block = new BlockWater(); break;
                case 13: block = new BlockWaterFlowing(); break;

                case 14: block = new BlockOak(); break;
                case 15: block = new BlockLeaves(); break;
                case 16: block = new BlockSapling(); break;
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
        public static bool IsNotTransparent(byte id)
        {
            return GetBlockLightOpacity(id) > 13;
        }

        /// <summary>
        /// Блок воды ли
        /// </summary>
        public static bool IsWater(byte id)
        {
            return id == (int)EnumBlock.Water || id == (int)EnumBlock.WaterFlowing;
        }
        /// <summary>
        /// Сколько света вычитается для прохождения этого блока
        /// </summary>
        public static byte GetBlockLightOpacity(byte id)
        {
            return GetBlockLightOpacity((EnumBlock)id);
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
            }
            return 15;
        }

        public static Block GetBlock(Voxel voxel, BlockPos pos)
        {
            Block block = GetBlock(voxel);
            block.SetPosition(pos);
            return block;
        }

        public static Block GetBlock(byte id, BlockPos pos)
        {
            Voxel voxel = new Voxel();
            voxel.SetIdByte(id);
            Block block = GetBlock(voxel);
            block.SetPosition(pos);
            return block;
        }

        public static Block GetBlock(EnumBlock eBlock, BlockPos pos)
        {
            Voxel voxel = new Voxel();
            voxel.SetIdByte((byte)eBlock);
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
