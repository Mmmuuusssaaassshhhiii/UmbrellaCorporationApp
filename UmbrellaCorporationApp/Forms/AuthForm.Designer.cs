namespace UmbrellaCorporationApp;

partial class AuthForm
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthForm));
        panel1 = new System.Windows.Forms.Panel();
        button1 = new System.Windows.Forms.Button();
        password = new System.Windows.Forms.TextBox();
        login = new System.Windows.Forms.TextBox();
        panel1.SuspendLayout();
        SuspendLayout();
        // 
        // panel1
        // 
        panel1.Controls.Add(button1);
        panel1.Controls.Add(password);
        panel1.Controls.Add(login);
        panel1.Location = new System.Drawing.Point(96, 154);
        panel1.Name = "panel1";
        panel1.Size = new System.Drawing.Size(776, 108);
        panel1.TabIndex = 0;
        // 
        // button1
        // 
        button1.Font = new System.Drawing.Font("Oxanium", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)0));
        button1.Location = new System.Drawing.Point(302, 67);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(105, 31);
        button1.TabIndex = 2;
        button1.Text = "Войти";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // password
        // 
        password.Location = new System.Drawing.Point(163, 48);
        password.Name = "password";
        password.Size = new System.Drawing.Size(400, 22);
        password.TabIndex = 1;
        // 
        // login
        // 
        login.Location = new System.Drawing.Point(163, 28);
        login.Name = "login";
        login.Size = new System.Drawing.Size(400, 22);
        login.TabIndex = 0;
        // 
        // AuthForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackgroundImage = ((System.Drawing.Image)resources.GetObject("$this.BackgroundImage"));
        BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        ClientSize = new System.Drawing.Size(955, 372);
        Controls.Add(panel1);
        Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)204));
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "UmbrellaCorp.";
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        ResumeLayout(false);
    }

    private System.Windows.Forms.TextBox login;
    private System.Windows.Forms.TextBox password;
    private System.Windows.Forms.Button button1;

    private System.Windows.Forms.Panel panel1;

    #endregion
}