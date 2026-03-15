using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    [Tooltip("摄像机要跟随的目标对象（例如Player）")]
    public Transform target;
    
    [Header("位置设置")]
    [Tooltip("摄像机相对于目标的偏移量")]
    public Vector3 offset = new Vector3(0f, 6f, -8f);
    
    [Header("平滑设置")]
    [Tooltip("位置跟随的平滑速度（值越大越快，0表示无平滑）")]
    [Range(0f, 20f)]
    public float positionSmoothSpeed = 5f;
    
    [Tooltip("旋转跟随的平滑速度（值越大越快，0表示无平滑）")]
    [Range(0f, 20f)]
    public float rotationSmoothSpeed = 5f;
    
    [Header("跟随选项")]
    [Tooltip("是否跟随目标的旋转")]
    public bool followRotation = false;
    
    void Start()
    {
        // 检查是否设置了跟随目标
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: 未设置跟随目标！请在Inspector中将角色对象拖入Target字段。");
        }
    }
    
    void LateUpdate()
    {
        // 确保目标存在
        if (target == null)
        {
            return;
        }
        
        // 处理位置跟随
        HandlePositionFollow();
        
        // 处理旋转跟随（可选）
        if (followRotation)
        {
            HandleRotationFollow();
        }
    }
    
    /// <summary>
    /// 处理摄像机位置跟随
    /// </summary>
    void HandlePositionFollow()
    {
        // 计算目标位置（目标位置 + 偏移量）
        // 偏移量随着目标旋转，实现摄像机以Player为中心旋转
        Vector3 targetPosition = target.position + target.TransformDirection(offset);
        
        // 使用Lerp插值实现平滑移动
        if (positionSmoothSpeed > 0f)
        {
            // 平滑移动到目标位置
            transform.position = Vector3.Lerp(
                transform.position, 
                targetPosition, 
                positionSmoothSpeed * Time.deltaTime
            );
        }
        else
        {
            // 如果平滑速度为0，直接设置位置（无平滑）
            transform.position = targetPosition;
        }
        
        // 摄像机始终看向Player
        transform.LookAt(target.position);
    }
    
    /// <summary>
    /// 处理摄像机旋转跟随（可选功能）
    /// </summary>
    void HandleRotationFollow()
    {
        // 使用Slerp插值实现平滑旋转
        if (rotationSmoothSpeed > 0f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                target.rotation, 
                rotationSmoothSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.rotation = target.rotation;
        }
    }
    
    /// <summary>
    /// 在编辑器Scene视图中绘制调试信息
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (target == null)
        {
            return;
        }
        
        // 绘制目标到摄像机的连线
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(target.position, transform.position);
        
        // 绘制目标位置
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position, 0.5f);
        
        // 绘制摄像机位置
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        
        // 绘制偏移向量（从目标指向摄像机）
        Gizmos.color = Color.red;
        Vector3 offsetDirection = followRotation 
            ? target.TransformDirection(offset) 
            : offset;
        Gizmos.DrawLine(target.position, target.position + offsetDirection);
    }
}




