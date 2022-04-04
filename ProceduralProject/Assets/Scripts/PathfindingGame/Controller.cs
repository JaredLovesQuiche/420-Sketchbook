using UnityEngine;

public class Controller : MonoBehaviour
{
    public Transform camPivot;

    private Vector3 offset;
    private Vector3 cameraLerpTarget;
    public float lerp = 0.1f;

    private void Start()
    {
        offset = transform.position - camPivot.position;
        cameraLerpTarget = transform.localPosition;
    }

    private void FixedUpdate()
    {
        cameraLerpTarget = camPivot.position + offset;
        cameraLerpTarget.Set(0, cameraLerpTarget.y, cameraLerpTarget.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraLerpTarget, lerp);
    }
}
