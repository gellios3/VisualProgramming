using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PennyPixel.Scripts
{
    public class PhysicsObject : MonoBehaviour
    {
        [FormerlySerializedAs("minGroundNormalY")]
        public float MinGroundNormalY = .65f;

        [FormerlySerializedAs("gravityModifier")]
        public float GravityModifier = 1f;

        protected Vector2 TargetVelocity;
        protected bool Grounded;
        private Vector2 _groundNormal;
        private Rigidbody2D _rb2D;
        protected Vector2 Velocity;
        private ContactFilter2D _contactFilter;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
        private readonly List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);


        private const float MinMoveDistance = 0.001f;
        private const float ShellRadius = 0.01f;

        private void OnEnable()
        {
            _rb2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _contactFilter.useTriggers = false;
            _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            _contactFilter.useLayerMask = true;
        }

        private void Update()
        {
            TargetVelocity = Vector2.zero;
            ComputeVelocity();
        }

        protected virtual void ComputeVelocity()
        {
        }

        private void FixedUpdate()
        {
            Velocity += GravityModifier * Physics2D.gravity * Time.deltaTime;
            Velocity.x = TargetVelocity.x;

            Grounded = false;

            var deltaPosition = Velocity * Time.deltaTime;

            var moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);

            var move = moveAlongGround * deltaPosition.x;

            Movement(move, false);

            move = Vector2.up * deltaPosition.y;

            Movement(move, true);
        }

        private void Movement(Vector2 move, bool yMovement)
        {
            var distance = move.magnitude;

            if (distance > MinMoveDistance)
            {
                var count = _rb2D.Cast(move, _contactFilter, _hitBuffer, distance + ShellRadius);
                _hitBufferList.Clear();
                for (var i = 0; i < count; i++)
                {
                    _hitBufferList.Add(_hitBuffer[i]);
                }

                foreach (var item in _hitBufferList)
                {
                    var currentNormal = item.normal;
                    if (currentNormal.y > MinGroundNormalY)
                    {
                        Grounded = true;
                        if (yMovement)
                        {
                            _groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }

                    var projection = Vector2.Dot(Velocity, currentNormal);
                    if (projection < 0)
                    {
                        Velocity = Velocity - projection * currentNormal;
                    }

                    var modifiedDistance = item.distance - ShellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }

            _rb2D.position = _rb2D.position + move.normalized * distance;
        }
    }
}