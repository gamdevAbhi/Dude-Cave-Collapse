using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGroup : MonoBehaviour {
    [Header("Reference")]
    public List<Renderer> sortingRenderer;
    [Header("Group Properties")]
    [SerializeField] private string sortingLayerName;

    private Camera _mainCamera;
    private float difference = 200f;

    private void Awake() {
        _mainCamera = Camera.main;
        SortGroup();
    }

    private void LateUpdate() {
        SortGroup();
    }

    public void SortGroup() {
        int distance = (int)((_mainCamera.transform.position - transform.position).magnitude * difference);

        for(int i = 0; i < sortingRenderer.Count; i++) {
            sortingRenderer[i].GetComponent<Renderer>().sortingLayerName = sortingLayerName;
            sortingRenderer[i].GetComponent<Renderer>().sortingOrder = -(distance + i);
        }
    }
}
