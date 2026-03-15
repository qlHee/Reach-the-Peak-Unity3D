using UnityEngine;

/// <summary>
/// 游戏管理器 - 管理金币收集和游戏状态
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 关卡阶段
    /// </summary>
    public enum LevelPhase
    {
        Start,  // Phase1: Start
        Middle, // Phase2: Middle
        End     // Phase3: End
    }

    [Header("收集物设置")]
    [Tooltip("场景中金币总数")]
    public int totalCoins = 11;
    
    [Tooltip("已收集的金币数量")]
    private int collectedCoins = 0;
    
    [Header("阶段控制")]
    [Tooltip("当前关卡阶段（只读，仅用于调试查看）")]
    public LevelPhase currentPhase = LevelPhase.Start;

    [Tooltip("进入 Middle 阶段所需的最少金币数")]
    public int middlePhaseThreshold = 1;

    [Tooltip("进入 End 阶段所需的最少金币数")]
    public int endPhaseThreshold = 6;

    [Header("背景音乐设置")]
    [Tooltip("背景音乐 AudioSource（拖入 BGMplayer 上的 AudioSource）")]
    public AudioSource bgmAudioSource;

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
            // 初始化阶段提示为 Start（可选）
            uiManager.ShowPhaseMessage("Phase1: Start");
        }
        else
        {
            Debug.LogWarning("UIManager未设置！请在Inspector中拖入UIManager组件。");
        }
    }

    /// <summary>
    /// 是否已经收集完所有金币（供其他脚本查询）
    /// </summary>
    public bool IsAllCoinsCollected()
    {
        return collectedCoins >= totalCoins;
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

        // 根据收集数量更新阶段
        UpdatePhaseIfNeeded();
        
        // 检查是否收集完所有金币
        if (collectedCoins >= totalCoins)
        {
            OnAllCoinsCollected();
        }
    }

    /// <summary>
    /// 根据当前收集数量检查是否需要切换阶段
    /// </summary>
    private void UpdatePhaseIfNeeded()
    {
        // Start -> Middle
        if (currentPhase == LevelPhase.Start && collectedCoins >= middlePhaseThreshold)
        {
            SetPhase(LevelPhase.Middle);
        }
        // Middle -> End
        else if (currentPhase == LevelPhase.Middle && collectedCoins >= endPhaseThreshold)
        {
            SetPhase(LevelPhase.End);
        }
    }

    /// <summary>
    /// 设置并处理阶段切换
    /// </summary>
    /// <param name="newPhase">新的阶段</param>
    private void SetPhase(LevelPhase newPhase)
    {
        if (currentPhase == newPhase)
        {
            return;
        }

        currentPhase = newPhase;
        Debug.Log($"关卡阶段切换为：{currentPhase}");

        // 根据阶段调整背景音乐播放速率（使用 pitch 实现）
        if (bgmAudioSource != null)
        {
            switch (currentPhase)
            {
                case LevelPhase.Start:
                    bgmAudioSource.pitch = 1f;
                    break;
                case LevelPhase.Middle:
                    // 需求：Middle 阶段播放速率 1.2 倍
                    bgmAudioSource.pitch = 1.2f;
                    break;
                case LevelPhase.End:
                    // 需求：End 阶段播放速率 1.4 倍
                    bgmAudioSource.pitch = 1.4f;
                    break;
            }
        }
        else
        {
            Debug.LogWarning("bgmAudioSource 未设置！请在 Inspector 中将 BGM 对象上的 AudioSource 拖入 GameManager。");
        }

        // 根据阶段显示屏幕提示
        if (uiManager != null)
        {
            switch (currentPhase)
            {
                case LevelPhase.Start:
                    uiManager.ShowPhaseMessage("Phase1: Start");
                    break;
                case LevelPhase.Middle:
                    uiManager.ShowPhaseMessage("Phase2: Middle");
                    break;
                case LevelPhase.End:
                    uiManager.ShowPhaseMessage("Phase3: End");
                    break;
            }
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
        // 1. 查找终点门对象并播放开门动画
        // 2. 发送事件通知其他系统
        
        GameObject exitDoor = GameObject.FindGameObjectWithTag("ExitDoor");
        if (exitDoor != null)
        {
            // 不再禁用 Collider，让其继续作为触发器用于结算

            // 播放门的动画（如果有 Animator 组件）
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

