using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health target = null;
        [SerializeField] float speed = 1f;
        private Collider targetCollider;
        float damage = 0;
        // Update is called once per frame
        void Update()
        {
            if (target == null) return;

            transform.LookAt(GetVector());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }

        private Vector3 GetVector()
        {
            if (!targetCollider) {
                return target.transform.position;
            }
            else
            {
                targetCollider = target.GetComponent<Collider>();
            }
            return targetCollider.bounds.center;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

