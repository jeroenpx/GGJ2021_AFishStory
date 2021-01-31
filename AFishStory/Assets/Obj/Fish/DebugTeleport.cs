using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTeleport : MonoBehaviour
{
    public Transform fish;

    [ContextMenu("Teleport Here")]
    private void TeleportHere() {
        fish.transform.position = transform.position;
    }
}
