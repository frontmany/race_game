using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace race_game.Core {
    public class GameState {
        private Random m_random;
        private int m_width, m_height;
        private int moveSpeed = 5;
        private const int playerWidth = 50;
        private const int playerHeight = 100;
        private int roadLeft, roadWidth;

        public GameState(int width, int height) {
            m_random = new Random();
            HashSetPressedKeys = new HashSet<Keys>();
            m_width = width;
            m_height = height;

            roadWidth = IsMultiplayer ? m_width / 2 - 20 : m_width / 2;
            roadLeft = IsMultiplayer ? 10 : (m_width - roadWidth) / 2;
        }

        public HashSet<Keys> HashSetPressedKeys { get; set; }
        public bool IsMultiplayer { get; set; }
        public bool IsFirstPlayerOnGrass { get; set; }
        public bool IsSecondPlayerOnGrass { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public float GameSpeed { get; set; } = 1.0f;
        public bool IsPaused { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public PlayerState Player1 { get; set; }
        public PlayerState Player2 { get; set; }
        public List<TrafficCarState> TrafficCars { get; } = new();
        public RoadState RoadState { get; set; }
        public GameStatus Status { get; set; } = GameStatus.Racing;

        public enum GameStatus {
            Racing,
            Paused,
            GameOver
        }

        public void Init(bool isMultiplayer) {
            HashSetPressedKeys.Clear();
            IsMultiplayer = isMultiplayer;
            Player1Score = 0;
            Player2Score = 0;
            GameSpeed = 1.5f;
            IsPaused = false;
            ElapsedTime = TimeSpan.Zero;
            Status = GameStatus.Racing;

            roadWidth = IsMultiplayer ? m_width / 2 - 20 : m_width / 2;
            roadLeft = IsMultiplayer ? 10 : (m_width - roadWidth) / 2;

            Player1 = new PlayerState {
                Position = new Point(m_width / 2 - 10, 800),
                Color = Color.Red
            };

            if (IsMultiplayer) {

                Player1 = new PlayerState {
                    Position = new Point(m_width / 4, 800),
                    Color = Color.Red
                };

                Player2 = new PlayerState {
                    Position = new Point(m_width * 3 / 4, 800),
                    Color = Color.Blue
                };
            }
            else {
                Player1 = new PlayerState {
                    Position = new Point(m_width / 2, 800), 
                    Color = Color.Red
                };

                Player2 = null;
            }

            RoadState = new RoadState {
                ScrollOffset = 0,
                SegmentCount = 10
            };

            TrafficCars.Clear();
        }

        public void Update() {
            if (Status != GameStatus.Racing || IsPaused)
                return;

            UpdatePlayerMovement();
            CheckOffRoad();
            UpdateRoadAndTraffic();
            CheckCollisions();
            UpdateGameProgress();
        }

        private void UpdatePlayerMovement() {
            if (Player1 != null) {
                if (HashSetPressedKeys.Contains(Keys.W)) {
                    Player1.Position = new Point(
                        Player1.Position.X,
                        Math.Max(Player1.Position.Y - moveSpeed, 100));
                }
                if (HashSetPressedKeys.Contains(Keys.S)) {
                    Player1.Position = new Point(
                        Player1.Position.X,
                        Math.Min(Player1.Position.Y + moveSpeed, m_height - playerHeight));
                }
                if (HashSetPressedKeys.Contains(Keys.A)) {
                    Player1.Position = new Point(
                        Math.Max(Player1.Position.X - moveSpeed, playerWidth),
                        Player1.Position.Y);
                }
                if (HashSetPressedKeys.Contains(Keys.D)) {
                    Player1.Position = new Point(
                        Math.Min(Player1.Position.X + moveSpeed, m_width - playerWidth),
                        Player1.Position.Y);
                }
            }

            if (IsMultiplayer && Player2 != null) {
                if (HashSetPressedKeys.Contains(Keys.Up)) {
                    Player2.Position = new Point(
                        Player2.Position.X,
                        Math.Max(Player2.Position.Y - moveSpeed, 100));
                }
                if (HashSetPressedKeys.Contains(Keys.Down)) {
                    Player2.Position = new Point(
                        Player2.Position.X,
                        Math.Min(Player2.Position.Y + moveSpeed, m_height - playerHeight));
                }
                if (HashSetPressedKeys.Contains(Keys.Left)) {
                    Player2.Position = new Point(
                        Math.Max(Player2.Position.X - moveSpeed, playerWidth),
                        Player2.Position.Y);
                }
                if (HashSetPressedKeys.Contains(Keys.Right)) {
                    Player2.Position = new Point(
                        Math.Min(Player2.Position.X + moveSpeed, m_width - playerWidth),
                        Player2.Position.Y);
                }
            }
        }

        private void CheckOffRoad() {
            if (Player1 != null) {
                IsFirstPlayerOnGrass = false;

                if (IsMultiplayer) {
                    IsFirstPlayerOnGrass = Player1.Position.X > m_width / 2 - 10;
                }
                else {
                    IsFirstPlayerOnGrass = Player1.Position.X < roadLeft ||
                               Player1.Position.X > roadLeft + roadWidth;
                }

                if (IsFirstPlayerOnGrass) {
                    Player1Score = Math.Max(0, Player1Score - (int)(20 * GameSpeed));
                }
            }

            if (IsMultiplayer && Player2 != null) {
                IsSecondPlayerOnGrass = Player2.Position.X < m_width / 2 + 10;

                if (IsSecondPlayerOnGrass) {
                    Player2Score = Math.Max(0, Player2Score - (int)(20 * GameSpeed));
                }
            }
        }

        private void UpdateRoadAndTraffic() {
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1 / 60f));
            RoadState.ScrollOffset = (RoadState.ScrollOffset + (int)(5 * GameSpeed)) % 100;

            foreach (var trafficCar in TrafficCars) {
                trafficCar.Bounds = new Rectangle(
                    trafficCar.Bounds.X,
                    trafficCar.Bounds.Y + (int)(GameSpeed * 3),
                    trafficCar.Bounds.Width,
                    trafficCar.Bounds.Height);
            }

            if (ElapsedTime.TotalSeconds % 1 < 1 / 60f) {
                if (GetRandomBool()) {
                    int lane = m_random.Next(IsMultiplayer ? 2 : 3);
                    int xPos = IsMultiplayer ?
                        (lane == 0 ? m_random.Next(100, 300) : m_random.Next(400, 600)) :
                        m_random.Next(roadLeft + 50, roadLeft + roadWidth - 50);

                    TrafficCars.Add(new TrafficCarState {
                        Bounds = new Rectangle(xPos, -100, 50, 100),
                        Speed = 3 + (float)m_random.NextDouble()
                    });
                }
            }

            for (int i = TrafficCars.Count - 1; i >= 0; i--) {
                if (TrafficCars[i].Bounds.Bottom > m_height + playerHeight + 50) {
                    TrafficCars.RemoveAt(i);
                }
            }
        }

        private void CheckCollisions() {
            var player1Rect = new Rectangle(Player1.Position.X, Player1.Position.Y, 50, 100);
            foreach (var trafficCar in TrafficCars) {
                if (player1Rect.IntersectsWith(trafficCar.Bounds)) {
                    Status = GameStatus.GameOver;
                    return;
                }
            }

            if (IsMultiplayer && Player2 != null) {
                var player2Rect = new Rectangle(Player2.Position.X, Player2.Position.Y, 50, 100);
                foreach (var trafficCar in TrafficCars) {
                    if (player2Rect.IntersectsWith(trafficCar.Bounds)) {
                        Status = GameStatus.GameOver;
                        return;
                    }
                }
            }
        }

        private void UpdateGameProgress() {
            Player1Score += (int)(10 * GameSpeed);
            if (IsMultiplayer) {
                Player2Score += (int)(10 * GameSpeed);
            }

            if (Player1Score % 500 == 0 || (IsMultiplayer && Player2Score % 500 == 0)) {
                GameSpeed = Math.Min(GameSpeed + 0.1f, 2.0f);
            }
            if (Player1Score % 5000 == 0 || (IsMultiplayer && Player2Score % 5000 == 0)) {
                moveSpeed++;
            }
        }

        public bool GetRandomBool() {
            return m_random.Next(2) == 0;
        }
    }

    public class PlayerState {
        public Point Position { get; set; }
        public Color Color { get; set; }
    }

    public class TrafficCarState {
        public Rectangle Bounds { get; set; }
        public float Speed { get; set; }
    }

    public class RoadState {
        public int ScrollOffset { get; set; }
        public int SegmentCount { get; set; }
    }
}