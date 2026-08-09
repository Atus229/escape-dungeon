using EscapeDungeon.Core;
using UnityEngine;

namespace EscapeDungeon.GamePlay {
    public enum ItemType
    {
        Potion,
        SpeedBoost
    }

    public class ItemController : MonoBehaviour
    {
        [SerializeField] private int amountHealth = 20;
        public ItemType itemType;


        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerController>();
            if (GameManager.Instance == null) return;

            if (other.CompareTag("Player"))
            {
                switch (itemType)
                {
                    case ItemType.Potion:
                        GameManager.Instance.HealthPlayer(amountHealth);
                        GameManager.Instance.SetHudItemState(
                        EscapeDungeon.Ui.ItemType.Blood,
                        true
                           );
                        break;
                    case ItemType.SpeedBoost:
                        //h
                        player.ApplySpeedBoost();
                        GameManager.Instance.SetHudItemState(
                        EscapeDungeon.Ui.ItemType.Thunder,
                        true
                    );
                        break;
                }

                //áp dụng vật phẩm -> destroy
                Destroy(gameObject);
            }
        }
    }
}
