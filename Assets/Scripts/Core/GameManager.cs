using EscapeDungeon.Algorithms;
using EscapeDungeon.GamePlay;
using EscapeDungeon.Maze;
using EscapeDungeon.Ui;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeDungeon.Core
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Victory,
        Lose
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private MazeGenerator mazeGenerator;
        [SerializeField] private MazeVisualizer mazeVisualizer;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PathVisualizer pathVisualizer;
        [SerializeField] private VictoryUI victoryUI;
        [SerializeField] private LoseUI loseUI;
        [SerializeField] private PauseUI pauseUI;
        [SerializeField] public HubController hubController;
        [SerializeField] private MinimapController minimap;
        [SerializeField] private int startRound = 1;
        [SerializeField] private LevelConfig[] levelConfigs;
        [SerializeField] private EnemyController enemyPrefab;
        [SerializeField] private ItemController potionPrefab;
        [SerializeField] private ItemController speedBoostPrefab;

        //biến width/height tạm
        [SerializeField] private int mazeWidth = 2;
        [SerializeField] private int mazeHeight = 2;

        public static GameManager Instance { get; private set; }
        public GameState currentState { get; private set; }
        private MazeGrid currentMaze;
        private DFSSolver dFSSolver;
        private int currentRound;
        private int hintsUsedThisRound;
        private int maxHints = 2;
        private int hintRemain;
        private int highestCompletedRound = 0;
        private int maxHP = 100;
        private int currentHP;
        private HintState currentHintState = HintState.NoUse;
        private EnemyController currentEnemy;
        private List<ItemController> spawnedItem = new List<ItemController>();
        private int totalRounds;

        //biến đo thời gian chơi
        private float remainingTime;
        private float roundElapsed;


        private void Awake()
        {
            currentState = GameState.MainMenu; 

            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            dFSSolver = new DFSSolver();
        }

        private void Start()
        {
            if(levelConfigs != null)
            {
                totalRounds = levelConfigs.Length;
                Debug.Log($"[GameManager] HandlePlayerReachExit - currentRound = {currentRound}, totalRounds = {totalRounds}");
            }    

            if (SessionConfig.StartNewGame)
            {
                Debug.Log("[GameManager] Starting a new game from round 1.");
                SessionConfig.StartNewGame = false;
                StartNewGame();

                return;
            }


            if (SessionConfig.OverrideRound > 0)
            {
                int roundToPlay = SessionConfig.OverrideRound;

                //reset OverideRound để thua thì chạy luôn start round

                SessionConfig.OverrideRound = 0;

                StartRound(roundToPlay);
            }

            PlayerData loadedData = LocalSaveSystem.Load();

            if (loadedData != null &&
                loadedData.Progress != null &&
                loadedData.Progress.CurrentRound >= startRound)
            {
                var progress = loadedData.Progress;

                currentRound = progress.CurrentRound;
                highestCompletedRound = progress.HighestCompletedRound;
                currentHP = progress.CurrentHP;
                hintRemain = progress.HintRemaining;
                roundElapsed = progress.TotalPlayTime;
                remainingTime = progress.RemainingTime;

                // Ưu tiên dùng maze đã lưu nếu có
                if (progress.MazeWidth > 0 && progress.MazeHeight > 0 &&
                    progress.Cells != null && progress.Cells.Count > 0)
                {
                    Debug.Log("[GameManager] Load existing maze for current round.");

                    currentMaze = RebuildMazeFromSave(progress);

                    // cập nhật width/height
                    mazeWidth = currentMaze.Width;
                    mazeHeight = currentMaze.Height;

                    // không gọi MazeGenerate nữa, chỉ visual lại
                    mazeVisualizer.InitializeMaze(currentMaze);
                    minimap.initMiniMap(currentMaze);

                    // đặt HUD
                    if (hubController != null)
                    {
                        hubController.UpdateHP(currentHP, maxHP);
                        hubController.UpdateHintHub(hintRemain);
                        hubController.UpdateTimer(remainingTime);
                        hubController.UpdateRound(currentRound);
                        hubController.SetHintState(currentHintState);
                    }

                    // đặt player về vị trí đã lưu
                    Vector3 savedPos = new Vector3(
                        progress.PlayerPosX,
                        progress.PlayerPosY,
                        progress.PlayerPosZ
                    );
                    playerController.ResetPosition(savedPos);
                    playerController.allowCursorHandling = true;
                    playerController.SetCanMove(true);

                    // Spawn enemy, item dựa trên maze hiện có
                    SpawnItemForRound();
                    SpawnEnemyForRound();

                    currentState = GameState.Playing;
                }
                else
                {
                    // Không có dữ liệu maze -> tạo round mới như cũ
                    Debug.Log("[GameManager] No maze data in save, start fresh round.");
                    StartRound(currentRound);
                }
            }
            else
            {
                Debug.Log("[GameManager] No valid save found. Start from round 1.");
                StartNewGame();
            }
        }

        private void Update()
        {
            //Pause Game
            HandlePauseInput();


            if (currentState != GameState.Playing)
            {
                return;
            }

            roundElapsed += Time.deltaTime;
            remainingTime -= Time.deltaTime;

            if(remainingTime <= 0)
            {
                remainingTime = 0;
                if(hubController != null)
                {
                    hubController.UpdateTimer(remainingTime);
                }

                HandlePlayerLose();
                return;
            }

            //TODO:Timer đếm ngược
            if(hubController != null)
            {
                hubController.UpdateTimer(remainingTime);
            }

            minimap.RevealAroundPlayer();

            //Thao tác gọi hint
            if (Input.GetKeyDown(KeyCode.H))
            {
                RequestHint();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                //TODO khi có đk thua thì xóa
                HandlePlayerLose();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                //TODO khi có đk thua thì xóa
                HandlePlayerReachExit(); 
            }


        }

        private void HandlePauseInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(currentState == GameState.Playing)
                {
                    EnterPause();
                }
                else if(currentState == GameState.Paused)
                {
                    Debug.Log("111111111111111111111");
                    ResumeGameFromPause();
                }
            }
        }

        private void EnterPause()
        {
            if(currentState != GameState.Playing)
            {
                return;
            }

            currentState = GameState.Paused;

            //khóa input
            playerController.allowCursorHandling = false;
            playerController.SetCanMove(false);

            //Mở chuột
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //Hiện pausepanel
            if(pauseUI != null)
            {
                pauseUI.Show();
            }
        }

        public void StartNewGame()
        {
            Debug.Log($"[Game Manager] Start new game, go to round {startRound}");
            
            //bắt đầu chơi từ round đầu tiên
            StartRound(startRound);
        }

        public void StartRound(int roundIndex)
        { 

            Debug.Log($"[Game Manager] Start round, go to round {roundIndex}");
            roundElapsed = 0;
            remainingTime = 60; // giả sử thời gian để test thua và hiển thị time hub -> TODO: format vào level 
            currentRound = roundIndex;
            hintsUsedThisRound = 0;
            currentState = GameState.Playing;
            currentHP = maxHP;
            currentHintState = HintState.NoUse;

            //khởi tạo mê cung mới
            var config = GetLevelConfig(roundIndex);

            if(config != null)
            {
                mazeWidth = config.mazeWidth;
                mazeHeight = config.mazeHeight;
                remainingTime = config.timeLimits;
                maxHints = config.maxHints;
            }
            else
            {
                remainingTime = 60;
                maxHints = 2;
            }

            hintRemain = maxHints - hintsUsedThisRound;

            if (hubController != null)
            {
                hubController.SetItemState(Ui.ItemType.None, false);
                hubController.UpdateHP(currentHP, maxHP);
                hubController.SetHintState(currentHintState);
                hubController.UpdateTimer(remainingTime);
                hubController.UpdateRound(currentRound);
                hubController.UpdateHintHub(hintRemain);
            }

            var startCell = new MazeCell(0, 0);
            var exitCell = new MazeCell(this.mazeWidth - 1, this.mazeHeight - 1);
            currentMaze = new MazeGrid(this.mazeWidth, this.mazeHeight, startCell, exitCell);
            mazeGenerator.MazeGenerate(currentMaze);
            mazeVisualizer.InitializeMaze(currentMaze);
            minimap.initMiniMap(currentMaze);
            SpawnItemForRound();

            //Đặt player vào maze
            var worldPos = mazeVisualizer.CellToWorld(0, 0);
            playerController.ResetPosition(worldPos);
            playerController.allowCursorHandling = true;
            playerController.SetCanMove(true);

            SpawnEnemyForRound();
        }

        private void SpawnEnemyForRound()
        {
            if (currentEnemy != null)
            {
                Destroy(currentEnemy.gameObject);
                currentEnemy = null;
            }

            int row = mazeHeight / 2;
            int minLaneHeight = 3;

            var lane = FindLaneInRow(row, minLaneHeight);
            if(lane == null)
            {
                Debug.Log("[GameManager] No suitable lane for enemy in this maze.");
                return;
            }

            int startX = lane.Value.StartX;
            int endX = lane.Value.EndX;

            var waypoint1 = mazeVisualizer.CellToWorld(startX, row);
            var waypoint2 = mazeVisualizer.CellToWorld(endX, row);

            Vector3 enemySpawnPos = (waypoint1 + waypoint2) / 2;

            currentEnemy = Instantiate(enemyPrefab, enemySpawnPos, Quaternion.identity);

            currentEnemy.playerTransform = playerController.transform;

            Transform wp1 = new GameObject("EnemyWaypoint1").transform;
            wp1.position = waypoint1;

            Transform wp2 = new GameObject("EnemyWaypoint2").transform;
            wp2.position = waypoint2;

            currentEnemy.waypoints = new Transform[] { wp1, wp2 };
        }

        private (int StartX, int EndX)? FindLaneInRow(int row, int minLaneHeight)
        {
            int startX = -1;
            int currentLength = 0;

            for(int x = 0; x < mazeWidth - 1; x++)
            {
                var currentCell = currentMaze.GetCell(x, row);
                var nextCell = currentMaze.GetCell(x + 1, row);

                bool wallBetween = currentCell.HasRightWall || nextCell.HasLeftWall;

                if (!wallBetween)
                {
                    if (currentLength == 0)
                    {
                        startX = x;
                    }
                    currentLength++;
                }
                else
                {
                    if(currentLength + 1 >= minLaneHeight) // số cell bằng số khoảng + 1
                    {
                        int endX = x;
                        return (startX, endX);
                    }

                    //không thỏa thì reset tìm lại
                    startX = -1;
                    currentLength = 0;
                }
            }

            //tìm đường lane từ đầu đến cuối x
            if(currentLength+1 >= minLaneHeight && startX >= 0)
            {
                int endX = mazeWidth - 1;
                return (startX, endX);
            }

            return null;
        }

        public void HandlePlayerReachExit() //playerController sẽ nghe sự kiện khi chạm vào exit -> gọi hàm này
        {
            Debug.Log($"[GameManager] HandlePlayerReachExit - currentRound = {currentRound}, totalRounds = {totalRounds}");

            playerController.SetCanMove(false);

            playerController.allowCursorHandling = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            highestCompletedRound = Math.Max(highestCompletedRound, currentRound);
            currentState = GameState.Victory;

            ClearItemForRound();

            bool isFinalRound = (currentRound >= totalRounds);

            SessionConfig.StartNewGame = false;
            SessionConfig.LastRoundTime = roundElapsed;

            if (isFinalRound)
            {
                // Không có round kế tiếp
                SessionConfig.OverrideRound = 0;
            }
            else
            {
                // Round kế tiếp là currentRound + 1
                SessionConfig.OverrideRound = currentRound + 1;
            }

            PlayerData data = BuildCurrentPlayerData();
            LocalSaveSystem.Save(data);

            if (victoryUI != null)
            {
                victoryUI.Show(roundElapsed, isFinalRound);
            }
        }

        public void HandlePlayerLose()
        {
            //TODO: HubController
            Debug.Log("Unlucky!!! You lose...");

            currentState = GameState.Lose;
            playerController.SetCanMove(false);

            //Mo chuot
            playerController.allowCursorHandling = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ClearItemForRound();
            //TODO: save progress
            SessionConfig.StartNewGame = false;
            SessionConfig.OverrideRound = startRound;
            SessionConfig.LastRoundTime = roundElapsed;

            currentRound = startRound;
            highestCompletedRound = 0;
            currentHP = maxHP;
            hintRemain = maxHints;

            var data = BuildCurrentPlayerData();
            LocalSaveSystem.Save(data);

            if (currentEnemy != null)
            {
                // Ví dụ tắt script hoặc set một flag trong EnemyController
                currentEnemy.enabled = false;
            }

            loseUI.Show(roundElapsed);

        }

        public void RequestHint()
        {
            if (currentState != GameState.Playing)
            {
                Debug.Log("Phải sử dụng hint khi đang chơi...");
                return;
            }

            if (hintRemain <= 0)
            {
                Debug.Log("No hints left!!!");
                return;
            }

            if (currentMaze == null) return;

            var playerPos = playerController.transform.position;
            var player = mazeVisualizer.WorldToCell(playerPos);

            var playerCell = currentMaze.GetCell(player.X, player.Y);
            var exitCell = currentMaze.exitCell;

            var path = dFSSolver.FindPath(currentMaze, playerCell, exitCell);

            if (path == null)
            {
                Debug.Log("No valid path!!!");
                return;
            }

            // TĂNG SỐ HINT ĐÃ DÙNG
            hintsUsedThisRound++;

            // Hiển thị đường hint
            pathVisualizer.currentMaze = currentMaze;
            pathVisualizer.ShowPath(path, 5);

            // Cập nhật số hint còn lại + trạng thái
            hintRemain = maxHints - hintsUsedThisRound;
            currentHintState = HintState.Using;

            if (hubController != null)
            {
                hubController.UpdateHintHub(hintRemain);      // THÊM DÒNG NÀY
                hubController.SetHintState(currentHintState); // HUD biết đang dùng hint
            }


        }

        public void OnVictoryContinueClicked()
        {
            Debug.Log("[GameManager] OnVictoryContinueClicked");

            int nextRound = currentRound + 1;
            Debug.Log($"[GameManager] Next round = {nextRound}");

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerController.SetCanMove(true);
            playerController.allowCursorHandling = true;

            StartRound(nextRound);
        }

        public void OnLoseContinueClicked()
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerController.allowCursorHandling = true;
            playerController.SetCanMove(true);


            StartNewGame();
        }

        public void ResumeGameFromPause()
        {
            if(currentState != GameState.Paused)
            {
                return;
            }

            currentState = GameState.Playing;

            //Mở nhập input và khóa chuột
            playerController.SetCanMove(true);
            playerController.allowCursorHandling = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            pauseUI.Hide();
        }

        public void QuitToMainMenu()
        {
            if (currentState == GameState.Playing || currentState == GameState.Paused)
            {
                SaveCurrentProgress();
            }

            currentState = GameState.MainMenu;
            SceneManager.LoadScene("MainMenu");
        }

        public void OnHintTrailFinished()
        {
            if(hintRemain <= 0)
            {
                currentHintState = HintState.Used;
            }
            else
            {
                currentHintState = HintState.NoUse;
            }

            if(hubController != null)
            {
                hubController.UpdateHintHub(hintRemain);
                hubController.SetHintState(currentHintState);
            }

        }

        private LevelConfig GetLevelConfig(int roundIndex)
        {
            if(levelConfigs == null || levelConfigs.Length == 0)
            {
                Debug.LogWarning("No LevelConfigs assigned!");
                return null;
            }

            int arrayIndex = roundIndex - 1;

            if(arrayIndex < 0 || arrayIndex >= levelConfigs.Length)
            {
                Debug.LogWarning($"No LevelConfig for round{roundIndex}");
                return null;
            }

            return levelConfigs[arrayIndex];
        }

        public void ApplyDamageToPlayer(int amount)
        {
            if (currentState != GameState.Playing)
            {
                return;
            }


            currentHP -= amount;
            if(currentHP< 0)
            {
                currentHP = 0;
            }

            if (hubController != null)
            {
                hubController.UpdateHP(currentHP, maxHP);
            }

            if (currentHP == 0)
            {
                HandlePlayerLose();
            }
        }

        public void HealthPlayer(int amount)
        {
            if(currentState != GameState.Playing)
            {
                return;
            }

            currentHP += amount;

            if(currentHP >= maxHP)
            {
                currentHP = maxHP;
            }

            if (hubController != null)
            {
                hubController.UpdateHP(currentHP, maxHP);
            }
        }

        private void SpawnItemForRound()
        {
            if (potionPrefab != null)
            {
                int centerX = (mazeWidth / 2)-1 ;
                int centerY = (mazeHeight / 2)-1;
                SpawnItemAtCell(potionPrefab, centerX, centerY); //Vi du truoc sua sau
            }

            if(speedBoostPrefab != null)
            {
                int centerX = mazeWidth / 2;
                int centerY = mazeHeight / 2;
                SpawnItemAtCell(speedBoostPrefab, centerX, centerY);
            }
        }

        private void SpawnItemAtCell(ItemController prefab, int v1, int v2)
        {
            var worldPos = mazeVisualizer.CellToWorld(v1, v2);

            worldPos += new Vector3(0, 1.5f, 0);

            var item = Instantiate(prefab, worldPos, Quaternion.identity);

            if(item != null)
            {
                spawnedItem.Add(item);
            }
        }

        private void ClearItemForRound()
        {
            foreach (var item in spawnedItem)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            spawnedItem.Clear();
        }

        private PlayerData BuildCurrentPlayerData()
        {
            var profile = new PlayerProfileData
            {
                PlayerName = "Player",
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                LastPlayedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            // Lấy vị trí player hiện tại
            Vector3 playerPos = playerController.transform.position;

            // Build progress
            var progress = new GameProgressData
            {
                CurrentRound = currentRound,
                HighestCompletedRound = highestCompletedRound,
                CurrentHP = currentHP,
                HintRemaining = hintRemain,
                TotalPlayTime = roundElapsed,
                RemainingTime = remainingTime,

                PlayerPosX = playerPos.x,
                PlayerPosY = playerPos.y,
                PlayerPosZ = playerPos.z,
            };

            // Nếu đang có maze thì lưu cấu trúc maze
            if (currentMaze != null)
            {
                progress.MazeWidth = currentMaze.Width;
                progress.MazeHeight = currentMaze.Height;
                progress.Cells = new List<MazeCell>();

                for (int y = 0; y < currentMaze.Height; y++)
                {
                    for (int x = 0; x < currentMaze.Width; x++)
                    {
                        MazeCell cell = currentMaze.GetCell(x, y);
                        progress.Cells.Add(cell);
                    }
                }
            }

            var setting = new GameSettingData
            {
                //TODO: Lam setting data
            };

            return new PlayerData
            {
                Profile = profile,
                Progress = progress,
                Setting = setting
            };

        }

        public void OnFinalVictoryQuitClicked()
        {
            QuitToMainMenu();
        }

        public void RestartFromRoundOne()
        {
            // Reset progress về round 1
            SessionConfig.StartNewGame = true;
            SessionConfig.OverrideRound = 0;

            // Có thể reset thêm HighestCompletedRound nếu muốn:
            highestCompletedRound = 0;

            StartNewGame();
        }

        public void SetHudItemState(EscapeDungeon.Ui.ItemType type, bool isActive)
        {
            
            Debug.Log($"[GameManager] SetHudItemState type = {type}, isActive = {isActive}");

            if (hubController == null)
            {
                Debug.LogWarning("[GameManager] hubController is NULL in SetHudItemState");
                return;
            }

            hubController.SetItemState(type, isActive);
        }

        public void SaveCurrentProgress()
        {
            var data = BuildCurrentPlayerData();
            LocalSaveSystem.Save(data);
        }

        private void OnApplicationQuit()
        {
            // Chỉ auto save khi đang chơi
            if (currentState == GameState.Playing)
            {
                SaveCurrentProgress();
            }
        }



        private MazeGrid RebuildMazeFromSave(GameProgressData progress)
        {
            if (progress.Cells == null || progress.Cells.Count == 0)
                return null;

            var startCell = new MazeCell(0, 0);
            var exitCell = new MazeCell(progress.MazeWidth - 1, progress.MazeHeight - 1);

            MazeGrid grid = new MazeGrid(progress.MazeWidth, progress.MazeHeight, startCell, exitCell);

            int index = 0;
            for (int y = 0; y < progress.MazeHeight; y++)
            {
                for (int x = 0; x < progress.MazeWidth; x++)
                {
                    MazeCell savedCell = progress.Cells[index++];
                    // Tuỳ vào thiết kế MazeGrid, em cần set lại cell
                    grid.SetCell(x, y, savedCell); // cần tự cài đặt hàm SetCell
                }
            }

            return grid;
        }

    }
}