using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;

namespace RPG.Core
{
    public class ActionSchedular : MonoBehaviour
    {
        IAction currentAction;
        //Creating this script for not cycling between movement and combat
        public void StartAction(IAction action)
        {
            if (currentAction == action) return;
            if (currentAction != null)
            {
                currentAction.Cancel();
            }
            
            currentAction = action;
        }

        
    }
}
