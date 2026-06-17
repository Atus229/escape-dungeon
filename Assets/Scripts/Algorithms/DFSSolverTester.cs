using EscapeDungeon.Maze;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeDungeon.Algorithms
{
    public class DFSSolverTester : MonoBehaviour
{
        public MazeGeneratorTester mazeTester;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                MazeGrid maze = mazeTester.currentMaze; 
                if (maze == null)
                {
                    Debug.LogError("MazeTester.CurrentMaze == null (maze chưa sinh)");
                    return;
                }

                Debug.Log("Đã lấy được maze, chuẩn bị test DFS...");
                

                var startCell = maze.GetCell(0, 0);
                var exitCell = maze.GetCell(4, 4);

                var dfsSolver = new DFSSolver();

                List<MazeCell> path = dfsSolver.FindPath(maze, startCell, exitCell);

                if (path == null || path.Count == 0)
                {
                    Debug.Log("No path found!!!");
                }

                else
                {
                    for (int i = 0; i < path.Count; i++)
                    {
                        Debug.Log($"step {i}: ({path[i].X}, {path[i].Y})");
                    }
                }
            }
        }

    
}
}
