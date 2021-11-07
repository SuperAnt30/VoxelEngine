using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.World.Chk.Light
{
    /// <summary>
    /// Карта блоков освещения
    /// </summary>
    public class MapLight : Map
    {
        /// <summary>
        /// Ключ для блока освещения
        /// </summary>
        protected string Key(vec3i pos, bool sky)
        {
            return pos.ToString() + (sky ? "s" : "b");
        }

        /// <summary>
        /// Добавить или изменить
        /// </summary>
        public void Set(LightStruct light)
        {
            Set(Key(light.Pos, light.Sky), light);
        }

        /// <summary>
        /// Получить значение по ключу
        /// </summary>
        public LightStruct Get(vec3i pos, bool sky)
        {
            string key = Key(pos, sky);
            if (Contains(key)) return (LightStruct)_ht[key];
            return new LightStruct();
        }
    }
}
