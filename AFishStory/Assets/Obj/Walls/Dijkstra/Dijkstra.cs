using System.Collections.Generic;

/**
 * The type to use as keys in the dictionary, ... Can be an "int".
 */
public class Dijkstra<T> {

    /**
     * Current distances to the node from the start point
     */
    public Dictionary<T, float> distances = new Dictionary<T, float>();

    /**
     * Nodes that we completely visited...
     */
    public HashSet<T> explored = new HashSet<T>();

    /**
     * Actual shortest path
     */
    public Dictionary<T, T> pathPrevious = new Dictionary<T, T>();

    /**
     * Callback to find neighbours
     */
    public delegate int FindNeighbours(T nodeIdx, T[] neighbours, float[] neighbourDistances);
    private FindNeighbours findNeighbours;
    private T[] neightboursAlloc;
    private float[] neighboursDistAlloc;

    /**
     * Constructor
     */
    public Dijkstra(FindNeighbours findNeighbours, T startPointIdx, int maxNeighbours) {
        this.findNeighbours = findNeighbours;
        neightboursAlloc = new T[maxNeighbours];
        neighboursDistAlloc = new float[maxNeighbours];
        distances[startPointIdx] = 0;
        run();
    }

    /**
     * Run the algorithm
     */
    private void run() {
        while (explored.Count != distances.Count) {
            step();
        }
    }

    /**
     * Visit a node and update distances to neighbours
     */
    private void step() {
        // Find node with smallest distance... NOTE: O(n²) at the moment!
        float min = float.PositiveInfinity;
        T next = default(T);
        foreach (KeyValuePair<T, float> node in distances) {
            if (node.Value < min && !explored.Contains(node.Key)) {
                min = node.Value;
                next = node.Key;
            }
        }
        explored.Add(next);

        float baseDistance = distances[next];

        // Neighbours
        int neighboursAmount = findNeighbours(next, neightboursAlloc, neighboursDistAlloc);

        // Run through neighbours...
        for (int i = 0;i<neighboursAmount;i++) {
            T neighbour = neightboursAlloc[i];
            float neighbourDist = neighboursDistAlloc[i];

            // Go to the neighbour
            float currentDistance = float.PositiveInfinity;
            if (distances.ContainsKey(neighbour)) {
                currentDistance = distances[neighbour];
            }
            if (currentDistance > baseDistance + neighbourDist) {
                distances[neighbour] = baseDistance + neighbourDist;
                pathPrevious[neighbour] = next;
            }
        }
    }
}