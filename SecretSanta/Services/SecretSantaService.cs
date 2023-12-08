using SecretSanta.Exceptions;
using SecretSanta.Models;

namespace SecretSanta.Services
{
    public interface ISecretSantaService
    {
        IEnumerable<Friend> DrawSecretSata(IList<Friend> friends);
    }

    public class SecretSantaService : ISecretSantaService
    {
        private readonly ILogger<ISecretSantaService> logger;

        public SecretSantaService(ILogger<ISecretSantaService> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Friend> DrawSecretSata(IList<Friend> friends)
        {
            logger.LogInformation("Starting shuffle process");

            ValidateNumberOfFriends(friends);

            Random randNumber = new();

            var auxNumber = randNumber.Next(0, friends.Count - 2);

            for (var i = 0; i < friends.Count; i++)
            {
                if (i == friends.Count - 1 && friends.All(p => p.SecretSanta?.Name != friends[i].Name))
                {
                    friends[i].SecretSanta = friends[auxNumber].SecretSanta;
                    friends[auxNumber].SecretSanta = new Friend { Name = friends[i].Name };
                }
                else
                {
                    var nextAmigo = friends[randNumber.Next(0, friends.Count - 1)];

                    while (friends.Any(p => p.SecretSanta?.Name == nextAmigo.Name) || nextAmigo.Name == friends[i].Name)
                    {
                        nextAmigo = friends[randNumber.Next(0, friends.Count - 1)];
                    }
                    friends[i].SecretSanta = new Friend { Name = nextAmigo.Name };
                }
            }

            for (int i = 0; i < friends.Count; i++)
            {
                logger.LogDebug("Friend {i} is friend {j}'s secret santa", i, friends.IndexOf(friends.First(p => p.Name == friends[i].SecretSanta?.Name)));
            }

            logger.LogInformation("Suffle process ended");

            return friends;
        }

        private static void ValidateNumberOfFriends(ICollection<Friend> friends)
        {
            if (friends.Count < 3)
                throw new InvalidParticipantsCountException();
        }
    }
}
