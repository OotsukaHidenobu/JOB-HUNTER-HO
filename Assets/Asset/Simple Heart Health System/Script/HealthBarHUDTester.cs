/*
 *  Author: ariel oliveira [o.arielg@gmail.com]
 */

using UnityEngine;

public class HealthBarHUDTester : MonoBehaviour
{
    public void AddHealth()
    {
        PlayerStatsHP.Instance.AddHealth();
    }

    public void Heal(float health)
    {
        PlayerStatsHP.Instance.Heal(health);
    }

    public void Hurt(float dmg)
    {
        PlayerStatsHP.Instance.TakeDamage(dmg);
    }
}
