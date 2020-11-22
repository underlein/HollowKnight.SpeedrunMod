using System.Collections;
using JetBrains.Annotations;
using Vasi;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace SpeedrunMod.Modules {
    [UsedImplicitly]
    public class FixRng : FauxMod {

        public override void Initialize() {
            Unload();

            On.PlayMakerFSM.OnEnable += ModifyFsm;
        }

        public override void Unload() {
            On.PlayMakerFSM.OnEnable -= ModifyFsm;
        }

        private static void ModifyFsm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            switch (self.FsmName) {
                // first spit baldurs
                case "Blocker Control" when self.name == "Blocker": {
                    self.GetState("Idle").ChangeTransition("ATTACK", "Can Roller?");
                    break;
                }
                // gruz drop room
                case "Bouncer Control" when self.name.StartsWith("Fly") && GameManager.instance.sceneName == "Crossroads_07": {
                    HeroController.instance.StartCoroutine(FixGruzDrop(self));
                    break;
                }
            }

            orig(self);
        }

        private static IEnumerator FixGruzDrop(PlayMakerFSM fsm) {
            yield return null;

            fsm.FsmVariables.GetFsmFloat("Angle").Value = fsm.name switch {
                "Fly" => 167,
                "Fly 1" => 117,
                "Fly 2" => 56, // shoutouts
                "Fly 3" => 256,
                "Fly 4" => 130,
                "Fly 5" => 158,
                "Fly 6" => 98,
                "Fly 7" => 334,
                "Fly 10" => 0,
                _ => 270
            };
        }

    }
}