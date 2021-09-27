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
        uint sourceid = 0;
        uint sourceid2 = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            play(sourceid, 5);
            play(sourceid2, 0);

            int errorCode = Al.alGetError();
            label1.Text = string.Format("{0} {1} Er {2}", sourceid, sourceid2, errorCode);
        }

        protected void play(uint id, float x)
        {
            // тон звука
            Al.alSourcef(id, Al.AL_PITCH, 1.0f);
            // усиление звука. Этот параметр влияет на то, как будет изменяться сила звука, по мере изменения расстояния от источника до слушателя
            Al.alSourcef(id, Al.AL_GAIN, 0.99f);
            // позиция источника в трёхмерных координатах.
            Al.alSource3f(id, Al.AL_POSITION, x, 0, 0);
            // скорость движения звука. 
            // Работает это параметр не так как можно предположить изначально. 
            // Если вы установите этот параметр в какое-то значение, то при выполнении программы, 
            // ваш звук не будет двигаться согласно скорости заданной этим параметром. 
            // Этот параметр используется, всего лишь, как контейнер значения скорости, 
            // использовать который вы можете, как захотите.
            //Al.alSourcefv(id, Al.L_VELOCITY, mVel);
            // значение этого параметра определяет, будет ли ваш звук зациклен.
            //Al.alSourcei(id, Al.AL_LOOPING, mLooped);
            Al.alSourcePlay(id);
        }


        private void FormAudio_Load(object sender, EventArgs e)
        {
            IntPtr pDevice = Al.alcOpenDevice(null);
            IntPtr pContext = Al.alcCreateContext(pDevice, null);
            Al.alcMakeContextCurrent(pContext);

            //sourceid = Ogg();
            //sourceid2 = Wave();

            //dd();
        }

        protected uint Wave()
        {
            AudioLoad audio = new AudioLoad();
            audio.LoadWave(@"sound\cave2.wav");
            return Sample(audio, out uint bid);
        }

        protected uint Ogg()
        {
            AudioLoad audio = new AudioLoad();
            audio.LoadOgg(@"sound\say3.ogg");
            return Sample(audio, out uint bid);
        }

        /// <summary>
        /// Задать сэмпл
        /// </summary>
        /// <param name="audio">Объект загрузки сэмплов</param>
        /// <returns>id источника</returns>
        protected uint Sample(AudioLoad audio, out uint bid)
        {
            Al.alGenSources(1, out uint sid);
            Al.alGenBuffers(1, out bid);
            Al.alBufferData(bid, audio.AlFormat, audio.Buffer, audio.Size, audio.SamplesPerSecond);
            Al.alSourcei(sid, Al.AL_BUFFER, (int)bid);
            audio.Clear();
            return sid;
        }

        /// <summary>
        /// Задать сэмпл
        /// </summary>
        /// <param name="audio">Объект загрузки сэмплов</param>
        /// <returns>id источника</returns>
        protected void Sample(AudioLoad audio, int i)
        {
            Al.alBufferData(idbs[i], audio.AlFormat, audio.Buffer, audio.Size, audio.SamplesPerSecond);
            Al.alSourcei(ids[i], Al.AL_BUFFER, (int)idbs[i]);
            audio.Clear();
        }

        protected void dd()
        {
            for (int i = 0; i < 5; i++)
            {
                Al.alGenSources(1, out uint sid);
                ids[i] = sid;
                Al.alGenBuffers(1, out uint bid);
                idbs[i] = bid;
            }
        }

        uint[] ids = new uint[5];
        uint[] idbs = new uint[5];

        private void button2_Click(object sender, EventArgs e)
        {
            // тест цикл

            //string[] s = new string[] { "say1", "say2", "say3", "cave2", "hurt1" };
            string[] s = new string[] { "say1", "say2", "hurt1" };
            //uint[] ids = new uint[s.Length];
            //uint[] idbs = new uint[s.Length];


            for (int j = 0; j < 500; j++)
            {

                for (int i = 0; i < s.Length; i++)
                {
                    AudioLoad audio = new AudioLoad();
                    audio.LoadOgg("sound\\" + s[i] + ".ogg");
                    Sample(audio, i);
                    play(ids[i], 0);
                }
                System.Threading.Thread.Sleep(1000);

                //for (int i = 0; i < s.Length; i++)
                //{
                //    Al.alGetSourcei(ids[i], Al.AL_BUFFERS_PROCESSED, out int value);
                //    if (value != 0)
                //    {
                //        uint buf = 0;
                //        Al.alDeleteBuffers((int)idbs[i], ref buf);
                //        if (buf == 0) { }
                //    }
                //}
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Максимально буферов

            label2.Text = "Loading...";
            int count = 0;
            int errorCode = 0;
            for (int i = 0; i < 1000; i++)
            {
                Al.alGenSources(1, out uint sid);
                errorCode = Al.alGetError();
                // По ошибке определяем источник и буфер обмена
                if (Al.alIsSource(sid) && errorCode == 0)
                {
                    //ids[i] = sid;
                    Al.alGenBuffers(1, out uint bid);
                    errorCode = Al.alGetError();
                    if (!Al.alIsBuffer(bid) || errorCode != 0)
                    {
                        
                        break;
                    }
                    else
                    {
                        count++;
                    }
                    //idbs[i] = bid;
                }
                else
                {
                    break;
                }
                
            }
            label2.Text = count.ToString() + " " + errorCode.ToString();
        }
    }
}
