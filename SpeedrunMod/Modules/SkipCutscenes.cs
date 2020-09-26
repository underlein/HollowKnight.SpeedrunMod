using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using Modding;
using SpeedrunMod.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vasi;
using UObject = UnityEngine.Object;

namespace SpeedrunMod.Modules {
    [UsedImplicitly]
    public class SkipCutscenes : FauxMod {

        private static readonly string[] DREAMERS = {"Deepnest_Spider_Town", "Fungus3_archive_02", "Ruins2_Watcher_Room"};

        // Boss cutscenes, mostly.
        private static readonly string[] PD_BOOLS = {
            nameof(PlayerData.hasCharm),
            nameof(PlayerData.unchainedHollowKnight),
            nameof(PlayerData.encounteredMimicSpider),
            nameof(PlayerData.infectedKnightEncountered),
            nameof(PlayerData.mageLordEncountered),
            nameof(PlayerData.mageLordEncountered_2),
            nameof(PlayerData.encounteredHornet),
            nameof(PlayerData.enteredGGAtrium)
        };

        private static readonly Dictionary<string, int> PD_INTS = new Dictionary<string, int> {
            {nameof(PlayerData.hornetGreenpath), 4}
        };

        public override void Initialize() {
            On.CinematicSequence.Begin += CinematicBegin;
            On.FadeSequence.Begin += FadeBegin;
            On.AnimatorSequence.Begin += AnimatorBegin;
            On.InputHandler.SetSkipMode += OnSetSkip;
            On.GameManager.BeginSceneTransitionRoutine += BeginSceneTransition;
            On.HutongGames.PlayMaker.Actions.EaseColor.OnEnter += FastEaseColor;
            On.GameManager.FadeSceneInWithDelay += NoFade;
            ModHooks.Instance.NewGameHook += OnNewGame;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneChanged;
            On.PlayMakerFSM.OnEnable += ModifyFsm;
        }

        public override void Unload() {
            On.CinematicSequence.Begin -= CinematicBegin;
            On.FadeSequence.Begin -= FadeBegin;
            On.AnimatorSequence.Begin -= AnimatorBegin;
            On.InputHandler.SetSkipMode -= OnSetSkip;
            On.GameManager.BeginSceneTransitionRoutine -= BeginSceneTransition;
            On.HutongGames.PlayMaker.Actions.EaseColor.OnEnter -= FastEaseColor;
            On.GameManager.FadeSceneInWithDelay -= NoFade;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneChanged;
            ModHooks.Instance.NewGameHook -= OnNewGame;
            On.PlayMakerFSM.OnEnable -= ModifyFsm;
        }

        private static void OnNewGame() {
            foreach (string @bool in PD_BOOLS) {
                PlayerData.instance.SetBool(@bool, true);
            }

            foreach ((string pdInt, int pdValue) in PD_INTS) {
                PlayerData.instance.SetInt(pdInt, pdValue);
            }
        }

        private static IEnumerator NoFade(On.GameManager.orig_FadeSceneInWithDelay orig, GameManager self, float delay) {
            yield return orig(self, 0);
        }

        private static void FastEaseColor(On.HutongGames.PlayMaker.Actions.EaseColor.orig_OnEnter orig, EaseColor self) {
            if (self.Owner.name == "Blanker White" && Math.Abs(self.time.Value - 0.3) < .05) {
                self.time.Value = 0.066f;
            }

            orig(self);
        }

        private static void ModifyFsm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            switch (self.FsmName) {
                // vengful spirit pickup
                case "Get Fireball" when self.name == "Knight Get Fireball":
                    self.GetState("Start").GetAction<Wait>().time = 0;
                    self.GetState("Rumble").GetAction<Wait>().time = 0;

                    self.GetState("Start").RemoveAllOfType<iTweenMoveBy>();
                    self.GetState("Rumble").RemoveAllOfType<iTweenMoveBy>();
                    self.GetState("Get").RemoveAllOfType<iTweenMoveBy>();
                    break;
                case "Check Fall" when self.name == "Knight Cutscene Animator":
                    self.GetState("Black").RemoveAction<Wait>();
                    self.GetState("Fade Back").GetAction<Wait>().time = 3;
                    break;

                // shade soul pickup
                case "Get Fireball" when self.name == "Knight Get Fireball Lv2":
                    self.GetState("Start").GetAction<Wait>().time = 0;
                    self.GetState("Start").ChangeTransition("FINISHED", "Get PlayerData");

                    self.GetState("Start").RemoveAllOfType<iTweenMoveBy>();
                    self.GetState("Rumble").RemoveAllOfType<iTweenMoveBy>();
                    self.GetState("Get").RemoveAllOfType<iTweenMoveBy>();
                    break;

                // howling wraiths pickup
                case "Get Scream" when self.name == "Knight Get Scream":
                    self.GetState("Start").GetAction<Wait>().time = 0.1f;
                    self.GetState("Fall").GetAction<Wait>().time = 0.3f;

                    self.GetState("Start").ChangeTransition("FINISHED", "Get");

                    self.GetState("Start").RemoveAction<AudioPlayerOneShotSingle>();
                    self.GetState("Start").RemoveAllOfType<iTweenMoveBy>();
                    self.GetState("Get").RemoveAllOfType<iTweenMoveBy>();
                    self.gameObject.GetComponentsInChildren<Transform>().First(transform => transform.name == "Orbs").gameObject.SetActive(false);
                    break;

                // shade cloak pickup
                case "Get Shadow Dash" when self.name == "Dish Plat":
                    self.GetState("Fall").GetAction<Wait>().time = 1;
                    self.GetState("Fall").ChangeTransition("FINISHED", "UI Msg");
                    break;

                // desolate dive pickup
                case "corpse" when self.name == "Corpse Mage Lord 2(Clone)":
                    self.GetState("Steam").GetAction<Wait>().time = 0;
                    self.GetState("Ready").GetAction<Wait>().time = 0;
                    self.GetState("Pause").GetAction<Wait>().time = 0;
                    self.GetState("Land").GetAction<Wait>().time = 0;

                    self.gameObject.GetComponentsInChildren<Transform>().Where(transform => transform.name == "Soul Breath").ToList().ForEach(transform => transform.gameObject.SetActive(false));
                    break;
                case "Pickup" when self.name == "Quake Pickup":
                    self.GetState("Wait").ChangeTransition("FINISHED", "Appear");
                    self.GetState("Wait").GetAction<Wait>().time = 0.5f;
                    break;
                case "Get Quake" when self.name == "Knight Get Quake":
                    self.GetState("Start").GetAction<Wait>().time = 0.1f;
                    self.GetState("Fall").GetAction<Wait>().time = 0.3f;

                    self.GetState("Start").ChangeTransition("FINISHED", "Get");

                    self.GetState("Start").RemoveAction<AudioPlayerOneShotSingle>();
                    self.GetState("Start").RemoveAllOfType<iTweenMoveBy>();
                    self.GetState("Get").RemoveAllOfType<iTweenMoveBy>();
                    self.gameObject.GetComponentsInChildren<Transform>().First(transform => transform.name == "Orbs").gameObject.SetActive(false);
                    break;

                // descending dark pickup
                case "Control" when self.name == "Crystal Shaman":
                    self.GetState("Land Hero").ChangeTransition("FINISHED", "Get PlayerData 2");
                    break;

                // dream nail cutscene enter
                case "Conversation Control" when self.name == "Dreamer Plaque Inspect":
                    self.GetState("Hero Anim").ChangeTransition("FINISHED", "Map Msg?");
                    break;
                case "Control" when self.name == "Dreamer Scene 2":
                    self.GetState("Fade Out").GetAction<Wait>().time = 1;
                    self.GetState("Take Control").ChangeTransition("FINISHED", "Fade Out");
                    self.GetState("Fade Out").ChangeTransition("FINISHED", "Set Compass Point");
                    break;

                // gruz mother
                case "corpse" when self.name == "Corpse Big Fly 1(Clone)":
                    self.GetState("Init").GetAction<Wait>().time = 0;
                    self.GetState("Steam").GetAction<Wait>().time = 0;
                    self.GetState("Ready").GetAction<Wait>().time = 0;
                    break;
                case "burster" when self.name == "Corpse Big Fly Burster(Clone)":
                    self.GetState("Landed").RemoveAction<Wait>();
                    self.GetState("Stop Emit").RemoveAction<Wait>();
                    self.GetState("Stop").RemoveAction<Wait>();
                    self.GetState("Gurg 1").RemoveAction<Wait>();
                    self.GetState("Burst").RemoveAction<Wait>();
                    self.GetState("State 1").RemoveAction<Wait>();
                    break;

                // dreamer cutscene skip
                case "Control" when self.name == "Dreamer NPC":
                    self.GetState("Cinematic").RemoveAction<FindGameObject>();
                    self.GetState("Cinematic").RemoveAction<SendMessage>();
                    self.GetState("Cinematic").RemoveAction<FadeAudio>();
                    self.GetState("Cinematic").AddAction(new NextFrameEvent {
                        sendEvent = self.GetState("Cinematic").Fsm.GetEvent("CINEMATIC END")
                    });

                    self.GetState("Set Hegemol").ChangeTransition("FINISHED", "Return");
                    self.GetState("Set Lurien").ChangeTransition("FINISHED", "Return");
                    self.GetState("Set Monomon").ChangeTransition("FINISHED", "Return");
                    break;

                // spawn soul master on the right for all skills fight
                case "Mage Lord" when self.name == "Mage Lord":
                    FsmState setEntryPoint = self.GetState("Set Entry Point");

                    // remove knight position check and left spawn point
                    for (int i = 0; i < 4; i++) {
                        setEntryPoint.RemoveAction(2);
                    }

                    break;
            }

            orig(self);
        }

        private static void SceneChanged(Scene from, Scene to) {
            var hc = HeroController.instance;

            if (hc == null) return;

            hc.StartCoroutine(AbsRadSkip(to));
            hc.StartCoroutine(PureVesselPrimeSkip(to));
            hc.StartCoroutine(StatueWait(to));
            hc.StartCoroutine(StagCutscene());
            hc.StartCoroutine(AbyssShriekPickup(to));
            hc.StartCoroutine(KingsBrandHornet(to));
            hc.StartCoroutine(KingsBrandAvalanche(to));
            hc.StartCoroutine(BlackEgg(to));
            hc.StartCoroutine(DreamerReturn(to));
        }

        private static IEnumerator StagCutscene() {
            yield return null;

            GameObject stag = GameObject.Find("Stag");

            if (stag == null)
                yield break;

            PlayMakerFSM ctrl = stag.LocateMyFSM("Stag Control");

            // Remove the wait on the stag animation start, and before being able to interact.
            ctrl.GetState("Arrive Pause").RemoveAction<Wait>();
            ctrl.GetState("Activate").RemoveAction<Wait>();

            var anim = stag.GetComponent<tk2dSpriteAnimator>();

            // Speed up the actual arrival animation.
            anim.GetClipByName("Arrive").fps = 600;

            // Speed up the grate coming up, mostly a thing because of randomizer.
            GameObject grate = ctrl.GetState("Open Grate").GetAction<Tk2dPlayAnimationWithEvents>().gameObject.GameObject.Value;

            if (grate == null)
                yield break;

            var grateAnim = grate.GetComponent<tk2dSpriteAnimator>();

            grateAnim.GetClipByName("Grate_disappear").fps = 600;
        }

        private static IEnumerator StatueWait(Scene to) {
            if (to.name != "GG_Workshop") yield break;

            foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>().Where(x => x.FsmName == "GG Boss UI")) {
                fsm.GetState("On Left").ChangeTransition("FINISHED", "Dream Box Down");
                fsm.GetState("On Right").ChangeTransition("FINISHED", "Dream Box Down");

                fsm.GetState("Dream Box Down").InsertAction(0, fsm.GetAction<SetPlayerDataString>("Impact", 2));
            }
        }

        private static IEnumerator PureVesselPrimeSkip(Scene to) {
            if (to.name != "GG_Hollow_Knight") yield break;

            yield return null;

            PlayMakerFSM control = GameObject.Find("HK Prime").LocateMyFSM("Control");

            control.GetState("Init").ChangeTransition("FINISHED", "Intro Roar");
            control.GetAction<Wait>("Intro 2", 3).time = 0.01f;
            control.GetAction<Wait>("Intro 1", 0).time = 0.01f;
            control.GetAction<Wait>("Intro Roar", 7).time = 1f;
        }

        private static IEnumerator AbsRadSkip(Scene to) {
            if (to.name != "GG_Radiance") yield break;

            yield return null;

            PlayMakerFSM control = GameObject.Find("Boss Control").LocateMyFSM("Control");

            UObject.Destroy(GameObject.Find("Sun"));
            UObject.Destroy(GameObject.Find("feather_particles"));

            FsmState setup = control.GetState("Setup");

            setup.GetAction<Wait>(6).time = 1.5f;
            setup.RemoveAction(5);
            setup.RemoveAction(4);
            setup.ChangeTransition("FINISHED", "Eye Flash");

            control.GetAction<Wait>("Title Up", 6).time = 1f;
        }

        private static IEnumerator AbyssShriekPickup(Scene to) {
            if (to.name != "Abyss_12") yield break;

            yield return null;

            PlayMakerFSM shriek = GameObject.Find("Scream 2 Get").LocateMyFSM("Scream Get");
            shriek.GetState("Move To").ChangeTransition("FINISHED", "Get");
            shriek.GetState("Get Pause").RemoveAllOfType<Wait>();
            shriek.GetState("Land").RemoveAllOfType<Wait>();
        }

        private static IEnumerator KingsBrandHornet(Scene to) {
            if (to.name != "Deepnest_East_12") yield break;

            yield return null;

            PlayMakerFSM hornet = GameObject.Find("Hornet Blizzard Return Scene").LocateMyFSM("Control");

            hornet.GetState("Fade Pause").RemoveAllOfType<Wait>();
            hornet.GetState("Fade In").RemoveAllOfType<Wait>();
            hornet.GetState("Land").RemoveAllOfType<Wait>();
        }

        private static IEnumerator KingsBrandAvalanche(Scene to) {
            if (to.name != "Room_Wyrm") yield break;

            yield return null;

            PlayMakerFSM avalanche = GameObject.Find("Avalanche End").LocateMyFSM("Control");
            avalanche.GetState("Fade").GetAction<Wait>().time = 1;
        }

        private static IEnumerator BlackEgg(Scene to) {
            if (to.name != "Room_temple") yield break;

            yield return null;

            PlayMakerFSM door = GameObject.Find("Final Boss Door").LocateMyFSM("Control");

            door.GetState("Take Control").RemoveAllOfType<Wait>();
            door.GetState("Shake").GetAction<Wait>().time = 1;
            door.GetState("Barrier Flash").RemoveAllOfType<Wait>();
            door.GetState("Blow").RemoveAllOfType<Wait>();
            door.GetState("Door Off").RemoveAllOfType<Wait>();
            door.GetState("Roar").RemoveAllOfType<Wait>();
            door.GetState("Roar End").GetAction<Wait>().time = 1;
        }

        private static IEnumerator DreamerReturn(Scene to) {
            if (!DREAMERS.Contains(to.name) || !HeroController.instance.IsDreamReturning) yield break;

            yield return null;

            // cancel healing animation
            GameObject.Find("Knight").LocateMyFSM("Spell Control").SendEvent("FSM CANCEL");
        }

        private static void OnSetSkip(On.InputHandler.orig_SetSkipMode orig, InputHandler self, SkipPromptMode newmode) {
            orig(self, SkipPromptMode.SKIP_INSTANT);
        }

        private static void AnimatorBegin(On.AnimatorSequence.orig_Begin orig, AnimatorSequence self) => self.Skip();

        private static void FadeBegin(On.FadeSequence.orig_Begin orig, FadeSequence self) => self.Skip();

        private static void CinematicBegin(On.CinematicSequence.orig_Begin orig, CinematicSequence self) => self.Skip();

        private static IEnumerator BeginSceneTransition(On.GameManager.orig_BeginSceneTransitionRoutine orig, GameManager self, GameManager.SceneLoadInfo info) {
            info.EntryDelay = 0f;
            yield return orig(self, info);
        }

    }
}