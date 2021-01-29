using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;

[CreateAssetMenu()]
public class AlgorithmPolyPathMeshGenBasic: IPolyPathMeshGenImpl
{
    protected struct DistanceData
    {
        // Calculated in ShortestPath step
        public Dictionary<Vertex, Vertex> vertexDistances;

        public Transform transform;
    }

    public float minPolyPathResolution = 10000f;// No limit

    public override void Paint(Transform transform, PolygonCollider2D poly, MeshFilter filter)
    {
        // Step 1: triangluate
        TriangleNet.Mesh mesh = StepTriangluate (poly);

        DistanceData d = StepShortestPath(transform, mesh);

        // Step 5: draw mesh
        Mesh unityMesh = TriangleNetMeshToUnity(mesh, ref d);
        filter.mesh = unityMesh;
    }

    /**
    * Simply output the triangluated polygon
    */
    protected TriangleNet.Mesh StepTriangluate (PolygonCollider2D poly)
    {
        // Get the base polygon
        //List<List<string>> basePolyLoops = poly.ToBoundaryTrails(null, true);

        // Merge Mesh 
        // Remove intersections!
        float scale = 100;
        List<List<Vector2>> loops = new List<List<Vector2>>();
        for(int i=0;i<poly.pathCount;i++) {
            loops.Add(new List<Vector2>(poly.GetPath(i)));
        }

        // Triangluate
        // NOTE: ASSUMES NO SELF-INTERSECTIONS!
        var polygon = new Polygon();
        foreach(List<Vector2> path in loops) {
            if(path.Count < 3)
            {
                continue;
            }
            Vector2 previous = path[path.Count-1];
            List<Vector2> subDivPath = new List<Vector2>();
            foreach(Vector2 point in path)
            {
                float totalDist = Vector2.Distance(previous, point);
                int divisions = Mathf.FloorToInt(totalDist/minPolyPathResolution);
                for(int i=0;i<divisions;i++)
                {
                    subDivPath.Add(previous+(point-previous)*(i+1)/(divisions+1));
                }

                subDivPath.Add(point);
                previous = point;
            }

            Vector2 A = path[path.Count-2];
            Vector2 B = path[path.Count-1];
            float angle = 0;
            foreach(Vector2 point in path)
            {
                angle+=Vector2.SignedAngle(B-A, point-B);
                A = B;
                B = point;
            }

            List<Vertex> vertices = new List<Vertex>();
            foreach(Vector2 point in subDivPath) {
                Vertex v = new Vertex(point.x, point.y);
                v.Label = 1;// Add "1" => boundary?
                vertices.Add(v);
            }
            Contour c = new Contour(vertices);
            bool hole = false;//angle > 0;
            polygon.Add(c, hole);
        }

        // Set quality and constraint options.
        var options = new ConstraintOptions() { ConformingDelaunay = true };
        var quality = new QualityOptions() { MinimumAngle = 25.0 };

        return (TriangleNet.Mesh)polygon.Triangulate(options, quality);
    }

    protected Vector2 ToVector2(Vertex vertex)
    {
        return new Vector2((float) vertex.X, (float) vertex.Y);
    }

    /**
     * Helper function - is this vertex on the edge?
     * 
     * => Using information calculated by StepTriangluate ()
     */
    protected bool IsOnEdge(Vertex v)
    {
        return v.Label == 1;
    }

    protected delegate void GetVertexInfo(Vector2 point, out Vector3 normal, out float depth);

    /**
     * Convert a TriangleNet mesh to a Unity mesh
     */
    protected Mesh TriangleNetMeshToUnity(TriangleNet.Mesh mesh, ref DistanceData data)
    {
        ICollection<Vertex> outVertices = mesh.Vertices;
        ICollection<Triangle> outTriangles = mesh.Triangles;

        Mesh unityMesh = new Mesh();
        List<Vector3> unityVertices = new List<Vector3>(outVertices.Count);
        List<Vector2> unityUvs = new List<Vector2>(outVertices.Count);
        Dictionary<Vertex, int> indexMap = new Dictionary<Vertex, int>();
        foreach (var vertex in outVertices)
        {
            unityVertices.Add(ToVector2(vertex));
            indexMap.Add(vertex, unityVertices.Count-1);

            // Other info per vertex...
            float dist = GetDistanceFromEdge(ref data, vertex, ToVector2(vertex));

            // Output
            Vector2 pos = ToVector2(vertex);// IGNORE position for now (we have global position in shader anyway)
            unityUvs.Add(new Vector2(dist, 0));
        }
        List<int> unityTriangles = new List<int>(outTriangles.Count);
        foreach (var triangle in outTriangles)
        {
            
            unityTriangles.Add(indexMap[triangle.GetVertex(0)]);
            unityTriangles.Add(indexMap[triangle.GetVertex(2)]);
            unityTriangles.Add(indexMap[triangle.GetVertex(1)]);
        }
        
        unityMesh.vertices = unityVertices.ToArray();
        unityMesh.uv = unityUvs.ToArray();
        unityMesh.triangles = unityTriangles.ToArray();

        return unityMesh;
    }


    //
    // -- 
    //

    /**
     * Calculate shortest path in the graph...
     */
    protected DistanceData StepShortestPath(Transform transform, TriangleNet.Mesh mesh)
    {
        Dictionary<Vertex, int> vertexMap = new Dictionary<Vertex, int>();
        Dictionary<int, Vertex> vertexMapInv = new Dictionary<int, Vertex>();
        Dictionary<int, List<int>> neighbours = new Dictionary<int, List<int>>();
        Dictionary<int, List<float>> neighbourDistances = new Dictionary<int, List<float>>();
        neighbours[0] = new List<int>();
        neighbourDistances[0] = new List<float>();
        foreach (var triangle in mesh.Triangles)
        {
            Vertex v1 = triangle.GetVertex(0);
            Vertex v2 = triangle.GetVertex(1);
            Vertex v3 = triangle.GetVertex(2);
            int[] idx = new int[3] {0, 0, 0};
            for(int i=0;i<3;i++) {
                Vertex v = triangle.GetVertex(i);
                if(!vertexMap.ContainsKey(v)) {
                    int newIdx = vertexMap.Count+1;
                    vertexMap[v] = newIdx;
                    vertexMapInv[newIdx] = v;
                    neighbours[newIdx] = new List<int>();
                    neighbourDistances[newIdx] = new List<float>();
                }
                idx[i] = vertexMap[v];
            }
            {
                float dist = Vector2.Distance(ToVector2(v1), ToVector2(v2));
                neighbours[idx[0]].Add(idx[1]);
                neighbourDistances[idx[0]].Add(dist);
                neighbours[idx[1]].Add(idx[0]);
                neighbourDistances[idx[1]].Add(dist);
            }
            {
                float dist = Vector2.Distance(ToVector2(v1), ToVector2(v3));
                neighbours[idx[0]].Add(idx[2]);
                neighbourDistances[idx[0]].Add(dist);
                neighbours[idx[2]].Add(idx[0]);
                neighbourDistances[idx[2]].Add(dist);
            }
            {
                float dist = Vector2.Distance(ToVector2(v2), ToVector2(v3));
                neighbours[idx[1]].Add(idx[2]);
                neighbourDistances[idx[1]].Add(dist);
                neighbours[idx[2]].Add(idx[1]);
                neighbourDistances[idx[2]].Add(dist);
            }
        }

        // Start on the edge!
        foreach (var vertex in mesh.Vertices)
        {
            if(IsOnEdge(vertex))
            {
                if(!vertexMap.ContainsKey(vertex)) {
                    int newIdx = vertexMap.Count+1;
                    vertexMap[vertex] = newIdx;
                    vertexMapInv[newIdx] = vertex;
                    neighbours[newIdx] = new List<int>();
                    neighbourDistances[newIdx] = new List<float>();
                }

                // Distance is 1, BUT one-directional path!
                neighbours[0].Add(vertexMap[vertex]);
                neighbourDistances[0].Add(1);
            }
        }

        Dijkstra<int> d = new Dijkstra<int>((int nodeIdx, int[] neighboursOut, float[] neighbourDistancesOut) =>
        {
            for(int i=0;i<neighbours[nodeIdx].Count;i++) 
            {
                neighboursOut[i] = neighbours[nodeIdx][i];
                neighbourDistancesOut[i] = neighbourDistances[nodeIdx][i];
            }
            return neighbours[nodeIdx].Count;
        }, 0, neighbours[0].Count /* HACK - WRONG ASSUMPTION - need to calculate the max!! */);

        Dictionary<Vertex, Vertex> closestOuterVertex = new Dictionary<Vertex, Vertex>();
        foreach (KeyValuePair<int, int> node in d.pathPrevious)
        {
            int current = node.Key;
            int next = node.Value;
            while(next!=0)
            {
                current = next;
                next = d.pathPrevious[next];
            }
            // We are on the edge...
            closestOuterVertex[vertexMapInv[node.Key]] = vertexMapInv[current];
            if(node.Value!=0)
            {
                Debug.DrawLine(transform.TransformPoint(ToVector2(vertexMapInv[node.Key]))+Vector3.back*4f, transform.TransformPoint(ToVector2(vertexMapInv[node.Value]))+Vector3.back*4f, Color.red, 100f);
            }
        }

        DistanceData data = new DistanceData();
        data.vertexDistances = closestOuterVertex;
        data.transform = transform;
        return data;
    }

    /**
     * Helper function - find the distance from the edge
     *
     * => Using information calculated by StepShortestPath()
     * 
     * Where v is the closest vertex to point
     */
    protected float GetDistanceFromEdge (ref DistanceData data, Vertex v, Vector2 point)
    {
        if(IsOnEdge(v)) {
            return 0;
        } else {
            return Vector2.Distance(ToVector2(data.vertexDistances[v]), point);
        }
    }
}
