using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Events;


namespace TempleRun.Player
{


    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float initialPlayerSpeed = 8f;
        [SerializeField]
        private float maximumPlayerSpeed = 30f;
        [SerializeField]
        private float playerSpeedIncrease = .1f;
        [SerializeField]
        private float jumpHeight = 1.0f;
        [SerializeField]
        private float initialGravityValue = -9.81f;
        [SerializeField]
        private LayerMask groundLayer;
        [SerializeField]
        private LayerMask turnLayer;
        [SerializeField]
        private LayerMask obstacleLayer;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private AnimationClip slideAnimationClip;
        [SerializeField]
        private float playerSpeed;
        [SerializeField]
        private float scoreMultiplier = 10f;


        private float gravity;
        private Vector3 movementDirection = Vector3.forward;
        private Vector3 playerVelocity;

        private PlayerInput PlayerInput;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction slideAction;

        private CharacterController controller;

        private int slidingAnimationId;

        private bool sliding = false;
        private float score = 0;
        

        [SerializeField]
        private UnityEvent<Vector3> turnEvent;
        [SerializeField]
        private UnityEvent<int> gameOverEvent;
        [SerializeField]
        private UnityEvent<int> scoreUpdateEvent;

        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();

            slidingAnimationId = Animator.StringToHash("Sliding");

            turnAction = PlayerInput.actions["Turn"];
            jumpAction = PlayerInput.actions["Jump"];
            slideAction = PlayerInput.actions["Slide"];


        }

        private void OnEnable()
        {
            turnAction.performed += PlayerTurn;
            slideAction.performed += PlayerSlide;
            jumpAction.performed += PlayerJump;
        }

        private void OnDisable()
        {
            turnAction.performed -= PlayerTurn;
            slideAction.performed -= PlayerSlide;
            jumpAction.performed -= PlayerJump;
        }

        private void Start()
        {
            gravity = initialGravityValue;
            playerSpeed = initialPlayerSpeed;
        }

        private void PlayerTurn(InputAction.CallbackContext context)
        {
            Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
            if (!turnPosition.HasValue) {
                //GameOver();
                return;
            }
            Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;

            turnEvent.Invoke(targetDirection);

            Turn(context.ReadValue<float>(), turnPosition.Value);

        }

        private void Turn(float turnValue, Vector3 turnPosition) { 
            Vector3 tempPlayerPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
            controller.enabled = false;
            transform.position = tempPlayerPosition;
            controller.enabled = true;

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
            transform.rotation = targetRotation;
            movementDirection = transform.forward.normalized;
        }
        private Vector3? CheckTurn(float turnValue)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);

            if (hitColliders.Length > 0) {
                Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
                TileType type = tile.type;
                if (type == TileType.LEFT && turnValue == -1 ||
                    type == TileType.RIGHT && turnValue == 1 ||
                    type == TileType.SIDEWAYS){
                    return tile.pivot.position;
                }
            }
            return null;
        }

        private void PlayerSlide(InputAction.CallbackContext context)
        {
            if (!sliding && IsGrounded()) { 
                StartCoroutine(Slide());
            }
        }

        private IEnumerator Slide() {
            //reduir tamany del collider a la meitat
            Vector3 originalControllerCenter = controller.center;
            Vector3 newControllerCenter = originalControllerCenter;
            controller.height /= 2;
            newControllerCenter.y -= controller.height / 2;
            controller.center = newControllerCenter;
            
            //fer animacio de slide
            sliding = true;
            animator.Play(slidingAnimationId);
            yield return new WaitForSeconds(slideAnimationClip.length);
            
            //tornar el controller a la mida original
            controller.height *= 2;
            controller.center = originalControllerCenter;
            sliding = false;
        }

        private void PlayerJump(InputAction.CallbackContext context)
        {
            if (IsGrounded())
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
                controller.Move(playerVelocity * Time.deltaTime);
            }
        }

        private void Update() {
            if (!IsGrounded(20f)) {
                GameOver();
                return;
            }

            score += scoreMultiplier * Time.deltaTime;
            scoreUpdateEvent.Invoke((int)score);

            controller.Move(transform.forward * playerSpeed * Time.deltaTime);

            if (IsGrounded() && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        private bool IsGrounded(float length = .2f)
        {
            Vector3 raycastOriginFirst = transform.position;
            raycastOriginFirst.y -= controller.height / 2f;
            raycastOriginFirst.y += .1f;

            Vector3 raycastOriginSecond = raycastOriginFirst;
            raycastOriginFirst -= transform.forward * .2f;
            raycastOriginSecond += transform.forward * .2f;

            //Debug.DrawLine(raycastOriginFirst, Vector3.down, Color.green, 2f);
            //Debug.DrawLine(raycastOriginSecond, Vector3.down, Color.red, 2f);

            if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
            {
                return true;
            }
            else { return false; }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0) {
                GameOver();
            }
        }

        private void GameOver() {
            Debug.Log("GAME OVER");
            gameOverEvent.Invoke((int)score);
            gameObject.SetActive(false);
        }
    }
}
