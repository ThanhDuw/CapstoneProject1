using UnityEngine;

public class MoseMovement : MonoBehaviour
//{
//    public float mouseSensitivity = 100f;

//    float xRotation = 0f;
//    float yRotation = 0f;

//    public float topClamp = 90f;
//    public float bottomClamp = 90f;

//    void Start()
//    {
//        Cursor.lockState = CursorLockMode.Locked;
//    }

//    void Update()
//    {
//        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
//        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

//        xRotation -= mouseY;
//        xRotation = Mathf.Clamp(xRotation, -bottomClamp, topClamp);

//        yRotation += mouseX;

//        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
//    }
//}
{
    public Transform target;        // Gán là CameraRig
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 60f;

    float x = 0.0f;
    float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}
