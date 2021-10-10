using VoxelEngine.Glm;
using VoxelEngine.World;

namespace VoxelEngine.Gen.Group
{
    /// <summary>
    /// Перечень всех моделей групп
    /// </summary>
    public class Groups
    {
        public static GroupBase GetBin(WorldBase world, vec3i pos, EnumGroup group)
        {
            switch (group)
            {
                case EnumGroup.Door: return new GroupDoor(world, pos);//, Blocks.GetBlock(EnumBlock.Door, new BlockPos(pos)));
            }
            return null;
        }
    }
}
