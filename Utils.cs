using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Pick_Bandit
{
    public static class Utils
    {
        /**
        * <summary>
        * Converts a byte array representing an image into an ImageSource object.
        * </summary>
        * <param name="imageData">The byte array containing the image data.</param>
        * <returns>An ImageSource object representing the image.</returns>
        */
        public static ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }

        /**
        * <summary>
        * Calculates the sigmoid function for the given input number.
        * </summary>
        * <param name="num">The input number for which the sigmoid function is calculated.</param>
        * <returns>The result of applying the sigmoid function to the input number.</returns>
        */
        public static double Sigmoid(double num)
        {
            double lower = 1 + Math.Exp(-num);
            return 1 / lower;
        }

        /**
        * <summary>
        * Returns a random number between 0-2 that is different than the excluded number.
        * The method utilizes the geometric distribution mean, resulting in an average time complexity of O(1).
        * </summary>
        * <param name="exclude">The number to be excluded from the random selection.</param>
        * <param name="rnd">The instance of Random class used to generate random numbers.</param>
        * <returns>A random integer between 0-2, excluding the specified 'exclude' number.</returns>
        */
        public static int RandomExclude(int exclude, Random rnd)
        {
            int randomNum = rnd.Next(0, 3);

            // Loop until the generated random number is different from the 'exclude' parameter
            while (randomNum == exclude)
                randomNum = rnd.Next(0, 3);

            return randomNum;
        }

    }
}
