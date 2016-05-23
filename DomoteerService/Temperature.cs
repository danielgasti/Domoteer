using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DomoteerService
{
    [DataContract]
    public class Temperature
    {
        [DataMember]
        public Double temperature {get; set;}
        [DataMember]
        public String timestamp {get; set;}

    }
}