using System.DirectoryServices.AccountManagement;

namespace Intitek.Welcome.Service.Back
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("User")]
    public class UserPrincipalExtended : UserPrincipal
    {
        public UserPrincipalExtended(PrincipalContext context) : base(context)
        {
        }

        public UserPrincipalExtended(PrincipalContext context, string samAccountName, string password, bool enabled) : base(context, samAccountName, password, enabled)
        {
        }

        [DirectoryProperty("company")]
        public string company
        {
            get
            {
                if (ExtensionGet("company").Length != 1)
                    return null;
                return (string)ExtensionGet("company")[0];
            }
        }

        [DirectoryProperty("l")]
        public string l
        {
            get
            {
                if (ExtensionGet("l").Length != 1)
                    return null;
                return (string)ExtensionGet("l")[0];
            }
        }

        [DirectoryProperty("extensionAttribute8")]
        public string extensionAttribute8
        {
            get
            {
                if (ExtensionGet("extensionAttribute8").Length != 1)
                    return null;
                return (string)ExtensionGet("extensionAttribute8")[0];
            }
        }
        [DirectoryProperty("departement")]
        public string departement
        {
            get
            {
                if (ExtensionGet("departement").Length != 1)
                    return null;
                return (string)ExtensionGet("departement")[0];
            }
        }
        [DirectoryProperty("division")]
        public string division
        {
            get
            {
                if (ExtensionGet("division").Length != 1)
                    return null;
                return (string)ExtensionGet("division")[0];
            }
        }
        [DirectoryProperty("whenCreated")]
        public string whenCreated
        {
            get
            {
                if (ExtensionGet("whenCreated").Length != 1)
                    return null;
                return (string)ExtensionGet("whenCreate")[0];
            }
        }
    }
}
