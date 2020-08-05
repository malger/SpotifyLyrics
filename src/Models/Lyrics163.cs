using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace spotifyLyrics.Models
{
    public class Lyrics163 : Lyrics
    {
        private string rawtext {get;set;}

        Regex rx = new Regex(@"\[(?<time>[\d.:]+)\](?<text>.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        
        private const string timeformat = @"mm\:ss\.ff";
       private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 
                                                      DateTimeKind.Utc);
        public Lyrics163(string song,string artist,string album,string rawtext) : base(song,artist,album)
        {

            if (rawtext is null){
                throw new NullReferenceException("rawText is null");
            }


            if (rawtext.Length == 0){
                throw new ArgumentException("rawText is of zero length");
            }

            this.rawtext = rawtext;
            this.synced_songlines = rx.Matches(rawtext)
                .OfType<Match>()
                .Select(m => {
                    var timestring = m.Groups["time"].Value;
                    timestring = timestring.Length>8?
                                    timestring.Substring(0,8):timestring;
                    var relTime = TimeSpan.ParseExact(
                            timestring,
                            timeformat,
                            CultureInfo.InvariantCulture);
                    return new LyricLine() {
                        line = m.Groups["text"].Value,
                        relativeTime = Epoch.Add(relTime)
                    };
                })
                .Where(e => e.line.Length>0)
                .ToList();
                

                


        }

    }
}

