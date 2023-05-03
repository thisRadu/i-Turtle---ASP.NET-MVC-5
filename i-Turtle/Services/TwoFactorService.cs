namespace i_Turtle.Services
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Security.Cryptography;
    using Google.Authenticator;
    using Microsoft.EntityFrameworkCore;
    using i_Turtle.Models;
    using ZXing;
    using ZXing.QrCode;

    public class TwoFactorService
        {
            private readonly TurtleDbContext _context;

            public TwoFactorService(TurtleDbContext context)
            {
                _context = context;
            }


        public void GenerateAndStoreSecretKey(User user)
            {
                // Generate a new secret key for the user
                var secretKey = GenerateSecretKey();

                // Store the secret key in the database for the user
                user.TwoFactorCode = secretKey;
                _context.SaveChanges();
            }

            public string GetSecretKey(User user)
            {
                return user.TwoFactorCode;
            }

            public Bitmap GenerateQrCodeImage(string secretKey, string userDisplayName)
            {
                // Generate a QR code image using the secret key and user display name
                var qrCodeWriter = new QRCodeWriter();
                var qrCode = qrCodeWriter.encode($"otpauth://totp/YourApp:{userDisplayName}?secret={secretKey}&issuer=iTurtle", BarcodeFormat.QR_CODE, 200, 200);

                // Convert the QR code to a bitmap image
                var bitmap = new Bitmap(qrCode.Width, qrCode.Height);
                for (var x = 0; x < qrCode.Width; x++)
                {
                    for (var y = 0; y < qrCode.Height; y++)
                    {
                        bitmap.SetPixel(x, y, qrCode[x, y] ? Color.Black : Color.White);
                    }
                }

                return bitmap;
            }

            public bool VerifyToken(User user, string token)
            {
                // Verify the token using the secret key stored in the database
                var authenticator = new TwoFactorAuthenticator();
                return authenticator.ValidateTwoFactorPIN(user.TwoFactorCode, token);
            }

            private static string GenerateSecretKey()
            {
                // Generate a random 20-byte secret key
                var bytes = new byte[20];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(bytes);
                }

                // Encode the secret key as a Base32 string
                return Base32Encoding.ToString(bytes);
            }
        }
    

}
