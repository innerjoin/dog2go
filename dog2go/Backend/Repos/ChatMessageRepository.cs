using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using dog2go.Backend.Interfaces;
using dog2go.Backend.Model;

namespace dog2go.Backend.Repos
{
    public sealed class ChatMessageRepository : IChatRepository
    {
        private static ChatMessageRepository _instance;
        private static readonly Object LockObj = new Object();
        private ChatMessageRepository() { }

        public static ChatMessageRepository Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Monitor.Enter(LockObj);
                ChatMessageRepository temp = new ChatMessageRepository();
                Interlocked.Exchange(ref _instance, temp);
                Monitor.Exit(LockObj);
                return _instance;
            }
        }

        private readonly List<Message> _messagesList = new List<Message>();

        public void AddMessage(Message msg)
        {
            lock (_messagesList)
            {
                _messagesList.Add(msg);
            }
        }

        public List<Message> GetMessageList(string group)
        {
            lock (_messagesList)
            {
                return _messagesList.FindAll(msg => msg.Group == group);
            }
        }
    }
}