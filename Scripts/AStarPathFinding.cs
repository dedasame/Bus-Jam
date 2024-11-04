using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinding : MonoBehaviour
{
    public static AStarPathFinding instance;
    private void Awake()
    {
        instance = this;
    }

    public List<GridScript> GeneratePath(GridScript start,GridScript end)
    {
        List<GridScript> openSet = new List<GridScript>();

        //butun gridlerin gScore = +sonsuz yaptik
        foreach(GridScript n in FindObjectsOfType<GridScript>())
        {
            n.gScore = float.MaxValue;
        }

        //basladigimiz gridin skoru=0 
        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        //openSet -> ??
        while(openSet.Count > 0)
        {
            int lowestF = default;

            for(int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }
            
            GridScript currentNode = openSet[lowestF];
            openSet.Remove(currentNode); 

            if(currentNode == end)
            {
                List<GridScript> path = new List<GridScript>();

                path.Insert(0, end);

                while(currentNode != start)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }
                path.Reverse();
                return path;
            }

            foreach(GridScript connectedNode in currentNode.neighbours)
            {
                //komsularinda -> null / isAvaible = false / have person ->>> SKIP  || !connectedNode.isAvaible 
                if(connectedNode == null || connectedNode.personList.Count != 0) continue;
                float heldGScore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);

                if(heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }
        }
        return null;
    }

}
