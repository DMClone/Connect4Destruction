using UnityEngine;
using UnityEngine.InputSystem;

public class TargetAiming : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject targetIndicator;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private BoardManager boardManager;

    [Header("Settings")]
    [SerializeField] private float indicatorMoveSpeed;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        targetIndicator.SetActive(true);

        InputAction shootAction = inputActions.FindAction("Shoot");
        shootAction.performed += ctx => Shoot();
        shootAction.Enable();
    }

    private void OnDisable()
    {
        targetIndicator.SetActive(false);

        InputAction shootAction = inputActions.FindAction("Shoot");
        shootAction.performed -= ctx => Shoot();
        shootAction.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 input = inputActions.FindAction("MoveTarget").ReadValue<Vector2>();
        Vector3 newPosition = targetIndicator.transform.position + new Vector3(input.x, input.y, 0f) * indicatorMoveSpeed * Time.deltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        targetIndicator.transform.position = newPosition;
    }

    private void Shoot()
    {
        Debug.Log("Shoot action performed.");
        if (Physics.Raycast(targetIndicator.transform.position, Vector3.forward, out RaycastHit hitInfo))
        {
            Debug.Log("Raycast hit: " + hitInfo.collider.name);
            Piece piece = hitInfo.collider.GetComponent<Piece>();
            if (piece != null)
            {
                piece.PieceShot();
                targetIndicator.SetActive(false);
            }
        }
    }
}
