using UnityEngine;

/// <summary>
/// 旋转障碍物 - 持续旋转的陷阱
/// </summary>
public class RotatingObstacle : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("旋转轴（X, Y, Z）")]
    public Vector3 rotationAxis = new Vector3(0f, 1f, 0f);
    
    [Tooltip("旋转速度（度/秒）")]
    [Range(10f, 360f)]
    public float rotationSpeed = 90f;
    
    void Update()
    {
        // 按照设定的旋转轴和速度持续旋转
        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// 在编辑器中绘制旋转轴方向
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // 绘制旋转轴方向
        Gizmos.color = Color.cyan;
        Vector3 axisDirection = rotationAxis.normalized * 2f;
        Gizmos.DrawLine(transform.position - axisDirection, transform.position + axisDirection);
        
        // 绘制旋转方向指示器
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}

