using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CurrentSelected : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem = default;
    [HideInInspector] public GameObject selectedObj;

    private void OnDisable()
    {

    }
    private void Update()
    {
        selectedObj = eventSystem.currentSelectedGameObject.gameObject;
        print(selectedObj);
    }
}
