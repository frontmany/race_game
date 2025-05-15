using race_game.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

static class Program {
    [STAThread]
    static void Main() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var mainForm = new Form {
            Text = "Racing Game",
            FormBorderStyle = FormBorderStyle.None, // Убираем рамку
            WindowState = FormWindowState.Maximized, // Полноэкранный режим
            Bounds = Screen.PrimaryScreen.Bounds // Занимаем весь экран
        };

        // Обработчик для выхода по Alt+F4 или Escape
        mainForm.KeyPreview = true;
        mainForm.KeyDown += (s, e) => {
            if (e.KeyCode == Keys.Escape || (e.Alt && e.KeyCode == Keys.F4))
                Application.Exit();
        };

        new GameEngine(mainForm);
        Application.Run(mainForm);
    }
}