using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float speed;
    [SerializeField] private Transform handPivot;
    [SerializeField] private CaveCreator caveCreator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CameraController cameraController;
    
    private float minDistance = 0.25f;
    private Vector3 normalScale;
    private Transform targetTile;

    private void Move() {
        Vector3 targetPos = targetTile.position;
        targetPos.y = transform.position.y;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void ChangeTarget(Room currentRoom, float x, float z) {
        Vector2 pos = new Vector2(Mathf.Min(currentRoom.grid.GetLength(0) - 1, x), 
        Mathf.Min(currentRoom.grid.GetLength(1) - 1, z));
        if(pos.x < 0) pos.x = 0;
        if(pos.y < 0) pos.y = 0;

        if(currentRoom.grid[(int)pos.x, (int)pos.y].hex
        != caveCreator.hexItem["Water"]) {
            targetTile = currentRoom.grid[(int)pos.x, (int)pos.y].transform;
        }
    }

    private void FixPos() { // not need just for testing
        Room currentRoom = caveCreator._currentRoom;

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
        Vector3 distance = targetTile.position;
        distance.y = transform.position.y;
        distance = distance - transform.position;

        return new Tuple<Vector2, Vector3>(currentPos, distance);
    }

    private void MoveUp() {
        Room currentRoom = caveCreator._currentRoom;
        Tuple<Vector2, Vector3> vectors = GetDistance();
        Vector2 pos = vectors.Item1 + cameraController.cameraDirection.upVector;

        if(vectors.Item2.magnitude <= minDistance) ChangeTarget(currentRoom, pos.x, pos.y);
    }

    private void MoveDown() {
        Room currentRoom = caveCreator._currentRoom;
        Tuple<Vector2, Vector3> vectors = GetDistance();
        Vector2 pos = vectors.Item1 - cameraController.cameraDirection.upVector;

        if(vectors.Item2.magnitude <= minDistance) ChangeTarget(currentRoom, pos.x, pos.y);
    }

    private void MoveForward() {
        Room currentRoom = caveCreator._currentRoom;
        Tuple<Vector2, Vector3> vectors = GetDistance();
        Vector2 pos = vectors.Item1 + cameraController.cameraDirection.leftVector;

        if(vectors.Item2.magnitude <= minDistance) ChangeTarget(currentRoom, pos.x, pos.y);
    }

    private void MoveBackward() {
        Room currentRoom = caveCreator._currentRoom;
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

    private void AddKeys() {
        inputManager.inputs["Up"].onPress += MoveUp;
        inputManager.inputs["Down"].onPress += MoveDown;
        inputManager.inputs["Foward"].onPress += MoveForward;
        inputManager.inputs["Backward"].onPress += MoveBackward;
    }

    private void Start() {
        if(targetTile == null) {
            ChangeTarget(caveCreator._currentRoom, transform.position.x, transform.position.z);
        }
        FixPos();
        AddKeys();
        normalScale = transform.localScale;
    }

    private void Update() {
        if(targetTile == null) FixPos();
        Move();
        MoveHand();
        LookDirection();
    }
}
