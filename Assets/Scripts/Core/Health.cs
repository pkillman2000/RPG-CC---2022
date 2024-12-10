using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core  
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private float _healthPoints = 100f;

        private Animator _animator;
        private CapsuleCollider _capsuleCollider;
        private ActionScheduler _actionScheduler;

        private bool _isDead = false;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animator is Null!");
            }

            _capsuleCollider = GetComponent<CapsuleCollider>();
            if(_capsuleCollider == null)
            {
                Debug.LogError("Capsule Collider is Null!");
            }

            _actionScheduler = GetComponent<ActionScheduler>();
            if(_actionScheduler == null)
            {
                Debug.LogError("Action Scheduler is Null!");
            }
        }
        public void TakeDamage(float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);

            if(!_isDead && _healthPoints == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _animator.SetTrigger("die");

            /*
             * Rotate collider to horizontal.
             * Move collider to 0 to match the ground.
             * Change the radius to match the body better.
             * The radius was larger while still alive to
             * make it easier to click on the enemy vs. ground.
             * This collider can be used for looting.
            */
            _capsuleCollider.direction = 0;
            _capsuleCollider.center = new Vector3 (0, 0, 0);
            _capsuleCollider.radius = 0.25f;

            _actionScheduler.CancelCurrentAction();
            _isDead = true;
        }

        public bool IsDead()
        {
            return _isDead;
        }
    }
}