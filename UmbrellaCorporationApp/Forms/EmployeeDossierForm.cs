using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI
{
    public class EmployeeDossierForm : Form
    {
        private Panel sheet = null!;
        private Button closeBtn = null!;

        public EmployeeDossierForm(Employee emp)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.Black;
            Opacity = 0.95;

            InitializeSheet();
            InitializeCloseButton();
            Build(emp);
        }

        private void InitializeSheet()
        {
            sheet = new Panel
            {
                Size = new Size(800, 900),
                BackColor = Color.White
            };

            Controls.Add(sheet);

            sheet.Left = (ClientSize.Width - sheet.Width) / 2;
            sheet.Top = (ClientSize.Height - sheet.Height) / 2;

            Resize += (_, _) =>
            {
                sheet.Left = (ClientSize.Width - sheet.Width) / 2;
                sheet.Top = (ClientSize.Height - sheet.Height) / 2;
            };
        }

        private void InitializeCloseButton()
        {
            closeBtn = new Button
            {
                Text = "X",

                Width = 60,
                Height = 60,

                FlatStyle = FlatStyle.Flat,

                Font = new Font("Exo 2", 18, FontStyle.Bold),

                ForeColor = Color.White,

                Cursor = Cursors.Hand,

                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            closeBtn.FlatAppearance.BorderSize = 0;

            closeBtn.Location = new Point(
                ClientSize.Width - closeBtn.Width - 25,
                25);

            closeBtn.MouseEnter += (s, e) =>
            {
                closeBtn.ForeColor = Color.Gray;
            };

            closeBtn.MouseLeave += (s, e) =>
            {
                closeBtn.ForeColor = Color.White;
            };

            closeBtn.Click += (s, e) => Close();

            Controls.Add(closeBtn);

            closeBtn.BringToFront();
        }
        
        private void Build(Employee emp)
        {
            var photo = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(40, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = LoadPhoto(emp)
            };

            var name = new Label
            {
                Text = emp.FullName,
                Font = new Font("Exo 2", 22, FontStyle.Bold),
                Location = new Point(220, 50),
                AutoSize = true
            };

            var info = new Label
            {
                Text =
                    $"Отдел: {emp.Department}\n" +
                    $"Должность: {emp.Position}\n" +
                    $"Уровень Доступа: {emp.ClearanceLevel}\n" +
                    $"Статус: {emp.Status}\n" +
                    $"Дата найма: {emp.HireDate:dd.MM.yyyy}" +
                    (emp.TerminationDate.HasValue
                        ? $"\nДата увольнения (ликвидации): {emp.TerminationDate:dd.MM.yyyy}"
                        : ""),

                Font = new Font("Exo 2", 12),
                Location = new Point(220, 110),
                AutoSize = true
            };

            sheet.Controls.Add(photo);
            sheet.Controls.Add(name);
            sheet.Controls.Add(info);
        }

        private Image LoadPhoto(Employee emp)
        {
            try
            {
                // 1. Если в базе есть путь (рекомендую добавить поле PhotoPath)
                if (!string.IsNullOrWhiteSpace(emp.PhotoPath))
                {
                    var dbPath = GetFullPath(emp.PhotoPath);

                    if (File.Exists(dbPath))
                        return Image.FromFile(dbPath);
                }

                // 2. fallback по Id
                var idPath = GetFullPath($"Photos/{emp.Id}.png");

                if (File.Exists(idPath))
                    return Image.FromFile(idPath);

                // 3. fallback по имени (как у тебя сейчас в базе)
                var namePath = GetFullPath($"Photos/{emp.FullName}.png");

                if (File.Exists(namePath))
                    return Image.FromFile(namePath);

                // 4. дефолт
                var defaultPath = GetFullPath("Photos/default.png");

                if (File.Exists(defaultPath))
                    return Image.FromFile(defaultPath);

                return SystemIcons.Application.ToBitmap();
            }
            catch
            {
                return SystemIcons.Application.ToBitmap();
            }
        }

        private string GetFullPath(string relativePath)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }
    }
}