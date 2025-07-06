using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float destroyAfterSeconds = 2f;

    [Header("Target Tags")]
    public string[] targetTags; // Mảng các tag mục tiêu (Enemy, Boss, Dummy...)

    void Start()
    {
        Destroy(gameObject, destroyAfterSeconds); // Tự hủy sau X giây
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsTargetTag(other.tag))
        {
            Debug.Log("Projectile hit: " + other.name + " with tag: " + other.tag);
            // (Tùy chọn: Gây damage hoặc hiệu ứng ở đây)

            Destroy(gameObject); // Hủy sau khi va chạm
        }
    }

    private bool IsTargetTag(string tag)
    {
        foreach (string target in targetTags)
        {
            if (tag == target) return true;
        }
        return false;
    }
}