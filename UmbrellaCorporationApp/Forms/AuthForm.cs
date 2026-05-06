using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp
{
    public partial class AuthForm : Form
    {
        private readonly UmbrellaDbContext _context;

        private TextBox loginBox;
        private TextBox passwordBox;

        private CancellationTokenSource _fadeCts;
        private bool _isClosing;

        public AuthForm(UmbrellaDbContext context)
        {
            _context = context;

            BuildUI();

            Opacity = 0;

            Shown += (s, e) =>
            {
                _fadeCts = new CancellationTokenSource();
                _ = FadeIn(_fadeCts.Token);
            };
        }

        private async Task FadeIn(CancellationToken token)
        {
            try
            {
                while (Opacity < 1)
                {
                    if (_isClosing || token.IsCancellationRequested)
                        return;

                    Opacity += 0.05;
                    await Task.Delay(15, token);
                }
            }
            catch
            {
                // игнор
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _isClosing = true;
            _fadeCts?.Cancel();

            base.OnFormClosing(e);
        }

        private void BuildUI()
        {
            Text = "Umbrella Corp.";
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;

            var bg = new Panel
            {
                Dock = DockStyle.Fill,
                BackgroundImage = File.Exists("authBackground.png")
                    ? Image.FromFile("authBackground.png")
                    : null,
                BackgroundImageLayout = ImageLayout.Stretch,
                BackColor = Color.Black
            };
            Controls.Add(bg);

            var overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(90, 0, 0, 0)
            };
            bg.Controls.Add(overlay);

            var center = new Panel
            {
                Size = new Size(520, 360),
                BackColor = Color.FromArgb(140, 25, 0, 0)
            };
            overlay.Controls.Add(center);

            void CenterPanel()
            {
                center.Left = (overlay.Width - center.Width) / 2;
                center.Top = (overlay.Height - center.Height) / 2;
            }

            overlay.Resize += (s, e) => CenterPanel();
            CenterPanel();

            var logo = new PictureBox
            {
                Image = File.Exists("umbrellaCorpLogoFinal.png")
                    ? Image.FromFile("umbrellaCorpLogoFinal.png")
                    : null,
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Top,
                Height = 110
            };
            center.Controls.Add(logo);

            loginBox = CreateTextBox("Username", false);
            loginBox.Location = new Point(85, 130);
            center.Controls.Add(loginBox);

            passwordBox = CreateTextBox("Password", true);
            passwordBox.Location = new Point(85, 190);
            center.Controls.Add(passwordBox);

            var btn = new Button
            {
                Text = "LOGIN",
                Width = 180,
                Height = 45,
                Location = new Point(170, 260),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(150, 0, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += (s, e) =>
                btn.BackColor = Color.FromArgb(220, 0, 0);

            btn.MouseLeave += (s, e) =>
                btn.BackColor = Color.FromArgb(150, 0, 0);

            btn.Click += LoginClick;

            center.Controls.Add(btn);

            var title = new Label
            {
                Text = "Raccoon City Facility",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };

            overlay.Controls.Add(title);
            title.BringToFront();
        }

        private TextBox CreateTextBox(string placeholder, bool isPassword)
        {
            var tb = new TextBox
            {
                Width = 350,
                Height = 35,
                BackColor = Color.FromArgb(60, 0, 0),
                ForeColor = Color.Gray,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11),
                Text = placeholder,
                Tag = placeholder
            };

            if (isPassword)
                tb.UseSystemPasswordChar = false;

            tb.GotFocus += (s, e) =>
            {
                if (tb.Text == placeholder)
                {
                    tb.Text = "";
                    tb.ForeColor = Color.White;

                    if (isPassword)
                        tb.UseSystemPasswordChar = true;
                }
            };

            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.ForeColor = Color.Gray;

                    if (isPassword)
                        tb.UseSystemPasswordChar = false;
                }
            };

            return tb;
        }

        private void LoginClick(object sender, EventArgs e)
        {
            string fullName = loginBox.Text.Trim();
            string badgeId = passwordBox.Text.Trim();

            if (fullName == "Username" || badgeId == "Password")
            {
                MessageBox.Show("Заполни поля");
                return;
            }

            var employee = _context.Employees
                .FirstOrDefault(x =>
                    x.FullName.ToLower() == fullName.ToLower() &&
                    x.BadgeId == badgeId);

            if (employee == null)
            {
                MessageBox.Show("Неверные данные");
                return;
            }

            var main = new MainScreen(_context, employee);
            
            main.Show();
            
            this.Hide();
            
            main.FormClosed += (s, args) =>
            {
                this.Close();
            };
        }
    }
}