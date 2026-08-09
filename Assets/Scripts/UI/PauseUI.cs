using EscapeDungeon.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace EscapeDungeon.Ui
{
    public class PauseUI : BaseOverlayUI
    {
        [Header("Quit Confirmation")]
        [SerializeField] private GameObject quitConfirmPanel;
        public void Show(float Duration)
        {
            base.Show();
        }

        public void OnClickResume()
        {
            if (!isVisible)
            {
                return;
            }

            

            GameManager.Instance.ResumeGameFromPause();
        }

        public void OnClickSetting()
        {

        }

        public void OnClickQuit()
        {
            if(!isVisible)
            {
                return;
            }

            if (quitConfirmPanel == null)
            {
                Debug.LogError("[PauseUI] quitConfirmPanel chưa được gán trong Inspector.");
                return;
            }

            // Hiện panel xác nhận, không thoát ngay
            quitConfirmPanel.SetActive(true);
            quitConfirmPanel.transform.SetAsLastSibling();
        }

        public void OnConfirmQuit()
        {
            Debug.Log("[PauseUI] Confirm Quit to Main Menu.");

            // Ẩn popup (tuỳ em, có thể không cần nếu đổi scene liền)
            if (quitConfirmPanel != null)
            {
                quitConfirmPanel.SetActive(false);
            }

            // Ẩn pause UI để tránh còn overlay
            Hide();

            GameManager.Instance.QuitToMainMenu();
        }

        // Gắn vào nút No trên popup
        public void OnCancelQuit()
        {
            Debug.Log("[PauseUI] Cancel Quit.");

            if (quitConfirmPanel != null)
            {
                quitConfirmPanel.SetActive(false);
            }
        }


    }

    
}
