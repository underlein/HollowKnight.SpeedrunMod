using JetBrains.Annotations;

namespace SpeedrunMod.Modules {
    [UsedImplicitly]
    public abstract class FauxMod {

        public bool DefaultState { get; } = true;

        protected FauxMod() {
        }

        protected FauxMod(bool enabled) {
            DefaultState = enabled;
        }

        [PublicAPI]
        internal void Log(object obj) {
            Modding.Logger.Log($"[SpeedrunMod : {GetType().Name}] - {obj}");
        }

        internal bool IsLoaded { get; set; }

        public virtual void Initialize() {
        }

        public virtual void Unload() {
        }

    }
}