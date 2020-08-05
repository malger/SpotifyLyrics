using System.Threading.Tasks;
using spotifyLyrics.Models;

public interface ILyricRetrievalStrategy
{
    Task<Lyrics> retrive(string song, string arist,string album);
}