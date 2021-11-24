using System.Collections;
using UnityEngine;
using DG.Tweening;

// アセット 3rd Person Controller+Fly Mode のカメラスクリプトに機能を追加したもの
public class ThirdPersonOrbitCamBasic : MonoBehaviour
{
    public Transform player;                                           // Player's reference.
    public Transform playerCenter;
    public PlayerStatus playerStatus;
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f);       // Offset to repoint the camera.
    public Vector3 camOffset = new Vector3(0.4f, 0.5f, -2.0f);       // Offset to relocate the camera related to the player position.
    public float smooth = 10f;                                         // Speed of camera responsiveness.
    public float horizontalAimingSpeed = 6f;                           // Horizontal turn speed.
    public float verticalAimingSpeed = 6f;                             // Vertical turn speed.
    public float maxVerticalAngle = 30f;                               // Camera max clamp angle. 
    public float minVerticalAngle = -60f;                              // Camera min clamp angle.
    public string XAxis = "Analog X";                                  // The default horizontal axis input name.
    public string YAxis = "Analog Y";                                  // The default vertical axis input name.

    private float angleH = 0;                                          // Float to store camera horizontal angle related to mouse movement.
    private float angleV = 0;                                          // Float to store camera vertical angle related to mouse movement.
    private Transform cam;                                             // This transform.
    private Vector3 relCameraPos;                                      // Current camera position relative to the player.
    private float relCameraPosMag;                                     // Current camera distance to the player.
    private Vector3 smoothPivotOffset;                                 // Camera current pivot offset on interpolation.
    private Vector3 smoothCamOffset;                                   // Camera current offset on interpolation.
    private Vector3 targetPivotOffset;                                 // Camera pivot offset target to iterpolate.
    private Vector3 targetCamOffset;                                   // Camera offset target to interpolate.
    private float defaultFOV;                                          // Default camera Field of View.
    private float targetFOV;                                           // Target camera Field of View.
    private float targetMaxVerticalAngle;                              // Custom camera max vertical clamp angle.

    [SerializeField, Tooltip("揺れの強さ")] float power = 0.7f;
    [SerializeField, Tooltip("揺れる秒数")] float duration = 1;


    Vector3 startPosition;
    float initialDuration;

    // Get the camera horizontal angle.
    public float GetH { get { return angleH; } }

    void Awake()
    {
        // Reference to the camera transform.
        cam = transform;

        // Set camera default position.
        cam.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        cam.rotation = Quaternion.identity;

        // Get camera position relative to the player, used for collision test.
        relCameraPos = transform.position - player.position;
        relCameraPosMag = relCameraPos.magnitude - 0.5f;

        // Set up references and default values.
        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = cam.GetComponent<Camera>().fieldOfView;
        angleH = player.eulerAngles.y;

        ResetTargetOffsets();
        ResetFOV();
        ResetMaxVerticalAngle();

        initialDuration = duration;
        playerStatus = player.gameObject.GetComponent<PlayerStatus>();
    }
    void LateUpdate()
    {
        //ゲームオーバー時にカメラがプレイヤーを向く
        if (playerStatus.PlayerDead)
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);
            // ターゲット方向のベクトルを取得
            Vector3 relativePos = playerCenter.position + offset - this.transform.position;
            // 方向を、回転情報に変換
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            // 現在の回転情報と、ターゲット方向の回転情報を補完する
            transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime);

            return;
        }
        if (Time.timeScale == 0) return;
        // Get mouse movement to orbit the camera.
        // Mouse:
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * SensitivityControl.sensitivity;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * SensitivityControl.sensitivity;
        // Joystick:
        angleH += Mathf.Clamp(Input.GetAxis(XAxis), -1, 1) * 60 * SensitivityControl.sensitivity * Time.deltaTime;
        angleV += Mathf.Clamp(Input.GetAxis(YAxis), -1, 1) * 60 * SensitivityControl.sensitivity * Time.deltaTime;

        // Set vertical movement limit.
        angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);

        // Set camera orientation.
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
        cam.rotation = aimRotation;

        // Set FOV.
        cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);

        // Test for collision with the environment based on current camera position.
        Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
        Vector3 noCollisionOffset = targetCamOffset;
        for (float zOffset = targetCamOffset.z; zOffset <= 0; zOffset += 0.5f)
        {
            noCollisionOffset.z = zOffset;
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset, Mathf.Abs(zOffset)) || zOffset == 0)
            {
                break;
            }
        }

        // Repostition the camera.
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, noCollisionOffset, smooth * Time.deltaTime);

        cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;

        //プレイヤーがダメージを受けた時にカメラを揺らす
        if (playerStatus.PlayerDamage)
        {
            Shake();
        }
    }


    //カメラを揺らす処理
    void Shake()
    {
        if (duration > 0)
        {
            cam.position += Random.insideUnitSphere * power * duration;
            duration -= Time.deltaTime;
        }
        else
        {
            playerStatus.PlayerDamage = false;
            duration = initialDuration;
        }
    }

    // Set camera offsets to custom values.
    public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
    {
        targetPivotOffset = newPivotOffset;
        targetCamOffset = newCamOffset;
    }

    // Reset camera offsets to default values.
    public void ResetTargetOffsets()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
    }

    // Reset the camera vertical offset.
    public void ResetYCamOffset()
    {
        targetCamOffset.y = camOffset.y;
    }

    // Set camera vertical offset.
    public void SetYCamOffset(float y)
    {
        targetCamOffset.y = y;
    }

    // Set camera horizontal offset.
    public void SetXCamOffset(float x)
    {
        targetCamOffset.x = x;
    }

    // Set custom Field of View.
    public void SetFOV(float customFOV)
    {
        this.targetFOV = customFOV;
    }

    // Reset Field of View to default value.
    public void ResetFOV()
    {
        this.targetFOV = defaultFOV;
    }

    // Set max vertical camera rotation angle.
    public void SetMaxVerticalAngle(float angle)
    {
        this.targetMaxVerticalAngle = angle;
    }

    // Reset max vertical camera rotation angle to default value.
    public void ResetMaxVerticalAngle()
    {
        this.targetMaxVerticalAngle = maxVerticalAngle;
    }

    // Double check for collisions: concave objects doesn't detect hit from outside, so cast in both directions.
    bool DoubleViewingPosCheck(Vector3 checkPos, float offset)
    {
        float playerFocusHeight = player.GetComponent<CapsuleCollider>().height * 0.75f;
        return ViewingPosCheck(checkPos, playerFocusHeight) && ReverseViewingPosCheck(checkPos, playerFocusHeight, offset);
    }

    // Check for collision from camera to player.
    bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight)
    {
        // Cast target.
        Vector3 target = player.position + (Vector3.up * deltaPlayerHeight);
        int layerMaskInt = 1 << 20 | 1 << 21;
        // If a raycast from the check position to the player hits something...
        if (Physics.SphereCast(checkPos, 0.2f, target - checkPos, out RaycastHit hit, relCameraPosMag, layerMaskInt))
        {
            // ... if it is not the player...
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                // This position isn't appropriate.
                return false;
            }
        }
        // If we haven't hit anything or we've hit the player, this is an appropriate position.
        return true;
    }

    // Check for collision from player to camera.
    bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight, float maxDistance)
    {
        // Cast origin.
        Vector3 origin = player.position + (Vector3.up * deltaPlayerHeight);
        int layerMaskInt = 1 << 20 | 1 << 21;
        if (Physics.SphereCast(origin, 0.2f, checkPos - origin, out RaycastHit hit, maxDistance, layerMaskInt))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    // Get camera magnitude.
    public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
    {
        return Mathf.Abs((finalPivotOffset - smoothPivotOffset).magnitude);
    }

}
