using System;
using GlobalEnums;
using SpeedrunMod.Modules;
using UnityEngine;

namespace SpeedrunMod.Components {
    
    // also triggers for shade soul
    // if you want to filter for vs make sure it's "Fireball" and not "Fireball2"
    public class VengefulSpiritTrigger : MonoBehaviour {

        public Action<GameObject> OnVengefulSpiritHit { get; set; }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.layer != (int) PhysLayers.HERO_ATTACK)
                return;

            if (!other.gameObject.name.Contains("Fireball"))
                return;

            OnVengefulSpiritHit(other.gameObject);
        }
    }
}