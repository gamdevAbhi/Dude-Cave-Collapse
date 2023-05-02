using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IAttacker {
    [Header("Reference")]
    [SerializeField] private Transform handPivot;
    [SerializeField] private CaveManager caveManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CameraController cameraController;
    [Header("Player Properties")]
    [SerializeField] private float speed;
    
    private float minDistance = 0.25f;
    private Vector3 normalScale;
    private Tile targetTile;
    private Tile facingTile;
    private ColliderDetector detector;

    public Tile _targetTile {
        get {return targetTile;}
    }

    public bool _isMoving {
        get {
            if(transform.position != targetTile.transform.position) return true;
            else return false;
        }
    }

    public void Attack() {
        if(facingTile != null && facingTile._objectRef != null) {
            IHealth health = facingTile._objectRef.GetComponent<IHealth>();
            if(health != null) health.TakeDamage(1000);
        }
    }

    public void Enter() {
        if(targetTile == null || targetTile._object == null) return;
        
        CaveGate gate = targetTile._objectRef.GetComponent<CaveGate>();
        gate.VisitDoor();
    }

    private void Move() {
        Vector3 targetPos = targetTile.transform.position;
        targetPos.y = transform.position.y;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void ChangeTarget(Room currentRoom, float x, float z) {
        Vector2 pos = new Vector2(Mathf.Min(currentRoom.grid.GetLength(0) - 1, x), 
        Mathf.Min(currentRoom.grid.GetLength(1) - 1, z));
        if(pos.x < 0) pos.x = 0;
        if(pos.y < 0) pos.y = 0;

        if(currentRoom.grid[(int)pos.x, (int)pos.y].tile.env
        != Object.Enviroment.Water && currentRoom.grid[(int)pos.x, (int)pos.y].UpdateOnwer(this) != null) {
            targetTile = currentRoom.grid[(int)pos.x, (int)pos.y];
        }
    }

    private void FixPos() {
        Room currentRoom = caveManager._currentRoom;

        Vector2 currentPos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.z));
        currentPos = new Vector2(Mathf.Min(currentRoom.grid.GetLength(0) - 1, currentPos.x), 
        Mathf.Min(currentRoom.grid.GetLength(1) - 1, currentPos.y));

        if(currentPos.x < 0) currentPos.x = 0;
        if(currentPos.y < 0) currentPos.y = 0;

        if(currentRoom.grid[(int)currentPos.x,(int)currentPos.y].transform.position != transform.position) {
            ChangeTarget(currentRoom, currentPos.x, currentPos.y);
        }
    }

    private Tuple<Vector2, Vector3> GetDistance() {
        Vector2 currentPos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.z));
        Vector3 distance = targetTile.transform.position;
        distance.y = transform.position.y;
        distance = distance - transform.position;

        return new Tuple<Vector2, Vector3>(currentPos, distance);
    }

    private void MoveUp() {
        Room currentRoom = caveManager._currentRoom;
        Tuple<Vector2, Vector3> vectors = GetDistance();
        Vector2 pos = vectors.Item1 + cameraController.cameraDirection.upVector;

        if(vectors.Item2.magnitude <= minDistance) ChangeTarget(currentRoom, pos.x, pos.y);
    }

    private void MoveDown() {
        Room currentRoom = caveManager._currentRoom;
        Tuple<Vector2, Vector3> vectors = GetDistance();
        Vector2 pos = vectors.Item1 - cameraController.cameraDirection.upVector;

        if(vectors.Item2.magnitude <= minDistance) ChangeTarget(currentRoom, pos.x, pos.y);
    }

    private void MoveForward() {
        Room currentRoom = caveManager._currentRoom;
        Tuple<Vector2, Vector3> vectors = GetDistance();
        Vector2 pos = vectors.Item1 + cameraController.cameraDirection.leftVector;

        if(vectors.Item2.magnitude <= minDistance) ChangeTarget(currentRoom, pos.x, pos.y);
    }

    private void MoveBackward() {
        Room currentRoom = caveManager._currentRoom;
        Tuple<Vector2, Vector3> vectors = GetDistance();
        Vector2 pos = vectors.Item1 - cameraController.cameraDirection.leftVector;

        if(vectors.Item2.magnitude <= minDistance) ChangeTarget(currentRoom, pos.x, pos.y);
    }

    private void MoveHand() {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(handPivot.position);
        Vector3 direction = Input.mousePosition - screenPos;
        float angle = (transform.localScale.x > 0)? Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
        : Mathf.Atan2(direction.y, -direction.x)  * Mathf.Rad2Deg;
        
        handPivot.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    private void LookDirection() {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = Input.mousePosition - screenPos;

        if(direction.x >= 0f) transform.localScale = normalScale;
        else transform.localScale = new Vector3(-normalScale.x,normalScale.y,normalScale.z);

    }

    private void SetFacingTile() {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = Input.mousePosition - screenPos;
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);

        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
            pos += (direction.x > 0)? cameraController.cameraDirection.leftVector 
            : -cameraController.cameraDirection.leftVector;
        }
        else {
            pos += (direction.y > 0)? cameraController.cameraDirection.upVector
            : -cameraController.cameraDirection.upVector;
        }

        facingTile = caveManager.GetTile(pos.x, pos.y);
    }

    private void AddKeys() {
        inputManager.inputs["Up"].onPress += MoveUp;
        inputManager.inputs["Down"].onPress += MoveDown;
        inputManager.inputs["Foward"].onPress += MoveForward;
        inputManager.inputs["Backward"].onPress += MoveBackward;
        inputManager.inputs["Attack"].onPress += Attack;
        inputManager.inputs["Enter"].onDown += Enter;
    }

    private void Start() {
        if(targetTile == null) {
            ChangeTarget(caveManager._currentRoom, transform.position.x, transform.position.z);
        }
        AddKeys();
        normalScale = transform.localScale;
    }

    private void Update() {
        if(targetTile == null) FixPos();
        Move();
        MoveHand();
        LookDirection();
        SetFacingTile();
    }
}

public interface IAttacker {
    public void Attack();
}