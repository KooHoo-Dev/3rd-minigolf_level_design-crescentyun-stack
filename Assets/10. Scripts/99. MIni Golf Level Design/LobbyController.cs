using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study3D.MiniGolf
{
    public class LobbyController : MonoBehaviour
    {
        [Header("Move")]
        public float moveForce = 20.0f; // 이동 가속력
        public float maxSpeed = 6.0f;   // 수평 최대 속도

        [Header("Jump")]
        public float jumpSpeed = 6.0f;              // 점프 순간 위쪽 속도
        public float groundCheckDistance = 0.6f;    // 공 반지름 + 여유
        public LayerMask groundMask = ~0;           // 바닥으로 취급할 레이어 (기본: 전부)

        [Header("Camera")]
        public float yawSensitivity = 0.2f;         // 마우스 1픽셀당 회전 각도

        [Header("Ref")]
        public GolfPlainCamera cam;

        public bool IsGrounded { get; private set; }

        private Rigidbody rBody;
        private Camera mainCam;
        private Vector2 moveInput;
        private bool jumpRequested;

        private Vector3 startPosition;
        private bool respawnRequested;
        
        
        private void Awake()
        {
            rBody = GetComponent<Rigidbody>();
            mainCam = Camera.main;
            startPosition = transform.position;
        }

        // 입력은 Update에서 읽고
        private void Update()
        {
            moveInput = SimpleInput.GetMoveAxisRaw();

            if (SimpleInput.GetKeyDown(Key.Space))
                jumpRequested = true;

            if (SimpleInput.GetKeyDown(Key.R))
                respawnRequested = true;
            
            
            UpdateCamera();
        }

        // 물리 적용은 FixedUpdate에서 합니다.
        private void FixedUpdate()
        {
            if (respawnRequested)
            {
                Respawn();
                return; // 이번 물리 프레임은 이동, 점프 처리를 건너뜁니다.
            }

            CheckGround();
            Move();
            Jump();
            LimitHorizontalSpeed();
        }

        // 왼쪽 클릭을 누른 채 마우스를 좌우로 움직이면 카메라가 회전합니다.
        private void UpdateCamera()
        {
            if (cam == null) return;

            if (SimpleInput.GetMouseButton(SimpleInput.MouseButton.Left))
            {
                Vector2 delta = SimpleInput.GetMouseDelta();
                cam.Yaw += delta.x * yawSensitivity;
            }
        }

        private void CheckGround()
        {
            IsGrounded = Physics.Raycast(
                transform.position,
                Vector3.down,
                groundCheckDistance,
                groundMask,
                QueryTriggerInteraction.Ignore);
        }

        // 카메라 기준으로 W가 앞, D가 오른쪽입니다.
        private void Move()
        {
            Vector3 forward = mainCam.transform.forward;
            forward.y = 0.0f;
            forward.Normalize();

            Vector3 right = mainCam.transform.right;
            right.y = 0.0f;
            right.Normalize();

            Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
            rBody.AddForce(moveDirection * moveForce, ForceMode.Acceleration);
        }

        private void Jump()
        {
            // 요청은 이번 물리 프레임에서 처리하고 무조건 비웁니다.
            // (허공에서 누른 점프가 착지 후 나중에 발동하는 것을 막습니다.)
            bool wantJump = jumpRequested;
            jumpRequested = false;

            if (wantJump == false || IsGrounded == false) return;

            // 위쪽 속도를 직접 지정해서 항상 같은 높이로 뜨게 합니다.
            Vector3 velocity = rBody.linearVelocity;
            velocity.y = jumpSpeed;
            rBody.linearVelocity = velocity;
        }

        // 수평 속도만 제한합니다. (점프, 낙하 속도는 건드리지 않습니다.)
        private void LimitHorizontalSpeed()
        {
            Vector3 velocity = rBody.linearVelocity;
            Vector3 horizontal = new Vector3(velocity.x, 0.0f, velocity.z);

            if (horizontal.magnitude > maxSpeed)
            {
                horizontal = horizontal.normalized * maxSpeed;
                rBody.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
            }
        }
        
        private void Respawn()
        {
            respawnRequested = false;
            jumpRequested = false;

            // 운동량(선, 각)을 0으로 만들고
            rBody.linearVelocity = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;

            // Rigidbody와 transform 위치를 모두 옮깁니다.
            rBody.position = startPosition;
            transform.position = startPosition;
        }
        
        
    }
}