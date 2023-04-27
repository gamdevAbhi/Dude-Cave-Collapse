using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupChild : MonoBehaviour {
    [HideInInspector] public bool isVisible;

    private void OnBecameVisible() {
        isVisible = true;
    } 

    private void OnBecameInvisible() {
        isVisible = false;
    }
}
