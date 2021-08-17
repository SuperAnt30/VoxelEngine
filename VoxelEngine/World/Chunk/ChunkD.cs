using System;
using System.Collections;
using System.Collections.Generic;
using VoxelEngine.Gen;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.World.Chunk
{
    public class ChunkD : Coords
    {
        /// <summary>
        /// Данные чанка
        /// </summary>
        public ChunkStorage[] StorageArrays { get; protected set; } = new ChunkStorage[16];
        /// <summary>
        /// Пометка для перенести данные для записи
        /// </summary>
        protected bool isModified = false;
        /// <summary>
        /// Сылка на объект мира
        /// </summary>
        public WorldD World { get; protected set; }
        /// <summary>
        /// Загружен ли чанк
        /// </summary>
        public bool IsChunkLoaded { get; protected set; } = false;

        /// <summary>
        /// Прячем при необходимости чанк рендера
        /// </summary>
        public ChunkRender ChunkTag { get; set; }

        protected ChunkD() { }
        public ChunkD(WorldD worldIn, int x, int z)
        {
            Initialize(x, z);
            World = worldIn;
        }

        protected void Initialize(int x, int z)
        {
            X = x;
            Z = z;
            for (int i = 0; i < StorageArrays.Length; i++) StorageArrays[i] = new ChunkStorage();
        }

        #region Block Storage

        /// <summary>
        /// Получить блок
        /// </summary>
        public Block GetBlock(BlockPos pos)
        {
            return GetBlock(pos.ToVec3i());
        }

        /// <summary>
        /// Получить блок по глобальным координатам
        /// </summary>
        public Block GetBlock(vec3i pos)
        {
            if (pos.x >> 4 == X && pos.z >> 4 == Z)
            {
                return Blocks.GetBlock(GetVoxel(pos.x & 15, pos.y, pos.z & 15), new BlockPos(pos));
            }
            return World.GetBlock(pos);
        }

        /// <summary>
        /// Получить блок по координатам чанка
        /// </summary>
        public Block GetBlock0(vec3i pos)
        {
            if (pos.x >> 4 == 0 && pos.z >> 4 == 0)
            {
                return Blocks.GetBlock(GetVoxel(pos), new BlockPos(X << 4 | pos.x, pos.y, Z << 4 | pos.z));
            }
            return Blocks.GetAir(new BlockPos(X << 4 | pos.x, pos.y, Z << 4 | pos.z));
        }
        /// <summary>
        /// Получить значение вокселя по координатам чанка
        /// </summary>
        public Voxel GetVoxel(vec3i pos)
        {
            return GetVoxel(pos.x, pos.y, pos.z);
        }
        /// <summary>
        /// Получить значение вокселя по координатам чанка
        /// </summary>
        public Voxel GetVoxel(int x, int y, int z)
        {
            if (y >= 0 && y >> 4 < StorageArrays.Length)
            {
                ChunkStorage storage = StorageArrays[y >> 4];
                return storage.GetVoxel(x, y & 15, z);
            }
            return new Voxel();
        }

        /// <summary>
        /// Вернуть тип блока
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public EnumBlock GetBlockState(BlockPos pos)
        {
            return (EnumBlock)GetVoxel(pos.ToVec3i()).GetId();
        }

        /// <summary>
        /// Задать воксель
        /// </summary>
        public void SetVoxelId(int x, int y, int z, byte id)
        {
            if (y >= 0 && y >> 4 < StorageArrays.Length)
            {
                ChunkStorage storage = StorageArrays[y >> 4];
                storage.SetVoxelId(x, y & 15, z, id);
            }
        }

        /// <summary>
        /// Задать дополнительный параметр блока в 4 бита
        /// </summary>
        public void SetParam4bit(int x, int y, int z, byte param)
        {
            if (y >= 0 && y >> 4 < StorageArrays.Length)
            {
                ChunkStorage storage = StorageArrays[y >> 4];
                storage.SetParam4bit(x, y & 15, z, param);
            }
            //Voxels[y, x, z].SetParam4bit(param);
        }

        /// <summary>
        /// Задать дополнительный параметр блока в 4 бита по глобальным координатам
        /// </summary>
        public void SetParam4bit(vec3i pos, byte param)
        {
            if (pos.x >> 4 == X && pos.z >> 4 == Z)
            {
                SetParam4bit(pos.x & 15, pos.y, pos.z & 15, param);
            }
        }

        /// <summary>
        /// Задать яркость блока от неба
        /// </summary>
        public void SetLightFor(int x, int y, int z, EnumSkyBlock type, byte light)
        {
            //Voxels[y, x, z].SetLightFor(type, light);
            if (y >= 0 && y >> 4 < StorageArrays.Length)
            {
                ChunkStorage storage = StorageArrays[y >> 4];
                storage.SetLightFor(x, y & 15, z, type, light);
            }
            SetChunkModified();
        }

        /// <summary>
        /// Задать яркость блока от неба
        /// </summary>
        public void SetLightsFor(int x, int y, int z, byte light)
        {
            //Voxels[y, x, z].SetLightsFor(light);
            if (y >= 0 && y >> 4 < StorageArrays.Length)
            {
                ChunkStorage storage = StorageArrays[y >> 4];
                storage.SetLightsFor(x, y & 15, z, light);
            }
            SetChunkModified();
        }

        /// <summary>
        /// получить яркость блока от неба
        /// </summary>
        public byte GetLightFor(int x, int y, int z, EnumSkyBlock type)
        {
            //return Voxels[y, x, z].GetLightFor(type);
            if (y >= 0 && y >> 4 < StorageArrays.Length)
            {
                ChunkStorage storage = StorageArrays[y >> 4];
                return storage.GetLightFor(x, y & 15, z, type);
            }
            return (byte)type;
        }

        #endregion

        /// <summary>
        /// Загрузить или сгенерировать данные чанка
        /// </summary>
        public bool LoadinData()
        {
            RegionFile region = World.RegionPr.GetRegion(X, Z);
            if (region == null) return false;
            byte[] b = region.GetChunk(X, Z);
            if (b != null)
            {
                Read(b);
                GenerateHeightMap();
            }
            else
            {
                // Если его нет, то генерируем
                Generation();
                GenerateSkylightMap();
                // и сразу же записываем
                region.SetChunk(X, Z, Write());
            }
            return true;
        }

        /// <summary>
        /// Загружен чанк
        /// </summary>
        public void OnChunkLoad()
        {
            IsChunkLoaded = LoadinData();
            

            //this.worldObj.addTileEntities(this.chunkTileEntityMap.values());

            //for (int var1 = 0; var1 < this.entityLists.length; ++var1)
            //{
            //    Iterator var2 = this.entityLists[var1].iterator();

            //    while (var2.hasNext())
            //    {
            //        Entity var3 = (Entity)var2.next();
            //        var3.onChunkLoad();
            //    }

            //    this.worldObj.loadEntities(this.entityLists[var1]);
            //}
        }

        /// <summary>
        /// Выгружаем чанк
        /// </summary>
        public void OnChunkUnload()
        {
            IsChunkLoaded = false;
            Save();
            //Iterator var1 = this.chunkTileEntityMap.values().iterator();

            //while (var1.hasNext())
            //{
            //    TileEntity var2 = (TileEntity)var1.next();
            //    this.worldObj.markTileEntityForRemoval(var2);
            //}

            //for (int var3 = 0; var3 < this.entityLists.length; ++var3)
            //{
            //    this.worldObj.unloadEntities(this.entityLists[var3]);
            //}
        }

        /// <summary>
        /// Sets the isModified flag for this Chunk
        /// </summary>
        public void SetChunkModified()
        {
            isModified = true;
            OnModified();
        }

        /// <summary>
        /// Перезаписать чанк
        /// </summary>
        public void Save()
        {
            if (isModified)
            {
                RegionFile region = World.RegionPr.GetRegion(X, Z);
                if (region != null)
                {
                    region.SetChunk(X, Z, Write());
                    isModified = false;
                }
            }
        }

        /// <summary>
        /// Перегенерация чанка и тут же её записываем
        /// </summary>
        public void Regen()
        {
            Generation();
            GenerateSkylightMap();
            RegionFile region = World.RegionPr.GetRegion(X, Z);
            region.SetChunk(X, Z, Write());
        }

        /// <summary>
        /// Генерация чанка
        /// </summary>
        public void Generation()
        {
            Perlin noise = new Perlin();
            noise.Perlin2D(2);
            float size = 0.01f;
            for (int z = 0; z < 16; z++)
            {
                for (int x = 0; x < 16; x++)
                {
                    int realX = X << 4 | x;
                    int realZ = Z << 4 | z;

                    float fn = noise.Noise(size * realX, size * realZ, 5, 0.1f); // -1 .. 1
                    //float n = (fn + 1f) * 24f + 48f;
                    float n = (fn + 1f) * 64f;// + 18f;

                    for (int y = 0; y < 256; y++)
                    {
                        //int realY = y;

                        int id = 0;
                        // byte alphe = 0x00;
                        if (y <= n)
                        {
                            // если не воздух
                            if (y < 46)
                            {
                                // принудительно камень
                                id = 1;
                            }
                            else if (y < 67 && n < 67)
                            {
                                // камень
                                if (y < n - 3) id = 1;
                                // песок
                                else id = 4;
                            }
                            else if (y > 80)
                            {
                                // камень
                                id = 1;
                            }
                            else
                            {
                                // камень
                                if (y < n - 3) id = 1;
                                // земля
                                else if (y < n - 1) id = 2;
                                // дёрн
                                else id = 3;
                            }

                        }
                        else
                        {
                            if (y < 65 && n < 67)
                            {
                                // вода
                                id = 11;
                            }
                        }
                        //id = 3;
                        //if (id > 0 && realY > 4) id = 3;

                        //if (realX == -15 && realZ == -15 && realY == 6) id = 4;
                        //if (realX == -15 && realZ == -15 && realY == 7) id = 4;

                        //if (realX == 0 && realZ == 0 && realY == 8) id = 34;

                        //if (realX == 58 && realZ == 58 && realY == 5) id = 2;
                        //if (realX == 58 && realZ == 58 && realY == 6) id = 2;
                        //if (realX == 58 && realZ == 58 && realY == 7) id = 2;
                        //if (realX == 58 && realZ == 58 && realY == 8) id = 2;

                        //if (realX == 58 && realZ == 56 && realY == 5) id = 2;
                        //if (realX == 58 && realZ == 56 && realY == 6) id = 2;
                        //if (realX == 58 && realZ == 56 && realY == 7) id = 2;
                        //if (realX == 58 && realZ == 56 && realY == 8) id = 2;

                        //if (realX == 56 && realZ == 58 && realY == 5) id = 2;
                        //if (realX == 56 && realZ == 58 && realY == 6) id = 2;
                        //if (realX == 56 && realZ == 58 && realY == 7) id = 2;
                        //if (realX == 56 && realZ == 58 && realY == 8) id = 2;

                        //if (realX == 56 && realZ == 56 && realY == 5) id = 2;
                        //if (realX == 56 && realZ == 56 && realY == 6) id = 2;
                        //if (realX == 56 && realZ == 56 && realY == 7) id = 2;
                        //if (realX == 56 && realZ == 56 && realY == 8) id = 2;

                        //if (realX == 56 && realZ == 56 && realY == 4) id = 4;
                        //if (realX == 57 && realZ == 56 && realY == 4) id = 5;
                        //if (realX == 58 && realZ == 56 && realY == 4) id = 5;

                        //if (id == 2 || id == 143) alphe = 0x01;

                        //int index = (y * VE.CHUNK_WIDTH + z) * VE.CHUNK_WIDTH + x;
                        //Voxel voxel = new Voxel(Voxels[index])
                        //Voxel voxel = new Voxel(Voxels[y, x, z].GetId())
                        ////Voxel voxel = new Voxel(Voxels[y,x,z])
                        //{
                        //    Id = (byte)id
                        //  //  B1 = alphe
                        //};
                        //Voxels[index] = voxel.GetKey();
                        //Voxels[y, x, z] = voxel.GetKey();
                        SetVoxelId(x, y, z, (byte)id);
                        //GetVoxel(x, y, z).SetIdByte((byte)id);
                        //Voxels[y, x, z].SetIdByte((byte)id);//.SetId(voxel.GetKey());
                        //Voxels[y << 8 | z << 4 | x].SetIdByte((byte)id);//.SetId(voxel.GetKey());

                    }
                }
            }
        }

        #region Light

        /// <summary>
        /// Карта высот по чанку, XZ
        /// </summary>
        private int[,] heightMap = new int[16, 16];

        /// <summary>
        /// Может ли видеть небо
        /// </summary>
        /// <param name="pos"></param>
        public bool CanSeeSky(BlockPos pos)
        {
            return pos.Y >= heightMap[pos.X & 15, pos.Z & 15];
        }

        /// <summary>
        /// Заменить блок
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public Block SetBlockState(Block block, bool isTick)
        {
            int x = block.Position.X & 15;
            int y = block.Position.Y;
            int z = block.Position.Z & 15;
            // int var6 = z << 4 | x;

            //if (y >= this.precipitationHeightMap[var6] - 1)
            //{
            //    this.precipitationHeightMap[var6] = -999;
            //}

            int var7 = heightMap[x, z];
            Block blockOld = GetBlock0(new vec3i(x, y, z));

            //EnumBlock eBlockOld = GetBlockState(pos);

            if (block.EBlock == blockOld.EBlock)
            {
                if (block.Properties != blockOld.Properties)
                {
                    SetParam4bit(x, y, z, block.Properties);
                }
                return null;
            }

            // Если кликаем по яблоку меняем на листву TODO:: временно
            if (blockOld.EBlock == EnumBlock.LeavesApple && block.EBlock == EnumBlock.Air)
            {
                block = Blocks.GetBlock(EnumBlock.Leaves, block.Position);
            }
            if (block.EBlock == EnumBlock.Sapling)
            {
                // проверка что на землю садим
                Block blockNew = GetBlock(block.Position.OffsetDown());
                if (blockNew.EBlock != EnumBlock.Dirt && blockNew.EBlock != EnumBlock.Grass)
                    return null;
            }

            bool var12 = false;
            if (block.EBlock == EnumBlock.Water)
            {
                //block.Voxel.SetParam4bit(2);
                //SetVoxelId(x, y, z, block.Id, 2);
                //SetParam4bit(x, y, z, 2);
            }
            else
            {
                SetParam4bit(x, y, z, block.Properties);
            }
            SetVoxelId(x, y, z, block.Id);
            //block.SetVoxel(GetVoxel(x, y, z));




            if (isTick && block.EBlock == EnumBlock.Water)
            {

                AddTicks(liquidTicks, new BlockTick(block.Position, EnumBlock.Water, VE.TICK_WATER));
                //liquidTicks.Add(new BlockTick(block, 10));
                //tickBlock.Add(block);
            }
            else if (isTick && block.EBlock == EnumBlock.Sapling)
            {
                // саженец проростает
                AddTicks(liquidTicks, new BlockTick(block.Position, EnumBlock.Sapling, VE.TICK_TREE_TIME)); // TODO::рандом
            }
            else if (isTick && blockOld.EBlock == EnumBlock.LeavesApple)
            {
                // Яблоко вырастает заного
                AddTicks(liquidTicks, new BlockTick(block.Position, EnumBlock.LeavesApple, VE.TICK_TREE_TIME)); // TODO::рандом
            }
            else if (isTick)
            {
                // проверка соседних блоков на воду
                AddTicks(liquidTicks, SetBlockTicks(block.Position, blockOld.EBlock));
                //liquidTicks.AddRange(SetBlockLiquidTicks(block));
            }

            if (var12)
            {
                // Если не было генерации
                GenerateSkylightMap();
            }
            else
            {
                //int var13 = block.Voxel.GetBlockLightOpacity();
                //int var14 = blockOld.Voxel.GetBlockLightOpacity();
                int var13 = block.GetBlockLightOpacity();
                int var14 = blockOld.GetBlockLightOpacity();

                if (var13 > 0)
                {
                    if (y >= var7)
                    {
                        _RelightBlock(x, y + 1, z);
                    }
                }
                else if (y == var7 - 1)
                {
                    _RelightBlock(x, y, z);
                }

                if (var13 != var14 && (var13 < var14 || GetLightFor(x, y, z, EnumSkyBlock.Sky) > 0 || GetLightFor(x, y, z, EnumSkyBlock.Block) > 0))
                {
                    //PropagateSkylightOcclusion(x, z);
                }
            }

            // пометка на редактирование
            SetChunkModified();
            // так же соседние чанки тоже
            //ChunkNorth().SetChunkModified();
            //ChunkSouth().SetChunkModified();
            //ChunkWest().SetChunkModified();
            //ChunkEast().SetChunkModified();

            return blockOld;


        }

        /// <summary>
        /// проверка соседних блоков на такты
        /// </summary>
        /// <param name="pos"></param>
        protected List<BlockTick> SetBlockTicks(BlockPos pos, EnumBlock eBlock)
        {
            List<BlockTick> bt = new List<BlockTick>();
            for (int i = 0; i < 6; i++)
            {
                BlockPos bpos = pos.Offset((Pole)i);
                Block b = GetBlock(bpos.ToVec3i());

                // вода
                if ((eBlock == EnumBlock.Water || eBlock == EnumBlock.WaterFlowing) && b.IsWater)
                {
                    bt.Add(new BlockTick(b.Position, b.EBlock, VE.TICK_WATER_DRY));
                }
                // листва дерева
                else if ((eBlock == EnumBlock.Log || eBlock == EnumBlock.Leaves || eBlock == EnumBlock.LeavesApple)
                    && (b.EBlock == EnumBlock.Leaves || b.EBlock == EnumBlock.LeavesApple))
                {
                    bt.Add(new BlockTick(b.Position, b.EBlock, VE.TICK_LEAVES));
                }
            }
            return bt;
        }

        /// <summary>
        /// Задать высоту блока
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="id"></param>
        public void SetHeightMap(BlockPos pos, byte id)
        {
            int x = pos.X & 15;
            int y = pos.Y;
            int z = pos.Z & 15;
            if (id == 0)
            {
                if (y == heightMap[x, z])
                {
                    // ищем низ
                    int heightMapMinimum = 1024;
                    int y0 = y;
                    while (true)
                    {
                        if (y0 > 0)
                        {
                            if (GetBlockLightOpacity(x, y - 1, z) == 0)
                            {
                                --y;
                                continue;
                            }
                            heightMap[x, z] = y;
                            if (y < heightMapMinimum) heightMapMinimum = y;
                        }
                        break;
                    }
                }
            }
            else
            {
                heightMap[x, z] = y - 1;
            }
        }

        /// <summary>
        /// Задать верхний блок на x, z
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetUpBlock(int x, int y, int z)
        {
            heightMap[x, z] = y;
        }

        /// <summary>
        /// Сколько света вычитается для прохождения этого блока
        /// </summary>
        public byte GetBlockLightOpacity(int x, int y, int z)
        {
            return Blocks.GetBlockLightOpacity(GetVoxel(x, y, z).GetId());
        }

        /// <summary>
        /// Создает карту высот для блока с нуля
        /// </summary>
        protected void GenerateHeightMap()
        {
            int y0 = 239;// this.getTopFilledSegment();
            int heightMapMinimum = 1024;

            for (int x = 0; x < 16; ++x)
            {
                int z = 0;

                while (z < 16)
                {
                    // this.precipitationHeightMap[x + (z << 4)] = -999;
                    int y = y0 + 16;

                    while (true)
                    {
                        if (y > 0)
                        {
                            //Block block = GetBlock0(new vec3i(x, y -1, z));

                            //if (block.Voxel.GetBlockLightOpacity() == 0)
                            if (GetBlockLightOpacity(x, y - 1, z) == 0)

                            {
                                --y;
                                continue;
                            }

                            heightMap[x, z] = y;

                            if (y < heightMapMinimum)
                            {
                                heightMapMinimum = y;
                            }
                        }

                        ++z;
                        break;
                    }
                }
            }

            //this.isModified = true;
        }
        /// <summary>
        /// Создает начальную карту светового чанка для блока при генерации или загрузке.
        /// </summary>
        public void GenerateSkylightMap()
        {
            // return;
            //GenerateHeightMap();
            // Возвращает самый верхний экземпляр ExtendedBlockStorage для этого Chunk, который фактически содержит блок.
            int y0 = 239;// 239;// this.getTopFilledSegment();
            int heightMapMinimum = 1024;// Integer.MAX_VALUE;

            for (int x = 0; x < 16; ++x)
            {
                int z = 0;

                while (z < 16)
                {
                    // this.precipitationHeightMap[x + (z << 4)] = -999;
                    int y = y0 + 16;

                    while (true)
                    {
                        if (y > 0)
                        {
                            if (GetBlockLightOpacity(x, y - 1, z) == 0)
                            {
                                --y;
                                continue;
                            }
                            heightMap[x, z] = y;

                            if (y < heightMapMinimum) heightMapMinimum = y;
                        }

                        y = 15;
                        int y2 = y0 + 16 - 1;

                        do
                        {
                            int lightOp = GetBlockLightOpacity(x, y2, z);
                            if (lightOp == 0 && y != 15) lightOp = 1;
                            y -= lightOp;

                            if (y > 0)
                            {
                                SetLightFor(x, y2, z, EnumSkyBlock.Sky, (byte)y);
                                World.CheckLightFor(EnumSkyBlock.Sky, new BlockPos(X << 4 + x, y2, Z << 4 + z));
                            }

                            --y2;
                        }
                        while (y2 > 0 && y > 0);

                        ++z;
                        break;
                    }
                }
            }

            return;
            //  this.isModified = true;
        }

        /// <summary>
        /// Инициирует пересчет как блочного света, так и небесного света для данного блока внутри блока.
        /// </summary>
        /// <param name="x">в чанке 0-15</param>
        /// <param name="y">в чанке 0-15</param>
        /// <param name="z">в чанке 0-15</param>
        public void _RelightBlock(int x, int y, int z)
        {
            int var4 = heightMap[x, z] & 255;
            int var5 = var4;

            if (y > var4)
            {
                var5 = y;
            }

            while (var5 > 0 && GetBlockLightOpacity(x, var5 - 1, z) == 0)
            {
                --var5;
            }

            // if (var5 != var4)
            {
                int xReal = X << 4 | x;
                int zReal = Z << 4 | z;
                World.MarkBlocksDirtyVertical(xReal, zReal, var5, var4);
                heightMap[x, z] = var5;
                int var8;
                int var13;

                // if (!this.worldObj.provider.getHasNoSky())
                {

                    if (var5 < var4)
                    {
                        for (var8 = var5; var8 < var4; ++var8)
                        {
                            SetLightFor(x, var8 & 15, z, EnumSkyBlock.Sky, 15);
                        }
                    }
                    else
                    {
                        for (var8 = var4; var8 < var5; ++var8)
                        {
                            SetLightFor(x, var8 & 15, z, EnumSkyBlock.Sky, 0);
                        }
                    }

                    var8 = 15;

                    while (var5 > 0 && var8 > 0)
                    {
                        --var5;
                        var13 = GetBlockLightOpacity(x, var5, z);
                        if (var13 == 0) var13 = 1;
                        var8 -= var13;
                        if (var8 < 0) var8 = 0;
                        SetLightFor(x, var8 & 15, z, EnumSkyBlock.Sky, (byte)var8);
                    }
                }

                var8 = heightMap[x, z];
                var13 = var4;
                int var14 = var8;

                if (var8 < var4)
                {
                    var13 = var8;
                    var14 = var4;
                }

                //if (var8 < this.heightMapMinimum)
                //{
                //    this.heightMapMinimum = var8;
                //}

                //  if (!this.worldObj.provider.getHasNoSky())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        vec3i var12 = EnumFacing.DirectionHorizontalVec(EnumFacing.GetHorizontal(i));
                        _UpdateSkylightNeighborHeight(xReal + var12.x, zReal + var12.z, var13, var14);
                    }

                    _UpdateSkylightNeighborHeight(xReal, zReal, var13, var14);
                }

                //this.isModified = true;
            }
        }

        private void _UpdateSkylightNeighborHeight(int x, int z, int startY, int endY)
        {
            if (endY > startY)
            {
                for (int var5 = startY; var5 < endY; ++var5)
                {
                    World.CheckLightFor(EnumSkyBlock.Sky, new BlockPos(x, var5, z));
                }
                //this.isModified = true;
            }
        }

        #endregion

        /// <summary>
        /// Такт 20 в секунду
        /// </summary>
        public void Tick()
        {
            // Блоки для тактов
            BlocksTick();
        }

        /// <summary>
        /// Массив действий в чанке надо блоками жидкостей ()
        /// TODO:: 2021-07-17 научиться сохранять и считывать
        /// </summary>
        protected Hashtable liquidTicks = new Hashtable();


        protected void AddTicks(Hashtable lt, BlockTick blockTick)
        {
            string key = blockTick.Position.ToString();
            if (lt.ContainsKey(key)) lt[key] = blockTick;
            else lt.Add(key, blockTick);
        }

        protected void AddTicks(Hashtable lt, List<BlockTick> blockTicks)
        {
            foreach (BlockTick bt in blockTicks) AddTicks(lt, bt);
        }


        Hashtable liquidTicksNew;
        /// <summary>
        /// Блоки в тике
        /// </summary>
        protected void BlocksTick()
        {
            List<string> indexs = new List<string>();
            liquidTicksNew = new Hashtable();
            // массив блоковых тиков
            foreach (DictionaryEntry lt in liquidTicks)
            {
                BlockTick bt = lt.Value as BlockTick;
                bt.Tick();

                // блок готов для обработки
                if (bt.IsAction())
                {
                    indexs.Add(lt.Key.ToString());
                    Block block = GetBlock(bt.Position.ToVec3i());
                    if (block.IsWater) LiquidTickBlock(block);// bt.Blk);
                    else if (block.EBlock == EnumBlock.Sapling)
                    {
                        Trees trees = new Trees(World);
                        if (!trees.Generate(block.Position))
                        {
                            AddTicks(liquidTicksNew, new BlockTick(block.Position, block.EBlock, VE.TICK_TREE_REPEAT));
                        }
                    }
                    else if (block.EBlock == EnumBlock.Leaves || block.EBlock == EnumBlock.LeavesApple)
                    {
                        // проверка листвы в 5 блоков от древесины
                        Trees trees = new Trees(World);
                        if (!trees.IsWood(block.Position))
                        {
                            Block blockNew = Blocks.GetAir(block.Position);
                            World.SetBlockState(blockNew, false);
                            AddTicks(liquidTicksNew, SetBlockTicks(blockNew.Position, EnumBlock.Leaves));
                        }
                        else if (block.EBlock == EnumBlock.Leaves && bt.EBlock == EnumBlock.LeavesApple)
                        {
                            Block blockNew = Blocks.GetBlock(EnumBlock.LeavesApple, block.Position);
                            World.SetBlockState(blockNew, false);
                            AddTicks(liquidTicksNew, SetBlockTicks(blockNew.Position, EnumBlock.Leaves));
                        }
                        //AddTicks(liquidTicksNew, new BlockTick(blockNew.Position, 5));
                    }
                }
            }
            // удаляем
            if (indexs.Count > 0)
            {
                for (int i = 0; i < indexs.Count; i++)
                {
                    liquidTicks.Remove(indexs[i]);
                }
            }
            // добавляем новые
            if (liquidTicksNew.Count > 0)
            {
                foreach (DictionaryEntry lt in liquidTicksNew)
                {
                    AddTicks(liquidTicks, lt.Value as BlockTick);
                }
            }
            liquidTicksNew.Clear();
            if (OpenGLF.GetInstance().Cam.ToPositionChunk() == new vec2i(X, Z))
            {
                Debag.GetInstance().ChunkLiquidTicks = liquidTicks.Count;
            }
        }

        protected void LiquidTickBlock(Block block)
        {
            // максимальный параметр
            byte pmax = VE.WATER;

            if (block.EBlock == EnumBlock.Water)
            {
                // если стоячая вода
                bool isPit = false;
                Block bd = GetBlock(block.Position.Offset(Pole.Down).ToVec3i());
                bool isDown = false;
                if (bd.IsWater || bd.EBlock == EnumBlock.Air)
                {
                    isDown = true;
                    AddLquidTicksBlock(EnumBlock.WaterFlowing, bd.Position, 0, VE.TICK_WATER);
                }

                if (!isDown)
                {
                    for (int i = 2; i < 6; i++) // стороны блока
                    {
                        if (LiquidRowPit(block, (Pole)i) >= 0)
                        {
                            isPit = true;
                            AddLquidTicksBlock(
                                EnumBlock.WaterFlowing, block.Position.Offset((Pole)i), block.Properties + 1, VE.TICK_WATER
                            );
                        }
                    }
                }
                if (!isPit)
                {
                    for (int i = 2; i < 6; i++) // низ и стороны блока
                    {
                        Block b = GetBlock(block.Position.Offset((Pole)i).ToVec3i());
                        if (b.EBlock == EnumBlock.Air)
                        {
                            AddLquidTicksBlock(EnumBlock.WaterFlowing, b.Position, 0, VE.TICK_WATER);
                        }
                    }
                }
            }
            else if (block.EBlock == EnumBlock.WaterFlowing)
            {
                // если проточная вода
                // TODO :: 2021.08.01 надо продумать, алгоритм пробегания воды с проверкой и заменить её параметр, пример добавили рядом воду.

                // если сверху вода
                Block b = GetBlock(block.Position.Offset(Pole.Up).ToVec3i());
                bool up = b.EBlock == EnumBlock.Water || b.EBlock == EnumBlock.WaterFlowing;
                // проверка высыхания
                int p = up ? 0 : pmax;
                if (!up)
                {
                    for (int i = 2; i < 6; i++) // стороны блока
                    {
                        // блок снизу
                        b = GetBlock(block.Position.Offset((Pole)i).ToVec3i());
                        if (b.EBlock == EnumBlock.Water)
                        {
                            p = -1;
                            break;
                        }
                        else if (b.EBlock == EnumBlock.WaterFlowing && b.Properties < p)
                        {
                            p = b.Properties;
                        }
                    }
                }
                if (!up && p >= block.Properties) // если сверху нет воды и параметр блока меньше чем соседние блоки
                {
                    // режим высыхания, блок меняем на +2
                    int pp = block.Properties + 1;
                    if (pp >= pmax)
                    {
                        // меняем блок на воздух
                        AddLquidTicksBlock(EnumBlock.Air, block.Position, 0, VE.TICK_WATER_DRY);
                    }
                    else
                    {
                        // проточная вода тоньше слой
                        AddLquidTicksBlock(EnumBlock.WaterFlowing, block.Position, pp, VE.TICK_WATER_DRY);
                    }
                    // запуск проверки соседних блоков на воду
                    AddTicks(liquidTicksNew, SetBlockTicks(block.Position, EnumBlock.WaterFlowing));
                }
                else
                if (block.Properties <= pmax)
                {
                    // проверяем режим растекания
                    // блок снизу
                    b = GetBlock(block.Position.Offset(Pole.Down).ToVec3i());
                    if (b.EBlock == EnumBlock.Air)
                    {
                        AddLquidTicksBlock(EnumBlock.WaterFlowing, b.Position, 0, VE.TICK_WATER);
                    }
                    else if (block.Properties < pmax && !b.IsWater)
                    {
                        // Нижний блок твёрдый, продолжаем растекаться

                        // Проверка сторон на ямки
                        bool isPit = false;
                        for (int i = 2; i < 6; i++) // стороны блока
                        {
                            if (LiquidRowPit(block, (Pole)i) >= 0)
                            {
                                isPit = true;
                                AddLquidTicksBlock(
                                    EnumBlock.WaterFlowing, block.Position.Offset((Pole)i), block.Properties + 1, VE.TICK_WATER
                                );
                            }
                        }
                        if (!isPit)
                        {
                            // Проверка сторон
                            for (int i = 2; i < 6; i++) // стороны блока
                            {
                                Block b2 = GetBlock(block.Position.Offset((Pole)i).ToVec3i());
                                if (b2.EBlock == EnumBlock.Air)
                                {
                                    // Проверяем ниже на предмет не воздуха
                                    AddLquidTicksBlock(EnumBlock.WaterFlowing, b2.Position, block.Properties + 1, VE.TICK_WATER);
                                }
                            }
                        }
                    }
                    else if (block.Properties < pmax && b.IsWater)
                    {
                        // Нижний блок вода, проверка растекания
                        AddLquidTicksBlock(EnumBlock.WaterFlowing, b.Position, 0, VE.TICK_WATER);
                    }
                }
            }
        }

        /// <summary>
        /// Есть ли впереди ямка
        /// </summary>
        protected int LiquidRowPit(Block block, Pole side)
        {
            int pr = block.Properties;
            Block b = block;
            int count = 1;
            while (pr < VE.WATER)
            {
                Block b2 = GetBlock(b.Position.Offset(side).ToVec3i());
                if (b2.EBlock == EnumBlock.Air)
                {
                    // Проверяем ниже на предмет воздуха или воды
                    Block b3 = GetBlock(b2.Position.Offset(Pole.Down).ToVec3i());
                    if (b3.EBlock == EnumBlock.Air || b3.IsWater)
                    {
                        return count;
                    }
                }
                else
                {
                    return -1;
                }
                b = b2;
                pr++;
                count++;
            }
            return -1;
        }


        /// <summary>
        /// Добавить блок на проверку в очередь
        /// </summary>
        /// <param name="eBlock">тип блока</param>
        /// <param name="pos">позиция</param>
        /// <param name="properties">доп параметр</param>
        /// <param name="tick">через сколько тиков</param>
        protected void AddLquidTicksBlock(EnumBlock eBlock, BlockPos pos, int properties, int tick)
        {
            Block blockNew = Blocks.GetBlock(eBlock, pos);
            blockNew.Properties = (byte)properties;
            World.SetBlockState(blockNew, false);
            AddTicks(liquidTicksNew, new BlockTick(blockNew.Position, blockNew.EBlock, tick));
        }

        #region SaveChunk

        /// <summary>
        /// Чтение чанка
        /// </summary>
        /// <param name="source"></param>
        public void Read(byte[] source)
        {
            //for (int i = 0; i < VE.CHUNK_VOLUME; i++)
            //{
            //    byte b1 = source[i * 2];
            //    byte b2 = 0;// source[i * 2 + 1];
            //    if (b1 == 9) { b2 = 1 << 4 | 1; }
            //    else if (b1 == 10) { b2 = 1 << 4 | 0; }
            //    Voxels[i] = (short)(b1 << 8 | b2);
            //    ///Voxels[i] = (short)(source[i * 2] << 8 | source[i * 2 + 1]);
            //}

            int i = 0;
            for (int y = 0; y < 256; y++)

                for (int z = 0; z < 16; z++)
                    for (int x = 0; x < 16; x++)
                    {
                        byte b1 = source[i * 2];
                        byte b2 = source[i * 2 + 1]; // 0
                        // TODO:: Voxel bit
                        // if (b1 == 9) { b2 = 1 << 4 | 1; } // кактус
                        //else if (b1 == 10) { b2 = 1 << 4 | 0; } // стекло
                        //Voxels[y, x, z].LP = (b1 == 9 || b1 == 10);
                        //  VoxelsLP[y, x, z] = (b1 == 9 || b1 == 10);
                        //Vox v = Voxels[y, x, z];

                        //Block bl = 
                        //SetVoxelId(x, y, z, Blocks.GetBlock(b1, new BlockPos(x, y, z)));
                        SetVoxelId(x, y, z, b1);
                        SetLightsFor(x, y, z, b2);
                        //Voxels[y, x, z].SetId((short)(b1 << 8 | b2));
                        ////if (b1 == 9 || b1 == 10)
                        //if ((b1 == 9 || b1 == 10))
                        //{
                        //    Voxels[y, x, z].SetLightPassing((byte)1);
                        //}

                        //Voxels[y, x, z] = (short)(b1 << 8 | b2);

                        //if (b1 == 9)// || b1 == 10)
                        //{
                        //    VoxelsLP[y, x, z] = true;
                        //}


                        i++;
                    }
        }

        /// <summary>
        /// Запись чанка
        /// </summary>
        public byte[] Write()
        {
            List<byte> dest = new List<byte>();
            //for (int i = 0; i < VE.CHUNK_VOLUME; i++)
            //{
            //    short s = Voxels[i];
            //    dest.Add((byte)(s >> 8));
            //    dest.Add((byte)(s & 0x00FF));
            //}
            for (int y = 0; y < 256; y++)

                for (int z = 0; z < 16; z++)
                    for (int x = 0; x < 16; x++)
                    {
                        Voxel voxel = GetVoxel(x, y, z);
                        //byte s = GetVoxel(x, y, z).GetId();// Voxels[y,x,z].GetId();
                        //byte s = Voxels[y << 8 | z << 4 | x].GetId();

                        //short s = Voxels[y, x, z];
                        //dest.Add(s);
                        //dest.Add(0);// (byte)(s & 0x00FF));
                        dest.Add(voxel.GetId());
                        dest.Add(voxel.GetLightsFor());
                    }

            return dest.ToArray();
        }

        #endregion

        #region Events

        /// <summary>
        /// Событие изменение в чанке
        /// </summary>
        public event EventHandler Modified;
        /// <summary>
        /// Событие изменение в чанке
        /// </summary>
        protected void OnModified()
        {
            Modified?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}
