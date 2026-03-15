using UnityEngine;
using UnityEditor;

/// <summary>
/// 批量配置金币音效和特效的编辑器工具
/// 使用方法：Unity菜单栏 → Tools → 批量配置金币音效特效
/// </summary>
public class CollectibleBatchSetup : EditorWindow
{
    private AudioClip pickupSound;
    private GameObject pickupEffect;
    private float soundVolume = 0.8f;
    private float effectDestroyTime = 2f;
    
    [MenuItem("Tools/批量配置金币音效特效")]
    public static void ShowWindow()
    {
        GetWindow<CollectibleBatchSetup>("批量配置金币");
    }
    
    void OnGUI()
    {
        GUILayout.Label("批量配置所有金币的音效和特效", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // 音效设置
        GUILayout.Label("音效设置", EditorStyles.boldLabel);
        pickupSound = (AudioClip)EditorGUILayout.ObjectField("拾取音效", pickupSound, typeof(AudioClip), false);
        soundVolume = EditorGUILayout.Slider("音效音量", soundVolume, 0f, 1f);
        
        GUILayout.Space(10);
        
        // 特效设置
        GUILayout.Label("特效设置", EditorStyles.boldLabel);
        pickupEffect = (GameObject)EditorGUILayout.ObjectField("拾取特效预制体", pickupEffect, typeof(GameObject), false);
        effectDestroyTime = EditorGUILayout.FloatField("特效销毁时间", effectDestroyTime);
        
        GUILayout.Space(20);
        
        // 应用按钮
        if (GUILayout.Button("应用到场景中所有金币", GUILayout.Height(40)))
        {
            ApplyToAllCollectibles();
        }
        
        GUILayout.Space(10);
        
        // 帮助信息
        EditorGUILayout.HelpBox(
            "此工具会将上述设置应用到当前场景中所有带有 Collectible 组件的对象。\n\n" +
            "步骤：\n" +
            "1. 从 Assets/Audio 拖入音效文件\n" +
            "2. 从 Assets/Effects 拖入特效预制体\n" +
            "3. 点击「应用到场景中所有金币」按钮",
            MessageType.Info
        );
    }
    
    private void ApplyToAllCollectibles()
    {
        // 查找场景中所有的 Collectible 组件
        Collectible[] collectibles = FindObjectsOfType<Collectible>();
        
        if (collectibles.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "场景中没有找到任何金币对象（Collectible组件）", "确定");
            return;
        }
        
        // 记录修改操作，支持撤销
        Undo.RecordObjects(collectibles, "批量配置金币");
        
        int successCount = 0;
        
        foreach (Collectible collectible in collectibles)
        {
            // 应用音效设置
            if (pickupSound != null)
            {
                collectible.pickupSound = pickupSound;
            }
            collectible.soundVolume = soundVolume;
            
            // 应用特效设置
            if (pickupEffect != null)
            {
                collectible.pickupEffect = pickupEffect;
            }
            collectible.effectDestroyTime = effectDestroyTime;
            
            // 标记对象已修改
            EditorUtility.SetDirty(collectible);
            
            successCount++;
        }
        
        // 显示结果
        string message = $"成功配置了 {successCount} 个金币对象！\n\n";
        if (pickupSound != null)
        {
            message += $"音效：{pickupSound.name}\n";
        }
        if (pickupEffect != null)
        {
            message += $"特效：{pickupEffect.name}\n";
        }
        message += $"音量：{soundVolume}\n";
        message += $"特效销毁时间：{effectDestroyTime}秒";
        
        EditorUtility.DisplayDialog("配置完成", message, "确定");
        
        Debug.Log($"✓ 批量配置完成：已为 {successCount} 个金币配置音效和特效");
    }
}
