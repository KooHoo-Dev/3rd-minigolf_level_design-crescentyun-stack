using Study3D.MiniGolf;
using UnityEngine;

public class ClearZone : MonoBehaviour
{
    public Transform destination;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 콜라이더가 자식에 있어도 찾을 수 있도록 부모까지 검색합니다.
        GolfBall ball = other.GetComponentInParent<GolfBall>();
        if (ball == null) return;

        // 위치 이동 + 속도 초기화 + 마지막 샷 위치를 시작점으로 갱신
        ball.PlaceAt(destination.position);
    }
}