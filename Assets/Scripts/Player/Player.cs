using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHealth = 8;
    [SerializeField] private float invincibleDuration = 0.5f;

    [SerializeField] private int currentHealth;

    private float invincibleEndTime;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public event Action<Player> HealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        HealthChanged?.Invoke(this);
    }

    private void OnEnable()
    {
        GlobalEvents.RayHitPlayer += OnRayHitPlayer;
    }

    private void OnDisable()
    {
        GlobalEvents.RayHitPlayer -= OnRayHitPlayer;
    }

    private void OnRayHitPlayer(Transform rayTransform, Transform playerTransform)
    {
        if (playerTransform != transform)
            return;

        if (isDead)
            return;

        if (Time.time < invincibleEndTime)
            return;

        invincibleEndTime = Time.time + invincibleDuration;
        TakeDamage(1);
    }

    private void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        HealthChanged?.Invoke(this);
        Debug.Log($"我被击中了，剩余血量：{currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        Debug.Log("玩家死亡");
    }
}
