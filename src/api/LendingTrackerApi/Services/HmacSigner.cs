using System.Security.Cryptography;
using System.Text;

namespace LendingTrackerApi.Services
{
    public class HmacSigner : IHmacSigner
    {
        private ILogger<HmacSigner> _logger;
        private IConfiguration _config;
        private string _hmacKey;

        public HmacSigner(ILogger<HmacSigner> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;
            _hmacKey = _config["HmacKey"];
        }
        public string Sign(string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_hmacKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }

        public bool Verify(string data, string signature)
        {
            var computedSignature = Sign(data);
            return computedSignature == signature;
        }
    }
}
