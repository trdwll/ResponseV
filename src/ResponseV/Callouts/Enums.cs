namespace ResponseV.Callouts
{
    class Enums
    {
        public enum Callout
        {
            InPursuit,
            EnRoute,
            OnScene,
            Done
        };

        // used for when we have multiple outcomes for a callout like Officer Down
        public enum CalloutType
        {
            Stabbing,
            Shooting,
            Unknown
        }

        public enum Response
        {
            Code2,
            Code3,
            Silent
        }
    }
}
