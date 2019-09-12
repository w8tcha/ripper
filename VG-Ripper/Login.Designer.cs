using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Ripper
{
    using System.ComponentModel;
    using System.Timers;

    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

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
            ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RegisterLink = new System.Windows.Forms.LinkLabel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Timers.Timer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.GuestLoginButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();

            // groupBox1
            this.groupBox1.Controls.Add(this.RegisterLink);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.LoginBtn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(8, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(359, 241);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Provide Login Credentials for the ViperGirls Forums";
            this.groupBox1.UseCompatibleTextRendering = true;

            // RegisterLink
            this.RegisterLink.AutoSize = true;
            this.RegisterLink.Location = new System.Drawing.Point(99, 183);
            this.RegisterLink.Name = "RegisterLink";
            this.RegisterLink.Size = new System.Drawing.Size(177, 13);
            this.RegisterLink.TabIndex = 7;
            this.RegisterLink.TabStop = true;
            this.RegisterLink.Text = "Not a Member yet? Click to Register";
            this.RegisterLink.LinkClicked +=
                new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.RegisterLink_LinkClicked);

            // checkBox1
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(102, 152);
            this.checkBox1.MaximumSize = new System.Drawing.Size(144, 24);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(120, 23);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Remember Me";
            this.checkBox1.UseCompatibleTextRendering = true;

            // label4
            this.label4.Font = new System.Drawing.Font(
                "Verdana",
                9.75F,
                System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point,
                ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(16, 24);
            this.label4.MaximumSize = new System.Drawing.Size(352, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(293, 45);
            this.label4.TabIndex = 6;
            this.label4.Text = "WARNING! More than 3 failed tries will result in your Forum Account being locked!"
                               + " You have been warned!";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.UseCompatibleTextRendering = true;

            // label3
            this.label3.Font = new System.Drawing.Font(
                "Verdana",
                9.75F,
                System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point,
                ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(13, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(335, 24);
            this.label3.TabIndex = 5;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // LoginBtn
            this.LoginBtn.Image = ((System.Drawing.Image)(resources.GetObject("LoginBtn.Image")));
            this.LoginBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LoginBtn.Location = new System.Drawing.Point(252, 152);
            this.LoginBtn.MaximumSize = new System.Drawing.Size(96, 24);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(80, 23);
            this.LoginBtn.TabIndex = 3;
            this.LoginBtn.Text = "Login";
            this.LoginBtn.UseCompatibleTextRendering = true;
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtnClick);

            // label2
            this.label2.Location = new System.Drawing.Point(9, 123);
            this.label2.MaximumSize = new System.Drawing.Size(87, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 14);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password :";
            this.label2.UseCompatibleTextRendering = true;

            // textBox2
            this.textBox2.Location = new System.Drawing.Point(102, 120);
            this.textBox2.MaximumSize = new System.Drawing.Size(246, 20);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(205, 20);
            this.textBox2.TabIndex = 1;

            // label1
            this.label1.Location = new System.Drawing.Point(9, 88);
            this.label1.MaximumSize = new System.Drawing.Size(87, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "User Name :";
            this.label1.UseCompatibleTextRendering = true;

            // textBox1
            this.textBox1.Location = new System.Drawing.Point(102, 88);
            this.textBox1.MaximumSize = new System.Drawing.Size(246, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(205, 20);
            this.textBox1.TabIndex = 0;

            // label5
            this.label5.Anchor =
                ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom
                                                      | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(76, 363);
            this.label5.MaximumSize = new System.Drawing.Size(0, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(184, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "label5";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label5.UseCompatibleTextRendering = true;

            // comboBox2
            this.comboBox2.Anchor =
                ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom
                                                      | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.Items.AddRange(new object[] { "German", "French", "English" });
            this.comboBox2.Location = new System.Drawing.Point(266, 359);
            this.comboBox2.MaximumSize = new System.Drawing.Size(121, 0);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(101, 21);
            this.comboBox2.TabIndex = 4;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.ComboBox2SelectedIndexChanged);

            // timer1
            this.timer1.Interval = 400D;
            this.timer1.SynchronizingObject = this;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.Timer1Elapsed);

            // groupBox2
            this.groupBox2.Controls.Add(this.GuestLoginButton);
            this.groupBox2.Location = new System.Drawing.Point(8, 264);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(359, 81);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Or Login as Guest ...";

            // GuestLoginButton
            this.GuestLoginButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GuestLoginButton.Location = new System.Drawing.Point(102, 19);
            this.GuestLoginButton.Name = "GuestLoginButton";
            this.GuestLoginButton.Size = new System.Drawing.Size(244, 47);
            this.GuestLoginButton.TabIndex = 4;
            this.GuestLoginButton.Text = "Guest Login";
            this.GuestLoginButton.UseCompatibleTextRendering = true;
            this.GuestLoginButton.Click += new System.EventHandler(this.GuestLoginButton_Click);

            // Login
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(375, 395);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBox2);
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
            this.groupBox2.ResumeLayout(false);
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
        private Timer timer1;
        private Label label4;
        private CheckBox checkBox1;
        private ComboBox comboBox2;
        private Label label5;
        private GroupBox groupBox2;
        private Button GuestLoginButton;
        private LinkLabel RegisterLink;
    }
}
