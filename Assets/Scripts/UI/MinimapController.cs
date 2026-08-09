using EscapeDungeon.Maze;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeDungeon.Ui
{
    public class MinimapController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RawImage minimapImage;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private MazeVisualizer mazeVisualizer;

        private MazeGrid mazeGrid;
        private bool[,] isRevealed;

        //Texture vẽ minimap
        private Texture2D minimapTexture;

        //Kích thước maze -> lấy từ mazegrid
        private int mazeWidth;
        private int mazeHeight;

        // Cấu hình màu chung
        [Header("Color")]
        [SerializeField] private Color wallColor = Color.black;
        [SerializeField] private Color floorColor = Color.white;
        [SerializeField] private Color fogColor = Color.grey;
        [SerializeField] private Color playerColor = Color.yellow;
        [SerializeField] private Color exitColor = Color.red;


        //mỗi cell là ô vuông 4x4 pixel -> dễ nhìn hơn
        [Header("Display")]
        [SerializeField] private int pixelPerCell = 4;
        [SerializeField] private int wallThickness = 2;
        private int radius = 2;
        private MazeCell lastPlayerCell;

        //TODO: Thêm các hàm dưới này
        private void Update()
        {
            if (mazeGrid == null || playerTransform == null)
                return;

            RevealAroundPlayer();
        }

        public void initMiniMap(MazeGrid mazeGrid)
        {
            this.mazeGrid = mazeGrid;
            mazeWidth = mazeGrid.Width;
            mazeHeight = mazeGrid.Height;
            lastPlayerCell = null;

            isRevealed = new bool[mazeWidth, mazeHeight];

            //Khởi tạo pixelPerCell động -> phụ thuộc kích thước maze
            RectTransform rt = minimapImage.rectTransform;
            float rawWidth = rt.rect.width;
            float rawHeight = rt.rect.height;

            int pixelPerCellX = Mathf.FloorToInt(rawWidth / mazeWidth);
            int pixelPercellY = Mathf.FloorToInt(rawHeight / mazeHeight);
            pixelPerCell = Math.Min(pixelPercellY, pixelPerCellX);

            if (pixelPerCell < 6) pixelPerCell = 6;

            //Khởi tạo cho minimap
            int windowWidth = radius * 2 + 1;
            int windowHeight = radius * 2 + 1;

            int texWidth = windowWidth * pixelPerCell;
            int texHeight = windowHeight * pixelPerCell;

            minimapTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
            minimapTexture.filterMode = FilterMode.Point; //zoom vào ra không bị blur(mờ)

            minimapImage.texture = minimapTexture;

            Debug.Log($"Minimap texture: {minimapTexture.width}x{minimapTexture.height}, pixelPerCell={pixelPerCell}");
            //Vẽ lần đầu với toàn bộ là fog
            FillWithColor();
            minimapTexture.Apply();

        }

        private void FillWithColor()
        {
            int texWidth = minimapTexture.width;
            int texHeight = minimapTexture.height;

            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    minimapTexture.SetPixel(x, y, fogColor);
                }
            }
        }

        public void RevealAroundPlayer(){
            var playerCell = mazeVisualizer.WorldToCell(playerTransform.position);
            Debug.Log($"Player at cell ({playerCell.X},{playerCell.Y})");

            if (playerCell.X < 0 || playerCell.X >= mazeWidth || playerCell.Y < 0 || playerCell.Y >= mazeHeight)
            {
                return;
            }

            isRevealed[playerCell.X, playerCell.Y] = true;
            Debug.Log($"Reveal cell ({playerCell.X},{playerCell.Y})");

            RedrawMinimap();
           
        }

        private void RedrawMinimap()
        {
            if (minimapTexture == null || mazeGrid == null)
                return;

            var playerCell = mazeVisualizer.WorldToCell(playerTransform.position);


           int minX = Mathf.Max(0, playerCell.X - radius);
           int maxX = Mathf.Min(mazeWidth - 1, playerCell.X + radius);
           int minY = Mathf.Max(0, playerCell.Y - radius);
           int maxY = Mathf.Min(mazeHeight - 1, playerCell.Y + radius);

            FillWithColor();

            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    var cell = mazeGrid.GetCell(i, j);
                    if (cell == null) continue;

                    if (!isRevealed[i, j])
                    {
                        continue;
                    }

                    int localX = i - (playerCell.X - radius); // 0..windowWidth-1
                    int localY = j - (playerCell.Y - radius);

                    HandleIsRevealCell(localX, localY, cell);
                }
            }

            int windowWidth = radius * 2 + 1;
            int centerCellX = windowWidth / 2;
            int centerCellY = windowWidth / 2;
            DrawPlayerIcon(centerCellX, centerCellY);

            minimapTexture.Apply();

            }

        private void DrawPlayerIcon(int cellX, int cellY)
        {
            Debug.Log($"DrawPlayerIcon called at cell ({cellX}, {cellY})");
            
            int startX = cellX * pixelPerCell;
            int startY = cellY * pixelPerCell;

            int centerX = startX + pixelPerCell / 2;
            int centerY = startY + pixelPerCell / 2;

            // Vẽ biểu tượng người chơi lớn hơn để dễ nhìn thấy
            int iconRadius = 5;
            for (int dx = -iconRadius; dx <= iconRadius; dx++)
            {
                for (int dy = -iconRadius; dy <= iconRadius; dy++)
                {
                    int px = centerX + dx;
                    int py = centerY + dy;

                    if (px >= 0 && px < minimapTexture.width &&
                        py >= 0 && py < minimapTexture.height)
                    {
                        minimapTexture.SetPixel(px, py, playerColor);
                    }
                }
            }
            Debug.Log($"Center pixel: {centerX}, {centerY}");
        }

        private void HandleIsRevealCell(int i, int j, MazeCell cell)
        {
            DrawCellBlock(i, j, floorColor);

            if (MazeCell.IsEqual(cell, mazeGrid.exitCell))
            {
                DrawCellBlock(i, j, exitColor   );
            }

            if (cell.HasTopWall)
            {
                DrawCellTopWall(i, j, wallColor);
            }
            if (cell.HasDownWall)
            {
                DrawCellDownWall(i, j, wallColor);
            }
            if (cell.HasLeftWall)
            {
                DrawCellLeftWall(i, j, wallColor);
            }
            if (cell.HasRightWall)
            {
                DrawCellRightWall(i, j, wallColor);
            }
        }


        private void DrawCellRightWall(int i, int j, Color wallColor)
        {
            int startX = i * pixelPerCell;
            int startY = j * pixelPerCell;

            int xStart = startX + (pixelPerCell - wallThickness);

            for (int t = 0; t < wallThickness; t++)
            {
                int x = xStart + t;
                for (int dy = 0; dy < pixelPerCell; dy++)
                {
                    minimapTexture.SetPixel(x, startY + dy, wallColor);
                }
            }
        }

        private void DrawCellLeftWall(int i, int j, Color wallColor)
        {
            int startX = i * pixelPerCell;
            int startY = j * pixelPerCell;


            for (int t = 0; t < wallThickness; t++)
            {
                int x = startX + t;
                for (int dy = 0; dy < pixelPerCell; dy++)
                {
                    minimapTexture.SetPixel(x, startY + dy, wallColor);
                }
            }
        }

        private void DrawCellDownWall(int i, int j, Color wallColor)
        {
            int startX = i * pixelPerCell;
            int startY = j * pixelPerCell;

            for (int t = 0; t < wallThickness; t++)
            {
                int y = startY + t;
                for (int dx = 0; dx < pixelPerCell; dx++)
                {
                    minimapTexture.SetPixel(startX + dx, y, wallColor);
                }
            }
        }

        private void DrawCellTopWall(int i, int j, Color wallColor)
        {
            int startX = i * pixelPerCell;
            int startY = j * pixelPerCell;

            int yStart = startY + (pixelPerCell - 1);

            for (int t = 0; t < wallThickness; t++)
            {
                int y = yStart + t;
                for (int dx = 0; dx < pixelPerCell; dx++)
                {
                    minimapTexture.SetPixel(startX + dx, y, wallColor);
                }
            }
        }

        private void DrawCellBlock(int cellX, int cellY, Color color)
        {
            int startX = cellX * pixelPerCell;
            int startY = cellY * pixelPerCell;

            for (int dx = 0; dx < pixelPerCell; dx++)
            {
                for (int dy = 0; dy < pixelPerCell; dy++)
                {
                    minimapTexture.SetPixel(startX + dx, startY + dy, color);
                }
            }
        }
    }
}
