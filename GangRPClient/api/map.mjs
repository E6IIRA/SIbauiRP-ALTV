import alt from 'alt';
//import native from "natives"


//DEFAULT MAP
/*const ZOOM_LEVEL_0 = alt.MapZoomData.get('ZOOM_LEVEL_0');
ZOOM_LEVEL_0.fZoomScale = 2.73;
ZOOM_LEVEL_0.fZoomSpeed = 0.9;
ZOOM_LEVEL_0.fScrollSpeed = 0.08;
ZOOM_LEVEL_0.vTilesX = 0.0;
ZOOM_LEVEL_0.vTilesY = 0.0;

const ZOOM_LEVEL_1 = alt.MapZoomData.get('ZOOM_LEVEL_1');
ZOOM_LEVEL_1.fZoomScale = 2.8;
ZOOM_LEVEL_1.fZoomSpeed = 0.9;
ZOOM_LEVEL_1.fScrollSpeed = 0.08;
ZOOM_LEVEL_1.vTilesX = 0.0;
ZOOM_LEVEL_1.vTilesY = 0.0;

const ZOOM_LEVEL_2 = alt.MapZoomData.get('ZOOM_LEVEL_2');
ZOOM_LEVEL_2.fZoomScale = 8.0;
ZOOM_LEVEL_2.fZoomSpeed = 0.9;
ZOOM_LEVEL_2.fScrollSpeed = 0.08;
ZOOM_LEVEL_2.vTilesX = 0.0;
ZOOM_LEVEL_2.vTilesY = 0.0;

const ZOOM_LEVEL_3 = alt.MapZoomData.get('ZOOM_LEVEL_3');
ZOOM_LEVEL_3.fZoomScale = 11.0;
ZOOM_LEVEL_3.fZoomSpeed = 0.9;
ZOOM_LEVEL_3.fScrollSpeed = 0.08;
ZOOM_LEVEL_3.vTilesX = 0.0;
ZOOM_LEVEL_3.vTilesY = 0.0;

const ZOOM_LEVEL_4 = alt.MapZoomData.get('ZOOM_LEVEL_4');
ZOOM_LEVEL_4.fZoomScale = 16.0;
ZOOM_LEVEL_4.fZoomSpeed = 0.9;
ZOOM_LEVEL_4.fScrollSpeed = 0.08;
ZOOM_LEVEL_4.vTilesX = 0.0;
ZOOM_LEVEL_4.vTilesY = 0.0;

const ZOOM_LEVEL_GOLF_COURSE = alt.MapZoomData.get('ZOOM_LEVEL_GOLF_COURSE');
ZOOM_LEVEL_GOLF_COURSE.fZoomScale = 55.0;
ZOOM_LEVEL_GOLF_COURSE.fZoomSpeed = 0.0;
ZOOM_LEVEL_GOLF_COURSE.fScrollSpeed = 0.1;
ZOOM_LEVEL_GOLF_COURSE.vTilesX = 2.0;
ZOOM_LEVEL_GOLF_COURSE.vTilesY = 1.0;

const ZOOM_LEVEL_INTERIOR = alt.MapZoomData.get('ZOOM_LEVEL_INTERIOR');
ZOOM_LEVEL_INTERIOR.fZoomScale = 450.0;
ZOOM_LEVEL_INTERIOR.fZoomSpeed = 0.0;
ZOOM_LEVEL_INTERIOR.fScrollSpeed = 0.1;
ZOOM_LEVEL_INTERIOR.vTilesX = 1.0;
ZOOM_LEVEL_INTERIOR.vTilesY = 1.0;

const ZOOM_LEVEL_GALLERY = alt.MapZoomData.get('ZOOM_LEVEL_GALLERY');
ZOOM_LEVEL_GALLERY.fZoomScale = 4.5;
ZOOM_LEVEL_GALLERY.fZoomSpeed = 0.0;
ZOOM_LEVEL_GALLERY.fScrollSpeed = 0.0;
ZOOM_LEVEL_GALLERY.vTilesX = 0.0;
ZOOM_LEVEL_GALLERY.vTilesY = 0.0;

const ZOOM_LEVEL_GALLERY_MAXIMIZE = alt.MapZoomData.get('ZOOM_LEVEL_GALLERY_MAXIMIZE');
ZOOM_LEVEL_GALLERY_MAXIMIZE.fZoomScale = 11.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.fZoomSpeed = 0.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.fScrollSpeed = 0.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.vTilesX = 2.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.vTilesY = 3.0;*/


//SATELLITE MAP
const ZOOM_LEVEL_0 = alt.MapZoomData.get('ZOOM_LEVEL_0');
ZOOM_LEVEL_0.fZoomScale = 2.75;
ZOOM_LEVEL_0.fZoomSpeed = 0.9;
ZOOM_LEVEL_0.fScrollSpeed = 0.08;
ZOOM_LEVEL_0.vTilesX = 0.0;
ZOOM_LEVEL_0.vTilesY = 0.0;

const ZOOM_LEVEL_1 = alt.MapZoomData.get('ZOOM_LEVEL_1');
ZOOM_LEVEL_1.fZoomScale = 2.8;
ZOOM_LEVEL_1.fZoomSpeed = 0.9;
ZOOM_LEVEL_1.fScrollSpeed = 0.08;
ZOOM_LEVEL_1.vTilesX = 0.0;
ZOOM_LEVEL_1.vTilesY = 0.0;

const ZOOM_LEVEL_2 = alt.MapZoomData.get('ZOOM_LEVEL_2');
ZOOM_LEVEL_2.fZoomScale = 8.0;
ZOOM_LEVEL_2.fZoomSpeed = 0.9;
ZOOM_LEVEL_2.fScrollSpeed = 0.08;
ZOOM_LEVEL_2.vTilesX = 0.0;
ZOOM_LEVEL_2.vTilesY = 0.0;

const ZOOM_LEVEL_3 = alt.MapZoomData.get('ZOOM_LEVEL_3');
ZOOM_LEVEL_3.fZoomScale = 20.0;
ZOOM_LEVEL_3.fZoomSpeed = 0.9;
ZOOM_LEVEL_3.fScrollSpeed = 0.08;
ZOOM_LEVEL_3.vTilesX = 0.0;
ZOOM_LEVEL_3.vTilesY = 0.0;

const ZOOM_LEVEL_4 = alt.MapZoomData.get('ZOOM_LEVEL_4');
ZOOM_LEVEL_4.fZoomScale = 35.0;
ZOOM_LEVEL_4.fZoomSpeed = 0.9;
ZOOM_LEVEL_4.fScrollSpeed = 0.08;
ZOOM_LEVEL_4.vTilesX = 0.0;
ZOOM_LEVEL_4.vTilesY = 0.0;

const ZOOM_LEVEL_GOLF_COURSE = alt.MapZoomData.get('ZOOM_LEVEL_GOLF_COURSE');
ZOOM_LEVEL_GOLF_COURSE.fZoomScale = 55.0;
ZOOM_LEVEL_GOLF_COURSE.fZoomSpeed = 0.0;
ZOOM_LEVEL_GOLF_COURSE.fScrollSpeed = 0.1;
ZOOM_LEVEL_GOLF_COURSE.vTilesX = 2.0;
ZOOM_LEVEL_GOLF_COURSE.vTilesY = 1.0;

const ZOOM_LEVEL_INTERIOR = alt.MapZoomData.get('ZOOM_LEVEL_INTERIOR');
ZOOM_LEVEL_INTERIOR.fZoomScale = 450.0;
ZOOM_LEVEL_INTERIOR.fZoomSpeed = 0.0;
ZOOM_LEVEL_INTERIOR.fScrollSpeed = 0.1;
ZOOM_LEVEL_INTERIOR.vTilesX = 1.0;
ZOOM_LEVEL_INTERIOR.vTilesY = 1.0;

const ZOOM_LEVEL_GALLERY = alt.MapZoomData.get('ZOOM_LEVEL_GALLERY');
ZOOM_LEVEL_GALLERY.fZoomScale = 4.5;
ZOOM_LEVEL_GALLERY.fZoomSpeed = 0.0;
ZOOM_LEVEL_GALLERY.fScrollSpeed = 0.0;
ZOOM_LEVEL_GALLERY.vTilesX = 0.0;
ZOOM_LEVEL_GALLERY.vTilesY = 0.0;

const ZOOM_LEVEL_GALLERY_MAXIMIZE = alt.MapZoomData.get('ZOOM_LEVEL_GALLERY_MAXIMIZE');
ZOOM_LEVEL_GALLERY_MAXIMIZE.fZoomScale = 11.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.fZoomSpeed = 0.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.fScrollSpeed = 0.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.vTilesX = 2.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.vTilesY = 3.0;

//16K Atlas Map
/*const ZOOM_LEVEL_0 = alt.MapZoomData.get('ZOOM_LEVEL_0');
ZOOM_LEVEL_0.fZoomScale = 2.75;
ZOOM_LEVEL_0.fZoomSpeed = 0.9;
ZOOM_LEVEL_0.fScrollSpeed = 0.08;
ZOOM_LEVEL_0.vTilesX = 0.0;
ZOOM_LEVEL_0.vTilesY = 0.0;

const ZOOM_LEVEL_1 = alt.MapZoomData.get('ZOOM_LEVEL_1');
ZOOM_LEVEL_1.fZoomScale = 2.8;
ZOOM_LEVEL_1.fZoomSpeed = 0.9;
ZOOM_LEVEL_1.fScrollSpeed = 0.08;
ZOOM_LEVEL_1.vTilesX = 0.0;
ZOOM_LEVEL_1.vTilesY = 0.0;

const ZOOM_LEVEL_2 = alt.MapZoomData.get('ZOOM_LEVEL_2');
ZOOM_LEVEL_2.fZoomScale = 8.0;
ZOOM_LEVEL_2.fZoomSpeed = 0.9;
ZOOM_LEVEL_2.fScrollSpeed = 0.08;
ZOOM_LEVEL_2.vTilesX = 0.0;
ZOOM_LEVEL_2.vTilesY = 0.0;

const ZOOM_LEVEL_3 = alt.MapZoomData.get('ZOOM_LEVEL_3');
ZOOM_LEVEL_3.fZoomScale = 20.0;
ZOOM_LEVEL_3.fZoomSpeed = 0.9;
ZOOM_LEVEL_3.fScrollSpeed = 0.08;
ZOOM_LEVEL_3.vTilesX = 0.0;
ZOOM_LEVEL_3.vTilesY = 0.0;

const ZOOM_LEVEL_4 = alt.MapZoomData.get('ZOOM_LEVEL_4');
ZOOM_LEVEL_4.fZoomScale = 35.0;
ZOOM_LEVEL_4.fZoomSpeed = 0.9;
ZOOM_LEVEL_4.fScrollSpeed = 0.08;
ZOOM_LEVEL_4.vTilesX = 0.0;
ZOOM_LEVEL_4.vTilesY = 0.0;

const ZOOM_LEVEL_GOLF_COURSE = alt.MapZoomData.get('ZOOM_LEVEL_GOLF_COURSE');
ZOOM_LEVEL_GOLF_COURSE.fZoomScale = 55.0;
ZOOM_LEVEL_GOLF_COURSE.fZoomSpeed = 0.0;
ZOOM_LEVEL_GOLF_COURSE.fScrollSpeed = 0.1;
ZOOM_LEVEL_GOLF_COURSE.vTilesX = 2.0;
ZOOM_LEVEL_GOLF_COURSE.vTilesY = 1.0;

const ZOOM_LEVEL_INTERIOR = alt.MapZoomData.get('ZOOM_LEVEL_INTERIOR');
ZOOM_LEVEL_INTERIOR.fZoomScale = 450.0;
ZOOM_LEVEL_INTERIOR.fZoomSpeed = 0.0;
ZOOM_LEVEL_INTERIOR.fScrollSpeed = 0.1;
ZOOM_LEVEL_INTERIOR.vTilesX = 1.0;
ZOOM_LEVEL_INTERIOR.vTilesY = 1.0;

const ZOOM_LEVEL_GALLERY = alt.MapZoomData.get('ZOOM_LEVEL_GALLERY');
ZOOM_LEVEL_GALLERY.fZoomScale = 4.5;
ZOOM_LEVEL_GALLERY.fZoomSpeed = 0.0;
ZOOM_LEVEL_GALLERY.fScrollSpeed = 0.0;
ZOOM_LEVEL_GALLERY.vTilesX = 0.0;
ZOOM_LEVEL_GALLERY.vTilesY = 0.0;

const ZOOM_LEVEL_GALLERY_MAXIMIZE = alt.MapZoomData.get('ZOOM_LEVEL_GALLERY_MAXIMIZE');
ZOOM_LEVEL_GALLERY_MAXIMIZE.fZoomScale = 11.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.fZoomSpeed = 0.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.fScrollSpeed = 0.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.vTilesX = 2.0;
ZOOM_LEVEL_GALLERY_MAXIMIZE.vTilesY = 3.0;*/


/*alt.everyTick(() => {
    // fix map zoom
    if (native.isPedOnFoot(alt.Player.local.scriptID)) {
      native.setRadarZoom(1100);
    } else if (native.isPedInAnyVehicle(alt.Player.local.scriptID, true)) {
      native.setRadarZoom(1100);
    }
  });*/
