using UnityEngine;
using TMPro;
using System.Collections;

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
    
    private Coroutine trapHintCoroutine;

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

