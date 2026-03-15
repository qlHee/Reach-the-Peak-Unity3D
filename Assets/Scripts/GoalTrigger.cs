using UnityEngine;

/// <summary>
/// 终点触发器 - 判断是否收集完成并触发通关逻辑
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    [Header("引用设置")]
    [Tooltip("GameManager 引用（如果不设置，将在场景中自动查找）")]
    public GameManager gameManager;

    [Tooltip("UIManager 引用（如果不设置，将在场景中自动查找）")]
    public UIManager uiManager;

    [Header("音效设置")]
    [Tooltip("胜利音效（可选）")]
    public AudioClip winClip;

    [Range(0f, 1f)]
    [Tooltip("胜利音效音量")]
    public float winVolume = 0.8f;

    private AudioSource audioSource;

    private void Awake()
    {
        // 自动查找引用，方便使用
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D 音效
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (gameManager == null)
        {
            Debug.LogWarning("GoalTrigger：未找到 GameManager，对终点判断将失效。");
            return;
        }

        // 判断是否已收集全部金币
        bool isAllCollected = gameManager.IsAllCoinsCollected();

        if (!isAllCollected)
        {
            // 收集未完成，提示玩家
            if (uiManager != null)
            {
                uiManager.ShowCollectRequirementMessage("Collect all coins first!");
            }
            else
            {
                Debug.Log("Collect all coins first!");
            }
            return;
        }

        // 条件满足：播放胜利音效并显示通关界面
        PlayWinSound();

        if (uiManager != null)
        {
            uiManager.ShowWinPanel();
        }
    }

    private void PlayWinSound()
    {
        if (winClip == null || audioSource == null)
        {
            return;
        }

        audioSource.clip = winClip;
        audioSource.volume = winVolume;
        audioSource.Play();
    }
}



