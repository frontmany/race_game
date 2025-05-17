using race_game.Screens;

namespace race_game.Core {
    public enum CurrentScreen {
        MainMenu,
        PauseMenu,
        GameOver,
        Game
    }

    public class GameEngine {
        private readonly Form m_main_form;
        private readonly MainMenuScreen m_menu;
        private readonly GameScreen m_game;
        private readonly PauseMenuScreen m_pause_menu;
        private GameOverScreen m_game_over_screen;
        public CurrentScreen CurrentScreen { get; private set; }

        public GameEngine(Form mainForm) {
            CurrentScreen = CurrentScreen.MainMenu;
            m_main_form = mainForm;
            m_main_form.KeyPreview = true;
            m_main_form.KeyDown += HandleKeyDown;
            m_main_form.KeyUp += HandleKeyUp;

            m_menu = new MainMenuScreen(StartGame, mainForm);
            m_game = new GameScreen(ShowGameOver, mainForm.ClientSize.Width, mainForm.ClientSize.Height);
            m_pause_menu = new PauseMenuScreen(ResumeGame, ReturnToMenu, mainForm);
            m_game_over_screen = new GameOverScreen(ReturnToMenu, mainForm);

            m_game.Visible = false;
            m_pause_menu.Visible = false;
            m_game_over_screen.Visible = false;

            mainForm.Controls.Add(m_menu);
            mainForm.Controls.Add(m_game);
            mainForm.Controls.Add(m_pause_menu);
            mainForm.Controls.Add(m_game_over_screen);
        }

        public void ShowGameOver() {
            CurrentScreen = CurrentScreen.GameOver;
            m_game.Visible = false;
            m_game_over_screen.IsMultiplayer = m_game.GetGameState().IsMultiplayer;
            m_game_over_screen.FirstPlayerScore = m_game.GetGameState().Player1Score;
            m_game_over_screen.SecondPlayerScore = m_game.GetGameState().Player2Score;
            m_game_over_screen.CrashedPlayerNumber = m_game.GetGameState().CrashedPlayerNumber;
            m_game_over_screen.InitializeUI();
            m_game_over_screen.Visible = true;
        }


        private void HandleKeyDown(object? sender, KeyEventArgs? e) {
            switch (CurrentScreen) {
                case CurrentScreen.MainMenu:
                    m_menu.HandleKeyDown(sender, e);
                    break;
                case CurrentScreen.Game:
                    if (e.KeyCode == Keys.Escape) {
                        PauseGame();
                        e.Handled = true;
                    }
                    else {
                        m_game.HandleKeyDown(sender, e);
                    }
                    break;
                case CurrentScreen.PauseMenu:
                    m_pause_menu.HandleKeyDown(sender, e);
                    break;
            }
        }

        private void HandleKeyUp(object? sender, KeyEventArgs? e) {
            switch (CurrentScreen) {
                case CurrentScreen.MainMenu:
                    m_menu.HandleKeyUp(sender, e);
                    break;
                case CurrentScreen.Game:
                    m_game.HandleKeyUp(sender, e);
                    break;
                case CurrentScreen.PauseMenu:
                    m_pause_menu.HandleKeyUp(sender, e);
                    break;
            }
        }

        private void StartGame(bool isMultiplayer) {
            CurrentScreen = CurrentScreen.Game;
            SwitchScreens(m_menu, m_game);
            m_game.Start(isMultiplayer);
        }

        private void ReturnToMenu() {
            CurrentScreen = CurrentScreen.MainMenu;
            SwitchScreens(m_game, m_menu);
        }

        private void PauseGame() {
            if (CurrentScreen == CurrentScreen.Game) {
                CurrentScreen = CurrentScreen.PauseMenu;
                m_game.Pause(true);
                SwitchScreens(m_game, m_pause_menu);
            }
        }

        private void ResumeGame() {
            if (CurrentScreen == CurrentScreen.PauseMenu) {
                m_game.Visible = true;
                m_game.Pause(false);
                CurrentScreen = CurrentScreen.Game;
                SwitchScreens(m_pause_menu, m_game);
            }
        }

        private void SwitchScreens(Control hideControl, Control showControl) {
            m_main_form.SuspendLayout();
            showControl.Visible = true;
            hideControl.Visible = false; 
            m_main_form.ResumeLayout();    
        }
    }
}