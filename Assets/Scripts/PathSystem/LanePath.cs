using System.Collections.Generic;
using UnityEngine;

namespace PathSystem
{
    public class LanePath : MonoBehaviour
    {
        [Tooltip("The waypoints that define this path - troops will follow these in order")]
        public List<Vector3> waypoints = new List<Vector3>();
        
        [Tooltip("Visual representation of the path for debugging")]
        public bool showGizmos = true;
        
        [Tooltip("Color for gizmos in the Scene view")]
        public Color gizmoColor = Color.green;

        // Get the starting position of the path
        public Vector3 GetStartPoint()
        {
            if (waypoints.Count > 0)
                return waypoints[0];
            return Vector3.zero;
        }

        // Get the end position of the path
        public Vector3 GetEndPoint()
        {
            if (waypoints.Count > 0)
                return waypoints[waypoints.Count - 1];
            return Vector3.zero;
        }

        // Get the waypoint at a specific index
        public Vector3 GetWaypoint(int index)
        {
            if (index >= 0 && index < waypoints.Count)
                return waypoints[index];
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
                    Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);
                    
                    // Draw waypoint spheres
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(waypoints[i - 1], 0.2f);
                }
                
                // Draw last waypoint
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(waypoints[waypoints.Count - 1], 0.2f);
            }
        }
    }
}