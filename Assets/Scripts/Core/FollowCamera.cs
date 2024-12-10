using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This acts as a target for the camera.
 * It uses the Player's position but not
 * the Player's rotation.  Camera will always
 * look at the player from the same direction.
 */

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        void Start()
        {
            GameObject player = GameObject.FindWithTag("Player");
            _target = player.transform;
        }

        /*
         * This allows movement/animation to complete on this frame
         * before moving the camera position
        */
        void LateUpdate()
        {
            this.transform.position = _target.position;
        }
    }
}