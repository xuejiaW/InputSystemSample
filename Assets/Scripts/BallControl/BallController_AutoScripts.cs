using UnityEngine;

public class BallController_AutoScripts : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10;

    private BallControls m_Controls;

    private Rigidbody m_Rb;
    private Vector2 m_Move;

    private void Awake()
    {
        m_Controls = new BallControls();
        m_Rb = GetComponent<Rigidbody>();

        m_Controls.BallPlayer.Button.performed += _ => OnButton();
        m_Controls.BallPlayer.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        m_Controls.BallPlayer.Move.canceled += _ => OnMove(Vector2.zero);
    }

    private void OnEnable() { m_Controls.BallPlayer.Enable(); }

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

    private void OnDisable() { m_Controls.BallPlayer.Disable(); }
}
