using UnityEngine;

/// <summary>
/// 游戏管理器 - 管理金币收集和游戏状态
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("收集物设置")]
    [Tooltip("场景中金币总数")]
    public int totalCoins = 11;
    
    [Tooltip("已收集的金币数量")]
    private int collectedCoins = 0;
    
    [Header("UI管理器引用")]
    [Tooltip("拖入UIManager组件")]
    public UIManager uiManager;
    
    [Header("完成提示")]
    [Tooltip("收集完成后的提示信息")]
    public string completionMessage = "All coins have been collected!";

    void Start()
    {
        // 初始化UI显示
        if (uiManager != null)
        {
            uiManager.UpdateCollectInfo(collectedCoins, totalCoins);
        }
        else
        {
            Debug.LogWarning("UIManager未设置！请在Inspector中拖入UIManager组件。");
        }
    }

    /// <summary>
    /// 增加收集计数
    /// </summary>
    public void AddCoin()
    {
        collectedCoins++;
        
        // 更新UI显示
        if (uiManager != null)
        {
            uiManager.UpdateCollectInfo(collectedCoins, totalCoins);
        }
        
        Debug.Log($"收集金币：{collectedCoins} / {totalCoins}");
        
        // 检查是否收集完所有金币
        if (collectedCoins >= totalCoins)
        {
            OnAllCoinsCollected();
        }
    }

    /// <summary>
    /// 所有金币收集完成后的处理
    /// </summary>
    private void OnAllCoinsCollected()
    {
        Debug.Log(completionMessage);
        
        // 显示完成提示
        if (uiManager != null)
        {
            uiManager.ShowCompletionMessage(completionMessage);
        }
        
        // 触发"开启终点门"事件
        OpenExitDoor();
    }

    /// <summary>
    /// 开启终点门
    /// </summary>
    private void OpenExitDoor()
    {
        // 可以通过以下方式实现：
        // 1. 查找终点门对象并激活/禁用碰撞器
        // 2. 播放门开启动画
        // 3. 发送事件通知其他系统
        
        GameObject exitDoor = GameObject.FindGameObjectWithTag("ExitDoor");
        if (exitDoor != null)
        {
            // 示例：禁用门的碰撞器
            Collider doorCollider = exitDoor.GetComponent<Collider>();
            if (doorCollider != null)
            {
                doorCollider.enabled = false;
            }
            
            // 示例：播放门的动画（如果有Animator组件）
            Animator doorAnimator = exitDoor.GetComponent<Animator>();
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("Open");
            }
            
            Debug.Log("终点门已开启！");
        }
        else
        {
            Debug.LogWarning("未找到标签为'ExitDoor'的对象。请为终点门添加'ExitDoor'标签。");
        }
    }
}

