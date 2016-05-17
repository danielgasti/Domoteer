using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DomoteerRestWcfService
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IRestService" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract]
    
    public interface IRestService
    {
        [OperationContract]
        [WebInvoke(
        Method = "GET",
        ResponseFormat = WebMessageFormat.Xml,
        BodyStyle = WebMessageBodyStyle.Wrapped,
        UriTemplate = "getTemperatures/{n}")]
        String getTemperatures  (String n);

        [OperationContract]
        [WebInvoke(
        Method = "PUT",
        ResponseFormat = WebMessageFormat.Xml,
        BodyStyle = WebMessageBodyStyle.Wrapped,
        UriTemplate = "putTemperatures/{t}/{date}")]
        String putTemperatures(String t, String date);

    }
}
