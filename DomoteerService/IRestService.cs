using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DomoteerService
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IService1" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract]
    public interface IRestService
    {
        [OperationContract]
        [WebInvoke(
        Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Wrapped,
        UriTemplate = "getTemperatures?n={n}")]
        List<Temperature> getTemperatures(String n);

        [OperationContract]
        [WebInvoke(
        Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Wrapped,
        UriTemplate = "putTemperatures?temperature={t}&date={date}")]
        String putTemperatures(String t, String date);

        [OperationContract]
        [WebInvoke(
        Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Wrapped,
        UriTemplate = "getGas?n={n}")]
        List<Gas> getGas(String n);

        [OperationContract]
        [WebInvoke(
        Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Wrapped,
        UriTemplate = "putGas?lpg={lpg}&co={co}&smoke={smoke}&date={date}")]
        String putGas(String lpg, String co, String smoke, String date);

    }



}
