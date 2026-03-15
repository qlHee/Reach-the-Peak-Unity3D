using UnityEngine;

/// <summary>
/// Coin旋转脚本 - 让收集物持续旋转，增强视觉效果
/// </summary>
public class CoinRotation : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("旋转速度（度/秒）")]
    public float rotationSpeed = 100f;
    
    [Tooltip("旋转轴向 - 默认绕Y轴（垂直轴）旋转")]
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // 每帧旋转物体
        // Time.deltaTime确保旋转速度不受帧率影响
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
