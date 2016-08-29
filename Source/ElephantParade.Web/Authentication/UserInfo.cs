using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace NHSD.ElephantParade.Web.Authentication
{
    /// <summary>
    /// Used to serialize de-serialize participant information 
    /// We only want regularly used data in here
    /// </summary>
    public class UserInfo
    {
        public string DisplayName { get; set; }      
        public string UserId { get; set; }
        public string StudyID { get; set; }
        public string PatientId { get; set; }

        public override string ToString()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserInfo));
            using (var stream = new StringWriter())
            {
                serializer.Serialize(stream, this);
                return stream.ToString();
            }
        }

        public static UserInfo FromString(string userContextData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserInfo));
            using (var stream = new StringReader(userContextData))
            {
                return serializer.Deserialize(stream) as UserInfo;
            }
        }
    }
}
