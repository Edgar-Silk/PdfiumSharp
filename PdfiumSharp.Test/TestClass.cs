using System;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;


namespace PdfiumSharp.Test
{
  [TestFixture]
  public class PdfiumTest
  {
    static PDF P;

    [OneTimeSetUp]
    public void SetUp() 
    {
      // Initialize the library
      P = new PDF();

      // Load the pdf file
      string Path = AppDomain.CurrentDomain.BaseDirectory;
      string FileName = "test.pdf";

      string File = Path + FileName;

      Console.WriteLine("\nOpen PDF file: " + File);

      PDF.Load(File);
    }

    [OneTimeTearDown]
    public void TearDown() 
    {
      P = null;
    }

    [Test]
    public void MetaInfoTest() 
    {
      
      Console.WriteLine("\nNumber of Pages:" + PDF.PageCount().ToString());

      var inf = PDF.GetInformation();
      Console.WriteLine("\nCreator: " + inf.Creator);
      Console.WriteLine("\nTitle: " + inf.Title);
      Console.WriteLine("\nAuthor: " + inf.Author);
      Console.WriteLine("\nSubject: " + inf.Subject);
      Console.WriteLine("\nKeywords: " + inf.Keywords);
      Console.WriteLine("\nProducer: " + inf.Producer);
      Console.WriteLine("\nCreationDate: " + inf.CreationDate);
      Console.WriteLine("\nModDate: " + inf.ModificationDate);

      int expectedResult = PDF.PageCount();

      Assert.That(expectedResult, Is.EqualTo(1000));

    }

   [Test]
    public void ParserImageTest() 
   {
      int pageNumber  = 1;
      int width = 460;
      int height = 520;
      int dpiX = 460;
      int dpiY = 520;

      Image i = PDF.Render(pageNumber, width, height, dpiX, dpiY);

      string imageType = new ImageFormatConverter().ConvertToString(i.RawFormat);
                    
      Console.WriteLine("Image type: " + imageType);
      Assert.That(i.RawFormat.Equals(ImageFormat.MemoryBmp));

      i.Dispose();

   }
    
  }
}
