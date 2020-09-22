using System;
using System.Collections.Generic;
using System.Text;

namespace AppUtils.Lib.Common
{

    /// <summary>
    /// Codifiche caratteri
    /// </summary>
    public static class CharCodes
    {

        /// <summary>
        /// Null char
        /// </summary>
        /// <remarks></remarks>
        public static readonly char NUL = Convert.ToChar(0);

        /// <summary>
        /// Start of header
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SOH = Convert.ToChar(1);

        /// <summary>
        /// Start of text
        /// </summary>
        /// <remarks></remarks>
        public static readonly char STX = Convert.ToChar(2);

        /// <summary>
        /// End of text
        /// </summary>
        /// <remarks></remarks>
        public static readonly char ETX = Convert.ToChar(3);

        /// <summary>
        /// End of transmission
        /// </summary>
        /// <remarks></remarks>
        public static readonly char EOT = Convert.ToChar(4);

        /// <summary>
        /// Enquiry
        /// </summary>
        /// <remarks></remarks>
        public static readonly char ENQ = Convert.ToChar(3);

        /// <summary>
        /// Acknowledge
        /// </summary>
        /// <remarks></remarks>
        public static readonly char ACK = Convert.ToChar(6);

        /// <summary>
        /// Bell
        /// </summary>
        /// <remarks></remarks>
        public static readonly char BEL = Convert.ToChar(7);

        /// <summary>
        /// Backspace
        /// </summary>
        /// <remarks></remarks>
        public static readonly char BS = Convert.ToChar(8);

        /// <summary>
        /// Horizontal tab
        /// </summary>
        /// <remarks></remarks>
        public static readonly char TAB = Convert.ToChar(9);

        /// <summary>
        /// Line feed
        /// </summary>
        /// <remarks></remarks>
        public static readonly char LF = Convert.ToChar(10);

        /// <summary>
        /// Vertical tab
        /// </summary>
        /// <remarks></remarks>
        public static readonly char VT = Convert.ToChar(11);

        /// <summary>
        /// NP form feed, new page
        /// </summary>
        /// <remarks></remarks>
        public static readonly char FF = Convert.ToChar(12);

        /// <summary>
        /// Carriage return
        /// </summary>
        /// <remarks></remarks>
        public static readonly char CR = Convert.ToChar(13);

        /// <summary>
        /// Shift out
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SO = Convert.ToChar(14);

        /// <summary>
        /// Shift in
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SI = Convert.ToChar(15);

        /// <summary>
        /// Data link escape
        /// </summary>
        /// <remarks></remarks>
        public static readonly char DLE = Convert.ToChar(16);

        /// <summary>
        /// Device control 1
        /// </summary>
        /// <remarks></remarks>
        public static readonly char DC1 = Convert.ToChar(17);

        /// <summary>
        /// Device control 2
        /// </summary>
        /// <remarks></remarks>
        public static readonly char DC2 = Convert.ToChar(18);

        /// <summary>
        /// Device control 3
        /// </summary>
        /// <remarks></remarks>
        public static readonly char DC3 = Convert.ToChar(19);

        /// <summary>
        /// Device control 4
        /// </summary>
        /// <remarks></remarks>
        public static readonly char DC4 = Convert.ToChar(20);

        /// <summary>
        /// Negative acknowledge
        /// </summary>
        /// <remarks></remarks>
        public static readonly char NAK = Convert.ToChar(21);

        /// <summary>
        /// Synchronous idle
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYN = Convert.ToChar(22);

        /// <summary>
        /// End of transmission block
        /// </summary>
        /// <remarks></remarks>
        public static readonly char ETB = Convert.ToChar(23);

        /// <summary>
        /// Cancel
        /// </summary>
        /// <remarks></remarks>
        public static readonly char CAN = Convert.ToChar(24);

        /// <summary>
        /// End of medium
        /// </summary>
        /// <remarks></remarks>
        public static readonly char EM = Convert.ToChar(25);

        /// <summary>
        /// Substitute
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SUBS = Convert.ToChar(26);

        /// <summary>
        /// Escape
        /// </summary>
        /// <remarks></remarks>
        public static readonly char ESC = Convert.ToChar(27);

        /// <summary>
        /// File separator
        /// </summary>
        /// <remarks></remarks>
        public static readonly char FS = Convert.ToChar(28);

        /// <summary>
        /// Group separator
        /// </summary>
        /// <remarks></remarks>
        public static readonly char GS = Convert.ToChar(29);

        /// <summary>
        /// Record separator
        /// </summary>
        /// <remarks></remarks>
        public static readonly char RS = Convert.ToChar(30);

        /// <summary>
        /// Unit separator
        /// </summary>
        /// <remarks></remarks>
        public static readonly char US = Convert.ToChar(31);

        /// <summary>
        /// Blank
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SPACE = Convert.ToChar(32);

        /// <summary>
        /// Carattere !
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_ESCL = Convert.ToChar(33);

        /// <summary>
        /// Carattere "
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_QUOTE = Convert.ToChar(34);

        /// <summary>
        /// Carattere #
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_HASH = Convert.ToChar(35);

        /// <summary>
        /// Carattere $
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_DOLL = Convert.ToChar(36);

        /// <summary>
        /// Carattere %
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_PERC = Convert.ToChar(37);

        /// <summary>
        /// Carattere &amp;
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_AMP = Convert.ToChar(38);

        /// <summary>
        /// Carattere &quot;
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_APOS = Convert.ToChar(39);

        /// <summary>
        /// Carattere (
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_PARL = Convert.ToChar(40);

        /// <summary>
        /// Carattere )
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_PARR = Convert.ToChar(41);

        /// <summary>
        /// Carattere *
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_MULT = Convert.ToChar(42);

        /// <summary>
        /// Carattere +
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_PLUS = Convert.ToChar(43);

        /// <summary>
        /// Carattere ,
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_COMMA = Convert.ToChar(44);

        /// <summary>
        /// Carattere -
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_MINU = Convert.ToChar(45);

        /// <summary>
        /// Carattere .
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_POINT = Convert.ToChar(46);

        /// <summary>
        /// Carattere /
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_SLASH = Convert.ToChar(47);


        public static readonly char NUM_0 = Convert.ToChar(48);
        public static readonly char NUM_1 = Convert.ToChar(49);
        public static readonly char NUM_2 = Convert.ToChar(50);
        public static readonly char NUM_3 = Convert.ToChar(51);
        public static readonly char NUM_4 = Convert.ToChar(52);
        public static readonly char NUM_5 = Convert.ToChar(53);
        public static readonly char NUM_6 = Convert.ToChar(54);
        public static readonly char NUM_7 = Convert.ToChar(55);
        public static readonly char NUM_8 = Convert.ToChar(56);
        public static readonly char NUM_9 = Convert.ToChar(57);

        /// <summary>
        /// Carattere :
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_DPOINT = Convert.ToChar(58);

        /// <summary>
        /// Carattere ;
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_SEMICOL = Convert.ToChar(59);

        /// <summary>
        /// Carattere &lt;
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_LT = Convert.ToChar(60);

        /// <summary>
        /// Carattere =
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_EQ = Convert.ToChar(61);

        /// <summary>
        /// Carattere &gt;
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_GT = Convert.ToChar(62);

        /// <summary>
        /// Carattere ?
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_QUESTION = Convert.ToChar(63);

        /// <summary>
        /// Carattere @
        /// </summary>
        /// <remarks></remarks>
        public static readonly char SYM_AT = Convert.ToChar(64);

        // Lettere maiuscole
        public static readonly char UC_A = Convert.ToChar(65);
        public static readonly char UC_B = Convert.ToChar(66);
        public static readonly char UC_C = Convert.ToChar(67);
        public static readonly char UC_D = Convert.ToChar(68);
        public static readonly char UC_E = Convert.ToChar(69);
        public static readonly char UC_F = Convert.ToChar(70);
        public static readonly char UC_G = Convert.ToChar(71);
        public static readonly char UC_H = Convert.ToChar(72);
        public static readonly char UC_I = Convert.ToChar(73);
        public static readonly char UC_J = Convert.ToChar(74);
        public static readonly char UC_K = Convert.ToChar(75);
        public static readonly char UC_L = Convert.ToChar(76);
        public static readonly char UC_M = Convert.ToChar(77);
        public static readonly char UC_N = Convert.ToChar(78);
        public static readonly char UC_O = Convert.ToChar(79);
        public static readonly char UC_P = Convert.ToChar(80);
        public static readonly char UC_Q = Convert.ToChar(81);
        public static readonly char UC_R = Convert.ToChar(82);
        public static readonly char UC_S = Convert.ToChar(83);
        public static readonly char UC_T = Convert.ToChar(84);
        public static readonly char UC_U = Convert.ToChar(85);
        public static readonly char UC_V = Convert.ToChar(86);
        public static readonly char UC_W = Convert.ToChar(87);
        public static readonly char UC_X = Convert.ToChar(88);
        public static readonly char UC_Y = Convert.ToChar(89);
        public static readonly char UC_Z = Convert.ToChar(90);
    }

}
