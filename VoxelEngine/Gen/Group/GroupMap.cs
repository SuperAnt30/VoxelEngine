using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.Gen.Group
{
    public class GroupMap : Map
    {
        /// <summary>
        /// Добавить или изменить группу
        /// </summary>
        public void Set(vec3i pos, GroupBase group)
        {
            base.Set(pos.ToString(), group);
        }

        /// <summary>
        /// Получить значение по ключу
        /// </summary>
        public new GroupBase Get(string key)
        {
            return base.Get(key) as GroupBase;
        }

        /// <summary>
        /// Получить значение по позиции
        /// </summary>
        public GroupBase Get(vec3i pos)
        {
            return base.Get(pos.ToString()) as GroupBase;
        }

        /// <summary>
        /// Проверить по наличию ключа
        /// </summary>
        public bool Contains(GroupBase group)
        {
            return _ht.ContainsKey(group.Position.ToString());
        }

        /// <summary>
        /// Удалить группу
        /// </summary>
        public void Remove(vec3i pos)
        {
            _ht.Remove(pos.ToString());
        }
    }
}
