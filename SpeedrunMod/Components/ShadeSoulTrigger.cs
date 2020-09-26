using System;
using GlobalEnums;
using UnityEngine;

namespace SpeedrunMod.Components {
    public class ShadeSoulTrigger : MonoBehaviour {

        public Action OnShadeSoulHit { get; set; }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.layer != (int) PhysLayers.HERO_ATTACK)
                return;

            if (!other.gameObject.name.Contains("Fireball2"))
                return;
            
            OnShadeSoulHit();
        }
    }
}