using UnityEngine;

/// <summary>
/// 自动钟摆设置 - 一键修复所有配置问题
/// </summary>
public class AutoPendulumSetup : MonoBehaviour
{
    [Header("支点设置")]
    [Tooltip("支点对象（父对象）- 会自动设置")]
    public Transform pivotTransform;
    
    [Header("启动力设置")]
    [Tooltip("启动推力")]
    [Range(10f, 200f)]
    public float startForce = 100f;
    
    [Header("摆动设置")]
    [Tooltip("初始摆动角度")]
    [Range(10f, 60f)]
    public float initialAngle = 30f;
    
    [Tooltip("自动在Start时配置")]
    public bool autoSetupOnStart = true;
    
    private Rigidbody rb;
    private HingeJoint hinge;
    
    void Start()
    {
        // 自动查找父对象作为支点
        if (pivotTransform == null)
        {
            pivotTransform = transform.parent;
        }
        
        if (autoSetupOnStart)
        {
            SetupPendulum();
            Invoke("ApplyStartForce", 0.2f);
        }
    }
    
    /// <summary>
    /// 配置钟摆（也可手动调用）
    /// </summary>
    [ContextMenu("手动配置钟摆")]
    public void SetupPendulum()
    {
        Debug.Log("===== 开始自动配置钟摆 =====");
        
        // 1. 确定支点
        if (pivotTransform == null)
        {
            pivotTransform = transform.parent;
            if (pivotTransform == null)
            {
                Debug.LogError("❌ 未找到支点！请将此脚本挂在摆球上，并确保摆球有父对象。");
                return;
            }
        }
        
        Debug.Log($"✅ 支点: {pivotTransform.name}");
        
        // 2. 配置或检查支点的Rigidbody
        Rigidbody pivotRb = pivotTransform.GetComponent<Rigidbody>();
        if (pivotRb == null)
        {
            pivotRb = pivotTransform.gameObject.AddComponent<Rigidbody>();
            Debug.Log("✅ 为支点添加了Rigidbody");
        }
        pivotRb.isKinematic = true;
        pivotRb.useGravity = false;
        
        // 3. 配置摆球的Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("✅ 为摆球添加了Rigidbody");
        }
        
        rb.mass = 10f;
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.None;
        
        Debug.Log("✅ Rigidbody配置完成");
        
        // 4. 配置HingeJoint
        hinge = GetComponent<HingeJoint>();
        if (hinge == null)
        {
            hinge = gameObject.AddComponent<HingeJoint>();
            Debug.Log("✅ 为摆球添加了HingeJoint");
        }
        
        // 计算到支点的向量
        Vector3 toPivot = pivotTransform.position - transform.position;
        float ropeLength = toPivot.magnitude;
        Vector3 localToPivot = transform.InverseTransformDirection(toPivot);
        
        Debug.Log($"   绳子长度: {ropeLength:F2}");
        Debug.Log($"   到支点的本地方向: {localToPivot}");
        
        // 设置HingeJoint
        hinge.connectedBody = pivotRb;
        hinge.anchor = localToPivot; // 关键：设置正确的锚点
        hinge.axis = new Vector3(0, 0, 1); // 绕Z轴旋转
        hinge.autoConfigureConnectedAnchor = true;
        
        // 设置限制
        hinge.useLimits = true;
        JointLimits limits = new JointLimits();
        limits.min = -75f;
        limits.max = 75f;
        limits.bounciness = 0f;
        hinge.limits = limits;
        
        hinge.useSpring = false;
        hinge.useMotor = false;
        
        Debug.Log("✅ HingeJoint配置完成");
        Debug.Log($"   Anchor: {hinge.anchor}");
        Debug.Log($"   Connected Anchor: {hinge.connectedAnchor}");
        
        Debug.Log("===== 钟摆配置完成 =====");
    }
    
    /// <summary>
    /// 施加启动力
    /// </summary>
    [ContextMenu("施加启动力")]
    public void ApplyStartForce()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        
        if (rb == null)
        {
            Debug.LogError("❌ 未找到Rigidbody！");
            return;
        }
        
        // 重置速度
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // 施加力（沿X轴或Z轴，取决于场景布局）
        // 尝试X轴
        Vector3 force = Vector3.right * startForce;
        rb.AddForce(force, ForceMode.Impulse);
        
        Debug.Log($"✅ 施加启动力: {force}");
        
        // 如果0.5秒后还不动，尝试其他方向
        Invoke("CheckAndRetry", 0.5f);
    }
    
    /// <summary>
    /// 检查是否在运动，如果不动则尝试其他方向
    /// </summary>
    void CheckAndRetry()
    {
        if (rb != null && rb.velocity.magnitude < 0.1f)
        {
            Debug.LogWarning("⚠️ 摆球还是不动，尝试Z轴方向的力...");
            Vector3 force = Vector3.forward * startForce;
            rb.AddForce(force, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("✅ 摆球正在运动！");
        }
    }
    
    void Update()
    {
        // 按R键重新启动
        if (Input.GetKeyDown(KeyCode.R))
        {
            ApplyStartForce();
        }
    }
}

