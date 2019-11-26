using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace PdfiumSharp
{

    public class PdfInformation
    {
        public string Author { get; set; }
        public string Creator { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Keywords { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string Producer { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
    }

    public static class CallNative
    {

        [Flags]
        public enum FPDF
        {
            ANNOT = 0x01,
            LCD_TEXT = 0x02,
            NO_NATIVETEXT = 0x04,
            GRAYSCALE = 0x08,
            DEBUG_INFO = 0x80,
            NO_CATCH = 0x100,
            RENDER_LIMITEDIMAGECACHE = 0x200,
            RENDER_FORCEHALFTONE = 0x400,
            PRINTING = 0x800,
            REVERSE_BYTE_ORDER = 0x10
        }
        public static void GetFPDFInitLibrary()
        {
            NativeMethods.FPDF_InitLibrary();
        }

        public static void GetFPDFDestroyLibrary()
        {
            NativeMethods.FPDF_DestroyLibrary();
        }

        public static IntPtr GetFPDFLoadDocument(string filepath, string password)
        {

            return NativeMethods.FPDF_LoadDocument(filepath, password);
        }

        public static int GetFPDFGetPageCount(IntPtr document)
        {

            return NativeMethods.FPDF_GetPageCount(document);
        }

        public static string GetMetaText(IntPtr document, string tag)
        {

            uint length = NativeMethods.FPDF_GetMetaText(document, tag, null, 0);

            if (length <= 2)
                return string.Empty;

            byte[] buffer = new byte[length];
            NativeMethods.FPDF_GetMetaText(document, tag, buffer, length);

            return Encoding.Unicode.GetString(buffer, 0, (int)(length - 2));
        }

        public static DateTime? GetMetaTextAsDate(IntPtr document, string tag)
        {
            string dt = CallNative.GetMetaText(document, tag);
            DateTime parseDate;

            if (string.IsNullOrEmpty(dt))
                return null;

            Regex dtRegex =
                new Regex(
                    @"(?:D:)(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)(?<tz_offset>[+-zZ])?(?<tz_hour>\d\d)?'?(?<tz_minute>\d\d)?'?");

            Match match = dtRegex.Match(dt);

            if (match.Success)
            {
                var year = match.Groups["year"].Value;
                var month = match.Groups["month"].Value;
                var day = match.Groups["day"].Value;
                var hour = match.Groups["hour"].Value;
                var minute = match.Groups["minute"].Value;
                var second = match.Groups["second"].Value;
                var tzOffset = match.Groups["tz_offset"]?.Value;
                var tzHour = match.Groups["tz_hour"]?.Value;
                var tzMinute = match.Groups["tz_minute"]?.Value;

                string formattedDate = $"{year}-{month}-{day}T{hour}:{minute}:{second}.0000000";

                if (!string.IsNullOrEmpty(tzOffset))
                {
                    switch (tzOffset)
                    {
                        case "Z":
                        case "z":
                            formattedDate += "+0";
                            break;
                        case "+":
                        case "-":
                            formattedDate += $"{tzOffset}{tzHour}:{tzMinute}";
                            break;
                    }
                }

                if (!DateTime.TryParse(formattedDate, out parseDate))
                    throw new FormatException();

                return parseDate;
            }

            return null;
        }

        public static IntPtr GetFPDFBitmapCreateEx(int width, int height, int format, IntPtr firstScan, int stride)
        {
            return NativeMethods.FPDFBitmap_CreateEx(width, height, format, firstScan, stride);
        }

        public static void GetFPDFBitmapFillRect(IntPtr bitmapHandle, int left, int top, int width, int height, uint color)
        {
            NativeMethods.FPDFBitmap_FillRect(bitmapHandle, left, top, width, height, color);
        }

        public static IntPtr GetFPDFBitmapDestroy(IntPtr bitmapHandle)
        {
            return NativeMethods.FPDFBitmap_Destroy(bitmapHandle);
        }

        public static void GetFPDFRenderPageBitmap(IntPtr bitmapHandle, IntPtr page, int startX, int startY, int sizeX, int sizeY, int rotate, FPDF flags)
        {
            NativeMethods.FPDF_RenderPageBitmap(bitmapHandle, page, startX, startY, sizeX, sizeY, rotate, flags);
        }

        public static IntPtr GetFPDFLoadPage(IntPtr document, int pageIndex)
        {
            return NativeMethods.FPDF_LoadPage(document, pageIndex);
        }

        public static IntPtr GetFPDFTextLoadPage(IntPtr page)
        {
            return NativeMethods.FPDFText_LoadPage(page);
        }

        public class PageData : IDisposable
        {
            public IntPtr Page { get; private set; }
            public IntPtr TextPage { get; private set; }

            private bool _disposed;

            public PageData(IntPtr document, int pageNumber)
            {
                Page = GetFPDFLoadPage(document, pageNumber);
                TextPage = GetFPDFTextLoadPage(Page);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~PageData()
            {
                Dispose(false);
            }

            protected virtual void Dispose(bool _disposing)
            {
                if (!_disposed)
                {
                    _disposed = true;
                }
                else
                {
                    _disposed = _disposing;
                }

            }

        }

    }

    internal static class NativeMethods
    {

        [DllImport("pdfium.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern void FPDF_InitLibrary();

        [DllImport("pdfium.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern void FPDF_DestroyLibrary();

        [DllImport("pdfium.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr FPDF_LoadDocument([MarshalAs(UnmanagedType.LPStr)] string filepath, [MarshalAs(UnmanagedType.LPStr)] string password);

        [DllImport("pdfium.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern int FPDF_GetPageCount(IntPtr document);

        [DllImport("pdfium.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern uint FPDF_GetMetaText(IntPtr document, string tag, byte[] buffer, uint buflen);

        [DllImport("pdfium.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr FPDFBitmap_CreateEx(int width, int height, int format, IntPtr first_scan, int stride);

        [DllImport("pdfium.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern void FPDFBitmap_FillRect(IntPtr bitmapHandle, int left, int top, int width, int height, uint color);

        [DllImport("pdfium.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr FPDFBitmap_Destroy(IntPtr bitmapHandle);

        [DllImport("pdfium.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern void FPDF_RenderPageBitmap(IntPtr bitmapHandle, IntPtr page, int start_x, int start_y, int size_x, int size_y, int rotate, CallNative.FPDF flags);

        [DllImport("pdfium.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr FPDF_LoadPage(IntPtr document, int page_index);

        [DllImport("pdfium.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr FPDFText_LoadPage(IntPtr page);
    }


    public class Pdfium
    {

        private IntPtr _doc;

        public Pdfium()
        {
            CallNative.GetFPDFInitLibrary();
        }

        ~Pdfium()
        {
            CallNative.GetFPDFDestroyLibrary();
        }

        public bool LoadFile(string F)
        {

            string file = F;

            _doc = CallNative.GetFPDFLoadDocument(file, null);

            if (_doc == IntPtr.Zero) return false;
            else return true;

        }

        public int PageCount()
        {

            return CallNative.GetFPDFGetPageCount(_doc);

        }

        public PdfInformation GetInformation()
        {
            var pdfInfo = new PdfInformation();

            pdfInfo.Creator = CallNative.GetMetaText(_doc, "Creator");
            pdfInfo.Title = CallNative.GetMetaText(_doc, "Title");
            pdfInfo.Author = CallNative.GetMetaText(_doc, "Author");
            pdfInfo.Subject = CallNative.GetMetaText(_doc, "Subject");
            pdfInfo.Keywords = CallNative.GetMetaText(_doc, "Keywords");
            pdfInfo.Producer = CallNative.GetMetaText(_doc, "Producer");
            pdfInfo.CreationDate = CallNative.GetMetaTextAsDate(_doc, "CreationDate");
            pdfInfo.ModificationDate = CallNative.GetMetaTextAsDate(_doc, "ModDate");

            return pdfInfo;
        }

        public bool RenderPDFPageToBitmap(int pageNumber, IntPtr bitmapHandle, int dpiX, int dpiY,
                                                    int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight,
                                                    int rotate, CallNative.FPDF flags, bool renderFormFill)
        {

            using (var pageData = new CallNative.PageData(_doc, pageNumber))
            {

                CallNative.GetFPDFRenderPageBitmap(bitmapHandle, pageData.Page, boundsOriginX, boundsOriginY,
                                            boundsWidth, boundsHeight, 0, flags);

            }

            return true;
        }

        public Image Render(int page, int width, int height, float dpiX, float dpiY)
        {

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(dpiX, dpiY);

            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            try
            {
                var handle = CallNative.GetFPDFBitmapCreateEx(width, height, 4, data.Scan0, width * 4);

                uint background = 0xFFFFFFFF;
                CallNative.GetFPDFBitmapFillRect(handle, 0, 0, width, height, background);

                this.RenderPDFPageToBitmap(
                    page,
                    handle,
                    (int)dpiX, (int)dpiY,
                    0, 0, width, height,
                    0,
                    0,
                    false);

                CallNative.GetFPDFBitmapDestroy(handle);

            }
            catch (Exception ex)
            {
                Console.Write("Unable to render PDF image.");
                throw new Win32Exception("RenderFPDFPageToBitmap.", ex);

            }

            finally
            {
                bitmap.UnlockBits(data);
            }

            return bitmap;
        }

    }
}
