using UnityEngine;

/// <summary>
/// 陷阱 - 检测玩家碰撞并触发重生
/// </summary>
public class Hazard : MonoBehaviour
{
    [Header("陷阱设置")]
    [Tooltip("玩家标签（用于识别玩家）")]
    public string playerTag = "Player";
    
    [Header("音效设置")]
    [Tooltip("陷阱触发音效")]
    public AudioClip hazardSound;
    
    [Range(0f, 1f)]
    [Tooltip("音效音量")]
    public float soundVolume = 0.8f;
    
    [Header("特效设置")]
    [Tooltip("陷阱触发特效")]
    public GameObject hazardEffectPrefab;
    
    [Tooltip("特效自动销毁时间（秒）")]
    public float effectDestroyTime = 2f;
    
    [Header("调试信息")]
    [Tooltip("显示调试日志")]
    public bool showDebugLog = true;
    
    [Header("UI设置")]
    [Tooltip("UI管理器引用（可选，不设置将自动查找场景中的UIManager）")]
    public UIManager uiManager;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        
        // 获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D音效
        }
    }
    
    /// <summary>
    /// 当触发器被进入时调用
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // 检查碰撞对象是否为玩家
        if (other.CompareTag(playerTag))
        {
            if (showDebugLog)
            {
                Debug.Log($"玩家触碰到陷阱：{gameObject.name}");
            }
            
            // 获取玩家的PlayerController组件
            PlayerController playerController = other.GetComponent<PlayerController>();
            
            if (playerController != null)
            {
                // 播放陷阱音效
                PlayHazardSound();
                
                // 播放陷阱特效
                PlayHazardEffect(other.transform.position);
                
                // 触发玩家重生
                playerController.Respawn();
                
                 // 显示陷阱提示
                if (uiManager != null)
                {
                    uiManager.ShowTrapHint("You're in a trap!");
                }
            }
            else
            {
                Debug.LogError("Hazard: 玩家对象上未找到PlayerController组件！");
            }
        }
    }
    
    /// <summary>
    /// 播放陷阱音效
    /// </summary>
    private void PlayHazardSound()
    {
        if (audioSource != null && hazardSound != null)
        {
            audioSource.PlayOneShot(hazardSound, soundVolume);
        }
    }
    
    /// <summary>
    /// 播放陷阱特效
    /// </summary>
    private void PlayHazardEffect(Vector3 position)
    {
        if (hazardEffectPrefab != null)
        {
            GameObject effect = Instantiate(hazardEffectPrefab, position, Quaternion.identity);
            Destroy(effect, effectDestroyTime);
        }
    }
    
    /// <summary>
    /// 在编辑器中绘制陷阱范围
    /// </summary>
    void OnDrawGizmos()
    {
        // 获取碰撞器组件
        Collider col = GetComponent<Collider>();
        
        if (col != null)
        {
            // 绘制陷阱范围（红色表示危险区域）
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            
            // 根据碰撞器类型绘制不同形状
            if (col is BoxCollider)
            {
                BoxCollider boxCol = col as BoxCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphereCol = col as SphereCollider;
                Gizmos.DrawSphere(transform.position + sphereCol.center, sphereCol.radius);
            }
            else if (col is CapsuleCollider)
            {
                CapsuleCollider capsuleCol = col as CapsuleCollider;
                Gizmos.DrawWireSphere(transform.position + capsuleCol.center, capsuleCol.radius);
            }
        }
    }
}

