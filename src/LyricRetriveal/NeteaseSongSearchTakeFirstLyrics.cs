using System.Linq;
using System.Threading.Tasks;
using F23.StringSimilarity;
using Music163API;
using spotifyLyrics.Models;

public class NeteaseSongSearchTakeFirstLyrics : ILyricRetrievalStrategy
{

    private NeteaseMusicAPI api163;
    public NeteaseSongSearchTakeFirstLyrics()
    {
        api163 = new NeteaseMusicAPI();
    }

    public async Task<Lyrics> retrive(string song, string arist, string album)
    {
         var query = song.Split('-')[0].Trim()+" "+arist.Trim()+" "+album.Trim();
        var cosine = new Cosine(2);
        Music163API.SearchSongResponse sug = await api163.SearchSongSuggest(query);
        
            if(sug.Result.SongCount==0){
                return null;
            }
            var lyricsob = await api163.GetLyric(sug.Result.Songs[0].Id);
            if (lyricsob.Lrc ==null || lyricsob.Lrc.Lyric == null)
                return null;
            if (!lyricsob.Lrc.Lyric.Contains("[")) //no synced lyrics
                return null;


            var lyrics = new Lyrics163(
                sug.Result.Songs[0].Name,
                sug.Result.Songs[0].Ar[0].Name,
                sug.Result.Songs[0].Al.Name,
                lyricsob.Lrc.Lyric);
                return lyrics;
    }
}