using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform objectTofollow;
    public float followSpeed = 10f;
    public float sensitivity = 100f;
    public float ClampAngle = 70f;

    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;
    public Vector3 finalDir;
    public float minDistance = 1f;
    public float maxDistance = 4f;
    public float finalDistance;
    public float smoothness = 10f;

    [Header("View Offset")]
    [Tooltip("카메라가 바라보는 피벗을 캐릭터 위로 올리는 높이")]
    public float heightOffset = 1.5f;
    [Tooltip("카메라 방향. y가 클수록 위에서 내려다봄, z는 항상 음수(뒤)")]
    public Vector3 viewDirection = new Vector3(0f, 0.35f, -1f);

    void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = viewDirection.normalized;
        finalDistance = maxDistance;

        if (realCamera != null)
            realCamera.localPosition = dirNormalized * finalDistance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
    }

    void Update()
    {
        rotX += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -ClampAngle, ClampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }
    private void LateUpdate()
    {
        Vector3 followTarget = objectTofollow.position + Vector3.up * heightOffset;
        transform.position = Vector3.MoveTowards(transform.position, followTarget, followSpeed * Time.deltaTime);

        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;

        if (Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else {
            finalDistance = maxDistance;
        }
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);

    }

}
