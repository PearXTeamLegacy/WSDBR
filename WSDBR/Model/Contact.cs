using System;

namespace WSDBR.Model
{
    public class Contact
    {
        public string DisplayName { get; set; }
        public bool WhatsappUser { get; set; }
        public string Status { get; set; }
        public DateTime StatusTimestamp { get; set; }
        public string Number { get; set; }
        public string WaName { get; set; }
        public string JId { get; set; }
    }
}