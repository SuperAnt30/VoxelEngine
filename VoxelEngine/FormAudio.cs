using System;
using System.Windows.Forms;
using VoxelEngine.Audio;

namespace VoxelEngine
{
    public partial class FormAudio : Form
    {
        public FormAudio()
        {
            InitializeComponent();
        }

        Random random = new Random();
        string[] s = new string[] { "say1", "say2", "say3", "cave2", "hurt1" };
        int count = 0;
        AudioBase audioBase = new AudioBase(new World.WorldBase());

        private void FormAudio_Load(object sender, EventArgs e)
        {
            audioBase.Initialize();
            
            // Запустить тик проверки занятых источников
            timer1.Start();
        }

        /// <summary>
        /// Проиграть звук
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            int i = random.Next(5);
            label1.Text = string.Format("i={0} \"{1}\"", i, s[i]);
            audioBase.PlaySound(s[i], new Glm.vec3(0), 1f, 1f);
        }
        
        /// <summary>
        /// Проиграть массив звуков
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            count = (int)numericUpDown1.Value;
            timer2.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            audioBase.Tick();
            label2.Text = audioBase.StrDebug;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            count--;
            button4_Click(sender, e);
            label3.Text = count.ToString();
            if (count <= 0) timer2.Stop();
        }
    }
}
