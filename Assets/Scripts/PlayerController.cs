using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody m_Rb;
    private float m_MovementX;
    private float m_MovementY;
    [SerializeField] private float m_Speed = 5;

    private void Start() { m_Rb = GetComponent<Rigidbody>(); }

    private void OnMove(InputValue value)
    {
        var inputVector = value.Get<Vector2>();
        m_MovementX = inputVector.x;
        m_MovementY = inputVector.y;
    }

    private void FixedUpdate()
    {
        Vector3 movement = new(m_MovementX, 0.0f, m_MovementY);
        m_Rb.AddForce(movement * m_Speed);
    }
}