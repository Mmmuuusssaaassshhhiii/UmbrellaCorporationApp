using System;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp
{
    public partial class AuthForm : Form
    {
        private readonly UmbrellaDbContext _context;

        public AuthForm(UmbrellaDbContext context)
        {
            _context = context;

            BuildUI();
        }

        private TextBox loginBox;
        private TextBox passwordBox;

        private void BuildUI()
        {
            // FORM
            this.Text = "UmbrellaCorp.";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            // ===== BACKGROUND =====
            var bg = new Panel
            {
                Dock = DockStyle.Fill,
                BackgroundImage = Image.FromFile("authBackground.png"), 
                BackgroundImageLayout = ImageLayout.Stretch
            };
            Controls.Add(bg);

            // ===== CENTER PANEL =====
            var center = new Panel
            {
                Size = new Size(500, 350),
                BackColor = Color.FromArgb(160, 20, 0, 0)
            };

            // центрирование
            center.Left = (this.Width - center.Width) / 2;
            center.Top = (this.Height - center.Height) / 2;
            center.Anchor = AnchorStyles.None;

            bg.Controls.Add(center);

            // ===== LOGO =====
            var logo = new PictureBox
            {
                Image = Image.FromFile("umbrellaCorpLogoFinal.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Top,
                Height = 120
            };
            center.Controls.Add(logo);

            // ===== LOGIN BOX =====
            loginBox = CreateTextBox("Username");
            loginBox.Top = 130;
            center.Controls.Add(loginBox);

            // ===== PASSWORD =====
            passwordBox = CreateTextBox("Password");
            passwordBox.Top = 180;
            passwordBox.UseSystemPasswordChar = true;
            center.Controls.Add(passwordBox);

            // ===== BUTTON =====
            var btn = new Button
            {
                Text = "LOGIN",
                Width = 150,
                Height = 40,
                Top = 240,
                Left = 175,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Oxanium", 10, FontStyle.Bold)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.Click += LoginClick;

            center.Controls.Add(btn);

            // ===== TITLE =====
            var title = new Label
            {
                Text = "Racoon City Facility",
                ForeColor = Color.White,
                Font = new Font("Oxanium", 16, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            bg.Controls.Add(title);
            title.BringToFront();
        }

        private TextBox CreateTextBox(string placeholder)
        {
            var tb = new TextBox
            {
                Width = 350,
                Left = 75,
                Height = 30,
                BackColor = Color.FromArgb(50, 0, 0),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Oxanium", 10),
                Text = placeholder
            };

            // placeholder логика
            tb.GotFocus += (s, e) =>
            {
                if (tb.Text == placeholder)
                    tb.Text = "";
            };

            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                    tb.Text = placeholder;
            };

            return tb;
        }

        private void LoginClick(object sender, EventArgs e)
        {
            string fullName = loginBox.Text.Trim();
            string badgeId = passwordBox.Text.Trim();

            var employee = _context.Employees
                .FirstOrDefault(e =>
                    e.FullName.ToLower() == fullName.ToLower() &&
                    e.BadgeId == badgeId);

            if (employee != null)
            {
                var main = new MainScreen(_context, employee);
                main.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверные данные");
            }
        }
    }
}