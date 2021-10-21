using System.Collections.Generic;
using VoxelEngine.Actions;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.Renderer
{
    /// <summary>
    /// Объект Виджета
    /// </summary>
    public class GuiWidget
    {
        /// <summary>
        /// Ширина экрана
        /// </summary>
        protected int width = 1280;
        /// <summary>
        /// Высота экрана
        /// </summary>
        protected int height = 720;
        /// <summary>
        /// Для виджетов
        /// </summary>
        protected RenderMesh widget = new RenderMesh();
        /// <summary>
        /// Для текстур с атласом
        /// </summary>
        protected RenderMesh atlas = new RenderMesh();
        /// <summary>
        /// Для затемнения экрана перед формами
        /// </summary>
        protected RenderMesh darken = new RenderMesh();
        /// <summary>
        /// центер
        /// </summary>
        protected vec2 center;

        /// <summary>
        /// Изменить размеры экрана
        /// </summary>
        public void Resized(int width, int height)
        {
            this.width = width;
            this.height = height;
            center = new vec2(width / 2f, height / 2f);
            RefreshDraw();
        }

        /// <summary>
        /// Обновыить перерисовку картинки
        /// </summary>
        public void RefreshDraw()
        {
            RefreshDrawWidget();
            RefreshDrawAtlas();
            RefreshDrawDarken();
        }

        /// <summary>
        /// Прорисовать виджет
        /// </summary>
        public void DrawWidget() => widget.Draw();

        /// <summary>
        /// Прорисовать атлас
        /// </summary>
        public void DrawAtlas() => atlas.Draw();

        /// <summary>
        /// Прорисовать затемнения экрана перед формами
        /// </summary>
        public void DrawDarken() => darken.Draw();

        /// <summary>
        /// Обновыить перерисовку виджета
        /// </summary>
        protected void RefreshDrawWidget()
        {
            List<float> buffer = new List<float>();

            // туман под водой
            if (PlayerWidget.IsEyesWater)
            {
                buffer.AddRange(RenderMesh.Rectangle2d(
                new vec2(0), new vec2(width, height),
                new vec2(0.0625f, 0), new vec2(0.1249f, 0.0624f),
                new vec4(.0f, .6f, .9f, 0.7f)));
            }

            // Курсор
            buffer.AddRange(RenderMesh.Rectangle2d(center - 16f, center + 16f, new vec2(0),
                new vec2(0.0624f), new vec4(.9f, .9f, .9f, 0.9f)));

            // перечень инструмента
            buffer.AddRange(RenderMesh.Rectangle2d(
                new vec2(center.x - 182, height - 46), new vec2(center.x + 182, height - 2),
                new vec2(0, 0.0625f), new vec2(0.7109375f, 0.1484375f),
                new vec4(1)));

            // курсор какой инструмент выбран
            int index = PlayerWidget.Index * 40;
            buffer.AddRange(RenderMesh.Rectangle2d(
                new vec2(center.x - 183 + index, height - 48), new vec2(center.x - 135 + index, height),
                new vec2(0, 0.1484375f), new vec2(0.09375f, 0.2421875f),
                new vec4(1)));

            widget.Render(buffer);
        }

        /// <summary>
        /// Обновыить перерисовку атласа
        /// </summary>
        protected void RefreshDrawAtlas()
        {
            vec2 center = new vec2(width / 2f, height / 2f);
            List<float> buffer = new List<float>();

            if (VEC.isDebugTextureAtlas)
            {
                // Видем ли на экране дебаг текстурного атласа
                buffer.AddRange(RenderMesh.Rectangle2d(
                    new vec2(width - 512, 0), new vec2(width, 512),
                    new vec2(0), new vec2(1f),
                    new vec4(1)));
            }

            // Текстурки блоков в ячейках
            for (int i = 0; i < 9; i++)
            {
                EnumBlock enumBlock = PlayerWidget.GetCell(i);
                BlockBase block = Blocks.GetBlock(enumBlock, new Util.BlockPos());
                Box box = block.Boxes[0];
                bool isColor = box.Faces[0].IsColor;
                vec4 color = new vec4(1);
                if (isColor)
                {
                    color = block.IsWater ? new vec4(.3f, .6f, 1f, 1f) : new vec4(0f, .6f, .1f, 1f);
                }
                int numTexture = box.Faces[0].NumberTexture;
                float u1 = (numTexture % 16) * VE.UV_SIZE;
                float v2 = numTexture / 16 * VE.UV_SIZE;

                if (enumBlock != EnumBlock.None)
                {
                    int index = i * 40;
                    buffer.AddRange(RenderMesh.Rectangle2d(
                        new vec2(center.x - 174 + index, height - 38), new vec2(center.x - 146 + index, height - 10),
                        new vec2(u1, v2),
                        new vec2(u1 + VE.UV_SIZE, v2 + VE.UV_SIZE),
                        color));
                }
            }

            atlas.Render(buffer);
        }

        /// <summary>
        /// Обновыить перерисовку затемнения экрана перед формами
        /// </summary>
        public void RefreshDrawDarken()
        {
            List<float> buffer = new List<float>();

            // затемнить экран для GUI
            if (PlayerWidget.IsOpenForm)
            {
                buffer.AddRange(RenderMesh.Rectangle2d(
                    new vec2(0), new vec2(width, height), 0.01f,
                    new vec2(0.015625f, 0.078125f), new vec2(0.07421875f, 0.13671875f),
                    new vec4(1)));
            }

            darken.Render(buffer);
        }
    }
}
