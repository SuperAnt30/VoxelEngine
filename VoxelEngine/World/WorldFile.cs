using VoxelEngine.Glm;
using System.IO;

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
        public static void Load()
        {
            if (File.Exists(path))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    // Время игры
                    Debag.GetInstance().TickCount = reader.ReadInt64();

                    Camera cam = OpenGLF.GetInstance().Cam;
                    // Позиция камеры
                    cam.Position = new vec3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    // Pitch & Yaw
                    cam.Yaw = reader.ReadSingle();
                    cam.Pitch = reader.ReadSingle();
                    cam.Rotate(cam.Pitch, cam.Yaw, 0);
                }
            }
        }

        /// <summary>
        /// Записать
        /// </summary>
        public static void Save()
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                // Время игры
                writer.Write(Debag.GetInstance().TickCount);

                Camera cam = OpenGLF.GetInstance().Cam;
                // Позиция камеры
                writer.Write(cam.Position.x);
                writer.Write(cam.Position.y);
                writer.Write(cam.Position.z);
                // Pitch & Yaw
                writer.Write(cam.Yaw);
                writer.Write(cam.Pitch);
            }
        }
    }
}
