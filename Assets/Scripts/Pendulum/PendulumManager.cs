using UnityEngine;

/// <summary>
/// 钟摆管理器 - 统一管理钟摆的位置和配置
/// 挂载到PendulumPivot（支点）上
/// </summary>
[ExecuteInEditMode] // 允许在编辑器模式下运行
public class PendulumManager : MonoBehaviour
{
    [Header("钟摆结构")]
    [Tooltip("摆球对象（自动查找子对象）")]
    public Transform pendulumBall;
    
    [Tooltip("绳子对象（自动查找子对象）")]
    public Transform pendulumRope;
    
    [Header("绳子设置")]
    [Tooltip("绳子长度")]
    [Range(1f, 10f)]
    public float ropeLength = 3f;
    
    [Tooltip("摆球大小")]
    [Range(0.2f, 2f)]
    public float ballScale = 0.5f;
    
    [Header("自动更新")]
    [Tooltip("编辑器模式下实时更新位置")]
    public bool autoUpdateInEditor = true;
    
    private float lastRopeLength;
    private float lastBallScale;
    private Vector3 lastPivotPosition;
    
    void Start()
    {
        // 运行时自动查找子对象
        if (Application.isPlaying)
        {
            FindChildObjects();
            UpdatePendulumLayout();
        }
    }
    
    void OnValidate()
    {
        // 在Inspector中修改参数时自动更新
        if (autoUpdateInEditor)
        {
            FindChildObjects();
            UpdatePendulumLayout();
        }
    }
    
    void Update()
    {
        // 编辑器模式下检测参数变化
        if (!Application.isPlaying && autoUpdateInEditor)
        {
            bool needsUpdate = false;
            
            // 检测参数变化
            if (Mathf.Abs(lastRopeLength - ropeLength) > 0.01f)
            {
                needsUpdate = true;
                lastRopeLength = ropeLength;
            }
            
            if (Mathf.Abs(lastBallScale - ballScale) > 0.01f)
            {
                needsUpdate = true;
                lastBallScale = ballScale;
            }
            
            if (Vector3.Distance(lastPivotPosition, transform.position) > 0.01f)
            {
                needsUpdate = true;
                lastPivotPosition = transform.position;
            }
            
            if (needsUpdate)
            {
                UpdatePendulumLayout();
            }
        }
    }
    
    /// <summary>
    /// 查找子对象
    /// </summary>
    void FindChildObjects()
    {
        if (pendulumBall == null)
        {
            pendulumBall = transform.Find("PendulumBall");
        }
        
        if (pendulumRope == null)
        {
            pendulumRope = transform.Find("PendulumRope");
        }
    }
    
    /// <summary>
    /// 更新钟摆布局
    /// </summary>
    [ContextMenu("更新钟摆布局")]
    public void UpdatePendulumLayout()
    {
        FindChildObjects();
        
        // 更新摆球位置
        if (pendulumBall != null)
        {
            // 设置本地位置（相对于支点）
            pendulumBall.localPosition = new Vector3(0, -ropeLength, 0);
            
            // 设置摆球大小
            pendulumBall.localScale = Vector3.one * ballScale;
            
            // 更新AutoPendulumSetup的配置（如果有）
            AutoPendulumSetup autoSetup = pendulumBall.GetComponent<AutoPendulumSetup>();
            if (autoSetup != null && !Application.isPlaying)
            {
                // 在编辑器模式下更新引用
                autoSetup.pivotTransform = transform;
            }
        }
        
        // 更新绳子渲染器的引用
        if (pendulumRope != null)
        {
            RopeRenderer ropeRenderer = pendulumRope.GetComponent<RopeRenderer>();
            if (ropeRenderer != null)
            {
                ropeRenderer.startPoint = transform;
                ropeRenderer.endPoint = pendulumBall;
            }
            
            // 更新绳子碰撞器（如果有）
            CapsuleCollider ropeCollider = pendulumRope.GetComponent<CapsuleCollider>();
            if (ropeCollider != null)
            {
                ropeCollider.height = ropeLength;
                ropeCollider.center = new Vector3(0, -ropeLength * 0.5f, 0);
            }
        }
        
        Debug.Log($"✅ 钟摆布局已更新：绳长={ropeLength:F2}, 球大小={ballScale:F2}");
    }
    
    /// <summary>
    /// 创建完整的钟摆系统
    /// </summary>
    [ContextMenu("创建完整钟摆系统")]
    public void CreateCompletePendulum()
    {
        Debug.Log("===== 开始创建钟摆系统 =====");
        
        // 1. 创建或查找摆球
        if (pendulumBall == null)
        {
            GameObject ballObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ballObj.name = "PendulumBall";
            ballObj.transform.SetParent(transform);
            pendulumBall = ballObj.transform;
            Debug.Log("✅ 创建了摆球对象");
        }
        
        // 2. 创建或查找绳子
        if (pendulumRope == null)
        {
            GameObject ropeObj = new GameObject("PendulumRope");
            ropeObj.transform.SetParent(transform);
            pendulumRope = ropeObj.transform;
            
            // 添加LineRenderer
            LineRenderer lr = ropeObj.AddComponent<LineRenderer>();
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = new Color(0.4f, 0.3f, 0.2f);
            lr.endColor = new Color(0.4f, 0.3f, 0.2f);
            
            // 添加RopeRenderer
            ropeObj.AddComponent<RopeRenderer>();
            
            // 添加CapsuleCollider
            CapsuleCollider col = ropeObj.AddComponent<CapsuleCollider>();
            col.direction = 1; // Y轴
            col.radius = 0.05f;
            col.isTrigger = true;
            
            // 添加Hazard
            ropeObj.AddComponent<Hazard>();
            
            Debug.Log("✅ 创建了绳子对象及组件");
        }
        
        // 3. 配置支点Rigidbody
        Rigidbody pivotRb = GetComponent<Rigidbody>();
        if (pivotRb == null)
        {
            pivotRb = gameObject.AddComponent<Rigidbody>();
        }
        pivotRb.isKinematic = true;
        pivotRb.useGravity = false;
        
        // 4. 配置摆球组件
        // Rigidbody
        Rigidbody ballRb = pendulumBall.GetComponent<Rigidbody>();
        if (ballRb == null)
        {
            ballRb = pendulumBall.gameObject.AddComponent<Rigidbody>();
        }
        
        // Collider设置为Trigger
        SphereCollider ballCol = pendulumBall.GetComponent<SphereCollider>();
        if (ballCol != null)
        {
            ballCol.isTrigger = true;
        }
        
        // AutoPendulumSetup
        AutoPendulumSetup autoSetup = pendulumBall.GetComponent<AutoPendulumSetup>();
        if (autoSetup == null)
        {
            autoSetup = pendulumBall.gameObject.AddComponent<AutoPendulumSetup>();
        }
        autoSetup.pivotTransform = transform;
        
        // Hazard
        Hazard hazard = pendulumBall.GetComponent<Hazard>();
        if (hazard == null)
        {
            pendulumBall.gameObject.AddComponent<Hazard>();
        }
        
        Debug.Log("✅ 组件配置完成");
        
        // 5. 更新布局
        UpdatePendulumLayout();
        
        Debug.Log("===== 钟摆系统创建完成 =====");
    }
    
    /// <summary>
    /// 在编辑器中绘制钟摆范围
    /// </summary>
    void OnDrawGizmos()
    {
        // 绘制支点
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.15f);
        
        // 绘制绳子方向
        if (pendulumBall != null || ropeLength > 0)
        {
            Vector3 ballPos = pendulumBall != null 
                ? pendulumBall.position 
                : transform.position + Vector3.down * ropeLength;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, ballPos);
            
            // 绘制摆动范围（大约±70度）
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            float angle = 70f * Mathf.Deg2Rad;
            Vector3 leftLimit = transform.position + new Vector3(-Mathf.Sin(angle) * ropeLength, -Mathf.Cos(angle) * ropeLength, 0);
            Vector3 rightLimit = transform.position + new Vector3(Mathf.Sin(angle) * ropeLength, -Mathf.Cos(angle) * ropeLength, 0);
            
            Gizmos.DrawLine(transform.position, leftLimit);
            Gizmos.DrawLine(transform.position, rightLimit);
        }
    }
}

