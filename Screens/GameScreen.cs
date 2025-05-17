using race_game.Core;

namespace race_game.Screens
{
    public class GameScreen : Panel {
        private readonly GameState m_state;
        private readonly Renderer m_renderer;
        private readonly System.Windows.Forms.Timer m_game_timer;
        Action m_game_over_call_back;


        public GameState GetGameState() {
            return m_state;
        }

        public GameScreen(Action gameOverCallBack, int width, int height) {
            m_game_over_call_back = gameOverCallBack;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;

            m_state = new GameState(width, height);
            m_renderer = new Renderer(width, height);

            m_game_timer = new System.Windows.Forms.Timer { Interval = 1 }; 
            m_game_timer.Tick += GameTimer_Tick;
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

        private void GameTimer_Tick(object? sender, EventArgs? e) {
            if (m_state.Status == GameState.GameStatus.Racing) {
                m_state.Update();

                if (m_state.Status == GameState.GameStatus.GameOver) {
                    m_game_over_call_back();
                }
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            m_renderer.Render(e.Graphics, m_state);
            base.OnPaint(e);
        }
    }
}
