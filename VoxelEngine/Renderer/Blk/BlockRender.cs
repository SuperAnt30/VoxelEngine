using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Renderer.Chk;
using VoxelEngine.Util;
using VoxelEngine.Vxl;
using VoxelEngine.World.Biome;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;
using VoxelEngine.World.Chk;

namespace VoxelEngine.Renderer.Blk
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
            
            // позиция блока в чанке
            posChunk = new vec3i(Blk.Position.X & 15, Blk.Position.Y, Blk.Position.Z & 15);
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
        /// Получть Сетку блока
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <returns>сетка</returns>
        public float[] RenderMesh()
        {
            
            List<float> buffer = new List<float>();

            //if (Blk.EBlock == EnumBlock.Water 
            //    && ChunkRend.GetVoxel(posChunk + EnumFacing.DirectionVec(Pole.Up)).GetId() == (int)EnumBlock.Water)
            //    //&& !IsBlockedLight(posChunk + EnumFacing.DirectionVec(Pole.Up)).IsDraw)
            //{
            //    Blk.BoxesTwo();
            //}


            //if (Blk.EBlock == EnumBlock.WaterFlowing)
            //{
            //    Blk.Boxes[0].To = new vec3(1f, 1f - (VE.UV_SIZE * Blk.Properties * 3.6f), 1f);
            //}
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
                            RenderMeshSide(buffer, (Pole)i);
                        }
                    }
                    else
                    {
                        RenderMeshSide(buffer, face.Side);
                    }
                }
            }

            return buffer.ToArray();
        }

        /// <summary>
        /// Получть Сетку стороны блока
        /// </summary>
        protected void RenderMeshSide(List<float> buffer, Pole side)
        {
            _br = BlockedLight(posChunk + EnumFacing.DirectionVec(side));
            if (Blk.AllDrawing || _br.IsDraw)
            {
                _side = side;
                buffer.AddRange(_RenderMeshFaceShadow());
            }
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
        /// Определение высот воды
        /// </summary>
        /// <returns></returns>
        public vec4 HeightWater()
        {
            //return new vec4();
            Voxel v = GetVoxel(posChunk.x, posChunk.y + 1, posChunk.z);
            if (v.IsEmpty) return new vec4(1f);
            EnumBlock eBlcok = v.GetEBlock();
            if (Blocks.IsWater(eBlcok))
            {
                // значит над блоком вода
                return new vec4(0f);
            }

            // Если над блоком нет воды, проверяем соседние блоки, для высоты каждого угла
            int a, b, c, d, e, f, g, h;
            int j = BlockedWater();

            a = BlockedWater(posChunk.x + 1, posChunk.y, posChunk.z);
            b = BlockedWater(posChunk.x, posChunk.y, posChunk.z + 1);
            c = BlockedWater(posChunk.x - 1, posChunk.y, posChunk.z);
            d = BlockedWater(posChunk.x, posChunk.y, posChunk.z - 1);

            e = BlockedWater(posChunk.x - 1, posChunk.y, posChunk.z - 1);
            f = BlockedWater(posChunk.x - 1, posChunk.y, posChunk.z + 1);
            g = BlockedWater(posChunk.x + 1, posChunk.y, posChunk.z + 1);
            h = BlockedWater(posChunk.x + 1, posChunk.y, posChunk.z - 1);

            return new vec4(
                (c == -1 || d == -1 || e == -1) ? 0 : j + c + d + e,
                (b == -1 || c == -1 || f == -1) ? 0 : j + b + c + f,
                (a == -1 || b == -1 || g == -1) ? 0 : j + a + b + g,
                (a == -1 || d == -1 || h == -1) ? 0 : j + a + d + h
            ) / 4f;
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
            vec4 color = new vec4(1f);
            if (_face.IsColor)
            {
                if (Blk.IsGrass)
                {
                    EnumBiome biome = ChunkRend.Chunk.GetBiome(Blk.Position);
                    color = BlockColorBiome.Grass(biome);
                }
                else if (Blk.IsLeaves)
                {
                    EnumBiome biome = ChunkRend.Chunk.GetBiome(Blk.Position);
                    color = BlockColorBiome.Leaves(biome);
                }
                else if (Blk.IsWater)
                {
                    EnumBiome biome = ChunkRend.Chunk.GetBiome(Blk.Position);
                    color = BlockColorBiome.Water(biome);
                }
            }
            //_face.IsColor ? Blk.Color : new vec4(1f);
            float l = 1f - _LightPole(_side);
            color.x -= l; if (color.x < 0) color.x = 0;
            color.y -= l; if (color.y < 0) color.y = 0;
            color.z -= l; if (color.z < 0) color.z = 0;
            vec3 col = new vec3(color.x, color.y, color.z);
            vec2 leg;
            if (Blk.LightingYourself)
            {
                // Яркость освещения от себя
                leg = new vec2(((float)Blk.Voxel.GetLightFor(EnumSkyBlock.Block)) / 15f,
                    ((float)Blk.Voxel.GetLightFor(EnumSkyBlock.Sky)) / 15f);
            }
            else
            {
                // Яркость освещения от соседнего блока
                leg = new vec2(((float)_br.LightBlock) / 15f, ((float)_br.LightSky) / 15f);
            }
            //color.w = _br.Light;
            //color.w = (((float)Blk.Voxel.GetLightFor(EnumSkyBlock.Sky)) / 15f);
            //color.w =((float)Mth.Max(_br.LightBlock, _br.LightSky) / 15f);
            //color.w = (((float)_br.LightBlock) / 60f + (float)_br.LightSky / 60f + .25f);// Blk.LightBlock;
            //color.w = (((float)_br.LightSky) / 15f);
            //color.w = ((float)_lightValue / 15f);
            //color.w = 1f;
            vec3 pos = Blk.Position.ToVec3();// + new vec3(0, -0.2f, 0);
            BlockFaceUV blockUV = new BlockFaceUV(col, lg, leg, pos);
            if (Blk.EBlock == EnumBlock.WaterFlowing && _side != Pole.Down)
            {
                vec4 vw = HeightWater() * VE.WATER_LEVEL;
                BlockFaceLiquid blockFaceLiquid = new BlockFaceLiquid(blockUV, Blk, _box, vw, new vec2(u1, v2));
                blockFaceLiquid.RenderMeshSide(_side);
            }
            else
            {
                
                blockUV.SetVecUV(
                    pos + _box.From,
                    pos + _box.To,
                    new vec2(u1, v2 + VE.UV_SIZE),
                    new vec2(u1 + VE.UV_SIZE, v2)
                );

                blockUV.RotateYaw(_box.RotateYaw);
                blockUV.RotatePitch(_box.RotatePitch);
                //if (_box.RotateYaw != 0) 
                //if (Blk.EBlock == EnumBlock.TallGrass)
                //{
                //    blockUV.RotateYaw(_box.RotateYaw);
                //}
            }
            if (_face.IsTwoSides)
            {
                // Слой воды чтоб увидеть снизу
                List<float> ar = new List<float>(blockUV.Side(_side));
                blockUV.SetVecUV(
                    new vec3(pos.x + _box.From.x, pos.y + _box.To.y, pos.z + _box.From.z),
                    new vec3(pos.x + _box.To.x, pos.y + _box.To.y, pos.z + _box.To.z),
                    new vec2(u1, v2 + VE.UV_SIZE),
                    new vec2(u1 + VE.UV_SIZE, v2)
                );
                ar.AddRange(blockUV.Side(Pole.Down));
                return ar.ToArray();
            }
            else
            {
                return blockUV.Side(_side);
            }
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
        /// Поиск вокселя
        /// </summary>
        public Voxel GetVoxel(int x, int y, int z)
        {
            if (y < 0 || y > 255) return new Voxel();

            int xc = ChunkRend.Chunk.X + (x >> 4);
            int zc = ChunkRend.Chunk.Z + (z >> 4);
            int xv = x & 15;
            int zv = z & 15;

            if (xc == ChunkRend.Chunk.X && zc == ChunkRend.Chunk.Z)
            {
                // Соседний блок в этом чанке
                return ChunkRend.Chunk.GetVoxel(x, y, z);
            }
            // Соседний блок в соседнем чанке
            //ChunkRender chunk = null;


            ChunkBase chunkD = ChunkRend.Chunk.World.ChunkPr.ProvideChunk(xc, zc);

            //if (xc == ChunkRend.X - 1 && zc == ChunkRend.Z + 1 && ChunkRend.ChunkWest() != null) chunk = ChunkRend.ChunkWest().ChunkSouth();
            //else if (xc == ChunkRend.X - 1 && zc == ChunkRend.Z - 1 && ChunkRend.ChunkWest() != null) chunk = ChunkRend.ChunkWest().ChunkNorth();
            //else if (xc == ChunkRend.X + 1 && zc == ChunkRend.Z + 1 && ChunkRend.ChunkEast() != null) chunk = ChunkRend.ChunkEast().ChunkSouth();
            //else if (xc == ChunkRend.X + 1 && zc == ChunkRend.Z - 1 && ChunkRend.ChunkEast() != null) chunk = ChunkRend.ChunkEast().ChunkNorth();
            //else if (xc == ChunkRend.X - 1) chunk = ChunkRend.ChunkWest();
            //else if (xc == ChunkRend.X + 1) chunk = ChunkRend.ChunkEast();
            //else if (zc == ChunkRend.Z + 1) chunk = ChunkRend.ChunkSouth();
            //else if (zc == ChunkRend.Z - 1) chunk = ChunkRend.ChunkNorth();



            if (chunkD != null) return chunkD.GetVoxel(xv, y, zv);

            return new Voxel();
        }

        /// <summary>
        /// Проверка блока по координате AmbientOcclusion
        /// </summary>
        public bool IsBlockedAO(int x, int y, int z)
        {
            Voxel v = GetVoxel(x, y, z);
            if (v.IsEmpty) return true;
            return v.GetEBlock() != EnumBlock.Air && Blocks.IsNotTransparent(v.GetEBlock());
        }

        /// <summary>
        /// Проверка блока по параметру
        /// </summary>
        public byte BlockedParam(int x, int y, int z)
        {
            Voxel v = GetVoxel(x, y, z);
            if (v.IsEmpty) return 0;
            return v.GetParam4bit();
        }

        public int BlockedWater()
        {
            return Blk.Properties;
        }

        public int BlockedWater(int x, int y, int z)
        {
            Voxel v = GetVoxel(x, y, z);
            if (v.IsEmpty) return VE.WATER;
            EnumBlock eBlock = v.GetEBlock();
            if (eBlock == EnumBlock.Water) return -1;
            if (eBlock == EnumBlock.WaterFlowing)
            {
                byte b = v.GetParam4bit();
                if (b == 0)
                {
                    // проверяем вверхний блок
                    Voxel v2 = GetVoxel(x, y + 1, z);
                    if (!v2.IsEmpty)
                    {
                        EnumBlock eBlock2 = v2.GetEBlock();
                        if (Blocks.IsWater(eBlock2)) return -1;
                    }
                }
                return b;
            }
            return VE.WATER;
        }

        /// <summary>
        /// Проверка блока по координате для освещения
        /// </summary>
        public BlockedResult BlockedLight(vec3i pos)
        {
            Voxel v = GetVoxel(pos.x, pos.y, pos.z);
            if (v.IsEmpty) return new BlockedResult();

            EnumBlock eBlock = v.GetEBlock();
            BlockedResult br = new BlockedResult()
            {
                IsDraw = eBlock == EnumBlock.Air || !Blocks.IsNotTransparent(eBlock),
                LightSky = v.GetLightFor(EnumSkyBlock.Sky),
                LightBlock = v.GetLightFor(EnumSkyBlock.Block),
                Light = v.GetLightsFor()
            };
            // Для слияния однотипных блоков
            if (br.IsDraw)
            {
                if (eBlock == Blk.EBlock) br.IsDraw = false;
                else if (Blk.IsWater && Blocks.IsWater(eBlock)) br.IsDraw = false;
            }
            return br;
        }
        
    }
}
