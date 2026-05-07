using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI
{
    public class EmployeeCard : Panel
    {
        private readonly Employee _employee;

        public EmployeeCard(Employee employee)
        {
            _employee = employee;

            Size = new Size(200, 220);
            Margin = new Padding(15);
            BackColor = Color.FromArgb(45, 0, 0);
            Cursor = Cursors.Hand;

            DoubleBuffered = true;

            var photo = new PictureBox
            {
                Dock = DockStyle.Top,
                Height = 130,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = LoadPhoto(employee)
            };

            var name = new Label
            {
                Text = employee.FullName,
                Dock = DockStyle.Top,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Exo 2", 10, FontStyle.Bold),
                Height = 30
            };
            
            Controls.Add(name);
            Controls.Add(photo);

            Click += OpenProfile;
            photo.Click += OpenProfile;
            name.Click += OpenProfile;

            MouseEnter += (_, _) => BackColor = Color.FromArgb(80, 0, 0);
            MouseLeave += (_, _) => BackColor = Color.FromArgb(45, 0, 0);
        }

        private void OpenProfile(object? sender, EventArgs e)
        {
            new EmployeeDossierForm(_employee).ShowDialog();
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