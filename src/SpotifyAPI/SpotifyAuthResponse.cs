namespace SpotifyAPI{

public class SpotifyAuthResponse{

	//An access token that can be provided in subsequent calls to Spotify’s Web API.
	public string access_token {get;set;}

	// 	How the access token may be used: always “Bearer”.
	public string token_type {get;set;}
	
	//A space-separated list of scopes which have been granted for this access_token
	public string scope {get;set;}

	// 	The time period (in seconds) for which the access token is valid.
	public string expires_in {get;set;}

	//A token that can be sent to the Spotify Accounts service in place of an authorization code.
	public string refresh_token {get;set;}


}

}
