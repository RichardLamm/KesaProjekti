using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Environment : MonoBehaviour {

    private float timer = 0f;
    private float spreadingInterval = 15f;
    public Tilemap map;
    public Tilemap nodes;
    private List<List<Vector3Int>> forestEdges = new List<List<Vector3Int>>();
    private bool initDone = false;
    private struct Direction
    {
        int dir;
        const int max_dir = 3;
        public int RandomizeDir()
        {
            dir = Random.Range(0, max_dir);
            return dir;
        }
        public int NextDir()
        {
            if(++dir > max_dir) { dir = 0; }
            return dir;
        }
    }
    Direction direction;
	
	// Update is called once per frame
	void Update () {
        if (initDone)
        {
            timer += Time.deltaTime;
            if (timer >= spreadingInterval)
            {
                timer = 0;
                SpreadForest();
            }
        }
	}

    public void Ready()
    {
        initDone = true;
    }

    public void AddForest(List<Vector3Int> edges)
    {
        forestEdges.Add(new List<Vector3Int>(edges));
    }

    private void SpreadForest()
    {
        foreach(List<Vector3Int> self in forestEdges)
        {
            if(self.Count == 0) { continue; }
            int node = Random.Range(0, self.Count);
            if(self[node] == null) { continue; }
            int spreadDirection = direction.RandomizeDir();
            Vector3Int spreadPosition = new Vector3Int();
            Vector3Int selfPosition = self[node];
            for (int i = 0; i < 4; i++)
            {
                switch (spreadDirection)
                {
                    case 0:
                        spreadPosition = new Vector3Int(selfPosition.x, selfPosition.y + 1, 0);
                        break;
                    case 1:
                        spreadPosition = new Vector3Int(selfPosition.x + 1, selfPosition.y, 0);
                        break;
                    case 2:
                        spreadPosition = new Vector3Int(selfPosition.x, selfPosition.y - 1, 0);
                        break;
                    case 3:
                        spreadPosition = new Vector3Int(selfPosition.x - 1, selfPosition.y, 0);
                        break;
                }
                if (nodes.GetTile(spreadPosition) == null
                && map.GetTile(spreadPosition) != null)
                {
                    if (map.GetTile(spreadPosition).name.Contains("grass"))
                    {
                        Tile tempTile = (Tile)nodes.GetTile(new Vector3Int(selfPosition.x, selfPosition.y, 0));
                        tempTile.gameObject.transform.position = nodes.CellToWorld(selfPosition);
                        nodes.SetTile(spreadPosition,
                            nodes.GetTile(selfPosition));
                        break; // for (0 to 3) loop
                    }
                }
                spreadDirection = direction.NextDir();
                if(i == 3)
                {
                    continue;
                }
            }
            self.Remove(selfPosition);
            self.Add(spreadPosition);
        }
    }
}
