using System;
using System.Text;

using com.krestfield.ezsign.client;

namespace EzSignDotNetClientSample
{
    class EzSignClientSample
    {
        /// <summary>
        /// This example demonstrates how simply signing and verification services can be built
        /// into your .NET applications using the PKCloud online test service
        /// 
        /// The online test service is up between 06:00 and 19:00 UTC (this window can be extended on request)
        /// 
        /// PKCloud can be configured to use many different HSMs, including nCipher, Thales Luna,
        /// Utimaco, Azure Key Vault, Google KMS, AWS CloudHSM and Thales PayShield.  It can be hosted in the
        /// cloud or on your own premises
        /// 
        /// Refer to https://krestfield.com/index.php/pkcloud for more information.  
        /// Questions can be directed to: support@krestfield.com
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Connection and authentication details
            // If the credentials do not work, contact support@krestfield.com to request updated versions
            String server = "demoapi.krestfield.com";
            int port = 80;
            String authCode = "password";

            // Signature generation details
            String channel = "P7_RSA_SIGN_CHANNEL";
            byte[] dataToSign = Encoding.ASCII.GetBytes("Sign Me");
            bool dataIsDigest = false;

            // Create a client instance
            EzSignClient client = new EzSignClient(server, port, authCode);
            byte[] signature = null;

            try
            {
                // Sign the data.  PKCloud server takes care of the hash and signature algorithm to use
                // the signature construction and interfacing to the HSM
                signature = client.signData(channel, dataToSign, dataIsDigest);

                Console.WriteLine("Generated Signature OK");
                Console.WriteLine("Base64 Signature Data: " + Convert.ToBase64String(signature));
                Console.WriteLine();
            }
            catch (KSigningException signingEx)
            {
                Console.WriteLine("Signature generation error. " + signingEx.Message);
            }

            try
            {
                // Verify this signature.  This will carry out certificate validation and path building
                // Revocation can be configured (OCSP and CRL)
                client.verifySignature(channel, signature, dataToSign, dataIsDigest);

                Console.WriteLine("Signature Verified OK");
                Console.WriteLine();

                // Now we create a failure by altering the original data
                Console.WriteLine("We expect a failure next as we have altered the original data...");
                dataToSign = Encoding.ASCII.GetBytes("Sign Me 2");
                client.verifySignature(channel, signature, dataToSign, dataIsDigest);

                Console.WriteLine("Signature Verified OK");// We will not get here...
            }
            catch (KVerificationException verifyEx)
            {
                Console.WriteLine("Signature verification error. " + verifyEx.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Press a key to close");
            Console.ReadKey();
        }
    }
}
