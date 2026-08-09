using EscapeDungeon.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace EscapeDungeon.Ui
{
    public class VictoryUI : BaseOverlayUI
    {
        [Header("Victory References")]
        [SerializeField] private TMP_Text timeText;

        [Header("Victory All round")]
        [SerializeField] private GameObject victoryAllRoundPanel;

        private bool isFinalVictory = false;
        public void Show(float Duration, bool isFinalVictory)
        {
            if (timeText != null)
            {
                timeText.text = FormatTime(Duration);
            }

            this.isFinalVictory = isFinalVictory;
            base.Show();
        }

        public override void Hide()
        {
            // TẮT NGAY, không dùng coroutine nữa
            StopAllCoroutines();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            isVisible = false;
            gameObject.SetActive(false);
        }

        public void OnClickContinue()
        {
            Debug.Log("[VictoryUI] OnClickContinue clicked.");

            if (!isVisible)
            {
                return;
            }

            Debug.Log($"[VictoryUI] isFinalVictory = {isFinalVictory}");

            if (isFinalVictory)
            {
                Debug.Log("[VictoryUI] Final victory -> hide Victory panel + show Victory All Round panel");

                // Tắt overlay Victory (fade out, setActive false sau 0.5s)
                Hide();

                if (victoryAllRoundPanel != null)
                {
                    victoryAllRoundPanel.SetActive(true);
                    victoryAllRoundPanel.transform.SetAsLastSibling();
                }
                else
                {
                    Debug.LogWarning("[VictoryUI] victoryAllRoundPanel is NULL.");
                }

                return;
            }

            Debug.Log("[VictoryUI] Normal victory -> Hide() + OnVictoryContinueClicked()");
            Hide();
            GameManager.Instance.OnVictoryContinueClicked();
        }

        // THÊM: nút “Restart From Round 1” dùng khi final victory
        public void OnClickRestartFromBeginning()
        {
            Debug.Log("[VictoryUI] OnClickRestartFromBeginning clicked.");

            if (victoryAllRoundPanel != null)
            {
                victoryAllRoundPanel.SetActive(false);
            }

            Hide(); // Tắt luôn overlay Victory (nếu còn)

            GameManager.Instance.RestartFromRoundOne();
        }

    }
}
