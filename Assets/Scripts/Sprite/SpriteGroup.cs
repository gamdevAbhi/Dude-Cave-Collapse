using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGroup : MonoBehaviour {
    [Header("Group Properties")]
    [SerializeField] private string sortingLayerName;
    public List<Renderer> sortingRenderer;

    private List<GroupChild> groupChild;
    private Camera _mainCamera;
    private float difference = 200f;
    private bool isVisible = false;

    private void Awake() {
        _mainCamera = Camera.main;
        SortGroup();
        if(GetComponent<Renderer>() == null) {
            groupChild = new List<GroupChild>();
            CreateChild(transform);
        }
    }

    private void LateUpdate() {
        if(groupChild != null) IsVisible();
        if(isVisible) SortGroup();
    }

    private void IsVisible() {
        isVisible = false;

        foreach(GroupChild child in groupChild) {
            if(child.isVisible) {
                isVisible = true;
                return;
            }
        }
    }

    private void CreateChild(Transform transform) {
        Renderer rend = transform.GetComponent<Renderer>();

        if(rend != null) {
            groupChild.Add(rend.gameObject.AddComponent<GroupChild>());
        }

        for(int i = 0; i < transform.childCount; i++) CreateChild(transform.GetChild(i));
    }

    public void SortGroup() {
        int distance = (int)((_mainCamera.transform.position - transform.position).magnitude * difference);

        for(int i = 0; i < sortingRenderer.Count; i++) {
            sortingRenderer[i].GetComponent<Renderer>().sortingLayerName = sortingLayerName;
            sortingRenderer[i].GetComponent<Renderer>().sortingOrder = -(distance + i);
        }
    }

    private void OnBecameVisible() {
        isVisible = true;
    }

    private void OnBecameInvisible() {
        isVisible = false;
    }
}
