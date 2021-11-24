using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//武器を拾う処理
public class WeaponGather : MonoBehaviour
{

    [SerializeField, Tooltip("Aボタンを何秒押すと武器を拾うか")] private float a_Push = default;

    [SerializeField, Tooltip("Weapon")] BulletChange bulletChange = default;

    [SerializeField, Tooltip("HaveWeapon")] GameObject weaponComponent = default;

    [SerializeField, Tooltip("GatherUI")] RectTransform gatherUI = default;
    float pushTimer = default;
    Rigidbody rb = default;
    ItemRotation rt = default;
    BoxCollider coll;
    MeshRenderer[] meshRenderer;
    MonoBehaviour monoBehaviour;

    TextMeshProUGUI gatherGunText;
    TextMeshProUGUI gatherText;


    Transform child;

    void Start()
    {
        pushTimer = a_Push;
        gatherUI.gameObject.SetActive(false);
        gatherGunText = gatherUI.transform.Find("GatherGunText").GetComponent<TextMeshProUGUI>();

        gatherText = gatherUI.transform.Find("GatherText").GetComponent<TextMeshProUGUI>();
    }


    private void OnTriggerStay(Collider other)
    {
        rb = other.gameObject.GetComponent<Rigidbody>();
        rt = other.gameObject.GetComponent<ItemRotation>();
        coll = other.gameObject.GetComponent<BoxCollider>();
        meshRenderer = other.gameObject.GetComponentsInChildren<MeshRenderer>();
        monoBehaviour = other.gameObject.GetComponent<MonoBehaviour>();


        child = other.transform;

        if (other.gameObject.CompareTag("Gun"))
        {
            Vector3 offset = new Vector3(0, 1.0f, 0);

            gatherUI.gameObject.SetActive(true);
            GameWeaponKind kind = other.gameObject.GetComponent<GameWeaponKind>();
            gatherGunText.text = kind.weaponKind.ToString().Replace("_", " ");
            gatherUI.transform.position = other.transform.position + offset;

            if (CurrentDevice.anyKey)
            {
                gatherText.text = "Fキー 取得";
            }
            else
            {
                gatherText.text = "Aボタン 取得";
            }
            GunPush();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        gatherUI.gameObject.SetActive(false);
    }

    //武器を拾う
    void GunPush()
    {
        if (Input.GetButton("A Button"))
        {
            pushTimer -= Time.deltaTime;
        }
        if (Input.GetButtonUp("A Button"))
        {
            pushTimer = a_Push;
        }
        if (pushTimer < 0)
        {
            AudioManager.Instance.PlaySE(AUDIO.SE_ITEMS, 0.8f);
            pushTimer = a_Push;
            //所持品の子に追加
            child.parent = weaponComponent.transform;
            //コライダーをＯＦＦに
            coll.enabled = false;
            //見た目の非表示
            foreach (MeshRenderer mesh in meshRenderer)
            {
                mesh.enabled = false;
            }
            rt.enabled = false;
            //スクリプトをアクティブに
            monoBehaviour.enabled = true;
            //拾ったらカウントアップ
            bulletChange.getCount++;
            //拾うＵＩの非表示
            gatherUI.gameObject.SetActive(false);


            //スクリプトのみつける場合に使う
            // UnityEditorInternal.ComponentUtility.CopyComponent(monoBehaviour);
            // UnityEditorInternal.ComponentUtility.PasteComponentAsNew(weapons);

        }
    }
}
