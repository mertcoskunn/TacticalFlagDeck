using System;
using UnityEngine;

namespace Network.Messages
{
    [Serializable]
    public class MatchMakingMessage
    {
        public string type;
        public string match_id;
    }

    [Serializable]
    public class LoginMessage
    {
        public string type;
        public string userId;
        public string token;
    }

    [Serializable]
    public class MoveMessage
    {
        public string type;
        public string playerId;
        public Vector2 direction;
    }

    // Gibi...
}
