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

        private readonly Label _name;

        private bool _hovered;

        public EmployeeCard(Employee employee)
        {
            _employee = employee;

            Size = new Size(150, 180);
            Margin = new Padding(20);

            Cursor = Cursors.Hand;

            DoubleBuffered = true;

            BackColor = Color.Transparent;
            Padding = new Padding(10);

            // ===== AVATAR =====
            var photo = new PictureBox
            {
                Dock = DockStyle.Top,
                Height = 120,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Image = LoadPhoto(employee)
            };

            // ===== NAME =====
            _name = new Label
            {
                Text = employee.FullName,
                Dock = DockStyle.Top,
                Height = 40,
                ForeColor = Color.White,
                Font = new Font("Exo 2", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true,
                BackColor = Color.Transparent
            };

            Controls.Add(_name);
            Controls.Add(photo);

            // ===== CLICK =====
            Click += OpenProfile;
            photo.Click += OpenProfile;
            _name.Click += OpenProfile;

            // ===== HOVER =====
            MouseEnter += CardMouseEnter;
            MouseLeave += CardMouseLeave;

            photo.MouseEnter += CardMouseEnter;
            photo.MouseLeave += CardMouseLeave;

            _name.MouseEnter += CardMouseEnter;
            _name.MouseLeave += CardMouseLeave;

            Paint += DrawBorder;
        }

        private void CardMouseEnter(object? sender, EventArgs e)
        {
            _hovered = true;

            BackColor = Color.FromArgb(40, 0, 0);

            Invalidate();
        }

        private void CardMouseLeave(object? sender, EventArgs e)
        {
            Point pos = PointToClient(Cursor.Position);

            if (ClientRectangle.Contains(pos))
                return;

            _hovered = false;

            BackColor = Color.Transparent;

            Invalidate();
        }

        private void DrawBorder(object? sender, PaintEventArgs e)
        {
            if (!_hovered)
                return;

            using var pen = new Pen(Color.FromArgb(180, 0, 0), 2);

            e.Graphics.DrawRectangle(
                pen,
                1,
                1,
                Width - 3,
                Height - 3
            );
        }

        private void OpenProfile(object? sender, EventArgs e)
        {
            new EmployeeDossierForm(_employee).ShowDialog();
        }

        private Image LoadPhoto(Employee emp)
        {
            try
            {
                return LoadImageSafe(emp.PhotoPath)
                    ?? LoadImageSafe($"Photos/{emp.Id}.png")
                    ?? LoadImageSafe($"Photos/{emp.FullName}.png")
                    ?? SystemIcons.Application.ToBitmap();
            }
            catch
            {
                return SystemIcons.Application.ToBitmap();
            }
        }

        private Image? LoadImageSafe(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var full = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                path);

            if (!File.Exists(full))
                return null;

            return Image.FromFile(full);
        }
    }
}