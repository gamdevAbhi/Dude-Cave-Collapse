using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Reference")]
    [SerializeField] private Transform target;
    [SerializeField] private InputManager inputManager;
    [Header("Ghost Properties")]
    [SerializeField] private float ghostSpeed;
    [SerializeField] private float ghostDelay;
    [Header("Camera Properties")]
    [SerializeField] private float cameraRotateSpeed;
    [SerializeField] private float maxXRotation;
    [SerializeField] private float maxYRotation;
    [SerializeField] private float cameraFocusPower = 0.1f;

    public CameraDirection cameraDirection;
    private Ghost ghost;
    private Vector2 originalRotation;
    private float targetY;
    private float minRotateY = 0.15f;
    private bool isRight = false;

    private void FocusCamera() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition -=  GetComponent<Camera>().WorldToScreenPoint(target.transform.position);

        float angleX = cameraFocusPower * mousePosition.y / 100f;
        float angleY = cameraFocusPower * mousePosition.x / 100f;

        if(angleX > maxXRotation) angleX = maxXRotation;
        else if(angleX < -maxXRotation) angleX = -maxXRotation; 
        if(angleY > maxYRotation) angleY = maxYRotation;
        else if(angleY < -maxYRotation) angleY = -maxYRotation;

        transform.localEulerAngles = new Vector3(originalRotation.x - angleX, 
        originalRotation.y + angleY, transform.eulerAngles.z);
    }

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
        originalRotation = new Vector2(transform.localEulerAngles.x, transform.localEulerAngles.y);
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
        FocusCamera();
    }

    [System.Serializable]
    public class CameraDirection {
        public enum Direction {Zero, Right, Left, Opposite};
        public Direction direction;
        public CameraDirection left;
        public CameraDirection right;
        [HideInInspector] public Vector2 upVector;
        [HideInInspector] public Vector2 leftVector;
    }
}
