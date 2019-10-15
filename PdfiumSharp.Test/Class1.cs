using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace PdfiumSharp.Test
{
  [TestFixture]
  public class PdfiumTest
  {
    [Test]
    public void LoaderTest() 
    {
      PDF P = new PDF();
      string File = "test.pdf";

      Console.WriteLine("\nOpen PDF file: " + File);

      //IntPtr doc = Native.FPDF_LoadDocument(File, null);
      PDF.Load(File);

      Console.WriteLine("\nNumber of Pages:" + PDF.PageCount().ToString());

      int expectedResult = PDF.PageCount();

      Assert.That(expectedResult, Is.EqualTo(31));

    }

  }
}
