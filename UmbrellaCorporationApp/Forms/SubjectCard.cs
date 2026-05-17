using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class SubjectCard : Panel
{
    public SubjectCard(TestSubject subject)
    {
        Width = 500;

        Height = 300;

        Margin = new Padding(60);
        
        Padding = new Padding(15);

        BackColor = Color.FromArgb(45, 0, 0);

        BorderStyle = BorderStyle.FixedSingle;

        Cursor = Cursors.Hand;

        InitializeUI(subject);
    }

    private void InitializeUI(TestSubject subject)
    {
        var code = new Label
        {
            Text = subject.Code,

            Font = new Font("Exo 2", 16, FontStyle.Bold),

            ForeColor = Color.White,

            AutoSize = true,

            Location = new Point(20, 20)
        };

        Controls.Add(code);

        int startY = 70;
        int gap = 40;

        var virus = CreateInfo(
            $"Вирус: {subject.Virus?.Name ?? "НЕТ"}",
            startY);

        Controls.Add(virus);

        var status = CreateInfo(
            $"Статус: {subject.Status}",
            startY + gap);

        Controls.Add(status);

        var location = CreateInfo(
            $"Местонахождение: {subject.Location}",
            startY + gap * 2);

        Controls.Add(location);

        var date = CreateInfo(
            $"Дата появления: {subject.AcquiredDate:dd.MM.yyyy}",
            startY + gap * 3);

        Controls.Add(date);

        var notes = CreateInfo(
            $"Заметки: {subject.Notes}",
            startY + gap * 4);

        Controls.Add(notes);
    }

    private Label CreateInfo(string text, int y)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Exo 2", 11),
            ForeColor = Color.Gainsboro,
            AutoSize = true,
            Location = new Point(20, y)
        };
    }
}