using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace PGRipper
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cBForum = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Timers.Timer();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cBForum);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.LoginBtn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(8, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(376, 263);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Provide Login Credentials for the Forums";
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // cBForum
            // 
            this.cBForum.IntegralHeight = false;
            this.cBForum.ItemHeight = 13;
            this.cBForum.Items.AddRange(new object[] {
            "<Select a Forum>",
            "The Kitty-Kats Forum (perved.com)",
            "ViperGirls Forums",
            "Sexy and Funny Forums",
            "Scanlover Forums",
            "Big Naturals Only",
            "The phun.org forum",
            "<Other - Enter URL bellow>"});
            this.cBForum.Location = new System.Drawing.Point(102, 82);
            this.cBForum.Name = "cBForum";
            this.cBForum.Size = new System.Drawing.Size(246, 21);
            this.cBForum.TabIndex = 2;
            this.cBForum.SelectedIndexChanged += new System.EventHandler(this.CBForumSelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Forum URL:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Select Forum:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(102, 117);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(246, 20);
            this.textBox3.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(126, 234);
            this.label5.MaximumSize = new System.Drawing.Size(0, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "label5";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.UseCompatibleTextRendering = true;
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.Items.AddRange(new object[] {
            "German",
            "French",
            "English"});
            this.comboBox2.Location = new System.Drawing.Point(252, 231);
            this.comboBox2.MaximumSize = new System.Drawing.Size(121, 0);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 6;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.ComboBox2SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(102, 144);
            this.checkBox1.MaximumSize = new System.Drawing.Size(144, 24);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(144, 24);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Remember Me";
            this.checkBox1.UseCompatibleTextRendering = true;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(9, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(339, 24);
            this.label3.TabIndex = 5;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoginBtn
            // 
            this.LoginBtn.Image = ((System.Drawing.Image)(resources.GetObject("LoginBtn.Image")));
            this.LoginBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LoginBtn.Location = new System.Drawing.Point(252, 143);
            this.LoginBtn.MaximumSize = new System.Drawing.Size(96, 24);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(96, 24);
            this.LoginBtn.TabIndex = 5;
            this.LoginBtn.Text = "Login";
            this.LoginBtn.UseCompatibleTextRendering = true;
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtnClick);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.MaximumSize = new System.Drawing.Size(87, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password :";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(102, 51);
            this.textBox2.MaximumSize = new System.Drawing.Size(246, 20);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(246, 20);
            this.textBox2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.MaximumSize = new System.Drawing.Size(87, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "User Name :";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(102, 19);
            this.textBox1.MaximumSize = new System.Drawing.Size(246, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(246, 20);
            this.textBox1.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Interval = 400D;
            this.timer1.SynchronizingObject = this;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.Timer1Elapsed);
            // 
            // Login
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(394, 291);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.Text = "Login";
            this.Load += new System.EventHandler(this.LoginLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private GroupBox groupBox1;
        private TextBox textBox1;
        private Label label1;
        private Label label2;
        private TextBox textBox2;
        private Button LoginBtn;
        private Label label3;
        private System.Timers.Timer timer1;
        private CheckBox checkBox1;
        private ComboBox comboBox2;
        private Label label5;
        private Label label6;
        private TextBox textBox3;
        private Label label4;
        private ComboBox cBForum;
    }
}
