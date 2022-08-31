using System;

namespace Intitek.Welcome.Service.Back
{
    public class LocationLDAP: IEquatable<LocationLDAP>
    {
        public string Zone { get; set; }
        public string Country { get; set; }
        public string Entity { get; set; }
        public string Agency { get; set; }

        public bool Equals(LocationLDAP other)
        {
            if (other == null) return false;

            return (this.Agency == other.Agency &&
                this.Country == other.Country &&
                this.Entity == other.Entity &&
                this.Zone == other.Zone);
        }
    }
}
