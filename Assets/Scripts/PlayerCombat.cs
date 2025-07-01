using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public int damage = 10;
    public LayerMask enemyLayer;
    public Transform attackPoint; // điểm phát ra đòn (trước mặt)

    public GameObject hitEffectPrefab; // hiệu ứng nếu trúng (tuỳ chọn)

    // Hàm này sẽ được gọi từ Animation Event
    public void MeleeAttackStart()
    {
        Debug.Log("Melee Attack Start!");

        // Tìm enemy trong vùng tấn công
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Hit enemy: " + enemy.name);
            enemy.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

            // Gắn hiệu ứng nếu có
            if (hitEffectPrefab != null)
                Instantiate(hitEffectPrefab, enemy.transform.position, Quaternion.identity);
        }
    }
    public void MeleeAttackEnd()
    {
        Debug.Log("Melee Attack End!");
        // (Tùy chọn) kết thúc frame gây damage
    }

    // Hiển thị vùng đòn đánh trong Scene view để dễ chỉnh
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}