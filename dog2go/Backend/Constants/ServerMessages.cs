namespace  dog2go.Backend.Constants
{
    internal static class ServerMessages
    {
        public static readonly string JoinedGame = "Player {0} joined the game.";
        public static readonly string NofityActualPlayer = "It's your turn!";
        public static readonly string NewRoundStart = "New round started. Shuffling and dealing cards";
        public static readonly string NoValidCardAvailable = "Player {0} has no valid cards and is skipped!";
        public static readonly string InformOtherPlayer = "It's Player {0}'s turn. ";
        public static readonly string GameFinished = "Game finished. The winners are {0}";
    }
}