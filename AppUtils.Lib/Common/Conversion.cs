using System;

/// <summary>
/// Classe con metodi per convertire numeri in rappresentazioni ennarie su base alfabetica
/// </summary>
public static class Conversion
{

    // Private Const CHARLIST36 As String = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    private const string PROTOCOL_CHARLIST_ENCODE = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const short PROTOCOL_MAX_NUM_DEC = 10000;


    /// <summary>
    ///  Codifica il progressivo del protocollo
    ///  </summary>
    ///  <param name="iNumIn"></param>
    ///  <returns></returns>
    ///  <remarks></remarks>
    public static string CodificaProgressivoProtocollo(long iNumIn, short len)
    {
        var size = (int)Math.Pow(10, len);

        if (iNumIn >= size)
            return Conversion.ConvNumToBase(iNumIn - size, (short)PROTOCOL_CHARLIST_ENCODE.Length, PROTOCOL_CHARLIST_ENCODE).PadLeft(4, PROTOCOL_CHARLIST_ENCODE[0]);
        else
            return iNumIn.ToString().PadLeft(len, '0');
    }

    /// <summary>
    ///  Decodifica il progressivo del protocollo
    ///  </summary>
    ///  <param name="sNumIn"></param>
    ///  <returns></returns>
    ///  <remarks></remarks>
    public static long DecodificaProgressivoProtocollo(string sNumIn)
    {
        long iNum = 0;

        if (!long.TryParse(sNumIn, out iNum))
            iNum = Conversion.ConvBaseToNum(sNumIn, (short)PROTOCOL_CHARLIST_ENCODE.Length, Conversion.PROTOCOL_CHARLIST_ENCODE);

        return iNum;
    }


    /// <summary>
    ///  Codifica numero in formato arbitrario (compreso tra 2 e 36)
    ///  </summary>
    ///  <param name="number"></param>
    ///  <param name="radix"></param>
    ///  <returns></returns>
    ///  <remarks></remarks>
    public static string ConvNumToBase(long number, short radix, string CharList)
    {
        const short BitsInLong = 64;

        // If radix < 2 OrElse radix > CharList.Length Then
        // Throw New ArgumentException("La radice deve essere compresa tra 2 e " + CharList.Length.ToString())
        // End If

        if (number == 0)
            return CharList[0].ToString();

        int index = BitsInLong - 1;
        long currentNumber = Math.Abs(number);
        char[] charArray = new char[BitsInLong + 1];
        int remainder;

        while (currentNumber != 0)
        {
            remainder = (int)(currentNumber % radix);
            charArray[index] = CharList[(int)remainder];
            index -= 1;
            currentNumber = currentNumber / radix;
        }

        string result = new string(charArray, index + 1, BitsInLong - index - 1);
        
        if (number < 0)
            result = "-" + result;

        return result;
    }


    /// <summary>
    ///  Decodifica stringa in formato arbitrario (tra 2 e 36) in formato decimale
    ///  </summary>
    ///  <param name="number"></param>
    ///  <param name="radix"></param>
    ///  <returns></returns>
    ///  <remarks></remarks>
    public static long ConvBaseToNum(string number, short radix, string CharList)
    {
        // If radix < 2 OrElse radix > CharList.Length Then
        // Throw New ArgumentException("La radice deve essere compresa tra 2 e " + CharList.Length.ToString())
        // End If

        if (string.IsNullOrEmpty(number))
            return 0L;

        number = number.ToUpperInvariant();

        long result = 0L;
        long multiplier = 1L;
        int lastChar = 0;

        // Se minore di zero
        if (number[0] == '-')
            lastChar = 1;


        for (int index = number.Length - 1; index >= lastChar; index += -1)
        {
            char c = number[index];
            int digit = CharList.IndexOf(c);

            if (digit == -1)
                throw new ArgumentException($"Carattere '{c}' non valido");

            result += digit * multiplier;
            multiplier *= radix;
        }

        // Se minore di zero
        if (lastChar > 0)
            result *= -1L;

        return result;
    }
}
