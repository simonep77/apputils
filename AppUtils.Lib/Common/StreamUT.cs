using System;
using System.IO;
using System.Text;

namespace AppUtils.Lib.Common
{
    /// <summary>
    ///  Utility per la gestione degli stream generici
    ///  </summary>
    ///  <remarks></remarks>
    ///  
    public static class StreamUT
    {

        /// <summary>
        /// Copia il contenuto di uno stream su un altro stream indicando la dimensione del buffer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="bufferLen"></param>
        /// <returns></returns>
        public static int Copia(Stream input, Stream output, int bufferLen)
        {
            const int bLen = 4096;
            byte[] buff = new byte[4097];
            int totRead = 0;
            int bRead = 0;

            // Si posiziona ad inizio stream
            input.Position = 0;

            // Copia
            bRead = input.Read(buff, 0, bLen);
            while (bRead > 0)
            {
                output.Write(buff, 0, bRead);
                totRead += bRead;
                bRead = input.Read(buff, 0, bLen);
            }

            return totRead;
        }

        /// <summary>
        /// Copia stream su altro utilizzando un buffer di 32K 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static int Copia(Stream input, Stream output)
        {
            return Copia(input, output, Int16.MaxValue);
        }

    }

}
