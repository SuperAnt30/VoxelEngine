using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    /// <summary>
    /// Объект облости изменения
    /// </summary>
    public class RangeModified : WorldHeir
    {
        /// <summary>
        /// Список всех псевдочанков для обновления рендера
        /// </summary>
        protected List<vec3i> listForRenderUpdate = new List<vec3i>();
        protected vec3i min;
        protected vec3i max;

        public RangeModified(WorldBase world) : base(world)
        {
            RangeForRenderBegin();
        }
        public RangeModified(WorldBase world, BlockPos pos) : base(world)
        {
            RangeForRenderBegin(pos);
        }

        /// <summary>
        /// Запуск области для рендера
        /// </summary>
        /// <param name="pos">позиция блока</param>
        protected void RangeForRenderBegin(BlockPos pos)
        {
            min = pos.ToVec3i();
            max = min;
        }

        /// <summary>
        /// Запуск области для рендера c расчётом, что может и не быть рендера
        /// </summary>
        protected void RangeForRenderBegin()
        {
            min = new vec3i(1024);
            max = new vec3i(0);
        }

        /// <summary>
        /// Окончить область для рендара
        /// </summary>
        protected void RangeForRenderEnd()
        {
            listForRenderUpdate.Clear();
            if (min.y == 1024)
            {
                // изменения не было
                return;
            }

            min -= new vec3i(1);
            max += new vec3i(1);
            vec3i c0 = new vec3i(min.x >> 4, min.y >> 4, min.z >> 4);
            vec3i c1 = new vec3i(max.x >> 4, max.y >> 4, max.z >> 4);

            for (int x = c0.x; x <= c1.x; x++)
            {
                for (int y = c0.y; y <= c1.y; y++)
                {
                    for (int z = c0.z; z <= c1.z; z++)
                    {
                        listForRenderUpdate.Add(new vec3i(x, y, z));
                    }
                }
            }
        }

        /// <summary>
        /// зафиксировать диапазон
        /// </summary>
        /// <param name="pos">позиция блока</param>
        public void BlockModify(BlockPos pos)
        {
            if (min.x > pos.X) min.x = pos.X;
            if (min.y > pos.Y) min.y = pos.Y;
            if (min.z > pos.Z) min.z = pos.Z;
            if (max.x < pos.X) max.x = pos.X;
            if (max.y < pos.Y) max.y = pos.Y;
            if (max.z < pos.Z) max.z = pos.Z;
        }

        /// <summary>
        /// Запустить запрос рендера
        /// </summary>
        public void ModifiedRender()
        {
            RangeForRenderEnd();
            foreach (vec3i cpos in listForRenderUpdate)
            {
                World.SetModifiedRender(cpos);
            }
        }
    }
}
