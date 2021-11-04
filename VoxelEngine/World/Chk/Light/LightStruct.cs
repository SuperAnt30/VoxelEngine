using VoxelEngine.Glm;

namespace VoxelEngine.World.Chk.Light
{
    /// <summary>
    /// Структура для расчётов освещения
    /// </summary>
    public struct LightStruct
    {
        /// <summary>
        /// Глобальная позиция
        /// </summary>
        public vec3i Pos;
        /// <summary>
        /// Вектор от центра
        /// </summary>
        public vec3i Vec;
        /// <summary>
        /// Освещение
        /// </summary>
        public byte Light;

        public LightStruct(vec3i pos, vec3i vec, byte light)
        {
            Pos = pos;
            Vec = vec;
            Light = light;
        }

        public LightStruct(vec3i pos, byte light) : this(pos, new vec3i(0), light) { }
    }
}
