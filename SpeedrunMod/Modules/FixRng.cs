using JetBrains.Annotations;
using Vasi;

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
                case "Blocker Control" when self.name == "Blocker":
                    self.GetState("Idle").ChangeTransition("ATTACK", "Can Roller?");
                    break;
            }

            orig(self);
        }
    }
}