﻿using System.Collections;
using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.World.Chunk;

namespace VoxelEngine.World
{
    /// <summary>
    /// Объект который хранит и отвечает за кэш регионов
    /// </summary>
    public class RegionProvider
    {
        /// <summary>
        /// Список регионов
        /// </summary>
        protected RegionMap regionMapping = new RegionMap();

        /// <summary>
        /// Параметр для тестов, false - сохранение не будет сохранять
        /// TODO::TEST
        /// </summary>
        protected readonly bool canSave = false;

        /// <summary>
        /// Сылка на объект мира
        /// </summary>
        public WorldD world { get; protected set; }

        public RegionProvider(WorldD worldIn)
        {
            world = worldIn;
        }

        /// <summary>
        /// Добавить регион файл и вернуть его ключ
        /// </summary>
        public vec2i RegionSet(int chunkX, int chunkZ)
        {
            int x = chunkX >> 5;
            int z = chunkZ >> 5;

            if (!regionMapping.Contains(x, z))
            {
                RegionBinary region = new RegionBinary(chunkX, chunkZ);
                region.ReadFile();
                regionMapping.Set(region);
                Debug();
            }
            return new vec2i(x, z);
        }

        /// <summary>
        /// Удалить регион но перед этим записать
        /// </summary>
        public void RegionRemove(int rX, int rZ)
        {
            if (regionMapping.Contains(rX, rZ))
            {
                RegionBinary region = regionMapping.Get(rX, rZ);
                if (canSave) region.WriteFile();
                regionMapping.Remove(rX, rZ);
                Debug();
            }
        }

        /// <summary>
        /// Получить файл региона по координатам чанка
        /// </summary>
        public RegionBinary GetRegion(int chunkX, int chunkZ)
        {
            int x = chunkX >> 5;
            int z = chunkZ >> 5;
            return regionMapping.Get(x, z);
        }

        /// <summary>
        /// Записать все регионы
        /// </summary>
        public void RegionsWrite()
        {
            if (canSave)
            {
                foreach (ChunkD chunk in world.ChunkPr.Values)
                {
                    chunk.Save();
                }
                WorldFile.Save();
                foreach (RegionBinary region in regionMapping.Values)
                {
                    region.WriteFile();
                }
            }
        }

        /// <summary>
        /// Получить массив ключей всех регионов
        /// </summary>
        /// <returns></returns>
        public List<vec2i> KeyRegions()
        {
            List<vec2i> ls = new List<vec2i>();
            foreach (RegionBinary s in  regionMapping.Values) ls.Add(new vec2i(s.X, s.Z));
            return ls;
        }

        protected void Debug()
        {
            Debag.GetInstance().CacheRegion = regionMapping.Count;
            Debag.GetInstance().CacheRegionMem = regionMapping.Mem();
        }

        /// <summary>
        /// Вернуть коллекцию
        /// </summary>
        public virtual ICollection Values { get { return regionMapping.Values; } }
    }
}