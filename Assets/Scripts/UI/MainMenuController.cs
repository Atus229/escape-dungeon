using EscapeDungeon.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeDungeon.Ui
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "GameplayScene";

        [Header("New Game Confirmation")]
        [SerializeField] private GameObject newGameConfirmPanel;

        [Header("Exit Confirmation")]
        [SerializeField] private GameObject exitConfirmPanel;

        [Header("How To Play Panel")]
        [SerializeField] private GameObject howToPlayPanel;

        private void Start()
        { 
            if (newGameConfirmPanel != null)
            {
                newGameConfirmPanel.SetActive(false);
            }

            if (exitConfirmPanel != null)
            {
                exitConfirmPanel.SetActive(false);
            }

            if (howToPlayPanel != null)
            {
                howToPlayPanel.SetActive(false);
            }
        }
        public void OnClickNewGame()
        {
            Debug.Log("[MainMenu] NewGame clicked.");

            if (newGameConfirmPanel == null)
            {
                Debug.LogError(
                    "[MainMenu] confirmNewGamePanel CHƯA ĐƯỢC GÁN trong Inspector."
                );

                return;
            }

            Debug.Log(
                $"[MainMenu] Panel trước khi bật: " +
                $"activeSelf = {newGameConfirmPanel.activeSelf}, " +
                $"activeInHierarchy = {newGameConfirmPanel.activeInHierarchy}"
            );

            newGameConfirmPanel.SetActive(true);

            Debug.Log(
                $"[MainMenu] Panel sau khi bật: " +
                $"activeSelf = {newGameConfirmPanel.activeSelf}, " +
                $"activeInHierarchy = {newGameConfirmPanel.activeInHierarchy}"
            );
        }

        public void OnClickLoadGame()
        {
            SessionConfig.StartNewGame = false;
            SceneManager.LoadScene(gameplaySceneName);
        }
        
        //TODO: OnClickSetting 


        public void OnConfirmNewGame()
        {
            Debug.Log("[MainMenu] Confirm New Game.");

            SessionConfig.StartNewGame = true;

            SessionConfig.OverrideRound = 0;

            LocalSaveSystem.ResetProgressToNewGame();

            HideConfirmNewGamePanel();

            SceneManager.LoadScene(gameplaySceneName);
        }
        public void OnCancelNewGame()
        {
            Debug.Log("[MainMenu] Cancel New Game.");
            HideConfirmNewGamePanel();
        }
        private void HideConfirmNewGamePanel()
        {
            if (newGameConfirmPanel != null)
            {
                newGameConfirmPanel.SetActive(false);
            }
        }

        public void OnClickExit()
        {
            Debug.Log("[MainMenu] Exit clicked.");

            if (exitConfirmPanel == null)
            {
                Debug.LogError(
                    "[MainMenu] exitConfirmPanel chưa được gán."
                );

                return;
            }

            // Đảm bảo panel New Game không mở cùng lúc
            if (newGameConfirmPanel != null)
            {
                newGameConfirmPanel.SetActive(false);
            }


            // Chỉ hiện thông báo, chưa thoát game
            exitConfirmPanel.SetActive(true);

            // Đưa ExitConfirmPanel lên trên cùng
            exitConfirmPanel.transform.SetAsLastSibling();
        }

        public void OnConfirmExit()
        {
            Debug.Log("[MainMenu] Confirm Exit clicked.");

            // Có thể lưu dữ liệu tại đây nếu cần.
            // Hiện tại Continue đã dùng save trước đó.
            Application.Quit();

            // Khi chạy trong Unity Editor, Application.Quit()
            // không đóng Play Mode.
            // Chỉ dùng đoạn dưới nếu muốn nút Yes dừng Play Mode trong Editor.
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        public void OnCancelExit()
        {
            Debug.Log("[MainMenu] Cancel Exit clicked.");

            if (exitConfirmPanel != null)
            {
                exitConfirmPanel.SetActive(false);
            }
        }

        public void OnClickHowToPlay()
        {
            Debug.Log("[MainMenu] HowToPlay clicked.");

            if (howToPlayPanel == null)
            {
                Debug.LogError("[MainMenu] howToPlayPanel chưa được gán trong Inspector.");
                return;
            }

            // Đảm bảo các panel khác tắt để UI gọn
            if (newGameConfirmPanel != null)
            {
                newGameConfirmPanel.SetActive(false);
            }

            if (exitConfirmPanel != null)
            {
                exitConfirmPanel.SetActive(false);
            }

            howToPlayPanel.SetActive(true);
            howToPlayPanel.transform.SetAsLastSibling();  // Đưa lên trên cùng
        }

        public void OnCloseHowToPlay()
        {
            Debug.Log("[MainMenu] Close HowToPlay.");

            if (howToPlayPanel != null)
            {
                howToPlayPanel.SetActive(false);
            }
        }
    }
}
