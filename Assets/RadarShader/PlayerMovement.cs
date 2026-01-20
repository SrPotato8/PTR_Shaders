using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float mouseSensitivity = 150f;

    float yRotation = 0f;
    float fixedY;

    void Start()
    {
        fixedY = transform.position.y;
    }
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        transform.position += move * movementSpeed * Time.deltaTime;

        transform.position = new Vector3(
            transform.position.x,
            fixedY,
            transform.position.z
        );
    }
}
