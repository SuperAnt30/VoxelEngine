using System.Collections;
using System.Collections.Generic;
using VoxelEngine.Entity;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.Renderer.Chk;
using VoxelEngine.Renderer.Entity;
using VoxelEngine.Util;
using VoxelEngine.World.Chk;

namespace VoxelEngine.Renderer
{
    /// <summary>
    /// Объект для 3д хранение Mesh
    /// </summary>
    public class WorldMesh
    {
        /// <summary>
        /// Массив кэша чанков vec2i, ChunkMeshs
        /// </summary>
        protected Hashtable chunks = new Hashtable();

        /// <summary>
        /// Массив кэша сущностей EntityMesh
        /// </summary>
        protected Hashtable entities = new Hashtable();

        /// <summary>
        /// Прорисовка сущностей
        /// </summary>
        public void DrawEntities()
        {
            Debug.GetInstance().CountMeshEntities = entities.Count;

            foreach (EntityMesh entity in entities.Values)
            {
                entity.Draw();
            }
        }

        /// <summary>
        /// Ключ для массива
        /// </summary>
        public static string KeyChunk(int x, int z)
        {
            return new vec2i(x, z).ToString();
        }

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void Draw()
        {
            Debug.GetInstance().CountPoligonChunk = 0;
            Debug.GetInstance().CountMeshChunk = 0;
            Camera cam = OpenGLF.GetInstance().Cam;
            ChunkLoading[] spiral = cam.ChunkLoadingFC;

            // Прорисовка для алфы с далека и ближе
            for (int i = spiral.Length - 1; i >= 0; i--)
            {
                vec2i v = new vec2i(spiral[i].X, spiral[i].Z);
                if (cam.ChunksFC.ContainsKey(v.ToString()))
                {
                    // Если Frustum Culling прошёл
                    string key = v.ToString();
                    if (chunks.ContainsKey(key))
                    {
                        // Чанк имеется
                        ChunkMeshs cm = chunks[key] as ChunkMeshs;
                        // Прорисовка чанка сплошных блоков
                        cm.MeshDense.Draw();
                        Debug.GetInstance().CountMeshChunk++;
                        Debug.GetInstance().CountPoligonChunk += cm.MeshDense.CountPoligon;
                        if (cm.MeshAlpha.CountPoligon > 0)
                        {
                            // Прорисовка чанка альфа блоков если такие имеются
                            cm.MeshAlpha.Draw();
                            Debug.GetInstance().CountPoligonChunk += cm.MeshAlpha.CountPoligon;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получить чанк с кэша, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkMeshs GetChunk(int x, int z)
        {
            string key = KeyChunk(x, z);
            if (!chunks.ContainsKey(key))
            {
                chunks.Add(key, new ChunkMeshs(x, z));
                Debug.GetInstance().RenderChunk = chunks.Count;
            }

            return chunks[key] as ChunkMeshs;
        }

        /// <summary>
        /// Внести сетку буфера в чанк
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="buffer"></param>
        public void RenderChank(int x, int z, float[] buffer)
        {
            GetChunk(x, z).MeshDense.Render(buffer);
        }

        /// <summary>
        /// Внести сетку буфера в чанк
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="buffer"></param>
        public void RenderChankAlpha(int x, int z, float[] buffer)
        {
            GetChunk(x, z).MeshAlpha.Render(buffer);
        }

        /// <summary>
        /// Внести сущность
        /// </summary>
        public void RenderEntity(int index, EnumEntity key, float[] buffer)
        {
            if (buffer.Length == 0)
            {
                // Удаление
                entities.Remove(index);
            }
            else
            {
                if (entities.ContainsKey(index))
                {
                    EntityMesh mesh = entities[index] as EntityMesh;
                    mesh.Render(buffer);
                }
                else
                {
                    EntityMesh mesh = new EntityMesh(index, key);
                    mesh.Render(buffer);
                    entities.Add(index, mesh);
                }
            }
        }

        /// <summary>
        /// Удалить дальние чанки из массива кэша сеток
        /// </summary>
        public void Cleaning(vec2i positionCam)
        {
            List<string> vs = new List<string>();
            // дальность чанков с учётом кэша
            int visiblityCache = VE.CHUNK_VISIBILITY + 2;

            int xMin = positionCam.x - visiblityCache;
            int xMax = positionCam.x + visiblityCache;
            int zMin = positionCam.y - visiblityCache;
            int zMax = positionCam.y + visiblityCache;
            // Собираем массив чанков которые уже не попадают в видимость
            foreach (DictionaryEntry s in chunks)
            {
                ChunkMeshs cm = s.Value as ChunkMeshs;
                if (cm.X <= xMin || cm.X >= xMax || cm.Z <= zMin || cm.Z >= zMax)
                {
                    //Debug.GetInstance().CountPoligonChunk -= cm.CountPoligon;
                    OnRemoveChanged(new vec2i(cm.X, cm.Z));
                    cm.Delete();
                    vs.Add(s.Key.ToString());
                }
            }

            // Удаляем
            if (vs.Count > 0)
            {
                foreach (string key in vs)
                {
                    chunks.Remove(key);
                }
            }

            Debug.GetInstance().RenderChunk = chunks.Count;
        }

        /// <summary>
        /// Событие удалена сетка
        /// </summary>
        public event CoordEventHandler RemoveChanged;

        /// <summary>
        /// удалена сетка
        /// </summary>
        protected virtual void OnRemoveChanged(vec2i position)
        {
            RemoveChanged?.Invoke(this, new CoordEventArgs(position));
        }
    }
}
