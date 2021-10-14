namespace VoxelEngine
{
    partial class FormGame
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.openGLControl1 = new SharpGL.OpenGLControl();
            this.guiControl1 = new VoxelEngine.Gui.GuiControl();
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // openGLControl1
            // 
            this.openGLControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.openGLControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openGLControl1.DrawFPS = false;
            this.openGLControl1.FrameRate = 64;
            this.openGLControl1.Location = new System.Drawing.Point(0, 0);
            this.openGLControl1.Name = "openGLControl1";
            this.openGLControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL3_3;
            this.openGLControl1.RenderContextType = SharpGL.RenderContextType.NativeWindow;
            this.openGLControl1.RenderTrigger = SharpGL.RenderTrigger.Manual;
            this.openGLControl1.Size = new System.Drawing.Size(1264, 681);
            this.openGLControl1.TabIndex = 43;
            this.openGLControl1.OpenGLInitialized += new System.EventHandler(this.openGLControl1_OpenGLInitialized);
            this.openGLControl1.OpenGLDraw += new SharpGL.RenderEventHandler(this.openGLControl1_OpenGLDraw);
            this.openGLControl1.Resized += new System.EventHandler(this.openGLControl1_Resized);
            this.openGLControl1.Click += new System.EventHandler(this.openGLControl1_Click);
            this.openGLControl1.Enter += new System.EventHandler(this.openGLControl1_Enter);
            this.openGLControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.openGLControl1_KeyDown);
            this.openGLControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.openGLControl1_KeyUp);
            this.openGLControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseDown);
            this.openGLControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseMove);
            this.openGLControl1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.openGLControl1_PreviewKeyDown);
            // 
            // guiControl1
            // 
            this.guiControl1.BackColor = System.Drawing.SystemColors.Control;
            this.guiControl1.Location = new System.Drawing.Point(100, 100);
            this.guiControl1.Name = "guiControl1";
            this.guiControl1.Size = new System.Drawing.Size(390, 272);
            this.guiControl1.TabIndex = 44;
            this.guiControl1.VisibleChanged += new System.EventHandler(this.guiControl1_VisibleChanged);
            // 
            // FormGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.guiControl1);
            this.Controls.Add(this.openGLControl1);
            this.Name = "FormGame";
            this.Text = "Test by SuperAnt";
            this.Deactivate += new System.EventHandler(this.FormGame_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGame_FormClosing);
            this.Load += new System.EventHandler(this.FormGame_Load);
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SharpGL.OpenGLControl openGLControl1;
        private Gui.GuiControl guiControl1;
    }
}

