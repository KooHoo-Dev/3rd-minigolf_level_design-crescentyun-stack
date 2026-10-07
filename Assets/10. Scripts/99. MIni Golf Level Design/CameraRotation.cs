using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float waitTime = 1.0f;       // 대기 시간
    [SerializeField] private float rotateDuration = 2.0f; // 회전 시간

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // 1. 시작 시 X축 회전값을 -1도로 설정
        Vector3 currentRot = cameraTransform.eulerAngles;
        cameraTransform.eulerAngles = new Vector3(-1f, currentRot.y, currentRot.z);

        // 2. 대기 후 X축만 -36도로 회전
        // Vector3의 X값만 변경하고, Y/Z는 현재 카메라 회전값을 유지
        Vector3 targetRot = new Vector3(-36f, currentRot.y, currentRot.z);

        cameraTransform.DORotate(targetRot, rotateDuration)
            .SetDelay(waitTime)
            .SetEase(Ease.InOutQuad); // 부드러운 회전 효과
    }
}