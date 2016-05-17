using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DomoteerRestWcfService
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di classe "RestService" nel codice, nel file svc e nel file di configurazione contemporaneamente.
    // NOTA: per avviare il client di prova WCF per testare il servizio, selezionare RestService.svc o RestService.svc.cs in Esplora soluzioni e avviare il debug.
    public class RestService : IRestService
    {
        public String getTemperatures(String n)
        {
            return "got: " + n;
        }

        public String putTemperatures(String t, String date)
        {
            return "put: " + t + " at: " + date;
        }
    }
}
