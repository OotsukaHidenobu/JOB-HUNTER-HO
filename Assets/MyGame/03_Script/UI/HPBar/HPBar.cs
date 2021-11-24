using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPBar : MonoBehaviour
{

    [SerializeField] private Image GreenGauge = default;

    [SerializeField] private Image RedGauge = default;

    private BossState enemy;
    private Tween redGaugeTween;
    public void GaugeReduction(float reducationValue, float time = 1f)
    {
        var valueFrom = enemy.hp / enemy.maxHP;
        var valueTo = (enemy.hp - reducationValue) / enemy.maxHP;

        // 緑ゲージ減少
        GreenGauge.fillAmount = valueTo;

        if (redGaugeTween != null)
        {
            redGaugeTween.Kill();
        }

        // 赤ゲージ減少
        redGaugeTween = DOTween.To(
            () => valueFrom,
            x =>
            {
                RedGauge.fillAmount = x;
            },
            valueTo,
            time
        );
    }

    public void SetEnemy(BossState enemy)
    {
        this.enemy = enemy;
    }
}
