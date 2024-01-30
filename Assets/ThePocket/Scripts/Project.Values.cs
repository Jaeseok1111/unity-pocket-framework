namespace ThePocket
{
    [System.Serializable]
    public class IntValue
    {
        public string Identifier;
        public int Value;
    }

    [System.Serializable]
    public class StringValue
    {
        public string Identifier;
        public string Value;

        public bool IsNullOrEmpty()
        {
            return string.IsNullOrEmpty(Value);
        }
    }

    [System.Serializable]
    public class Vector2Value
    {
        public string Identifier;
        public UnityEngine.Vector2 Value;

        public bool IsZero()
        {
            return Value == UnityEngine.Vector2.zero;
        }
    }
}