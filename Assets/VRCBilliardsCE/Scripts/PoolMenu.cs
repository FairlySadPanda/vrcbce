using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace VRCBilliards
{
    public class PoolMenu : UdonSharpBehaviour
    {
        [Header("Pool State Manager")]
        public PoolStateManager manager;

        [Header("Style")]
        public Color selectedColor = Color.white;
        public Color unselectedColor = Color.gray;

        [Header("Menu / Buttons")]
        public bool useUnityUI;
        public Button player1UIButton;
        public Button player2UIButton;
        public Button player3UIButton;
        public Button player4UIButton;

        public GameObject resetGameButton;
        public GameObject lockMenu;
        public GameObject mainMenu;

        public GameObject startGameButton;

        [Header("Game Mode")]
        public TextMeshProUGUI gameModeTxt;
        public Image[] gameModeButtons = { };

        [Header("Guide Line")]
        public bool toggleGuideLineButtonsActive = true;
        public GameObject guideLineEnableButton;
        public GameObject guideLineDisableButton;
        public TextMeshProUGUI guidelineStatus;
        public Image[] guideLineButtons = { };

        [Header("Timer")]
        public TextMeshProUGUI timer;
        public string noTimerText = "No Limit";
        public string timerValueText = "{}s Limit";
        public Image timerButton, noTimerButton;

        [Header("Teams")]
        public TextMeshProUGUI teamsTxt;
        public Image[] teamsButtons = { };

        [Header("Players")]
        public GameObject player1Button;
        public GameObject player2Button;
        public GameObject player3Button;
        public GameObject player4Button;
        public GameObject leaveButton;
        public string defaultEmptyPlayerSlotText = "<color=grey>Player {}</color>";
        public TextMeshProUGUI player1MenuText;
        public TextMeshProUGUI player2MenuText;
        public TextMeshProUGUI player3MenuText;
        public TextMeshProUGUI player4MenuText;
        public bool instanceOwnerCanReset = true;
        public bool masterCanReset = false;

        [Header("Score")]
        public TextMeshProUGUI player1ScoreText;
        public TextMeshProUGUI player2ScoreText;
        public TextMeshProUGUI player3ScoreText;
        public TextMeshProUGUI player4ScoreText;

        public TextMeshProUGUI team1ScoreText;
        public TextMeshProUGUI team2ScoreText;

        public TextMeshProUGUI winnerText;

        private bool isTeams;
        private bool isSignedUpToPlay;
        private bool canStartGame;
        private bool canBypass;

        // TODO: This all needs to be secured.
        public void UnlockTable()
        {
            manager.UnlockTable();
        }

        public void LockTable()
        {
            manager.LockTable();
        }

        public void SelectTeams()
        {
            manager.SelectTeams();
        }

        public void DeselectTeams()
        {
            manager.DeselectTeams();
        }

        public void Select4BallJapanese()
        {
            manager.Select4BallJapanese();
        }

        public void Select4BallKorean()
        {
            manager.Select4BallKorean();
        }

        public void Select8Ball()
        {
            manager.Select8Ball();
        }

        public void Select9Ball()
        {
            manager.Select9Ball();
        }

        public void IncreaseTimer()
        {
            manager.IncreaseTimer();
        }

        public void DecreaseTimer()
        {
            manager.DecreaseTimer();
        }

        public void EnableGuideline()
        {
            manager.EnableGuideline();
        }

        public void DisableGuideline()
        {
            manager.DisableGuideline();
        }

        public void SignUpAsPlayer1()
        {
            if (!isSignedUpToPlay)
            {
                manager.JoinGame(0);
            }
        }

        public void SignUpAsPlayer2()
        {
            if (!isSignedUpToPlay)
            {
                manager.JoinGame(1);
            }
        }

        public void SignUpAsPlayer3()
        {
            if (!isSignedUpToPlay)
            {
                manager.JoinGame(2);
            }
        }

        public void SignUpAsPlayer4()
        {
            if (!isSignedUpToPlay)
            {
                manager.JoinGame(3);
            }
        }

        public void LeaveGame()
        {
            manager.LeaveGame();
        }

        public void StartGame()
        {
            if (canStartGame)
            {
                manager.StartNewGame();
            }
        }

        public void EndGame()
        {
            if (isSignedUpToPlay || canBypass)
            {
                manager.ForceReset();
                return;
            }
        }

        public void EnableResetButton()
        {
            resetGameButton.SetActive(true);
            lockMenu.SetActive(false);
            mainMenu.SetActive(false);

            winnerText.text = "";
        }

        public void EnableUnlockTableButton()
        {
            resetGameButton.SetActive(false);
            lockMenu.SetActive(true);
            mainMenu.SetActive(false);

            ResetScoreScreen();
        }

        public void EnableMainMenu()
        {
            resetGameButton.SetActive(false);
            lockMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        private void UpdateButtonColors(Image[] buttons, int selectedIndex)
        {
            if (buttons == null) return;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == null) continue;

                buttons[i].color = i == selectedIndex ? selectedColor : unselectedColor;
            }
        }

        /// <summary>
        /// Recieve a new set of data from the manager that can be displayed to viewers.
        /// </summary>
        public void UpdateMainMenuView(
            bool newIsTeams,
            bool isTeam2Playing,
            int gameMode,
            bool isKorean4Ball,
            int timerMode,
            int player1ID,
            int player2ID,
            int player3ID,
            int player4ID,
            bool guideline
        )
        {
            Debug.Log($"Got a new menu update: teams? {newIsTeams} team 2's turn? {isTeam2Playing} game mode {gameMode} timer mode {timerMode} player 1 {player1ID} player 2 {player2ID} player 3 {player3ID} player 4 {player4ID}");

            if (newIsTeams)
            {
                if (Utilities.IsValid(teamsTxt)) teamsTxt.text = "Teams: YES";
                isTeams = true;
            }
            else
            {
                if (Utilities.IsValid(teamsTxt)) teamsTxt.text = "Teams: NO";
                isTeams = false;
            }
            UpdateButtonColors(teamsButtons, newIsTeams ? 0 : 1);

            switch (gameMode)
            {
                case 0:
                    if (Utilities.IsValid(gameModeTxt)) gameModeTxt.text = "American 8-Ball";
                    UpdateButtonColors(gameModeButtons, 0);

                    break;
                case 1:
                    if (Utilities.IsValid(gameModeTxt)) gameModeTxt.text = "American 9-Ball";
                    UpdateButtonColors(gameModeButtons, 1);

                    break;
                case 2:
                    if (isKorean4Ball)
                    {
                        if (Utilities.IsValid(gameModeTxt)) gameModeTxt.text = "Korean 4-Ball";
                        UpdateButtonColors(gameModeButtons, 3);
                    }
                    else
                    {
                        if (Utilities.IsValid(gameModeTxt)) gameModeTxt.text = "Japanese 4-Ball";
                        UpdateButtonColors(gameModeButtons, 2);
                    }

                    break;
            }

            switch (timerMode)
            {
                case 0:
                    if (Utilities.IsValid(timer)) timer.text = noTimerText;
                    break;
                case 1:
                    if (Utilities.IsValid(timer)) timer.text = timerValueText.Replace("{}", "10");
                    break;
                case 2:
                    if (Utilities.IsValid(timer)) timer.text = timerValueText.Replace("{}", "15");
                    break;
                case 3:
                    if (Utilities.IsValid(timer)) timer.text = timerValueText.Replace("{}", "30");
                    break;
                case 4:
                    if (Utilities.IsValid(timer)) timer.text = timerValueText.Replace("{}", "60");
                    break;
            }
            if (Utilities.IsValid(timerButton)) timerButton.color = timerMode != 0 ? selectedColor : unselectedColor;
            if (Utilities.IsValid(noTimerButton)) noTimerButton.color = timerMode == 0 ? selectedColor : unselectedColor;

            leaveButton.SetActive(false);

            if (useUnityUI)
            {
                player1UIButton.interactable = false;
                player2UIButton.interactable = false;
                player3UIButton.interactable = false;
                player4UIButton.interactable = false;
            }
            else
            {
                player1Button.SetActive(false);
                player2Button.SetActive(false);
                player3Button.SetActive(false);
                player4Button.SetActive(false);
            }

            bool found = false;

            if (player1ID > 0)
            {
                found = HandlePlayerState(player1MenuText, player1ScoreText, VRCPlayerApi.GetPlayerById(player1ID));
            }
            else
            {
                player1MenuText.text = defaultEmptyPlayerSlotText.Replace("{}", "1");
                player1ScoreText.text = "";
            }

            if (player2ID > 0)
            {
                found = HandlePlayerState(player2MenuText, player2ScoreText, VRCPlayerApi.GetPlayerById(player2ID));
            }
            else
            {
                player2MenuText.text = defaultEmptyPlayerSlotText.Replace("{}", "2");
                player2ScoreText.text = "";
            }

            if (player3ID > 0)
            {
                found = HandlePlayerState(player3MenuText, player3ScoreText, VRCPlayerApi.GetPlayerById(player3ID));
            }
            else
            {
                player3MenuText.text = newIsTeams ? defaultEmptyPlayerSlotText.Replace("{}", "3") : "";
                player3ScoreText.text = "";
            }

            if (player4ID > 0)
            {
                found = HandlePlayerState(player4MenuText, player4ScoreText, VRCPlayerApi.GetPlayerById(player4ID));
            }
            else
            {
                player4MenuText.text = newIsTeams ? defaultEmptyPlayerSlotText.Replace("{}", "4") : "";
                player4ScoreText.text = "";
            }

            VRCPlayerApi networkPlayer = Networking.LocalPlayer;
            int id = networkPlayer.playerId;
            if (id == player1ID || id == player2ID || id == player3ID || id == player4ID)
            {
                isSignedUpToPlay = true;
                canBypass = false;

                if (id == player1ID)
                {
                    canStartGame = true;
                    startGameButton.SetActive(true);
                }
                else
                {
                    canStartGame = false;
                    startGameButton.SetActive(false);
                }
            }
            else
            {
                if (networkPlayer.isInstanceOwner && instanceOwnerCanReset || networkPlayer.isMaster && masterCanReset)
                {
                    canBypass = true;
                    isSignedUpToPlay = false;
                    canStartGame = false;
                    startGameButton.SetActive(false);
                    return;
                }

                canBypass = false;
                isSignedUpToPlay = false;
                canStartGame = false;
                startGameButton.SetActive(false);
            }

            if (!found)
            {
                if (useUnityUI)
                {
                    player1UIButton.interactable = true;
                    player2UIButton.interactable = true;

                }
                else
                {
                    player1Button.SetActive(true);
                    player2Button.SetActive(true);
                }

                if (newIsTeams)
                {
                    if (useUnityUI)
                    {
                        player3UIButton.interactable = true;
                        player4UIButton.interactable = true;
                    }
                    else
                    {
                        player3Button.SetActive(true);
                        player4Button.SetActive(true);
                    }
                }
            }

            if (guideline)
            {
                if (toggleGuideLineButtonsActive && !useUnityUI)
                {
                    guideLineDisableButton.SetActive(true);
                    guideLineEnableButton.SetActive(false);
                }

                UpdateButtonColors(guideLineButtons, 0);
                if (Utilities.IsValid(guidelineStatus)) guidelineStatus.text = "Guideline On";
            }
            else
            {
                if (toggleGuideLineButtonsActive && !useUnityUI)
                {
                    guideLineDisableButton.SetActive(false);
                    guideLineEnableButton.SetActive(true);
                }

                UpdateButtonColors(guideLineButtons, 1);
                if (Utilities.IsValid(guidelineStatus)) guidelineStatus.text = "Guideline Off";
            }
        }

        private bool HandlePlayerState(TextMeshProUGUI menuText, TextMeshProUGUI scoreText, VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player))
            {
                return false;
            }

            menuText.text = player.displayName;
            scoreText.text = player.displayName;

            if (player.playerId == Networking.LocalPlayer.playerId)
            {
                leaveButton.SetActive(true);

                if (useUnityUI)
                {
                    player1UIButton.interactable = false;
                    player2UIButton.interactable = false;
                    player3UIButton.interactable = false;
                    player4UIButton.interactable = false;
                }
                else
                {
                    player1Button.SetActive(false);
                    player2Button.SetActive(false);
                    player3Button.SetActive(false);
                    player4Button.SetActive(false);

                }

                return true;
            }

            return false;
        }

        public void SetScore(bool isTeam2, int score)
        {
            if (score < 0)
            {
                team1ScoreText.text = "";
                team2ScoreText.text = "";

                return;
            }

            if (isTeam2)
            {
                team2ScoreText.text = $"{score}";
            }
            else
            {
                team1ScoreText.text = $"{score}";
            }
        }

        public void GameWasReset()
        {
            winnerText.text = "The game was ended!";
        }

        public void TeamWins(bool isTeam2)
        {
            if (isTeams)
            {
                if (isTeam2)
                {
                    winnerText.text = $"{player2ScoreText.text} and {player4ScoreText.text} win!";
                }
                else
                {
                    winnerText.text = $"{player1ScoreText.text} and {player3ScoreText.text} win!";
                }
            }
            else
            {
                if (isTeam2)
                {
                    winnerText.text = $"{player2ScoreText.text} wins!";
                }
                else
                {
                    winnerText.text = $"{player1ScoreText.text} wins!";
                }
            }
        }

        private void ResetScoreScreen()
        {
            player1ScoreText.text = "";
            player2ScoreText.text = "";
            player3ScoreText.text = "";
            player4ScoreText.text = "";

            team1ScoreText.text = "";
            team2ScoreText.text = "";

            winnerText.text = "";
        }
    }
}
