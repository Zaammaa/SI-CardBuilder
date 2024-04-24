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
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            varianceBox = new NumericUpDown();
            label2 = new Label();
            label1 = new Label();
            targetPowerLevelBox = new NumericUpDown();
            generateArtChkBox = new CheckBox();
            tabPage2 = new TabPage();
            numericUpDown1 = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            numericUpDown2 = new NumericUpDown();
            deckNameBox = new TextBox();
            label5 = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)varianceBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)targetPowerLevelBox).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            SuspendLayout();
            // 
            // scanBtn
            // 
            scanBtn.Location = new Point(416, 302);
            scanBtn.Name = "scanBtn";
            scanBtn.Size = new Size(118, 96);
            scanBtn.TabIndex = 0;
            scanBtn.Text = "Scan";
            scanBtn.UseVisualStyleBackColor = true;
            scanBtn.Click += scanBtn_Click;
            // 
            // generateDeckBtn
            // 
            generateDeckBtn.Location = new Point(3, 305);
            generateDeckBtn.Name = "generateDeckBtn";
            generateDeckBtn.Size = new Size(768, 96);
            generateDeckBtn.TabIndex = 1;
            generateDeckBtn.Text = "Generate Deck";
            generateDeckBtn.UseVisualStyleBackColor = true;
            generateDeckBtn.Click += generateDeckBtn_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(6, 6);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(782, 432);
            tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(deckNameBox);
            tabPage1.Controls.Add(numericUpDown1);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(numericUpDown2);
            tabPage1.Controls.Add(varianceBox);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(targetPowerLevelBox);
            tabPage1.Controls.Add(generateArtChkBox);
            tabPage1.Controls.Add(generateDeckBtn);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(774, 404);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // varianceBox
            // 
            varianceBox.DecimalPlaces = 1;
            varianceBox.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            varianceBox.Location = new Point(132, 76);
            varianceBox.Name = "varianceBox";
            varianceBox.Size = new Size(120, 23);
            varianceBox.TabIndex = 6;
            varianceBox.Value = new decimal(new int[] { 1, 0, 0, 65536 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(132, 58);
            label2.Name = "label2";
            label2.Size = new Size(117, 15);
            label2.TabIndex = 5;
            label2.Text = "Power Level Variance";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 58);
            label1.Name = "label1";
            label1.Size = new Size(105, 15);
            label1.TabIndex = 4;
            label1.Text = "Target Power Level";
            // 
            // targetPowerLevelBox
            // 
            targetPowerLevelBox.DecimalPlaces = 1;
            targetPowerLevelBox.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            targetPowerLevelBox.Location = new Point(6, 76);
            targetPowerLevelBox.Name = "targetPowerLevelBox";
            targetPowerLevelBox.Size = new Size(120, 23);
            targetPowerLevelBox.TabIndex = 3;
            targetPowerLevelBox.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // generateArtChkBox
            // 
            generateArtChkBox.AutoSize = true;
            generateArtChkBox.Location = new Point(6, 280);
            generateArtChkBox.Name = "generateArtChkBox";
            generateArtChkBox.Size = new Size(92, 19);
            generateArtChkBox.TabIndex = 2;
            generateArtChkBox.Text = "Generate Art";
            generateArtChkBox.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(scanBtn);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(774, 404);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Increment = new decimal(new int[] { 0, 0, 0, 0 });
            numericUpDown1.Location = new Point(132, 123);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(120, 23);
            numericUpDown1.TabIndex = 10;
            numericUpDown1.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(132, 105);
            label3.Name = "label3";
            label3.Size = new Size(94, 15);
            label3.TabIndex = 9;
            label3.Text = "Max Complexity";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 105);
            label4.Name = "label4";
            label4.Size = new Size(92, 15);
            label4.TabIndex = 8;
            label4.Text = "Min Complexity";
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(6, 123);
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(120, 23);
            numericUpDown2.TabIndex = 7;
            numericUpDown2.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // deckNameBox
            // 
            deckNameBox.Location = new Point(6, 32);
            deckNameBox.Name = "deckNameBox";
            deckNameBox.Size = new Size(100, 23);
            deckNameBox.TabIndex = 11;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 14);
            label5.Name = "label5";
            label5.Size = new Size(68, 15);
            label5.TabIndex = 12;
            label5.Text = "Deck Name";
            // 
            // CardGeneratorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl1);
            Name = "CardGeneratorForm";
            Text = "SI Card Generator";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)varianceBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)targetPowerLevelBox).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button scanBtn;
        private Button generateDeckBtn;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private CheckBox generateArtChkBox;
        private NumericUpDown varianceBox;
        private Label label2;
        private Label label1;
        private NumericUpDown targetPowerLevelBox;
        private NumericUpDown numericUpDown1;
        private Label label3;
        private Label label4;
        private NumericUpDown numericUpDown2;
        private Label label5;
        private TextBox deckNameBox;
    }
}
