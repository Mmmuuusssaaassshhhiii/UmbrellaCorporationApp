using System;
using System.Drawing;
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
        private Panel _activeIndicator;

        private Panel content;
        private Panel sidebar;
        private Panel header;
        private Panel menuContainer;

        public MainScreen(UmbrellaDbContext context, Employee user)
        {
            _context = context;
            _currentUser = user;

            InitializeUI();
        }

        private async void LoadScreen(UserControl control)
        {
            control.Dock = DockStyle.Fill;

            content.Controls.Clear();
            content.Controls.Add(control);
        }

        private void SetActiveButton(Button btn)
        {
            if (_activeButton != null)
            {
                _activeButton.BackColor = Color.FromArgb(40, 0, 0);
            }

            _activeButton = btn;
            btn.BackColor = Color.FromArgb(120, 0, 0);
            
            _activeIndicator.Height = btn.Height;
            _activeIndicator.Top = btn.Top;
            _activeIndicator.Visible = true;
            _activeIndicator.BringToFront();
        }

        private void InitializeUI()
        {
            // FORM
            Text = "Umbrella Corp.";
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(20, 0, 0);

            // CONTENT
            content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 0, 0)
            };
            Controls.Add(content);

            // SIDEBAR
            sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
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
                TextAlign = ContentAlignment.MiddleLeft
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

            // BUTTONS
            AddMenuButton("ЛАБОРАТОРНЫЕ ОТЧЁТЫ", "UmbrellaCorporationApp/Source/Report.png",
                () => { LoadScreen(new ReportsControl()); });
            AddMenuButton("ОБРАЗЦЫ И ВИРУСЫ", "UmbrellaCorporationApp/Source/Virus.png",
                () => { LoadScreen(new VirusControl()); });
            AddMenuButton("ИСПЫТУЕМЫЕ", "UmbrellaCorporationApp/Source/Subject.png",
                () => { LoadScreen(new SubjectsControl()); });
            AddMenuButton("СТАТИСТИКА", "UmbrellaCorporationApp/Source/Statistic.png",
                () => { LoadScreen(new StatisticsControl()); });
            AddMenuButton("АВАРИЙНЫЕ ПРОТОКОЛЫ", "UmbrellaCorporationApp/Source/Protocol.png",
                () => { LoadScreen(new ProtocolsControl()); });
            AddMenuButton("ЗАСЕКРЕЧЕННЫЕ ФАЙЛЫ", "UmbrellaCorporationApp/Source/Document.png",
                () => { LoadScreen(new FilesControl()); });
            AddMenuButton("СОТРУДНИКИ", "UmbrellaCorporationApp/Source/Report.png",
                () => { LoadScreen(new EmployeesControl(_context)); });
            AddMenuButton("ЖУРНАЛ ПРОИСШЕСТВИЙ", "UmbrellaCorporationApp/Source/Log.png",
                () => { LoadScreen(new LogsControl()); });
            AddMenuButton("МУТАЦИИ", "UmbrellaCorporationApp/Source/Mutation.png",
                () => { LoadScreen(new MutationsControl()); });
            AddMenuButton("РАЗРАБОТКИ", "UmbrellaCorporationApp/Source/Development.png",
                () => { LoadScreen(new DevelopmentsControl()); });
            AddMenuButton("ВЫХОД", "UmbrellaCorporationApp/Source/Exit.png", () => { Application.Exit(); });

            // HEADER
            header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(60, 0, 0)
            };
            Controls.Add(header);
            
            var logo = new PictureBox
            {
                Image = LoadLogo(),
                Dock = DockStyle.Left,
                Width = sidebar.Width,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            header.Controls.Add(logo);
            header.Controls.SetChildIndex(logo, 0);
            
            header.Controls.Add(logo);

            header.Controls.Add(new Label
            {
                Text = _currentUser.FullName,
                ForeColor = Color.White,
                Font = new Font("Exo 2", 12),
                Location = new Point(400, 20)
            });

            header.Controls.Add(new Label
            {
                Text = $"Доступ: {_currentUser.ClearanceLevel}",
                ForeColor = Color.Gray,
                Location = new Point(400, 45)
            });
        }
        
        private Image? LoadLogo()
        {
            string path = "umbrellaCorpLogoFinal.png";
            return System.IO.File.Exists(path) ? Image.FromFile(path) : null;
        }

        private void AddMenuButton(string text, string iconPath, Action onClick)
        {
            var btn = new Button
            {
                Text = "   " + text,
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Exo 2", 10)
            };

            btn.FlatAppearance.BorderSize = 0;
            
            if (System.IO.File.Exists(iconPath))
            {
                using (var img = Image.FromFile(iconPath))
                {
                    btn.Image = new Bitmap(img, new Size(24, 24));
                }
            }
            
            btn.Click += (s, e) =>
            {
                SetActiveButton(btn);
                onClick();
            };

            btn.MouseEnter += (s, e) =>
            {
                if (btn != _activeButton)
                    btn.BackColor = Color.FromArgb(80, 0, 0);
            };

            btn.MouseLeave += (s, e) =>
            {
                if (btn != _activeButton)
                    btn.BackColor = Color.FromArgb(40, 0, 0);
            };
            
            menuContainer.Controls.Add(btn);
            menuContainer.Controls.SetChildIndex(btn, 0);
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
        }
    }
}