using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DomoteerService
{
    [DataContract]
    public class Gas
    {
        [DataMember]
        public String LGP { get; set; }
        [DataMember]
        public String CO { get; set; }
        [DataMember]
        public String SMOKE { get; set; }
        [DataMember]
        public String timestamp { get; set; }

    }
}