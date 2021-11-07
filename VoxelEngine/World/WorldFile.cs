using VoxelEngine.Glm;
using System.IO;
using VoxelEngine.World;
using System.Runtime.Serialization.Formatters.Binary;
using VoxelEngine.Binary;

namespace VoxelEngine
{
    /// <summary>
    /// Сохранения параметров мира
    /// </summary>
    public class WorldFile
    {
        #region Old
        protected static string path = "save.dat";

        /// <summary>
        /// Загрузить
        /// </summary>
        public static void Load(WorldBase world)
        {
            if (File.Exists(path))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    // Время игры
                    VEC.SetTick(reader.ReadInt64());
                    
                    // Позиция камеры
                    world.Entity.HitBox.SetPos(new vec3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                    // Pitch & Yaw
                    world.Entity.SetRotation(reader.ReadSingle(), reader.ReadSingle());
                }
            }
        }

        /// <summary>
        /// Записать
        /// </summary>
        public static void Save(WorldBase world)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                // Время игры
                writer.Write(VEC.TickCount);

                // Позиция камеры
                writer.Write(world.Entity.HitBox.Position.x);
                writer.Write(world.Entity.HitBox.Position.y);
                writer.Write(world.Entity.HitBox.Position.z);
                // Pitch & Yaw
                writer.Write(world.Entity.RotationYaw);
                writer.Write(world.Entity.RotationPitch);
            }
        }

        #endregion


        
        /// <summary>
        /// Путь к директории мира в который играешь
        /// </summary>
        public static string ToPath()
        {
            return ToPath(VEC.Name);
        }

        /// <summary>
        /// Путь к директории мира с именем
        /// </summary>
        public static string ToPath(string name)
        {
            return "saves" + Path.DirectorySeparatorChar + name + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Путь к директории регионов
        /// </summary>
        public static string ToPathRegions()
        {
            return ToPath() + "regions" + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Проверка пути, если нет, то создаём
        /// </summary>
        public static void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Сохранить данные мира
        /// </summary>
        public static void WriteFile(WorldBase world)
        {
            string path = ToPath();
            CheckPath(path);
            path += "info.dat";
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream ms = new FileStream(path, FileMode.Create))
            {
                WorldBin worldBin = WorldBaseToBin(world);
                formatter.Serialize(ms, worldBin);
            }
        }

        /// <summary>
        /// Чтение данного мира
        /// </summary>
        /// <param name="name">Имя мира</param>
        /// <param name="world">объект мира</param>
        /// <returns>true - мир прочтён</returns>
        public static bool ReadFile(string name, WorldBase world)
        {
            VEC.SetName(name);
            string path = ToPath(name) + "info.dat";
            if (File.Exists(path))
            {
                using (FileStream ms = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    WorldBin worldBin = formatter.Deserialize(ms) as WorldBin;
                    WorldBinToBase(world, worldBin);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Из бинарника в базовый мир
        /// </summary>
        protected static void WorldBinToBase(WorldBase worldBase, WorldBin worldBin)
        {
            VEC.SetTick(worldBin.TickCount);
            //VEC.SetName(worldBin.Name);
            worldBase.Entity.HitBox.SetPos(worldBin.Position);
            worldBase.Entity.SetRotation(worldBin.RotationYaw, worldBin.RotationPitch);
            worldBase.SetSeed(worldBin.Seed);
        }

        /// <summary>
        /// Из базового мира в бинарник
        /// </summary>
        protected static WorldBin WorldBaseToBin(WorldBase world)
        {
            return new WorldBin()
            {
                TickCount = VEC.TickCount,
                Name = VEC.Name,
                Position = world.Entity.HitBox.Position,
                RotationYaw = world.Entity.RotationYaw,
                RotationPitch = world.Entity.RotationPitch,
                Seed = world.Seed
            };
        }
    }
}
