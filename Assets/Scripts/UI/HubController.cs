using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeDungeon.Ui {
    public enum HintState
    {
        NoUse,
        Using,
        Used
    }

    public enum ItemType
    {
        None,
        Thunder,
        Blood
    }
    public class HubController : MonoBehaviour
    {
        //[Header("HP")]
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private Slider hpSlider;

        //[Header("Round")]
        [SerializeField] private TMP_Text roundText;

        //[Header("Hints")]
        [SerializeField] private TMP_Text hintNoUse;
        [SerializeField] private TMP_Text hintUsing;
        [SerializeField] private TMP_Text hintUsed;

        //[Header("Timer")]
        [SerializeField] private TMP_Text timerText;

        //[Header("Item")]
        [Header("Item Icons")]
        [SerializeField] private GameObject thunderItemNoColor;
        [SerializeField] private GameObject thunderItemColor;
        [SerializeField] private GameObject bloodItemNoColor;
        [SerializeField] private GameObject bloodItemColor;

        public void UpdateHP(int currentHP, int maxHP)
        {
            if (hpText != null)
            {
                hpText.text = $"HP: {currentHP}/{maxHP}";
            }

            if(hpSlider != null)
            {
                float ratio = (maxHP > 0) ? (float)maxHP-currentHP : 0f;
                hpSlider.value = ratio;
            }
        }

        public void SetHintState(HintState state)
        {
            bool noUseState = (state == HintState.NoUse);
            bool usingState = (state == HintState.Using);
            bool usedState = (state == HintState.Used);

            if(hintNoUse != null)
            {
                hintNoUse.gameObject.SetActive(noUseState);
            }

            if(hintUsing!= null)
            {
                hintUsing.gameObject.SetActive(usingState);
            }

            if (hintUsed != null)
            {
                hintUsed.gameObject.SetActive(usedState);
            }

        }

        public void UpdateHintHub(int hintRemain)
        {
            string hintText = $"{hintRemain} Hints left";

            if (hintRemain <= 0)
            {
                hintText = "No Hint Left!";
            }

            if (hintNoUse != null)
            {
                hintNoUse.text = hintText;
            }

            if (hintUsing != null)
            {
                hintUsing.text = hintText;
            }

            if (hintUsed != null && hintRemain <= 0)
            {
                hintUsed.text = "No Hint Left!";
            }
        }

        public void UpdateTimer(float remainingTime)
        {
            if(timerText == null)
            {
                return;
            }

            if (remainingTime < 0)
            {
                remainingTime = 0;
            }

            timerText.text = $"{(int)remainingTime}";
        }

        public void UpdateRound(int round)
        {
            if (roundText == null) return;

            roundText.text = $"Round {round}";
        }


        public void SetItemState(ItemType type, bool isActive)
        {
            // Nếu truyền None -> Reset cả 2 về NoColor
            if (type == ItemType.None)
            {
                if (thunderItemNoColor != null) thunderItemNoColor.SetActive(true);
                if (thunderItemColor != null) thunderItemColor.SetActive(false);

                if (bloodItemNoColor != null) bloodItemNoColor.SetActive(true);
                if (bloodItemColor != null) bloodItemColor.SetActive(false);
                return;
            }

            // Cập nhật riêng cho Thunder
            if (type == ItemType.Thunder)
            {
                if (thunderItemNoColor != null) thunderItemNoColor.SetActive(!isActive);
                if (thunderItemColor != null) thunderItemColor.SetActive(isActive);
            }

            // Cập nhật riêng cho Blood
            if (type == ItemType.Blood)
            {
                if (bloodItemNoColor != null) bloodItemNoColor.SetActive(!isActive);
                if (bloodItemColor != null) bloodItemColor.SetActive(isActive);
            }
        }
        }
    }
