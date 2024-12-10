using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;

        void Start()
        {
            _mover = GetComponent<Mover>();
            if (_mover == null)
            {
                Debug.LogError("Mover is Null!");
            }

            _fighter = GetComponent<Fighter>();
            if (_fighter == null)
            {
                Debug.LogError("Fighter is Null!");
            }

            _health = GetComponent<Health>();
            if (_health == null)
            {
                Debug.LogError("Health is Null!");
            }
        }


        void Update()
        {
            if (_health.IsDead()) return;

            if (InteractWithCombat())
            {
                return;
            }
            if (InteractWithMovement())
            {
                return;
            }
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits;
            Ray ray;

            /*
             * Raycasting needs to execute every frame because
             * the cursor will change depending on what 
             * it is hovering over. (cursor affordance)
            */
            ray = GetMouseRay();
            hits = Physics.RaycastAll(ray);

            // Sorts hits array from closest to farthest
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();

                if (target == null) continue;

                if (!_fighter.CanAttack(target.gameObject))
                {
                    continue;
                }

                if (Input.GetMouseButton(0))
                {
                    _fighter.Attack(target.gameObject);
                }
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            Ray ray;
            RaycastHit hit;

            ray = GetMouseRay();
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            // Creates a ray from main camera to mouse cursor
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}