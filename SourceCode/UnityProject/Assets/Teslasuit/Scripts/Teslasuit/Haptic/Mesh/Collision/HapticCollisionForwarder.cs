using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslasuitAPI
{

    class HapticCollisionForwarder : MonoBehaviour
    {
        HapticCollisionEventsSource source;

        public void SetCollisionEventSource(HapticCollisionEventsSource source)
        {
            this.source = source;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (source != null)
                source.ProcessCollision(new CollisionWithType(collision, CollisionType.ENTER));
        }

        private void OnCollisionStay(Collision collision)
        {
            if (source != null)
                source.ProcessCollision(new CollisionWithType(collision, CollisionType.STAY));
        }

        private void OnCollisionExit(Collision collision)
        {
            if (source != null)
                source.ProcessCollision(new CollisionWithType(collision, CollisionType.EXIT));
        }

    }
}
