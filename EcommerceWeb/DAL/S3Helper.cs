using System;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace EcommerceWeb.DAL
{
    public class S3Helper
    {
        private readonly string _accessKey = "AKIAT7LXM4KWF425Y2MN";
        private readonly string _secretKey = "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";
        private readonly string _bucketName = "website-project11111";
        private readonly RegionEndpoint _region = RegionEndpoint.USEast1;

        public string UploadImage(IFormFile file)
        {
            try
            {
                var client = new AmazonS3Client(_accessKey, _secretKey, _region);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Get correct content type
                string contentType = "image/jpeg";
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (ext == ".png") contentType = "image/png";
                else if (ext == ".gif") contentType = "image/gif";
                else if (ext == ".webp") contentType = "image/webp";
                else if (ext == ".heic" || ext == ".heif")
                    throw new Exception("iPhone HEIC images are not supported. Please upload a JPG or PNG.");

                using (var stream = file.OpenReadStream())
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = fileName,
                        InputStream = stream,
                        ContentType = contentType,
                        AutoCloseStream = false
                    };
                    client.PutObjectAsync(request).Wait();
                }

                return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception("Image upload failed: " + ex.Message);
            }
        }

        public void DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl)) return;

                // Extract file name from URL
                string fileName = imageUrl.Replace(
                    $"https://{_bucketName}.s3.amazonaws.com/", "");

                if (string.IsNullOrWhiteSpace(fileName)) return;

                var client = new AmazonS3Client(_accessKey, _secretKey, _region);

                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                client.DeleteObjectAsync(request).Wait();
            }
            catch
            {
                // Silently fail — don't block the app if delete fails
            }
        }
    }
}