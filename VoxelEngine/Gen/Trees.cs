using System;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.Gen
{
    /// <summary>
    /// Генерация дерева
    /// </summary>
    public class Trees
    {
        /// <summary>
        /// Объект мира которы берёт из объекта ThreadWorld
        /// </summary>
        public WorldRender World { get; protected set; }

        public Trees(WorldRender world)
        {
            World = world;
        }

        public bool Generate(BlockPos pos)
        {
            return Generate(new Random(), pos);
        }

        protected int[,] ar1 = new int[1, 1] { { 2 } };
        protected int[,] ar3 = new int[3, 3] { { 3, 2, 3 }, { 2, 1, 2 }, { 3, 2, 3 } };
        protected int[,] ar5 = new int[5, 5] {
            { 0, 3, 2, 3, 0 },
            { 3, 1, 1, 1, 3 },
            { 2, 1, 1, 1, 2 },
            { 3, 1, 1, 1, 3 },
            { 0, 3, 2, 3, 0 } };
        protected int[,] ar7 = new int[7, 7] {
            { 0, 0, 3, 2, 3, 0, 0 },
            { 0, 3, 1, 1, 1, 3, 0 },
            { 3, 1, 5, 4, 5, 1, 3 },
            { 2, 1, 4, 1, 4, 1, 2 },
            { 3, 1, 5, 4, 5, 1, 3 },
            { 0, 3, 1, 1, 1, 3, 0 },
            { 0, 0, 3, 2, 3, 0, 0 } };
        protected int[,] ar9 = new int[9, 9] {
            { 0, 0, 3, 2, 2, 2, 3, 0, 0 },
            { 0, 3, 1, 1, 1, 1, 1, 3, 0 },
            { 3, 1, 1, 5, 5, 5, 1, 1, 3 },
            { 2, 1, 5, 5, 4, 5, 5, 1, 2 },
            { 2, 1, 5, 4, 1, 4, 5, 1, 2 },
            { 2, 1, 5, 5, 4, 5, 5, 1, 2 },
            { 3, 1, 1, 5, 5, 5, 1, 1, 3 },
            { 0, 3, 1, 1, 1, 1, 1, 3, 0 },
            { 0, 0, 3, 2, 2, 2, 3, 0, 0 } };

        /// <summary>
        /// высота ствола (5-8)
        /// </summary>
        protected int h = 5;
        /// <summary>
        /// ширина листвы от ствола (2-4)
        /// </summary>
        protected int wl = 2;
        /// <summary>
        /// высота ствола до листвы (2-4)
        /// </summary>
        protected int hl = 2;
        /// <summary>
        /// высота дерева (8-12)
        /// </summary>
        protected int ht = 8;

        /// <summary>
        /// Сгенерировать дерево
        /// </summary>
        /// <param name="random"></param>
        /// <param name="pos">позиция саженца</param>
        /// <returns>true - сгенерировали, false - нет</returns>
        public bool Generate(Random random, BlockPos pos)
        {
            h += random.Next(4);
            wl += random.Next(3);
            hl += random.Next(3);
            ht = (int)((float)h * 1.6f);

            if (pos.Y < 1 || pos.Y > 200) return false;

            Layer layer;

            // Проверка для дерева
            for (int y = hl; y <= ht + 1; y++)
            {
                if (y < 0 ||y > 200) return false;

                layer = GetLayer(y);
                for (int x = -layer.Size; x <= layer.Size; x++)
                {
                    for (int z = -layer.Size; z <= layer.Size; ++z)
                    {
                        if (layer.Ar[x + layer.Size, z + layer.Size] > 0)
                        {
                            if (!CheckBlock(World.GetBlock(new BlockPos(pos.X + x, pos.Y + y, pos.Z + z))))
                                return false;
                        }
                    }
                }
            }

            Block block = World.GetBlock(pos.OffsetDown());
            // проверка что саженей на земле и высота позволяет
            if ((block.EBlock == EnumBlock.Grass || block.EBlock == EnumBlock.Dirt) && pos.Y < 200)
            {
                // Меняем основание на землю
                SetDirt(pos.OffsetDown());
                        

                // листва
                for (int y = hl; y <= ht + 1; y++)
                {
                    layer = GetLayer(y);

                    for (int x = -layer.Size; x <= layer.Size; x++)
                    {
                        for (int z = -layer.Size; z <= layer.Size; ++z)
                        {
                            int a = layer.Ar[x + layer.Size, z + layer.Size];
                                        
                            if (a == 1 || (a == 2 && random.Next(2) == 0) || (a == 3 && random.Next(4) == 0) || a == 4 || a == 5)
                            {
                                Block block2 = World.GetBlock(new vec3i(pos.X + x, pos.Y + y, pos.Z + z));
                                EnumBlock eblock = EnumBlock.Leaves;
                                // ветки
                                if ((a == 4 && random.Next(10) == 0) // 10%
                                    || (a == 5 && random.Next(50) == 0)) // 2%
                                    eblock = EnumBlock.Log;
                                // яблоко
                                if ((a == 2 || a == 3) && random.Next(20) == 0) // 5%
                                    eblock = EnumBlock.LeavesApple;

                                if (block2.EBlock == EnumBlock.Air || block2.EBlock == EnumBlock.Leaves || block2.EBlock == EnumBlock.LeavesApple)
                                {
                                    SetBlock(block2.Position, eblock, metaLeaves);
                                }
                            }
                        }
                    }
                }

                // ствол
                for (int y = 0; y < h; y++)
                {
                    Block var21 = World.GetBlock(pos.OffsetUp(y));

                    if (var21.EBlock == EnumBlock.Air || var21.EBlock == EnumBlock.Leaves || var21.EBlock == EnumBlock.Sapling)
                    {
                        SetBlock(pos.OffsetUp(y), EnumBlock.Log, metaWood);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Получить объект слоя от уровня дерева
        /// </summary>
        protected Layer GetLayer(int y)
        {
            if (y == ht + 1) return new Layer(0, ar1);
            if (y == hl || y == ht - 1) return new Layer(2, ar5);
            if (y == ht) return new Layer(1, ar3);
            if (wl >= 3 && y == ht - 2) return new Layer(3, ar7);
            return new Layer(4, ar9);
        }

        /// <summary>
        /// Объект слоя
        /// </summary>
        protected class Layer
        {
            /// <summary>
            /// Размер от ствола до края листвы
            /// </summary>
            public int Size { get; protected set; }
            /// <summary>
            /// Массив слоя
            /// </summary>
            public int[,] Ar { get; protected set; }

            public Layer(int s, int[,] ar)
            {
                Size = s;
                Ar = ar;
            }
        }

        /// <summary>
        /// Минимальная высота сгенерированного дерева
        /// </summary>
        private int minTreeHeight = 4;

        /// <summary>
        /// Верно, если на этом дереве должно расти лоза
        /// </summary>
        //private bool vinesGrow;

        /// <summary>
        /// Значение метаданных древесины для использования при генерации деревьев.
        /// </summary>
        private int metaWood = 0;

        /// <summary>
        /// Значение метаданных листьев для использования при создании дерева.
        /// </summary>
        private int metaLeaves = 0;

        public bool GenerateMinecraft(Random random, BlockPos pos)
        {
            // высота
            int h = random.Next(3) + minTreeHeight;
            bool var5 = true;

            if (pos.Y >= 1 && pos.Y + h + 1 <= 256)
            {
                byte var7;

                // Проверка для дерева
                for (int y = pos.Y; y <= pos.Y + 1 + h; ++y)
                {
                    var7 = 1;

                    if (y == pos.Y)
                    {
                        var7 = 0;
                    }

                    if (y >= pos.Y + 1 + h - 2)
                    {
                        var7 = 2;
                    }

                    for (int x = pos.X - var7; x <= pos.X + var7 && var5; x++)
                    {
                        for (int z = pos.Z - var7; z <= pos.Z + var7 && var5; z++)
                        {
                            if (y >= 0 && y < 256)
                            {
                                if (!CheckBlock(World.GetBlock(new BlockPos(x, y, z))))
                                {
                                    var5 = false;
                                }
                            }
                            else
                            {
                                var5 = false;
                            }
                        }
                    }
                }

                if (var5)
                {
                    Block var19 = World.GetBlock(pos.OffsetDown());

                    if ((var19.EBlock == EnumBlock.Grass || var19.EBlock == EnumBlock.Dirt) && pos.Y < 256 - h - 1)
                    {
                        SetDirt(pos.OffsetDown());
                        var7 = 3;
                        byte var20 = 0;
                        int yh;
                        int w;
                        int var13;

                        // листва
                        for (int y = pos.Y - var7 + h; y <= pos.Y + h; y++)
                        {
                            yh = y - (pos.Y + h);
                            w = var20 + 1 - yh / 2;

                            for (int x = pos.X - w; x <= pos.X + w; x++)
                            {
                                var13 = x - pos.X;

                                for (int z = pos.Z - w; z <= pos.Z + w; ++z)
                                {
                                    int var15 = z - pos.Z;

                                    if (Mth.Abs(var13) != w || Mth.Abs(var15) != w || random.Next(2) != 0 && yh != 0)
                                    {
                                        Block block = World.GetBlock(new vec3i(x, y, z));

                                        if (block.EBlock == EnumBlock.Air || block.EBlock == EnumBlock.Leaves)
                                        {
                                            SetBlock(block.Position, EnumBlock.Leaves, metaLeaves);
                                        }
                                    }
                                }
                            }
                        }

                        // ствол
                        for (int y = 0; y < h; y++)
                        {
                            Block var21 = World.GetBlock(pos.OffsetUp(y));

                            if (var21.EBlock == EnumBlock.Air || var21.EBlock == EnumBlock.Leaves || var21.EBlock == EnumBlock.Sapling)
                            {
                                SetBlock(pos.OffsetUp(y), EnumBlock.Log, metaWood);
                            }
                        }

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверка на конкретные блоки где может сгенерироваться дерево
        /// </summary>
        protected bool CheckBlock(Block block)
        {
            return block.EBlock == EnumBlock.Air || block.EBlock == EnumBlock.Leaves 
                || block.EBlock == EnumBlock.LeavesApple || block.EBlock == EnumBlock.Grass 
                || block.EBlock == EnumBlock.Dirt || block.EBlock == EnumBlock.Sapling;
        }

        /// <summary>
        /// Заменить блок под деревом на землю
        /// </summary>
        protected void SetDirt(BlockPos pos)
        {
            if (World.GetBlock(pos).EBlock != EnumBlock.Dirt)
            {
                SetBlock(pos, EnumBlock.Dirt);
            }
        }

        /// <summary>
        /// Задать блок
        /// </summary>
        protected void SetBlock(BlockPos pos, EnumBlock eBlock)
        {
            Block blockNew = Blocks.GetBlock(eBlock, pos);
            World.SetBlockState(blockNew, false);
        }

        /// <summary>
        /// Задать блок
        /// </summary>
        protected void SetBlock(BlockPos pos, EnumBlock eBlock, int properties)
        {
            Block blockNew = Blocks.GetBlock(eBlock, pos);
            blockNew.Properties = (byte)properties;
            World.SetBlockState(blockNew, false);
        }

        /// <summary>
        /// Проверим есть ли рядом блок древесины
        /// </summary>
        public bool IsWood(BlockPos pos)
        {
            int r = VE.SIZE_LEAVES;
            for (int y = pos.Y - r; y <= pos.Y + r; y++)
            {
                for (int x = pos.X - r; x <= pos.X + r; x++)
                {
                    for (int z = pos.Z - r; z <= pos.Z + r; ++z)
                    {
                        Block block = World.GetBlock(new vec3i(x, y, z));
                        if (block.EBlock == EnumBlock.Log)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
             
    }
}
