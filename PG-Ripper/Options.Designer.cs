using System.Globalization;
namespace Ripper
{
    partial class Options
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.showTrayPopups = new System.Windows.Forms.CheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.languageSelector = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUDThreads = new System.Windows.Forms.NumericUpDown();
            this.numericUDThanks = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.saveHistoryChk = new System.Windows.Forms.CheckBox();
            this.mDownInSepFolderChk = new System.Windows.Forms.CheckBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.FBD = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUDThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUDThanks)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox11);
            this.groupBox3.Controls.Add(this.checkBox10);
            this.groupBox3.Controls.Add(this.checkBox9);
            this.groupBox3.Controls.Add(this.checkBox3);
            this.groupBox3.Controls.Add(this.checkBox5);
            this.groupBox3.Controls.Add(this.showTrayPopups);
            this.groupBox3.Controls.Add(this.pictureBox2);
            this.groupBox3.Controls.Add(this.languageSelector);
            this.groupBox3.Location = new System.Drawing.Point(12, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(357, 161);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Main Options";
            this.groupBox3.UseCompatibleTextRendering = true;
            // 
            // checkBox11
            // 
            this.checkBox11.AutoSize = true;
            this.checkBox11.Location = new System.Drawing.Point(8, 135);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(159, 17);
            this.checkBox11.TabIndex = 24;
            this.checkBox11.Text = "Show Last Download Image";
            this.checkBox11.UseVisualStyleBackColor = true;
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.Location = new System.Drawing.Point(8, 111);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(128, 18);
            this.checkBox10.TabIndex = 5;
            this.checkBox10.Text = "Clipboard Monitoring";
            this.checkBox10.UseCompatibleTextRendering = true;
            this.checkBox10.UseVisualStyleBackColor = true;
            // 
            // checkBox9
            // 
            this.checkBox9.Location = new System.Drawing.Point(8, 67);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(337, 18);
            this.checkBox9.TabIndex = 3;
            this.checkBox9.Text = "Show \"Downloads Complete\" PopUp?";
            this.checkBox9.UseCompatibleTextRendering = true;
            this.checkBox9.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox3.Location = new System.Drawing.Point(17, 267);
            this.checkBox3.MaximumSize = new System.Drawing.Size(0, 18);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(0, 18);
            this.checkBox3.TabIndex = 23;
            this.checkBox3.Text = "Automatically press the \"Thank You\" Button";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.Location = new System.Drawing.Point(8, 89);
            this.checkBox5.MaximumSize = new System.Drawing.Size(0, 17);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(337, 17);
            this.checkBox5.TabIndex = 4;
            this.checkBox5.Text = "Always on Top";
            this.checkBox5.UseCompatibleTextRendering = true;
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // showTrayPopups
            // 
            this.showTrayPopups.Checked = true;
            this.showTrayPopups.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showTrayPopups.Location = new System.Drawing.Point(8, 46);
            this.showTrayPopups.MaximumSize = new System.Drawing.Size(0, 17);
            this.showTrayPopups.Name = "showTrayPopups";
            this.showTrayPopups.Size = new System.Drawing.Size(337, 17);
            this.showTrayPopups.TabIndex = 2;
            this.showTrayPopups.Text = "Show Tray PopUps";
            this.showTrayPopups.UseCompatibleTextRendering = true;
            this.showTrayPopups.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(9, 25);
            this.pictureBox2.MaximumSize = new System.Drawing.Size(16, 16);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            // 
            // languageSelector
            // 
            this.languageSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageSelector.Items.AddRange(new object[] {
            "German",
            "French",
            "English"});
            this.languageSelector.Location = new System.Drawing.Point(53, 19);
            this.languageSelector.MaximumSize = new System.Drawing.Size(121, 0);
            this.languageSelector.Name = "languageSelector";
            this.languageSelector.Size = new System.Drawing.Size(121, 21);
            this.languageSelector.TabIndex = 1;
            this.languageSelector.SelectedIndexChanged += new System.EventHandler(this.LanguageSelectorSelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUDThreads);
            this.groupBox1.Controls.Add(this.numericUDThanks);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.saveHistoryChk);
            this.groupBox1.Controls.Add(this.mDownInSepFolderChk);
            this.groupBox1.Controls.Add(this.checkBox8);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.checkBox4);
            this.groupBox1.Controls.Add(this.checkBox7);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.checkBox6);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(13, 182);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(357, 260);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download Options";
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // numericUDThreads
            // 
            this.numericUDThreads.Location = new System.Drawing.Point(22, 224);
            this.numericUDThreads.Name = "numericUDThreads";
            this.numericUDThreads.Size = new System.Drawing.Size(50, 20);
            this.numericUDThreads.TabIndex = 14;
            this.numericUDThreads.ValueChanged += new System.EventHandler(this.NumericUdThreadsValueChanged);
            // 
            // numericUDThanks
            // 
            this.numericUDThanks.Location = new System.Drawing.Point(181, 153);
            this.numericUDThanks.Name = "numericUDThanks";
            this.numericUDThanks.Size = new System.Drawing.Size(49, 20);
            this.numericUDThanks.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "Min. Image Count to give Thanks:";
            // 
            // saveHistoryChk
            // 
            this.saveHistoryChk.Location = new System.Drawing.Point(7, 179);
            this.saveHistoryChk.Name = "saveHistoryChk";
            this.saveHistoryChk.Size = new System.Drawing.Size(337, 18);
            this.saveHistoryChk.TabIndex = 13;
            this.saveHistoryChk.Text = "Save Ripped posts for checking";
            this.saveHistoryChk.UseCompatibleTextRendering = true;
            this.saveHistoryChk.UseVisualStyleBackColor = true;
            // 
            // mDownInSepFolderChk
            // 
            this.mDownInSepFolderChk.Checked = true;
            this.mDownInSepFolderChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mDownInSepFolderChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mDownInSepFolderChk.Location = new System.Drawing.Point(22, 107);
            this.mDownInSepFolderChk.Name = "mDownInSepFolderChk";
            this.mDownInSepFolderChk.Size = new System.Drawing.Size(329, 18);
            this.mDownInSepFolderChk.TabIndex = 9;
            this.mDownInSepFolderChk.Text = "Download each post in its own folder";
            this.mDownInSepFolderChk.UseCompatibleTextRendering = true;
            this.mDownInSepFolderChk.UseVisualStyleBackColor = true;
            // 
            // checkBox8
            // 
            this.checkBox8.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox8.Location = new System.Drawing.Point(7, 131);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(337, 18);
            this.checkBox8.TabIndex = 11;
            this.checkBox8.Text = "Automatically press the \"Thank You\" Button";
            this.checkBox8.UseCompatibleTextRendering = true;
            this.checkBox8.UseVisualStyleBackColor = true;
            this.checkBox8.CheckedChanged += new System.EventHandler(this.CheckBox8CheckedChanged);
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button4.Location = new System.Drawing.Point(99, 19);
            this.button4.MaximumSize = new System.Drawing.Size(56, 20);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(56, 20);
            this.button4.TabIndex = 8;
            this.button4.Text = "Browse";
            this.button4.UseCompatibleTextRendering = true;
            this.button4.Click += new System.EventHandler(this.Button4Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 39;
            this.label2.Text = "Download Folder :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.UseCompatibleTextRendering = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(8, 42);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(336, 20);
            this.textBox2.TabIndex = 7;
            // 
            // checkBox4
            // 
            this.checkBox4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox4.Location = new System.Drawing.Point(8, 56);
            this.checkBox4.MaximumSize = new System.Drawing.Size(0, 17);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(0, 17);
            this.checkBox4.TabIndex = 36;
            this.checkBox4.Text = "Download each post in its own folder";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox7
            // 
            this.checkBox7.Checked = true;
            this.checkBox7.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox7.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox7.Location = new System.Drawing.Point(8, 94);
            this.checkBox7.MaximumSize = new System.Drawing.Size(0, 18);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(0, 18);
            this.checkBox7.TabIndex = 35;
            this.checkBox7.Text = "Automatically press the \"Thank You\" Button";
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox2.Location = new System.Drawing.Point(8, 42);
            this.checkBox2.MaximumSize = new System.Drawing.Size(0, 17);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(0, 17);
            this.checkBox2.TabIndex = 34;
            this.checkBox2.Text = "Download each post in its own folder";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.Checked = true;
            this.checkBox6.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox6.Location = new System.Drawing.Point(8, 87);
            this.checkBox6.MaximumSize = new System.Drawing.Size(0, 18);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(0, 18);
            this.checkBox6.TabIndex = 33;
            this.checkBox6.Text = "Automatically press the \"Thank You\" Button";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox1.Location = new System.Drawing.Point(8, 85);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(337, 16);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Download each Thread in its own folder";
            this.checkBox1.UseCompatibleTextRendering = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 201);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(338, 20);
            this.label6.TabIndex = 29;
            this.label6.Text = "Maximum Simultaneous Downloads:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.UseCompatibleTextRendering = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(214, 453);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 27);
            this.okButton.TabIndex = 15;
            this.okButton.Text = "OK";
            this.okButton.UseCompatibleTextRendering = true;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(297, 453);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 27);
            this.cancelButton.TabIndex = 16;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // FBD
            // 
            this.FBD.ShowNewFolderButton = false;
            // 
            // Options
            // 
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(384, 492);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(390, 350);
            this.Name = "Options";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PG-Ripper Options";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUDThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUDThanks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox showTrayPopups;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ComboBox languageSelector;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.CheckBox mDownInSepFolderChk;
        private System.Windows.Forms.FolderBrowserDialog FBD;
        private System.Windows.Forms.CheckBox saveHistoryChk;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox10;
        private System.Windows.Forms.NumericUpDown numericUDThanks;
        private System.Windows.Forms.NumericUpDown numericUDThreads;
        private System.Windows.Forms.CheckBox checkBox11;
    }
}