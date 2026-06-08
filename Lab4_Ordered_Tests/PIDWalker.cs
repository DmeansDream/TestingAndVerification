using System;
using UnityEngine;
using VContainer;
using Other;

namespace ScriptsToTest
{
    public class PIDWalker : MonoBehaviour
    {
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] Collider _collider;
        [SerializeField] private float maxForce = 200f;

        public PIDController _controller;

        private float destination;
        private bool shouldBeMoving = false;

        private Vector3 pos;
        
        public float GetLinearVel =>  _rigidbody.linearVelocity.magnitude;

        [Inject]
        private void Construct(PIDController pidController)
        {
            _controller = pidController;
            _controller.Initialize(0, 0, 0, DerivativeType.Velocity);
        }

        public void Restart()
        {
            this.transform.position = new Vector3(0,0,0);
            shouldBeMoving = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            destination = 0;
            _controller.Reset();
        }

        public void SendWalkerTo(float x)
        {
            shouldBeMoving = true;
            destination = x;
        }

        private void FixedUpdate()
        {
            pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            
            if (shouldBeMoving)
            {
                float pidCoef = _controller.Update(Time.fixedDeltaTime, this.transform.position.x, destination);
                
                float resultForce = Mathf.Clamp(maxForce * pidCoef, -maxForce, maxForce);
                _rigidbody.AddForce(Vector3.right * resultForce, ForceMode.Force);
                Debug.Log($"{_rigidbody.linearVelocity.x} , {pidCoef}, error : {destination - transform.position.x},");
                Events.OnVelocityChange.Invoke(_rigidbody.linearVelocity.x);
                
                Debug.DrawRay(pos, Vector3.right * (maxForce * pidCoef) * 0.1f, Color.white);
                
                Debug.DrawRay(pos + Vector3.up * 0.2f, Vector3.right * _controller.LastP, Color.green);
                Debug.DrawRay(pos + Vector3.up * 0.4f, Vector3.right * _controller.LastD, Color.blue);
                Debug.DrawRay(pos + Vector3.up * 0.6f, Vector3.right * _controller.LastI, Color.yellow);

                // Linear velocity
                Debug.DrawRay(pos + Vector3.up * 0.8f, Vector3.right * _rigidbody.linearVelocity.x, Color.red);
                
                if (Mathf.Abs(destination - transform.position.x) < 0.01f && _rigidbody.linearVelocity.magnitude < 0.01f)
                {
                    shouldBeMoving = false;
                }
            }
        }
    }
}