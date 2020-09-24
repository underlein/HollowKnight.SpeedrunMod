using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Modding;
using UnityEngine;
using Vasi;

namespace SpeedrunMod {
    public class Settings : ModSettings, ISerializationCallbackReceiver {

        private readonly Assembly _asm = Assembly.GetAssembly(typeof(Settings));

        private readonly Dictionary<FieldInfo, Type> _fields = new Dictionary<FieldInfo, Type>();

        public Settings() {
            foreach (Type t in _asm.GetTypes()) {
                foreach (FieldInfo fi in t.GetFields().Where(x => x.GetCustomAttributes(typeof(SerializeToSetting), false).Length > 0)) {
                    _fields.Add(fi, t);
                }
            }
        }

        public void OnBeforeSerialize() {
            foreach ((FieldInfo fi, Type type) in _fields) {
                if (fi.FieldType == typeof(bool)) {
                    BoolValues[$"{type.Name}:{fi.Name}"] = (bool) fi.GetValue(null);
                } else if (fi.FieldType == typeof(float)) {
                    FloatValues[$"{type.Name}:{fi.Name}"] = (float) fi.GetValue(null);
                } else if (fi.FieldType == typeof(int)) {
                    IntValues[$"{type.Name}:{fi.Name}"] = (int) fi.GetValue(null);
                }
            }
        }

        public void OnAfterDeserialize() {
            foreach ((FieldInfo fi, Type type) in _fields) {
                if (fi.FieldType == typeof(bool)) {
                    if (BoolValues.TryGetValue($"{type.Name}:{fi.Name}", out bool val))
                        fi.SetValue(null, val);
                } else if (fi.FieldType == typeof(float)) {
                    if (FloatValues.TryGetValue($"{type.Name}:{fi.Name}", out float val))
                        fi.SetValue(null, val);
                } else if (fi.FieldType == typeof(int)) {
                    if (IntValues.TryGetValue($"{type.Name}:{fi.Name}", out int val))
                        fi.SetValue(null, val);
                }
            }
        }

    }
}