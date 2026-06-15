using System;
using UnityEngine;
namespace EscapeDungeon.Maze
{
    public class NewMonoBehaviourScript : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

            var maze = new MazeGrid(10, 10);
            Debug.Log(maze.Width);
            Debug.Log(maze.Height);
            Debug.Log(maze.IsInside(5, 5));
            Debug.Log(maze.IsInside(2, 9));
            Debug.Log(maze.IsInside(8, 0));
            Debug.Log(maze.IsInside(7, 11));
            Debug.Log(maze.IsInside(14, 3));
            Debug.Log(maze.IsInside(14, 15));
            Debug.Log(maze.GetCell(0, 0));
            Debug.Log(maze.GetCell(2, 1));
            Debug.Log(maze.GetCell(5, 5));
            Debug.Log(maze.GetCell(10, 9));

        }
    }
}
