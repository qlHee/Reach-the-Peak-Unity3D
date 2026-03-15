using UnityEngine;

/// <summary>
/// 收集物脚本 - 处理玩家拾取金币的逻辑
/// </summary>
public class Collectible : MonoBehaviour
{
    [Header("音效设置")]
    [Tooltip("拾取音效（可选）")]
    public AudioClip pickupSound;
    
    [Range(0f, 1f)]
    [Tooltip("音效音量 (0-1)")]
    public float soundVolume = 0.8f;
    
    [Header("特效设置")]
    [Tooltip("拾取特效（可选）")]
    public GameObject pickupEffect;
    
    [Tooltip("特效自动销毁时间（秒）")]
    public float effectDestroyTime = 2f;

    private void OnTriggerEnter(Collider other)
    {
        // 检测碰撞对象是否为玩家
        if (other.CompareTag("Player"))
        {
            // 播放拾取音效
            PlayPickupSound();
            
            // 播放拾取特效
            PlayPickupEffect();
            
            // 通知游戏管理器增加收集计数
            NotifyGameManager();
            
            // 销毁收集物对象
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 播放拾取音效
    /// </summary>
    private void PlayPickupSound()
    {
        // 检查场景中是否有AudioListener
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("场景中没有AudioListener！请在主摄像机上添加AudioListener组件。");
            return;
        }
        
        if (pickupSound != null)
        {
            // 创建临时音频源来播放音效（支持音量控制）
            GameObject audioObject = new GameObject("PickupSound");
            audioObject.transform.position = transform.position;
            
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = pickupSound;
            audioSource.volume = soundVolume;
            audioSource.spatialBlend = 0f; // 2D音效
            audioSource.playOnAwake = false;
            audioSource.Play();
            
            // 音效播放完毕后销毁临时对象
            Destroy(audioObject, pickupSound.length);
            
            Debug.Log($"✓ 播放金币拾取音效：{pickupSound.name}，音量：{soundVolume}");
        }
        else
        {
            Debug.LogWarning($"⚠ 金币 '{gameObject.name}' 未设置拾取音效！请在Inspector中指定音效文件。");
            Debug.LogWarning("→ 解决方法：将音频文件拖入 Collectible 组件的 'Pickup Sound' 字段");
        }
    }
    
    /// <summary>
    /// 播放拾取特效
    /// </summary>
    private void PlayPickupEffect()
    {
        if (pickupEffect != null)
        {
            // 在拾取位置生成特效（稍微抬高一点，更明显）
            Vector3 effectPosition = transform.position + Vector3.up * 0.5f;
            GameObject effect = Instantiate(pickupEffect, effectPosition, Quaternion.identity);
            
            // 自动销毁特效对象，避免内存泄漏
            Destroy(effect, effectDestroyTime);
            
            Debug.Log($"✓ 播放金币拾取特效：{pickupEffect.name}，位置：{effectPosition}");
        }
        else
        {
            Debug.LogWarning($"⚠ 金币 '{gameObject.name}' 未设置拾取特效！请在Inspector中指定特效预制体。");
            Debug.LogWarning("→ 解决方法：将特效预制体拖入 Collectible 组件的 'Pickup Effect' 字段");
        }
    }
    
    /// <summary>
    /// 通知游戏管理器
    /// </summary>
    private void NotifyGameManager()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddCoin();
        }
        else
        {
            Debug.LogWarning("未找到GameManager！请确保场景中存在GameManager对象。");
        }
    }
}

