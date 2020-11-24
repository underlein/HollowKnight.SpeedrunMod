using System.Collections;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vasi;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace SpeedrunMod.Modules {
    [UsedImplicitly]
    public class FixRng : FauxMod {

        public override void Initialize() {
            Unload();

            On.PlayMakerFSM.OnEnable += ModifyFsm;
            USceneManager.activeSceneChanged += SceneChanged;
        }

        public override void Unload() {
            On.PlayMakerFSM.OnEnable -= ModifyFsm;
            USceneManager.activeSceneChanged -= SceneChanged;
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
                // remove shade spells
                case "Shade Control" when self.name.StartsWith("Hollow Shade"): {
                    self.GetState("Init").RemoveAction(6);
                    self.FsmVariables.FindFsmInt("SP").Value = 0;
                    break;
                }
                // make furious vengeflies trigger later
                case "Control" when self.name.StartsWith("Angry Buzzer"): {
                    self.gameObject.GetComponentInChildren<AlertRange>().transform.localScale = new Vector3(10, 10, 1.3f);
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
                _ => 0
            };
        }

        private static void SceneChanged(Scene from, Scene to) {
            switch (to.name) {
                case "Abyss_02": {
                    HeroController.instance.StartCoroutine(Abyss_02());
                    break;
                }
                case "Tutorial_01": {
                    HeroController.instance.StartCoroutine(Tutorial_01());
                    break;
                }
            }
        }

        private static IEnumerator Abyss_02() {
            yield return null;

            string[] names = {"Ruins Flying Sentry", "Ruins Flying Sentry Javelin", "Ruins Flying Sentry (1)"};

            foreach (string name in names) {
                RemoveIdleBuzzSpeed(GameObject.Find(name).LocateMyFSM("Flying Sentry " + (name.EndsWith("Javelin") ? "Javelin" : "Nail")).GetState("Idle Buzz").GetAction<IdleBuzz>());
            }
        }

        private static IEnumerator Tutorial_01() {
            yield return null;

            GameObject buzzer = GameObject.Find("Buzzer");
            RemoveIdleBuzzSpeed(buzzer.LocateMyFSM("chaser").GetState("Idle").GetAction<IdleBuzz>());
            buzzer.transform.position = new Vector3(88, 32.4f);
            
            RemoveIdleBuzzSpeed(GameObject.Find("Buzzer 1").LocateMyFSM("chaser").GetState("Idle").GetAction<IdleBuzz>());
        }

        private static void RemoveIdleBuzzSpeed(IdleBuzz idleBuzz) {
            idleBuzz.speedMax = 0;
            idleBuzz.accelerationMax = 0;
        }

    }
}