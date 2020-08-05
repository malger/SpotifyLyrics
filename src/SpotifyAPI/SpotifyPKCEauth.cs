using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace SpotifyAPI{
    public class SpotifyPKCEauth{

        const string secValidChars =  "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890._~";
        const int secLength = 128;
        public string clientId {get;}
        public string codeVerifier {get;}
        public string codeChallenge {get;}
         
         public SpotifyPKCEauth(IConfiguration config)
         {
            this.codeVerifier = genSecret();
            this.codeChallenge = sha256HashasBase64(this.codeVerifier);
            this.clientId = config["Spotify:ClientId"];

         }
         
         private String genSecret(){
            var length = secLength;
            var sb = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider()){

                byte[] uintBuffer = new byte[sizeof(uint)]; //normally 4bytes
                while(length-- >0){
                    //random fill unsigned int buffer
                    rng.GetBytes(uintBuffer);
                    uint i = BitConverter.ToUInt32(uintBuffer,0); //random i
                    char c = secValidChars[(int)(i % (uint)secValidChars.Length)]; //random char,
                    //TODO: fix uneven distribution that is potentially a sec problem

                    sb.Append(c);
                }

                
            }
            return sb.ToString();
        }
        
        private String sha256HashasBase64(String s){

              byte[] hash;
              using (SHA256 sha256 = SHA256.Create()){
                  var byterep = Encoding.UTF8.GetBytes(s);
                  hash = sha256.ComputeHash(byterep);

              } 
             return WebEncoders.Base64UrlEncode(hash);

        }

    }
}