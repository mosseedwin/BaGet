using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BaGet.Core
{
    public class ApiKeyAuthenticationService : IAuthenticationService
    {
        private readonly string _apiKey;

        private readonly string _apiKeyFile;

        public ApiKeyAuthenticationService(IOptionsSnapshot<BaGetOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _apiKey = string.IsNullOrWhiteSpace(options.Value.ApiKey) ? null : options.Value.ApiKey;
            _apiKeyFile = string.IsNullOrWhiteSpace(options.Value.ApiKeyFile) ? null : options.Value.ApiKeyFile;
        }

        public Task<bool> AuthenticateAsync(string apiKey, CancellationToken cancellationToken)
            => Task.FromResult(Authenticate(apiKey));

        private bool Authenticate(string apiKey)
        {
            // No authentication is necessary if there is no required API key.
            if (_apiKeyFile != null)
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return false;
                }
                if (!File.Exists(_apiKeyFile))
                {
                    return false;
                }
                return File.ReadAllLines(_apiKeyFile).Contains(apiKey);
            }

            if (_apiKey == null) return true;

            return _apiKey == apiKey;
        }
    }
}
