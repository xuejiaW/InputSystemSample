using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10;
    [SerializeField] private InputActionAsset m_Asset = null;

    private Rigidbody m_Rb;
    private Vector2 m_Move;

    private InputActionMap m_ActionMap = null;
    private InputAction m_ButtonsAction = null;
    private InputAction m_MoveAction = null;

    private void Awake()
    {
        m_ActionMap = m_Asset.FindActionMap("BallPlayer");
        m_ButtonsAction = m_ActionMap.FindAction("Button");
        m_MoveAction = m_ActionMap.FindAction("Move");
        m_ButtonsAction.performed += _ => OnButton();
        m_MoveAction.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        m_MoveAction.canceled += _ => OnMove(Vector2.zero);

        m_Rb = GetComponent<Rigidbody>();
    }

    private void OnEnable() { m_ActionMap.Enable(); }

    private void OnButton() { Debug.Log("On Buttons clicked triggered"); }

    private void OnMove(Vector2 coordinates)
    {
        Debug.Log($"On move clicked triggered {coordinates.ToString("f4")}");
        m_Move = coordinates;
    }

    private void FixedUpdate()
    {
        Vector3 movement = new(m_Move.x, 0.0f, m_Move.y);
        m_Rb.AddForce(movement * m_Speed);
    }

    private void OnDisable() { m_ActionMap.Disable(); }
}