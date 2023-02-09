using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    public Memory memory;
    private void OnTriggerEnter(Collider other) {
        Item item = other.GetComponent<Item>();
        if (item != null && !memory.knownItems.Contains(item)) memory.knownItems.Add(item);
    }
}
