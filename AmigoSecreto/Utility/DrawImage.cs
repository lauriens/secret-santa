using System.Drawing;

namespace AmigoSecreto.Utility
{
    public static class DrawImage
    {
        public static Image DrawText(string text, Font? fontOptional = null, Color? textColorOptional = null, Color? backColorOptional = null)
        {
            Font font = SystemFonts.DefaultFont;
            if (fontOptional != null)
                font = fontOptional;

            Color textColor = Color.Black;
            if (textColorOptional != null)
                textColor = (Color)textColorOptional;

            Color backColor = Color.White;
            if (backColorOptional != null)
                backColor = (Color)backColorOptional;

            Size minSize = Size.Empty;

            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    textSize = drawing.MeasureString(text, font);
                    if (!minSize.IsEmpty)
                    {
                        textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
                        textSize.Height = textSize.Height > minSize.Height ? textSize.Height : minSize.Height;
                    }
                }
            }

            //create a new image of the right size
            Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(backColor);

                //create a brush for the text
                using Brush textBrush = new SolidBrush(textColor);
                drawing.DrawString(text, font, textBrush, 0, 0);
                drawing.Save();
            }
            return retImg;
        }
    }
}
