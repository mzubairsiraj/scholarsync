using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace ScholarSync.Infrastructure.Services
{
    /// <summary>
    /// Handles image loading with fallback to user initials
    /// Simple static helper pattern like DbConnector
    /// </summary>
    public static class ImageHandler
    {
        private static readonly string DefaultImageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Profiles");

        /// <summary>
        /// Gets user profile image or generates initials image
        /// </summary>
        /// <param name="imagePath">Path to user's image</param>
        /// <param name="userName">User's full name for generating initials</param>
        /// <param name="size">Size of the output image (default 100x100)</param>
        /// <returns>Image object</returns>
        public static Image GetUserImage(string imagePath, string userName, int size = 100)
        {
            // Try to load existing image
            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    return Image.FromFile(imagePath);
                }
                catch
                {
                    // If image loading fails, fall through to generate initials
                }
            }

            // Generate initials image as fallback
            return GenerateInitialsImage(userName, size);
        }

        /// <summary>
        /// Gets user profile image by user ID or generates initials image
        /// </summary>
        /// <param name="userId">User's unique ID</param>
        /// <param name="userName">User's full name for generating initials</param>
        /// <param name="size">Size of the output image (default 100x100)</param>
        /// <returns>Image object</returns>
        public static Image GetUserImageById(string userId, string userName, int size = 100)
        {
            EnsureImageDirectoryExists();

            // Try common image formats
            string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp" };
            
            foreach (var ext in extensions)
            {
                string imagePath = Path.Combine(DefaultImageDirectory, $"{userId}{ext}");
                if (File.Exists(imagePath))
                {
                    try
                    {
                        return Image.FromFile(imagePath);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            // No image found, generate initials
            return GenerateInitialsImage(userName, size);
        }

        /// <summary>
        /// Generates an image with user's initials
        /// </summary>
        /// <param name="userName">Full name of the user</param>
        /// <param name="size">Size of the image</param>
        /// <returns>Generated image with initials</returns>
        public static Image GenerateInitialsImage(string userName, int size = 100)
        {
            // Get initials
            string initials = GetInitials(userName);

            // Create bitmap
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Set high quality rendering
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Get background color based on name
                Color backgroundColor = GetColorFromName(userName);

                // Fill background
                using (SolidBrush brush = new SolidBrush(backgroundColor))
                {
                    g.FillEllipse(brush, 0, 0, size, size);
                }

                // Draw initials
                using (Font font = new Font("Arial", size / 2.5f, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(initials, font, textBrush, size / 2f, size / 2f, format);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Saves user image to profile directory
        /// </summary>
        /// <param name="userId">User's unique ID</param>
        /// <param name="sourceImagePath">Path to source image file</param>
        /// <returns>True if saved successfully</returns>
        public static bool SaveUserImage(string userId, string sourceImagePath)
        {
            try
            {
                EnsureImageDirectoryExists();

                string extension = Path.GetExtension(sourceImagePath);
                string destinationPath = Path.Combine(DefaultImageDirectory, $"{userId}{extension}");

                // Copy and resize if needed
                using (Image sourceImage = Image.FromFile(sourceImagePath))
                {
                    // Resize to 500x500 for consistency
                    using (Image resizedImage = ResizeImage(sourceImage, 500, 500))
                    {
                        resizedImage.Save(destinationPath);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes user profile image
        /// </summary>
        /// <param name="userId">User's unique ID</param>
        /// <returns>True if deleted successfully</returns>
        public static bool DeleteUserImage(string userId)
        {
            try
            {
                string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp" };

                foreach (var ext in extensions)
                {
                    string imagePath = Path.Combine(DefaultImageDirectory, $"{userId}{ext}");
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extracts initials from user's full name
        /// </summary>
        private static string GetInitials(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return "?";

            string[] parts = userName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "?";

            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();

            // First and last name initials
            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }

        /// <summary>
        /// Generates a consistent color based on user's name
        /// </summary>
        private static Color GetColorFromName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return Color.FromArgb(100, 100, 100); // Default gray

            // Predefined color palette
            Color[] colors = new Color[]
            {
                Color.FromArgb(244, 67, 54),   // Red
                Color.FromArgb(233, 30, 99),   // Pink
                Color.FromArgb(156, 39, 176),  // Purple
                Color.FromArgb(103, 58, 183),  // Deep Purple
                Color.FromArgb(63, 81, 181),   // Indigo
                Color.FromArgb(33, 150, 243),  // Blue
                Color.FromArgb(0, 188, 212),   // Cyan
                Color.FromArgb(0, 150, 136),   // Teal
                Color.FromArgb(76, 175, 80),   // Green
                Color.FromArgb(255, 152, 0),   // Orange
                Color.FromArgb(255, 87, 34),   // Deep Orange
                Color.FromArgb(121, 85, 72)    // Brown
            };

            // Use hash of name to consistently pick same color
            int hash = Math.Abs(userName.GetHashCode());
            return colors[hash % colors.Length];
        }

        /// <summary>
        /// Resizes an image to specified dimensions
        /// </summary>
        private static Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Ensures the profile images directory exists
        /// </summary>
        private static void EnsureImageDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(DefaultImageDirectory))
                {
                    Directory.CreateDirectory(DefaultImageDirectory);
                }
            }
            catch
            {
                // Silently fail - will use initials instead
            }
        }
    }
}
