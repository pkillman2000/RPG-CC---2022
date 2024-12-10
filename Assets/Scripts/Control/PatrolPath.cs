using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField]
        private Color _color = Color.red;

        [SerializeField]
        private float _waypointSize = .3f;

        void Start()
        {
        }


        void Update()
        {

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            int j;

            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.DrawSphere(GetWaypoint(i), _waypointSize);

                j = GetNextWaypointIndex(i);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public Vector3 GetWaypoint (int waypointIndex)
        {
            return transform.GetChild(waypointIndex).position;
        }

        public int GetNextWaypointIndex(int waypointIndex)
        {
            if(waypointIndex < transform.childCount - 1)
            {
                return waypointIndex + 1;
            }
            else
            {
                return 0;
            }
        }
    }
}