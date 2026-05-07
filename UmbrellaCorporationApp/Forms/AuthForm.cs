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
    public class SmoothPanel : Panel
    {
        public SmoothPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }
    }

    public partial class AuthForm : Form
    {
        private readonly UmbrellaDbContext _context;

        private TextBox loginBox;
        private TextBox passwordBox;

        private CancellationTokenSource _fadeCts;
        private bool _isClosing;

        private Image _backgroundImage;

        public AuthForm(UmbrellaDbContext context)
        {
            _context = context;

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint,
                     true);

            UpdateStyles();

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
                // ignore
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
            BackColor = Color.Black;

            if (File.Exists("authBackground.png"))
            {
                _backgroundImage = Image.FromFile("authBackground.png");
            }

            var bg = new SmoothPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };

            bg.Paint += (s, e) =>
            {
                if (_backgroundImage != null)
                {
                    e.Graphics.DrawImage(_backgroundImage, bg.ClientRectangle);
                }
            };

            Controls.Add(bg);

            var overlay = new SmoothPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            bg.Controls.Add(overlay);

            var center = new SmoothPanel
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
                Height = 110,
                BackColor = Color.Transparent
            };

            center.Controls.Add(logo);

            loginBox = CreateTextBox("Username", false);

            var loginContainer = CreateTextBoxContainer(
                loginBox,
                new Point(85, 130));

            center.Controls.Add(loginContainer);

            passwordBox = CreateTextBox("Badge-ID", true);

            var passwordContainer = CreateTextBoxContainer(
                passwordBox,
                new Point(85, 190));

            center.Controls.Add(passwordContainer);

            var btn = new Button
            {
                Text = "LOGIN",
                Width = 200,
                Height = 50,
                Location = new Point(170, 260),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(150, 0, 0),
                ForeColor = Color.White,
                Font = new Font("Oxanium", 20, FontStyle.Bold),
                TabStop = false
            };

            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(220, 0, 0);
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(150, 0, 0);
            };

            btn.Click += LoginClick;

            center.Controls.Add(btn);

            var title = new Label
            {
                Text = "Raccoon City Facility",
                ForeColor = Color.White,
                Font = new Font("Oxanium", 18, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            overlay.Controls.Add(title);

            title.BringToFront();
        }

        private SmoothPanel CreateTextBoxContainer(TextBox tb, Point location)
        {
            var container = new SmoothPanel
            {
                Size = new Size(350, 50),
                Location = location,
                BackColor = Color.FromArgb(60, 0, 0)
            };

            tb.Location = new Point(10, 12);

            container.Controls.Add(tb);

            return container;
        }

        private TextBox CreateTextBox(string placeholder, bool isPassword)
        {
            var tb = new TextBox
            {
                Width = 330,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(60, 0, 0),
                ForeColor = Color.Gray,
                Font = new Font("Oxanium", 20),
                Text = placeholder,
                Tag = placeholder
            };

            if (isPassword)
            {
                tb.UseSystemPasswordChar = false;
            }

            tb.GotFocus += (s, e) =>
            {
                if (tb.Text == placeholder)
                {
                    tb.Text = "";
                    tb.ForeColor = Color.White;

                    if (isPassword)
                    {
                        tb.UseSystemPasswordChar = true;
                    }
                }
            };

            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.ForeColor = Color.Gray;

                    if (isPassword)
                    {
                        tb.UseSystemPasswordChar = false;
                    }
                }
            };

            return tb;
        }

        private void LoginClick(object sender, EventArgs e)
        {
            string fullName = loginBox.Text.Trim();
            string badgeId = passwordBox.Text.Trim();

            if (fullName == "Username" || badgeId == "Badge-ID")
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

            Hide();

            main.FormClosed += (s, args) =>
            {
                Close();
            };
        }
    }
}