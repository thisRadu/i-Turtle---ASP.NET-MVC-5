using System.Drawing;

namespace i_Turtle.Models
{
    public class TwoFactorViewModel
    {
        public string SharedKey { get; set; }
        public byte[] QrCodeUrl { get; set; }
    }
}