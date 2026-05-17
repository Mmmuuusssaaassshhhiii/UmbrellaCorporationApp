using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class FileCard : Panel
{
    private readonly ClassifiedFile _file;

    private readonly Label _title;

    private bool _expanded;
    private bool _hovered;

    private readonly int _collapsedHeight = 150;
    private readonly int _expandedHeight = 240;

    private static FileCard? _currentlyExpanded;

    public FileCard(ClassifiedFile file)
    {
        _file = file;

        Size = new Size(150, _collapsedHeight);
        Margin = new Padding(60);
        Cursor = Cursors.Hand;
        BorderStyle = BorderStyle.None;
        DoubleBuffered = true;
        Padding = new Padding(15);
        
        BackColor = Color.Transparent;

        var icon = new PictureBox
        {
            Dock = DockStyle.Top,
            Height = 90,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            BorderStyle = BorderStyle.None,
            Image = LoadTransparentImage()
        };

        _title = new Label
        {
            Text = file.Title,
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Exo 2", 10, FontStyle.Bold),
            TextAlign = ContentAlignment.TopCenter,
            BackColor = Color.Transparent,
            AutoEllipsis = true
        };

        Controls.Add(_title);
        Controls.Add(icon);

        Click += ToggleExpand;
        icon.Click += ToggleExpand;
        _title.Click += ToggleExpand;

        DoubleClick += OpenFile;
        icon.DoubleClick += OpenFile;
        _title.DoubleClick += OpenFile;

        MouseEnter += OnHover;
        MouseLeave += OnLeave;
        icon.MouseEnter += OnHover;
        icon.MouseLeave += OnLeave;
        _title.MouseEnter += OnHover;
        _title.MouseLeave += OnLeave;
        
        ParentChanged += (_, _) =>
        {
            if (Parent != null)
            {
                Parent.Click += (_, _) =>
                {
                    Collapse();
                };
            }
        };

        Paint += DrawEffects;
    }

    private void DrawEffects(object? sender, PaintEventArgs e)
    {
        if (!_hovered && !_expanded)
            return;

        using var pen = new Pen(Color.FromArgb(180, 0, 0), 2);

        e.Graphics.DrawRectangle(
            pen,
            1,
            1,
            Width - 3,
            Height - 3);
    }

    private void ToggleExpand(object? sender, EventArgs e)
    {
        if (_currentlyExpanded != null && _currentlyExpanded != this)
            _currentlyExpanded.Collapse();

        _expanded = !_expanded;

        if (_expanded)
        {
            _currentlyExpanded = this;
            Height = _expandedHeight;

            _title.AutoEllipsis = false;
            _title.Dock = DockStyle.Bottom;
            _title.Height = 80;
        }
        else
        {
            Collapse();
        }

        Invalidate();
    }

    private void Collapse()
    {
        _expanded = false;
        Height = _collapsedHeight;

        _title.AutoEllipsis = true;
        _title.Dock = DockStyle.Fill;

        if (_currentlyExpanded == this)
            _currentlyExpanded = null;

        Invalidate();
    }

    private void OnHover(object? sender, EventArgs e)
    {
        _hovered = true;
        Invalidate();
    }

    private void OnLeave(object? sender, EventArgs e)
    {
        _hovered = false;
        Invalidate();
    }

    private void OpenFile(object? sender, EventArgs e)
    {
        new FileViewerForm(_file).ShowDialog();
    }

    private Image LoadTransparentImage()
    {
        string path = "SecretFile.png";

        if (!System.IO.File.Exists(path))
            return SystemIcons.Application.ToBitmap();

        using var bmp = new Bitmap(path);
        return new Bitmap(bmp);
    }
}