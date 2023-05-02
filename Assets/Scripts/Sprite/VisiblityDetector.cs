using UnityEngine;

public class VisiblityDetector : MonoBehaviour
{
    private bool isVisible;

    public bool _isVisble {
        get {return isVisible;}
    }

    private void OnBecameInvisible() {
        isVisible = false;
    }

    private void OnBecameVisible() {
        isVisible = true;
    }
}
