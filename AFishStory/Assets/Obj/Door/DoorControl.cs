using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorControl : MonoBehaviour
{

    public Transform door;
    public Vector3 openPosition;

    public int memoriesFound = 0;
    public int totalMemories = 2;

	public TMP_Text text;

    private void UnlockDoor() {
        door.transform.position = openPosition;
        Destroy(text.gameObject);
    }

    private void UpdateCount() {
        text.text = memoriesFound+"/"+totalMemories+" memories";
    }

    public void FoundMemory() {
        memoriesFound++;
        UpdateCount();
        if(memoriesFound == totalMemories) {
            UnlockDoor();
        }
    }
}
