using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class CharacterController2D : MonoBehaviour
    {
        [Header("Movement Variables")]
        public float jumpForce = 15f;
        public float maxSlideSpeed;
        public float coyoteTime = 0.2f;
        private float _coyoteTimeCounter;
        
        [Header("Debug")]
        [field: SerializeField] public Vector2 PlayerSpeed { get; private set; } = Vector2.zero;

        private Rigidbody2D _rb;
        
        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundCheckRadius = 0.2f;
        private int _groundLayerId;
        
        [Header("Jump Settings")]
        public int possibleJumpsInRow = 2;
        private int _currentJumpsInRow;
        public float jumpCooldownMillis = 300f;
        private float _currentJumpCooldownTimestamp;

        [Header("Spring Settings")]
        public float springCompressionFactor = 0.8f;
        
        private Rigidbody2D[] _bodyRigidbodies;
        private SpringJoint2D[] _bodySpringJoints;
        private int _lastDirection = 1;
        
        [Header("Dashing Settings")]
        private bool _canDash = true;
        private bool _isDashing;
        public float dashingPower = 24f;
        public float dashingTime = 0.2f;
        public float dashingCoolDown = 1f;
        [SerializeField] private TrailRenderer trailRenderer;
        
        [Header("Hurt Settings")]
        public Transform hurtTrigger;
        public float hurtTriggerRadius = 1f;
        private int _hurtLayerId;
        private bool _isInvincible;
        [Tooltip("Duration of a whole iframe - player changes color from white to normal")]
        public float invincibilityFrameDurationSecs = 0.3f;
        private SpriteRenderer[] _spriteRenderers;
        private readonly List<Color> _normalSpriteColors = new List<Color>();
        public int numberOfIframes = 10;

        [Header("Disable movement")] [Tooltip("Useful for cutscenes")]
        public bool disablePlayerInteractivity = false; 
        
        private GameManager _gameManager;

        // jump modifier 

        [Header("Downward extra force")] 
        public float downwardForceMultiplier = 1.5f;
        public SpriteRenderer umbrellaVisualQueue;
        public Vector2 pressBoxOffsets = new Vector2(0f, 1f);
        private GameObject _ceilingForCrouching;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            
            _groundLayerId = LayerMask.GetMask("Ground");
            _hurtLayerId = LayerMask.GetMask("Damage");
            
            _currentJumpCooldownTimestamp = Time.time * 1000;
            var parentBody = transform.parent;
            
            _bodyRigidbodies = parentBody.GetComponentsInChildren<Rigidbody2D>();
            _bodySpringJoints = parentBody.GetComponentsInChildren<SpringJoint2D>();
            _spriteRenderers = parentBody.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer spr in _spriteRenderers)
            {
                _normalSpriteColors.Add(spr.color);
            }

            _gameManager = GameManager.Instance;
            _gameManager.OnPlayerTakeDamage(0);

            
            if (umbrellaVisualQueue) umbrellaVisualQueue.enabled = false;

            _ceilingForCrouching = new GameObject("SoftBodyCrouchLimiter");
            BoxCollider2D col = _ceilingForCrouching.AddComponent<BoxCollider2D>();
           

        }

        private void Update()
        {
           
            if (disablePlayerInteractivity) return; 
            if (_isDashing) return;
            
            _coyoteTimeCounter = IsGrounded() ? coyoteTime : _coyoteTimeCounter - Time.deltaTime;
            float capturedTime = Time.time * 1000;
            
            if (capturedTime - _currentJumpCooldownTimestamp > jumpCooldownMillis && Input.GetButton("Jump") &&
                (IsGrounded() || possibleJumpsInRow - 1 > _currentJumpsInRow || _coyoteTimeCounter > 0f))
            {
                Jump();
                _currentJumpCooldownTimestamp = capturedTime;
                _coyoteTimeCounter = 0;
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash) StartCoroutine(Dash());
            if (IsGrounded()) _currentJumpsInRow = 0;
            if (!_isInvincible && IsHurt()) StartCoroutine(OnCharacterTakeDamage());
            
        }

        private void FixedUpdate()
        {
            
            if (disablePlayerInteractivity) return; 
            if (_isDashing) return;

            if (_rb.linearVelocity.y < -0.1 && !IsGrounded() && Input.GetAxis("Vertical") <= 0)
                ApplyForce(Vector2.down * downwardForceMultiplier);

            if (Input.GetAxis("Vertical") == 0)
            {
                _ceilingForCrouching.GetComponent<BoxCollider2D>().isTrigger = true;
            }

            if (Input.GetAxis("Vertical") > 0 && !IsGrounded())
            {
                if (umbrellaVisualQueue) umbrellaVisualQueue.enabled = true; 
            }else if (umbrellaVisualQueue) umbrellaVisualQueue.enabled = false; 

            Vector2 movementDir = new Vector2(Input.GetAxis("Horizontal") * maxSlideSpeed, 0);
            if (movementDir.x != 0) _lastDirection = movementDir.x > 0 ? 1 : -1;
            
            _rb.linearVelocity = movementDir;

            // crouch mechanic using invisible box that presses  on the player when he's grounded and down key has been pressed --Bench 
            if (Input.GetAxis("Vertical") < 0 && IsGrounded())
            {
                _ceilingForCrouching.transform.position = new Vector3(transform.position.x + pressBoxOffsets.x, transform.position.y + pressBoxOffsets.y, transform.position.z);
                _ceilingForCrouching.GetComponent<BoxCollider2D>().isTrigger = false;
                SetLinearVelocity(movementDir * 0.5f); // just add linear x movement on every spring joint so the player can actually move 
            }

            PlayerSpeed = _rb.linearVelocity;
           
        }

        private bool IsGrounded() => groundCheck && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, _groundLayerId);
        private bool IsHurt() => hurtTrigger && Physics2D.OverlapCircle(hurtTrigger.position, hurtTriggerRadius, _hurtLayerId);

        private IEnumerator OnCharacterTakeDamage()
        {
            
            foreach (var rb in _bodyRigidbodies)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * -5f, 3f);
            }

            _isInvincible = true;
            _gameManager.OnPlayerTakeDamage(1);
           
            for (int i = 0; i < numberOfIframes; i++)
            {

                float currentTimer = invincibilityFrameDurationSecs * Mathf.Pow(10, -i * 0.1f) * 1/2f; 
                
                foreach (var spr in _spriteRenderers)
                {
                    spr.color = new Color(255, 0, 0, 0.75f);
                }
                
                yield return new WaitForSeconds(currentTimer / 2);

                for (var j = 0; j < _spriteRenderers.Length; j++)
                {
                    _spriteRenderers[j].color = _normalSpriteColors[j]; 
                }
                
                yield return new WaitForSeconds(currentTimer / 2);

            }
            
            _isInvincible = false;
        }

        private void Jump()
        {
            foreach (var rb in _bodyRigidbodies)
            {

                rb.linearVelocity += new Vector2(0, jumpForce);
                _currentJumpsInRow++;
            }
        }
        
        // good enough for now --Bench 
        
        private void ApplyForce(Vector2 forceAmount, ForceMode2D forceMode = ForceMode2D.Impulse)
        {
            foreach (var rb in _bodyRigidbodies)
            {
                rb.AddForce(forceAmount, forceMode);                
            }
        }

        private void SetLinearVelocity(Vector2 velocity)
        {
            foreach (var rb in _bodyRigidbodies)
            {
                rb.linearVelocity = velocity; 
            }
        }

        public void AddOnVelocity(Vector2 velocity)
        {
            foreach (var rb in _bodyRigidbodies)
            {
                rb.linearVelocity += velocity; 
            }
        }


        private void OnDestroy()
        {
            Destroy(_ceilingForCrouching);
        }

        private IEnumerator Dash()
        {
            _canDash = false;
            _isDashing = true;
            
            var originalGravities = new float[_bodyRigidbodies.Length];
            for (int i = 0; i < _bodyRigidbodies.Length; i++)
            {
                originalGravities[i] = _bodyRigidbodies[i].gravityScale;
                _bodyRigidbodies[i].gravityScale = 0;
                _bodyRigidbodies[i].linearVelocity = new Vector2(transform.localScale.x * dashingPower * _lastDirection, 0f);
            }
            
            trailRenderer.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            
            trailRenderer.emitting = false;
            for (int i = 0; i < _bodyRigidbodies.Length; i++)
                _bodyRigidbodies[i].gravityScale = originalGravities[i];
            
            _isDashing = false;
            yield return new WaitForSeconds(dashingCoolDown);
            _canDash = true;
        }

        private void OnDrawGizmosSelected()
        {
            if (!groundCheck || !hurtTrigger) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hurtTrigger.position, hurtTriggerRadius);
        }

        public void DestroyWholePlayer()
        {
            var grandParentObject = transform.parent.parent.gameObject; 
            Debug.Log(grandParentObject);
            Destroy(grandParentObject);
        }

        public float GetPlayerSpeedMagnitude()
        {
            return this._rb.linearVelocity.magnitude; 
        }


    }
}
