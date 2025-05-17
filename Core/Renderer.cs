using System.Drawing;
using System.Drawing.Drawing2D;

namespace race_game.Core {
    public class Renderer {
        private readonly Pen _roadMarkingPen = new Pen(Color.White, 2);
        private readonly Pen _roadEdgePen = new Pen(Color.DarkGray, 3);
        private readonly Brush _playerCarBrush = new SolidBrush(Color.Red);
        private readonly Brush _trafficCarBrush = new SolidBrush(Color.Gray);
        private readonly Brush _player2CarBrush = new SolidBrush(Color.Blue);
        private readonly Font _hudFont = new Font("Arial", 16);
        private readonly Font _pauseFont = new Font("Arial", 32);
        private readonly Brush _grassBrush = new SolidBrush(Color.ForestGreen);
        private readonly Brush _treeBrush = new SolidBrush(Color.DarkGreen);

        int m_width, m_height;

        public Renderer(int width, int height) {
            m_width = width;
            m_height = height;
        }

        public void Render(Graphics g, GameState state) {
            if (state.Status != GameState.GameStatus.Racing &&
                state.Status != GameState.GameStatus.Paused)
                return;

            if (state.IsMultiplayer) {
                DrawMultiplayerRoad(g, state.RoadState);
            }
            else {
                DrawSinglePlayerRoad(g, state.RoadState);
            }

            foreach (var trafficCar in state.TrafficCars) {
                var carRect = new Rectangle(
                    trafficCar.Bounds.X - 25,
                    trafficCar.Bounds.Y - 50,
                    50, 90);
                DrawCar(g, carRect, _trafficCarBrush);
            }

            var player1Rect = new Rectangle(
                state.Player1.Position.X - 25,
                state.Player1.Position.Y - 50,
                50, 90);
            DrawCar(g, player1Rect, _playerCarBrush);

            if (state.IsMultiplayer && state.Player2 != null) {
                var player2Rect = new Rectangle(
                    state.Player2.Position.X - 25,
                    state.Player2.Position.Y - 50,
                    50, 90);
                DrawCar(g, player2Rect, _player2CarBrush);
            }

            DrawHUD(g, state);
        }

        private void DrawSinglePlayerRoad(Graphics g, RoadState road) {
            int roadWidth = m_width / 2;
            int roadLeft = (m_width - roadWidth) / 2;
    

            g.FillRectangle(_grassBrush, 0, 0, roadLeft, m_height); 
            g.FillRectangle(_grassBrush, roadLeft + roadWidth, 0, roadLeft, m_height);

            g.FillRectangle(Brushes.DarkGray, roadLeft, 0, roadWidth, m_height);

            int laneWidth = roadWidth / 3;
            for (int i = 0; i < 20; i++) {
                int yPos = (i * 100 + road.ScrollOffset) % m_height;

                g.DrawLine(_roadMarkingPen,
                    roadLeft + laneWidth, yPos,
                    roadLeft + laneWidth, yPos + 50);

                g.DrawLine(_roadMarkingPen,
                    roadLeft + laneWidth * 2, yPos,
                    roadLeft + laneWidth * 2, yPos + 50);
            }

            g.DrawLine(_roadEdgePen, roadLeft, 0, roadLeft, m_height);
            g.DrawLine(_roadEdgePen, roadLeft + roadWidth, 0, roadLeft + roadWidth, m_height);
        }

        private void DrawMultiplayerRoad(Graphics g, RoadState road) {
            int roadWidth = m_width / 2 - 20;
            int road1Left = 10;
            int road2Left = m_width / 2 + 10;

            for (int i = 0; i < 2; i++) {
                int currentRoadLeft = i == 0 ? road1Left : road2Left;

                g.FillRectangle(_grassBrush, currentRoadLeft - 10, 0, 10, m_height); // Левая граница
                g.FillRectangle(_grassBrush, currentRoadLeft + roadWidth, 0, 10, m_height); // Правая граница

                g.FillRectangle(Brushes.DarkGray, currentRoadLeft, 0, roadWidth, m_height);

                int laneWidth = roadWidth / 2;
                for (int j = 0; j < 20; j++) {
                    int yPos = (j * 100 + road.ScrollOffset) % m_height;

                    g.DrawLine(_roadMarkingPen,
                        currentRoadLeft + laneWidth, yPos,
                        currentRoadLeft + laneWidth, yPos + 50);
                }

                g.DrawLine(_roadEdgePen, currentRoadLeft, 0, currentRoadLeft, m_height);
                g.DrawLine(_roadEdgePen, currentRoadLeft + roadWidth, 0, currentRoadLeft + roadWidth, m_height);
            }
        }

        private void DrawCar(Graphics g, Rectangle bounds, Brush brush) {
            // 1. Кузов машины (с закругленными углами)
            using (GraphicsPath carBody = new GraphicsPath()) {
                float cornerRadius = 10f; // Радиус закругления

                carBody.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90); // Верхний левый угол
                carBody.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270, 90); // Верхний правый угол
                carBody.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90); // Нижний правый угол
                carBody.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90); // Нижний левый угол
                carBody.CloseFigure();

                g.FillPath(brush, carBody);
            }

            // 2. Окна (с закругленными углами)
            using (GraphicsPath windowPath = new GraphicsPath()) {
                float cornerRadius = 10f;

                Rectangle windowBounds = new Rectangle(bounds.X + 7, bounds.Y + 20, bounds.Width - 15, 20);
                windowPath.AddArc(windowBounds.X, windowBounds.Y, cornerRadius, cornerRadius, 180, 90); // Верхний левый угол
                windowPath.AddArc(windowBounds.X + windowBounds.Width - cornerRadius, windowBounds.Y, cornerRadius, cornerRadius, 270, 90); // Верхний правый угол
                windowPath.AddArc(windowBounds.X + windowBounds.Width - cornerRadius, windowBounds.Y + windowBounds.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90); // Нижний правый угол
                windowPath.AddArc(windowBounds.X, windowBounds.Y + windowBounds.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90); // Нижний левый угол
                windowPath.CloseFigure();

                g.FillPath(Brushes.LightBlue, windowPath);
            }

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
                10, 25);
            g.FillEllipse(Brushes.Black,
                bounds.X + bounds.Width - 5,
                bounds.Y + 20,
                10, 25);
            g.FillEllipse(Brushes.Black,
                bounds.X - 5,
                bounds.Y + 55,
                10, 25);
            g.FillEllipse(Brushes.Black,
                bounds.X + bounds.Width - 5,
                bounds.Y + 55,
                10, 25);
        }



        private void DrawHUD(Graphics g, GameState state) {
            Brush player1ScoreBrush = state.IsFirstPlayerOnGrass ? Brushes.Red : Brushes.White;
            Brush player2ScoreBrush = state.IsSecondPlayerOnGrass ? Brushes.Red : Brushes.White;

            string player1ScoreText = $"Score: {state.Player1Score}";
            g.DrawString(player1ScoreText, _hudFont, player1ScoreBrush, 20, 20);

            if (state.IsMultiplayer) {
                string player2ScoreText = $"Score: {state.Player2Score}";
                float textWidth = g.MeasureString(player2ScoreText, _hudFont).Width;
                g.DrawString(player2ScoreText, _hudFont, player2ScoreBrush,
                             m_width - textWidth - 20, 20);
            }
        }
    }
}