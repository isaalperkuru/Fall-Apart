using UnityEngine;

namespace RPG.Movement
{
    internal class SerializeableVector3
    {
        private Vector3 position;

        public SerializeableVector3(Vector3 position)
        {
            this.position = position;
        }
    }
}