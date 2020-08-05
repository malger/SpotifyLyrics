using System.Linq;
using System.Threading.Tasks;
using F23.StringSimilarity;
using Music163API;
using spotifyLyrics.Models;

public class NeteaseSongSearchLyrics : ILyricRetrievalStrategy
{

    private NeteaseMusicAPI api163;
    public NeteaseSongSearchLyrics()
    {
        api163 = new NeteaseMusicAPI();
    }

    public async Task<Lyrics> retrive(string song, string arist, string album)
    {
         var query = song.Split('-')[0].Trim()+" "+arist.Trim()+" "+album.Trim();
        var cosine = new Cosine(2);
        Music163API.SearchSongResponse sug = await api163.SearchSongSuggest(query);
        var bestmatches = sug.Result.Songs
                    .Select(s=> new {
                        song = s,
                        ssim = cosine.Similarity(song,s.Name),
                        asim = cosine.Similarity(arist,s.Ar[0].Name)
                    })
                    .Where(s => s.ssim > 0.8 & s.asim>0.7)
                    .OrderByDescending(s => s.asim)
                    .ThenByDescending(s=>s.ssim)
                    .Take(3)
                    .ToArray();

            var crawled_lyrics = await Task.WhenAll(bestmatches
                .Select(s => api163.GetLyric(s.song.Id))
                .ToArray()
            );

            var bestlyric = bestmatches
                .Zip(crawled_lyrics,(first,sec)=>new {song = first.song, lyr = sec.Lrc})
                .Where(e => e.lyr !=null && e.lyr.Lyric !=null)
                .Where(e => e.lyr.Lyric.Contains("[")) //choose synced lyric
                .FirstOrDefault();

            if(bestlyric == null){
                return null;
            }
            var lyrics = new Lyrics163(
                bestlyric.song.Name,
                bestlyric.song.Ar[0].Name,
                bestlyric.song.Al.Name,
                bestlyric.lyr.Lyric);
                return lyrics;
    }
}