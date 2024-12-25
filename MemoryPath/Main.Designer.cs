namespace MemoryPath
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            BlockPanel = new Panel();
            GameButton = new Button();
            DifficultyInput = new TextBox();
            Watcher = new Label();
            GY = new Label();
            SuspendLayout();
            // 
            // BlockPanel
            // 
            BlockPanel.BackColor = Color.Transparent;
            BlockPanel.Enabled = false;
            BlockPanel.Location = new Point(2, 2);
            BlockPanel.Name = "BlockPanel";
            BlockPanel.Size = new Size(222, 222);
            BlockPanel.TabIndex = 0;
            // 
            // GameButton
            // 
            GameButton.Location = new Point(76, 228);
            GameButton.Name = "GameButton";
            GameButton.Size = new Size(75, 23);
            GameButton.TabIndex = 1;
            GameButton.Text = "开始游戏";
            GameButton.UseVisualStyleBackColor = true;
            GameButton.Click += GameButton_Click;
            // 
            // DifficultyInput
            // 
            DifficultyInput.Location = new Point(8, 228);
            DifficultyInput.Name = "DifficultyInput";
            DifficultyInput.Size = new Size(62, 23);
            DifficultyInput.TabIndex = 2;
            DifficultyInput.KeyPress += DifficultyInput_KeyPress;
            DifficultyInput.KeyUp += DifficultyInput_KeyUp;
            // 
            // Watcher
            // 
            Watcher.AutoSize = true;
            Watcher.Enabled = false;
            Watcher.ForeColor = Color.Black;
            Watcher.Location = new Point(157, 231);
            Watcher.Name = "Watcher";
            Watcher.Size = new Size(56, 17);
            Watcher.TabIndex = 3;
            Watcher.Text = "点击查看";
            Watcher.Visible = false;
            Watcher.MouseDown += Watcher_MouseDown;
            Watcher.MouseUp += Watcher_MouseUp;
            // 
            // GY
            // 
            GY.AutoSize = true;
            GY.Location = new Point(194, 237);
            GY.Name = "GY";
            GY.Size = new Size(32, 17);
            GY.TabIndex = 4;
            GY.Text = "关于";
            GY.Click += GY_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(226, 256);
            Controls.Add(GY);
            Controls.Add(Watcher);
            Controls.Add(DifficultyInput);
            Controls.Add(GameButton);
            Controls.Add(BlockPanel);
            Enabled = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Main";
            Text = "记忆路径";
            FormClosed += Main_FormClosed;
            Load += Main_Load;
            Shown += Main_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel BlockPanel;
        private Button GameButton;
        private TextBox DifficultyInput;
        private Label Watcher;
        private Label GY;
    }
}
