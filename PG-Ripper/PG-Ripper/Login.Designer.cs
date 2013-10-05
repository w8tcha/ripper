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
            this.GuestLogin = new System.Windows.Forms.CheckBox();
            this.ForumList = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ForumUrl = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LoginButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.PasswordField = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.UserNameField = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Timers.Timer();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.GuestLogin);
            this.groupBox1.Controls.Add(this.ForumList);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.ForumUrl);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.LoginButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.PasswordField);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.UserNameField);
            this.groupBox1.Location = new System.Drawing.Point(8, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(376, 304);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Provide Login Credentials for the Forums";
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // GuestLogin
            // 
            this.GuestLogin.AutoSize = true;
            this.GuestLogin.Location = new System.Drawing.Point(102, 78);
            this.GuestLogin.Name = "GuestLogin";
            this.GuestLogin.Size = new System.Drawing.Size(92, 17);
            this.GuestLogin.TabIndex = 17;
            this.GuestLogin.Text = "Guest Access";
            this.GuestLogin.UseVisualStyleBackColor = true;
            this.GuestLogin.CheckedChanged += new System.EventHandler(this.GuestLogin_CheckedChanged);
            // 
            // ForumList
            // 
            this.ForumList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ForumList.IntegralHeight = false;
            this.ForumList.ItemHeight = 13;
            this.ForumList.Items.AddRange(new object[] {
            "<Select a Forum>",
            "The Kitty-Kats Forum (perved.com)",
            "ViperGirls Forums",
            "Sexy and Funny Forums",
            "Scanlover Forums",
            "Big Naturals Only",
            "The phun.org forum",
            "<Other - Enter URL bellow>"});
            this.ForumList.Location = new System.Drawing.Point(102, 109);
            this.ForumList.Name = "ForumList";
            this.ForumList.Size = new System.Drawing.Size(246, 21);
            this.ForumList.TabIndex = 2;
            this.ForumList.SelectedIndexChanged += new System.EventHandler(this.CBForumSelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Forum URL:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Select Forum:";
            // 
            // ForumUrl
            // 
            this.ForumUrl.Location = new System.Drawing.Point(102, 144);
            this.ForumUrl.Name = "ForumUrl";
            this.ForumUrl.Size = new System.Drawing.Size(246, 20);
            this.ForumUrl.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(114, 273);
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
            this.comboBox2.Location = new System.Drawing.Point(240, 270);
            this.comboBox2.MaximumSize = new System.Drawing.Size(121, 0);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 6;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.ComboBox2SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(9, 213);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(339, 24);
            this.label3.TabIndex = 5;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoginButton
            // 
            this.LoginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginButton.Image = ((System.Drawing.Image)(resources.GetObject("LoginButton.Image")));
            this.LoginButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LoginButton.Location = new System.Drawing.Point(102, 170);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(246, 40);
            this.LoginButton.TabIndex = 5;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseCompatibleTextRendering = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginBtnClick);
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
            // PasswordField
            // 
            this.PasswordField.Location = new System.Drawing.Point(102, 51);
            this.PasswordField.MaximumSize = new System.Drawing.Size(246, 20);
            this.PasswordField.Name = "PasswordField";
            this.PasswordField.PasswordChar = '*';
            this.PasswordField.Size = new System.Drawing.Size(246, 20);
            this.PasswordField.TabIndex = 1;
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
            // UserNameField
            // 
            this.UserNameField.Location = new System.Drawing.Point(102, 19);
            this.UserNameField.MaximumSize = new System.Drawing.Size(246, 20);
            this.UserNameField.Name = "UserNameField";
            this.UserNameField.Size = new System.Drawing.Size(246, 20);
            this.UserNameField.TabIndex = 0;
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
            this.ClientSize = new System.Drawing.Size(394, 332);
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
        private TextBox UserNameField;
        private Label label1;
        private Label label2;
        private TextBox PasswordField;
        private Button LoginButton;
        private Label label3;
        private System.Timers.Timer timer1;
        private ComboBox comboBox2;
        private Label label5;
        private Label label6;
        private TextBox ForumUrl;
        private Label label4;
        private ComboBox ForumList;
        private CheckBox GuestLogin;
    }
}
