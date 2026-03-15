using UnityEngine;
using TMPro;

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
    
    [Header("显示设置")]
    [Tooltip("完成提示显示时长（秒）")]
    public float completionDisplayDuration = 3f;

    void Start()
    {
        // 隐藏完成提示文本
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
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
            
            // 可选：一段时间后自动隐藏提示
            Invoke(nameof(HideCompletionMessage), completionDisplayDuration);
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
}

