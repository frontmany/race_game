using System.Drawing;
using System.Windows.Forms;

namespace race_game.Graphics {
    public class MainMenuRenderer {
        private readonly StringFormat _centerFormat = new StringFormat {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public void Render(System.Drawing.Graphics g, Rectangle bounds, int selectedIndex, string[] menuItems) {
            // 1. Фон
            g.Clear(Color.Navy);

            // 2. Заголовок (точно по центру)
            RenderTitle(g, bounds);

            // 3. Кнопки меню (центрированная группа)
            RenderMenuButtons(g, bounds, selectedIndex, menuItems);

            // 4. Подсказка (по центру внизу)
            RenderHint(g, bounds);
        }

        private void RenderTitle(System.Drawing.Graphics g, Rectangle bounds) {
            using (var titleFont = new Font("Arial", 36, FontStyle.Bold)) {
                var titleY = bounds.Height * 0.15f; // 15% от высоты
                g.DrawString("RACING GAME", titleFont, Brushes.Gold,
                    bounds.Width / 2, titleY, _centerFormat);
            }
        }

        private void RenderMenuButtons(System.Drawing.Graphics g, Rectangle bounds, int selectedIndex, string[] menuItems) {
            const int buttonWidth = 300;
            const int buttonHeight = 50;
            const int buttonSpacing = 10;

            // Общая высота всех кнопок с промежутками
            int totalHeight = (buttonHeight + buttonSpacing) * menuItems.Length - buttonSpacing;

            // Начальная позиция Y для центрирования группы кнопок
            int startY = (bounds.Height - totalHeight) / 2;

            for (int i = 0; i < menuItems.Length; i++) {
                var buttonRect = new Rectangle(
                    (bounds.Width - buttonWidth) / 2,
                    startY + i * (buttonHeight + buttonSpacing),
                    buttonWidth,
                    buttonHeight);

                // Фон кнопки
                g.FillRectangle(
                    selectedIndex == i ? Brushes.DarkBlue : Brushes.Transparent,
                    buttonRect);

                // Текст кнопки
                using (var font = new Font("Arial", 24,
                    selectedIndex == i ? FontStyle.Bold : FontStyle.Regular)) {
                    g.DrawString(menuItems[i], font,
                        selectedIndex == i ? Brushes.Yellow : Brushes.White,
                        buttonRect, _centerFormat);
                }
            }
        }

        private void RenderHint(System.Drawing.Graphics g, Rectangle bounds) {
            using (var hintFont = new Font("Arial", 12)) {
                var hintY = bounds.Height - 50;
                g.DrawString("Use ↑↓ arrows to select, ENTER to confirm",
                    hintFont, Brushes.Silver,
                    bounds.Width / 2, hintY, _centerFormat);
            }
        }
    }
}