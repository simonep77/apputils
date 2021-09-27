using System;
using System.Collections.Generic;

namespace AppUtils.Lib.Controlli
{


    /// <summary>
    /// Elenco esiti controlli
    /// </summary>
    public class EsitoControlloLista : List<EsitoControllo>
    {
        public bool Positivo
        {
            get
            {
                foreach (EsitoControllo Item in this)
                {
                    if (!Item.Positivo)
                        return false;
                }

                return true;
            }
        }

        public void Add(bool positivo, int codice, string testo)
        {
            EsitoControllo oCtrl = new EsitoControllo();
            oCtrl.Positivo = positivo;
            oCtrl.EsitoCodice = codice;
            oCtrl.EsitoTesto = testo;

            this.Add(oCtrl);
        }

    }
}
