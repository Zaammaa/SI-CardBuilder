namespace Spirit_Island_Card_Generator
{
    partial class CardGeneratorForm
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
            scanBtn = new Button();
            generateDeckBtn = new Button();
            SuspendLayout();
            // 
            // scanBtn
            // 
            scanBtn.Location = new Point(326, 204);
            scanBtn.Name = "scanBtn";
            scanBtn.Size = new Size(118, 96);
            scanBtn.TabIndex = 0;
            scanBtn.Text = "Scan";
            scanBtn.UseVisualStyleBackColor = true;
            scanBtn.Click += scanBtn_Click;
            // 
            // generateDeckBtn
            // 
            generateDeckBtn.Location = new Point(560, 204);
            generateDeckBtn.Name = "generateDeckBtn";
            generateDeckBtn.Size = new Size(135, 96);
            generateDeckBtn.TabIndex = 1;
            generateDeckBtn.Text = "Generate Deck";
            generateDeckBtn.UseVisualStyleBackColor = true;
            generateDeckBtn.Click += generateDeckBtn_Click;
            // 
            // CardGeneratorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(generateDeckBtn);
            Controls.Add(scanBtn);
            Name = "CardGeneratorForm";
            Text = "SI Card Generator";
            ResumeLayout(false);
        }

        #endregion

        private Button scanBtn;
        private Button generateDeckBtn;
    }
}
