using System.Drawing;
using race_game.Core;

namespace race_game.Graphics {
    public class Renderer {
        // Стили для отрисовки
        private readonly Pen _roadMarkingPen = new Pen(Color.White, 3);
        private readonly Brush _playerCarBrush = new SolidBrush(Color.Red);
        private readonly Brush _trafficCarBrush = new SolidBrush(Color.Gray);
        private readonly Brush _player2CarBrush = new SolidBrush(Color.Blue);
        private readonly Font _hudFont = new Font("Arial", 16);
        private readonly Font _pauseFont = new Font("Arial", 32);

        public void Render(System.Drawing.Graphics g, GameState state) {
            if (state.Status != GameState.GameStatus.Racing &&
                state.Status != GameState.GameStatus.Paused)
                return;

            // 1. Отрисовка дороги
            DrawRoad(g, state.Road);

            // 2. Отрисовка машин трафика
            foreach (var trafficCar in state.TrafficCars) {
                // Конвертируем Point в Rectangle для отрисовки
                var carRect = new Rectangle(
                    trafficCar.Bounds.X - 25,
                    trafficCar.Bounds.Y - 50,
                    50, 100);
                DrawCar(g, carRect, _trafficCarBrush);
            }

            // 3. Отрисовка игрока(ов)
            var player1Rect = new Rectangle(
                state.Player1.Position.X - 25,
                state.Player1.Position.Y - 50,
                50, 100);
            DrawCar(g, player1Rect, _playerCarBrush);

            if (state.IsMultiplayer && state.Player2 != null) {
                var player2Rect = new Rectangle(
                    state.Player2.Position.X - 25,
                    state.Player2.Position.Y - 50,
                    50, 100);
                DrawCar(g, player2Rect, _player2CarBrush);
            }

            // 4. Отрисовка HUD
            DrawHUD(g, state);
        }

        private void DrawRoad(System.Drawing.Graphics g, RoadState road) {
            // Асфальт
            g.FillRectangle(Brushes.DarkGray, 0, 0, 800, 600);

            // Разметка (движущаяся)
            for (int i = 0; i < 10; i++) {
                int yPos = (i * 100 + road.ScrollOffset) % 600;
                g.DrawLine(_roadMarkingPen, 400, yPos, 400, yPos + 50);
            }
        }

        private void DrawCar(System.Drawing.Graphics g, Rectangle bounds, Brush brush) {
            // 1. Кузов машины
            g.FillRectangle(brush, bounds);

            // 2. Окна
            g.FillRectangle(Brushes.LightBlue,
                bounds.X + 5,
                bounds.Y + 15,
                bounds.Width - 10,
                25);

            // 3. Фары
            g.FillEllipse(Brushes.Yellow,
                bounds.X + 5,
                bounds.Y + 5,
                10, 10);
            g.FillEllipse(Brushes.Yellow,
                bounds.X + bounds.Width - 15,
                bounds.Y + 5,
                10, 10);

            // 4. Колеса
            g.FillEllipse(Brushes.Black,
                bounds.X - 5,
                bounds.Y + 20,
                15, 25);
            g.FillEllipse(Brushes.Black,
                bounds.X + bounds.Width - 10,
                bounds.Y + 20,
                15, 25);
            g.FillEllipse(Brushes.Black,
                bounds.X - 5,
                bounds.Y + 55,
                15, 25);
            g.FillEllipse(Brushes.Black,
                bounds.X + bounds.Width - 10,
                bounds.Y + 55,
                15, 25);
        }

        private void DrawHUD(System.Drawing.Graphics g, GameState state) {
            string scoreText = $"Score: {state.Score}";
            g.DrawString(scoreText, _hudFont, Brushes.White, 20, 20);

            if (state.Status == GameState.GameStatus.Paused) {
                var pauseText = "PAUSED";
                var textSize = g.MeasureString(pauseText, _pauseFont);
                g.DrawString(pauseText, _pauseFont, Brushes.Yellow,
                    400 - textSize.Width / 2,
                    300 - textSize.Height / 2);
            }
        }
    }
}