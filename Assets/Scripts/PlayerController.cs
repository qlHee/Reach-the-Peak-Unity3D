using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("角色移动速度")]
    [Range(3f, 6f)]
    public float moveSpeed = 5f;
    
    [Tooltip("角色旋转速度")]
    public float rotationSpeed = 10f;
    
    [Tooltip("按键旋转速度（Q/E键）")]
    public float keyRotationSpeed = 90f;
    
    [Header("跳跃设置")]
    [Tooltip("跳跃高度")]
    public float jumpHeight = 2f;
    
    [Header("重力设置")]
    [Tooltip("重力值")]
    public float gravity = -9.81f;
    
    // 组件引用
    private CharacterController characterController;
    private Transform cameraTransform;
    
    // 移动相关变量
    private Vector3 velocity;
    private float verticalVelocity;
    private Vector3 moveDirection; // 当前帧的移动方向
    
    // 重生相关变量
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    
    void Start()
    {
        // 获取CharacterController组件
        characterController = GetComponent<CharacterController>();
        
        if (characterController == null)
        {
            Debug.LogError("PlayerController: 未找到CharacterController组件！");
        }
        
        // 获取主相机Transform
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("PlayerController: 未找到主相机，将使用世界坐标系移动");
        }
        
        // 记录初始位置和旋转（用于重生）
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }
    
    void Update()
    {
        // 处理跳跃和重力（必须先处理，以便正确检测地面状态）
        HandleJumpAndGravity();
        
        // 处理移动和旋转
        HandleMovement();
    }
    
    /// <summary>
    /// 处理角色移动和旋转
    /// </summary>
    void HandleMovement()
    {
        // 1. 处理Q/E键旋转
        if (Input.GetKey(KeyCode.Q))
        {
            // 按Q键向左旋转
            transform.Rotate(0f, -keyRotationSpeed * Time.deltaTime, 0f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            // 按E键向右旋转
            transform.Rotate(0f, keyRotationSpeed * Time.deltaTime, 0f);
        }
        
        // 2. 读取输入轴
        float horizontal = Input.GetAxis("Horizontal"); // A/D 或 左/右方向键
        float vertical = Input.GetAxis("Vertical");     // W/S 或 上/下方向键
        
        // 3. 计算移动方向（相对于Player自身朝向）
        moveDirection = Vector3.zero;
        
        // 使用Player自身的前方和右方向
        Vector3 playerForward = transform.forward;
        Vector3 playerRight = transform.right;
        
        // 将方向投影到水平面上（Y=0）
        playerForward.y = 0f;
        playerRight.y = 0f;
        
        // 归一化方向向量
        playerForward.Normalize();
        playerRight.Normalize();
        
        // 计算相对于Player朝向的移动方向
        moveDirection = playerForward * vertical + playerRight * horizontal;
        
        // 4. 归一化移动方向，避免斜向移动时速度过快
        if (moveDirection.magnitude >= 0.1f)
        {
            moveDirection.Normalize();
        }
    }
    
    /// <summary>
    /// 处理跳跃和重力（CharacterController需要手动处理重力）
    /// </summary>
    void HandleJumpAndGravity()
    {
        // 地面检测
        if (characterController.isGrounded)
        {
            // 如果在地面上且垂直速度为负，重置垂直速度
            if (verticalVelocity < 0)
            {
                verticalVelocity = -2f; // 小的负值确保角色贴地
            }
            
            // 跳跃输入检测（按下空格键）
            if (Input.GetButtonDown("Jump"))
            {
                // 根据跳跃高度计算初始跳跃速度
                // 公式：v = sqrt(2 * jumpHeight * |gravity|)
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravity));
            }
        }
        
        // 应用重力（无论是否在地面上都持续应用）
        verticalVelocity += gravity * Time.deltaTime;
        
        // 合并水平移动和垂直移动，一次性执行Move
        Vector3 finalMove = moveDirection * moveSpeed * Time.deltaTime;
        finalMove.y = verticalVelocity * Time.deltaTime;
        characterController.Move(finalMove);
    }
    
    /// <summary>
    /// 重生方法 - 使角色回到初始位置
    /// </summary>
    public void Respawn()
    {
        // 禁用CharacterController以允许直接设置位置
        characterController.enabled = false;
        
        // 重置位置和旋转
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        
        // 重置垂直速度
        verticalVelocity = 0f;
        velocity = Vector3.zero;
        moveDirection = Vector3.zero;
        
        // 重新启用CharacterController
        characterController.enabled = true;
        
        Debug.Log("玩家已重生到初始位置");
    }
    
    /// <summary>
    /// 设置重生点（可供外部调用，用于设置检查点）
    /// </summary>
    public void SetSpawnPoint(Vector3 newSpawnPosition, Quaternion newSpawnRotation)
    {
        spawnPosition = newSpawnPosition;
        spawnRotation = newSpawnRotation;
        Debug.Log($"重生点已更新到：{spawnPosition}");
    }
    
    /// <summary>
    /// 设置重生点（重载方法，只设置位置）
    /// </summary>
    public void SetSpawnPoint(Vector3 newSpawnPosition)
    {
        spawnPosition = newSpawnPosition;
        spawnRotation = transform.rotation;
        Debug.Log($"重生点已更新到：{spawnPosition}");
    }
    
    /// <summary>
    /// 在编辑器中绘制调试信息
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // 绘制移动方向
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        
        // 绘制地面检测状态
        if (characterController != null)
        {
            Gizmos.color = characterController.isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        
        // 绘制重生点位置（在运行时和编辑器中）
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnPosition, 0.7f);
            Gizmos.DrawLine(spawnPosition, spawnPosition + Vector3.up * 2f);
        }
    }
}