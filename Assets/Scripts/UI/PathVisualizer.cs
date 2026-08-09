using EscapeDungeon.Core;
using EscapeDungeon.Maze;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EscapeDungeon.Ui
{
    public class PathVisualizer : MonoBehaviour
    {
        [SerializeField] private MazeVisualizer mazeVisualizer;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float hintYOffset = 1f;
        public MazeGrid currentMaze { private get; set; }

        public void ShowPath(List<MazeCell> path, float duration)
        {
            if(path==null || path.Count == 0)
            {
                Debug.Log("Đường đi không tồn tại!!!");
                return;
            }

            var worldPositions = new List<Vector3>(path.Count);
            var yOffset = new Vector3(0, hintYOffset, 0);

            foreach (var item in path)
            {
                var pos = mazeVisualizer.CellToWorld(item.X, item.Y) + yOffset;
                worldPositions.Add(pos);
            }

            lineRenderer.positionCount = worldPositions.Count;
            var positionArray = worldPositions.ToArray();
            lineRenderer.SetPositions(positionArray);
            lineRenderer.enabled = true;

            StartCoroutine(HideAfterSeconds(duration));

        }

        private void ClearPath()
        {
            if(lineRenderer == null)
            {
                return;
            }

            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }

        private IEnumerator HideAfterSeconds(float duration)
        {
            yield return new WaitForSeconds(duration);
            ClearPath();

            if(GameManager.Instance != null)
            {
                GameManager.Instance.OnHintTrailFinished();
            }
        }
    }

}
