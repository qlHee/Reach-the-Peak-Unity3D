using UnityEngine;

/// <summary>
/// 绳子渲染器 - 使用LineRenderer绘制钟摆的绳子
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class RopeRenderer : MonoBehaviour
{
    [Header("绳子设置")]
    [Tooltip("绳子起点（支点）")]
    public Transform startPoint;
    
    [Tooltip("绳子终点（摆球）")]
    public Transform endPoint;
    
    [Tooltip("绳子宽度")]
    [Range(0.01f, 0.5f)]
    public float ropeWidth = 0.05f;
    
    [Tooltip("绳子颜色")]
    public Color ropeColor = new Color(0.4f, 0.3f, 0.2f); // 棕色
    
    [Header("高级设置")]
    [Tooltip("绳子分段数（越多越平滑，但性能消耗越大）")]
    [Range(2, 20)]
    public int segments = 2;
    
    [Tooltip("使用世界空间坐标")]
    public bool useWorldSpace = true;
    
    private LineRenderer lineRenderer;
    
    void Start()
    {
        // 获取LineRenderer组件
        lineRenderer = GetComponent<LineRenderer>();
        
        // 配置LineRenderer
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
        lineRenderer.positionCount = segments;
        lineRenderer.useWorldSpace = useWorldSpace;
        
        // 设置材质和颜色
        if (lineRenderer.material == null)
        {
            // 使用默认材质
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
        lineRenderer.startColor = ropeColor;
        lineRenderer.endColor = ropeColor;
        
        // 检查是否设置了起点和终点
        if (startPoint == null || endPoint == null)
        {
            Debug.LogWarning("RopeRenderer: 未设置起点或终点！");
        }
    }
    
    void Update()
    {
        if (startPoint == null || endPoint == null || lineRenderer == null)
            return;
        
        // 更新绳子的位置
        UpdateRopePositions();
    }
    
    /// <summary>
    /// 更新绳子的位置点
    /// </summary>
    void UpdateRopePositions()
    {
        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;
        
        // 如果只有2个分段，直接连接起点和终点
        if (segments == 2)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }
        else
        {
            // 多段绳子可以添加轻微的曲线效果（可选）
            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1);
                Vector3 position = Vector3.Lerp(start, end, t);
                
                // 可选：添加轻微的下垂效果
                // float sag = Mathf.Sin(t * Mathf.PI) * 0.1f;
                // position.y -= sag;
                
                lineRenderer.SetPosition(i, position);
            }
        }
    }
    
    /// <summary>
    /// 设置绳子颜色
    /// </summary>
    public void SetRopeColor(Color color)
    {
        ropeColor = color;
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
    
    /// <summary>
    /// 设置绳子宽度
    /// </summary>
    public void SetRopeWidth(float width)
    {
        ropeWidth = width;
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }
}

