namespace SpotifyAPI{

     
    public class SpotifySettings
    {
        public SpotifySettings(){
            Token = "";
            ClientId = "";
            RefreshToken = "";
        }

        public string ClientId {get;set;}

        public string RefreshToken {get;set;}

        public string Token { get; set; }


        public void updateSettings(SpotifyAuthResponse auth){
            this.RefreshToken = auth.refresh_token;
            this.Token = auth.access_token;
        }
        

    }
}