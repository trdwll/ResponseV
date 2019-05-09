namespace ResponseV.Callouts
{
    public class Enums
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
            FailureToCheckIn,
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
