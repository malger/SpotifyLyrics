
using System;
using System.Collections.Generic;
using System.Linq;

namespace spotifyLyrics.Models {

    public abstract class Lyrics
    {

        public Lyrics(String song, String artist, String album)
        {
            this.song = song;
            this.artist = artist;
            this.album = album;

        }

        public String song{get;}
        public String artist{get;}
        public String album{get;}


        public List<LyricLine> synced_songlines {
            get;
            set;
        }


        public String songText() => String.Join("\n",this.synced_songlines.Select(e => e.line));

        override public String ToString() => String.Join("\n",this.synced_songlines.Select(
                                    e => e.relativeTime.ToString()+" "+e.line));

        
    }

    public class LyricLine{
        public string line {get;set;}
        public DateTimeOffset relativeTime{get;set;}

    }

}
