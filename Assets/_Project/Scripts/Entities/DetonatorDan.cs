using UnityEngine;

public class DetonatorDan : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float chaseRange = 1.5f;
    [SerializeField] private float detonateRange = 0.5f;
    [SerializeField] private float chaseDirection = 0;
    [SerializeField] private float jumpCooldown = 2;
    [SerializeField] private float speed = 1;
    [SerializeField] private GameObject explotionEffect;
    private float cooldown;

    private void Update()
    {
        CheckForPlayerInRange();
        UpdateAnimator();

        if (chaseDirection != 0)
        {
            transform.position = (Vector2)transform.position + new Vector2(chaseDirection * speed * Time.deltaTime, 0);
        }
    }

    private void CheckForPlayerInRange()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        foreach (GameObject player in players)
        {
            // Chase
            if (chaseDirection == 0)
            {
                if (Vector2.Distance(player.transform.position, transform.position) <= chaseRange)
                {
                    chaseDirection = player.transform.position.x < transform.position.x ? -1 : 1;
                    cooldown = jumpCooldown;
                }
            }

            // Detonate
            if (Vector2.Distance(player.transform.position, transform.position) <= detonateRange)
            {
                Detonate();
            }
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isChasing", !(chaseDirection == 0));
    }

    private void Detonate()
    {
        Instantiate(explotionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detonateRange);
    }
}