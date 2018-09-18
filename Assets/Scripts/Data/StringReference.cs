using System;

namespace SF {
    [Serializable]
    public class StringReference {
        public bool useConstant = false;
        public string constantValue;
        public StringVariable variable;

        public StringReference(string value) {
            useConstant = true;
            constantValue = value;
        }

        public string value {
            get {
                return useConstant ? constantValue : variable.value;
            }
        }

        public static implicit operator string(StringReference reference) {
            return reference.value;
        }
    }
}
