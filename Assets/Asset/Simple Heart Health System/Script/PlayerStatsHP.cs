/*
 *  Author: ariel oliveira [o.arielg@gmail.com]
 */

using UnityEngine;

public class PlayerStatsHP : MonoBehaviour
{
    public delegate void OnHealthChangedDelegate();
    public OnHealthChangedDelegate onHealthChangedCallback;

    #region Sigleton
    private static PlayerStatsHP instance;
    public static PlayerStatsHP Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerStatsHP>();
            return instance;
        }
    }
    #endregion

    [SerializeField]
    private float health = default;
    [SerializeField]
    private float maxHealth = default;
    [SerializeField]
    private float maxTotalHealth = default;


    public float Health { get { return health; } }
    public float MaxHealth { get { return maxHealth; } }
    public float MaxTotalHealth { get { return maxTotalHealth; } }

    public void Heal(float health)
    {
        this.health += health;
        ClampHealth();
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        ClampHealth();
    }

    public void AddHealth()
    {
        if (maxHealth < maxTotalHealth)
        {
            maxHealth += 1;
            health = maxHealth;

            if (onHealthChangedCallback != null)
                onHealthChangedCallback.Invoke();
        }
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        if (onHealthChangedCallback != null)
            onHealthChangedCallback.Invoke();
    }
}
