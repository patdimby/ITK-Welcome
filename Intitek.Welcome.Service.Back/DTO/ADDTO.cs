using System;

namespace Intitek.Welcome.Service.Back
{
    public class ADDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool ToBeSynchronized { get; set; }
        public DateTime? LastSynchronized { get; set; }
    }
}
