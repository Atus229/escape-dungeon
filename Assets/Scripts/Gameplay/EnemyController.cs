using System;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;


namespace EscapeDungeon.GamePlay {
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float waitDuration = 2f;
        [SerializeField] private float rotationSpeed = 2f;

        [Header("Punching")]
        [SerializeField] private float punchRange = 1.5f;
        public Transform playerTransform;
        [SerializeField] private Animator punchAnimator;
        [SerializeField] private float punchYawOffset = -15f;

        [Header("Chasing")]
        [SerializeField] private float chaseRange = 6f;
        [SerializeField] private float chaseSpeed = 4f;
        [SerializeField] private float chaseRotationSpeed = 2f;
        private float chaseDuration = 10f;
        private float chaseTimer = 0f;

        public Transform[] waypoints;

        private bool isPunching = false;
        private bool isPatrolling = true;
        private bool isChasing = false;

        private int currentIndex = 0; //đang ở waypoints
        private Animator animator;
        private float waitTimer = 0f;
        private float punchDuration = 0.5f;
        private float punchTimer = 0f;


        private void Start()
        {
            animator = GetComponent<Animator>();
        }
        private void Update()
        {

            if (isPunching)
            {
                punchTimer -= Time.deltaTime;
                if (punchTimer <= 0f)
                {
                    isPunching = false;
                    if (animator != null)
                    {
                        animator.SetBool("isPunching", false);
                        animator.SetBool("isWalking", true);
                    }

                    //sau khi punch xong bắt đầu chase 1 đoạn nhất định
                    StartChase();
                }
                return;
            }

            if (isChasing)
            {
                UpdateChase();
                return;
            }

            MoveToCurrentWaypoint();


            if (playerTransform != null)
            {
                Vector3 enemyPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 playerPosXZ = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);

                var playerDistance = Vector3.Distance(enemyPosXZ, playerPosXZ);


                if (playerDistance <= punchRange && IsPlayerInFront(enemyPosXZ, playerPosXZ, 60))
                {
                    isChasing = false;
                    StartPunch();
                    return;
                }
                else if(playerDistance <= chaseRange && CanChaseOnSameRowOrColumn(enemyPosXZ, playerPosXZ) && IsPlayerInFront(enemyPosXZ, playerPosXZ, 90))
                {
                    StartChase();
                }
            }

            

            
        }

        private bool CanChaseOnSameRowOrColumn(Vector3 enemyPosXZ, Vector3 playerPosXZ)
        {
            var dx = Math.Abs(enemyPosXZ.x - playerPosXZ.x);
            var dy = Math.Abs(enemyPosXZ.z - playerPosXZ.z);

            var sameLineThreshold = 0.5f;

            bool sameColumn = dx <= sameLineThreshold;
            bool sameRow = dy <= sameLineThreshold;

            return sameColumn || sameRow;
        }

        private bool IsPlayerInFront(Vector3 enemyPosXZ, Vector3 playerPosXZ, float angleLimit)
        {
            var toPlayer = (playerPosXZ - enemyPosXZ).normalized;
            Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

            float angle = Vector3.Angle(forwardXZ, toPlayer);
            return angle <= angleLimit;
        }

        private void UpdateChase()
        {
            chaseTimer -= Time.deltaTime;

            if(chaseTimer <= 0f)
            {
                isChasing = false;
                if (animator != null)
                {
                    animator.SetBool("isChasing", false);
                    animator.SetBool("isWalking", true); // quay về patrol
                }
                return;
            }

            if (playerTransform == null) return;


            Vector3 enemyPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 playerPosXZ = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);

            if (!IsPlayerWithinPatrolSegment(enemyPosXZ, playerPosXZ))
            {
                isChasing = false;
                if (animator != null)
                {
                    animator.SetBool("isChasing", false);
                    animator.SetBool("isWalking", true);
                }
                return;
            }

            //kiểm tra nếu đã vào tầm punch thì chuyển sang punch
            float distanceToPlayer = Vector3.Distance(enemyPosXZ, playerPosXZ);
            if (distanceToPlayer <= punchRange && IsPlayerInFront(enemyPosXZ, playerPosXZ, 45))
            {
                isChasing = false;
                animator.SetBool("isChasing", false);
                StartPunch();
            }

            if (!CanChaseOnSameRowOrColumn(enemyPosXZ, playerPosXZ))
            {
                isChasing = false;
                return;
            }

            Vector3 direction = (playerPosXZ - enemyPosXZ).normalized;


            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, chaseRotationSpeed * Time.deltaTime);
            }

            transform.position += direction * chaseSpeed * Time.deltaTime;

        }


        private void StartChase()
        {
            if (playerTransform == null) return;

            Vector3 enemyPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 playerPosXZ = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);

            if (!IsPlayerWithinPatrolSegment(enemyPosXZ, playerPosXZ))
            {
                return;
            }

            isChasing = true;
            chaseTimer = chaseDuration;
            
            if (playerTransform != null)
            { 
                Vector3 lookDir = (playerPosXZ - enemyPosXZ).normalized;

                if (lookDir.sqrMagnitude > 0.001f)
                {
                    Quaternion baseRotation = Quaternion.LookRotation(lookDir);

                    // Thêm offset góc Y (yaw) để enemy hơi quay sang trái/phải
                    Quaternion offsetRotation = Quaternion.Euler(0f, punchYawOffset, 0f);

                    transform.rotation = baseRotation * offsetRotation;
                }
            }

            if (animator != null)
            {
                animator.SetBool("isWalking", false);   // không patrol nữa
                animator.SetBool("isPunching", false);
                animator.SetBool("isChasing", true);
            }
        }

        private void StartPunch()
        {
            isPunching = true;
            punchTimer = punchDuration;

            if (playerTransform != null)
            {
                Vector3 enemyPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 playerPosXZ = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
                Vector3 lookDir = (playerPosXZ - enemyPosXZ).normalized;

                if (lookDir.sqrMagnitude > 0.001f)
                {
                    Quaternion baseRotation = Quaternion.LookRotation(lookDir);
                    Quaternion offsetRotation = Quaternion.Euler(0f, punchYawOffset, 0f);
                    transform.rotation = baseRotation * offsetRotation;
                }
            }

            if (animator != null)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isChasing", false);
                animator.SetBool("isPunching", true);
            }
        }

        private void MoveToCurrentWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return;
            }

            if (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    currentIndex++;
                    if (currentIndex >= waypoints.Length)
                    {
                        currentIndex = 0;
                    }

                    if (animator != null)
                    {
                        animator.SetBool("isWalking", true);
                    }
                }

                return;
            }

            Transform target = waypoints[currentIndex];

            Vector3 direction = (target.position - transform.position).normalized;


            Vector3 lookDirection = new Vector3(direction.x, 0f, direction.z);


            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            transform.position += direction * moveSpeed * Time.deltaTime;


            //Kiểm tra đã đến gần waypoint chưa
            Vector3 currentPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetPosXZ = new Vector3(target.position.x, 0, target.position.z);


            // Tính khoảng cách dựa trên mặt phẳng XZ
            float distance = Vector3.Distance(currentPosXZ, targetPosXZ);
            if (animator != null && distance > 0.1f)
            {
                animator.SetBool("isWalking", true);
            }


            if (distance < 0.2f)
            {
                waitTimer = waitDuration;

                if (animator != null)
                {
                    animator.SetBool("isWalking", false);
                }

            }
        }

        private bool IsPlayerWithinPatrolSegment(Vector3 enemyPosXZ, Vector3 playerPosXZ)
        {
            if (waypoints == null || waypoints.Length < 2) return false;

            Vector3 wp1XZ = new Vector3(waypoints[0].position.x, 0, waypoints[0].position.z);
            Vector3 wp2XZ = new Vector3(waypoints[1].position.x, 0, waypoints[1].position.z);

            float minX = MathF.Min(wp1XZ.x, wp2XZ.x);
            float maxX = MathF.Max(wp1XZ.x, wp2XZ.x);

            float margin = 0.1f;

            bool withinX = playerPosXZ.x >= minX - margin && playerPosXZ.x <= maxX + margin;

            //nam tren truc z
            float dzEnemyLane = MathF.Abs(playerPosXZ.z - enemyPosXZ.z);
            float laneThreshold = 0.5f;

            bool sameRowApprox = dzEnemyLane <= laneThreshold;

            return withinX && sameRowApprox;
        }
    }
}
