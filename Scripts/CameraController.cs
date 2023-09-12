using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float duration;
    [SerializeField] private Vector3 strength;
    [SerializeField] private int vibrato;
    [SerializeField] [ReadOnly] private Vector3 originalPos;

    private Tween cameraShake;

    private void Awake()
    {
        originalPos = transform.localPosition;
    }

    [Button]
    public void Shake()
    {
        cameraShake.Kill();
        mainCamera.localPosition = Vector3.zero;
        cameraShake = mainCamera.DOShakePosition(duration, strength, vibrato);
    }

    public void ChangeHeadPosition(Vector3 headPositionChange)
    {
        transform.localPosition = originalPos + headPositionChange;
    }

    public void ResetHeadPosition()
    {
        transform.localPosition = originalPos;
    }
}