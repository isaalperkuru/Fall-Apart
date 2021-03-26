using Game.Inventories;
using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Potion"))]
    public class HealthPotion : ActionItem
    {
        [SerializeField] float healAmount = 30f;

        public override void Use(GameObject user)
        {
            Health health = user.GetComponent<Health>();
            if (!health) return; 
            if(health.GetMaxHealthPoints() != health.GetHealthPoints())
            {
                health.Heal(healAmount);
            }
        }
    }
}

