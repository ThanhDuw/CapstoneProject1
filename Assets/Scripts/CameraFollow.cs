using UnityEngine;

public class CameraFollow : MonoBehaviour
//{
//    public Transform target; // Gán Player (Role_T)
//    public Vector3 offset = new Vector3(1.5f, 2.5f, -4f);
//    public float followSpeed = 10f;

//    void LateUpdate()
//    {
//        if (target == null) return;

//        Vector3 desiredPos = target.position + offset;
//        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
//    }
//}
{
    public Transform target;  // Gán là Player (Role_T)
    public Vector3 offset = Vector3.zero; // Bám sát

    void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;
    }
}