using System;
using System.Collections;
using System.Collections.Generic;
using VoxelEngine.Binary;
using VoxelEngine.Gen;
using VoxelEngine.Gen.Group;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.Renderer.Chk;
using VoxelEngine.Util;
using VoxelEngine.Vxl;
using VoxelEngine.World.Biome;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk.Light;

namespace VoxelEngine.World.Chk
{
    public class ChunkBase : Coords
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
        public WorldBase World { get; protected set; }
        /// <summary>
        /// Загружен ли чанк
        /// </summary>
        public bool IsChunkLoaded { get; protected set; } = false;
        /// <summary>
        /// Подготовка чанка к рендеру
        /// </summary>
        public EnumGeterationStatus PreparationStatus { get; protected set; } = EnumGeterationStatus.Chunk;
        /// <summary>
        /// Прячем при необходимости чанк рендера
        /// </summary>
        public ChunkRender ChunkTag { get; set; }

        /// <summary>
        /// Для записи в файл
        /// </summary>
        protected ChunkBinary chunkBinary;
        /// <summary>
        /// Столбцы биомов
        /// </summary>
        protected EnumBiome[,] eBiomes = new EnumBiome[16, 16];
        /// <summary>
        /// Колекция групповых моделей
        /// </summary>
        public GroupMap GroupModel { get; protected set; } = new GroupMap();
        /// <summary>
        /// Объект работы с освещением
        /// </summary>
        public ChunkLight Light { get; protected set; }

        protected ChunkBase() { }
        public ChunkBase(WorldBase worldIn, int x, int z)
        {
            World = worldIn;
            Initialize(x, z);
        }

        protected void Initialize(int x, int z)
        {
            X = x;
            Z = z;
            for (int i = 0; i < StorageArrays.Length; i++) StorageArrays[i] = new ChunkStorage();
            chunkBinary = new ChunkBinary(this);
            Light = new ChunkLight(this);
        }

        /// <summary>
        /// Задать биом
        /// </summary>
        public void SetBiome(int x, int z, EnumBiome biome)
        {
            eBiomes[x, z] = biome;
        }
        /// <summary>
        /// Получить биом по локальным координатам
        /// </summary>
        public EnumBiome GetBiome(int x, int z)
        {
            return eBiomes[x, z];
        }

        /// <summary>
        /// Получить биом глобальным координатам блока
        /// </summary>
        public EnumBiome GetBiome(BlockPos pos)
        {
            return eBiomes[pos.X & 15, pos.Z & 15];
        }


        #region Block Storage

        /// <summary>
        /// Получить группу по локальной координате
        /// </summary>
        public GroupBase GetGroup(vec3i pos)
        {
            string key = pos.ToString();
            if (GroupModel.Contains(key))
            {
                return GroupModel.Get(key);
            }
            return null;
        }

        /// <summary>
        /// Получить блок
        /// </summary>
        public BlockBase GetBlock(BlockPos pos)
        {
            return GetBlock(pos.ToVec3i());
        }

        /// <summary>
        /// Получить блок по глобальным координатам
        /// </summary>
        public BlockBase GetBlock(vec3i pos)
        {
            if (pos.x >> 4 == X && pos.z >> 4 == Z)
            {
                vec3i pos0 = new vec3i(pos.x & 15, pos.y, pos.z & 15);
                return Blocks.GetBlock(GetVoxel(pos0.x, pos0.y, pos0.z), new BlockPos(pos), GetGroup(pos0));
            }
            return World.GetBlock(pos);
        }

        /// <summary>
        /// Получить блок по координатам чанка
        /// </summary>
        public BlockBase GetBlock0(vec3i pos)
        {
            if (pos.x >> 4 == 0 && pos.z >> 4 == 0)
            {
                return Blocks.GetBlock(GetVoxel(pos), new BlockPos(X << 4 | pos.x, pos.y, Z << 4 | pos.z), GetGroup(pos));
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
        /// Получить значение вокселя по координатам чанка
        /// </summary>
        public void SetVoxel(int x, int y, int z, Voxel voxel)
        {
            if (y >= 0 && y >> 4 < StorageArrays.Length)
            {
                ChunkStorage storage = StorageArrays[y >> 4];
                storage.SetVoxel(x, y & 15, z, voxel);
            }
        }

        /// <summary>
        /// Вернуть тип блока
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public EnumBlock GetBlockState(BlockPos pos)
        {
            return GetVoxel(pos.ToVec3i()).GetEBlock();
        }

        /// <summary>
        /// Задать воксель
        /// </summary>
        public void SetBlockState(int x, int y, int z, EnumBlock eBlock)
        {
            Voxel voxel = GetVoxel(x, y, z);
            voxel.SetEBlock(eBlock);
            SetVoxel(x, y, z, voxel);
        }

        /// <summary>
        /// Задать дополнительный параметр блока в 4 бита
        /// </summary>
        public void SetParam4bit(int x, int y, int z, byte param)
        {
            Voxel voxel = GetVoxel(x, y, z);
            if (!voxel.IsEmpty)
            {
                voxel.SetParam4bit(param);
                SetVoxel(x, y, z, voxel);
               // SetChunkModified();
            }
        }

        /// <summary>
        /// Задать яркость блока от неба
        /// </summary>
        public void SetLightFor(int x, int y, int z, EnumSkyBlock type, byte light)
        {
            Voxel voxel = GetVoxel(x, y, z);
            if (!voxel.IsEmpty)
            {
                voxel.SetLightFor(type, light);
                SetVoxel(x, y, z, voxel);
                // TODO::211022 узнать и вынести за пределы, обработчика всех вокселей. По команде
                //SetChunkModified(); 
                
            }
        }

        /// <summary>
        /// получить яркость блока от неба
        /// </summary>
        public byte GetLightFor(int x, int y, int z, EnumSkyBlock type)
        {
            Voxel voxel = GetVoxel(x, y, z);
            if (voxel.IsEmpty) return (byte)type;
            return voxel.GetLightFor(type);
        }

        #endregion


        public bool IsLoad { get; protected set; } = false;

        /// <summary>
        /// Загрузить или сгенерировать данные чанка
        /// </summary>
        public bool LoadinData()
        {
            RegionBinary region = World.RegionPr.GetRegion(X, Z);
            if (region == null)
            {
                // Если региона нет, мы его загружаем но выдаём не загруженный данные, для повторных проверок
                //World.RegionPr.RegionSet(X, Z);
                return false;
            }
            byte[] b = region.GetChunk(X, Z);
            if (b != null)
            {
                chunkBinary.Read(b);
                IsLoad = true;
                //Light.GenerateHeightMap();
                //StartRecheckGaps(); // TODO::Под вопросом, возможно записать данные в файл
            }
            else
            {
                // Если его нет, то генерируем
                GenerationChunk();
                //GenerateHeightMap();
                // TODO::TEST
                //GenerateSkylightMap();
                //StartRecheckGaps(); 
                //func_177441_y();
                // и сразу же записываем
                //region.SetChunk(X, Z, chunkBinary.Write());
            }
            return true;
        }

        /// <summary>
        /// Загружен чанк
        /// </summary>
        public void ChunkLoad()
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
        public void ChunkUnload()
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
                RegionBinary region = World.RegionPr.GetRegion(X, Z);
                if (region != null)
                {
                    region.SetChunk(X, Z, chunkBinary.Write());
                    isModified = false;
                }
            }
        }

        /// <summary>
        /// Перегенерация чанка и тут же её записываем
        /// </summary>
        public void Regen()
        {
            // TODO:: пробежаться по блокам и убрать блоки с освещением и после этого генерацию

            GenerationChunk();
            GenerationArea();
            //GenerateSkylightMap();
            
            RegionBinary region = World.RegionPr.GetRegion(X, Z);
            region.SetChunk(X, Z, chunkBinary.Write());
        }

        /// <summary>
        /// Получить параметр генерации чанка
        /// </summary>
        /// <returns></returns>
        public byte GetGenerationStatus()
        {
            byte status = (byte)PreparationStatus;
            if (status > 2) status = 2;
            return status;
        }

        /// <summary>
        /// Получить параметр генерации чанка
        /// </summary>
        /// <returns></returns>
        public void SetGenerationStatus(byte status)
        {
            if (status > 2) status = 2;
            PreparationStatus = (EnumGeterationStatus)status;
        }

        #region Generation

        /// <summary>
        /// Генерация тикущего чанка без соседних
        /// </summary>
        public void GenerationChunk()
        {
            ChunkGenerate chunkGenerate = new ChunkGenerate(this);
            chunkGenerate.Generation();
            PreparationStatus = EnumGeterationStatus.Chunk;
        }

        /// <summary>
        /// Генерация с соседними чанками, деревья
        /// </summary>
        public void GenerationArea()
        {
            ChunkGenerate chunkGenerate = new ChunkGenerate(this);
            // Генерация деревьев
            chunkGenerate.GenerationArea(); // долгий процесс

            //Light.GenerateHeightMap();
            PreparationStatus = EnumGeterationStatus.Area;
        }

        /// <summary>
        /// Генерация с соседними чанками, деревья освещение
        /// </summary>
        public void GenerationDoubleArea()
        {
            //LightOld.GenerateHeightMapSky();
            //LightOld.StartRecheckGaps();

            if (IsLoad) Light.GenerateHeightMap();
            else Light.GenerateHeightMapSky();
            //GenerateHeightMap(); // вместо освещения
            // Проверка освещения
            // StartRecheckGaps(true); // очень долгий процесс
            PreparationStatus = EnumGeterationStatus.DoubleArea;
        }

        /// <summary>
        /// Генерация подготовка для рендера
        /// </summary>
        public void GenerationReady()
        {
            if (!IsLoad) Light.StartRecheckGaps();
            PreparationStatus = EnumGeterationStatus.Ready;
        }

        #endregion

        /// <summary>
        /// Генерация чанка
        /// </summary>
        //public void GenerationOld()
        //{
        //    Perlin noise = new Perlin();
        //    noise.Perlin2D(2);
        //    float size = 0.01f;
        //    for (int z = 0; z < 16; z++)
        //    {
        //        for (int x = 0; x < 16; x++)
        //        {
        //            int realX = X << 4 | x;
        //            int realZ = Z << 4 | z;

        //            float fn = noise.Noise(size * realX, size * realZ, 5, 0.1f); // -1 .. 1
        //            //float n = (fn + 1f) * 24f + 48f;
        //            float n = (fn + 1f) * 64f;// + 18f;

        //            for (int y = 0; y < 256; y++)
        //            {
        //                //int realY = y;

        //                int id = 0;
        //                // byte alphe = 0x00;
        //                if (y <= n)
        //                {
        //                    // если не воздух
        //                    if (y < 46)
        //                    {
        //                        // принудительно камень
        //                        id = 1;
        //                    }
        //                    else if (y < 67 && n < 67)
        //                    {
        //                        // камень
        //                        if (y < n - 3) id = 1;
        //                        // песок
        //                        else id = 4;
        //                    }
        //                    else if (y > 80)
        //                    {
        //                        // камень
        //                        id = 1;
        //                    }
        //                    else
        //                    {
        //                        // камень
        //                        if (y < n - 3) id = 1;
        //                        // земля
        //                        else if (y < n - 1) id = 2;
        //                        // дёрн
        //                        else id = 3;
        //                    }

        //                }
        //                else
        //                {
        //                    if (y < 65 && n < 67)
        //                    {
        //                        // вода
        //                        id = 11;
        //                    }
        //                }
        //                //id = 3;
        //                //if (id > 0 && realY > 4) id = 3;

        //                //if (realX == -15 && realZ == -15 && realY == 6) id = 4;
        //                //if (realX == -15 && realZ == -15 && realY == 7) id = 4;

        //                //if (realX == 0 && realZ == 0 && realY == 8) id = 34;

        //                //if (realX == 58 && realZ == 58 && realY == 5) id = 2;
        //                //if (realX == 58 && realZ == 58 && realY == 6) id = 2;
        //                //if (realX == 58 && realZ == 58 && realY == 7) id = 2;
        //                //if (realX == 58 && realZ == 58 && realY == 8) id = 2;

        //                //if (realX == 58 && realZ == 56 && realY == 5) id = 2;
        //                //if (realX == 58 && realZ == 56 && realY == 6) id = 2;
        //                //if (realX == 58 && realZ == 56 && realY == 7) id = 2;
        //                //if (realX == 58 && realZ == 56 && realY == 8) id = 2;

        //                //if (realX == 56 && realZ == 58 && realY == 5) id = 2;
        //                //if (realX == 56 && realZ == 58 && realY == 6) id = 2;
        //                //if (realX == 56 && realZ == 58 && realY == 7) id = 2;
        //                //if (realX == 56 && realZ == 58 && realY == 8) id = 2;

        //                //if (realX == 56 && realZ == 56 && realY == 5) id = 2;
        //                //if (realX == 56 && realZ == 56 && realY == 6) id = 2;
        //                //if (realX == 56 && realZ == 56 && realY == 7) id = 2;
        //                //if (realX == 56 && realZ == 56 && realY == 8) id = 2;

        //                //if (realX == 56 && realZ == 56 && realY == 4) id = 4;
        //                //if (realX == 57 && realZ == 56 && realY == 4) id = 5;
        //                //if (realX == 58 && realZ == 56 && realY == 4) id = 5;

        //                //if (id == 2 || id == 143) alphe = 0x01;

        //                //int index = (y * VE.CHUNK_WIDTH + z) * VE.CHUNK_WIDTH + x;
        //                //Voxel voxel = new Voxel(Voxels[index])
        //                //Voxel voxel = new Voxel(Voxels[y, x, z].GetId())
        //                ////Voxel voxel = new Voxel(Voxels[y,x,z])
        //                //{
        //                //    Id = (byte)id
        //                //  //  B1 = alphe
        //                //};
        //                //Voxels[index] = voxel.GetKey();
        //                //Voxels[y, x, z] = voxel.GetKey();
        //                SetVoxelId(x, y, z, (byte)id);
        //                //GetVoxel(x, y, z).SetIdByte((byte)id);
        //                //Voxels[y, x, z].SetIdByte((byte)id);//.SetId(voxel.GetKey());
        //                //Voxels[y << 8 | z << 4 | x].SetIdByte((byte)id);//.SetId(voxel.GetKey());

        //            }
        //        }
        //    }
        //}

        

        /// <summary>
        /// Заменить блок
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public BlockBase SetBlockState(BlockBase block, bool isTick)
        {
            int x = block.Position.X & 15;
            int y = block.Position.Y;
            int z = block.Position.Z & 15;
            // int var6 = z << 4 | x;

            //if (y >= this.precipitationHeightMap[var6] - 1)
            //{
            //    this.precipitationHeightMap[var6] = -999;
            //}

            //int yh = Light.GetHeight(x, z);
            BlockBase blockOld = GetBlock0(new vec3i(x, y, z));

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
            if (block.IsOnGrass)
            {
                // проверка что на землю садим
                BlockBase blockNew = GetBlock(block.Position.OffsetDown());
                if (blockNew.EBlock != EnumBlock.Dirt && blockNew.EBlock != EnumBlock.Grass)
                    return null;
            }
            if (block.EBlock == EnumBlock.Cactus)
            {
                // проверка что на песок садим
                BlockBase blockNew = GetBlock(block.Position.OffsetDown());
                if (blockNew.EBlock != EnumBlock.Sand && blockNew.EBlock != EnumBlock.Cactus) return null;
            }

           // bool var12 = false;
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
            SetBlockState(x, y, z, block.EBlock);
            //block.SetVoxel(GetVoxel(x, y, z));




            if (isTick)
            {
                if (block.EBlock == EnumBlock.Water)
                {

                    AddTicks(liquidTicks, new BlockTick(block.Position, EnumBlock.Water, VE.TICK_WATER));
                    //liquidTicks.Add(new BlockTick(block, 10));
                    //tickBlock.Add(block);
                }
                else if (block.EBlock == EnumBlock.Sapling)
                {
                    // саженец проростает
                    AddTicks(liquidTicks, new BlockTick(block.Position, EnumBlock.Sapling, VE.TICK_TREE_TIME)); // TODO::рандом
                }
                else if (blockOld.EBlock == EnumBlock.LeavesApple)
                {
                    // Яблоко вырастает заного
                    AddTicks(liquidTicks, new BlockTick(block.Position, EnumBlock.LeavesApple, VE.TICK_TREE_TIME)); // TODO::рандом
                }
                else
                {
                    // проверка соседних блоков на воду
                    AddTicks(liquidTicks, SetBlockTicks(block.Position, blockOld.EBlock));
                    //liquidTicks.AddRange(SetBlockLiquidTicks(block));
                }
            }

            //if (y >= yh)
            //{
            //    // Если не было генерации
            //    Light.GenerateHeightMapSky();
            //    //GenerateSkylightMap();
            //}
            //else
            {
                //int var13 = block.Voxel.GetBlockLightOpacity();
                //int var14 = blockOld.Voxel.GetBlockLightOpacity();
                /**
                int opacity = block.GetBlockLightOpacity();
                int opacityOld = blockOld.GetBlockLightOpacity();

                if (opacity > 0)
                {
                    if (y >= yh)
                    {
                        Light.RelightBlock(x, y + 1, z);
                    }
                }
                else if (y == yh - 1)
                {
                    Light.RelightBlock(x, y, z);
                }
                */
                //if (opacity != opacityOld && (opacity < opacityOld || GetLightFor(x, y, z, EnumSkyBlock.Sky) > 0 || GetLightFor(x, y, z, EnumSkyBlock.Block) > 0))
                //{
                //    //PropagateSkylightOcclusion(x, z);
                //}
            }

            // пометка на редактирование
            //SetChunkModified();

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
                BlockBase b = GetBlock(bpos.ToVec3i());

                // вода
                if (/*(eBlock == EnumBlock.Water || eBlock == EnumBlock.WaterFlowing) && */b.IsWater)
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
        /// Такт 20 в секунду
        /// </summary>
        public void Tick(long tick)
        {
            // Блоки для тактов
            BlocksTick();
        }

        /// <summary>
        /// Массив действий в чанке надо блоками жидкостей ()
        /// </summary>
        protected Hashtable liquidTicks = new Hashtable();

        /// <summary>
        /// Вернуть массив задач
        /// </summary>
        public BlockTickBin[] GetBlockTickBins()
        {
            List<BlockTickBin> btbs = new List<BlockTickBin>();
            foreach (BlockTick bt in liquidTicks.Values)
            {
                btbs.Add(bt.Get());
            }
            return btbs.ToArray();
        }
        /// <summary>
        /// Задать массив задачь
        /// </summary>
        /// <param name="blockTickBins"></param>
        public void SetBlockTickBins(BlockTickBin[] blockTickBins)
        {
            if (blockTickBins != null && blockTickBins.Length > 0)
            {
                foreach (BlockTickBin btb in blockTickBins)
                {
                    if (btb != null)
                    {
                        AddTicks(liquidTicks, new BlockTick(new BlockPos(btb.Position), (EnumBlock)btb.EBlock, btb.CountTick));
                    }
                }
            }
        }


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
            Hashtable hashtable = (Hashtable)liquidTicks.Clone();
            foreach (DictionaryEntry lt in hashtable)
            {
                BlockTick bt = lt.Value as BlockTick;
                bt.Tick();

                // блок готов для обработки
                if (bt.IsAction())
                {
                    indexs.Add(lt.Key.ToString());
                    BlockBase block = GetBlock(bt.Position.ToVec3i());
                    if (block.IsWater)
                    {
                        // жидкость вода
                        LiquidTickBlock(block);// bt.Blk);
                    }
                    else if (block.EBlock == EnumBlock.Sapling)
                    {
                        // ростёт дерево
                        GenTrees trees = new GenTrees(World, block.Position.ToVec3i());
                        trees.Light();
                        if (!trees.Put())
                        {
                            AddTicks(liquidTicksNew, new BlockTick(block.Position, block.EBlock, VE.TICK_TREE_REPEAT));
                        }
                    }
                    else if (block.EBlock == EnumBlock.Leaves || block.EBlock == EnumBlock.LeavesApple)
                    {
                        // проверка листвы в 5 блоков от древесины
                        GenTrees trees = new GenTrees(World, block.Position.ToVec3i());
                        trees.Light();
                        if (!trees.IsWood(block.Position))
                        {
                            BlockBase blockNew = Blocks.GetAir(block.Position);
                            World.SetBlockState(blockNew, false);
                            AddTicks(liquidTicksNew, SetBlockTicks(blockNew.Position, EnumBlock.Leaves));
                        }
                        else if (block.EBlock == EnumBlock.Leaves && bt.EBlock == EnumBlock.LeavesApple)
                        {
                            BlockBase blockNew = Blocks.GetBlock(EnumBlock.LeavesApple, block.Position);
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
            if (OpenGLF.GetInstance().Cam.ChunkPos == new vec2i(X, Z))
            {
                Debug.ChunkLiquidTicks = liquidTicks.Count;
            }
        }

        protected void LiquidTickBlock(BlockBase block)
        {
            // максимальный параметр
            byte pmax = VE.WATER;

            if (block.EBlock == EnumBlock.Water)
            {
                // если стоячая вода
                bool isPit = false;
                BlockBase bd = GetBlock(block.Position.Offset(Pole.Down).ToVec3i());
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
                        BlockBase b = GetBlock(block.Position.Offset((Pole)i).ToVec3i());
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
                BlockBase b = GetBlock(block.Position.Offset(Pole.Up).ToVec3i());
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
                                BlockBase b2 = GetBlock(block.Position.Offset((Pole)i).ToVec3i());
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
        protected int LiquidRowPit(BlockBase block, Pole side)
        {
            int pr = block.Properties;
            BlockBase b = block;
            int count = 1;
            while (pr < VE.WATER)
            {
                BlockBase b2 = GetBlock(b.Position.Offset(side).ToVec3i());
                if (b2.EBlock == EnumBlock.Air)
                {
                    // Проверяем ниже на предмет воздуха или воды
                    BlockBase b3 = GetBlock(b2.Position.Offset(Pole.Down).ToVec3i());
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
            BlockBase blockNew = Blocks.GetBlock(eBlock, pos);
            blockNew.SetProperties((byte)properties);
            World.SetBlockState(blockNew, false);
            AddTicks(liquidTicksNew, new BlockTick(blockNew.Position, blockNew.EBlock, tick));
        }

        /// <summary>
        /// Внести группы в чанк блоков из сохранения
        /// </summary>
        public void SetGroupModels(GroupBin[] groups)
        {
            if (groups != null && groups.Length > 0)
            {
                foreach (GroupBin group in groups)
                {
                    int cx = group.X >> 4;
                    int cz = group.Z >> 4;
                    //vec3i pos = new vec3i(group.X, group.Y, group.Z);
                    if (cx == X && cz == Z)
                    {
                        vec3i pos = new vec3i(group.X, group.Y, group.Z);
                        // Если позиция блока находится в этом чанке, мы прорисовываем всю дверь
                        GroupBase groupBase = Groups.GetBin(World, pos, (EnumGroup)group.Type);
                        groupBase.LoadBin(this, group.Properties);
                    }
                    else
                    {
                        // Позиция в другом чанке, мы даём запрос на прогрузку двери 
                        // чанка с нулевым параметром 
                        if (World.IsChunk(cx, cz))
                        {
                            vec3i pos0 = new vec3i(group.X & 15, group.Y, group.Z & 15);
                            GroupBase groupBase = World.GetChunk(cx, cz).GetGroup(pos0);
                            if (groupBase != null)
                            {
                                groupBase.RequestLoad(this);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Сгенерировать группы моделей в чанке для сохранения
        /// </summary>
        public GroupBin[] GetGroupBins()
        {
            List<GroupBin> groups = new List<GroupBin>();
            List<string> keys = new List<string>();

            foreach (GroupBase group in GroupModel.Values)
            {
                GroupBin groupBin = new GroupBin()
                {
                    X = group.Position.x,
                    Y = group.Position.y,
                    Z = group.Position.z,
                    Type = (byte)group.Type,
                    Properties = group.GetProperties()
                };

                string key = group.Position.ToString();

                if (!keys.Contains(key))
                {
                    groups.Add(groupBin);
                    keys.Add(key);
                }
            }
            return groups.ToArray();
        }

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
