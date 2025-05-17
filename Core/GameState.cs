using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace race_game.Core {
    public class GameState {
        private Random  m_random;
        private int     m_main_form_width, m_main_form_height;
        private int     m_move_speed;
        private int     m_player_width;
        private int     m_player_height;
        private int     m_road_left, m_road_width;
        private float   m_traffic_spawn_rate ; 
        private float   m_max_traffic_spawn_rate; 
        private float   m_difficulty_increase_interval;

        public enum GameStatus {
            Racing,
            Paused,
            GameOver
        }

        public GameState(int mainFormWidth, int mainFormHeight) {
            m_random = new Random();
            m_main_form_width = mainFormWidth;
            m_main_form_height = mainFormHeight;
            m_move_speed = 5;
            m_player_width = 50;
            m_player_height = 100;
            m_traffic_spawn_rate = 0.4f;
            m_max_traffic_spawn_rate = 0.001f;
            m_difficulty_increase_interval = 30.0f;
            m_road_width = IsMultiplayer ? m_main_form_width / 2 - 20 : m_main_form_width / 2;
            m_road_left = IsMultiplayer ? 10 : (m_main_form_width - m_road_width) / 2;
            HashSetPressedKeys = new HashSet<Keys>();
            GameSpeed = 1.0f;
            Status  = GameStatus.Racing;
            TrafficCars = new();
            Player1Score = 0;
            Player2Score = 0;
        }

        public HashSet<Keys> HashSetPressedKeys { get; set; }

        public bool IsMultiplayer { get; set; }

        public bool IsFirstPlayerOnGrass { get; set; }

        public bool IsSecondPlayerOnGrass { get; set; }

        public int CrashedPlayerNumber { get; set; }

        public int Player1Score { get; set; }

        public int Player2Score { get; set; }

        public float GameSpeed { get; set; }

        public bool IsPaused { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public PlayerState Player1 { get; set; }

        public PlayerState Player2 { get; set; }

        public List<TrafficCarState> TrafficCars { get; } = new();

        public RoadState RoadState { get; set; }

        public GameStatus Status { get; set; }

        public void Init(bool isMultiplayer) {
            HashSetPressedKeys.Clear();
            IsMultiplayer = isMultiplayer;
            IsPaused = false;
            ElapsedTime = TimeSpan.Zero;
            Status = GameStatus.Racing;

            Player1 = new PlayerState {
                Position = new Point(m_main_form_width / 2 - 10, 800),
                Color = Color.Red
            };

            if (IsMultiplayer) {
                Player1 = new PlayerState {
                    Position = new Point(m_main_form_width / 4, 800),
                    Color = Color.Red
                };

                Player2 = new PlayerState {
                    Position = new Point(m_main_form_width * 3 / 4, 800),
                    Color = Color.Blue
                };
            }
            else {
                Player1 = new PlayerState {
                    Position = new Point(m_main_form_width / 2, 800), 
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
            m_traffic_spawn_rate = Math.Max(
                m_traffic_spawn_rate,
                0.33f - (float)(ElapsedTime.TotalSeconds / 360.0)
            );

            m_move_speed = 5 + (int)(ElapsedTime.TotalSeconds / 30.0);
            m_move_speed = Math.Min(m_move_speed, 10);
        }


        private void UpdatePlayerMovement() {
            if (Player1 != null) {
                if (HashSetPressedKeys.Contains(Keys.W)) {
                    Player1.Position = new Point(
                        Player1.Position.X,
                        Math.Max(Player1.Position.Y - m_move_speed, 100));
                }
                if (HashSetPressedKeys.Contains(Keys.S)) {
                    Player1.Position = new Point(
                        Player1.Position.X,
                        Math.Min(Player1.Position.Y + m_move_speed, m_main_form_height - m_player_height));
                }
                if (HashSetPressedKeys.Contains(Keys.A)) {
                    Player1.Position = new Point(
                        Math.Max(Player1.Position.X - m_move_speed, m_player_width),
                        Player1.Position.Y);
                }
                if (HashSetPressedKeys.Contains(Keys.D)) {
                    Player1.Position = new Point(
                        Math.Min(Player1.Position.X + m_move_speed, m_main_form_width - m_player_width),
                        Player1.Position.Y);
                }
            }

            if (IsMultiplayer && Player2 != null) {
                if (HashSetPressedKeys.Contains(Keys.Up)) {
                    Player2.Position = new Point(
                        Player2.Position.X,
                        Math.Max(Player2.Position.Y - m_move_speed, 100));
                }
                if (HashSetPressedKeys.Contains(Keys.Down)) {
                    Player2.Position = new Point(
                        Player2.Position.X,
                        Math.Min(Player2.Position.Y + m_move_speed, m_main_form_height - m_player_height));
                }
                if (HashSetPressedKeys.Contains(Keys.Left)) {
                    Player2.Position = new Point(
                        Math.Max(Player2.Position.X - m_move_speed, m_player_width),
                        Player2.Position.Y);
                }
                if (HashSetPressedKeys.Contains(Keys.Right)) {
                    Player2.Position = new Point(
                        Math.Min(Player2.Position.X + m_move_speed, m_main_form_width - m_player_width),
                        Player2.Position.Y);
                }
            }
        }

        private void CheckOffRoad() {
            if (Player1 != null) {
                IsFirstPlayerOnGrass = false;

                if (IsMultiplayer) {
                    IsFirstPlayerOnGrass = Player1.Position.X > m_main_form_width / 2 - 10;
                }
                else {
                    IsFirstPlayerOnGrass = Player1.Position.X < m_road_left ||
                               Player1.Position.X > m_road_left + m_road_width;
                }

                if (IsFirstPlayerOnGrass) {
                    Player1Score = Math.Max(0, Player1Score - (int)(20 * GameSpeed));
                }
            }

            if (IsMultiplayer && Player2 != null) {
                IsSecondPlayerOnGrass = Player2.Position.X < m_main_form_width / 2 + 10;

                if (IsSecondPlayerOnGrass) {
                    Player2Score = Math.Max(0, Player2Score - (int)(20 * GameSpeed));
                }
            }
        }

        private void UpdateRoadAndTraffic() {
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(3 / (60.0 * m_traffic_spawn_rate)));
            RoadState.ScrollOffset = (RoadState.ScrollOffset + (int)(5 * GameSpeed)) % 100;

            foreach (var trafficCar in TrafficCars) {
                trafficCar.Bounds = new Rectangle(
                    trafficCar.Bounds.X,
                    trafficCar.Bounds.Y + (int)(trafficCar.Speed * GameSpeed),
                    trafficCar.Bounds.Width,
                    trafficCar.Bounds.Height
                );
            }

            if (m_random.NextDouble() < (1.0 / (60.0 * m_traffic_spawn_rate))) {
                if (IsMultiplayer) {
                    TrySpawnTrafficCar(50, m_main_form_width / 2 - 70); 
                    TrySpawnTrafficCar(m_main_form_width / 2 + 70, m_main_form_width); 
                }
                else {
                    TrySpawnTrafficCar(m_road_left + 50, m_road_left + m_road_width + 50);
                }
            }

            TrafficCars.RemoveAll(car => car.Bounds.Bottom > m_main_form_height + m_player_height + 50);
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

                if (canSpawn) {
                    TrafficCars.Add(new TrafficCarState {
                        Bounds = newCarRect,
                        Speed = speed
                    });
                    spawned = true;
                }

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

                if (player1Rect.IntersectsWith(player2Rect)) {
                    Status = GameStatus.GameOver;
                    CrashedPlayerNumber = -1;
                    return;
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