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

        private void LoadScreen(UserControl control)
        {
            content.Controls.Clear();
            content.Controls.Add(control);
        }

        private Panel header;
        private Panel sidebar;
        private Panel content;
        private FlowLayoutPanel filesPanel;
        private Panel menuContainer; 

        public MainScreen(UmbrellaDbContext context, Employee user)
        {
            _context = context;
            _currentUser = user;

            InitializeUI();
        }

       private void InitializeUI()
{
    // form
    this.Text = "Umbrella System";
    this.WindowState = FormWindowState.Maximized;
    this.BackColor = Color.FromArgb(20, 0, 0);

    // header
    header = new Panel
    {
        Dock = DockStyle.Top,
        Height = 80,
        BackColor = Color.FromArgb(60, 0, 0)
    };
    this.Controls.Add(header);

    var userLabel = new Label
    {
        Text = _currentUser.FullName,
        ForeColor = Color.White,
        Font = new Font("Oxanium", 12),
        Location = new Point(400, 20),
        AutoSize = true
    };

    var accessLabel = new Label
    {
        Text = $"Доступ: {_currentUser.ClearanceLevel}",
        ForeColor = Color.Gray,
        Location = new Point(400, 45),
        AutoSize = true
    };

    header.Controls.Add(userLabel);
    header.Controls.Add(accessLabel);

    // sidebar
    sidebar = new Panel
    {
        Dock = DockStyle.Left,
        Width = 250,
        BackColor = Color.FromArgb(40, 0, 0)
    };
    this.Controls.Add(sidebar);
    
    menuContainer = new Panel
    {
        Dock = DockStyle.Fill,
        AutoScroll = true
    };
    sidebar.Controls.Add(menuContainer);

    var menuLabel = new Label
    {
        Text = "ГЛАВНОЕ МЕНЮ",
        ForeColor = Color.Gray,
        Dock = DockStyle.Top,
        Height = 40,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(15, 0, 0, 0),
        Font = new Font("Oxanium", 9, FontStyle.Bold)
    };
    sidebar.Controls.Add(menuLabel);

    var logo = new PictureBox
    {
        Image = LoadLogo(),
        SizeMode = PictureBoxSizeMode.Zoom,
        Dock = DockStyle.Top,
        Height = 80
    };
    sidebar.Controls.Add(logo);

    // buttons
    AddMenuButton("ЛАБОРАТОРНЫЕ ОТЧЁТЫ", () =>
    {
        LoadScreen(new ReportsControl());
    });
    //AddMenuButton("ОБРАЗЦЫ И ВИРУСЫ");
    //AddMenuButton("ИСПЫТУЕМЫЕ");
    //AddMenuButton("СТАТИСТИКА");
    //AddMenuButton("АВАРИЙНЫЕ ПРОТОКОЛЫ");
    //AddMenuButton("ЗАСЕКРЕЧЕННЫЕ ФАЙЛЫ");
    //AddMenuButton("СОТРУДНИКИ");
   // AddMenuButton("ЖУРНАЛ ПРОИСШЕСТВИЙ");
   // AddMenuButton("МУТАЦИИ");
    //AddMenuButton("РАЗРАБОТКИ");
    AddMenuButton("ВЫХОД", () =>
    {
        Application.Exit();
    });

    // content
    content = new Panel
    {
        Dock = DockStyle.Fill,
        BackColor = Color.FromArgb(30, 0, 0)
    };
    this.Controls.Add(content);

    filesPanel = new FlowLayoutPanel
    {
        Dock = DockStyle.Fill,
        AutoScroll = true
    };
    content.Controls.Add(filesPanel);

    GenerateFiles();
}

        // logo loading
        private Image? LoadLogo()
        {
            string path = "umbrellaCorpLogoFinal.png";

            if (System.IO.File.Exists(path))
                return Image.FromFile(path);

            return null;
        }

        // menu buttons
        private void AddMenuButton(string text, Action onClick)
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
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Exo 2", 20)
            };

            btn.FlatAppearance.BorderSize = 0;

            btn.Click += (s, e) => onClick();

            btn.MouseEnter += (s, e) =>
                btn.BackColor = Color.FromArgb(80, 0, 0);
            
            btn.MouseLeave += (s, e) =>
                btn.BackColor = Color.FromArgb(40, 0, 0);
            
            menuContainer.Controls.Add(btn);
            menuContainer.Controls.SetChildIndex(btn, 0);
        }

        // demo content
        private void GenerateFiles()
        {
            for (int i = 1; i <= 50; i++)
            {
                var filePanel = new Panel
                {
                    Size = new Size(100, 120),
                    BackColor = Color.FromArgb(50, 0, 0),
                    Margin = new Padding(10)
                };

                var icon = new Label
                {
                    Text = " ",
                    Font = new Font("Oxanium", 30),
                    Dock = DockStyle.Top,
                    Height = 70,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var label = new Label
                {
                    Text = $"Отчёт №{i}",
                    ForeColor = Color.White,
                    Dock = DockStyle.Bottom,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                filePanel.Controls.Add(label);
                filePanel.Controls.Add(icon);

                filesPanel.Controls.Add(filePanel);
            }
        }
    }
}