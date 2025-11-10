using System.Collections.Generic;
using UnityEngine;

namespace PathSystem
{
    public enum TroopSpawnState
    {
        Center,
        Up,
        Down
    }

    public class LanePath : MonoBehaviour
    {
        [Tooltip("The waypoints that define this path - troops will follow these in order")]
        public List<GameObject> waypoints = new List<GameObject>();
        
        [Tooltip("Visual representation of the path for debugging")]
        public bool showGizmos = true;
        
        [Tooltip("Color for gizmos in the Scene view")]
        public Color gizmoColor = Color.green;

        // Track the last spawn position state for this path
        public TroopSpawnState lastSpawnState = TroopSpawnState.Center;

        // Get the starting position of the path
        public Vector3 GetStartPoint()
        {
            if (waypoints.Count > 0)
                return waypoints[0].transform.position;
            return Vector3.zero;
        }

        // Get the end position of the path
        public Vector3 GetEndPoint()
        {
            if (waypoints.Count > 0)
                return waypoints[waypoints.Count - 1].transform.position;
            return Vector3.zero;
        }

        // Get the waypoint at a specific index
        public Vector3 GetWaypoint(int index)
        {
            if (index >= 0 && index < waypoints.Count)
                return waypoints[index].transform.position;
            return Vector3.zero;
        }

        // Get the total number of waypoints in this path
        public int GetWaypointCount()
        {
            return waypoints.Count;
        }

        // Visualize the path in the editor
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            // Draw path segments
            if (waypoints != null && waypoints.Count >= 2)
            {
                for (int i = 1; i < waypoints.Count; i++)
                {
                    Gizmos.color = gizmoColor;
                    Gizmos.DrawLine(waypoints[i - 1].transform.position, waypoints[i].transform.position);
                    
                    // Draw waypoint spheres
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(waypoints[i - 1].transform.position, 0.2f);
                }
                
                // Draw last waypoint
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(waypoints[waypoints.Count - 1].transform.position, 0.2f);
            }
        }
    }
}