using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Объект рендера блока
    /// </summary>
    public class BlockRender
    {
        /// <summary>
        /// Объект рендера чанков
        /// </summary>
        public ChunkRender ChunkRend { get; protected set; }
        /// <summary>
        /// Объект блока
        /// </summary>
        public Block Blk { get; protected set; }

        public BlockRender(ChunkRender chunkRender, Block block)
        {
            ChunkRend = chunkRender;
            Blk = block;
        }

        /// <summary>
        /// позиция блока в чанке
        /// </summary>
        protected vec3i posChunk;
        /// <summary>
        /// Текущая коробка
        /// </summary>
        protected Box _box;
        /// <summary>
        /// Текущая сторона блока
        /// </summary>
        protected Face _face;
        /// <summary>
        /// Направление
        /// </summary>
        protected Pole _side;

        protected BlockedResult _br;

        /// <summary>
        /// яркость дневного соседнего блока
        /// </summary>
        //protected byte _lightValue;

        /// <summary>
        /// Получть Сетку блока
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <returns>сетка</returns>
        public float[] RenderMesh()
        {
            // позиция блока в чанке
            posChunk = new vec3i(Blk.Position.X & 15, Blk.Position.Y, Blk.Position.Z & 15);
            List<float> buffer = new List<float>();

            if (Blk.EBlock == EnumBlock.Water 
                && ChunkRend.GetVoxel(posChunk + EnumFacing.DirectionVec(Pole.Up)).GetId() == (int)EnumBlock.Water)
                //&& !IsBlockedLight(posChunk + EnumFacing.DirectionVec(Pole.Up)).IsDraw)
            {
                Blk.BoxesTwo();
            }

            
            if (Blk.EBlock == EnumBlock.WaterFlowing)
            {
                Blk.Boxes[0].To = new vec3(1f, 1f - (VE.UV_SIZE * Blk.Properties * 4f), 1f);
            }
            foreach (Box box in Blk.Boxes)
            {
                _box = box;
                foreach (Face face in box.Faces)
                {
                    _face = face;
                    if (face.Side == Pole.All)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            _br = IsBlockedLight(posChunk + EnumFacing.DirectionVec((Pole)i));
                           // _br.Voxel
                            if (Blk.AllDrawing || _br.IsDraw)
                            //if (Blk.AllDrawing || _bl.LightPassing)
                            {
                                _side = (Pole)i;
                                buffer.AddRange(_RenderMeshFaceShadow());
                            }
                        }
                    }
                    else
                    {
                        //_bl = ChunkRend.GetBlock(Blk.Position.ToVec3i() + EnumFacing.DirectionVec(face.Side));
                        _br = IsBlockedLight(posChunk + EnumFacing.DirectionVec(face.Side));
                        if (Blk.AllDrawing || _br.IsDraw)
                        //if (Blk.AllDrawing || _bl.LightPassing)
                        {
                            _side = face.Side;
                            buffer.AddRange(_RenderMeshFaceShadow());
                        }
                    }
                }
            }

            return buffer.ToArray();
        }

        /// <summary>
        /// Получть Сетку стороны блока c тенью
        /// </summary>
        /// <param name="posChunk">позиция блока в чанке</param>
        /// <param name="pos">позиция блока</param>
        /// <param name="side">Сторона блока</param>
        /// <param name="face">Объект стороны</param>
        /// <param name="box">Объект коробки</param>
        /// <param name="lightBlock">яркость дневного света стороны блока</param>
        /// <returns>сетка</returns>
        protected float[] _RenderMeshFaceShadow()
        {
            // Параметр затемнение одного угла к блоку
            float aoFactor = 0.2f;
            // Параметры углов
            float a, b, c, d, e, f, g, h;
            // Вершины затемнения
            vec4 lg = new vec4(1.0f);

            if (VEC.GetInstance().AmbientOcclusion)
            {
                switch (_side)
                {
                    case Pole.Up:
                        a = IsBlockedAO(posChunk.x + 1, posChunk.y + 1, posChunk.z) ? aoFactor : 0;
                        b = IsBlockedAO(posChunk.x, posChunk.y + 1, posChunk.z + 1) ? aoFactor : 0;
                        c = IsBlockedAO(posChunk.x - 1, posChunk.y + 1, posChunk.z) ? aoFactor : 0;
                        d = IsBlockedAO(posChunk.x, posChunk.y + 1, posChunk.z - 1) ? aoFactor : 0;

                        e = IsBlockedAO(posChunk.x - 1, posChunk.y + 1, posChunk.z - 1) ? aoFactor : 0;
                        f = IsBlockedAO(posChunk.x - 1, posChunk.y + 1, posChunk.z + 1) ? aoFactor : 0;
                        g = IsBlockedAO(posChunk.x + 1, posChunk.y + 1, posChunk.z + 1) ? aoFactor : 0;
                        h = IsBlockedAO(posChunk.x + 1, posChunk.y + 1, posChunk.z - 1) ? aoFactor : 0;
                        lg.x -= c + d + e;
                        lg.y -= c + b + f;
                        lg.z -= a + b + g;
                        lg.w -= a + d + h;
                        break;
                    case Pole.Down:
                        a = IsBlockedAO(posChunk.x + 1, posChunk.y - 1, posChunk.z) ? aoFactor : 0;
                        b = IsBlockedAO(posChunk.x, posChunk.y - 1, posChunk.z + 1) ? aoFactor : 0;
                        c = IsBlockedAO(posChunk.x - 1, posChunk.y - 1, posChunk.z) ? aoFactor : 0;
                        d = IsBlockedAO(posChunk.x, posChunk.y - 1, posChunk.z - 1) ? aoFactor : 0;

                        e = IsBlockedAO(posChunk.x - 1, posChunk.y - 1, posChunk.z - 1) ? aoFactor : 0;
                        f = IsBlockedAO(posChunk.x - 1, posChunk.y - 1, posChunk.z + 1) ? aoFactor : 0;
                        g = IsBlockedAO(posChunk.x + 1, posChunk.y - 1, posChunk.z + 1) ? aoFactor : 0;
                        h = IsBlockedAO(posChunk.x + 1, posChunk.y - 1, posChunk.z - 1) ? aoFactor : 0;
                        lg.x -= c + d + e;
                        lg.y -= a + b + g;
                        lg.z -= c + b + f;
                        lg.w -= a + d + h;
                        break;
                    case Pole.East:
                        a = IsBlockedAO(posChunk.x + 1, posChunk.y + 1, posChunk.z) ? aoFactor : 0;
                        b = IsBlockedAO(posChunk.x + 1, posChunk.y, posChunk.z + 1) ? aoFactor : 0;
                        c = IsBlockedAO(posChunk.x + 1, posChunk.y - 1, posChunk.z) ? aoFactor : 0;
                        d = IsBlockedAO(posChunk.x + 1, posChunk.y, posChunk.z - 1) ? aoFactor : 0;

                        e = IsBlockedAO(posChunk.x + 1, posChunk.y - 1, posChunk.z - 1) ? aoFactor : 0;
                        f = IsBlockedAO(posChunk.x + 1, posChunk.y - 1, posChunk.z + 1) ? aoFactor : 0;
                        g = IsBlockedAO(posChunk.x + 1, posChunk.y + 1, posChunk.z + 1) ? aoFactor : 0;
                        h = IsBlockedAO(posChunk.x + 1, posChunk.y + 1, posChunk.z - 1) ? aoFactor : 0;
                        lg.x -= c + d + e;
                        lg.y -= d + a + h;
                        lg.z -= a + b + g;
                        lg.w -= b + c + f;
                        break;
                    case Pole.West:
                        a = IsBlockedAO(posChunk.x - 1, posChunk.y + 1, posChunk.z) ? aoFactor : 0;
                        b = IsBlockedAO(posChunk.x - 1, posChunk.y, posChunk.z + 1) ? aoFactor : 0;
                        c = IsBlockedAO(posChunk.x - 1, posChunk.y - 1, posChunk.z) ? aoFactor : 0;
                        d = IsBlockedAO(posChunk.x - 1, posChunk.y, posChunk.z - 1) ? aoFactor : 0;

                        e = IsBlockedAO(posChunk.x - 1, posChunk.y - 1, posChunk.z - 1) ? aoFactor : 0;
                        f = IsBlockedAO(posChunk.x - 1, posChunk.y - 1, posChunk.z + 1) ? aoFactor : 0;
                        g = IsBlockedAO(posChunk.x - 1, posChunk.y + 1, posChunk.z + 1) ? aoFactor : 0;
                        h = IsBlockedAO(posChunk.x - 1, posChunk.y + 1, posChunk.z - 1) ? aoFactor : 0;
                        lg.x -= c + d + e;
                        lg.y -= a + b + g;
                        lg.z -= d + a + h;
                        lg.w -= b + c + f;
                        break;
                    case Pole.South:
                        a = IsBlockedAO(posChunk.x, posChunk.y + 1, posChunk.z + 1) ? aoFactor : 0;
                        b = IsBlockedAO(posChunk.x + 1, posChunk.y, posChunk.z + 1) ? aoFactor : 0;
                        c = IsBlockedAO(posChunk.x, posChunk.y - 1, posChunk.z + 1) ? aoFactor : 0;
                        d = IsBlockedAO(posChunk.x - 1, posChunk.y, posChunk.z + 1) ? aoFactor : 0;

                        e = IsBlockedAO(posChunk.x - 1, posChunk.y - 1, posChunk.z + 1) ? aoFactor : 0;
                        f = IsBlockedAO(posChunk.x + 1, posChunk.y - 1, posChunk.z + 1) ? aoFactor : 0;
                        g = IsBlockedAO(posChunk.x + 1, posChunk.y + 1, posChunk.z + 1) ? aoFactor : 0;
                        h = IsBlockedAO(posChunk.x - 1, posChunk.y + 1, posChunk.z + 1) ? aoFactor : 0;
                        lg.x -= c + d + e;
                        lg.y -= a + b + g;
                        lg.z -= a + d + h;
                        lg.w -= b + c + f;
                        break;
                    case Pole.North:
                        a = IsBlockedAO(posChunk.x, posChunk.y + 1, posChunk.z - 1) ? aoFactor : 0;
                        b = IsBlockedAO(posChunk.x + 1, posChunk.y, posChunk.z - 1) ? aoFactor : 0;
                        c = IsBlockedAO(posChunk.x, posChunk.y - 1, posChunk.z - 1) ? aoFactor : 0;
                        d = IsBlockedAO(posChunk.x - 1, posChunk.y, posChunk.z - 1) ? aoFactor : 0;

                        e = IsBlockedAO(posChunk.x - 1, posChunk.y - 1, posChunk.z - 1) ? aoFactor : 0;
                        f = IsBlockedAO(posChunk.x + 1, posChunk.y - 1, posChunk.z - 1) ? aoFactor : 0;
                        g = IsBlockedAO(posChunk.x + 1, posChunk.y + 1, posChunk.z - 1) ? aoFactor : 0;
                        h = IsBlockedAO(posChunk.x - 1, posChunk.y + 1, posChunk.z - 1) ? aoFactor : 0;
                        lg.x -= c + d + e;
                        lg.y -= a + d + h;
                        lg.z -= a + b + g;
                        lg.w -= b + c + f;
                        break;
                }
            }
            return _RenderMeshFace(lg);
        }

        /// <summary>
        /// Генерация сетки стороны коробки
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <param name="side">направление блока</param>
        /// <param name="face">объект стороны блока</param>
        /// <param name="box">объект коробки</param>
        /// <param name="lightValue">яркость дневного соседнего блока</param>
        /// <returns></returns>
        protected float[] _RenderMeshFace(vec4 lg)
        {
            float u1 = (_face.NumberTexture % 16) * VE.UV_SIZE;
            float v2 = _face.NumberTexture / 16 * VE.UV_SIZE;
            vec4 color = _face.IsColor ? Blk.Color : new vec4(1f);
            float l = 1f - _LightPole(_side);
            color.x -= l; if (color.x < 0) color.x = 0;
            color.y -= l; if (color.y < 0) color.y = 0;
            color.z -= l; if (color.z < 0) color.z = 0;
            vec3 col = new vec3(color.x, color.y, color.z);
            vec2 leg = new vec2(((float)_br.LightBlock) / 15f, ((float)_br.LightSky) / 15f);
            //color.w = _br.Light;
            //color.w = (((float)Blk.Voxel.GetLightFor(EnumSkyBlock.Sky)) / 15f);
            //color.w =((float)Mth.Max(_br.LightBlock, _br.LightSky) / 15f);
            //color.w = (((float)_br.LightBlock) / 60f + (float)_br.LightSky / 60f + .25f);// Blk.LightBlock;
            //color.w = (((float)_br.LightSky) / 15f);
            //color.w = ((float)_lightValue / 15f);
            //color.w = 1f;
            BlockFaceUV blockUV = new BlockFaceUV(
                new vec3(Blk.Position.X + _box.From.x, Blk.Position.Y + _box.From.y, Blk.Position.Z + _box.From.z),
                new vec3(Blk.Position.X + _box.To.x, Blk.Position.Y + _box.To.y, Blk.Position.Z + _box.To.z),
                new vec2(u1, v2 + VE.UV_SIZE),
                new vec2(u1 + VE.UV_SIZE, v2),
                col, lg, leg
            );
            return blockUV.Side(_side);
        }

        /// <summary>
        /// Затемнение стороны от стороны блока
        /// </summary>
        protected float _LightPole(Pole side)
        {
            switch (side)
            {
                case Pole.Up: return 1f;
                //case Pole.South: return 0.8f;
                //case Pole.East: return 0.6f;
                //case Pole.West: return 0.6f;
                //case Pole.North: return 0.8f;
                case Pole.South: return 0.9f;
                case Pole.East: return 0.8f;
                case Pole.West: return 0.8f;
                case Pole.North: return 0.9f;
            }
            return 0.7f;// 0.5f;
        }


        public struct BlockedResult
        {
            /// <summary>
            /// Прорисовывать ли сторону
            /// </summary>
            public bool IsDraw;
//            public Voxel Voxel;
            /// <summary>
            /// Яркость света неба
            /// </summary>
            public byte LightSky;
            /// <summary>
            /// Яркость света от блока
            /// </summary>
            public byte LightBlock;
            /// <summary>
            /// Общаяя яркость света неба и блока в одном байте
            /// </summary>
            public byte Light;

        }
        /// <summary>
        /// Проверка блока по координате для освещения
        /// </summary>
        public BlockedResult IsBlockedLight(vec3i pos)
        {
            if (pos.y < 0 || pos.y > 255) return new BlockedResult();

            int xc = ChunkRend.X + (pos.x >> 4);
            int zc = ChunkRend.Z + (pos.z >> 4);
            int xv = pos.x & 15;
            int zv = pos.z & 15;
            Voxel v = new Voxel();
            if (xc == ChunkRend.X && zc == ChunkRend.Z)
            {
                // Соседний блок в этом чанке
                v = ChunkRend.GetVoxel(pos);
                //Blocks.GetBlockLightOpacity(id)
                //v = ChunkRend.Voxels[pos.y, pos.x, pos.z];
                //v = ChunkRend.Voxels[pos.y << 8 | pos.z << 4 | pos.x];
            }
            else
            {
                // Соседний блок в соседнем чанке
                ChunkRender chunk = null;
                if (xc == ChunkRend.X - 1) chunk = ChunkRend.ChunkWest();
                else if (xc == ChunkRend.X + 1) chunk = ChunkRend.ChunkEast();
                else if (zc == ChunkRend.Z + 1) chunk = ChunkRend.ChunkSouth();
                else if (zc == ChunkRend.Z - 1) chunk = ChunkRend.ChunkNorth();
                if (chunk != null) v = chunk.GetVoxel(xv, pos.y, zv);// chunk.Voxels[pos.y, xv, zv];
                //if (chunk != null) v = chunk.Voxels[pos.y << 8 | zv << 4 | xv];
            }
            if (v.IsEmpty()) return new BlockedResult();

            byte id = v.GetId();
            BlockedResult br = new BlockedResult()
            {
                //Voxel = v,
                //IsDraw = v.GetId() == 0 || !v.IsNotTransparent(),
                //IsDraw = id == 0 || !v.IsNotTransparent(),
                IsDraw = id == 0 || !Blocks.IsNotTransparent(id),
                LightSky = v.GetLightFor(EnumSkyBlock.Sky),
                LightBlock = v.GetLightFor(EnumSkyBlock.Block),
                Light = v.GetLightsFor()
            };
            if (br.IsDraw && id == Blk.Id) br.IsDraw = false;
            return br;
            ////if (Is)

            //return v.IsEmpty()
            //    ? new BlockedResult()
            //    : new BlockedResult()
            //    {
            //        //Voxel = v,
            //        //IsDraw = v.GetId() == 0 || !v.IsNotTransparent(),
            //        IsDraw = v.GetId() == 0 || !v.IsNotTransparent() || v.GetId() != Blk.Id,
            //        LightSky = v.GetLightFor(EnumSkyBlock.Sky),
            //        LightBlock = v.GetLightFor(EnumSkyBlock.Block),
            //        Light = v.GetLightsFor()
            //    };
        }

        /// <summary>
        /// Проверка блока по координате AmbientOcclusion
        /// </summary>
        public bool IsBlockedAO(int x, int y, int z)
        {
            if (y < 0 || y > 255) return true; // true чтоб не видно было

            int xc = ChunkRend.X + (x >> 4);
            int zc = ChunkRend.Z + (z >> 4);
            int xv = x & 15;
            int zv = z & 15;
            Voxel v = new Voxel();
            if (xc == ChunkRend.X && zc == ChunkRend.Z)
            {
                // Соседний блок в этом чанке
                //v = ChunkRend.Voxels[y, x, z];
                v = ChunkRend.GetVoxel(x, y, z);
                //v = ChunkRend.Voxels[y << 8 | z << 4 | x];
            }
            else
            {
                // Соседний блок в соседнем чанке
                ChunkRender chunk = null;
                if (xc == ChunkRend.X - 1 && zc == ChunkRend.Z + 1 && ChunkRend.ChunkWest() != null) chunk = ChunkRend.ChunkWest().ChunkSouth();
                else if (xc == ChunkRend.X - 1 && zc == ChunkRend.Z - 1 && ChunkRend.ChunkWest() != null) chunk = ChunkRend.ChunkWest().ChunkNorth();
                else if (xc == ChunkRend.X + 1 && zc == ChunkRend.Z + 1 && ChunkRend.ChunkEast() != null) chunk = ChunkRend.ChunkEast().ChunkSouth();
                else if (xc == ChunkRend.X + 1 && zc == ChunkRend.Z - 1 && ChunkRend.ChunkEast() != null) chunk = ChunkRend.ChunkEast().ChunkNorth();
                else if (xc == ChunkRend.X - 1) chunk = ChunkRend.ChunkWest();
                else if (xc == ChunkRend.X + 1) chunk = ChunkRend.ChunkEast();
                else if (zc == ChunkRend.Z + 1) chunk = ChunkRend.ChunkSouth();
                else if (zc == ChunkRend.Z - 1) chunk = ChunkRend.ChunkNorth();

                if (chunk != null) v = chunk.GetVoxel(xv, y, zv); //chunk.Voxels[y, xv, zv];
                //if (chunk != null) v = chunk.Voxels[y << 8 | zv << 4 | xv];
            }

            return v.IsEmpty()
                ? true
                //: (v.GetId() != 0 && v.IsNotTransparent());
                : (v.GetId() != 0 && Blocks.IsNotTransparent(v.GetId()));
        }
        /*
        /// <summary>
        /// Проверка блока по координате
        /// </summary>
        public bool IsBlockedOld(vec3i pos)
        {
            if (pos.y < 0 || pos.y >= VE.CHUNK_HEIGHT) return true; // true чтоб не видно было
            int xc = ChunkRend.X;
            int zc = ChunkRend.Z;
            int xv = pos.x;
            int zv = pos.z;
            bool isOtherChunk = false;
            if (pos.x == -1)
            {
                xv = VE.CHUNK_WIDTH - 1;
                xc--;
                isOtherChunk = true;
            }
            else if (pos.x == VE.CHUNK_WIDTH)
            {
                xv = 0;
                xc++;
                isOtherChunk = true;
            }
            if (pos.z == -1)
            {
                zv = VE.CHUNK_WIDTH - 1;
                zc--;
                isOtherChunk = true;
            }
            else if (pos.z == VE.CHUNK_WIDTH)
            {
                zv = 0;
                zc++;
                isOtherChunk = true;
            }
            //Block blockBeside = null;
            // short id = 0;

            if (isOtherChunk)
            {
                //return true;
                // Соседний блок в соседнем чанке
                ChunkRender chunk = ChunkRend.World.GetChunk(xc, zc);
                if (chunk != null)
                {
                    //short v = chunk.GetVoxel2(xv, pos.y, zv);
                    //return (byte)(v >> 8) != 0 && (byte)((v & 0xF0) >> 4) == 0;
                    Voxel v = chunk.Voxels[pos.y, xv, zv];
                    //if (v.Id == 0) return false;
                    //if (v.GetLightPassing() == 1) id = v.Id;
                    //id = v.GetLightPassing() == 1;
                    return v.GetId() != 0 && v.GetLightPassing() == 0;




                    //Voxel voxel = chunk.GetVoxel(xv, pos.y, zv);
                    //if (voxel.Id == 0) return false;
                    ////return !chunk.Voxels[pos.y, xv, zv].LP;
                    //return true;
                    //return true;// !chunk.VoxelsLP[pos.y, xv, zv];

                    //return voxel.Id != 0 && voxel.B1 == 0;

                    //return chunk.GetVoxel(xv, y, zv).Id != 0 && chunk.GetVoxel(xv, y, zv).B1 == 0;
                    //blockBeside = Blocks.GetBlock(chunk.GetVoxel(xv, pos.y, zv), new vec3i(xv, pos.y, zv));
                }
            }
            //else
            {
                // Соседний блок в этом чанке

                //return (byte)(Voxels[(y * VE.CHUNK_WIDTH + zv) * VE.CHUNK_WIDTH + xv] >> 8) != 0
                //    && (byte)((Voxels[(y * VE.CHUNK_WIDTH + zv) * VE.CHUNK_WIDTH + xv] & 0xF0) >> 4) == 0;
                // blockBeside = Block;
                //                return Block.

                //Vox v = ChunkRend.Voxels[pos.y, xv, zv];
                //return v.Id != 0 && v.GetLightPassing() == 0;

                Voxel v = ChunkRend.Voxels[pos.y, xv, zv];
                //if (v.Id == 0) return false;
                //if (v.GetLightPassing() == 1) id = v.Id;
                return v.GetId() != 0 && v.GetLightPassing() == 0;

                //v.GetId() != 0 && v.GetLightPassing() == 0;


                //Voxel voxel = ChunkRend.GetVoxel(xv, pos.y, zv);
                //if (voxel.Id == 0) return false;
                ////return !ChunkRend.Voxels[pos.y, xv, zv].LP;
                //return true;
                //return !Blocks.GetBlock(voxel).LightPassing;
                //return voxel.Id != 0 && voxel.B1 == 0;

                //return (byte)(ChunkRend.Voxels[pos.y, xv, zv] >> 8) != 0
                //    && (byte)((ChunkRend.Voxels[pos.y, xv, zv] & 0xF0) >> 4) == 0;


                // TODO:: ускорение, создание объекта block сильно тормазит
                //return (byte)(Voxels[(y * VE.CHUNK_WIDTH + zv) * VE.CHUNK_WIDTH + xv] >> 8) != 0;
                //Voxel vox = GetVoxel(xv, y, zv);

                //Voxels[(y * VE.CHUNK_WIDTH + z) * VE.CHUNK_WIDTH + x]
                //return vox.Id != 0;
                //// Соседний блок в этом чанке
                //blockBeside = Blocks.GetBlock(ChunkRend.GetVoxel(xv, pos.y, zv), new vec3i(xv, pos.y, zv));
            }
            //if (blockBeside == null)
            //return false;

            //if (id != 0)
            //{
            //    Block.Id 
            //}

            //// Если блок прозрачный
            //if (blockBeside.LightPassing)
            //{
            //    return Block.Id == blockBeside.Id; // TODO:: сделать определение по типу блока 
            //} 
            //return true;
        }*/
    }
}
