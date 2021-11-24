using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorAnimation : MonoBehaviour
{
    [SerializeField, Tooltip("通常時の色"), ColorUsage(false, true)] Color safeColor = default;
    [SerializeField, Tooltip("戦闘時の色"), ColorUsage(false, true)] Color dangerColor = default;
    public float m_Alpha;
    Material m_Material;
    Animator m_Animator;
    private void Awake()
    {
        m_Material = GetComponent<Renderer>().material;
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        Color color = m_Material.color;
        color.a = m_Alpha;
        m_Material.color = color;
    }


    public enum Mode
    {
        Danger,
        Safe,
    }

    public void SetMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Danger:
                m_Material.SetColor("_EmissionColor", dangerColor);
                m_Animator.speed = 1;
                break;
            case Mode.Safe:
                m_Material.SetColor("_EmissionColor", safeColor);
                m_Animator.speed = 0.5f;
                break;
            default:
                break;
        }

    }
}
