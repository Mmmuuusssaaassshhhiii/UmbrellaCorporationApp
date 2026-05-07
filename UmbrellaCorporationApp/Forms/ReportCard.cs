using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class ReportCard : Panel
{
    private readonly LabReport _report;

    private readonly Label _title;

    private bool _expanded;

    private bool _hovered;

    private readonly int _collapsedHeight = 150;

    private readonly int _expandedHeight = 240;

    private static ReportCard? _currentlyExpanded;

    public ReportCard(LabReport report)
    {
        _report = report;

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
            Text = report.Title,

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

        DoubleClick += OpenReport;

        icon.DoubleClick += OpenReport;

        _title.DoubleClick += OpenReport;

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

    private void OpenReport(object? sender, EventArgs e)
    {
        new ReportViewerForm(_report).ShowDialog();
    }

    private Image LoadTransparentImage()
    {
        string path = "Document.png";

        if (!System.IO.File.Exists(path))
        {
            return SystemIcons.Application.ToBitmap();
        }

        Bitmap bmp = new Bitmap(path);

        bmp.MakeTransparent();

        return bmp;
    }
}