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

        private float m_trafficSpawnRate = 1.0f; 
        private const float m_maxTrafficSpawnRate = 0.3f; 
        private const float m_difficultyIncreaseInterval = 30.0f;

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
        public int CrashedPlayerNumber { get; set; }
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
            moveSpeed = 5;
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
            if (Status != GameStatus.Racing || IsPaused) return;

            UpdateDifficulty(); 
            UpdatePlayerMovement();
            CheckOffRoad();
            UpdateRoadAndTraffic();
            CheckCollisions();
            UpdateGameProgress();
        }

        private void UpdateDifficulty() {
            m_trafficSpawnRate = Math.Max(
                m_maxTrafficSpawnRate,
                1.0f - (float)(ElapsedTime.TotalSeconds / 120.0)
            );

            moveSpeed = 5 + (int)(ElapsedTime.TotalSeconds / 30.0);
            moveSpeed = Math.Min(moveSpeed, 10);
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
                    trafficCar.Bounds.Y + (int)(trafficCar.Speed * GameSpeed),
                    trafficCar.Bounds.Width,
                    trafficCar.Bounds.Height
                );
            }

            if (m_random.NextDouble() < (1.0 / (60.0 * m_trafficSpawnRate))) {
                if (IsMultiplayer) {
                    TrySpawnTrafficCar(10, m_width / 2 - 70); 
                    TrySpawnTrafficCar(m_width / 2 + 70, m_width - 10); 
                }
                else {
                    TrySpawnTrafficCar(roadLeft + 50, roadLeft + roadWidth - 50);
                }
            }

            TrafficCars.RemoveAll(car => car.Bounds.Bottom > m_height + playerHeight + 50);
        }

        private void TrySpawnTrafficCar(int minX, int maxX) {
            const int carWidth = 55;
            const int carHeight = 90;
            const int spawnHeight = -100;
            const int safetyMargin = 25; 

            int attempts = 5; 
            bool spawned = false;

            while (attempts-- > 0 && !spawned) {
                int xPos = m_random.Next(minX, maxX - carWidth);
                float speed = 3 + (float)m_random.NextDouble() * 3.0f;

                var newCarRect = new Rectangle(xPos, spawnHeight, carWidth, carHeight);

                bool canSpawn = true;

                foreach (var existingCar in TrafficCars) {
                    var safetyRect = new Rectangle(
                        existingCar.Bounds.X - safetyMargin,
                        existingCar.Bounds.Y,
                        existingCar.Bounds.Width + safetyMargin * 2,
                        existingCar.Bounds.Height
                    );

                    if (newCarRect.IntersectsWith(safetyRect)) {
                        if (speed > existingCar.Speed) {
                            canSpawn = false;
                            break;
                        }
                    }

                   
                    if (existingCar.Bounds.X < xPos + carWidth &&
                        existingCar.Bounds.X + existingCar.Bounds.Width > xPos &&
                        existingCar.Bounds.Y > newCarRect.Y &&
                        existingCar.Speed < speed) {
                        canSpawn = false;
                        break;
                    }
                }

                if (canSpawn) {
                    TrafficCars.Add(new TrafficCarState {
                        Bounds = newCarRect,
                        Speed = speed
                    });
                    spawned = true;
                }
            }
        }


        private void UpdateGameProgress() {
            Player1Score += (int)(10 * GameSpeed);
            if (IsMultiplayer) Player2Score += (int)(10 * GameSpeed);

            GameSpeed = Math.Min(1.5f + (float)(ElapsedTime.TotalSeconds / 120.0), 2.5f);
        }
    

    private void CheckCollisions() {
            var player1Rect = new Rectangle(Player1.Position.X, Player1.Position.Y, 50, 100);
            foreach (var trafficCar in TrafficCars) {
                if (player1Rect.IntersectsWith(trafficCar.Bounds)) {
                    Status = GameStatus.GameOver;
                    CrashedPlayerNumber = 1;
                    return;
                }
            }

            if (IsMultiplayer && Player2 != null) {
                var player2Rect = new Rectangle(Player2.Position.X, Player2.Position.Y, 50, 100);
                foreach (var trafficCar in TrafficCars) {
                    if (player2Rect.IntersectsWith(trafficCar.Bounds)) {
                        Status = GameStatus.GameOver;
                        CrashedPlayerNumber = 2;
                        return;
                    }
                }
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