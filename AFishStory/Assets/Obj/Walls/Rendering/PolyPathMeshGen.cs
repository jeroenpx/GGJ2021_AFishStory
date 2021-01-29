using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPolyPathMeshGenImpl: ScriptableObject
{
    public abstract void Paint(Transform transform, PolygonCollider2D poly, MeshFilter filter);
}

[RequireComponent(typeof(MeshFilter), typeof(PolygonCollider2D))]
public class PolyPathMeshGen : MonoBehaviour
{
    protected PolygonCollider2D poly;

    public IPolyPathMeshGenImpl genImpl;

    // Start is called before the first frame update
    void Start()
    {
        poly = GetComponent<PolygonCollider2D>();
        genImpl.Paint(transform, poly, GetComponent<MeshFilter>());
    }

    [ContextMenu("Generate Mesh")]
    protected void MenuDoPaint() {
        Start();
    }
}
