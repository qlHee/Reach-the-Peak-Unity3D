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

    [Tooltip("阶段二的背景音乐")]
    public AudioClip middlePhaseBGM;

    [Tooltip("阶段三的背景音乐")]
    public AudioClip endPhaseBGM;

    [Header("UI管理器引用")]
    [Tooltip("拖入UIManager组件")]
    public UIManager uiManager;
    
    [Header("完成提示")]
    [Tooltip("收集完成后的提示信息")]
    public string completionMessage = "All coins have been collected!";

    [Header("环境光设置")]
    [Tooltip("阶段二的环境光颜色（橘红色）")]
    public Color middlePhaseAmbientColor = new Color(1f, 0.4f, 0.2f, 1f);

    [Tooltip("阶段三的环境光颜色（黑夜效果）")]
    public Color endPhaseAmbientColor = new Color(0.15f, 0.15f, 0.25f, 1f);

    // 保存初始环境光设置
    private UnityEngine.Rendering.AmbientMode originalAmbientMode;
    private Color originalAmbientSkyColor;

    void Start()
    {
        // 保存初始环境光设置
        originalAmbientMode = RenderSettings.ambientMode;
        originalAmbientSkyColor = RenderSettings.ambientSkyColor;

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

        // 根据阶段切换背景音乐
        if (bgmAudioSource != null)
        {
            switch (currentPhase)
            {
                case LevelPhase.Start:
                    bgmAudioSource.pitch = 1f;
                    break;
                case LevelPhase.Middle:
                    // 切换到阶段二背景音乐
                    if (middlePhaseBGM != null)
                    {
                        bgmAudioSource.clip = middlePhaseBGM;
                        bgmAudioSource.loop = true;
                        bgmAudioSource.Play();
                    }
                    bgmAudioSource.pitch = 1.2f;
                    // 激活所有移动平台
                    ActivateAllMovingPlatforms();
                    // 设置阶段二环境光（橘红色）
                    SetAmbientColor(middlePhaseAmbientColor);
                    break;
                case LevelPhase.End:
                    // 切换到阶段三背景音乐
                    if (endPhaseBGM != null)
                    {
                        bgmAudioSource.clip = endPhaseBGM;
                        bgmAudioSource.loop = true;
                        bgmAudioSource.Play();
                    }
                    bgmAudioSource.pitch = 1.4f;
                    // 设置阶段三环境光（黑夜效果）
                    SetAmbientColor(endPhaseAmbientColor);
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
                    uiManager.ShowPhaseMessage("Phase2: Middle", Color.red, 3f);
                    break;
                case LevelPhase.End:
                    uiManager.ShowPhaseMessage("Phase3: End", Color.red, 3f);
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
    /// 设置环境光颜色（切换到Flat模式）
    /// </summary>
    private void SetAmbientColor(Color color)
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientSkyColor = color;
    }

    /// <summary>
    /// 激活所有移动平台
    /// </summary>
    private void ActivateAllMovingPlatforms()
    {
        MovingPlatform[] platforms = FindObjectsOfType<MovingPlatform>();
        foreach (MovingPlatform platform in platforms)
        {
            platform.Activate();
        }
        Debug.Log($"已激活 {platforms.Length} 个移动平台");
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

