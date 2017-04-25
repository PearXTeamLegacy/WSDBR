

using System;

namespace WSDBR.Model
{
    public class Message
    {
        public string Jid { get; set; }
        public string RemoteJid { get; set; }
        public string Data { get; set; }
        public bool FromMe { get; set; }
        public DateTime Timestamp { get; set; }
    }
}