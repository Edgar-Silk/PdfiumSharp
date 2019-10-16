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

      string dir = AppDomain.CurrentDomain.BaseDirectory;
      string File = "test.pdf";

      Console.WriteLine("\nOpen PDF file: " + File);

      string all = dir + File;

      Console.WriteLine(all);

      //IntPtr doc = Native.FPDF_LoadDocument(File, null);
      PDF.Load(all);

      Console.WriteLine("\nNumber of Pages:" + PDF.PageCount().ToString());

      int expectedResult = PDF.PageCount();

      Assert.That(expectedResult, Is.EqualTo(311));

    }

   [Test]
    public void AddTest() 
   {
      string path = AppDomain.CurrentDomain.BaseDirectory;
      string dir = NUnit.Framework.TestContext.CurrentContext.TestDirectory;
      Console.WriteLine(dir);
      Console.WriteLine(path);
      Assert.That(5, Is.EqualTo(5));
   }
    
  }
}
