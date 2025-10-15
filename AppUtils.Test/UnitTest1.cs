using AppUtils.Lib.Banca;
using AppUtils.Lib.Common;
using AppUtils.Lib.Tracciati;
using System.Text;

namespace AppUtils.Test
{
    [TestClass]
    public class UnitTest1
    {
        private Encoding encoding;


        [TestMethod]
        public void TestIban()
        {

            var iban = new Iban("IT001234567890000000000000");
            iban.ValidateFormalNoException();
            Assert.ThrowsException<ArgumentException>(() => iban.ValidateFormal());
        }


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

        [TestMethod]
        public void TestMethodDates()
        {

            var d1 = new DateTime(2024, 1, 1);
            var d2 = new DateTime(2024, 12, 31);
            var mid = DateUT.MinData(d1, d2);
            Assert.AreEqual(d1, mid);

            var mad = DateUT.MaxData(d1, d2);
            Assert.AreEqual(d2, mad);

            Assert.AreEqual(DateUT.MinData(null), DateTime.MinValue);
            Assert.AreEqual(DateUT.MinData(new DateTime[] {}), DateTime.MinValue);

            Assert.AreEqual(DateUT.MaxData(null), DateTime.MinValue);
            Assert.AreEqual(DateUT.MaxData(new DateTime[] { }), DateTime.MinValue);
        }
    }
}