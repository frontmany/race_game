using System.Drawing.Drawing2D;

namespace race_game.Core {
    public class Renderer {
        private readonly Pen    m_road_marking_pen;
        private readonly Pen    m_road_edge_pen;
        private readonly Brush  m_player_car_brush;
        private readonly Brush  m_traffic_car_brush ;
        private readonly Brush  m_player_2_car_brush;
        private readonly Font   m_hud_font;
        private readonly Brush  m_grass_brush;
        private int             m_main_form_width, m_main_form_height;

        public Renderer(int mainFormWidth, int mainFormHeight) {
            
            m_main_form_width = mainFormWidth;
            m_main_form_height = mainFormHeight;
            m_road_marking_pen = new Pen(Color.White, 2);
            m_road_edge_pen = new Pen(Color.DarkGray, 3);
            m_player_car_brush = new SolidBrush(Color.Red);
            m_traffic_car_brush = new SolidBrush(Color.Gray);
            m_player_2_car_brush = new SolidBrush(Color.Blue);
            m_hud_font = new Font("Arial", 16);
            m_grass_brush = new SolidBrush(Color.ForestGreen);
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
                DrawCar(g, carRect, m_traffic_car_brush);
            }

            var player1Rect = new Rectangle(
                state.Player1.Position.X - 25,
                state.Player1.Position.Y - 50,
                50, 90);
            DrawCar(g, player1Rect, m_player_car_brush);

            if (state.IsMultiplayer && state.Player2 != null) {
                var player2Rect = new Rectangle(
                    state.Player2.Position.X - 25,
                    state.Player2.Position.Y - 50,
                    50, 90);
                DrawCar(g, player2Rect, m_player_2_car_brush);
            }

            DrawHUD(g, state);
        }



        private void DrawSinglePlayerRoad(Graphics g, RoadState road) {
            int roadWidth = m_main_form_width / 2;
            int roadLeft = (m_main_form_width - roadWidth) / 2;

            g.FillRectangle(m_grass_brush, 0, 0, roadLeft, m_main_form_height);
            g.FillRectangle(m_grass_brush, roadLeft + roadWidth, 0, roadLeft, m_main_form_height);

            g.FillRectangle(Brushes.DarkGray, roadLeft, 0, roadWidth, m_main_form_height);

            int laneWidth = roadWidth / 3;
            for (int i = 0; i < 20; i++) {
                int yPos = (i * 100 + road.ScrollOffset) % m_main_form_height;

                g.DrawLine(m_road_marking_pen,
                    roadLeft + laneWidth, yPos,
                    roadLeft + laneWidth, yPos + 50);

                g.DrawLine(m_road_marking_pen,
                    roadLeft + laneWidth * 2, yPos,
                    roadLeft + laneWidth * 2, yPos + 50);
            }

            g.DrawLine(m_road_edge_pen, roadLeft, 0, roadLeft, m_main_form_height);
            g.DrawLine(m_road_edge_pen, roadLeft + roadWidth, 0, roadLeft + roadWidth, m_main_form_height);
        }

        private void DrawMultiplayerRoad(Graphics g, RoadState road) {
            int roadWidth = m_main_form_width / 2 - 20;
            int road1Left = 10;
            int road2Left = m_main_form_width / 2 + 10;

            for (int i = 0; i < 2; i++) {
                int currentRoadLeft = i == 0 ? road1Left : road2Left;

                g.FillRectangle(m_grass_brush, currentRoadLeft - 10, 0, 10, m_main_form_height);
                g.FillRectangle(m_grass_brush, currentRoadLeft + roadWidth, 0, 10, m_main_form_height);

                g.FillRectangle(Brushes.DarkGray, currentRoadLeft, 0, roadWidth, m_main_form_height);

                int laneWidth = roadWidth / 2;
                for (int j = 0; j < 20; j++) {
                    int yPos = (j * 100 + road.ScrollOffset) % m_main_form_height;

                    g.DrawLine(m_road_marking_pen,
                        currentRoadLeft + laneWidth, yPos,
                        currentRoadLeft + laneWidth, yPos + 50);
                }

                g.DrawLine(m_road_edge_pen, currentRoadLeft, 0, currentRoadLeft, m_main_form_height);
                g.DrawLine(m_road_edge_pen, currentRoadLeft + roadWidth, 0, currentRoadLeft + roadWidth, m_main_form_height);
            }
        }

        private void DrawCar(Graphics g, Rectangle bounds, Brush brush) {
            // body
            using (GraphicsPath carBody = new GraphicsPath()) {
                float cornerRadius = 10f;

                carBody.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90);
                carBody.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270, 90);
                carBody.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                carBody.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                carBody.CloseFigure();

                g.FillPath(brush, carBody);
            }

            // window
            using (GraphicsPath windowPath = new GraphicsPath()) {
                float cornerRadius = 10f;

                Rectangle windowBounds = new Rectangle(bounds.X + 7, bounds.Y + 20, bounds.Width - 15, 20);
                windowPath.AddArc(windowBounds.X, windowBounds.Y, cornerRadius, cornerRadius, 180, 90);
                windowPath.AddArc(windowBounds.X + windowBounds.Width - cornerRadius, windowBounds.Y, cornerRadius, cornerRadius, 270, 90);
                windowPath.AddArc(windowBounds.X + windowBounds.Width - cornerRadius, windowBounds.Y + windowBounds.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                windowPath.AddArc(windowBounds.X, windowBounds.Y + windowBounds.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                windowPath.CloseFigure();

                g.FillPath(Brushes.LightBlue, windowPath);
            }

            // lights
            g.FillEllipse(Brushes.Yellow,
                bounds.X + 5,
                bounds.Y + 5,
                10, 10);
            g.FillEllipse(Brushes.Yellow,
                bounds.X + bounds.Width - 15,
                bounds.Y + 5,
                10, 10);

            // wheels
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
            g.DrawString(player1ScoreText, m_hud_font, player1ScoreBrush, 20, 20);

            if (state.IsMultiplayer) {
                string player2ScoreText = $"Score: {state.Player2Score}";
                float textWidth = g.MeasureString(player2ScoreText, m_hud_font).Width;
                g.DrawString(player2ScoreText, m_hud_font, player2ScoreBrush,
                             m_main_form_width - textWidth - 20, 20);
            }
        }
    }
}