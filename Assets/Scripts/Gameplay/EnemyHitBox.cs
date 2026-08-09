using UnityEngine;

namespace EscapeDungeon.GamePlay
{
    public class EnemyHitBox : MonoBehaviour
    {
        [SerializeField] private int damageAmount = 50;
        [SerializeField] private Animator animator;

        private void OnTriggerEnter(Collider other)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController == null)
            {
                return;
            }

            if(animator == null)
            {
                return;
            }

            bool isPunching = animator.GetBool("isPunching");
            if (!isPunching)
            {
                return;
            }

            Debug.Log("Take Damageee!");
            playerController.TakeDamage(damageAmount);
        }
    }
}