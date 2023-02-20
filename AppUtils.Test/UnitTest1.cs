using AppUtils.Lib.Tracciati;
using System.Text;

namespace AppUtils.Test
{
    [TestClass]
    public class UnitTest1
    {
        private Encoding encoding;

        [TestMethod]
        public void TestMethod1()
        {

            using var f = new FixedFileWriter(@"C:\Users\simone.pelaia\Desktop\LAV\aa.txt", Encoding.ASCII);

            f.AggiungiCampo(0, 30, "simone", ' ');
            f.AggiungiCampo(30, 30, "pelaia", ' ');
            f.AggiungiCampo(60, 10, DateTime.Today.ToString("yyyyMMdd"), '-', true);
            f.ScriviRecord();
            f.AggiungiCampo(0, 30, "mario", ' ');
            f.AggiungiCampo(30, 30, "rosssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss", ' ');
            f.AggiungiCampo(60, 10, DateTime.Today.ToString("yyyyMMdd"), '-', true);
            f.ScriviRecord();
            f.AggiungiCampo(0, 30, "dario", ' ');
            f.AggiungiCampo(30, 30, "verdi", ' ');
            f.AggiungiCampo(60, 10, DateTime.Today.ToString("yyyyMMdd"), '0', false);
            f.ScriviRecord();
            f.Dispose();
        }
    }
}