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
                "User.png",
                new Point(85, 130));

            center.Controls.Add(loginContainer);

            passwordBox = CreateTextBox("Badge-ID", true);

            var passwordContainer = CreateTextBoxContainer(
                passwordBox,
                "Card.png",
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
                Font = new Font("Exo 2", 20, FontStyle.Bold),
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
            
            var topContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,

                Height = 120,

                FlowDirection = FlowDirection.TopDown,

                WrapContents = false,

                BackColor = Color.Transparent,

                Padding = new Padding(0, 0, 0, 0)
            };

            overlay.Controls.Add(topContainer);
            
            var bottomContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,

                Height = 170,

                FlowDirection = FlowDirection.TopDown,

                WrapContents = false,

                BackColor = Color.Transparent,

                Padding = new Padding(0, 0, 0, 0)
            };

            overlay.Controls.Add(bottomContainer);

            var title = CreateTitle(
                "UMBRELLA Corp Biotechnological Division LLC",
                false);

            var title2 = CreateTitle(
                "Raccoon City Facility",
                true);

            var title3 = CreateTitle(
                "SECURITY WARNING!",
                true);

            var title4 = CreateTitle(
                "Anything viewed beyond this point is covered under the UMBRELLA Corp Security Agreement section of the Employee Code of Conduct.",
                false);

            var title5 = CreateTitle(
                "Second Party viewing by individuals without proper security clearance, will be handled by the highest administrative level possible!",
                false);

            topContainer.Controls.Add(title);

            topContainer.Controls.Add(title2);

            bottomContainer.Controls.Add(title3);

            bottomContainer.Controls.Add(title4);

            bottomContainer.Controls.Add(title5);
        }
        
        private Label CreateTitle(string text, bool bold)
        {
            return new Label
            {
                Text = text,

                Width = Screen.PrimaryScreen.Bounds.Width,

                Height = 45,

                Margin = new Padding(0, 2, 0, 2),

                ForeColor = Color.White,

                Font = new Font(
                    "Exo 2",
                    18,
                    bold
                        ? FontStyle.Bold
                        : FontStyle.Regular),

                TextAlign = ContentAlignment.MiddleCenter,

                BackColor = Color.Transparent
            };
        }

        private SmoothPanel CreateTextBoxContainer(TextBox tb, string iconPath, Point location)
        {
            var container = new SmoothPanel
            {
                Size = new Size(350, 50),
                Location = location,
                BackColor = Color.FromArgb(60, 0, 0)
            };

            var icon = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(12, 13),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Image = File.Exists(iconPath)
                    ? Image.FromFile(iconPath)
                    : SystemIcons.Information.ToBitmap()
            };

            tb.Location = new Point(45, 10);
            tb.Width = 290;

            container.Controls.Add(icon);
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
                Font = new Font("Exo 2", 15),
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
            
            employee.IsOnline = true;
            employee.LastSeen = DateTime.Now;

            _context.SaveChanges();

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