using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommonControls.CommonMethods.ImgWork
{
    public class ImgRotation
    {
        /// <summary>
        /// Creates a new Image containing the same image only rotated
        /// </summary>
        /// <param name="image">The <see cref="System.Drawing.Image"/> to rotate</param>
        /// <param name="angle">The amount to rotate the image, clockwise, in degrees</param>
        /// <returns>A new <see cref="System.Drawing.Bitmap"/> that is just large enough
        /// to contain the rotated image without cutting any corners off.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <see cref="image"/> is null.</exception>
        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            // angle fomt 0 to 360
            if (angle > 360)
            {
                angle = angle - (((int)(angle / 360)) * 360);
            }

            const double pi2 = Math.PI / 2.0;

            // Why can't C# allow these to be const, or at least readonly
            // *sigh*  I'm starting to talk like Christian Graus :omg:
            double oldWidth = (double)image.Width;
            double oldHeight = (double)image.Height;

            // Convert degrees to radians
            double theta = ((double)angle) * Math.PI / 180.0;
            double locked_theta = theta;

            // Ensure theta is now [0, 2pi)
            while (locked_theta < 0.0)
                locked_theta += 2 * Math.PI;

            double newWidth, newHeight;
            int nWidth, nHeight; // The newWidth/newHeight expressed as ints

            #region Explaination of the calculations
            /*
			 * The trig involved in calculating the new width and height
			 * is fairly simple; the hard part was remembering that when 
			 * PI/2 <= theta <= PI and 3PI/2 <= theta < 2PI the width and 
			 * height are switched.
			 * 
			 * When you rotate a rectangle, r, the bounding box surrounding r
			 * contains for right-triangles of empty space.  Each of the 
			 * triangles hypotenuse's are a known length, either the width or
			 * the height of r.  Because we know the length of the hypotenuse
			 * and we have a known angle of rotation, we can use the trig
			 * function identities to find the length of the other two sides.
			 * 
			 * sine = opposite/hypotenuse
			 * cosine = adjacent/hypotenuse
			 * 
			 * solving for the unknown we get
			 * 
			 * opposite = sine * hypotenuse
			 * adjacent = cosine * hypotenuse
			 * 
			 * Another interesting point about these triangles is that there
			 * are only two different triangles. The proof for which is easy
			 * to see, but its been too long since I've written a proof that
			 * I can't explain it well enough to want to publish it.  
			 * 
			 * Just trust me when I say the triangles formed by the lengths 
			 * width are always the same (for a given theta) and the same 
			 * goes for the height of r.
			 * 
			 * Rather than associate the opposite/adjacent sides with the
			 * width and height of the original bitmap, I'll associate them
			 * based on their position.
			 * 
			 * adjacent/oppositeTop will refer to the triangles making up the 
			 * upper right and lower left corners
			 * 
			 * adjacent/oppositeBottom will refer to the triangles making up 
			 * the upper left and lower right corners
			 * 
			 * The names are based on the right side corners, because thats 
			 * where I did my work on paper (the right side).
			 * 
			 * Now if you draw this out, you will see that the width of the 
			 * bounding box is calculated by adding together adjacentTop and 
			 * oppositeBottom while the height is calculate by adding 
			 * together adjacentBottom and oppositeTop.
			 */
            #endregion

            double adjacentTop, oppositeTop;
            double adjacentBottom, oppositeBottom;

            // We need to calculate the sides of the triangles based
            // on how much rotation is being done to the bitmap.
            //   Refer to the first paragraph in the explaination above for 
            //   reasons why.
            if ((locked_theta >= 0.0 && locked_theta < pi2) ||
                (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
            {
                adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth;

                adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight;
                oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
            }
            else
            {
                adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight;

                adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth;
                oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
            }

            newWidth = adjacentTop + oppositeBottom;
            newHeight = adjacentBottom + oppositeTop;

            nWidth = (int)Math.Ceiling(newWidth);
            nHeight = (int)Math.Ceiling(newHeight);

            Bitmap rotatedBmp = new Bitmap(nWidth, nHeight);

            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {
                // This array will be used to pass in the three points that 
                // make up the rotated image
                PointF[] points;

                /*
                 * The values of opposite/adjacentTop/Bottom are referring to 
                 * fixed locations instead of in relation to the
                 * rotating image so I need to change which values are used
                 * based on the how much the image is rotating.
                 * 
                 * For each point, one of the coordinates will always be 0, 
                 * nWidth, or nHeight.  This because the Bitmap we are drawing on
                 * is the bounding box for the rotated bitmap.  If both of the 
                 * corrdinates for any of the given points wasn't in the set above
                 * then the bitmap we are drawing on WOULDN'T be the bounding box
                 * as required.
                 */
                if (locked_theta >= 0.0 && locked_theta < pi2)
                {
                    points = new PointF[] { 
											 new PointF( (float)oppositeBottom, 0 ), 
											 new PointF( nWidth, (float)oppositeTop ),
											 new PointF( 0, (float)adjacentBottom )
										 };

                }
                else if (locked_theta >= pi2 && locked_theta < Math.PI)
                {
                    points = new PointF[] { 
											 new PointF( nWidth, (float)oppositeTop ),
											 new PointF(  (float)adjacentTop, nHeight ),
											 new PointF( (float)oppositeBottom, 0 )						 
										 };
                }
                else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
                {
                    points = new PointF[] { 
											 new PointF( (float)adjacentTop, nHeight ), 
											 new PointF( 0, (float)adjacentBottom ),
											 new PointF( nWidth,  (float)oppositeTop )
										 };
                }
                else
                {
                    points = new PointF[] { 
											 new PointF( 0, (float)adjacentBottom ), 
											 new PointF(  (float)oppositeBottom, 0 ),
											 new PointF( (float)adjacentTop, nHeight )		
										 };
                }
                
                g.DrawImage(image, points);
                g.Dispose();
                
            }

            return rotatedBmp;
        }

        public static Bitmap RotateImageUsingGraphics(Image image, float angle)
        {
            // angle fomt 0 to 360
            if (angle > 360)
            {
                angle = angle - (((int)(angle / 360)) * 360);
            }
            if (angle == 360) angle = 0;

            if (angle == 0 || angle == 90 || angle == 270)
            {
                    return RotateImage90_180_270(image,(int)angle);
            }

            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {
                g.ScaleTransform(0.99f, 0.99f); // чтобы за гранцицу не выходило, а также .tp этого изображение прдпрагивает
                g.RotateTransform(angle);

                double HalfWidth = image.Width / 2, HalfHeight = image.Height / 2,
                      PosX = 0, PosY = 0;
                double theta = ((double)angle + 45.0) * Math.PI / 180.0;
                double sqrt2 = Math.Sqrt(2.0);

                PosX = -HalfWidth + (Math.Sin(theta) * HalfWidth * sqrt2);
                PosY = -HalfWidth + (Math.Cos(theta) * HalfHeight * sqrt2);

                g.DrawImage(image, (float)PosX, (float)PosY, image.Width, image.Height);
                g.Dispose();
            }

            return rotatedBmp;
        }

        public static Bitmap RotateImage(Image image, RotateFlipType RotType)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            image.RotateFlip(RotType);
            Bitmap rotatedBmp = new Bitmap(image);            

            return new Bitmap(image);
        }

        public static Bitmap RotateImage90_180_270(Image image, int angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (angle > 360)
            {
                angle = angle - (((int)(angle / 360)) * 360);
            }
            if (angle == 360) angle = 0;

            if (angle == 90)
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            else if (angle == 180)
            {
                image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            else if (angle == 270)
            {
                image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            
            Bitmap rotatedBmp = new Bitmap(image);

            return new Bitmap(image);
        }

        public static Bitmap rotateImage(Bitmap b, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);
            //move rotation point to center of image
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            //rotate
            g.RotateTransform(angle);
            //move image back
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }
    }
}
