using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using Serilog;
using System;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace FacebookCreator.Helper
{
    public class FileHelper
    {
        public void CopyImagesToFolder(string sourceFolderPath, string destinationFolderPath)
        {
            if (!Directory.Exists(sourceFolderPath))
            {
                Console.WriteLine("Thư mục nguồn không tồn tại.");
                Log.Error("Thư mục nguồn không tồn tại. \n" + sourceFolderPath);
                return;
            }

            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            string[] imageFiles = Directory.GetFiles(sourceFolderPath)
                .Where(file => IsImageFile(file))
                .ToArray();

            foreach (string imagePath in imageFiles)
            {
                string fileName = Path.GetFileName(imagePath);
                string destinationPath = Path.Combine(destinationFolderPath, fileName);
                File.Copy(imagePath, destinationPath, true);
            }

            Console.WriteLine($"Đã sao chép {imageFiles.Length} tệp tin hình ảnh thành công.");
        }

        public bool IsImageFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);

            if (extension != null && (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                      extension.Equals(".png", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }
        public string GetRandomFileLink(string folderPath)
        {
            Random random = new Random();
            string[] files = Directory.GetFiles(folderPath);

            if (files.Length == 0)
            {
                Log.Error("The specified folder does not contain any files.");
                return null;
            }

            string randomFile = files[random.Next(files.Length)];
            string fileLink = Path.GetFullPath(randomFile);

            return fileLink;
        }
        public static string RandomStringWithExtraChars(List<string> stringList)
        {
            Random random = new Random();
            string selectedString = stringList[random.Next(stringList.Count)];
            return selectedString;
        }
        public static List<string> ReadImageFiles(string folderPath)
        {
            List<string> files = new List<string>();
            if (Directory.Exists(folderPath))
            {
                string[] imageExtensions = { ".jpg", ".png" };

                // Lấy tất cả các tệp hình ảnh trong thư mục và các thư mục con.
                string[] imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .ToArray();

                // In danh sách các tệp hình ảnh.
                foreach (string imageFile in imageFiles)
                {
                    files.Add(imageFile);
                }
            }
            else
            {
                MessageCommon.ShowMessageBox("Directory does not exist.", 4);
            }
            return files;
        }
        public static List<string> ReadFile(string path)
        {
            var list = new List<string>();
            try
            {
                var result = File.ReadAllLines(path);
                Log.Information("ReadFile " + path);
                for (int i = 0; i < result.Length; i++)
                {
                    string[] lines = result[i].Split(':', ';', ',', '|');
                    if (!string.IsNullOrEmpty(lines[0]))
                    {
                        list.Add(lines[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return list;
        }
        public static string ReadQRCode(Bitmap bitmap)
        {
            QRCodeDecoder decoder = new QRCodeDecoder();
            try
            {
                // Đọc nội dung từ tập tin ảnh chứa mã QR
                string decodedText = decoder.Decode(new QRCodeBitmapImage(bitmap));
                if (!string.IsNullOrEmpty(decodedText))
                {
                    return decodedText;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }
    }
}
