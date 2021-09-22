using VoxelEngine.Entity;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.Models
{

    /// <summary>
    /// Базовый объект 3д моделей
    /// </summary>
    public class ModelBase : BufferHeir
    {
        /// <summary>
        /// Размер ширины файла текстуры в пикселях.
        /// </summary>
        public vec2 TextureSize { get; protected set; } = new vec2(64f, 32f);

        

        /// <summary>
        /// Список моделей
        /// </summary>
        //public List<ModelBox> CubeList { get; protected set; } = new List<ModelBox>();

        ///** The X offset into the texture used for displaying this model */
        //private int textureOffsetX;

        ///** The Y offset into the texture used for displaying this model */
        //private int textureOffsetY;



        //Устанавливает модели под разными углами поворота, а затем визуализирует модель.
        public virtual void Render(EntityBase entity, float limbSwing, float limbSwingAmount, float ageInTicks, float headYaw, float headPitch, float scale) {}

        /// <summary>
        /// Устанавливает различные углы поворота модели. Для двуногих, par1 и par2 используются для анимации движения рук и ног, где par1 представляет время (так что руки и ноги качаются вперед и назад), а par2 представляет, насколько "далеко" руки и ноги могут раскачиваться максимум.
        /// </summary>
        /// <param name="limbSwing">счётчик скорости</param>
        /// <param name="limbSwingAmount">Амплитуда 0 нет движения 1.0 максимальная амплитуда</param>
        /// <param name="ageInTicks"></param>
        /// <param name="headYaw"></param>
        /// <param name="headPitch"></param>
        /// <param name="scale">0.0625F коэффициент масштабирования</param>
        /// <param name="entity">Сущность</param>
        public virtual void SetRotationAngles(EntityBase entity, float limbSwing, float limbSwingAmount, float ageInTicks, float headYaw, float headPitch, float scale) { }

        
             
    }
}
