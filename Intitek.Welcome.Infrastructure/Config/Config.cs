using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Config
{
    [Serializable]
    public class Config
    {
        public string DBServer { get; set; }
        public string DBName { get; set; }
        public string DBUserID { get; set; }
        public string DBPassword { get; set; }
        public string SMTPUser { get; set; }
        public string SMTPPassword { get; set; }

        public void Serialize(string filePath)
        {
            var formatter = new BinaryFormatter();
            using (var stream = File.Open(filePath, FileMode.Create))
            {                
                formatter.Serialize(stream, this);
            }
        }

        sealed class CustomBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (!(typeName == "Intitek.Welcome.Infrastructure.Config.Config"))
                {
                    throw new SerializationException("Only Config are allowed"); // Compliant
                }
                return Assembly.Load(assemblyName).GetType(typeName);
            }
        }

        public static Config Deserialize(string filePath)
        {
            var formatter = new BinaryFormatter();
            formatter.Binder = new CustomBinder();
            using (var stream = File.OpenRead(filePath))
            {
                return (Config)formatter.Deserialize(stream);
            }
        }
    }
}
