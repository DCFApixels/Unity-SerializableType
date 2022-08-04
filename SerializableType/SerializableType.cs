using System;
using UnityEngine;

namespace DCFAPixels
{
    [Serializable]
    public class SerializableType : ISerializationCallbackReceiver
    {
        public static Type GetDefaultType() => typeof(SerializableType);
        private void ResetToDefault() => Value = GetDefaultType();

        private Type value;
        public Type Value
        {
            get
            {
#if UNITY_EDITOR
                if (assemblyQualifiedName != value.AssemblyQualifiedName)
                    ConvertNameToType();
#endif
                return value;
            }
            set
            {
                if (value == null)
                {
                    ResetToDefault();
                    return;
                }

                this.value = value;
                assemblyQualifiedName = value.AssemblyQualifiedName;
            }
        }

        private void ConvertNameToType()
        {
            value = Type.GetType(assemblyQualifiedName);
            if (value == null)
                ResetToDefault();
        }
        
        [SerializeField]
        private string assemblyQualifiedName;

        public string FullName { get => value.FullName; }
        public string AssemblyQualifiedName { get => assemblyQualifiedName; }

        public SerializableType() => ResetToDefault();
        public SerializableType(Type type) => Value = type;

        #region ISerializationCallbackReceiver
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            ConvertNameToType();
        }
        #endregion

        #region Equals&GetHashCode
        public override bool Equals(object obj) => obj is SerializableType type && (Value == type.Value);
        public override int GetHashCode() => Value.GetHashCode();
        #endregion

        #region Operators
        public static implicit operator Type(SerializableType value)
        {
            return value.Value;
        }

        public static implicit operator SerializableType(Type type)
        {
            return new SerializableType(type);
        }
        #endregion
    }

    public static class SerializableTypeExtensions
    {
        public static SerializableType GetSerializableType(this object obj) => new SerializableType(obj.GetType());
    }
}
