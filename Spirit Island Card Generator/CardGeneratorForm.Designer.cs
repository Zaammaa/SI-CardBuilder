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
            generateDeckBtn = new Button();
            tabControl1 = new TabControl();
            generateDeckPage = new TabPage();
            label7 = new Label();
            spiritTargetBuffBox = new NumericUpDown();
            label6 = new Label();
            customEffectLevelBox = new NumericUpDown();
            label5 = new Label();
            deckNameBox = new TextBox();
            maxComplexityBox = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            minComplexityBox = new NumericUpDown();
            varianceBox = new NumericUpDown();
            label2 = new Label();
            label1 = new Label();
            targetPowerLevelBox = new NumericUpDown();
            generateArtChkBox = new CheckBox();
            deckViewerPage = new TabPage();
            loadDeckbtn = new Button();
            webBrowser1 = new WebBrowser();
            tabPage1 = new TabPage();
            folderBrowserDialog1 = new FolderBrowserDialog();
            tabControl1.SuspendLayout();
            generateDeckPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spiritTargetBuffBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)customEffectLevelBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)maxComplexityBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)minComplexityBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)varianceBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)targetPowerLevelBox).BeginInit();
            deckViewerPage.SuspendLayout();
            SuspendLayout();
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
            tabControl1.Controls.Add(generateDeckPage);
            tabControl1.Controls.Add(deckViewerPage);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new Point(6, 6);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(782, 432);
            tabControl1.TabIndex = 2;
            // 
            // generateDeckPage
            // 
            generateDeckPage.Controls.Add(label7);
            generateDeckPage.Controls.Add(spiritTargetBuffBox);
            generateDeckPage.Controls.Add(label6);
            generateDeckPage.Controls.Add(customEffectLevelBox);
            generateDeckPage.Controls.Add(label5);
            generateDeckPage.Controls.Add(deckNameBox);
            generateDeckPage.Controls.Add(maxComplexityBox);
            generateDeckPage.Controls.Add(label3);
            generateDeckPage.Controls.Add(label4);
            generateDeckPage.Controls.Add(minComplexityBox);
            generateDeckPage.Controls.Add(varianceBox);
            generateDeckPage.Controls.Add(label2);
            generateDeckPage.Controls.Add(label1);
            generateDeckPage.Controls.Add(targetPowerLevelBox);
            generateDeckPage.Controls.Add(generateArtChkBox);
            generateDeckPage.Controls.Add(generateDeckBtn);
            generateDeckPage.Location = new Point(4, 24);
            generateDeckPage.Name = "generateDeckPage";
            generateDeckPage.Padding = new Padding(3);
            generateDeckPage.Size = new Size(774, 404);
            generateDeckPage.TabIndex = 0;
            generateDeckPage.Text = "Generate Deck";
            generateDeckPage.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(132, 153);
            label7.Name = "label7";
            label7.Size = new Size(94, 15);
            label7.TabIndex = 16;
            label7.Text = "Spirit Target Buff";
            // 
            // spiritTargetBuffBox
            // 
            spiritTargetBuffBox.DecimalPlaces = 2;
            spiritTargetBuffBox.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            spiritTargetBuffBox.Location = new Point(132, 171);
            spiritTargetBuffBox.Name = "spiritTargetBuffBox";
            spiritTargetBuffBox.Size = new Size(120, 23);
            spiritTargetBuffBox.TabIndex = 15;
            spiritTargetBuffBox.Value = new decimal(new int[] { 1, 0, 0, 65536 });
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(4, 153);
            label6.Name = "label6";
            label6.Size = new Size(112, 15);
            label6.TabIndex = 14;
            label6.Text = "Custom Effect Level";
            // 
            // customEffectLevelBox
            // 
            customEffectLevelBox.Location = new Point(6, 171);
            customEffectLevelBox.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            customEffectLevelBox.Name = "customEffectLevelBox";
            customEffectLevelBox.Size = new Size(120, 23);
            customEffectLevelBox.TabIndex = 13;
            customEffectLevelBox.Value = new decimal(new int[] { 3, 0, 0, 0 });
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
            // deckNameBox
            // 
            deckNameBox.Location = new Point(6, 32);
            deckNameBox.Name = "deckNameBox";
            deckNameBox.Size = new Size(100, 23);
            deckNameBox.TabIndex = 11;
            // 
            // maxComplexityBox
            // 
            maxComplexityBox.Location = new Point(132, 123);
            maxComplexityBox.Name = "maxComplexityBox";
            maxComplexityBox.Size = new Size(120, 23);
            maxComplexityBox.TabIndex = 10;
            maxComplexityBox.Value = new decimal(new int[] { 20, 0, 0, 0 });
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
            // minComplexityBox
            // 
            minComplexityBox.Location = new Point(6, 123);
            minComplexityBox.Name = "minComplexityBox";
            minComplexityBox.Size = new Size(120, 23);
            minComplexityBox.TabIndex = 7;
            minComplexityBox.Value = new decimal(new int[] { 8, 0, 0, 0 });
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
            // deckViewerPage
            // 
            deckViewerPage.Controls.Add(loadDeckbtn);
            deckViewerPage.Controls.Add(webBrowser1);
            deckViewerPage.Location = new Point(4, 24);
            deckViewerPage.Name = "deckViewerPage";
            deckViewerPage.Padding = new Padding(3);
            deckViewerPage.Size = new Size(774, 404);
            deckViewerPage.TabIndex = 1;
            deckViewerPage.Text = "Deck Viewer";
            deckViewerPage.UseVisualStyleBackColor = true;
            // 
            // loadDeckbtn
            // 
            loadDeckbtn.Location = new Point(6, 6);
            loadDeckbtn.Name = "loadDeckbtn";
            loadDeckbtn.Size = new Size(75, 23);
            loadDeckbtn.TabIndex = 28;
            loadDeckbtn.Text = "Load Deck";
            loadDeckbtn.UseVisualStyleBackColor = true;
            loadDeckbtn.Click += loadDeckbtn_Click;
            // 
            // webBrowser1
            // 
            webBrowser1.Location = new Point(257, 6);
            webBrowser1.Margin = new Padding(5, 3, 5, 3);
            webBrowser1.MinimumSize = new Size(27, 27);
            webBrowser1.Name = "webBrowser1";
            webBrowser1.Size = new Size(238, 306);
            webBrowser1.TabIndex = 27;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(774, 404);
            tabPage1.TabIndex = 2;
            tabPage1.Text = "Scan";
            tabPage1.UseVisualStyleBackColor = true;
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
            generateDeckPage.ResumeLayout(false);
            generateDeckPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)spiritTargetBuffBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)customEffectLevelBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)maxComplexityBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)minComplexityBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)varianceBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)targetPowerLevelBox).EndInit();
            deckViewerPage.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button generateDeckBtn;
        private TabControl tabControl1;
        private TabPage generateDeckPage;
        private TabPage deckViewerPage;
        private CheckBox generateArtChkBox;
        private NumericUpDown varianceBox;
        private Label label2;
        private Label label1;
        private NumericUpDown targetPowerLevelBox;
        private NumericUpDown maxComplexityBox;
        private Label label3;
        private Label label4;
        private NumericUpDown minComplexityBox;
        private Label label5;
        private TextBox deckNameBox;
        private Label label6;
        private NumericUpDown customEffectLevelBox;
        private TabPage tabPage1;
        private WebBrowser webBrowser1;
        private Button loadDeckbtn;
        private FolderBrowserDialog folderBrowserDialog1;
        private Label label7;
        private NumericUpDown spiritTargetBuffBox;
    }
}
