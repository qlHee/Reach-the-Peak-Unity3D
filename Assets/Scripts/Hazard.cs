using UnityEngine;

/// <summary>
/// 陷阱 - 检测玩家碰撞并触发重生
/// </summary>
public class Hazard : MonoBehaviour
{
    [Header("陷阱设置")]
    [Tooltip("玩家标签（用于识别玩家）")]
    public string playerTag = "Player";
    
    [Header("调试信息")]
    [Tooltip("显示调试日志")]
    public bool showDebugLog = true;
    
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
                // 触发玩家重生
                playerController.Respawn();
            }
            else
            {
                Debug.LogError("Hazard: 玩家对象上未找到PlayerController组件！");
            }
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

