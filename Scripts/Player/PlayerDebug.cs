using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    [Tooltip("How many times per second to update stats")]
    [SerializeField] private float refreshRate = 4;

    private int frameCount;
    private float time;
    private float FPS;
    private float topSpeed;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        frameCount++;
        time += Time.deltaTime;
        if (time > 1f / refreshRate)
        {
            FPS = Mathf.Round(frameCount / time);
            frameCount = 0;
            time -= 1f / refreshRate;
        }
        
        if (rb.velocity.magnitude > topSpeed)
        {
            topSpeed = rb.velocity.magnitude;
        }
    }

    // private void OnGUI()
    // {
    //     var velocity = rb.velocity;
    //     velocity.y = 0f;
    //     GUI.Box(new Rect(0, 0, 130, 100),
    //         "FPS: " + FPS + "\n" +
    //         "Speed: " + Mathf.Round(velocity.magnitude * 100) / 100 + " (ups)\n" +
    //         "Top: " + Mathf.Round(topSpeed * 100) / 100 + " (ups)");
    // }
}