namespace SecretSanta.Exceptions
{
    public class InvalidParticipantsCountException : Exception
    {
        public InvalidParticipantsCountException() : base("Draw must have at least 3 participants") { }
    }
}
