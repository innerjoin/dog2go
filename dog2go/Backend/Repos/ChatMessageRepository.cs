using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;

namespace dog2go.Backend.Repos
{
    public sealed class ChatMessageRepository : IChatRepository
    {
        private ChatMessageRepository() { }

        public static ChatMessageRepository Instance { get; } = new ChatMessageRepository();

        private readonly List<Message> _messagesList = new List<Message>();

        public void AddMessage(Message msg)
        {
            _messagesList.Add(msg);
        }

        public List<Message> GetMessageList(string group)
        {
            return _messagesList.FindAll(msg => msg.Group == group);
        }
    }
}