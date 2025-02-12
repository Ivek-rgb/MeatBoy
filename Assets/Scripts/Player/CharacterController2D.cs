using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Player
{
    /**
 * PLAYER CONTROLLER FOR OUR LITTLE RED BLOB OF PAIN 
 */
    public class CharacterController2D : MonoBehaviour
    {
        [Header("Movement variables")]
        public float jumpForce = 15; // 15 offers really good take 
        public float maxSlideSpeed;
        public float coyoteTime = 0.2f;
        private float _coyoteTimeCounter; 
        
        [Header("Debug print variables")]
        [field: SerializeField] public Vector2 PlayerSpeed { get; private set; } = new(0, 0);

        private Rigidbody2D _rb; 

        [Header("Ground check settings")] 
        public Transform groundCheck;
        public float groundCheckRadius = 0.2f;
        public int groundLayerId;
        
        [Header("Character settings")]
        public int possibleJumpsInRow = 2;
        private int _currentJumpsInRow = 0;

        [Header("Jump settings")] 
        public float jumpCooldownMillis = 300f;   
        private float _currentJumpCooldownTimestamp = 0;

        [Header("Spring settings")] 
        public float springCompressionFactor = 0.8f;

        private Transform _parentBodyComponent;
        private Rigidbody2D[] _bodyRigidbodies;
        private SpringJoint2D[] _bodySpringJoints;
        private int _lastDirection = 1; 
        
        [Header("Dashing settings")] 
        private bool _canDash = true;
        private bool _isDashing = false;
        public float dashingPower = 24f;
        public float dashingTime = 0.2f;
        public float dashingCoolDown = 1f;
        [SerializeField] private TrailRenderer trailRenderer;

        private GameManager _gameManager; // for player UI connection 
        
        void Start()
        {
            
            _rb = GetComponent<Rigidbody2D>();
            groundLayerId = LayerMask.GetMask("Ground");
            _currentJumpCooldownTimestamp = Time.time * 1000;
            _parentBodyComponent = transform.parent;
            
            _bodyRigidbodies = _parentBodyComponent.GetComponentsInChildren<Rigidbody2D>(); 
            _bodySpringJoints = _parentBodyComponent.GetComponentsInChildren<SpringJoint2D>();

            _gameManager = GameManager.Instance; 

        }

        private void Update()
        {
            if (_isDashing) return; // prevent any additional movements while dashing // don't touch this 
            
            CharacterWobbleOnWalk();
            
            Vector2 movementDir = new Vector2(Input.GetAxis("Horizontal") * maxSlideSpeed, 0);

            if (movementDir.x != 0)
                _lastDirection = movementDir.x > 0 ? 1 : -1; 
            
            _rb.AddForce(movementDir, ForceMode2D.Force);  
            _rb.linearVelocity = movementDir;
            float capturedTime = Time.time * 1000;
            
            // implemented to give small timeframe to player to make grounded jump even after leaving the ground  
            if (IsGrounded())
            {
                _coyoteTimeCounter = coyoteTime; 
            }
            else
            {
                _coyoteTimeCounter -= Time.deltaTime; 
            }

            if ( capturedTime - _currentJumpCooldownTimestamp > jumpCooldownMillis &&  Input.GetButton("Jump") && (IsGrounded() || (possibleJumpsInRow - 1 > _currentJumpsInRow) || _coyoteTimeCounter > 0f))
            {
                Jump();
                _currentJumpCooldownTimestamp = capturedTime;
                _coyoteTimeCounter = 0; 
            }

            PlayerSpeed = _rb.linearVelocity;

            if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
            {
                StartCoroutine(Dash()); 
            }
            
            // TODO: implement box for ground checking because it's much more cleaner and better for checking our 'blocky' fellow 

            if (IsGrounded())
            {
                _currentJumpsInRow = 0; 
            }

        }

        private void FixedUpdate()
        {
            if (_isDashing) return; 
        }


        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerId); 
        }


        // TODO: get push factor from damage source and activate iframe with visual display for character 
        private void OnCharacterTakeDamage()
        {
            _gameManager.OnPlayerTakeDamage(1);
        }

        private void Jump()
        {

            foreach (var springRb in _bodyRigidbodies)
            {
                springRb.linearVelocity += new Vector2(0, jumpForce); 
                _currentJumpsInRow++;
            }
            
        }

        private IEnumerator Dash()
        {
            _canDash = false;
            _isDashing = true;
            
            var originalGravities = new float[this._bodyRigidbodies.Length];
            for (var i = 0; i < originalGravities.Length; i++)
                originalGravities[i] = _bodyRigidbodies[i].gravityScale;

            foreach (var t in _bodyRigidbodies)
                t.gravityScale = 0;

            foreach (var t in _bodyRigidbodies)
                t.linearVelocity = new Vector2(transform.localScale.x * dashingPower * _lastDirection, 0f);

            trailRenderer.emitting = true; 
            
            yield return new WaitForSeconds(dashingTime);
            
            trailRenderer.emitting = false;
            for (int i = 0; i < originalGravities.Length; i++) 
                _bodyRigidbodies[i].gravityScale = originalGravities[i]; 

            _isDashing = false;
            
            yield return new WaitForSeconds(dashingCoolDown);
            _canDash = true; 
            
        }


        private void CharacterWobbleOnWalk()
        {
            var contraction = Mathf.Abs(Mathf.Sin(Time.time)) *  0.1f; 
            
            foreach (var springJoint2D in _bodySpringJoints)
            {
                springJoint2D.distance *= contraction; 
            }
            
        }

        private void CompressSprings()
        {
            foreach (var spring2D in _bodySpringJoints)
            {
                spring2D.distance *= springCompressionFactor; 

            }
        }

        private void RestoreSprings()
        {
            foreach (var spring2D in _bodySpringJoints)
            {
                spring2D.distance *= springCompressionFactor; 

            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.red; 
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        }

    }
}