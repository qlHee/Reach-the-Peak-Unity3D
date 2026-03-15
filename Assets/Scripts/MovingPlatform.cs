using UnityEngine;

/// <summary>
/// 移动平台 - 在两点之间往返移动的陷阱/平台
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("起始位置（相对于物体初始位置的偏移）")]
    public Vector3 startOffset = Vector3.zero;
    
    [Tooltip("结束位置（相对于物体初始位置的偏移）")]
    public Vector3 endOffset = new Vector3(5f, 0f, 0f);
    
    [Tooltip("移动速度")]
    [Range(0.5f, 10f)]
    public float moveSpeed = 2f;
    
    [Header("移动模式")]
    [Tooltip("使用平滑移动（Lerp）而不是线性移动")]
    public bool useSmoothMovement = true;
    
    // 私有变量
    private Vector3 initialPosition;
    private Vector3 pointA;
    private Vector3 pointB;
    private bool movingToB = true;
    private float journeyProgress = 0f;
    
    void Start()
    {
        // 记录初始位置
        initialPosition = transform.position;
        
        // 计算两个目标点的世界坐标
        pointA = initialPosition + startOffset;
        pointB = initialPosition + endOffset;
        
        // 将平台设置到起始位置
        transform.position = pointA;
    }
    
    void Update()
    {
        if (useSmoothMovement)
        {
            MoveSmoothly();
        }
        else
        {
            MoveLinearly();
        }
    }
    
    /// <summary>
    /// 平滑移动（使用Lerp）
    /// </summary>
    void MoveSmoothly()
    {
        // 计算移动进度
        journeyProgress += moveSpeed * Time.deltaTime / Vector3.Distance(pointA, pointB);
        
        if (movingToB)
        {
            // 从A移动到B
            transform.position = Vector3.Lerp(pointA, pointB, journeyProgress);
            
            // 到达B点，切换方向
            if (journeyProgress >= 1f)
            {
                journeyProgress = 0f;
                movingToB = false;
            }
        }
        else
        {
            // 从B移动到A
            transform.position = Vector3.Lerp(pointB, pointA, journeyProgress);
            
            // 到达A点，切换方向
            if (journeyProgress >= 1f)
            {
                journeyProgress = 0f;
                movingToB = true;
            }
        }
    }
    
    /// <summary>
    /// 线性移动（使用MoveTowards）
    /// </summary>
    void MoveLinearly()
    {
        Vector3 targetPosition = movingToB ? pointB : pointA;
        
        // 向目标点移动
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
        
        // 检查是否到达目标点
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingToB = !movingToB;
        }
    }
    
    /// <summary>
    /// 在编辑器中绘制移动路径
    /// </summary>
    void OnDrawGizmos()
    {
        // 计算两个目标点
        Vector3 basePosition = Application.isPlaying ? initialPosition : transform.position;
        Vector3 gizmoPointA = basePosition + startOffset;
        Vector3 gizmoPointB = basePosition + endOffset;
        
        // 绘制起点和终点
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gizmoPointA, 0.3f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoPointB, 0.3f);
        
        // 绘制移动路径
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gizmoPointA, gizmoPointB);
    }
}

