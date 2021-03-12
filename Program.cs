using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace ImageAnnotations
{
    static class Program
    {
        const string LongText = @"Lorem: ipsum dolor sit amet, consectetur adipiscing elit. Donec aliquet lorem at magna mollis, non semper erat aliquet. In leo tellus, sollicitudin non eleifend et, luctus vel magna. Proin at lacinia tortor, malesuada molestie nisl. Quisque mattis dui quis eros ultricies, quis faucibus turpis dapibus. Donec urna ipsum, dignissim eget condimentum at, condimentum non magna. Donec non urna sit amet lectus tincidunt interdum vitae vitae leo. Aliquam in nisl accumsan, feugiat ipsum condimentum, scelerisque diam. Vivamus quam diam, rhoncus ut semper eget, gravida in metus.";

        static void Main(string[] args)
        {
            Directory.CreateDirectory("output");

            using (var img = Image.Load(@"camera.jpg"))
            {
                using (var img2 = img.Clone(ctx => ctx.AddCaption(LongText, Color.White, Color.Black)))
                {
                    img2.Save("output/simple.jpg");
                }
            }
        }

        private static IImageProcessingContext AddCaption(this IImageProcessingContext processingContext, string text, Color color, Color backgroundColor)
        {
            Size imgSize = processingContext.GetCurrentSize();

            float defaultFontSize = 12;
            float defaultResolution = 645;
            float defaultPadding = 10;

            float fontSize = imgSize.Width * defaultFontSize / defaultResolution;
            float padding = imgSize.Width * defaultPadding / defaultResolution;
            float captionWidth = imgSize.Width - (2 * padding);

           
            FontCollection collection = new FontCollection();
            FontFamily family = collection.Install("Roboto/Roboto-Regular.ttf");
            Font font = family.CreateFont(fontSize, FontStyle.Regular);

            // measure the text size
            FontRectangle fontRectangle = TextMeasurer.Measure(text, new RendererOptions(font) { WrappingWidth = captionWidth });

            var location = new PointF(padding, imgSize.Height + padding);
            var textGraphicOptions = new TextGraphicsOptions()
            {
                TextOptions = { WrapTextWidth = captionWidth }
            };

            var resizeOptions = new ResizeOptions()
            {
                // increse image height to include caption height
                Size = new Size(imgSize.Width, imgSize.Height + (int)fontRectangle.Height + (int)(2 * padding)),
                Mode = ResizeMode.BoxPad,
                Position = AnchorPositionMode.Top
            };

            return processingContext
                .Resize(resizeOptions)
                .BackgroundColor(backgroundColor)
                .DrawText(textGraphicOptions, text, font, color, location);
        }
    }
}

