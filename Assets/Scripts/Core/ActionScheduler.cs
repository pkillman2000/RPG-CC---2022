using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        private IAction _currentAction;

        /*
         * Substitution Principle - 
        */
        public void StartAction(IAction action)
        {
            if (action == _currentAction)
            {
                return;
            }

            if(_currentAction != null)
            {
                _currentAction.Cancel();
            }
            _currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}