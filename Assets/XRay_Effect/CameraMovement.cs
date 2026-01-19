using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float m_rotationX = 0f;
    float m_rotationY = 0f;

    public float m_sensitivity = 5f;
    public float m_movementSpeed = 10f;

    void Update()
    {
        // For rotation
        m_rotationY += Input.GetAxis("Mouse X") * m_sensitivity;
        m_rotationX += Input.GetAxis("Mouse Y") * -m_sensitivity;

        transform.localEulerAngles = new Vector3(m_rotationX, m_rotationY, 0);

        // For movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * vertical + transform.right * horizontal;
        transform.position += movement * m_movementSpeed * Time.deltaTime;
    }
}
