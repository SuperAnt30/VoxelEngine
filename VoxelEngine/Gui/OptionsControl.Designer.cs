namespace VoxelEngine.Gui
{
    partial class OptionsControl
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.numericUpDownFPS = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownChunk = new System.Windows.Forms.NumericUpDown();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChunk)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownFPS
            // 
            this.numericUpDownFPS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFPS.Location = new System.Drawing.Point(122, 15);
            this.numericUpDownFPS.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownFPS.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownFPS.Name = "numericUpDownFPS";
            this.numericUpDownFPS.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownFPS.TabIndex = 0;
            this.numericUpDownFPS.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Кадры в сек FPS:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = " Обзор чанков:";
            // 
            // numericUpDownChunk
            // 
            this.numericUpDownChunk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownChunk.Location = new System.Drawing.Point(122, 41);
            this.numericUpDownChunk.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericUpDownChunk.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDownChunk.Name = "numericUpDownChunk";
            this.numericUpDownChunk.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownChunk.TabIndex = 3;
            this.numericUpDownChunk.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // buttonOk
            // 
            this.buttonOk.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.buttonOk.Location = new System.Drawing.Point(15, 77);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "Применить";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(112, 77);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // OptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.numericUpDownChunk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownFPS);
            this.Name = "OptionsControl";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Size = new System.Drawing.Size(202, 115);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChunk)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownFPS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownChunk;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
    }
}
