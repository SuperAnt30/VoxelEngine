using System;
using System.Drawing;
using System.Windows.Forms;
using VoxelEngine.Actions;
using VoxelEngine.World.Blk;

namespace VoxelEngine.Gui
{
    /// <summary>
    /// Контрол инвентарь
    /// </summary>
    public partial class InventoryControl : BaseControl
    {
        public InventoryControl(FormGame form) : base(form)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Запуск
        /// </summary>
        public override void Open()
        {
            Size = new Size(350, 410);
            listView1.Size = new Size(320, 320);
            for (int i = 0; i < Blocks.BLOCKS_COUNT; i++)
            {
                EnumBlock enumBlock = (EnumBlock)i + 1;
                string name = enumBlock.ToString();
                imageList1.Images.Add(name, VES.ImageGui[i]);
                if (enumBlock != EnumBlock.WaterFlowing)
                {
                    ListViewItem listViewItem = new ListViewItem(name, i)
                    {
                        Tag = enumBlock
                    };
                    listView1.Items.Add(listViewItem);
                }
            }

            for (int i = 0; i < 9; i++)
            {
                PictureBox picture = new PictureBox
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(15 + i * 36, 363),
                    Size = new Size(32, 32),
                    Tag = i,
                    TabIndex = i + 2
                };
                EnumBlock enumBlock = PlayerWidget.GetCell(i);
                if (enumBlock != EnumBlock.None)
                {
                    picture.Image = GetImage(enumBlock);
                }
                picture.Click += new EventHandler(pictureBox_Click);
                Controls.Add(picture);
            }
        }

        protected Image GetImage(EnumBlock enumBlock)
        {
            foreach(ListViewItem listViewItem in listView1.Items)
            {
                if ((EnumBlock)listViewItem.Tag == enumBlock)
                {
                    return imageList1.Images[listViewItem.ImageIndex];
                }
            }
            return new Bitmap(16, 16);
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDownClose(e, Keys.Escape, Keys.E, Keys.Enter);
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox picture = sender as PictureBox;
            int index = (int)picture.Tag;
            if (listView1.SelectedItems.Count > 0)
            {
                picture.Image = imageList1.Images[listView1.SelectedItems[0].ImageIndex];
                EnumBlock enumBlock = (EnumBlock)listView1.SelectedItems[0].Tag;
                PlayerWidget.SetCell(index, enumBlock);
            } else
            {
                // Пусто
                picture.Image = new Bitmap(32, 32);
                PlayerWidget.SetCell(index, EnumBlock.None);
            }
        }
    }
}
