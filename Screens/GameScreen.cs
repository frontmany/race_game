using race_game.Core;

namespace race_game.Screens
{
    public class GameScreen : Panel {
        private readonly System.Windows.Forms.Timer m_game_timer;
        private readonly GameState                  m_state;
        private readonly Renderer                   m_renderer;
        Action                                      m_game_over_call_back;
        decimal                                     m_scaling_factor;

        public GameScreen(Action gameOverCallBack, int mainFormWidth, int mainFormHeight, decimal scalingFactor) {
            m_scaling_factor = scalingFactor;
            m_game_over_call_back = gameOverCallBack;
            m_scaling_factor = scalingFactor;
            this.Dock = DockStyle.Fill;

            m_state = new GameState(mainFormWidth, mainFormHeight, m_scaling_factor);
            m_renderer = new Renderer(mainFormWidth, mainFormHeight);

            m_game_timer = new System.Windows.Forms.Timer { Interval = 1 }; 
            m_game_timer.Tick += GameTimer_Tick;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        public GameState GetGameState() {
            return m_state;
        }

        public void Start(bool isMultiplayer) {
            
            m_state.Init(isMultiplayer);
            m_game_timer.Start();
        }

        public void Pause(bool isPaused) {
            if (isPaused) {
                m_state.Status = GameState.GameStatus.Paused;
            }
            else {
                m_state.Status = GameState.GameStatus.Racing;
            }
        }

        public void HandleKeyDown(object? sender, KeyEventArgs? e) {
            if (m_state.Status == GameState.GameStatus.Racing) {
                m_state.HashSetPressedKeys.Add(e.KeyCode);
            }
        }

        public void HandleKeyUp(object? sender, KeyEventArgs? e) {
            if (m_state.Status == GameState.GameStatus.Racing) {
                m_state.HashSetPressedKeys.Remove(e.KeyCode);
            }
        }



        protected override void OnPaint(PaintEventArgs e) {
            m_renderer.Render(e.Graphics, m_state);
            base.OnPaint(e);
        }



        private void GameTimer_Tick(object? sender, EventArgs? e) {
            if (m_state.Status == GameState.GameStatus.Racing) {
                m_state.Update();

                if (m_state.Status == GameState.GameStatus.GameOver) {
                    m_game_over_call_back();
                }
                this.Invalidate();
            }
        }
    }
}
