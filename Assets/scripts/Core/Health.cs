using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoints = 20f;
        bool isDead = false;

        public bool IsDead()
        {
            return isDead;
        }
        // Update is called once per frame
        void Update()
        {

        }
        public void TakeDamage(float damage)
        {
                healthPoints = Mathf.Max(healthPoints - damage, 0);
                print(healthPoints);
                if (healthPoints == 0)
                {
                    Die();
                }

            void Die()
            {
                if (isDead) return;

                isDead = true;
                GetComponent<Animator>().SetTrigger("die");
                GetComponent<ActionSchedular>().CancelCurrentAction();
            }
        }
        }
    }


