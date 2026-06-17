using System.Text;
using UnityEngine;

namespace EscapeDungeon.Maze
{
    public class MazeVisualizer : MonoBehaviour
    {
        public int width;
        public int height;

        [SerializeField] private float cellSize = 2f;
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private GameObject gatePrefab;

        private MazeGrid mazeGrid;
        private MazeGenerator mazeGenerator;

        //Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            InitializeMaze();

            //Tạo sàn cho width x height mazecell
            BuildFloor();

            //Tạo tường cho từng mazecell
            BuildWallsAndGates();
        }

        private void BuildWallsAndGates()
        {
            Vector3 wallPos;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var mazeCell = mazeGrid.Cells[x, y];
                    var center = new Vector3(x * cellSize, 0, y * cellSize);

                    //tính theo từng tường
                    if (mazeCell.HasLeftWall)
                    {
                        wallPos = center + new Vector3(-cellSize / 2f, 0, 0);
                        Instantiate(wallPrefab, wallPos, Quaternion.Euler(0f, 90f, 0f), transform);
                        //Debug.Log($"SPAWN LEFT wall at ({x},{y})");
                    }

                    if (x == width - 1 && mazeCell.HasRightWall)
                    {
                        wallPos = center + new Vector3(cellSize / 2f, 0, 0);
                        Instantiate(wallPrefab, wallPos, Quaternion.Euler(0f, 90f, 0f), transform);
                        //Debug.Log($"SPAWN Right wall at ({x},{y})");
                    }

                    if (y == height - 1 && mazeCell.HasTopWall)
                    {
                        wallPos = center + new Vector3(0, 0, cellSize / 2f);
                        Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
                        //Debug.Log($"SPAWN Top wall at ({x},{y})");
                    }

                    if (mazeCell.HasDownWall)
                    {
                        wallPos = center + new Vector3(0, 0, -cellSize / 2f);
                        if (x == 0 && y == 0)
                        {
                            wallPos = new Vector3(3.55f, 4.3f, -3.66f);
                            Instantiate(gatePrefab, wallPos, Quaternion.identity, transform);
                        }
                        else
                        {
                            Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
                        }
                        //Debug.Log($"SPAWN Down wall at ({x},{y})");
                    }
                }
            }
        }

        private void BuildFloor()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var center = new Vector3(x * cellSize, 0, y * cellSize);
                    Instantiate(floorPrefab, center, Quaternion.identity, transform);
                }
            }
        }

        private void InitializeMaze()
        {
            mazeGrid = new MazeGrid(width, height);
            mazeGenerator = new MazeGenerator();

            mazeGenerator.MazeGenerate(mazeGrid, 0, 0);
        }
    }
}