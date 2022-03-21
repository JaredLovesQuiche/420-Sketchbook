using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    public class Node
    {
        public Vector3 position;

        public float G { get; private set; }
        public float H { get; private set; }
        public float F => G + H;

        public float moveCost = 1;
        public List<Node> neighbors = new List<Node>();
        public Node parent { get; private set; }
        public void UpdateParentAndG(Node parent, float extraG = 0)
        {
            this.parent = parent;
            if (parent != null)
            {
                G = parent.G + moveCost + extraG;
            }
            else G = 0;
        }

        public void DoHeuristic(Node end)
        {
            H = (end.position - this.position).magnitude;
        }
    }

    public static List<Node> Solve(Node start, Node end)
    {
        if (start == null || end == null) return new List<Node>();

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        open.Add(start);

        // 1. travel from start to end
        while(open.Count > 0)
        {
            // find node in open list with smallest F val
            float bestF = float.MaxValue;
            Node current = null;
            foreach(Node n in open)
            {
                if (n.F < bestF)
                {
                    current = n;
                    bestF = n.F;
                }
            }

            if (current == end)
            {
                break;
            }
            bool isDone = false;    
            foreach(Node neighbor in current.neighbors)
            {
                if (!closed.Contains(neighbor))
                {
                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                        //set parent
                        //set G & H cost

                        float dis = (neighbor.position - current.position).magnitude;

                        neighbor.UpdateParentAndG(current);
                        if (neighbor == end) { isDone = true; }
                        neighbor.DoHeuristic(end);
                    }
                    else
                    {
                        // TODO: if G cost is lower, change neighbor's parent

                        float dis = (neighbor.position - current.position).magnitude;
                        if (current.G + neighbor.moveCost + dis < neighbor.G)
                        {
                            // its shorter to move to neighbor from current
                            neighbor.UpdateParentAndG(current, dis);
                        }
                    }
                }
            }

            closed.Add(current);
            open.Remove(current);

            if (isDone) break;
        }
        // 2. travel from end to start, building path
        List<Node> path = new List<Node>();

        for (Node temp = end; temp != null; temp = temp.parent)
        {
            path.Add(temp);
        }

        // 3. reverse created path
        path.Reverse();

        return path;
    }
}

/*
Dijkstra -
    Keep a list of OPEN nodes
    Foreach node:
        Record how far node is from start
        Add neighbors to OPEN list
            if it is END, return chain of nodes
        Move to closed list

Greedy Best-Search -
    Keep a list of OPEN nodes
    Pick one node most likely to be closer to END // heuristic
        Add neighbors to OPEN list
            if is END, return the chain of nodes
        move to CLOSED list

A* -
    Keep a list of OPEN nodes
    Pick one node with lowest cost (cost == how far from start + how far from end)
        Add neighbors to OPEN list
            Record how far node is from START
            if is END, return the chain of nodes
        move to CLOSED list
    
    F = G + H
    Heuristic
        Euclidean (Line to end)
        Manhattan (dx + dy)
*/
