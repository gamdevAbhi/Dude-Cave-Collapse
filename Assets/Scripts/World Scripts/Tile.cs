using UnityEngine;

public class Tile : MonoBehaviour {
    [HideInInspector] public Object tile;
    [HideInInspector] public Object _object;
    [HideInInspector] public GameObject _objectRef;
    
    private PlayerController tileOwner;
    private bool isFree;

    public PlayerController _tileOwner {
        get {return tileOwner;}
    }

    public bool _isFree {
        get {
            CheckOwner();
            return isFree;
        }
    }

    private void CheckOwner() {
        if(tileOwner != null && tileOwner._targetTile == this
        || _object != null && _object.walkable == false && _objectRef != null) isFree = false;  
        else isFree = true;
    }

    public Tile UpdateOnwer(PlayerController holder) {
        CheckOwner();
        if(!isFree) return null;

        tileOwner = holder;
        return this;
    }

    private void Awake() {
        CheckOwner();
    }

    private void LateUpdate() {
        CheckOwner();
    }
}
