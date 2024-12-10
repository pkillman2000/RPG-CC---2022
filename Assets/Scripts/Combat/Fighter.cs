using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField]
        private float _weaponRange = 2f;
        [SerializeField]
        private float _weaponDamage = 10f;
        [SerializeField]
        private float _timeBetweenAttacks = 1f;

        private Health _target;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;

        private float _timeSinceLastAttack = Mathf.Infinity;

        void Start()
        {
            _mover = GetComponent<Mover>();
            if(_mover == null )
            {
                Debug.LogError("Mover is null!");
            }

            _actionScheduler = GetComponent<ActionScheduler>();
            if(_actionScheduler == null )
            {
                Debug.LogError("Action Scheduler is Null!");
            }

            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animator is Null!");
            }
        }

        void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if (_target == null)
            {
                return;
            }
            if (_target.IsDead())
            {
                return;
            }

            if (!GetIsInRange())
            {
                _mover.MoveTo(_target.transform.position);
            }
            else
            {
                _mover.Cancel();
                AttackBehavior();
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(this.transform.position, _target.transform.position) < _weaponRange;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }

            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }

        private void AttackBehavior()
        {
            transform.LookAt(_target.transform.position);

            if (_timeSinceLastAttack > _timeBetweenAttacks)
            {
                TriggerAttack();
                _timeSinceLastAttack = 0f;
            }
        }

        /* 
         * If the player is moved while attacking
         * the stopAttack trigger can be left
         * in an ON state.  This means that
         * the player will not attack during the
         * first cycle because the trigger is ON.
         * Using ResetTrigger will return the trigger
         * to it's default state.
        */
        private void TriggerAttack()
        {
            _animator.ResetTrigger("stopAttack");
            _animator.SetTrigger("attack");
        }


        /*
         * This is a animation event from 
         * Unarmed-Attack-L3
        */
        public void Hit()
        {
            /*
             * The attack can be cancelled before the 'hit' 
             * event is reached in the animation.  This will
             * make the target null before the animation is complete.
             */
            if (_target == null)
            {
                return;
            }
            _target.TakeDamage(_weaponDamage);
        }

        public void Cancel()
        {
            StopAttack();
            _target = null;
        }

        private void StopAttack()
        {
            _animator.SetTrigger("stopAttack");
            _animator.ResetTrigger("attack");
        }
    }
}