using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        Vector3 guardPosition;
        private void Start()
        {
            fighter = GetComponent<Fighter>();
            //find if it is player from object's tag
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            guardPosition = transform.position;

        }
        private void Update()
        {
            if (health.IsDead()) return;
            // if the enemy is in chase distance to player and if we can attack the player (not dead)
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                 fighter.Attack(player);
            }
            else
            {
                mover.StartMoveAction(guardPosition);
            }
        }

        private bool InAttackRangeOfPlayer()
        {
            //distance enemy to player
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;
        }

        //Called by Unity, this shows you sight of enemy when you in range.
        private void OnDrawGizmosSelected()
        {
            //red gizmos
            Gizmos.color = Color.red;
            //draw spyhere
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

