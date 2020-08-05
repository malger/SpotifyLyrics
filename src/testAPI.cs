using System;
using Music163API;

public class testAPI
    {
        public async static void Main(string[] args)
        {
            var api =  new NeteaseMusicAPI();
            var res = await api.SearchAlbumSuggest("Born in the U.S.A");
            Console.Write(res.Result.Albums[0].Name);
             Console.Write(" by ");
             Console.Write(res.Result.Albums[0].Artists[0].Name);
        }
    }