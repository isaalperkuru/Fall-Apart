using Game.Inventories;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        //Config Data
        [Tooltip("How far can the pickups be scattered from dropper.")]
        [SerializeField] float scatterDistance = 1;
        [SerializeField] DropLibrary dropLibrary;

        //Constants
        const int ATTEMPTS = 30;

        public void RandomDrop()
        {
            var baseStats = GetComponent<BaseStats>();
            
            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach(var drop in drops)
            {
                DropItem(drop.item, drop.number);
            }  
        }
        protected override Vector3 GetDropLocation()
        {
            //We might to try more than once to get on the NavMesh
            for(int i=0; i < ATTEMPTS; i++)
            {
                Vector3 randomPoint = transform.position +
                    Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;
        }
    }
}


