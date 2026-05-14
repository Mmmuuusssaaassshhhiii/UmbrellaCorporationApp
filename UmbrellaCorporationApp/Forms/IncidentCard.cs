using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class IncidentCard : Panel
{
    private readonly IncidentLog _log;

    private readonly Label _title;

    private bool _expanded;

    private bool _hovered;

    private readonly int _collapsedHeight = 150;

    private readonly int _expandedHeight = 240;

    private static IncidentCard? _currentlyExpanded;

    public IncidentCard(IncidentLog log)
    {
        _log = log;

        Size = new Size(150, _collapsedHeight);

        Margin = new Padding(60);

        Cursor = Cursors.Hand;

        DoubleBuffered = true;

        Padding = new Padding(15);

        BackColor = Color.Transparent;

        var icon = new PictureBox
        {
            Dock = DockStyle.Top,

            Height = 90,

            SizeMode = PictureBoxSizeMode.Zoom,

            BackColor = Color.Transparent,

            Image = LoadTransparentImage()
        };

        _title = new Label
        {
            Text = log.Type,

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

        DoubleClick += OpenViewer;

        icon.DoubleClick += OpenViewer;

        _title.DoubleClick += OpenViewer;

        MouseEnter += OnHover;

        MouseLeave += OnLeave;

        icon.MouseEnter += OnHover;

        icon.MouseLeave += OnLeave;

        _title.MouseEnter += OnHover;

        _title.MouseLeave += OnLeave;

        Paint += DrawEffects;
    }

    private void DrawEffects(object? sender, PaintEventArgs e)
    {
        if (!_hovered && !_expanded)
            return;

        using var pen = new Pen(Color.DarkRed, 2);

        e.Graphics.DrawRectangle(
            pen,
            1,
            1,
            Width - 3,
            Height - 3);
    }

    private void ToggleExpand(object? sender, EventArgs e)
    {
        if (_currentlyExpanded != null &&
            _currentlyExpanded != this)
        {
            _currentlyExpanded.Collapse();
        }

        _expanded = !_expanded;

        if (_expanded)
        {
            _currentlyExpanded = this;

            Height = _expandedHeight;

            _title.AutoEllipsis = false;

            _title.Dock = DockStyle.Bottom;

            _title.Height = 100;
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
        {
            _currentlyExpanded = null;
        }

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

    private void OpenViewer(object? sender, EventArgs e)
    {
        new IncidentViewerForm(_log).ShowDialog();
    }
    
    private Image LoadTransparentImage()
    {
        string path = "Incident.png";

        if (!System.IO.File.Exists(path))
        {
            return SystemIcons.Application.ToBitmap();
        }

        Bitmap bmp = new Bitmap(path);

        bmp.MakeTransparent();

        return bmp;
    }
}