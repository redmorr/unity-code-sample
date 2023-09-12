using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private enum Status
    {
        Open,
        Opening,
        Closed,
        Closing
    }
    
    [SerializeField] private Transform doorLeft;
    [SerializeField] private Transform doorRight;
    [SerializeField] private Vector3 openPosition = new(1.5f, 0f, 0f);
    [SerializeField] private Ease tweenEasing;
    [SerializeField] private float speed;
    [SerializeField] private AudioSource doorSlidingStart;
    [SerializeField] private AudioSource doorSlidingStop;

    [Header("Debug")]
    [SerializeField] [ReadOnly] private bool locked;
    [SerializeField] [ReadOnly] private Status doorStatus = Status.Closed;
    [SerializeField] [ReadOnly] private bool playerInside;

    private Tween tween;

    private void Awake()
    {
        doorStatus = Status.Closed;
    }
    
    [Button]
    public void Lock()
    {
        locked = true;
        Close();
    }

    [Button]
    public void Unlock()
    {
        locked = false;
        if (playerInside) Open();
    }

    private void Open()
    {
        if (!locked && (doorStatus == Status.Closed || doorStatus == Status.Closing))
        {
            if (tween != null) tween.Kill();

            tween =
                tween = doorRight.DOLocalMove(openPosition, speed)
                    .OnStart(() =>
                    {
                        doorSlidingStart.Stop();
                        doorSlidingStart.Play();
                        doorStatus = Status.Opening;
                    })
                    .SetSpeedBased(true)
                    .SetEase(tweenEasing)
                    .OnUpdate(() => doorLeft.localPosition = -doorRight.localPosition)
                    .OnComplete(() =>
                    {
                        tween = null;
                        doorSlidingStart.Stop();
                        doorSlidingStop.Play();
                        doorStatus = Status.Open;
                    });
        }
    }

    private void Close()
    {
        if (doorStatus == Status.Open || doorStatus == Status.Opening)
        {
            if (tween != null) tween.Kill();

            tween = doorRight.DOLocalMove(Vector3.zero, speed)
                .OnStart(() =>
                {
                    doorSlidingStart.Stop();
                    doorSlidingStart.Play();
                    doorStatus = Status.Closing;
                })
                .SetSpeedBased(true)
                .SetEase(tweenEasing)
                .OnUpdate(() => doorLeft.localPosition = -doorRight.localPosition)
                .OnComplete(() =>
                {
                    tween = null;
                    doorSlidingStart.Stop();
                    doorSlidingStop.Play();
                    doorStatus = Status.Closed;
                });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Constants.PlayerHitbox)
        {
            playerInside = true;
            Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Constants.PlayerHitbox)
        {
            playerInside = false;
            Close();
        }
    }
}