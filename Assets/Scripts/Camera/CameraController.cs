using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float ghostSpeed;
    [SerializeField] private float ghostDelay;
    [SerializeField] private float cameraRotateSpeed;
    [SerializeField] private InputManager inputManager;

    public CameraDirection cameraDirection;
    private Ghost ghost;
    private float targetY;
    private bool isRight = false;
    private float minRotateY = 0.15f;

    private void RotateCamera() {
        if(targetY == 0) return;

        float nextY = targetY * cameraRotateSpeed * Time.deltaTime;

        if(nextY < minRotateY) nextY = minRotateY;
        if(targetY - nextY < 0f) nextY = targetY;

        targetY -= nextY;
        if(isRight) nextY = -nextY;

        ghost.transform.eulerAngles = new Vector3(0f, ghost.transform.eulerAngles.y + nextY, 0f);
    }

    private void CameraLeft() {
        if(isRight == false) targetY += 90f;
        else targetY = 90f - targetY;
        isRight = false;
        cameraDirection = cameraDirection.left;
    }

    private void CameraRight() {
        if(isRight == true) targetY += 90f;
        else targetY = 90f - targetY;
        isRight = true;
        cameraDirection = cameraDirection.right;
    }
    
    private void AddKeys() {
        inputManager.inputs["Camera Left"].onDown += CameraLeft;
        inputManager.inputs["Camera Right"].onDown += CameraRight;
    }

    private void CreateDirection() {
        CameraDirection zero = new CameraDirection();
        CameraDirection left = new CameraDirection();
        CameraDirection right = new CameraDirection();
        CameraDirection opposite = new CameraDirection();

        zero.direction = CameraDirection.Direction.Zero;
        zero.leftVector = new Vector2(1f,0f);
        zero.upVector = new Vector2(0f,1f);
        left.direction = CameraDirection.Direction.Left;
        left.leftVector = new Vector2(0f,-1f);
        left.upVector = new Vector2(1f,0f);
        right.direction = CameraDirection.Direction.Right;
        right.leftVector = new Vector2(0f,1f);
        right.upVector = new Vector2(-1f,0f);
        opposite.direction = CameraDirection.Direction.Opposite;
        opposite.leftVector = new Vector2(-1f,0f);
        opposite.upVector = new Vector2(0f,-1f);

        zero.left = left;
        zero.right = right;
        left.left = opposite;
        left.right = zero;
        right.left = zero;
        right.right = opposite;
        opposite.right = left;
        opposite.left = right;

        cameraDirection = zero;
    }

    private void Awake() {
        GameObject obj = new GameObject("Player Follower");
        obj.AddComponent<Ghost>();
        ghost = obj.GetComponent<Ghost>();
        transform.parent = ghost.transform;
        ghost.SetTarget(target, ghostSpeed, ghostDelay);
        targetY = 0f;

        CreateDirection();
    }

    private void Start() {
        AddKeys();
    }

    private void Update() {
        RotateCamera();
    }

    [System.Serializable]
    public class CameraDirection {
        public enum Direction {Zero, Right, Left, Opposite};
        public Direction direction;
        public CameraDirection left;
        public CameraDirection right;
        public Vector2 upVector;
        public Vector2 leftVector;
    }
}
