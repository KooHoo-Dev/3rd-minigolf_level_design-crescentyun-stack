using UnityEngine;

namespace Study3D.MiniGolf
{
    // 물 오브젝트에 붙이는 낙하 감지 스크립트입니다.
    // 공이 닿으면 마지막으로 친 위치로 되돌립니다.
    public class WaterTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // 콜라이더가 자식에 있어도 찾을 수 있도록 부모까지 검색합니다.
            GolfBall ball = other.GetComponentInParent<GolfBall>();
            if (ball == null) return;

            ball.ReturnToLasShotPosition();
        }
    }
}