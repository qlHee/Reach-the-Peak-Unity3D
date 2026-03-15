using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// UI管理器 - 负责更新游戏界面
/// </summary>
public class UIManager : MonoBehaviour
{
	[Header("UI组件引用 (TMP)")]
	[Tooltip("信息文本（Collected等） - TextMeshProUGUI")]
	public TextMeshProUGUI textInfo;
	[Tooltip("临时提示文本 - TextMeshProUGUI")]
	public TextMeshProUGUI textHint;
	[Tooltip("胜利文本 - TextMeshProUGUI")]
	public TextMeshProUGUI textWin;
	
	[Header("显示设置")]
	[Tooltip("默认提示显示时长（秒）")]
	public float defaultHintDuration = 2f;
	
	private Coroutine hintCoroutine;

	void Awake()
	{
		// 启动时隐藏不必要UI
		if (textHint != null) textHint.gameObject.SetActive(false);
		if (textWin != null) textWin.gameObject.SetActive(false);
	}

	// ===== 按需求新增的公开接口 =====
	/// <summary>
	/// 设置信息文本（例如：Collected: x / y）
	/// </summary>
	public void SetInfo(string message)
	{
		if (textInfo != null)
		{
			textInfo.text = message;
		}
	}

	/// <summary>
	/// 显示或隐藏胜利文本
	/// </summary>
	public void ShowWin(bool active)
	{
		if (textWin != null)
		{
			textWin.gameObject.SetActive(active);
		}
	}

	/// <summary>
	/// 显示短暂提示并在持续时间后自动隐藏
	/// </summary>
	public void ShowHint(string message, float duration)
	{
		if (textHint == null) return;
		
		if (hintCoroutine != null)
		{
			StopCoroutine(hintCoroutine);
		}
		hintCoroutine = StartCoroutine(Co_ShowHint(message, duration <= 0f ? defaultHintDuration : duration));
	}

	private IEnumerator Co_ShowHint(string message, float duration)
	{
		textHint.text = message;
		textHint.gameObject.SetActive(true);
		yield return new WaitForSeconds(duration);
		textHint.gameObject.SetActive(false);
	}

	// ===== 兼容原有方法（最小改动保留）=====
	public void UpdateCollectInfo(int current, int total)
	{
		SetInfo($"Collected: {current} / {total}");
	}
	
	public void ShowCompletionMessage(string message)
	{
		ShowHint(message, defaultHintDuration);
	}
	
	private void HideCompletionMessage()
	{
		if (textHint != null)
		{
			textHint.gameObject.SetActive(false);
		}
	}
}

