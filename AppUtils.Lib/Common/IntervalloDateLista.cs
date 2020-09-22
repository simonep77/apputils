using System;
using System.Collections.Generic;
using System.Text;

namespace AppUtils.Lib.Common
{
    /// <summary>
    /// Lista intervalli date
    /// </summary>
    public class IntervalloDateLista: List<IntervalloDate>
    {

        /// <summary>
        /// Ritorna elenco date
        /// </summary>
        /// <returns></returns>
        public List<DateTime> ToDateList()
        {
            var lst = new List<DateTime>(2 * Math.Max(1, this.Count));
            foreach (var item in this)
            {
                lst.Add(item.Inizio);
                lst.Add(item.Fine);
            }

            return lst;
        }


        /// <summary>
        /// Data una lista di date torna gli intervalli ordinati. In assenza di ultima data fine imposta Maxvalue
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public static IntervalloDateLista FromDateEnum(IEnumerable<DateTime> lista)
        {
            var retLista = new IntervalloDateLista();
            var sorted = new SortedList<DateTime, object>();
            IntervalloDate currInt = null;

            foreach (var item in lista)
            {
                sorted[item] = item;
            }

            foreach (var item in sorted)
            {
                if (currInt == null)
                {
                    currInt = new IntervalloDate();
                    currInt.Inizio = item.Key;
                    retLista.Add(currInt);
                }
                else
                {
                    currInt.Fine = item.Key;
                    currInt = null;
                }
            }

            if (currInt != null)
                currInt.Fine = DateTime.MaxValue;

            return retLista;
        }
    }
}
