using race_game.Screens;

namespace race_game.Core {
    public class GameEngine {
        private GameState m_current_state;
        private readonly MainMenuScreen m_menu;
        private readonly GameScreen m_game;

        public GameEngine(Form mainForm) {
            m_menu = new MainMenuScreen(StartGame);
            m_game = new GameScreen(ReturnToMenu);

            mainForm.Controls.Add(m_menu);
            mainForm.Controls.Add(m_game);
            m_game.Visible = false;
        }

        private void StartGame(bool isMultiplayer) {
            m_menu.Visible = false;
            m_game.Initialize(isMultiplayer);
            m_game.Visible = true;
        }

        private void ReturnToMenu() {
            m_game.Visible = false;
            m_menu.Visible = true;
        }
    }
}