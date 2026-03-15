using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// UI管理器 - 负责更新游戏界面
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI组件引用")]
    [Tooltip("收集信息文本 - 拖入Text_CollectInfo对象（TextMeshPro）")]
    public TextMeshProUGUI collectInfoText;
    
    [Tooltip("完成提示文本（可选）（TextMeshPro）")]
    public TextMeshProUGUI completionText;
    
    [Tooltip("陷阱提示文本（TextMeshPro）")]
    public TextMeshProUGUI trapHintText;
    
    [Tooltip("陷阱提示CanvasGroup（可选，未设置时会自动添加到TrapHint对象上）")]
    public CanvasGroup trapHintCanvasGroup;
    
    [Tooltip("收集提醒文本（TextMeshPro）")]
    public TextMeshProUGUI collectRequirementText;
    
    [Header("显示设置")]
    [Tooltip("完成提示显示时长（秒）")]
    public float completionDisplayDuration = 3f;
    
    [Tooltip("陷阱提示显示时长（秒）")]
    public float trapHintDisplayDuration = 2f;
    
    [Tooltip("陷阱提示渐变时间（秒）")]
    public float trapHintFadeDuration = 0.3f;

    [Header("阶段提示设置")]
    [Tooltip("阶段提示文本（例如：Phase2: Middle）")]
    public TextMeshProUGUI phaseText;

    [Tooltip("阶段提示显示时长（秒）")]
    public float phaseDisplayDuration = 2f;
    
    private Coroutine trapHintCoroutine;

    [Header("通关界面")]
    [Tooltip("通关界面根对象（包含 Restart / NextLevel 按钮）")]
    public GameObject winPanel;

    void Start()
    {
        // 隐藏完成提示文本
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
        
        if (collectRequirementText != null)
        {
            collectRequirementText.text = "You need to collect all coins first!";
            collectRequirementText.gameObject.SetActive(true);
        }
        
        // 隐藏陷阱提示文本
        if (trapHintText != null)
        {
            if (trapHintCanvasGroup == null)
            {
                trapHintCanvasGroup = trapHintText.GetComponent<CanvasGroup>();
                if (trapHintCanvasGroup == null)
                {
                    trapHintCanvasGroup = trapHintText.gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            trapHintCanvasGroup.alpha = 0f;
            trapHintCanvasGroup.interactable = false;
            trapHintCanvasGroup.blocksRaycasts = false;
            trapHintText.gameObject.SetActive(false);
        }

        // 隐藏通关界面
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // 隐藏阶段提示文本并保存原始颜色
        if (phaseText != null)
        {
            phaseTextOriginalColor = phaseText.color;
            phaseText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 更新收集信息显示
    /// </summary>
    /// <param name="current">当前收集数量</param>
    /// <param name="total">总数量</param>
    public void UpdateCollectInfo(int current, int total)
    {
        if (collectInfoText != null)
        {
            collectInfoText.text = $"Collected: {current} / {total}";
        }
        else
        {
            Debug.LogWarning("collectInfoText未设置！请在Inspector中拖入Text组件。");
        }
    }

    /// <summary>
    /// 显示完成提示信息
    /// </summary>
    /// <param name="message">提示信息内容</param>
    public void ShowCompletionMessage(string message)
    {
        if (completionText != null)
        {
            completionText.text = message;
            completionText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideCompletionMessage));
            
            if (collectRequirementText != null)
            {
                collectRequirementText.gameObject.SetActive(false);
            }
            
            Time.timeScale = 0f;
        }
    }

    /// <summary>
    /// 隐藏完成提示信息
    /// </summary>
    private void HideCompletionMessage()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 显示陷阱提示信息
    /// </summary>
    /// <param name="message">提示信息内容</param>
    public void ShowTrapHint(string message)
    {
        if (trapHintText != null)
        {
            trapHintText.text = message;
            if (trapHintCoroutine != null)
            {
                StopCoroutine(trapHintCoroutine);
            }
            trapHintCoroutine = StartCoroutine(TrapHintRoutine());
        }
    }

    /// <summary>
    /// 显示“需要先收集所有金币”的提示
    /// </summary>
    public void ShowCollectRequirementMessage(string message)
    {
        if (collectRequirementText != null)
        {
            collectRequirementText.text = message;
            collectRequirementText.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log(message);
        }
    }

    // 保存阶段提示的原始颜色
    private Color phaseTextOriginalColor;

    /// <summary>
    /// 显示阶段提示信息（不会暂停游戏）
    /// </summary>
    /// <param name="message">提示信息内容，例如 "Phase2: Middle"</param>
    public void ShowPhaseMessage(string message)
    {
        ShowPhaseMessage(message, phaseTextOriginalColor, phaseDisplayDuration);
    }

    /// <summary>
    /// 显示阶段提示信息（支持自定义颜色和时长）
    /// </summary>
    /// <param name="message">提示信息内容</param>
    /// <param name="color">文字颜色</param>
    /// <param name="duration">显示时长（秒）</param>
    public void ShowPhaseMessage(string message, Color color, float duration)
    {
        if (phaseText == null)
        {
            Debug.LogWarning("phaseText 未设置！请在 Inspector 中将阶段提示 Text 对象拖入 UIManager。");
            return;
        }

        phaseText.text = message;
        phaseText.color = color;
        phaseText.gameObject.SetActive(true);

        CancelInvoke(nameof(HidePhaseMessage));
        Invoke(nameof(HidePhaseMessage), duration);
    }

    /// <summary>
    /// 隐藏阶段提示信息
    /// </summary>
    private void HidePhaseMessage()
    {
        if (phaseText != null)
        {
            phaseText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 显示通关界面（不再暂停全局 TimeScale，避免按钮无法点击）
    /// </summary>
    public void ShowWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("winPanel 未设置！请在 Inspector 中将通关界面对象拖入 UIManager。");
        }
    }

    /// <summary>
    /// 供 Restart 按钮调用：重启当前关卡
    /// </summary>
    public void RestartLevel()
    {
        // 恢复时间缩放后重新加载当前场景
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    /// <summary>
    /// 供 Next Level 按钮调用：加载下一关（占位实现）
    /// </summary>
    public void LoadNextLevel()
    {
        // 这里先做占位实现：重新加载当前场景
        // 未来你可以改为加载真正的下一关（例如按 buildIndex + 1）
        Debug.Log("LoadNextLevel 被调用，目前作为占位重新加载当前关卡。");
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    /// <summary>
    /// 隐藏陷阱提示信息
    /// </summary>
    private void HideTrapHint()
    {
        if (trapHintText != null && trapHintCanvasGroup != null)
        {
            if (trapHintCoroutine != null)
            {
                StopCoroutine(trapHintCoroutine);
            }
            trapHintCoroutine = StartCoroutine(FadeTrapHint(0f, true));
        }
    }
    
    private IEnumerator TrapHintRoutine()
    {
        trapHintText.gameObject.SetActive(true);
        yield return FadeTrapHint(1f, false);
        yield return new WaitForSecondsRealtime(trapHintDisplayDuration);
        yield return FadeTrapHint(0f, true);
        trapHintCoroutine = null;
    }
    
    private IEnumerator FadeTrapHint(float targetAlpha, bool deactivateOnEnd)
    {
        if (trapHintCanvasGroup == null)
        {
            yield break;
        }
        
        float startAlpha = trapHintCanvasGroup.alpha;
        float elapsed = 0f;
        float duration = Mathf.Max(0.001f, trapHintFadeDuration);
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            trapHintCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        
        trapHintCanvasGroup.alpha = targetAlpha;
        
        if (deactivateOnEnd)
        {
            trapHintText.gameObject.SetActive(false);
        }
        
        trapHintCoroutine = null;
    }
}

