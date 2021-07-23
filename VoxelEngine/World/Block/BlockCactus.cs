﻿using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Блок кактуса
    /// </summary>
    public class BlockCactus : Block
    {
        public BlockCactus()
        {
            Boxes = new Box[] {
                new Box()
                {
                    Faces = new Face[]
                    {
                        new Face(Pole.Up, 18),
                        new Face(Pole.Down, 16),
                    }
                },
                new Box()
                {
                    From = new vec3(0, 0, VE.UV_SIZE),
                    To = new vec3(1f, 1f, 1f - VE.UV_SIZE),
                    Faces = new Face[]
                    {
                        new Face(Pole.North, 17),
                        new Face(Pole.South, 17),
                    }
                },
                new Box()
                {
                    From = new vec3(VE.UV_SIZE, 0, 0),
                    To = new vec3(1f - VE.UV_SIZE, 1f, 1f),
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 17),
                        new Face(Pole.West, 17)
                    }
                }
            };
            //LightOpacity = 1;
            AllDrawing = true;
            //IsAlphe = true;
        }
    }
}