using UnityEngine;
using UnityEngine.Serialization;

namespace PennyPixel.Scripts
{
    public class PlayerPlatformerController : PhysicsObject
    {
        [SerializeField] private float maxSpeed = 7;
        [SerializeField] private float jumpTakeOffSpeed = 7;

        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        
        private static readonly int kGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int kVelocityX = Animator.StringToHash("VelosityX"); 
        private static readonly int kIsHit = Animator.StringToHash("IsHit");
        private static readonly int IsSeet = Animator.StringToHash("IsSeet");

        // Use this for initialization
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        protected override void ComputeVelocity()
        {
            var move = Vector2.zero;

            move.x = Input.GetAxis("Horizontal");

            _animator.SetBool(kIsHit, Input.GetKeyDown(KeyCode.E));
            
            _animator.SetBool(IsSeet, Input.GetKey(KeyCode.S));
            

            if (Input.GetButtonDown("Jump") && Grounded)
            {
                Velocity.y = jumpTakeOffSpeed;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                if (Velocity.y > 0)
                {
                    Velocity.y = Velocity.y * 0.5f;
                }
            }

            if (move.x > 0.01f)
            {
                if (_spriteRenderer.flipX)
                {
                    _spriteRenderer.flipX = false;
                }
            }
            else if (move.x < -0.01f)
            {
                if (_spriteRenderer.flipX == false)
                {
                    _spriteRenderer.flipX = true;
                }
            }

            _animator.SetBool(kGrounded, Grounded);
            if (_animator.GetBool(IsSeet)) 
                return;
            _animator.SetFloat(kVelocityX, Mathf.Abs(Velocity.x) / maxSpeed);
            TargetVelocity = move * maxSpeed;


        }
    }
}