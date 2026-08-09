using EscapeDungeon.Core;
using System;
using TMPro;
using UnityEngine;

namespace EscapeDungeon.Ui
{
    public class LoseUI : BaseOverlayUI
    {
        [Header("Lose References")]
        [SerializeField] private TMP_Text timeText;
        
        public void Show(float Duration)
        {
            if (timeText != null)
            {
                timeText.text = FormatTime(Duration);
            }

            base.Show();
        }

        public void OnClickRestart()
        {
            if (!isVisible)
            {
                return;
            }

            Hide();

            GameManager.Instance.OnLoseContinueClicked();
        }
    }

    
}
