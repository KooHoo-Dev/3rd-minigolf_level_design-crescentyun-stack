using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study3D.MiniGolf
{
    public class Golf : MonoBehaviour
    {
        [Header("Settings")]
        public float minPower;
        public float maxPower;
        public float yawSensitivity = 0.2f;     // 마우스 1픽셀당 회전 각도
        public float maxPullDistance = 200.0f;  // 최대 파워가 되는 마우스 당김 거리 (픽셀)
        public float cancelThreshold = 0.05f;   // 이 비율(0~1) 미만이면 발사 취소

        [Header("Ref")] 
        public GolfBall ball;
        public GolfPlainCamera cam;
        public GolfAimIndicator aimIndicator;

        private Camera mainCam;
        private float pullAmount; // 마우스 상하 이동 누적값 (음수: 아래로 당김 / 양수: 위로 당김)
        private float power01;    // 0~1 까지로 치환한 세기

        
        private Rigidbody rBody;
        private Vector3 startPosition;
        private bool respawnRequested;
        
        
        private void Awake()
        {
            rBody = GetComponent<Rigidbody>();
            
            mainCam = Camera.main;
            
            startPosition = transform.position;
        }

        private void Update()
        {
            Vector3 aimDirection = mainCam.transform.forward;
            aimDirection.y = 0.0f;
            aimDirection.Normalize();

            bool canShoot = ball.IsStopped;
            Vector3 shotDirection = aimDirection;

            if (canShoot)
            {
                if (SimpleInput.GetMouseButtonDown(SimpleInput.MouseButton.Left))
                {
                    ResetPull();
                }

                if (SimpleInput.GetMouseButton(SimpleInput.MouseButton.Left))
                {
                    Vector2 delta = SimpleInput.GetMouseDelta();

                    // 좌우 이동 : 방향 전환
                    cam.Yaw += delta.x * yawSensitivity;

                    // 상하 이동 : 당김 누적 (아래로 당기면 음수)
                    pullAmount = Mathf.Clamp(pullAmount + delta.y, -maxPullDistance, maxPullDistance);
                    power01 = Mathf.Abs(pullAmount) / maxPullDistance;
                }

                // 아래로 당기면 앞쪽, 위로 당기면 뒤쪽으로 발사
                shotDirection = pullAmount <= 0.0f ? aimDirection : -aimDirection;

                if (SimpleInput.GetMouseButtonUp(SimpleInput.MouseButton.Left))
                {
                    // 당김이 너무 작으면 취소
                    if (power01 >= cancelThreshold)
                    {
                        float power = Mathf.Lerp(minPower, maxPower, power01);
                        ball.Shoot(shotDirection, power);
                    }

                    ResetPull();
                }
            }
            else
            {
                ResetPull();
            }

            
            if (SimpleInput.GetKeyDown(Key.R))
                respawnRequested = true;
            
            UpdateFeedback(shotDirection, canShoot);
        }

        private void ResetPull()
        {
            pullAmount = 0.0f;
            power01 = 0.0f;
        }

        // 보통 플레이어의 입력정보들에 관련된 UI나 피드백들은
        // 하나의 함수로 모아두는게 좋습니다. (찢지 마세여)
        // (생각외로 엄청 많이 바뀜)
        private void UpdateFeedback(Vector3 direction, bool canShoot)
        {
            if (canShoot)
                aimIndicator.Show(ball.transform.position, direction, power01);
            else
                aimIndicator.Hide();
            
            if (respawnRequested)
            {
                Debug.Log("Respawn requested");
                Respawn();
                return; // 이번 물리 프레임은 이동, 점프 처리를 건너뜁니다.
            }
            
            
        }
        
        
        private void Respawn()
        {
            respawnRequested = false;

            // 운동량(선, 각)을 0으로 만들고
            rBody.linearVelocity = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;

            // Rigidbody와 transform 위치를 모두 옮깁니다.
            rBody.position = startPosition;
            transform.position = startPosition;
        }
        
    }
}