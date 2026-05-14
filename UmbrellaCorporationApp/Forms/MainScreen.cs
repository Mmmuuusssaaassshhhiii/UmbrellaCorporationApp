using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorporationApp.UI;

namespace UmbrellaCorporationApp
{
    public class MainScreen : Form
    {
        private readonly UmbrellaDbContext _context;
        private readonly Employee _currentUser;

        private Button? _activeButton;
        private Button? _reportsButton;

        private Panel _activeIndicator = null!;
        private Panel content = null!;
        private Panel sidebar = null!;
        private Panel header = null!;
        private Panel menuContainer = null!;

        private bool _isLoggingOut;

        public MainScreen(UmbrellaDbContext context, Employee user)
        {
            _context = context;
            _currentUser = user;

            InitializeUI();

            Shown += (_, _) => OpenDefaultScreen();
        }

        // ================= SAFE CLOSE =================
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isLoggingOut)
            {
                base.OnFormClosing(e);
                return;
            }

            e.Cancel = true;
            Logout(); // всегда через единый метод
        }

        // ================= LOGOUT FIX =================
        private void Logout()
        {
            if (_isLoggingOut)
                return;

            _isLoggingOut = true;

            try
            {
                _currentUser.IsOnline = false;
                _currentUser.LastSeen = DateTime.Now;
                _context.SaveChanges();
            }
            catch { }

            // КРИТИЧНО: закрываем форму БЕЗ вложенных FormClosed событий
            BeginInvoke(new Action(() =>
            {
                Hide();

                var auth = new AuthForm(_context);
                auth.Show();

                Dispose();
            }));
        }

        // ================= NAV =================
        private void OpenDefaultScreen()
        {
            if (_reportsButton == null)
                return;

            SetActiveButton(_reportsButton);
            LoadScreen(new ReportsControl(_context, _currentUser));
        }

        private void LoadScreen(UserControl control)
        {
            control.Dock = DockStyle.Fill;
            content.Controls.Clear();
            content.Controls.Add(control);
        }

        private void SetActiveButton(Button btn)
        {
            if (_activeButton != null)
                _activeButton.BackColor = Color.FromArgb(40, 0, 0);

            _activeButton = btn;

            btn.BackColor = Color.FromArgb(120, 0, 0);

            _activeIndicator.Height = btn.Height;
            _activeIndicator.Top = btn.Top;
            _activeIndicator.Visible = true;
            _activeIndicator.BringToFront();
        }

        // ================= UI =================
        private void InitializeUI()
        {
            Text = "UmbrellaCorp.";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(20, 0, 0);

            content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 0, 0)
            };
            Controls.Add(content);

            sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 400,
                BackColor = Color.FromArgb(40, 0, 0)
            };
            Controls.Add(sidebar);

            var menuLabel = new Label
            {
                Text = "ГЛАВНОЕ МЕНЮ",
                Dock = DockStyle.Top,
                Height = 40,
                ForeColor = Color.Gray,
                Padding = new Padding(15, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Exo 2", 15)
            };

            menuContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            sidebar.Controls.Add(menuContainer);
            sidebar.Controls.Add(menuLabel);

            _activeIndicator = new Panel
            {
                Width = 4,
                BackColor = Color.Red,
                Visible = false
            };
            menuContainer.Controls.Add(_activeIndicator);

            header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(60, 0, 0)
            };
            Controls.Add(header);

            var logo = new PictureBox
            {
                Image = LoadLogo(),
                Dock = DockStyle.Left,
                Width = sidebar.Width,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            header.Controls.Add(logo);

            var userPanel = new Panel
            {
                Size = new Size(500, 90),
                Location = new Point(sidebar.Width + 40, 5)
            };
            header.Controls.Add(userPanel);

            var userName = new Label
            {
                Text = _currentUser.FullName,
                ForeColor = Color.White,
                Font = new Font("Exo 2", 15, FontStyle.Bold),
                Location = new Point(95, 18),
                AutoSize = true
            };
            userPanel.Controls.Add(userName);

            var access = new Label
            {
                Text = $"Уровень доступа: {_currentUser.ClearanceLevel}",
                ForeColor = Color.Gray,
                Font = new Font("Exo 2", 10),
                Location = new Point(95, 48),
                AutoSize = true
            };
            userPanel.Controls.Add(access);

            // ================= MENU =================
            _reportsButton = AddMenuButton("ЛАБОРАТОРНЫЕ ОТЧЁТЫ", "Report.png",
                () => LoadScreen(new ReportsControl(_context, _currentUser)));

            AddMenuButton("ОБРАЗЦЫ И ВИРУСЫ", "Virus.png",
                () => LoadScreen(new VirusControl(_context)));

            AddMenuButton("ИСПЫТУЕМЫЕ", "Zombie.png",
                () => LoadScreen(new SubjectsControl()));

            AddMenuButton("СТАТИСТИКА И АНАЛИЗ", "Statistic.png",
                () => LoadScreen(new StatisticsControl()));

            AddMenuButton("АВАРИЙНЫЕ ПРОТОКОЛЫ", "Protocol.png",
                () => LoadScreen(new ProtocolsControl()));

            AddMenuButton("ЗАСЕКРЕЧЕННЫЕ ФАЙЛЫ", "Document.png",
                () => LoadScreen(new FilesControl(_context, _currentUser)));

            AddMenuButton("СОТРУДНИКИ", "Employee.png",
                () => LoadScreen(new EmployeesControl(_context)));

            AddMenuButton("ЖУРНАЛ ПРОИСШЕСТВИЙ", "Log.png",
                () => LoadScreen(new LogsControl()));

            AddMenuButton("ИССЛЕДОВАНИЕ МУТАЦИЙ", "Mutation.png",
                () => LoadScreen(new MutationsControl()));

            AddMenuButton("УТРАТЫ И ЛИКВИДАЦИИ", "Loss.png",
                () => LoadScreen(new MutationsControl()));

            AddMenuButton("РАЗРАБОТКИ", "Development.png",
                () => LoadScreen(new DevelopmentsControl()));

            AddMenuButton("СООБЩЕНИЯ", "Message.png",
                () => LoadScreen(new MessageControl(_context, _currentUser)));

            AddMenuButton("ВЫХОД", "Exit.png",
                Logout);
        }

        private Image? LoadLogo()
        {
            string path = "umbrellaCorpLogoFinal.png";
            return File.Exists(path) ? Image.FromFile(path) : null;
        }

        // ================= BUTTONS =================
        private Button AddMenuButton(string text, string iconPath, Action onClick)
        {
            var btn = new Button
            {
                Text = "   " + text,
                Cursor = Cursors.Hand,
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Exo 2", 15)
            };

            btn.FlatAppearance.BorderSize = 0;

            if (File.Exists(iconPath))
            {
                using var img = Image.FromFile(iconPath);
                btn.Image = new Bitmap(img, new Size(30, 30));
            }

            btn.Click += (_, _) =>
            {
                SetActiveButton(btn);
                onClick();
            };

            btn.MouseEnter += (_, _) =>
            {
                if (btn != _activeButton)
                    btn.BackColor = Color.FromArgb(80, 0, 0);
            };

            btn.MouseLeave += (_, _) =>
            {
                if (btn != _activeButton)
                    btn.BackColor = Color.FromArgb(40, 0, 0);
            };

            menuContainer.Controls.Add(btn);
            menuContainer.Controls.SetChildIndex(btn, 0);

            return btn;
        }
    }
}