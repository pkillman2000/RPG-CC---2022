using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Combat;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField]
        private float _destinationTolerance = .2f;

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Fighter _fighter;
        private ActionScheduler _actionScheduler;
        private Health _health;

        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_navMeshAgent == null)
            {
                Debug.LogError("Nav Mesh Agent is Null!");
            }

            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animator is Null!");
            }

            _fighter = GetComponent<Fighter>();
            if (_fighter == null)
            {
                Debug.LogError("Fighter is Null!");
            }

            _actionScheduler = GetComponent<ActionScheduler>();
            if(_actionScheduler == null)
            {
                Debug.LogError("Action Scheduler is Null!");
            }

            _health = GetComponent<Health>();
            if(_health == null)
            {
                Debug.LogError("Health is Null!");
            }
        }

        void Update()
        {
            /*
             * Disables the NavMesh Agent if the character 
             * is dead.  This allows the other characters to
             * move over the body.
            */
            _navMeshAgent.enabled = !_health.IsDead();
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination)
        {
            _actionScheduler.StartAction(this);
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            if (Vector3.Distance(this.transform.position, destination) > _destinationTolerance)
            {
                _navMeshAgent.isStopped = false;
                _navMeshAgent.destination = destination;
                transform.LookAt(destination);
            }
            else
            {
                Cancel();
            }
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            // Get global velocity from NavMesh
            Vector3 velocity = _navMeshAgent.velocity;
            // Convert global velocity to local velocity
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            // We only want the forward, Z, speed
            float speed = localVelocity.z;
            /* forwardSpeed is a float in the BlendTree
             * that sets the forward speed of the animation
            */
            _animator.SetFloat("forwardSpeed", speed);
        }
    }
}