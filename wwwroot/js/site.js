// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const { ipcRenderer } = require('electron')
// ipcMain = {};
// ipcMain.on = function(a,b){};

const byId = (x) => document.getElementById(x)

const UPDATE_interval = 500; //MS
const fetchedLyricsPH = [{"line":"No Lyrics found!","relativeTime":new Date(0)}];
var fetchedLyrics = fetchedLyricsPH;
var fetchedPBms = 0;
var updateTextInt;
document.addEventListener('DOMContentLoaded', async () => {

    lyricsTextEle = byId("curPBname");

    if (await isPlaying()){
        await fetchLyrics();
        updateTextInt = initLyricUpdater(); //update lyrics every 500ms
    }
    

    ipcRenderer.on('song_changed', async (event, arg) => {
        clearInterval(updateTextInt);
        await fetchLyrics()
        updateTextInt = initLyricUpdater();
    });
    ipcRenderer.on('pause', (event, arg) =>clearInterval(updateTextInt));
    ipcRenderer.on('play', (event, arg) => updateTextInt = initLyricUpdater());


});

function initLyricUpdater(){
    return setInterval(updateText, UPDATE_interval);
}

async function isPlaying(){
    const resp = await fetch("/Spotify/isPlaying",{method:"GET"});
    return await resp.json();
}

async function fetchLyrics() {

    try {
        lyricsTextEle.innerText = "fetching lyrics..."
        timerStart = new Date();

        const resp = await fetch("/Lyrics/json?strategy=NeteaseTF",{method:"GET"});
        const lyrics = await resp.json();
                        
        if (!Array.isArray(lyrics)){
            fetchedLyrics = fetchedLyricsPH;
            return;
        }
        fetchedLyrics = lyrics.map(e => ({
            "line" : e.line,
            "relativeTime" : new Date(e.relativeTime)
        }));
        
        console.dir(fetchedLyrics);


    } catch (e) {
        console.error("error fetching lyrics")
        console.debug(e.message)

        
    }
    

}

async function fetchPBms(){
    const resp = await fetch("/Spotify/SongMS",{method:"GET"})
    const pb_ms = await resp.json(); 
    return new Date(pb_ms);
    //document.pb_ms.setMilliseconds(pb_ms);
}

async function updateText(){

    if(!fetchedLyrics){
        await fetchLyrics();
    }
    if(!fetchPBms){
        fetchedPBms = await fetchPBms();
    }

    var timeElasped = new Date()-timerStart;
    //every 3 seconds
    if(timeElasped>3000){
        fetchedPBms = await fetchPBms();
        console.debug("got pb ms from spotify");
        timerStart = new Date();
        i = 0;
    } else {
    }
    timeElasped = new Date()-timerStart;

    lyricsTextEle.innerText = findNNLyric(
        fetchedLyrics,
        fetchedPBms.getTime()+timeElasped //TODO:better
    );
}

function findNNLyric(lyrics,timeMS){
    const diffs = lyrics
        .map(e => e.relativeTime.getTime()-timeMS)

    const min_i = diffs
        .reduce((indxMin,v,i,arr)=>{
            if (v<0){
                return ++i;
            }
            if (v<arr[indxMin]) {
                return i
            } else {
                return indxMin;
            }
            
        },0);
        
    return lyrics[min_i==0?min_i:(min_i-1)].line;


}