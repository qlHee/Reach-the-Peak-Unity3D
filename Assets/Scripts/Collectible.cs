using UnityEngine;

/// <summary>
/// 收集物脚本 - 处理玩家拾取金币的逻辑
/// </summary>
public class Collectible : MonoBehaviour
{
    [Header("音效设置")]
    [Tooltip("拾取音效（可选）")]
    public AudioClip pickupSound;
    
    [Header("特效设置")]
    [Tooltip("拾取特效（可选）")]
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        // 检测碰撞对象是否为玩家
        if (other.CompareTag("Player"))
        {
            // 播放拾取音效
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            
            // 播放拾取特效
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }
            
            // 通知游戏管理器增加收集计数
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddCoin();
            }
            else
            {
                Debug.LogWarning("未找到GameManager！请确保场景中存在GameManager对象。");
            }
            
            // 销毁收集物对象
            Destroy(gameObject);
        }
    }
}

