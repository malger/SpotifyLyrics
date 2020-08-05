using System.Linq;
using System.Threading.Tasks;
using F23.StringSimilarity;
using Music163API;
using spotifyLyrics.Models;

public class NeteaseAlbumSearchLyrics : ILyricRetrievalStrategy
{

    private NeteaseMusicAPI api163;
    public NeteaseAlbumSearchLyrics()
    {
        api163 = new NeteaseMusicAPI();
    }

    public async Task<Lyrics> retrive(string song, string arist, string album)
    {
         var query = album.Trim();
        var cosine = new Cosine(2);
        var sug = await api163.SearchAlbumSuggest(query);
        var bestmatch = sug.Result.Albums
                    .Select(alb=> new {
                        al = alb,
                        asim = cosine.Similarity(query,alb.Name),
                    })
                    .Where(e => e.asim > 0.9)
                    .OrderByDescending(e => e.asim)
                    .Select(e=> e.al)
                    .FirstOrDefault();

            if (bestmatch == null)
                return null;
          
            var albumObj = await api163.GetAlbum(bestmatch.Id);

            var bestsong = albumObj.Songs
                .Where(s => cosine.Similarity(song,s.Name)>.8)
                .FirstOrDefault();

            if(bestsong == null)
                return null;


            var lyricResult = await api163.GetLyric(bestsong.Id);
            if (lyricResult.Lrc == null)
                return null;
            var lyrics = new Lyrics163(
                bestsong.Name,
                bestsong.Ar[0].Name,
                bestsong.Al.Name,
                lyricResult.Lrc.Lyric);
            return lyrics;
    }
}