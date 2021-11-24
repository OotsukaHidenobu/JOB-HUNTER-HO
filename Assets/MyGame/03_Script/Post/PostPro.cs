using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostPro : MonoBehaviour
{
    PostProcessVolume m_Volume;
    DepthOfField m_DepthOfField;
    void Start()
    {
        m_DepthOfField = ScriptableObject.CreateInstance<DepthOfField>();

        m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, m_DepthOfField);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            m_DepthOfField.enabled.Override(false);
            m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, m_DepthOfField);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            m_DepthOfField.enabled.Override(true);
            m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, m_DepthOfField);
        }
    }
}
