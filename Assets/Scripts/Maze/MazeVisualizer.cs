using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace EscapeDungeon.Maze
{
    public class MazeVisualizer : MonoBehaviour
    {

        [SerializeField] private float cellSize = 2f;
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private GameObject gatePrefab;
        [SerializeField] private GameObject ceilingPrefab;

        private MazeGrid mazeGrid;

        public void InitializeMaze(MazeGrid mazeGrid)
        {
            this.mazeGrid = mazeGrid;

            ClearMazeVisual();
            BuildFloorNCeiling();
            BuildWallsAndGates();



        }

        private void ClearMazeVisual()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void BuildWallsAndGates()
        {
            Vector3 wallPos;
            for (int x = 0; x < mazeGrid.Width; x++)
            {
                for (int y = 0; y < mazeGrid.Height; y++)
                {
                    var mazeCell = mazeGrid.Cells[x, y];
                    Vector3 center = CellToWorld(x, y);

                    //tính theo từng tường
                    if (mazeCell.HasLeftWall)
                    {
                        wallPos = center + new Vector3(-cellSize / 2f, 0, 0);
                        Instantiate(wallPrefab, wallPos, Quaternion.Euler(0f, 90f, 0f), transform);
                        //Debug.Log($"SPAWN LEFT wall at ({x},{y})");
                    }

                    if (x == mazeGrid.Width - 1 && mazeCell.HasRightWall)
                    {
                        wallPos = center + new Vector3(cellSize / 2f, 0, 0);
                        Instantiate(wallPrefab, wallPos, Quaternion.Euler(0f, 90f, 0f), transform);
                        //Debug.Log($"SPAWN Right wall at ({x},{y})");
                    }

                    if (y == mazeGrid.Height - 1 && mazeCell.HasTopWall)
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
                            wallPos = new Vector3(5f, 4.3f, -4.65f);
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

        public Vector3 CellToWorld(int cellX, int cellY)
        {
            var worldX = cellX * cellSize;
            var worldZ = cellY * cellSize;
            var worldY = 0;

            return new Vector3(worldX, worldY, worldZ);
        }

        public MazeCell WorldToCell(Vector3 worldPosition)
        {
            var worldX = worldPosition.x;
            var worldZ = worldPosition.z;

            int cellX = (int)Math.Round(worldX / cellSize);
            int cellY = (int)Math.Round(worldZ / cellSize);

            //Giới hạn 
            if(cellX < 0)
            {
                cellX = 0;
            }

            if(cellX >= mazeGrid.Width)
            {
                cellX = mazeGrid.Width - 1;
            }

            if(cellY < 0)
            {
                cellY = 0;
            }

            if(cellY >= mazeGrid.Height)
            {
                cellY = mazeGrid.Height - 1;
            }

            return new MazeCell(cellX, cellY);
        }

        private void BuildFloorNCeiling()
        {
            for (int x = 0; x < mazeGrid.Width; x++)
            {
                for (int y = 0; y < mazeGrid.Height; y++)
                {
                    var center = CellToWorld(x, y) + new Vector3(0,0.5f,0);
                    Instantiate(floorPrefab, center, Quaternion.identity, transform);

                    var ceilingPosition = center + new Vector3(0, 9.7f, 0);
                    Instantiate(floorPrefab, ceilingPosition, Quaternion.identity, transform);
                }
            }

            SpawnExitFloor();

        }

        private void SpawnExitFloor()
        {
            var exitCell = mazeGrid.exitCell;
            var exit = CellToWorld(exitCell.X, exitCell.Y) + new Vector3(0, 1f,0);
            var exitObj = Instantiate(floorPrefab, exit, Quaternion.identity, transform);
            exitObj.tag = "Exit";

            //bật isTrigger
            var col = exitObj.GetComponent<Collider>();
            var mesh = exitObj.GetComponent<MeshRenderer>();
            if(col != null)
            {
                col.isTrigger = true;
                mesh.enabled = false;
            }
        }
    }
}