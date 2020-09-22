using System;
using System.Collections.Generic;

public class DecomposizioneCifre
{
    private SortedList<decimal, object> mListaCifreSorted = new SortedList<decimal, object>(new DescendDecimalComparer());

    public DecomposizioneCifre()
    {
    }

    public DecomposizioneCifre(IEnumerable<decimal> cifre)
    {
        foreach (decimal item in cifre)
            this.mListaCifreSorted.Add(item, null);
    }

    /// <summary>
    ///  Aggiunge cifra alla lista
    ///  </summary>
    ///  <param name="value"></param>
    public void AddCifra(decimal value)
    {
        this.mListaCifreSorted.Add(value, null);
    }


    /// <summary>
    ///  Esegue calcolo
    ///  </summary>
    ///  <param name="value"></param>
    ///  <returns></returns>
    public List<decimal> Calcola(decimal value)
    {
        List<decimal> oTmpLst;
        List<decimal> oListaRet = new List<decimal>();

        foreach (decimal item in mListaCifreSorted.Keys)
        {
            if (item > value)
                continue;

            decimal dTempValue = value - item;

            if (dTempValue > 0M)
            {
                // Va in ricorsione
                oTmpLst = this.Calcola(dTempValue);

                // Se non trovata soluzione dobbiamo scartare il risultato
                if (oTmpLst.Count == 0)
                    continue;

                // Consolidiamo il risultato attuale e della ricorsione
                value -= item;
                oListaRet.Add(item);
                oListaRet.AddRange(oTmpLst);
            }
            else
                // Consolidiamo attuale ed usciamo
                oListaRet.Add(item);

            // Usciamo sempre dal ciclo
            break;
        }

        return oListaRet;
    }


    /// <summary>
    ///  Calcola tutti i percorsi. Attenzione in presenza di parecchie cifre/valori bassi cifre in quanto scoppia la memoria.
    ///  Es: valore=100 e le seguenti cifre (es 5, 3, 2, 1.5, 1.2, 1) danno circa 250000 combinazioni in 30 sec di elaborazione con circa 250MB di memoria occupata.
    ///  Usarlo con cautela!!!!!!!
    ///  </summary>
    ///  <param name="value"></param>
    ///  <returns></returns>
    public List<List<decimal>> CalcolaPercorsi(decimal value)
    {
        return this.CalcolaPercorsi(value, 0M, int.MaxValue);
    }

    /// <summary>
    ///  Calcola tutti i percorsi escludendo cifre sotto una soglia percentuale
    ///  </summary>
    ///  <param name="value"></param>
    ///  <param name="percentuale"></param>
    ///  <returns></returns>
    public List<List<decimal>> CalcolaPercorsi(decimal value, decimal percentuale)
    {
        return this.CalcolaPercorsi(value, percentuale, int.MaxValue);
    }


    /// <summary>
    /// Calcola tutti i percorsi escludendo cifre sotto una soglia percentuale e con un massimo livello di profondita'
    /// </summary>
    /// <param name="value"></param>
    /// <param name="percentuale"></param>
    /// <param name="maxProfondita"></param>
    /// <returns></returns>
    public List<List<decimal>> CalcolaPercorsi(decimal value, decimal percentuale, int maxProfondita)
    {
        List<List<decimal>> oListaPaths = new List<List<decimal>>();
        List<decimal> oCurrPaths = new List<decimal>();
        var dPercValue = Math.Ceiling(value * (Math.Max(0M, Math.Min(100M, percentuale)) / 100M));
        var iMaxProf = Math.Max(1, maxProfondita);
        List<decimal> oListaCifre = new List<decimal>(this.mListaCifreSorted.Count);

        foreach (decimal item in this.mListaCifreSorted.Keys)
        {
            if (item >= dPercValue)
                oListaCifre.Add(item);
        }

        this.calcolaPercorsi(value, oListaCifre, oCurrPaths, oListaPaths, iMaxProf);

        return oListaPaths;
    }



    /// <summary>
    ///  Calcola tutti i percorsi escludendo cifre sotto una soglia percentuale e con un numero definito di profondita' di percorso
    ///  </summary>
    ///  <param name="value"></param>
    ///  <param name="listaCifre"></param>
    ///  <param name="currentPath"></param>
    ///  <param name="paths"></param>
    private void calcolaPercorsi(decimal value, IList<decimal> listaCifre, List<decimal> currentPath, List<List<decimal>> paths, int maxProfondita)
    {

        // Se raggiunta profondita' massima allora usciamo
        if (currentPath.Count == maxProfondita)
            return;

        while (listaCifre.Count > 0)
        {
            if (listaCifre[0] > value)
            {
                listaCifre.RemoveAt(0);
                continue;
            }

            decimal dTempValue = value - listaCifre[0];
            currentPath.Add(listaCifre[0]);



            if (dTempValue > 0M)
                // Va in ricorsione

                this.calcolaPercorsi(dTempValue, new List<decimal>(listaCifre), new List<decimal>(currentPath), paths, maxProfondita);
            else
            {
                // Salviamo path
                paths.Add(new List<decimal>(currentPath));

                // Eseguo valutazione solo se presenti almeno 2 cifre
                if (listaCifre.Count > 1)
                {
                    currentPath.RemoveAt(currentPath.Count - 1);
                    listaCifre.RemoveAt(0);
                    this.calcolaPercorsi(value, new List<decimal>(listaCifre), new List<decimal>(currentPath), paths, maxProfondita);

                    return;
                }
            }

            // arrivo alla fine e pulisco
            if (currentPath.Count > 0)
                currentPath.RemoveAt(currentPath.Count - 1);

            if (listaCifre.Count > 0)
                listaCifre.RemoveAt(0);
        }
    }

    private class DescendDecimalComparer : IComparer<decimal>
    {
        public int Compare(decimal x, decimal y)
        {
            return (-x.CompareTo(y));
        }
    }



    private void calcolaPercorsi2(decimal value, IList<decimal> listaCifre, List<decimal> currentPath, List<List<decimal>> paths)
    {
        List<List<decimal>> oListaPaths = new List<List<decimal>>();
        List<decimal> oCurrPaths = new List<decimal>();
        int iLevel = 0;
        var dValue = value;
        decimal dLastValueInPath = decimal.MaxValue;

        while (true)
        {
            foreach (decimal item in listaCifre)
            {
                iLevel += 1;

                // Se ultimo del currentpath maggiore di item non e' ammesso
                if (item > dLastValueInPath)
                    continue;

                if (item < dValue)
                {
                    dLastValueInPath = item;
                    oCurrPaths.Add(item);
                    dValue -= item;

                    break;
                }
                else if (item == dValue)
                {
                    dLastValueInPath = item - 0.01M;
                    oCurrPaths.Add(item);
                    oListaPaths.Add(new List<decimal>(currentPath));

                    // Torniamo su
                    currentPath.Remove(currentPath.Count - 1);
                    iLevel -= 1;

                    break;
                }
                else
                {
                }
            }
        }
    }
}
