using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;

namespace ImageLinuxOS
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            PDFfileGenerate();
        }

        public static void PDFfileGenerate()
        {
            string pdfFileName = string.Empty;
            try
            {
                System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
                string path = System.IO.Path.GetDirectoryName(asm.Location);

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                const string police = "Verdana";

                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont titleFont = new XFont(police, 16, XFontStyle.Bold);
                XFont regularFont = new XFont(police, 12, XFontStyle.Regular);
                XFont boldFont = new XFont(police, 12, XFontStyle.Bold);
                XFont italicFont = new XFont(police, 12, XFontStyle.Italic);
                XFont underlineFont = new XFont(police, 12, XFontStyle.Underline);

                XTextFormatter tf = new XTextFormatter(gfx);

                gfx.DrawString("PREVIEW", titleFont, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
                gfx.DrawString("Pack N° 1", titleFont, XBrushes.Black, new XRect(0, 32, page.Width, page.Height), XStringFormats.Center);

                var stream = new MemoryStream(File.ReadAllBytes(path + "/Images/Logo/LOG_Medline.jpg"));
                XImage image = XImage.FromStream(() => stream);

                gfx.DrawImage(image, 5, 5, 128, 128);

                var assemblyStep = 0;

                foreach (string file in Directory.EnumerateFiles(path + "/Images"))
                {
                    assemblyStep++;

                    PdfPage pageStep = document.AddPage();
                    XGraphics gfxStep = XGraphics.FromPdfPage(pageStep);

                    gfxStep.DrawImage(image, 5, 5, 128, 128);

                    gfxStep.DrawString("PREVIEW PACK N° 1", titleFont, XBrushes.Black, new XRect(150, 5, page.Width, page.Height), XStringFormats.TopLeft);
                    gfxStep.DrawString("STEP " + assemblyStep, titleFont, XBrushes.Black, new XRect(-5, 5, page.Width, page.Height), XStringFormats.TopRight);
                    gfxStep.DrawString((assemblyStep + 1) + "/" + 10, titleFont, XBrushes.Black, new XRect(-5, -5, page.Width, page.Height), XStringFormats.BottomRight);

                    var memoryStreamStep = new MemoryStream(File.ReadAllBytes(file));

                    MemoryStream strm = new MemoryStream();
                    Image img = Image.FromStream(memoryStreamStep);
                    //img.Save(strm, System.Drawing.Imaging.ImageFormat.Png);
                    img.Save(strm, System.Drawing.Imaging.ImageFormat.Jpeg);
                    strm.Position = 0;
                    XImage imageStep = XImage.FromStream(() => strm);
                    //XImage imageStep = XImage.FromFile(file);

                    XUnit height = XUnit.FromInch(imageStep.PixelHeight / imageStep.VerticalResolution);
                    XUnit width = XUnit.FromInch(imageStep.PixelWidth / imageStep.HorizontalResolution);

                    double ratioX = 0;
                    double ratioY = 0;
                    if (page.Width.Presentation > imageStep.PixelWidth)
                        ratioX = imageStep.PixelWidth / page.Width.Presentation;
                    else
                        ratioX = page.Width.Presentation / imageStep.PixelWidth;

                    if ((page.Height.Presentation - 300) > imageStep.PixelHeight)
                        ratioY = imageStep.PixelHeight / (page.Height.Presentation - 300);
                    else
                        ratioY = (page.Height.Presentation - 300) / imageStep.PixelHeight;

                    double ratio = ratioX < ratioY ? ratioX : ratioY;

                    gfxStep.DrawImage(imageStep, (page.Width / 2) - ((width.Presentation * ratio) / 2), 200, width.Presentation * ratio, height.Presentation * ratio);
                    gfxStep.DrawString("SOFT BILL ", titleFont, XBrushes.Black, new XRect(5, -130, page.Width, page.Height), XStringFormats.BottomLeft);

                    /*int lineSpace = 32;
                    foreach (var softBill in assemblyStep.SoftBills)
                    {
                        gfxStep.DrawString(softBill.ComponentPrmsProductNumber, regularFont, XBrushes.Black, new XRect(15, -130 + lineSpace, page.Width, page.Height), XStringFormats.BottomLeft);
                        gfxStep.DrawString(softBill.Quantity.ToString(), regularFont, XBrushes.Black, new XRect(page.Width / 3, -130 + lineSpace, page.Width, page.Height), XStringFormats.BottomLeft);
                        gfxStep.DrawString(softBill.UM, regularFont, XBrushes.Black, new XRect(page.Width / 1.5, -130 + lineSpace, page.Width, page.Height), XStringFormats.BottomLeft);

                        lineSpace += 32;
                    }
                    */
                    memoryStreamStep.Close();
                    memoryStreamStep.Dispose();
                }

                pdfFileName = @"/Images/PDFReview/" + DateTime.Now.ToString("yyyy-dd-MM-HH-mm-ss") + ".pdf";
                document.Save(path + pdfFileName);
                /*MemoryStream streamResult = new MemoryStream();
                document.Save(streamResult, true);

                var byteArray = streamResult.ToArray();
                stream.Close();
                stream.Dispose();
                streamResult.Close();
                streamResult.Dispose();*/
                document.Close();
                document.Dispose();

                Console.WriteLine("PDF preview file is generated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Message:-" + ex.Message);
            }
        }
    }
}