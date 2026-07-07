using UnityEngine;

public class SimpleHeliController : MonoBehaviour
{
    [Header("--- 移動設定 ---")]
    public float flySpeed = 15f;
    public float upSpeed = 8f;
    public float turnSpeed = 50f;

    [Header("--- 音效設定 ---")]
    public AudioSource audioSourceFly;
    public AudioSource audioSourceHit;
    public AudioSource audioSourceSuccess;

    [Header("--- 距離偵測目標 ---")]
    public Transform obstacleTransform;
    public Transform goalTransform;

    private bool isFlying = false;
    private bool hasPlayedHit = false;
    private bool hasPlayedSuccess = false;

    void Start()
    {
        // 確保遊戲一開始飛行音效不會自己亂播，由按 Shift 控制
        if (audioSourceFly != null) audioSourceFly.Stop();
    }

    void FixedUpdate()
    {
        // 1. 上升與起飛 (按 Left Shift)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isFlying)
            {
                isFlying = true;
                if (audioSourceFly != null && !audioSourceFly.isPlaying)
                {
                    audioSourceFly.loop = true;
                    audioSourceFly.Play();
                }
            }
            transform.Translate(Vector3.up * upSpeed * Time.fixedDeltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.Space) && isFlying)
        {
            transform.Translate(Vector3.down * upSpeed * Time.fixedDeltaTime, Space.World);
        }

        if (isFlying)
        {
            // 2. 前進與後退 (W / S)
            float moveForward = Input.GetAxis("Vertical");
            transform.Translate(Vector3.forward * moveForward * flySpeed * Time.fixedDeltaTime, Space.World);

            // 3. 左右旋轉 (A / D)
            float turn = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up * turn * turnSpeed * Time.fixedDeltaTime);

            // 執行距離偵測
            CheckDistances();
        }
    }

    void CheckDistances()
    {
        // 💡 關鍵修正：把原本的 3 公尺放大到 8 公尺！只要靠近就會響！
        if (obstacleTransform != null && !hasPlayedHit)
        {
            float distToObstacle = Vector3.Distance(transform.position, obstacleTransform.position);
            if (distToObstacle <= 8.0f)
            {
                if (audioSourceHit != null) audioSourceHit.Play();
                Debug.Log("【雷達成功】撞擊音效播放！距離：" + distToObstacle);
                hasPlayedHit = true;
            }
        }

        // 💡 關鍵修正：把原本的 4 公尺放大到 8 公尺！
        if (goalTransform != null && !hasPlayedSuccess)
        {
            float distToGoal = Vector3.Distance(transform.position, goalTransform.position);
            if (distToGoal <= 8.0f)
            {
                if (audioSourceSuccess != null) audioSourceSuccess.Play();
                Debug.Log("【雷達成功】成功音效播放！距離：" + distToGoal);
                hasPlayedSuccess = true;
            }
        }
    }

    // 💡 視覺化雷達：在編輯器畫面畫出藍色和紅色的偵測圈圈
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 8.0f);
    }
}