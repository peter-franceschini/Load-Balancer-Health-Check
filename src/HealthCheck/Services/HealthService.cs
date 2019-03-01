using HealthCheck.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheck.Services
{
    public class HealthService : IHealthService
    {
        private Settings Settings { get; set; }

        public HealthService(IOptions<Settings> settings)
        {
            Settings = settings.Value;
        }

        public bool IsHealthy()
        {
            // Return healthy if configured to forece online
            if (Settings.ForceOnline)
            {
                return true;
            }

            // Check if the url is healthy
            var isHealthy = CheckUrlHealth();

            // If the url is not healthy and it is configured for retries
            if(!isHealthy && Settings.Retries > 0)
            {
                int retries = 0;
                // Retry while not healthy and more retries exist
                while(retries < Settings.Retries && isHealthy == false)
                {
                    isHealthy = CheckUrlHealth();
                }
            }

            return isHealthy;
        }

        private bool CheckUrlHealth()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Settings.Url);
            
            // Allow non valid SSL certs
            if (!Settings.RequireValidSSL)
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                                        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true; // **** Always accept
                };
            }

            request.Timeout = Settings.Timeout;
            request.ReadWriteTimeout = Settings.Timeout;
            var wresp = (HttpWebResponse)request.GetResponse();

            using (Stream stream = wresp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string rawHtml = reader.ReadToEnd();
                if (rawHtml.ToLower().Contains(Settings.ValidationText.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
