using System;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using PdfiumSharp;

namespace Test {
  [TestFixture]
  public class PdfiumTest {
    public PDF pdf;

    [OneTimeSetUp]
    public void SetUp() {

      pdf = new PDF();

      string path = AppDomain.CurrentDomain.BaseDirectory;
      string fileName = "test.pdf";

      string file = path + fileName;

      Console.WriteLine("\nOpen PDF file: " + file);

      pdf.Load(file);
    }

    [OneTimeTearDown]
    public void TearDown() {
      Console.WriteLine("\nObject to Null");
      pdf = null;
    }

    [Test]
    public void MetaInfoTest() {

      Console.WriteLine("\nNumber of Pages:" + pdf.PageCount().ToString());

      var info = pdf.GetInformation();
      Console.WriteLine("\nCreator: " + info.Creator);
      Console.WriteLine("\nTitle: " + info.Title);
      Console.WriteLine("\nAuthor: " + info.Author);
      Console.WriteLine("\nSubject: " + info.Subject);
      Console.WriteLine("\nKeywords: " + info.Keywords);
      Console.WriteLine("\nProducer: " + info.Producer);
      Console.WriteLine("\nCreationDate: " + info.CreationDate);
      Console.WriteLine("\nModDate: " + info.ModificationDate);

      int expectedResult = pdf.PageCount();

      Assert.That(expectedResult, Is.EqualTo(1000));

    }

    [Test]
    public void ParserImageTest() {
      int pageNumber = 1;
      int width = 460;
      int height = 520;
      int dpiX = 460;
      int dpiY = 520;

      Image i = pdf.Render(pageNumber, width, height, dpiX, dpiY);


      string imageType = (i.RawFormat).ToString();

      Console.WriteLine("Image type: " + imageType);
      Assert.That(i.RawFormat.Equals(ImageFormat.MemoryBmp));

      i.Dispose();

    }

  }
}

