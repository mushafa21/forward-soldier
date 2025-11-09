using System.Collections.Generic;
using UnityEngine;

namespace PathSystem
{
    public class PathManager : MonoBehaviour
    {
        [Tooltip("All paths in the current level")]
        public List<LanePath> paths = new List<LanePath>();

        private static PathManager instance;

        public static PathManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PathManager>();
                    if (instance == null)
                    {
                        GameObject managerObject = new GameObject("PathManager");
                        instance = managerObject.AddComponent<PathManager>();
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Add a path to the manager
        public void AddPath(LanePath path)
        {
            if (!paths.Contains(path))
            {
                paths.Add(path);
            }
        }

        // Remove a path from the manager
        public void RemovePath(LanePath path)
        {
            paths.Remove(path);
        }

        // Get a path by index
        public LanePath GetPath(int index)
        {
            if (index >= 0 && index < paths.Count)
            {
                return paths[index];
            }
            return null;
        }

        // Get a random path
        public LanePath GetRandomPath()
        {
            if (paths.Count > 0)
            {
                int randomIndex = Random.Range(0, paths.Count);
                return paths[randomIndex];
            }
            return null;
        }

        // Get all paths
        public List<LanePath> GetAllPaths()
        {
            return paths;
        }

        // Find the closest path to a position
        public LanePath GetClosestPath(Vector3 position)
        {
            LanePath closestPath = null;
            float minDistance = float.MaxValue;

            foreach (LanePath path in paths)
            {
                if (path != null && path.waypoints.Count > 0)
                {
                    // Calculate distance to the start of the path
                    float distance = Vector3.Distance(path.GetStartPoint(), position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPath = path;
                    }
                }
            }

            return closestPath;
        }

        // Check if there are any paths available
        public bool HasPaths()
        {
            return paths.Count > 0;
        }

        // Get the total number of paths
        public int GetPathCount()
        {
            return paths.Count;
        }
    }
}