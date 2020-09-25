using AppUtils.Lib.Common;
using System;
using System.Collections.Generic;

namespace AppUtils.Lib.Banca
{

    /// <summary>
    /// ''' Classe per la Scrittura di File Flussi Bonifici SEPA XML 
    /// ''' </summary>
    /// ''' <remarks></remarks>
    public class FileSepaSCTWriterXml : IDisposable
    {
        private const string NBOFTXS = "[[[NBOFTXS]]]";
        private const string CTRLSUM = "[[[CTRLSUM]]]";

        private XmlWrite sdcXml = new XmlWrite();
        private int mProgressivo;
        private decimal mTotImportiPositivi;
        private string NomeFile { get; }

        /// <summary>
        /// Elenco disposizioni
        /// </summary>
        public List<Bonifico> Bonifici { get; } = new List<Bonifico>(50);


        public DateTime DataCreazione { get; set; }


        /// <summary>
        ///  in file SCT la data esecuzione (ReqdExctnDt) corrisponde alla data valuta
        ///  </summary>
        ///  <value></value>
        ///  <returns></returns>
        ///  <remarks></remarks>
        public DateTime DataValuta { get; set; }


        

        public int NumeroDisposizioni
        {
            get
            {
                return this.Bonifici.Count;
            }
        }


        public string CodiceDivisa { get; set; } = "E";


        public string CodiceSIA { get; set; } = string.Empty;


        public string TipoCodiceSIA { get; set; } = "9";


        public string NomeSupporto { get; set; } = string.Empty;


        public string CentroApplicativo { get; set; } = string.Empty;


        public Iban IbanCreditore { get; set; }


        public string NomeCreditore { get; set; } = string.Empty;

        public RecapitoPostale RecapitoCreditore { get; set; } = new RecapitoPostale();


        public string CreditoreCodiceFiscale { get; set; } = string.Empty;


        public string Cuc { get; set; } = string.Empty;


        public string PmtMtd { get; set; } = "TRF";


        public string InstrPrty { get; set; } = "NORM";


        public string SvcLvlCd { get; set; } = "SEPA";


        public string Issr { get; set; } = "CBI";


        public string ChrgBr { get; set; } = "SLEV";


        public string CtgyPurp_Cd { get; set; } = "CASH";


        public string Adrtp { get; set; } = "ADRPT";


        public string Ctry { get; set; } = "CTRY";


        public string MsgId { get; set; } = string.Empty;


        public string PmtInfId { get; set; } = string.Empty;


        public string EndToEndId { get; set; } = string.Empty;


        public bool EndToEndIdAutogenerato { get; set; } = true;







        public FileSepaSCTWriterXml(string nomeFile)
        {
            this.NomeFile = nomeFile;
        }


        /// <summary>
        ///  Aggiunge Bonifico a lista da evadere
        ///  </summary>
        ///  <param name="oBonificoIn"></param>
        ///  <remarks></remarks>
        public void AggiungiBonifico(Bonifico oBonificoIn)
        {
            if (oBonificoIn == null)
                throw new ArgumentException("Bonifico nullo");

            // Verifica bonifico
            if (oBonificoIn.Iban == null)
                throw new ArgumentException(string.Format("Il bonifico per {0} ha iban nullo", oBonificoIn.Nominativo));

            this.Bonifici.Add(oBonificoIn);
        }

        /// <summary>
        ///  Esegue la scrittura fisica del file con il flusso
        ///  </summary>
        ///  <remarks></remarks>
        public void ScriviFlusso()
        {
            decimal dZero = 0M;
            this.mProgressivo = 0;


            string dataGrpHdr = string.Concat(this.DataCreazione.ToString("yyyy-MM-dd"), "T00:00:00+01:00");

            string dataPmtInf = this.DataValuta.ToString("yyyy-MM-dd");

            // Esegue validazione flusso
            this.ValidazioneFlusso();

            using (System.IO.StreamWriter oFilesw = System.IO.File.CreateText(this.NomeFile))
            {
                this.SCT_ScriviAperturaTestataXml_SEPA();

                this.SCT_GrpHdr_SEPA(this.MsgId, dataGrpHdr, NomeCreditore, this.Cuc, this.Issr);  // imposto il default e lo sostituisco alla fine

                this.SCT_PmtInf_SEPA(this.PmtInfId, this.PmtMtd, this.InstrPrty, this.SvcLvlCd, dataPmtInf, this.NomeSupporto, this.IbanCreditore.IbanCompleto, this.IbanCreditore.IbanCodiceBanca, this.ChrgBr);

                foreach (Bonifico oBonifico in this.Bonifici)
                {

                    // Incrementa progressivo
                    this.mProgressivo += 1;

                    // Imposta Importo Totale
                    this.mTotImportiPositivi += Math.Max(oBonifico.Importo, dZero);

                    // Determina endtoend ID
                    string endToEndIdInput = oBonifico.IdentificativoOperazione;
                    if (this.EndToEndIdAutogenerato)
                        endToEndIdInput = string.Concat(this.EndToEndId, this.mProgressivo);

                    // Scrive disposizioni di dettaglio        
                    this.SCT_CdtTrfTxInf_IT_SEPA(this.mProgressivo, endToEndIdInput, this.CtgyPurp_Cd, oBonifico.Importo, oBonifico.Nominativo, oBonifico.Iban.IbanCompleto, oBonifico.DescrizioneOperazione, oBonifico.Bic);
                }

                this.SCT_ChiudiBloccoTestata_SEPA();

                // effettuo replace distinta generale
                oFilesw.WriteLine(this.SCT_ReplaceGrpHdr_SEPA(this.mProgressivo, this.mTotImportiPositivi));

                oFilesw.Flush();
            }
        }




        private void SCT_ScriviAperturaTestataXml_SEPA()
        {
            sdcXml.WriteStartElement("?xml version=\"1.0\" encoding=\"UTF-8\"?");
            sdcXml.WriteStartElement("CBIBdyPaymentRequest");
            sdcXml.WriteAttributeString("xmlns", "urn:CBI:xsd:CBIBdyPaymentRequest.00.04.00");
            sdcXml.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            sdcXml.WriteAttributeString("xsi:schemaLocation", "urn:CBI:xsd:CBIBdyPaymentRequest.00.04.00 CBIBdyPaymentRequest.00.04.00.xsd");

            sdcXml.WriteStartElement("CBIEnvelPaymentRequest");
            sdcXml.WriteStartElement("CBIPaymentRequest");
        }

        /// <summary>
        ///  Scrive tag chiusura finale (Testata, PmtTpinf)
        ///  </summary>
        ///  <remarks></remarks>
        private void SCT_ChiudiBloccoTestata_SEPA()
        {
            sdcXml.WriteEndElement(); // PmtInf
            sdcXml.WriteEndElement(); // CBIPaymentRequest
            sdcXml.WriteEndElement(); // CBIEnvelPaymentRequest
            sdcXml.WriteEndElement(); // CBIBdyPaymentRequest
        }

        private void SCT_GrpHdr_SEPA(string ProgressivoRID, string DataCreazioneFlusso, string AziEmittente, string Id, string Issr)
        {
            sdcXml.WriteStartElement("GrpHdr");
            try
            {
                sdcXml.WriteAttributeString("xmlns", "urn:CBI:xsd:CBIPaymentRequest.00.04.00");

                // Identificativo univoco messaggio  
                sdcXml.WriteElementString("MsgId", ProgressivoRID);

                // Data e Ora di Creazione
                sdcXml.WriteElementString("CreDtTm", DataCreazioneFlusso);

                // Numero transazioni incluse nella distinta
                sdcXml.WriteElementString("NbOfTxs", NBOFTXS);

                // Totale importi delle transazioni incluse nelle distinta
                sdcXml.WriteElementString("CtrlSum", CTRLSUM);

                // Nome azienda mittente
                sdcXml.WriteStartElement("InitgPty");

                sdcXml.WriteElementString("Nm", AziEmittente);

                sdcXml.WriteStartElement("Id");
                // Codice CUC azienda mittente
                sdcXml.WriteStartElement("OrgId");
                sdcXml.WriteStartElement("Othr");

                sdcXml.WriteElementString("Id", Id);

                sdcXml.WriteElementString("Issr", Issr);

                sdcXml.WriteEndElement(); // chiude Othr

                sdcXml.WriteEndElement(); // chiude Orgid

                sdcXml.WriteEndElement(); // chiude id

                sdcXml.WriteEndElement(); // chiude InitgPty
            }
            finally
            {
                sdcXml.WriteEndElement();
            }// chiude gphdr
        }


        /// <summary>
        ///  Rimpiazza i contatori nell'header
        ///  </summary>
        ///  <param name="dispTot"></param>
        ///  <param name="importoTot"></param>
        ///  <returns></returns>
        ///  <remarks></remarks>
        private string SCT_ReplaceGrpHdr_SEPA(int dispTot, decimal importoTot)
        {
            // [[[NBOFTXS]]]
            string asdcXml = sdcXml.ToString().Replace(NBOFTXS, dispTot.ToString());

            // [[[CTRLSUM]]]
            asdcXml = asdcXml.Replace(CTRLSUM, Numeri.DecimalToStringUNIV(importoTot));

            return asdcXml;
        }


        private void SCT_PmtInf_SEPA(string PmtInfId, string PmtMtd, string InstrPrty, string SvcLvlCd, string ReqdExctnDt, string NomeDebitore, string IbanDebitore, string MmbId, string ChrgBr)
        {
            sdcXml.WriteStartElement("PmtInf");
            sdcXml.WriteAttributeString("xmlns", "urn:CBI:xsd:CBIPaymentRequest.00.04.00");

            // Identificativo informazioni di accredito  
            sdcXml.WriteElementString("PmtInfId", PmtInfId);
            sdcXml.WriteElementString("PmtMtd", PmtMtd);
            sdcXml.WriteStartElement("PmtTpInf");

            try
            {
                // Identificativo univoco messaggio  
                sdcXml.WriteElementString("InstrPrty", InstrPrty);
                sdcXml.WriteStartElement("SvcLvl");
                try
                {
                    sdcXml.WriteElementString("Cd", SvcLvlCd);
                }
                finally
                {
                    sdcXml.WriteEndElement();
                }// chiude SvcLvl
            }
            finally
            {
                sdcXml.WriteEndElement();
            }// chiude PmtTpInf

            // Data di esecuzione richiesta  
            sdcXml.WriteElementString("ReqdExctnDt", ReqdExctnDt);

            // Nome azienda ordinante
            sdcXml.WriteStartElement("Dbtr");
            try
            {
                sdcXml.WriteElementString("Nm", NomeDebitore);
            }
            finally
            {
                sdcXml.WriteEndElement();
            }// chiude Dbtr


            // IBAN conto di addebito
            sdcXml.WriteStartElement("DbtrAcct");
            try
            {
                sdcXml.WriteStartElement("Id");
                try
                {
                    sdcXml.WriteElementString("IBAN", IbanDebitore);
                }
                finally
                {
                    sdcXml.WriteEndElement();
                }// chiude Id
            }
            finally
            {
                sdcXml.WriteEndElement();
            }// chiude DbtrAcct

            // Abi Banca debitore
            sdcXml.WriteStartElement("DbtrAgt");
            try
            {
                sdcXml.WriteStartElement("FinInstnId");
                sdcXml.WriteStartElement("ClrSysMmbId");
                sdcXml.WriteElementString("MmbId", MmbId);

                sdcXml.WriteEndElement(); // chiude ClrSysMmbId
                sdcXml.WriteEndElement(); // chiude FinInstnId
            }
            finally
            {
                sdcXml.WriteEndElement();
            }// chiude DbtrAgt

            // Ammessi solo i valori  
            sdcXml.WriteElementString("ChrgBr", ChrgBr);
        }

        /// <summary>
        ///  Contiene il dettaglio delle singole disposizioni facenti parte della distinta di un bonifico verso l’Italia
        ///  </summary>
        private void SCT_CdtTrfTxInf_IT_SEPA(int PrgDisp, string IdEndToEnd, string CtgyPurp_Cd, decimal DivisaImp, string NomeBeneficiario, string ibanBeneficiario, string InfCausale, string Bic)
        {
            sdcXml.WriteStartElement("CdtTrfTxInf");

            // Progressivo disposizione e Identificativo end-to-end
            sdcXml.WriteStartElement("PmtId");
            sdcXml.WriteElementString("InstrId", PrgDisp.ToString());

            sdcXml.WriteElementString("EndToEndId", IdEndToEnd);

            sdcXml.WriteEndElement(); // chiude PmtId

            // --------------------
            // Causale bancaria (category purpose)
            sdcXml.WriteStartElement("PmtTpInf");
            sdcXml.WriteStartElement("CtgyPurp");
            sdcXml.WriteElementString("Cd", CtgyPurp_Cd);

            sdcXml.WriteEndElement(); // chiude CtgyPurp
            sdcXml.WriteEndElement(); // chiude PmtTpInf

            // ------------------
            // Divisa e importo
            sdcXml.WriteStartElement("Amt");

            sdcXml.WriteStartElement("InstdAmt");
            sdcXml.WriteAttributeString("Ccy", "EUR");

            sdcXml.WriteValue(Numeri.DecimalToStringUNIV(DivisaImp));

            sdcXml.WriteEndElement(); // chiude InstdAmt
            sdcXml.WriteEndElement(); // chiude Amt

            // ------------------
            // BIC banca titolare CC di accredito (solo se valorizzato
            // If Not String.IsNullOrEmpty(Bic) Then
            // sdcXml.WriteStartElement("CdtrAgt")
            // sdcXml.WriteStartElement("FinInstnId")
            // sdcXml.WriteStartElement("Bic")
            // sdcXml.WriteValue(Bic)
            // sdcXml.WriteEndElement()
            // sdcXml.WriteEndElement() 'chiude FinInstnId
            // sdcXml.WriteEndElement() 'chiude CdtrAgt
            // End If
            // ------------------
            // Nome del beneficiario
            sdcXml.WriteStartElement("Cdtr");
            sdcXml.WriteStartElement("Nm");
            sdcXml.WriteValue(NomeBeneficiario.Truncate(70));
            sdcXml.WriteEndElement();

            sdcXml.WriteEndElement(); // chiude Cdtr

            // --------------
            // IBAN conto del creditore   
            sdcXml.WriteStartElement("CdtrAcct");
            sdcXml.WriteStartElement("Id");
            sdcXml.WriteElementString("IBAN", ibanBeneficiario);

            sdcXml.WriteEndElement(); // chiude Id
            sdcXml.WriteEndElement(); // chiude CdtrAcct

            // ------------------
            // Informazioni/Causale
            sdcXml.WriteStartElement("RmtInf");
            sdcXml.WriteElementString("Ustrd", InfCausale);

            sdcXml.WriteEndElement(); // chiude RmtInf
            sdcXml.WriteEndElement(); // chiude CdtTrfTxInf
        }


        /// <summary>
        ///  Esegue validazione dati standard creditore + flusso
        ///  </summary>
        ///  <remarks></remarks>
        private void ValidazioneFlusso()
        {
            if (this.DataValuta == default(DateTime) || this.DataValuta == DateTime.MinValue)
                throw new Exception("ATTENZIONE DATA VALUTA FLUSSO NON IMPOSTATA");

            if (this.DataCreazione == default(DateTime) || this.DataCreazione == DateTime.MinValue)
                throw new Exception("ATTENZIONE DATA CREAZIONE FLUSSO  NON IMPOSTATA");
        }


        public void Dispose()
        {
            // Elimina bonifici
            this.Bonifici.Clear();
        }
    }

}

