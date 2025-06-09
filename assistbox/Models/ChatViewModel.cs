using System.Collections.Generic;

namespace assistbox.Models
{
    public class ChatViewModel
    {
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public List<string> MenuOptions { get; set; } = new List<string>();
        public string CurrentState { get; set; } 
        public string UserName { get; set; }
        public string BotResponsePending { get; set; } 
    }
}
