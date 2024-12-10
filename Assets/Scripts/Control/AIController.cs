using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        private float _chaseDistance = 5f;
        [SerializeField]
        private PatrolPath _patrolPath;
        [SerializeField]
        private float _waypointTolerance = .1f;
        [SerializeField]
        private Color _gizmoColor = Color.red;        

        private Fighter _fighter;
        private GameObject _player;
        private Health _health;
        private Mover _mover;
        private ActionScheduler _actionScheduler;

        [SerializeField]
        private float _waypointDwellTime = 2f;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private Vector3 _guardPosition;
        private int _currentWaypointIndex = 0;

        [SerializeField]
        private float _suspicionTime = 5f;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;

        void Start()
        {
            _fighter = GetComponent<Fighter>();
            if(_fighter == null)
            {
                Debug.LogError("Fighter is Null!");
            }

            _player = GameObject.FindWithTag("Player");
            if(_player == null)
            {
                Debug.LogError("Player not Found!");
            }

            _health = GetComponent<Health>();
            if(_health == null)
            {
                Debug.LogError("Health is Null!");
            }

            _mover = GetComponent<Mover>();
            if(_mover == null)
            {
                Debug.LogError("Mover is Null!");
            }

            _actionScheduler = GetComponent<ActionScheduler>();
            if(_actionScheduler == null)
            {
                Debug.LogError("Action Scheduler is Null!");
            }

            _guardPosition = this.transform.position;
        }


        void Update()
        {
            if (_health.IsDead()) return;

            if (InAttackRangeOfPlayer() && _fighter.CanAttack(_player)) //Attack State
            {
                AttackBehavior();
                _timeSinceLastSawPlayer = 0;
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime) // Suspicion State
            {
                SuspicionBehavior();
            }
            else // Guard/Patrol State
            {
                PatrolBehavior();
            }
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            _fighter.Attack(_player);
        }

        private void SuspicionBehavior()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = _guardPosition;

            if(_patrolPath != null)
            {
                if(AtWaypoint())
                {
                        _timeSinceArrivedAtWaypoint = 0;
                        CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedAtWaypoint > _waypointDwellTime) // Dwell at waypoint before moving
            {
                _mover.StartMoveAction(nextPosition);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint =  Vector3.Distance(this.transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = _patrolPath.GetNextWaypointIndex(_currentWaypointIndex);
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(_player.transform.position, this.transform.position);
            return distanceToPlayer < _chaseDistance;
        }

        /*
         * Called by Unity
         * This will display a gizmo when this
         * gameobject is selected.
         * You can turn gizmos on and off in the 
         * Game window by clicking the Gizmos button
         * in the top right corner.
        */
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireSphere(this.transform.position, _chaseDistance);
        }
    }
}