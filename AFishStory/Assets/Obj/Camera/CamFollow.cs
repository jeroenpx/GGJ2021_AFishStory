using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = .4f;

    private float zDepth;
    private Vector3 currentVelocity;
    public float lookaheadAmount = .1f;

    public bool lookAheadEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        zDepth = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Adapt based on mouse position vs center
        Vector3 cursorpos = Input.mousePosition / new Vector2(Screen.height, Screen.height) * 2f - Vector2.one;
        float dist = cursorpos.magnitude;
        if(dist>1) {
            cursorpos /= dist;
        }
        if(!lookAheadEnabled) {
            cursorpos = Vector3.zero;
        }

        // Fish target position
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, zDepth);

        // Combined Target (fish + look around)
        Vector3 combinedTarget = targetPos + cursorpos * lookaheadAmount;

        transform.position = Vector3.SmoothDamp(transform.position, combinedTarget, ref currentVelocity, smoothTime);
    }
}
