using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camTransform;

    private void Start()
    {
        camTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (camTransform == null) return;

        Vector3 cameraPosition = camTransform.position;
        cameraPosition.y = transform.position.y;

        transform.LookAt(cameraPosition);
        transform.Rotate(0f, 180f, 0f);
    }
}