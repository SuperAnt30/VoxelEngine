using VoxelEngine.Glm;
using System.IO;
using VoxelEngine.Graphics;
using VoxelEngine.World;

namespace VoxelEngine
{
    /// <summary>
    /// Сохранения параметров мира
    /// </summary>
    public class WorldFile
    {
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
                    VEC.GetInstance().SetTick(reader.ReadInt64());
                    
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
                writer.Write(VEC.GetInstance().TickCount);

                // Позиция камеры
                writer.Write(world.Entity.HitBox.Position.x);
                writer.Write(world.Entity.HitBox.Position.y);
                writer.Write(world.Entity.HitBox.Position.z);
                // Pitch & Yaw
                writer.Write(world.Entity.RotationYaw);
                writer.Write(world.Entity.RotationPitch);
            }
        }
    }
}
