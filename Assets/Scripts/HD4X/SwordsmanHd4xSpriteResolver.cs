using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static partial class SwordsmanHd4xSpriteResolver
{
    const string BundleName = "swordsman-hd4x";
    const string BundleFolder = "SwordsmanHD4X";
    struct SingleSpec
    {
        public string group;
        public string resourcePath;
        public string bundlePath;

        public SingleSpec( string group, string resourcePath, string bundlePath )
        {
            this.group = group;
            this.resourcePath = resourcePath;
            this.bundlePath = bundlePath;
        }
    }

    struct AtlasSpec
    {
        public string textureName;
        public string bundlePath;
        public int expectedSpriteCount;
        public string allowedSpriteNames;

        public AtlasSpec( string textureName, string bundlePath, int expectedSpriteCount, string allowedSpriteNames = null )
        {
            this.textureName = textureName;
            this.bundlePath = bundlePath;
            this.expectedSpriteCount = expectedSpriteCount;
            this.allowedSpriteNames = allowedSpriteNames;
        }
    }

    static readonly SingleSpec[] SingleSpecs =
    {
        new SingleSpec( "Tanm_00_t0", "texture/map/stage00/Tanm_00_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage00/tanm_00_t0.png" ),
        new SingleSpec( "map00-1", "texture/map/stage00/map00-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage00/map00-1.png" ),
        new SingleSpec( "map00-2", "texture/map/stage00/map00-2", "assets/__swordsmanhd4xstaging/resources/texture/map/stage00/map00-2.png" ),
        new SingleSpec( "map00-3", "texture/map/stage00/map00-3", "assets/__swordsmanhd4xstaging/resources/texture/map/stage00/map00-3.png" ),
        new SingleSpec( "map00", "texture/map/stage00/map00", "assets/__swordsmanhd4xstaging/resources/texture/map/stage00/map00.png" ),
        new SingleSpec( "Tanm_01_t0", "texture/map/stage01/Tanm_01_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage01/tanm_01_t0.png" ),
        new SingleSpec( "map01", "texture/map/stage01/map01", "assets/__swordsmanhd4xstaging/resources/texture/map/stage01/map01.png" ),
        new SingleSpec( "TANM_02_t0", "texture/map/stage02/TANM_02_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage02/tanm_02_t0.png" ),
        new SingleSpec( "map02", "texture/map/stage02/map02", "assets/__swordsmanhd4xstaging/resources/texture/map/stage02/map02.png" ),
        new SingleSpec( "TANM_03_t0", "texture/map/stage03/TANM_03_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage03/tanm_03_t0.png" ),
        new SingleSpec( "map03", "texture/map/stage03/map03", "assets/__swordsmanhd4xstaging/resources/texture/map/stage03/map03.png" ),
        new SingleSpec( "TANM_04_t0", "texture/map/stage04/TANM_04_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage04/tanm_04_t0.png" ),
        new SingleSpec( "map04-1", "texture/map/stage04/map04-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage04/map04-1.png" ),
        new SingleSpec( "map04", "texture/map/stage04/map04", "assets/__swordsmanhd4xstaging/resources/texture/map/stage04/map04.png" ),
        new SingleSpec( "TANM_05_t0", "texture/map/stage05/TANM_05_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage05/tanm_05_t0.png" ),
        new SingleSpec( "map05", "texture/map/stage05/map05", "assets/__swordsmanhd4xstaging/resources/texture/map/stage05/map05.png" ),
        new SingleSpec( "Map06-1", "texture/map/stage06/Map06-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage06/map06-1.png" ),
        new SingleSpec( "TANM_06_t0", "texture/map/stage06/TANM_06_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage06/tanm_06_t0.png" ),
        new SingleSpec( "map06", "texture/map/stage06/map06", "assets/__swordsmanhd4xstaging/resources/texture/map/stage06/map06.png" ),
        new SingleSpec( "TANM_07_t0", "texture/map/stage07/TANM_07_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage07/tanm_07_t0.png" ),
        new SingleSpec( "map07", "texture/map/stage07/map07", "assets/__swordsmanhd4xstaging/resources/texture/map/stage07/map07.png" ),
        new SingleSpec( "Map08", "texture/map/stage08/Map08", "assets/__swordsmanhd4xstaging/resources/texture/map/stage08/map08.png" ),
        new SingleSpec( "TANM_08_t0", "texture/map/stage08/TANM_08_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage08/tanm_08_t0.png" ),
        new SingleSpec( "MAP09", "texture/map/stage09/MAP09", "assets/__swordsmanhd4xstaging/resources/texture/map/stage09/map09.png" ),
        new SingleSpec( "TANM_09_t0", "texture/map/stage09/TANM_09_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage09/tanm_09_t0.png" ),
        new SingleSpec( "MAP10", "texture/map/stage10/MAP10", "assets/__swordsmanhd4xstaging/resources/texture/map/stage10/map10.png" ),
        new SingleSpec( "Map10-1", "texture/map/stage10/Map10-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage10/map10-1.png" ),
        new SingleSpec( "TANM_10_t0", "texture/map/stage10/TANM_10_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage10/tanm_10_t0.png" ),
        new SingleSpec( "Map11-1", "texture/map/stage11/Map11-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage11/map11-1.png" ),
        new SingleSpec( "Map11", "texture/map/stage11/Map11", "assets/__swordsmanhd4xstaging/resources/texture/map/stage11/map11.png" ),
        new SingleSpec( "TANM_11_t0", "texture/map/stage11/TANM_11_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage11/tanm_11_t0.png" ),
        new SingleSpec( "Map12", "texture/map/stage12/Map12", "assets/__swordsmanhd4xstaging/resources/texture/map/stage12/map12.png" ),
        new SingleSpec( "TANM_12_t0", "texture/map/stage12/TANM_12_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage12/tanm_12_t0.png" ),
        new SingleSpec( "Map13-1", "texture/map/stage13/Map13-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage13/map13-1.png" ),
        new SingleSpec( "Map13-2", "texture/map/stage13/Map13-2", "assets/__swordsmanhd4xstaging/resources/texture/map/stage13/map13-2.png" ),
        new SingleSpec( "Map13", "texture/map/stage13/Map13", "assets/__swordsmanhd4xstaging/resources/texture/map/stage13/map13.png" ),
        new SingleSpec( "TANM_13_t0", "texture/map/stage13/TANM_13_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage13/tanm_13_t0.png" ),
        new SingleSpec( "Map14", "texture/map/stage14/Map14", "assets/__swordsmanhd4xstaging/resources/texture/map/stage14/map14.png" ),
        new SingleSpec( "TANM_14_t0", "texture/map/stage14/TANM_14_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage14/tanm_14_t0.png" ),
        new SingleSpec( "Map15", "texture/map/stage15/Map15", "assets/__swordsmanhd4xstaging/resources/texture/map/stage15/map15.png" ),
        new SingleSpec( "TANM_15_t0", "texture/map/stage15/TANM_15_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage15/tanm_15_t0.png" ),
        new SingleSpec( "Map16", "texture/map/stage16/Map16", "assets/__swordsmanhd4xstaging/resources/texture/map/stage16/map16.png" ),
        new SingleSpec( "TANM_16_t0", "texture/map/stage16/TANM_16_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage16/tanm_16_t0.png" ),
        new SingleSpec( "Map17", "texture/map/stage17/Map17", "assets/__swordsmanhd4xstaging/resources/texture/map/stage17/map17.png" ),
        new SingleSpec( "TANM_17_t0", "texture/map/stage17/TANM_17_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage17/tanm_17_t0.png" ),
        new SingleSpec( "Map18-1", "texture/map/stage18/Map18-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage18/map18-1.png" ),
        new SingleSpec( "Map18", "texture/map/stage18/Map18", "assets/__swordsmanhd4xstaging/resources/texture/map/stage18/map18.png" ),
        new SingleSpec( "TANM_18_t0", "texture/map/stage18/TANM_18_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage18/tanm_18_t0.png" ),
        new SingleSpec( "Map19", "texture/map/stage19/Map19", "assets/__swordsmanhd4xstaging/resources/texture/map/stage19/map19.png" ),
        new SingleSpec( "TANM_19_t0", "texture/map/stage19/TANM_19_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage19/tanm_19_t0.png" ),
        new SingleSpec( "Map20", "texture/map/stage20/Map20", "assets/__swordsmanhd4xstaging/resources/texture/map/stage20/map20.png" ),
        new SingleSpec( "TANM_20_t0", "texture/map/stage20/TANM_20_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage20/tanm_20_t0.png" ),
        new SingleSpec( "map20-1", "texture/map/stage20/map20-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage20/map20-1.png" ),
        new SingleSpec( "map20-2", "texture/map/stage20/map20-2", "assets/__swordsmanhd4xstaging/resources/texture/map/stage20/map20-2.png" ),
        new SingleSpec( "Map21", "texture/map/stage21/Map21", "assets/__swordsmanhd4xstaging/resources/texture/map/stage21/map21.png" ),
        new SingleSpec( "TANM_21_t0", "texture/map/stage21/TANM_21_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage21/tanm_21_t0.png" ),
        new SingleSpec( "Map22-1", "texture/map/stage22/Map22-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage22/map22-1.png" ),
        new SingleSpec( "Map22", "texture/map/stage22/Map22", "assets/__swordsmanhd4xstaging/resources/texture/map/stage22/map22.png" ),
        new SingleSpec( "TANM_22_t0", "texture/map/stage22/TANM_22_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage22/tanm_22_t0.png" ),
        new SingleSpec( "Map23", "texture/map/stage23/Map23", "assets/__swordsmanhd4xstaging/resources/texture/map/stage23/map23.png" ),
        new SingleSpec( "TANM_23_t0", "texture/map/stage23/TANM_23_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage23/tanm_23_t0.png" ),
        new SingleSpec( "map23-1", "texture/map/stage23/map23-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage23/map23-1.png" ),
        new SingleSpec( "map23-2", "texture/map/stage23/map23-2", "assets/__swordsmanhd4xstaging/resources/texture/map/stage23/map23-2.png" ),
        new SingleSpec( "Map24", "texture/map/stage24/Map24", "assets/__swordsmanhd4xstaging/resources/texture/map/stage24/map24.png" ),
        new SingleSpec( "TANM_24_t0", "texture/map/stage24/TANM_24_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage24/tanm_24_t0.png" ),
        new SingleSpec( "map24-1", "texture/map/stage24/map24-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage24/map24-1.png" ),
        new SingleSpec( "Map25-1", "texture/map/stage25/Map25-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage25/map25-1.png" ),
        new SingleSpec( "Map25", "texture/map/stage25/Map25", "assets/__swordsmanhd4xstaging/resources/texture/map/stage25/map25.png" ),
        new SingleSpec( "TANM_25_t0", "texture/map/stage25/TANM_25_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage25/tanm_25_t0.png" ),
        new SingleSpec( "Map26-1", "texture/map/stage26/Map26-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage26/map26-1.png" ),
        new SingleSpec( "Map26", "texture/map/stage26/Map26", "assets/__swordsmanhd4xstaging/resources/texture/map/stage26/map26.png" ),
        new SingleSpec( "TANM_26_t0", "texture/map/stage26/TANM_26_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage26/tanm_26_t0.png" ),
        new SingleSpec( "Map27", "texture/map/stage27/Map27", "assets/__swordsmanhd4xstaging/resources/texture/map/stage27/map27.png" ),
        new SingleSpec( "TANM_27_t0", "texture/map/stage27/TANM_27_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage27/tanm_27_t0.png" ),
        new SingleSpec( "Map28-1", "texture/map/stage28/Map28-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage28/map28-1.png" ),
        new SingleSpec( "Map28", "texture/map/stage28/Map28", "assets/__swordsmanhd4xstaging/resources/texture/map/stage28/map28.png" ),
        new SingleSpec( "TANM_28_t0", "texture/map/stage28/TANM_28_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage28/tanm_28_t0.png" ),
        new SingleSpec( "map28-2", "texture/map/stage28/map28-2", "assets/__swordsmanhd4xstaging/resources/texture/map/stage28/map28-2.png" ),
        new SingleSpec( "Map29", "texture/map/stage29/Map29", "assets/__swordsmanhd4xstaging/resources/texture/map/stage29/map29.png" ),
        new SingleSpec( "TANM_29_t0", "texture/map/stage29/TANM_29_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage29/tanm_29_t0.png" ),
        new SingleSpec( "map29-1", "texture/map/stage29/map29-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage29/map29-1.png" ),
        new SingleSpec( "Map30", "texture/map/stage30/Map30", "assets/__swordsmanhd4xstaging/resources/texture/map/stage30/map30.png" ),
        new SingleSpec( "TANM_30_t0", "texture/map/stage30/TANM_30_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage30/tanm_30_t0.png" ),
        new SingleSpec( "Map31-1", "texture/map/stage31/Map31-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage31/map31-1.png" ),
        new SingleSpec( "Map31", "texture/map/stage31/Map31", "assets/__swordsmanhd4xstaging/resources/texture/map/stage31/map31.png" ),
        new SingleSpec( "TANM_31_t0", "texture/map/stage31/TANM_31_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage31/tanm_31_t0.png" ),
        new SingleSpec( "map31-2", "texture/map/stage31/map31-2", "assets/__swordsmanhd4xstaging/resources/texture/map/stage31/map31-2.png" ),
        new SingleSpec( "Map32", "texture/map/stage32/Map32", "assets/__swordsmanhd4xstaging/resources/texture/map/stage32/map32.png" ),
        new SingleSpec( "TANM_32_t0", "texture/map/stage32/TANM_32_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage32/tanm_32_t0.png" ),
        new SingleSpec( "Map33", "texture/map/stage33/Map33", "assets/__swordsmanhd4xstaging/resources/texture/map/stage33/map33.png" ),
        new SingleSpec( "TANM_33_t0", "texture/map/stage33/TANM_33_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage33/tanm_33_t0.png" ),
        new SingleSpec( "Map34-3", "texture/map/stage34/Map34-3", "assets/__swordsmanhd4xstaging/resources/texture/map/stage34/map34-3.png" ),
        new SingleSpec( "Map34", "texture/map/stage34/Map34", "assets/__swordsmanhd4xstaging/resources/texture/map/stage34/map34.png" ),
        new SingleSpec( "TANM_34_t0", "texture/map/stage34/TANM_34_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage34/tanm_34_t0.png" ),
        new SingleSpec( "map34-1", "texture/map/stage34/map34-1", "assets/__swordsmanhd4xstaging/resources/texture/map/stage34/map34-1.png" ),
        new SingleSpec( "map34-2", "texture/map/stage34/map34-2", "assets/__swordsmanhd4xstaging/resources/texture/map/stage34/map34-2.png" ),
        new SingleSpec( "map34-4", "texture/map/stage34/map34-4", "assets/__swordsmanhd4xstaging/resources/texture/map/stage34/map34-4.png" ),
        new SingleSpec( "Map35", "texture/map/stage35/Map35", "assets/__swordsmanhd4xstaging/resources/texture/map/stage35/map35.png" ),
        new SingleSpec( "TANM_35_t0", "texture/map/stage35/TANM_35_t0", "assets/__swordsmanhd4xstaging/resources/texture/map/stage35/tanm_35_t0.png" ),
        new SingleSpec( "map00", "texture/rpg/map00", "assets/__swordsmanhd4xstaging/resources/texture/rpg/map00.png" ),
        new SingleSpec( "map02", "texture/rpg/map02", "assets/__swordsmanhd4xstaging/resources/texture/rpg/map02.png" ),
        new SingleSpec( "map05", "texture/rpg/map05", "assets/__swordsmanhd4xstaging/resources/texture/rpg/map05.png" ),
        new SingleSpec( "map06", "texture/rpg/map06", "assets/__swordsmanhd4xstaging/resources/texture/rpg/map06.png" ),
        new SingleSpec( "map11", "texture/rpg/map11", "assets/__swordsmanhd4xstaging/resources/texture/rpg/map11.png" ),
        new SingleSpec( "map12", "texture/rpg/map12", "assets/__swordsmanhd4xstaging/resources/texture/rpg/map12.png" ),
        new SingleSpec( "map17", "texture/rpg/map17", "assets/__swordsmanhd4xstaging/resources/texture/rpg/map17.png" ),
        new SingleSpec( "map18", "texture/rpg/map18", "assets/__swordsmanhd4xstaging/resources/texture/rpg/map18.png" ),
        new SingleSpec( "map26", "texture/rpg/map26", "assets/__swordsmanhd4xstaging/resources/texture/rpg/map26.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage00/floor_00/FLOOR_00_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage00/floor_00/floor_00_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage00/floor_00/FLOOR_00_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage00/floor_00/floor_00_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage00/floor_00/FLOOR_00_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage00/floor_00/floor_00_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage00/floor_00/FLOOR_00_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage00/floor_00/floor_00_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage00/floor_00/FLOOR_00_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage00/floor_00/floor_00_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage00/floor_00/FLOOR_00_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage00/floor_00/floor_00_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage01/floor_01/Floor_01_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage01/floor_01/floor_01_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage01/floor_01/Floor_01_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage01/floor_01/floor_01_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage01/floor_01/Floor_01_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage01/floor_01/floor_01_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage01/floor_01/Floor_01_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage01/floor_01/floor_01_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage01/floor_01/Floor_01_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage01/floor_01/floor_01_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage01/floor_01/Floor_01_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage01/floor_01/floor_01_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage02/floor_02/Floor_02_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage02/floor_02/floor_02_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage02/floor_02/Floor_02_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage02/floor_02/floor_02_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage02/floor_02/Floor_02_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage02/floor_02/floor_02_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage02/floor_02/Floor_02_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage02/floor_02/floor_02_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage02/floor_02/Floor_02_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage02/floor_02/floor_02_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage02/floor_02/Floor_02_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage02/floor_02/floor_02_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage03/floor_03/Floor_03_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage03/floor_03/floor_03_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage03/floor_03/Floor_03_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage03/floor_03/floor_03_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage03/floor_03/Floor_03_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage03/floor_03/floor_03_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage03/floor_03/Floor_03_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage03/floor_03/floor_03_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage03/floor_03/Floor_03_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage03/floor_03/floor_03_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage03/floor_03/Floor_03_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage03/floor_03/floor_03_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage04/floor_04/Floor_04_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage04/floor_04/floor_04_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage04/floor_04/Floor_04_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage04/floor_04/floor_04_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage04/floor_04/Floor_04_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage04/floor_04/floor_04_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage04/floor_04/Floor_04_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage04/floor_04/floor_04_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage04/floor_04/Floor_04_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage04/floor_04/floor_04_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage04/floor_04/Floor_04_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage04/floor_04/floor_04_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage05/floor_05/Floor_05_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage05/floor_05/floor_05_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage05/floor_05/Floor_05_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage05/floor_05/floor_05_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage05/floor_05/Floor_05_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage05/floor_05/floor_05_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage05/floor_05/Floor_05_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage05/floor_05/floor_05_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage05/floor_05/Floor_05_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage05/floor_05/floor_05_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage05/floor_05/Floor_05_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage05/floor_05/floor_05_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage06/floor_06/Floor_06_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage06/floor_06/floor_06_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage06/floor_06/Floor_06_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage06/floor_06/floor_06_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage06/floor_06/Floor_06_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage06/floor_06/floor_06_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage06/floor_06/Floor_06_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage06/floor_06/floor_06_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage06/floor_06/Floor_06_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage06/floor_06/floor_06_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage06/floor_06/Floor_06_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage06/floor_06/floor_06_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage07/floor_07/Floor_07_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage07/floor_07/floor_07_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage07/floor_07/Floor_07_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage07/floor_07/floor_07_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage07/floor_07/Floor_07_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage07/floor_07/floor_07_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage07/floor_07/Floor_07_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage07/floor_07/floor_07_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage07/floor_07/Floor_07_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage07/floor_07/floor_07_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage07/floor_07/Floor_07_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage07/floor_07/floor_07_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage08/floor_08/Floor_08_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage08/floor_08/floor_08_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage08/floor_08/Floor_08_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage08/floor_08/floor_08_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage08/floor_08/Floor_08_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage08/floor_08/floor_08_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage08/floor_08/Floor_08_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage08/floor_08/floor_08_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage08/floor_08/Floor_08_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage08/floor_08/floor_08_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage08/floor_08/Floor_08_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage08/floor_08/floor_08_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage09/floor_09/FLOOR_09_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage09/floor_09/floor_09_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage09/floor_09/FLOOR_09_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage09/floor_09/floor_09_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage09/floor_09/FLOOR_09_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage09/floor_09/floor_09_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage09/floor_09/FLOOR_09_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage09/floor_09/floor_09_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage09/floor_09/FLOOR_09_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage09/floor_09/floor_09_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage09/floor_09/FLOOR_09_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage09/floor_09/floor_09_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage10/floor_10/FLOOR_10_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage10/floor_10/floor_10_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage10/floor_10/FLOOR_10_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage10/floor_10/floor_10_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage10/floor_10/FLOOR_10_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage10/floor_10/floor_10_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage10/floor_10/FLOOR_10_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage10/floor_10/floor_10_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage10/floor_10/FLOOR_10_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage10/floor_10/floor_10_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage10/floor_10/FLOOR_10_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage10/floor_10/floor_10_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage11/floor_11/FLOOR_11_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage11/floor_11/floor_11_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage11/floor_11/FLOOR_11_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage11/floor_11/floor_11_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage11/floor_11/FLOOR_11_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage11/floor_11/floor_11_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage11/floor_11/FLOOR_11_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage11/floor_11/floor_11_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage11/floor_11/FLOOR_11_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage11/floor_11/floor_11_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage11/floor_11/FLOOR_11_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage11/floor_11/floor_11_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage12/floor_12/Floor_12_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage12/floor_12/floor_12_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage12/floor_12/Floor_12_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage12/floor_12/floor_12_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage12/floor_12/Floor_12_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage12/floor_12/floor_12_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage12/floor_12/Floor_12_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage12/floor_12/floor_12_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage12/floor_12/Floor_12_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage12/floor_12/floor_12_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage12/floor_12/Floor_12_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage12/floor_12/floor_12_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage13/floor_13/Floor_13_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage13/floor_13/floor_13_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage13/floor_13/Floor_13_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage13/floor_13/floor_13_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage13/floor_13/Floor_13_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage13/floor_13/floor_13_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage13/floor_13/Floor_13_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage13/floor_13/floor_13_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage13/floor_13/Floor_13_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage13/floor_13/floor_13_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage13/floor_13/Floor_13_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage13/floor_13/floor_13_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage14/floor_14/Floor_14_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage14/floor_14/floor_14_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage14/floor_14/Floor_14_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage14/floor_14/floor_14_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage14/floor_14/Floor_14_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage14/floor_14/floor_14_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage14/floor_14/Floor_14_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage14/floor_14/floor_14_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage14/floor_14/Floor_14_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage14/floor_14/floor_14_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage14/floor_14/Floor_14_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage14/floor_14/floor_14_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage15/floor_15/Floor_15_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage15/floor_15/floor_15_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage15/floor_15/Floor_15_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage15/floor_15/floor_15_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage15/floor_15/Floor_15_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage15/floor_15/floor_15_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage15/floor_15/Floor_15_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage15/floor_15/floor_15_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage15/floor_15/Floor_15_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage15/floor_15/floor_15_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage15/floor_15/Floor_15_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage15/floor_15/floor_15_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage16/floor_16/Floor_16_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage16/floor_16/floor_16_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage16/floor_16/Floor_16_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage16/floor_16/floor_16_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage16/floor_16/Floor_16_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage16/floor_16/floor_16_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage16/floor_16/Floor_16_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage16/floor_16/floor_16_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage16/floor_16/Floor_16_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage16/floor_16/floor_16_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage16/floor_16/Floor_16_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage16/floor_16/floor_16_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage17/floor_17/Floor_17_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage17/floor_17/floor_17_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage17/floor_17/Floor_17_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage17/floor_17/floor_17_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage17/floor_17/Floor_17_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage17/floor_17/floor_17_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage17/floor_17/Floor_17_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage17/floor_17/floor_17_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage17/floor_17/Floor_17_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage17/floor_17/floor_17_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage17/floor_17/Floor_17_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage17/floor_17/floor_17_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage18/floor_18/Floor_18_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage18/floor_18/floor_18_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage18/floor_18/Floor_18_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage18/floor_18/floor_18_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage18/floor_18/Floor_18_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage18/floor_18/floor_18_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage18/floor_18/Floor_18_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage18/floor_18/floor_18_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage18/floor_18/Floor_18_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage18/floor_18/floor_18_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage18/floor_18/Floor_18_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage18/floor_18/floor_18_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage19/floor_19/Floor_19_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage19/floor_19/floor_19_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage19/floor_19/Floor_19_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage19/floor_19/floor_19_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage19/floor_19/Floor_19_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage19/floor_19/floor_19_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage19/floor_19/Floor_19_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage19/floor_19/floor_19_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage19/floor_19/Floor_19_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage19/floor_19/floor_19_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage19/floor_19/Floor_19_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage19/floor_19/floor_19_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage20/floor_20/Floor_20_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage20/floor_20/floor_20_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage20/floor_20/Floor_20_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage20/floor_20/floor_20_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage20/floor_20/Floor_20_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage20/floor_20/floor_20_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage20/floor_20/Floor_20_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage20/floor_20/floor_20_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage20/floor_20/Floor_20_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage20/floor_20/floor_20_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage20/floor_20/Floor_20_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage20/floor_20/floor_20_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage21/floor_21/Floor_21_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage21/floor_21/floor_21_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage21/floor_21/Floor_21_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage21/floor_21/floor_21_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage21/floor_21/Floor_21_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage21/floor_21/floor_21_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage21/floor_21/Floor_21_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage21/floor_21/floor_21_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage21/floor_21/Floor_21_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage21/floor_21/floor_21_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage21/floor_21/Floor_21_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage21/floor_21/floor_21_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage22/floor_22/Floor_22_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage22/floor_22/floor_22_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage22/floor_22/Floor_22_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage22/floor_22/floor_22_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage22/floor_22/Floor_22_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage22/floor_22/floor_22_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage22/floor_22/Floor_22_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage22/floor_22/floor_22_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage22/floor_22/Floor_22_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage22/floor_22/floor_22_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage22/floor_22/Floor_22_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage22/floor_22/floor_22_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage23/floor_23/Floor_23_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage23/floor_23/floor_23_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage23/floor_23/Floor_23_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage23/floor_23/floor_23_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage23/floor_23/Floor_23_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage23/floor_23/floor_23_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage23/floor_23/Floor_23_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage23/floor_23/floor_23_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage23/floor_23/Floor_23_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage23/floor_23/floor_23_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage23/floor_23/Floor_23_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage23/floor_23/floor_23_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage24/floor_24/Floor_24_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage24/floor_24/floor_24_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage24/floor_24/Floor_24_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage24/floor_24/floor_24_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage24/floor_24/Floor_24_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage24/floor_24/floor_24_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage24/floor_24/Floor_24_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage24/floor_24/floor_24_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage24/floor_24/Floor_24_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage24/floor_24/floor_24_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage24/floor_24/Floor_24_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage24/floor_24/floor_24_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage25/floor_25/Floor_25_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage25/floor_25/floor_25_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage25/floor_25/Floor_25_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage25/floor_25/floor_25_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage25/floor_25/Floor_25_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage25/floor_25/floor_25_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage25/floor_25/Floor_25_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage25/floor_25/floor_25_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage25/floor_25/Floor_25_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage25/floor_25/floor_25_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage25/floor_25/Floor_25_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage25/floor_25/floor_25_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage26/floor_26/Floor_26_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage26/floor_26/floor_26_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage26/floor_26/Floor_26_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage26/floor_26/floor_26_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage26/floor_26/Floor_26_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage26/floor_26/floor_26_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage26/floor_26/Floor_26_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage26/floor_26/floor_26_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage26/floor_26/Floor_26_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage26/floor_26/floor_26_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage26/floor_26/Floor_26_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage26/floor_26/floor_26_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage27/floor_27/Floor_27_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage27/floor_27/floor_27_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage27/floor_27/Floor_27_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage27/floor_27/floor_27_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage27/floor_27/Floor_27_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage27/floor_27/floor_27_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage27/floor_27/Floor_27_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage27/floor_27/floor_27_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage27/floor_27/Floor_27_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage27/floor_27/floor_27_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage27/floor_27/Floor_27_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage27/floor_27/floor_27_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage28/floor_28/Floor_28_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage28/floor_28/floor_28_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage28/floor_28/Floor_28_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage28/floor_28/floor_28_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage28/floor_28/Floor_28_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage28/floor_28/floor_28_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage28/floor_28/Floor_28_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage28/floor_28/floor_28_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage28/floor_28/Floor_28_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage28/floor_28/floor_28_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage28/floor_28/Floor_28_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage28/floor_28/floor_28_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage29/floor_29/Floor_29_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage29/floor_29/floor_29_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage29/floor_29/Floor_29_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage29/floor_29/floor_29_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage29/floor_29/Floor_29_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage29/floor_29/floor_29_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage29/floor_29/Floor_29_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage29/floor_29/floor_29_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage29/floor_29/Floor_29_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage29/floor_29/floor_29_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage29/floor_29/Floor_29_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage29/floor_29/floor_29_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage30/floor_30/Floor_30_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage30/floor_30/floor_30_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage30/floor_30/Floor_30_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage30/floor_30/floor_30_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage30/floor_30/Floor_30_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage30/floor_30/floor_30_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage30/floor_30/Floor_30_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage30/floor_30/floor_30_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage30/floor_30/Floor_30_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage30/floor_30/floor_30_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage30/floor_30/Floor_30_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage30/floor_30/floor_30_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage31/floor_31/Floor_31_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage31/floor_31/floor_31_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage31/floor_31/Floor_31_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage31/floor_31/floor_31_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage31/floor_31/Floor_31_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage31/floor_31/floor_31_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage31/floor_31/Floor_31_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage31/floor_31/floor_31_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage31/floor_31/Floor_31_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage31/floor_31/floor_31_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage31/floor_31/Floor_31_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage31/floor_31/floor_31_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage32/floor_32/Floor_32_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage32/floor_32/floor_32_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage32/floor_32/Floor_32_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage32/floor_32/floor_32_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage32/floor_32/Floor_32_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage32/floor_32/floor_32_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage32/floor_32/Floor_32_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage32/floor_32/floor_32_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage32/floor_32/Floor_32_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage32/floor_32/floor_32_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage32/floor_32/Floor_32_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage32/floor_32/floor_32_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage32/floor_32/Floor_32_t6", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage32/floor_32/floor_32_t6.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage32/floor_32/Floor_32_t7", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage32/floor_32/floor_32_t7.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage33/floor_33/Floor_33_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage33/floor_33/floor_33_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage33/floor_33/Floor_33_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage33/floor_33/floor_33_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage33/floor_33/Floor_33_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage33/floor_33/floor_33_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage33/floor_33/Floor_33_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage33/floor_33/floor_33_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage33/floor_33/Floor_33_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage33/floor_33/floor_33_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage33/floor_33/Floor_33_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage33/floor_33/floor_33_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage34/floor_34/Floor_34_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage34/floor_34/floor_34_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage34/floor_34/Floor_34_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage34/floor_34/floor_34_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage34/floor_34/Floor_34_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage34/floor_34/floor_34_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage34/floor_34/Floor_34_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage34/floor_34/floor_34_t3.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage34/floor_34/Floor_34_t4", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage34/floor_34/floor_34_t4.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage34/floor_34/Floor_34_t5", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage34/floor_34/floor_34_t5.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage35/floor_35/Floor_35_t0", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage35/floor_35/floor_35_t0.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage35/floor_35/Floor_35_t1", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage35/floor_35/floor_35_t1.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage35/floor_35/Floor_35_t2", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage35/floor_35/floor_35_t2.png" ),
        new SingleSpec( "Fixed attack terrain", "texture/stage/stage35/floor_35/Floor_35_t3", "assets/__swordsmanhd4xstaging/resources/texture/stage/stage35/floor_35/floor_35_t3.png" ),
    };

    static readonly AtlasSpec[] PortraitAtlasSpecs =
    {
        new AtlasSpec( "FASE07", "assets/__swordsmanhd4xstaging/texture2d/fase07.png", 5, "FASE07_t0|FASE07_t1|FASE07_t2|FASE07_t3|FASE07_t4" ),
        new AtlasSpec( "Face50", "assets/__swordsmanhd4xstaging/texture2d/face50.png", 1, "Face50_t0" ),
        new AtlasSpec( "Face48", "assets/__swordsmanhd4xstaging/texture2d/face48.png", 1, "Face48_t0" ),
        new AtlasSpec( "Face47", "assets/__swordsmanhd4xstaging/texture2d/face47.png", 1, "Face47_t0" ),
        new AtlasSpec( "Face46", "assets/__swordsmanhd4xstaging/texture2d/face46.png", 1, "Face46_t0" ),
        new AtlasSpec( "Face42", "assets/__swordsmanhd4xstaging/texture2d/face42.png", 1, "Face42_t0" ),
        new AtlasSpec( "Face29", "assets/__swordsmanhd4xstaging/texture2d/face29.png", 1, "Face29_t0" ),
        new AtlasSpec( "Face21", "assets/__swordsmanhd4xstaging/texture2d/face21.png", 1, "Face21_t0" ),
        new AtlasSpec( "Face18", "assets/__swordsmanhd4xstaging/texture2d/face18.png", 2, "Face18_t0|Face18_t1" ),
        new AtlasSpec( "FACE01", "assets/__swordsmanhd4xstaging/texture2d/face01.png", 10, "FACE01_t0|FACE01_t1|FACE01_t2|FACE01_t3|FACE01_t4|FACE01_t5|FACE01_t6|FACE01_t7|FACE01_t8|FACE01_t9" ),
        new AtlasSpec( "FACE02", "assets/__swordsmanhd4xstaging/texture2d/face02.png", 12, "FACE02_t0|FACE02_t1|FACE02_t10|FACE02_t11|FACE02_t2|FACE02_t3|FACE02_t4|FACE02_t5|FACE02_t6|FACE02_t7|FACE02_t8|FACE02_t9" ),
        new AtlasSpec( "FACE03", "assets/__swordsmanhd4xstaging/texture2d/face03.png", 8, "FACE03_t0|FACE03_t1|FACE03_t2|FACE03_t3|FACE03_t4|FACE03_t5|FACE03_t6|FACE03_t7" ),
        new AtlasSpec( "FACE04", "assets/__swordsmanhd4xstaging/texture2d/face04.png", 8, "FACE04_t0|FACE04_t1|FACE04_t2|FACE04_t3|FACE04_t4|FACE04_t5|FACE04_t6|FACE04_t7" ),
        new AtlasSpec( "Face05", "assets/__swordsmanhd4xstaging/texture2d/face05.png", 3, "Face05_t0|Face05_t1|Face05_t2" ),
        new AtlasSpec( "Face06", "assets/__swordsmanhd4xstaging/texture2d/face06.png", 5, "Face06_t0|Face06_t1|Face06_t2|Face06_t3|Face06_t4" ),
        new AtlasSpec( "Face07", "assets/__swordsmanhd4xstaging/texture2d/face07.png", 4, "Face07_t0|Face07_t1|Face07_t2|Face07_t3" ),
        new AtlasSpec( "Face08", "assets/__swordsmanhd4xstaging/texture2d/face08.png", 6, "Face08_t0|Face08_t1|Face08_t2|Face08_t3|Face08_t4|Face08_t5" ),
        new AtlasSpec( "Face09", "assets/__swordsmanhd4xstaging/texture2d/face09.png", 4, "Face09_t0|Face09_t1|Face09_t2|Face09_t3" ),
        new AtlasSpec( "Face10", "assets/__swordsmanhd4xstaging/texture2d/face10.png", 5, "Face10_t0|Face10_t1|Face10_t2|Face10_t3|Face10_t4" ),
        new AtlasSpec( "Face11", "assets/__swordsmanhd4xstaging/texture2d/face11.png", 3, "Face11_t0|Face11_t1|Face11_t2" ),
        new AtlasSpec( "Face15", "assets/__swordsmanhd4xstaging/texture2d/face15.png", 2, "Face15_t0|Face15_t1" ),
        new AtlasSpec( "Face16", "assets/__swordsmanhd4xstaging/texture2d/face16.png", 2, "Face16_t0|Face16_t1" ),
        new AtlasSpec( "Face17", "assets/__swordsmanhd4xstaging/texture2d/face17.png", 4, "Face17_t0|Face17_t1|Face17_t2|Face17_t3" ),
        new AtlasSpec( "Face19", "assets/__swordsmanhd4xstaging/texture2d/face19.png", 2, "Face19_t0|Face19_t1" ),
        new AtlasSpec( "Face20", "assets/__swordsmanhd4xstaging/texture2d/face20.png", 3, "Face20_t0|Face20_t1|Face20_t2" ),
        new AtlasSpec( "Face22", "assets/__swordsmanhd4xstaging/texture2d/face22.png", 3, "Face22_t0|Face22_t1|Face22_t2" ),
        new AtlasSpec( "Face23", "assets/__swordsmanhd4xstaging/texture2d/face23.png", 3, "Face23_t0|Face23_t1|Face23_t2" ),
        new AtlasSpec( "Face24", "assets/__swordsmanhd4xstaging/texture2d/face24.png", 8, "Face24_t0|Face24_t1|Face24_t2|Face24_t3|Face24_t4|Face24_t5|Face24_t6|Face24_t7" ),
        new AtlasSpec( "Face25", "assets/__swordsmanhd4xstaging/texture2d/face25.png", 4, "Face25_t0|Face25_t1|Face25_t2|Face25_t3" ),
        new AtlasSpec( "Face26", "assets/__swordsmanhd4xstaging/texture2d/face26.png", 1, "Face26_t0" ),
        new AtlasSpec( "Face27", "assets/__swordsmanhd4xstaging/texture2d/face27.png", 3, "Face27_t0|Face27_t1|Face27_t2" ),
        new AtlasSpec( "Face28", "assets/__swordsmanhd4xstaging/texture2d/face28.png", 1, "Face28_t0" ),
        new AtlasSpec( "Face30", "assets/__swordsmanhd4xstaging/texture2d/face30.png", 3, "Face30_t0|Face30_t1|Face30_t2" ),
        new AtlasSpec( "Face31", "assets/__swordsmanhd4xstaging/texture2d/face31.png", 2, "Face31_t0|Face31_t1" ),
        new AtlasSpec( "Face32", "assets/__swordsmanhd4xstaging/texture2d/face32.png", 3, "Face32_t0|Face32_t1|Face32_t2" ),
        new AtlasSpec( "Face33", "assets/__swordsmanhd4xstaging/texture2d/face33.png", 3, "Face33_t0|Face33_t1|Face33_t2" ),
        new AtlasSpec( "Face34", "assets/__swordsmanhd4xstaging/texture2d/face34.png", 5, "Face34_t0|Face34_t1|Face34_t2|Face34_t3|Face34_t4" ),
        new AtlasSpec( "Face35", "assets/__swordsmanhd4xstaging/texture2d/face35.png", 5, "Face35_t0|Face35_t1|Face35_t2|Face35_t3|Face35_t4" ),
        new AtlasSpec( "Face36", "assets/__swordsmanhd4xstaging/texture2d/face36.png", 5, "Face36_t0|Face36_t1|Face36_t2|Face36_t3|Face36_t4" ),
        new AtlasSpec( "Face37", "assets/__swordsmanhd4xstaging/texture2d/face37.png", 3, "Face37_t0|Face37_t1|Face37_t2" ),
        new AtlasSpec( "Face38", "assets/__swordsmanhd4xstaging/texture2d/face38.png", 5, "Face38_t0|Face38_t1|Face38_t2|Face38_t3|Face38_t4" ),
        new AtlasSpec( "Face39", "assets/__swordsmanhd4xstaging/texture2d/face39.png", 3, "Face39_t0|Face39_t1|Face39_t2" ),
        new AtlasSpec( "Face40", "assets/__swordsmanhd4xstaging/texture2d/face40.png", 2, "Face40_t0|Face40_t1" ),
        new AtlasSpec( "Face41", "assets/__swordsmanhd4xstaging/texture2d/face41.png", 2, "Face41_t0|Face41_t1" ),
        new AtlasSpec( "Face43", "assets/__swordsmanhd4xstaging/texture2d/face43.png", 1, "Face43_t0" ),
        new AtlasSpec( "Face44", "assets/__swordsmanhd4xstaging/texture2d/face44.png", 1, "Face44_t0" ),
        new AtlasSpec( "Face45", "assets/__swordsmanhd4xstaging/texture2d/face45.png", 1, "Face45_t0" ),
        new AtlasSpec( "Face49", "assets/__swordsmanhd4xstaging/texture2d/face49.png", 2, "Face49_t0|Face49_t1" ),
        new AtlasSpec( "Face51", "assets/__swordsmanhd4xstaging/texture2d/face51.png", 1, "Face51_t0" ),
    };

    static readonly AtlasSpec[] UIAtlasSpecs = WithApprovedOmissions( new AtlasSpec[]
    {
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t0.png", 1, "TREASURE_t0" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t1.png", 1, "TREASURE_t1" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t2.png", 1, "TREASURE_t2" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t3.png", 1, "TREASURE_t3" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t4.png", 1, "TREASURE_t4" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t5.png", 1, "TREASURE_t5" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t6.png", 1, "TREASURE_t6" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t7.png", 1, "TREASURE_t7" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t8.png", 1, "TREASURE_t8" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t9.png", 1, "TREASURE_t9" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t10.png", 1, "TREASURE_t10" ),
        new AtlasSpec( "TREASURE", "assets/__swordsmanhd4xstaging/texture2d/treasure_t11.png", 1, "TREASURE_t11" ),
        new AtlasSpec( "camp01", "assets/__swordsmanhd4xstaging/texture2d/camp01.png", 1, "camp01_t4" ),
        new AtlasSpec( "camp02", "assets/__swordsmanhd4xstaging/texture2d/camp02.png", 1, "camp02_t0" ),
        new AtlasSpec( "camp03", "assets/__swordsmanhd4xstaging/texture2d/camp03.png", 1, "camp03_t0" ),
        new AtlasSpec( "camp04", "assets/__swordsmanhd4xstaging/texture2d/camp04.png", 1, "camp04_t0" ),
        new AtlasSpec( "camp05", "assets/__swordsmanhd4xstaging/texture2d/camp05.png", 1, "camp05_t0" ),
        new AtlasSpec( "camp06", "assets/__swordsmanhd4xstaging/texture2d/camp06.png", 1, "camp06_t0" ),
        new AtlasSpec( "camp07", "assets/__swordsmanhd4xstaging/texture2d/camp07.png", 1, "camp07_t0" ),
        new AtlasSpec( "camp09", "assets/__swordsmanhd4xstaging/texture2d/camp09.png", 1, "camp09_t0" ),
        new AtlasSpec( "camp08", "assets/__swordsmanhd4xstaging/texture2d/camp08.png", 1, "camp08_t0" ),
        new AtlasSpec( "camp12", "assets/__swordsmanhd4xstaging/texture2d/camp12.png", 1, "camp12_t0" ),
        new AtlasSpec( "camp14", "assets/__swordsmanhd4xstaging/texture2d/camp14.png", 1, "camp14_t0" ),
        new AtlasSpec( "camp17", "assets/__swordsmanhd4xstaging/texture2d/camp17.png", 1, "camp17_t0" ),
        new AtlasSpec( "camp18", "assets/__swordsmanhd4xstaging/texture2d/camp18.png", 1, "camp18_t0" ),
        new AtlasSpec( "camp20", "assets/__swordsmanhd4xstaging/texture2d/camp20.png", 1, "camp20_t0" ),
        new AtlasSpec( "camp21", "assets/__swordsmanhd4xstaging/texture2d/camp21.png", 1, "camp21_t0" ),
        new AtlasSpec( "Camp10", "assets/__swordsmanhd4xstaging/texture2d/camp10.png", 1, "Camp10_t0" ),
        new AtlasSpec( "Camp13", "assets/__swordsmanhd4xstaging/texture2d/camp13.png", 1, "Camp13_t0" ),
        new AtlasSpec( "camp11", "assets/__swordsmanhd4xstaging/texture2d/camp11.png", 1, "camp11_t0" ),
        new AtlasSpec( "camp15", "assets/__swordsmanhd4xstaging/texture2d/camp15.png", 1, "camp15_t0" ),
        new AtlasSpec( "camp16", "assets/__swordsmanhd4xstaging/texture2d/camp16.png", 1, "camp16_t0" ),
        new AtlasSpec( "camp22", "assets/__swordsmanhd4xstaging/texture2d/camp22.png", 1, "camp22_t0" ),
        new AtlasSpec( "camp19", "assets/__swordsmanhd4xstaging/texture2d/camp19.png", 1, "camp19_t0" ),
        new AtlasSpec( "camp23", "assets/__swordsmanhd4xstaging/texture2d/camp23.png", 1, "camp23_t0" ),
        new AtlasSpec( "camp24", "assets/__swordsmanhd4xstaging/texture2d/camp24.png", 1, "camp24_t0" ),
        new AtlasSpec( "Camp25", "assets/__swordsmanhd4xstaging/texture2d/camp25.png", 1, "Camp25_t0" ),
        new AtlasSpec( "camp26", "assets/__swordsmanhd4xstaging/texture2d/camp26.png", 1, "camp26_t0" ),
        new AtlasSpec( "Camp27", "assets/__swordsmanhd4xstaging/texture2d/camp27.png", 1, "Camp27_t0" ),
        new AtlasSpec( "Camp28", "assets/__swordsmanhd4xstaging/texture2d/camp28.png", 1, "Camp28_t16" ),
        new AtlasSpec( "Camp29", "assets/__swordsmanhd4xstaging/texture2d/camp29.png", 1, "Camp29_t0" ),
        new AtlasSpec( "camp30", "assets/__swordsmanhd4xstaging/texture2d/camp30.png", 1, "camp30_t0" ),
        new AtlasSpec( "camp31", "assets/__swordsmanhd4xstaging/texture2d/camp31.png", 1, "camp31_t0" ),
        new AtlasSpec( "camp32", "assets/__swordsmanhd4xstaging/texture2d/camp32.png", 1, "camp32_t0" ),
        new AtlasSpec( "camp33", "assets/__swordsmanhd4xstaging/texture2d/camp33.png", 1, "camp33_t0" ),
        new AtlasSpec( "Shop06", "assets/__swordsmanhd4xstaging/texture2d/shop06.png", 1, "Shop06_t16" ),
        new AtlasSpec( "Shop07", "assets/__swordsmanhd4xstaging/texture2d/shop07.png", 1, "Shop07_t0" ),
        new AtlasSpec( "Shop08", "assets/__swordsmanhd4xstaging/texture2d/shop08.png", 1, "Shop08_t0" ),
        new AtlasSpec( "Item_win", "assets/__swordsmanhd4xstaging/texture2d/item_win.png", 24, "Item_win_t0|Item_win_t1|Item_win_t2|Item_win_t3|Item_win_t4|Item_win_t5|Item_win_t6|Item_win_t7|Item_win_t8|Item_win_t10|Item_win_t11|Item_win_t12|Item_win_t13|Item_win_t14|Item_win_t15|Item_win_t16|Item_win_t17|Item_win_t19|Item_win_t20|Item_win_t21|Item_win_t22|Item_win_t23|Item_win_t24|Item_win_t25" ),
        new AtlasSpec( "Sqw_int", "assets/__swordsmanhd4xstaging/texture2d/sqw_int.png", 22, "Sqw_int_t0|Sqw_int_t1|Sqw_int_t2|Sqw_int_t3|Sqw_int_t4|Sqw_int_t5|Sqw_int_t6|Sqw_int_t7|Sqw_int_t8|Sqw_int_t9|Sqw_int_t10|Sqw_int_t11|Sqw_int_t12|Sqw_int_t13|Sqw_int_t14|Sqw_int_t15|Sqw_int_t16|Sqw_int_t17|Sqw_int_t18|Sqw_int_t19|Sqw_int_t20|Sqw_int_t21" ),
        new AtlasSpec( "selection", "assets/__swordsmanhd4xstaging/texture2d/selection.png", 1, "selection" ),
        new AtlasSpec( "Shop00", "assets/__swordsmanhd4xstaging/texture2d/shop00.png", 1, "Shop00_t2" ),
        new AtlasSpec( "shop01", "assets/__swordsmanhd4xstaging/texture2d/shop01.png", 1, "shop01_t0" ),
        new AtlasSpec( "shop02", "assets/__swordsmanhd4xstaging/texture2d/shop02.png", 1, "shop02_t7" ),
        new AtlasSpec( "Shop03", "assets/__swordsmanhd4xstaging/texture2d/shop03.png", 1, "Shop03_t0" ),
        new AtlasSpec( "Shop04", "assets/__swordsmanhd4xstaging/texture2d/shop04.png", 1, "Shop04_t0" ),
        new AtlasSpec( "Shop05", "assets/__swordsmanhd4xstaging/texture2d/shop05.png", 1, "Shop05_t0" ),
        new AtlasSpec( "shop09", "assets/__swordsmanhd4xstaging/texture2d/shop09.png", 1, "shop09_t0" ),
        new AtlasSpec( "Shop10", "assets/__swordsmanhd4xstaging/texture2d/shop10.png", 1, "Shop10_t0" ),
        new AtlasSpec( "Shop11", "assets/__swordsmanhd4xstaging/texture2d/shop11.png", 1, "Shop11_t0" ),
        new AtlasSpec( "Sys_int", "assets/__swordsmanhd4xstaging/texture2d/sys_int.png", 39, "Sys_int_t0|Sys_int_t1|Sys_int_t10|Sys_int_t11|Sys_int_t12|Sys_int_t13|Sys_int_t14|Sys_int_t15|Sys_int_t16|Sys_int_t17|Sys_int_t18|Sys_int_t19|Sys_int_t2|Sys_int_t20|Sys_int_t21|Sys_int_t22|Sys_int_t23|Sys_int_t24|Sys_int_t25|Sys_int_t26|Sys_int_t27|Sys_int_t28|Sys_int_t29|Sys_int_t3|Sys_int_t30|Sys_int_t31|Sys_int_t32|Sys_int_t33|Sys_int_t34|Sys_int_t35|Sys_int_t36|Sys_int_t37|Sys_int_t38|Sys_int_t4|Sys_int_t5|Sys_int_t6|Sys_int_t7|Sys_int_t8|Sys_int_t9" ),
        new AtlasSpec( "EANM_01_20", "assets/__swordsmanhd4xstaging/texture2d/eanm_01_20.png", 1, "EANM_01_t0" ),
        new AtlasSpec( "EANM_02_14", "assets/__swordsmanhd4xstaging/texture2d/eanm_02_14.png", 1, "EANM_02_t0" ),
        new AtlasSpec( "Eanm_01_3", "assets/__swordsmanhd4xstaging/texture2d/eanm_01_3.png", 1, "Eanm_01_t0" ),
        new AtlasSpec( "Eanm_02_7", "assets/__swordsmanhd4xstaging/texture2d/eanm_02_7.png", 1, "Eanm_02_t0" ),
        new AtlasSpec( "Eanm_03_18", "assets/__swordsmanhd4xstaging/texture2d/eanm_03_18.png", 1, "Eanm_03_t0" ),
        new AtlasSpec( "Eanm_04_0", "assets/__swordsmanhd4xstaging/texture2d/eanm_04_0.png", 1, "Eanm_04_t0" ),
        new AtlasSpec( "Eanm_07_4", "assets/__swordsmanhd4xstaging/texture2d/eanm_07_4.png", 1, "Eanm_07_t0" ),
        new AtlasSpec( "Eanm_12_6", "assets/__swordsmanhd4xstaging/texture2d/eanm_12_6.png", 1, "Eanm_12_t0" ),
        new AtlasSpec( "Eanm_14_2", "assets/__swordsmanhd4xstaging/texture2d/eanm_14_2.png", 1, "Eanm_14_t0" ),
        new AtlasSpec( "HelpWindow", "assets/__swordsmanhd4xstaging/texture2d/helpwindow.png", 1, "HelpWindow_0" ),
        new AtlasSpec( "Cha_tur", "assets/__swordsmanhd4xstaging/texture2d/cha_tur.png", 54 ),
        new AtlasSpec( "MSG_BOX", "assets/__swordsmanhd4xstaging/texture2d/msg_box.png", 40, "MSG_BOX_t0|MSG_BOX_t1|MSG_BOX_t10|MSG_BOX_t11|MSG_BOX_t12|MSG_BOX_t13|MSG_BOX_t14|MSG_BOX_t15|MSG_BOX_t16|MSG_BOX_t17|MSG_BOX_t18|MSG_BOX_t19|MSG_BOX_t2|MSG_BOX_t20|MSG_BOX_t21|MSG_BOX_t22|MSG_BOX_t23|MSG_BOX_t24|MSG_BOX_t25|MSG_BOX_t26|MSG_BOX_t27|MSG_BOX_t28|MSG_BOX_t29|MSG_BOX_t3|MSG_BOX_t30|MSG_BOX_t31|MSG_BOX_t32|MSG_BOX_t33|MSG_BOX_t34|MSG_BOX_t35|MSG_BOX_t36|MSG_BOX_t37|MSG_BOX_t38|MSG_BOX_t39|MSG_BOX_t4|MSG_BOX_t5|MSG_BOX_t6|MSG_BOX_t7|MSG_BOX_t8|MSG_BOX_t9" ),
        new AtlasSpec( "Title", "assets/__swordsmanhd4xstaging/texture2d/title.png", 2, "Title_t0|Title_t1" ),
        new AtlasSpec( "CAMP_MEN", "assets/__swordsmanhd4xstaging/texture2d/camp_men.png", 3, "CAMP_MEN_t0|CAMP_MEN_t1|CAMP_MEN_t2" ),
        new AtlasSpec( "EE_PRO", "assets/__swordsmanhd4xstaging/texture2d/ee_pro.png", 4, "EE_PRO_t0|EE_PRO_t1|EE_PRO_t2|EE_PRO_t3" ),
        new AtlasSpec( "FI_PRO", "assets/__swordsmanhd4xstaging/texture2d/fi_pro.png", 19, "FI_PRO_t0|FI_PRO_t1|FI_PRO_t10|FI_PRO_t11|FI_PRO_t12|FI_PRO_t13|FI_PRO_t14|FI_PRO_t15|FI_PRO_t16|FI_PRO_t17|FI_PRO_t18|FI_PRO_t2|FI_PRO_t3|FI_PRO_t4|FI_PRO_t5|FI_PRO_t6|FI_PRO_t7|FI_PRO_t8|FI_PRO_t9" ),
        new AtlasSpec( "FLOAT_MB", "assets/__swordsmanhd4xstaging/texture2d/float_mb.png", 1, "FLOAT_MB_t0" ),
        new AtlasSpec( "Fig_misc", "assets/__swordsmanhd4xstaging/texture2d/fig_misc.png", 4, "Fig_misc_t1|Fig_misc_t2|Fig_misc_t4|Fig_misc_t5" ),
        new AtlasSpec( "Fig_misc", "assets/__swordsmanhd4xstaging/texture2d/attackinfo/fig_misc.png", 2, "Fig_misc_t0|Fig_misc_t3" ),
        new AtlasSpec( "ITEM_MB", "assets/__swordsmanhd4xstaging/texture2d/item_mb.png", 1, "ITEM_MB_t0" ),
        new AtlasSpec( "ITEM_MEN", "assets/__swordsmanhd4xstaging/texture2d/item_men.png", 3, "ITEM_MEN_t0|ITEM_MEN_t1|ITEM_MEN_t2" ),
        new AtlasSpec( "ITEM_PRO", "assets/__swordsmanhd4xstaging/texture2d/item_pro.png", 2, "ITEM_PRO_t0|ITEM_PRO_t1" ),
        new AtlasSpec( "MAP_VIE", "assets/__swordsmanhd4xstaging/texture2d/map_vie.png", 53, "MAP_VIE_t0|MAP_VIE_t1|MAP_VIE_t10|MAP_VIE_t11|MAP_VIE_t12|MAP_VIE_t13|MAP_VIE_t14|MAP_VIE_t15|MAP_VIE_t16|MAP_VIE_t17|MAP_VIE_t18|MAP_VIE_t19|MAP_VIE_t2|MAP_VIE_t20|MAP_VIE_t21|MAP_VIE_t22|MAP_VIE_t23|MAP_VIE_t24|MAP_VIE_t25|MAP_VIE_t26|MAP_VIE_t27|MAP_VIE_t28|MAP_VIE_t29|MAP_VIE_t3|MAP_VIE_t30|MAP_VIE_t31|MAP_VIE_t32|MAP_VIE_t33|MAP_VIE_t34|MAP_VIE_t35|MAP_VIE_t36|MAP_VIE_t37|MAP_VIE_t38|MAP_VIE_t39|MAP_VIE_t4|MAP_VIE_t40|MAP_VIE_t41|MAP_VIE_t42|MAP_VIE_t43|MAP_VIE_t44|MAP_VIE_t45|MAP_VIE_t46|MAP_VIE_t47|MAP_VIE_t48|MAP_VIE_t49|MAP_VIE_t5|MAP_VIE_t50|MAP_VIE_t51|MAP_VIE_t52|MAP_VIE_t6|MAP_VIE_t7|MAP_VIE_t8|MAP_VIE_t9" ),
        new AtlasSpec( "MIS_BRI", "assets/__swordsmanhd4xstaging/texture2d/mis_bri.png", 2, "MIS_BRI_t0|MIS_BRI_t1" ),
        new AtlasSpec( "Man_bri", "assets/__swordsmanhd4xstaging/texture2d/man_bri.png", 7, "Man_bri_t0|Man_bri_t1|Man_bri_t2|Man_bri_t3|Man_bri_t4|Man_bri_t5|Man_bri_t6" ),
        new AtlasSpec( "Man_int", "assets/__swordsmanhd4xstaging/texture2d/man_int.png", 45, "Man_int_t0|Man_int_t1|Man_int_t10|Man_int_t11|Man_int_t12|Man_int_t13|Man_int_t14|Man_int_t15|Man_int_t16|Man_int_t17|Man_int_t18|Man_int_t19|Man_int_t2|Man_int_t20|Man_int_t21|Man_int_t22|Man_int_t23|Man_int_t24|Man_int_t25|Man_int_t26|Man_int_t27|Man_int_t28|Man_int_t29|Man_int_t3|Man_int_t30|Man_int_t31|Man_int_t32|Man_int_t33|Man_int_t34|Man_int_t35|Man_int_t36|Man_int_t37|Man_int_t38|Man_int_t39|Man_int_t4|Man_int_t40|Man_int_t41|Man_int_t42|Man_int_t43|Man_int_t44|Man_int_t5|Man_int_t6|Man_int_t7|Man_int_t8|Man_int_t9" ),
        new AtlasSpec( "Man_pro", "assets/__swordsmanhd4xstaging/texture2d/man_pro.png", 44, "Man_pro_t0|Man_pro_t1|Man_pro_t10|Man_pro_t11|Man_pro_t12|Man_pro_t13|Man_pro_t14|Man_pro_t15|Man_pro_t16|Man_pro_t17|Man_pro_t18|Man_pro_t19|Man_pro_t2|Man_pro_t20|Man_pro_t21|Man_pro_t22|Man_pro_t23|Man_pro_t24|Man_pro_t25|Man_pro_t26|Man_pro_t27|Man_pro_t28|Man_pro_t29|Man_pro_t3|Man_pro_t30|Man_pro_t31|Man_pro_t32|Man_pro_t33|Man_pro_t34|Man_pro_t35|Man_pro_t36|Man_pro_t37|Man_pro_t38|Man_pro_t39|Man_pro_t4|Man_pro_t40|Man_pro_t41|Man_pro_t42|Man_pro_t43|Man_pro_t5|Man_pro_t6|Man_pro_t7|Man_pro_t8|Man_pro_t9" ),
        new AtlasSpec( "Maplup", "assets/__swordsmanhd4xstaging/texture2d/maplup.png", 13, "Maplup_t0|Maplup_t1|Maplup_t10|Maplup_t11|Maplup_t12|Maplup_t2|Maplup_t3|Maplup_t4|Maplup_t5|Maplup_t6|Maplup_t7|Maplup_t8|Maplup_t9" ),
        new AtlasSpec( "SP_REV", "assets/__swordsmanhd4xstaging/texture2d/sp_rev.png", 28, "SP_REV_t0|SP_REV_t1|SP_REV_t10|SP_REV_t11|SP_REV_t12|SP_REV_t13|SP_REV_t14|SP_REV_t15|SP_REV_t16|SP_REV_t17|SP_REV_t18|SP_REV_t19|SP_REV_t2|SP_REV_t20|SP_REV_t21|SP_REV_t22|SP_REV_t23|SP_REV_t24|SP_REV_t25|SP_REV_t26|SP_REV_t27|SP_REV_t3|SP_REV_t4|SP_REV_t5|SP_REV_t6|SP_REV_t7|SP_REV_t8|SP_REV_t9" ),
        new AtlasSpec( "Sfman", "assets/__swordsmanhd4xstaging/texture2d/sfman.png", 5, "Sfman_t0|Sfman_t1|Sfman_t2|Sfman_t3|Sfman_t4" ),
    });

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
    static void RegisterSceneInterfaceReplacement()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += ReplaceSceneInterfaceSprites;
    }

    static void ReplaceSceneInterfaceSprites( UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode )
    {
        foreach ( GameObject root in scene.GetRootGameObjects() )
        {
            foreach ( UnityEngine.UI.Image image in root.GetComponentsInChildren<UnityEngine.UI.Image>( true ) )
            {
                AtlasSpec spec;
                if ( TryGetUIAtlasSpec( image.sprite, out spec ) )
                    image.sprite = ResolveUI( image.sprite, spec );
            }
        }
    }

    static AssetBundle bundle;
    static Stream hdBundleStream;
    static Dictionary<string, Sprite> singleSprites =
        new Dictionary<string, Sprite>( StringComparer.OrdinalIgnoreCase );
    static Dictionary<string, Sprite> portraitSprites;
    static HashSet<string> singleLoadAttempts =
        new HashSet<string>( StringComparer.OrdinalIgnoreCase );
    static HashSet<string> activationLogs =
        new HashSet<string>( StringComparer.Ordinal );
    static bool bundleLoadAttempted;
    static bool portraitLoadAttempted;
    static string bundleFailure = "HD AssetBundle is unavailable.";
    static Dictionary<string, Sprite> uiSprites = new Dictionary<string, Sprite>( StringComparer.OrdinalIgnoreCase );
    static HashSet<Sprite> loadedUISprites = new HashSet<Sprite>();
    static HashSet<string> uiLoadAttempts = new HashSet<string>( StringComparer.OrdinalIgnoreCase );

    public static Sprite Resolve( Sprite legacySprite )
    {
        if ( legacySprite != null && resolvedReferences.Contains( legacySprite ) ) return legacySprite;
        AtlasSpec uiSpec;
        if ( TryGetUIAtlasSpec( legacySprite, out uiSpec ) )
        {
            return ResolveUI( legacySprite, uiSpec );
        }
        AtlasSpec spec;
        if ( !TryGetPortraitAtlasSpec( legacySprite, out spec ) )
        {
            return legacySprite;
        }

        EnsurePortraitAtlasesLoaded();
        Sprite hdSprite;
        if ( portraitSprites != null &&
            portraitSprites.TryGetValue(
                GetAtlasSpriteKey( spec.textureName, legacySprite.name ), out hdSprite ) &&
            HasMatchingWorldGeometry( legacySprite, hdSprite ) )
        {
            LogActivationOnce( "Dialogue portraits" );
            return hdSprite;
        }

        throw RequiredResourceFailure( "Portrait Sprite is missing or its geometry differs: " + legacySprite.name );
    }

    public static Sprite Resolve( Sprite legacySprite, string resourcePath )
    {
        if ( legacySprite != null && resolvedReferences.Contains( legacySprite ) ) return legacySprite;
        SingleSpec spec;
        if ( string.IsNullOrEmpty( resourcePath ) || !TryGetSingleSpec( resourcePath, out spec ) )
        {
            return Resolve( legacySprite );
        }
        if ( legacySprite == null || legacySprite.texture == null )
        {
            throw RequiredResourceFailure( "Required source geometry is missing: " + resourcePath );
        }
        // Original atlases/actors can share a path suffix with an approved single.
        // They remain native resources unless both Sprite and texture identity match.
        string name = Path.GetFileName( spec.resourcePath );
        if ( !string.Equals( legacySprite.name, name, StringComparison.OrdinalIgnoreCase ) ||
             !string.Equals( legacySprite.texture.name, name, StringComparison.OrdinalIgnoreCase ) )
            return Resolve( legacySprite );
        Sprite hdSprite = LoadSingleSprite( spec, legacySprite );
        LogActivationOnce( spec.group );
        return hdSprite;
    }

    public static float GetUIScale( Sprite sprite, float legacyScale )
    {
        if ( ( IsPortraitTextureName( sprite ) || IsApprovedUISprite( sprite ) || IsApprovedFixedTerrain( sprite ) ) &&
            sprite.pixelsPerUnit > 0.0f )
        {
            return 1.0f / sprite.pixelsPerUnit;
        }

        return legacyScale;
    }

    static bool IsApprovedFixedTerrain( Sprite sprite )
    {
        if ( sprite == null || sprite.texture == null ) return false;
        for ( int i = 0; i < SingleSpecs.Length; i++ )
        {
            SingleSpec spec = SingleSpecs[i];
            if ( spec.group != "Fixed attack terrain" ) continue;
            string name = Path.GetFileName( spec.resourcePath );
            if ( string.Equals( sprite.name, name, StringComparison.OrdinalIgnoreCase ) &&
                string.Equals( sprite.texture.name, name, StringComparison.OrdinalIgnoreCase ) ) return true;
        }
        return false;
    }

    static bool IsApprovedUISprite( Sprite sprite )
    {
        AtlasSpec spec;
        return TryGetUIAtlasSpec( sprite, out spec ) || ( sprite != null && loadedUISprites.Contains( sprite ) );
    }

    static bool IsPortraitTextureName( Sprite sprite )
    {
        if ( sprite == null || sprite.texture == null )
        {
            return false;
        }

        for ( int i = 0; i < PortraitAtlasSpecs.Length; i++ )
        {
            if ( string.Equals(
                sprite.texture.name,
                PortraitAtlasSpecs[ i ].textureName,
                StringComparison.OrdinalIgnoreCase ) &&
                IsAllowedAtlasSlice( PortraitAtlasSpecs[ i ], sprite.name ) )
            {
                return true;
            }
        }
        return false;
    }

    static void EnsureBundleLoaded()
    {
        if ( bundle != null ) return;
        if ( bundleLoadAttempted )
        {
            throw RequiredResourceFailure( bundleFailure );
        }

        bundleLoadAttempted = true;
        string bundlePath = Path.Combine(
            Application.streamingAssetsPath, BundleFolder, BundleName );

#if UNITY_EDITOR
        if ( !File.Exists( bundlePath ) )
        {
            bundlePath = Path.GetFullPath( Path.Combine(
                Application.dataPath,
                "../Builds/HD4X/static-mvp/unity-bundles/macos/" + BundleName ) );
        }
#endif

        if ( !File.Exists( bundlePath ) )
        {
            bundleFailure = "Required HD AssetBundle is missing: " + bundlePath;
            throw RequiredResourceFailure( bundleFailure );
        }

        // The player ships a seekable LZFSE container; Editor source Bundles stay UnityFS.
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        using ( var header = File.OpenRead( bundlePath ) )
        {
            byte[] magic = new byte[8];
            if ( header.Read( magic, 0, magic.Length ) == magic.Length &&
                System.Text.Encoding.ASCII.GetString( magic ) == "SWLZF001" )
            {
                hdBundleStream = new SwordsmanLzfseStream( bundlePath, 16 * 1024 * 1024, true, 4, false );
                bundle = AssetBundle.LoadFromStream( hdBundleStream, 0, 1024 * 1024 );
            }
            else bundle = AssetBundle.LoadFromFile( bundlePath );
        }
#else
        bundle = AssetBundle.LoadFromFile( bundlePath );
#endif
        if ( bundle != null && !HasOnlyStaticAssets( bundle ) )
        {
            bundle.Unload( true );
            bundle = null;
            if ( hdBundleStream != null ) { hdBundleStream.Dispose(); hdBundleStream = null; }
            bundleFailure = "HD AssetBundle contains missing or unapproved assets: " + bundlePath;
            throw RequiredResourceFailure( bundleFailure );
        }
        if ( bundle == null )
        {
            if ( hdBundleStream != null ) { hdBundleStream.Dispose(); hdBundleStream = null; }
            bundleFailure = "Required HD AssetBundle cannot be loaded: " + bundlePath;
            throw RequiredResourceFailure( bundleFailure );
        }
    }

    static bool HasOnlyStaticAssets( AssetBundle candidate )
    {
        HashSet<string> expected = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
        for ( int i = 0; i < SingleSpecs.Length; i++ ) expected.Add( SingleSpecs[ i ].bundlePath );
        for ( int i = 0; i < PortraitAtlasSpecs.Length; i++ ) expected.Add( PortraitAtlasSpecs[ i ].bundlePath );
        string[] assets = candidate.GetAllAssetNames();
        for ( int i = 0; i < UIAtlasSpecs.Length; i++ ) expected.Add( UIAtlasSpecs[ i ].bundlePath );
        if ( assets.Length != expected.Count || expected.Count != 464 + ApprovedOmissionCount ) return false;
        for ( int i = 0; i < assets.Length; i++ )
        {
            if ( !expected.Contains( assets[ i ] ) ) return false;
        }
        return true;
    }

    static bool TryGetUIAtlasSpec( Sprite sprite, out AtlasSpec spec )
    {
        if ( sprite != null && sprite.texture != null )
        {
            for ( int i = 0; i < UIAtlasSpecs.Length; i++ )
            {
                if ( string.Equals( sprite.texture.name, UIAtlasSpecs[ i ].textureName, StringComparison.OrdinalIgnoreCase ) &&
                    IsAllowedAtlasSlice( UIAtlasSpecs[ i ], sprite.name ) )
                {
                    spec = UIAtlasSpecs[ i ];
                    return true;
                }
            }
        }
        spec = default( AtlasSpec );
        return false;
    }

    static Sprite ResolveUI( Sprite legacySprite, AtlasSpec spec )
    {
        if ( uiLoadAttempts.Add( spec.bundlePath ) )
        {
            EnsureBundleLoaded();
            if ( bundle != null )
            {
                Sprite[] sprites = bundle.LoadAssetWithSubAssets<Sprite>( spec.bundlePath );
                var names = new HashSet<string>( StringComparer.Ordinal );
                if ( sprites.Length == spec.expectedSpriteCount &&
                    Array.TrueForAll( sprites, sprite => names.Add( sprite.name ) && IsAllowedAtlasSlice( spec, sprite.name ) ) )
                {
                    foreach ( Sprite sprite in sprites )
                    {
                        uiSprites.Add( GetAtlasSpriteKey( spec.textureName, sprite.name ), sprite );
                        loadedUISprites.Add( sprite );
                    }
                }
            }
        }
        Sprite candidate;
        uiSprites.TryGetValue( GetAtlasSpriteKey( spec.textureName, legacySprite.name ), out candidate );
        // Already-HD views (including the expanded title viewport) keep their own crop.
        if ( candidate != null && legacySprite.texture == candidate.texture )
        {
            loadedUISprites.Add( legacySprite );
            return legacySprite;
        }
        if ( candidate != null && HasMatchingWorldGeometry( legacySprite, candidate ) )
        {
            LogActivationOnce( "Static atlas " + spec.textureName );
            return candidate;
        }
        throw RequiredResourceFailure( "UI Sprite is missing or its geometry differs: " + spec.textureName + "/" + legacySprite.name );
    }

    static void EnsurePortraitAtlasesLoaded()
    {
        if ( portraitLoadAttempted )
        {
            if ( portraitSprites == null ) throw RequiredResourceFailure( "Portrait atlas initialization did not complete." );
            return;
        }

        portraitLoadAttempted = true;
        EnsureBundleLoaded();
        Dictionary<string, Sprite> candidates =
            new Dictionary<string, Sprite>( StringComparer.OrdinalIgnoreCase );
        for ( int i = 0; i < PortraitAtlasSpecs.Length; i++ )
        {
            AtlasSpec spec = PortraitAtlasSpecs[ i ];
            Sprite[] loaded = bundle.LoadAssetWithSubAssets<Sprite>( spec.bundlePath );
            if ( loaded == null || loaded.Length != spec.expectedSpriteCount )
            {
                throw RequiredResourceFailure( "Portrait atlas is incomplete: " + spec.textureName );
            }
            for ( int j = 0; j < loaded.Length; j++ )
            {
                Sprite sprite = loaded[ j ];
                if ( !IsAllowedAtlasSlice( spec, sprite.name ) )
                {
                    throw RequiredResourceFailure( "Portrait atlas contains an unapproved Sprite: " + sprite.name );
                }
                string key = GetAtlasSpriteKey( spec.textureName, sprite.name );
                if ( candidates.ContainsKey( key ) ) throw RequiredResourceFailure( "Portrait atlas contains a duplicate Sprite: " + key );
                candidates.Add( key, sprite );
            }
        }

        portraitSprites = candidates;
    }

    static bool TryGetPortraitAtlasSpec( Sprite sprite, out AtlasSpec spec )
    {
        if ( sprite != null && sprite.texture != null )
        {
            for ( int i = 0; i < PortraitAtlasSpecs.Length; i++ )
            {
                if ( string.Equals(
                    sprite.texture.name,
                    PortraitAtlasSpecs[ i ].textureName,
                    StringComparison.OrdinalIgnoreCase ) &&
                    IsAllowedAtlasSlice( PortraitAtlasSpecs[ i ], sprite.name ) )
                {
                    spec = PortraitAtlasSpecs[ i ];
                    return true;
                }
            }
        }

        spec = default( AtlasSpec );
        return false;
    }

    static bool IsAllowedAtlasSlice( AtlasSpec spec, string spriteName )
    {
        return spec.allowedSpriteNames == null ||
            Array.IndexOf( spec.allowedSpriteNames.Split( '|' ), spriteName ) >= 0;
    }

    static string GetAtlasSpriteKey( string textureName, string spriteName )
    {
        return textureName + "/" + spriteName;
    }

    static Sprite LoadSingleSprite( SingleSpec spec, Sprite legacySprite )
    {
        Sprite cached;
        if ( singleSprites.TryGetValue( spec.resourcePath, out cached ) )
        {
            if ( !HasMatchingWorldGeometry( legacySprite, cached ) )
                throw RequiredResourceFailure( "Single Sprite geometry differs: " + spec.resourcePath );
            return cached;
        }

        if ( singleLoadAttempts.Contains( spec.resourcePath ) )
        {
            throw RequiredResourceFailure( "Single Sprite initialization did not complete: " + spec.resourcePath );
        }
        singleLoadAttempts.Add( spec.resourcePath );

        EnsureBundleLoaded();
        Sprite candidate = LoadOnlySprite( bundle, spec.bundlePath );
        if ( candidate != null && HasMatchingWorldGeometry( legacySprite, candidate ) )
        {
            // A legacy Sprite name can differ in case from its texture filename (stage12).
            candidate.name = legacySprite.name;
            singleSprites[ spec.resourcePath ] = candidate;
            return candidate;
        }

        throw RequiredResourceFailure( "Single Sprite is missing or its geometry differs: " + spec.resourcePath );
    }

    static Sprite LoadOnlySprite( AssetBundle targetBundle, string bundlePath )
    {
        Sprite[] sprites = targetBundle.LoadAssetWithSubAssets<Sprite>( bundlePath );
        return sprites != null && sprites.Length == 1 ? sprites[ 0 ] : null;
    }

    static bool TryGetSingleSpec( string resourcePath, out SingleSpec spec )
    {
        for ( int i = 0; i < SingleSpecs.Length; i++ )
        {
            if ( string.Equals(
                resourcePath, SingleSpecs[ i ].resourcePath, StringComparison.OrdinalIgnoreCase ) )
            {
                spec = SingleSpecs[ i ];
                return true;
            }
        }

        spec = default( SingleSpec );
        return false;
    }

    static bool HasMatchingWorldGeometry( Sprite legacySprite, Sprite hdSprite )
    {
        if ( legacySprite == null ||
            hdSprite == null ||
            legacySprite.pixelsPerUnit <= 0.0f ||
            hdSprite.pixelsPerUnit <= 0.0f )
        {
            return false;
        }

        const float epsilon = 0.001f;
        float legacyWidth = legacySprite.rect.width / legacySprite.pixelsPerUnit;
        float legacyHeight = legacySprite.rect.height / legacySprite.pixelsPerUnit;
        float hdWidth = hdSprite.rect.width / hdSprite.pixelsPerUnit;
        float hdHeight = hdSprite.rect.height / hdSprite.pixelsPerUnit;

        Vector2 legacyPivot = new Vector2(
            legacySprite.pivot.x / legacySprite.rect.width,
            legacySprite.pivot.y / legacySprite.rect.height );
        Vector2 hdPivot = new Vector2(
            hdSprite.pivot.x / hdSprite.rect.width,
            hdSprite.pivot.y / hdSprite.rect.height );

        return Mathf.Abs( legacyWidth - hdWidth ) <= epsilon &&
            Mathf.Abs( legacyHeight - hdHeight ) <= epsilon &&
            Vector2.Distance( legacyPivot, hdPivot ) <= epsilon &&
            Vector4.Distance( legacySprite.border / legacySprite.pixelsPerUnit,
                hdSprite.border / hdSprite.pixelsPerUnit ) <= epsilon;
    }

#if DEVELOPMENT_BUILD
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void RunDevelopmentSmokeCheck()
    {
        string[] args = Environment.GetCommandLineArgs();
        if ( Array.IndexOf( args, "-swordsman-static-mvp-check" ) < 0 ) return;
        try
        {
            CheckStaticProbe( "Tanm_00_t0", "Tanm_00_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage00/Tanm_00_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map00-1", "map00-1", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage00/map00-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map00-2", "map00-2", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage00/map00-2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map00-3", "map00-3", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage00/map00-3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map00", "map00", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage00/map00", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Tanm_01_t0", "Tanm_01_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage01/Tanm_01_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map01", "map01", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage01/map01", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_02_t0", "TANM_02_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage02/TANM_02_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map02", "map02", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage02/map02", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_03_t0", "TANM_03_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage03/TANM_03_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map03", "map03", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage03/map03", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_04_t0", "TANM_04_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage04/TANM_04_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map04-1", "map04-1", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage04/map04-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map04", "map04", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage04/map04", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_05_t0", "TANM_05_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage05/TANM_05_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map05", "map05", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage05/map05", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map06-1", "Map06-1", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage06/Map06-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_06_t0", "TANM_06_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage06/TANM_06_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map06", "map06", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage06/map06", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_07_t0", "TANM_07_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage07/TANM_07_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map07", "map07", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage07/map07", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map08", "Map08", 4096, 2048, new Rect( 0f, 0f, 4096f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage08/Map08", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_08_t0", "TANM_08_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage08/TANM_08_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "MAP09", "MAP09", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage09/MAP09", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_09_t0", "TANM_09_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage09/TANM_09_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "MAP10", "MAP10", 4096, 1024, new Rect( 0f, 0f, 4096f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage10/MAP10", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map10-1", "Map10-1", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage10/Map10-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_10_t0", "TANM_10_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage10/TANM_10_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map11-1", "Map11-1", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage11/Map11-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map11", "Map11", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage11/Map11", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_11_t0", "TANM_11_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage11/TANM_11_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map12", "map12", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage12/Map12", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_12_t0", "TANM_12_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage12/TANM_12_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map13-1", "map13-1", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage13/Map13-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map13-2", "Map13-2", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage13/Map13-2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map13", "map13", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage13/Map13", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_13_t0", "TANM_13_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage13/TANM_13_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map14", "map14", 1024, 4096, new Rect( 0f, 0f, 1024f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage14/Map14", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_14_t0", "TANM_14_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage14/TANM_14_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map15", "map15", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage15/Map15", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_15_t0", "TANM_15_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage15/TANM_15_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map16", "map16", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage16/Map16", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_16_t0", "TANM_16_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage16/TANM_16_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map17", "map17", 1211, 4096, new Rect( 0f, 0f, 1211f, 4096f ), new Vector2( 0f, 1f ), 1.8348485f, "texture/map/stage17/Map17", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_17_t0", "TANM_17_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage17/TANM_17_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map18-1", "map18-1", 1024, 2048, new Rect( 0f, 0f, 1024f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage18/Map18-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map18", "map18", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage18/Map18", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_18_t0", "TANM_18_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage18/TANM_18_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map19", "map19", 4096, 1024, new Rect( 0f, 0f, 4096f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage19/Map19", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_19_t0", "TANM_19_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage19/TANM_19_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map20", "map20", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage20/Map20", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_20_t0", "TANM_20_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage20/TANM_20_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map20-1", "map20-1", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage20/map20-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map20-2", "map20-2", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage20/map20-2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map21", "map21", 4096, 4096, new Rect( 0f, 0f, 4096f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage21/Map21", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_21_t0", "TANM_21_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage21/TANM_21_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map22-1", "map22-1", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage22/Map22-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map22", "map22", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage22/Map22", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_22_t0", "TANM_22_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage22/TANM_22_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map23", "map23", 4096, 2048, new Rect( 0f, 0f, 4096f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage23/Map23", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_23_t0", "TANM_23_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage23/TANM_23_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map23-1", "map23-1", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage23/map23-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map23-2", "map23-2", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage23/map23-2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map24", "map24", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage24/Map24", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_24_t0", "TANM_24_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage24/TANM_24_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map24-1", "map24-1", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage24/map24-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map25-1", "map25-1", 1024, 2048, new Rect( 0f, 0f, 1024f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage25/Map25-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map25", "map25", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage25/Map25", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_25_t0", "TANM_25_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage25/TANM_25_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map26-1", "map26-1", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage26/Map26-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map26", "map26", 4096, 2048, new Rect( 0f, 0f, 4096f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage26/Map26", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_26_t0", "TANM_26_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage26/TANM_26_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map27", "map27", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage27/Map27", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_27_t0", "TANM_27_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage27/TANM_27_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map28-1", "map28-1", 1024, 2048, new Rect( 0f, 0f, 1024f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage28/Map28-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map28", "map28", 4096, 4096, new Rect( 0f, 0f, 4096f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage28/Map28", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_28_t0", "TANM_28_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage28/TANM_28_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map28-2", "map28-2", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage28/map28-2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map29", "map29", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage29/Map29", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_29_t0", "TANM_29_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage29/TANM_29_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map29-1", "map29-1", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage29/map29-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map30", "map30", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage30/Map30", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_30_t0", "TANM_30_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage30/TANM_30_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map31-1", "map31-1", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage31/Map31-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map31", "Map31", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage31/Map31", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_31_t0", "TANM_31_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage31/TANM_31_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map31-2", "map31-2", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage31/map31-2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map32", "Map32", 4096, 2048, new Rect( 0f, 0f, 4096f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage32/Map32", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_32_t0", "TANM_32_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage32/TANM_32_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map33", "Map33", 4096, 2048, new Rect( 0f, 0f, 4096f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage33/Map33", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_33_t0", "TANM_33_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage33/TANM_33_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map34-3", "map34-3", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage34/Map34-3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map34", "map34", 4096, 4096, new Rect( 0f, 0f, 4096f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage34/Map34", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_34_t0", "TANM_34_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage34/TANM_34_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map34-1", "map34-1", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage34/map34-1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map34-2", "map34-2", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage34/map34-2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map34-4", "map34-4", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage34/map34-4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Map35", "map35", 4096, 4096, new Rect( 0f, 0f, 4096f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage35/Map35", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "TANM_35_t0", "TANM_35_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/map/stage35/TANM_35_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map00", "map00", 2048, 4096, new Rect( 0f, 0f, 2048f, 4096f ), new Vector2( 0f, 1f ), 2f, "texture/rpg/map00", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map02", "map02", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/rpg/map02", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map05", "map05", 4096, 1024, new Rect( 0f, 0f, 4096f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/rpg/map05", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map06", "map06", 1024, 1024, new Rect( 0f, 0f, 1024f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/rpg/map06", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map11", "map11", 2048, 2048, new Rect( 0f, 0f, 2048f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/rpg/map11", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map12", "map12", 2048, 512, new Rect( 0f, 0f, 2048f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/rpg/map12", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map17", "map17", 4096, 2048, new Rect( 0f, 0f, 4096f, 2048f ), new Vector2( 0f, 1f ), 2f, "texture/rpg/map17", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map18", "map18", 2048, 1024, new Rect( 0f, 0f, 2048f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/rpg/map18", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "map26", "map26", 4096, 1024, new Rect( 0f, 0f, 4096f, 1024f ), new Vector2( 0f, 1f ), 2f, "texture/rpg/map26", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_00_t0", "FLOOR_00_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage00/floor_00/FLOOR_00_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_00_t1", "FLOOR_00_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage00/floor_00/FLOOR_00_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_00_t2", "FLOOR_00_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage00/floor_00/FLOOR_00_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_00_t3", "FLOOR_00_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage00/floor_00/FLOOR_00_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_00_t4", "FLOOR_00_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage00/floor_00/FLOOR_00_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_00_t5", "FLOOR_00_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage00/floor_00/FLOOR_00_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_01_t0", "Floor_01_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage01/floor_01/Floor_01_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_01_t1", "Floor_01_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage01/floor_01/Floor_01_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_01_t2", "Floor_01_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage01/floor_01/Floor_01_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_01_t3", "Floor_01_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage01/floor_01/Floor_01_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_01_t4", "Floor_01_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage01/floor_01/Floor_01_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_01_t5", "Floor_01_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage01/floor_01/Floor_01_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_02_t0", "Floor_02_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage02/floor_02/Floor_02_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_02_t1", "Floor_02_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage02/floor_02/Floor_02_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_02_t2", "Floor_02_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage02/floor_02/Floor_02_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_02_t3", "Floor_02_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage02/floor_02/Floor_02_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_02_t4", "Floor_02_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage02/floor_02/Floor_02_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_02_t5", "Floor_02_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage02/floor_02/Floor_02_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_03_t0", "Floor_03_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage03/floor_03/Floor_03_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_03_t1", "Floor_03_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage03/floor_03/Floor_03_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_03_t2", "Floor_03_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage03/floor_03/Floor_03_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_03_t3", "Floor_03_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage03/floor_03/Floor_03_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_03_t4", "Floor_03_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage03/floor_03/Floor_03_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_03_t5", "Floor_03_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage03/floor_03/Floor_03_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_04_t0", "Floor_04_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage04/floor_04/Floor_04_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_04_t1", "Floor_04_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage04/floor_04/Floor_04_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_04_t2", "Floor_04_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage04/floor_04/Floor_04_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_04_t3", "Floor_04_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage04/floor_04/Floor_04_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_04_t4", "Floor_04_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage04/floor_04/Floor_04_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_04_t5", "Floor_04_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage04/floor_04/Floor_04_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_05_t0", "Floor_05_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage05/floor_05/Floor_05_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_05_t1", "Floor_05_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage05/floor_05/Floor_05_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_05_t2", "Floor_05_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage05/floor_05/Floor_05_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_05_t3", "Floor_05_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage05/floor_05/Floor_05_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_05_t4", "Floor_05_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage05/floor_05/Floor_05_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_05_t5", "Floor_05_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage05/floor_05/Floor_05_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_06_t0", "Floor_06_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage06/floor_06/Floor_06_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_06_t1", "Floor_06_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage06/floor_06/Floor_06_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_06_t2", "Floor_06_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage06/floor_06/Floor_06_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_06_t3", "Floor_06_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage06/floor_06/Floor_06_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_06_t4", "Floor_06_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage06/floor_06/Floor_06_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_06_t5", "Floor_06_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage06/floor_06/Floor_06_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_07_t0", "Floor_07_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage07/floor_07/Floor_07_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_07_t1", "Floor_07_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage07/floor_07/Floor_07_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_07_t2", "Floor_07_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage07/floor_07/Floor_07_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_07_t3", "Floor_07_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage07/floor_07/Floor_07_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_07_t4", "Floor_07_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage07/floor_07/Floor_07_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_07_t5", "Floor_07_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage07/floor_07/Floor_07_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_08_t0", "Floor_08_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage08/floor_08/Floor_08_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_08_t1", "Floor_08_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage08/floor_08/Floor_08_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_08_t2", "Floor_08_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage08/floor_08/Floor_08_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_08_t3", "Floor_08_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage08/floor_08/Floor_08_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_08_t4", "Floor_08_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage08/floor_08/Floor_08_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_08_t5", "Floor_08_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage08/floor_08/Floor_08_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_09_t0", "FLOOR_09_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage09/floor_09/FLOOR_09_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_09_t1", "FLOOR_09_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage09/floor_09/FLOOR_09_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_09_t2", "FLOOR_09_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage09/floor_09/FLOOR_09_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_09_t3", "FLOOR_09_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage09/floor_09/FLOOR_09_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_09_t4", "FLOOR_09_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage09/floor_09/FLOOR_09_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_09_t5", "FLOOR_09_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage09/floor_09/FLOOR_09_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_10_t0", "FLOOR_10_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage10/floor_10/FLOOR_10_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_10_t1", "FLOOR_10_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage10/floor_10/FLOOR_10_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_10_t2", "FLOOR_10_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage10/floor_10/FLOOR_10_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_10_t3", "FLOOR_10_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage10/floor_10/FLOOR_10_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_10_t4", "FLOOR_10_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage10/floor_10/FLOOR_10_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_10_t5", "FLOOR_10_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage10/floor_10/FLOOR_10_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_11_t0", "FLOOR_11_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage11/floor_11/FLOOR_11_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_11_t1", "FLOOR_11_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage11/floor_11/FLOOR_11_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_11_t2", "FLOOR_11_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage11/floor_11/FLOOR_11_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_11_t3", "FLOOR_11_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage11/floor_11/FLOOR_11_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_11_t4", "FLOOR_11_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage11/floor_11/FLOOR_11_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FLOOR_11_t5", "FLOOR_11_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage11/floor_11/FLOOR_11_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_12_t0", "Floor_12_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage12/floor_12/Floor_12_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_12_t1", "Floor_12_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage12/floor_12/Floor_12_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_12_t2", "Floor_12_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage12/floor_12/Floor_12_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_12_t3", "Floor_12_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage12/floor_12/Floor_12_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_12_t4", "Floor_12_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage12/floor_12/Floor_12_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_12_t5", "Floor_12_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage12/floor_12/Floor_12_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_13_t0", "Floor_13_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage13/floor_13/Floor_13_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_13_t1", "Floor_13_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage13/floor_13/Floor_13_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_13_t2", "Floor_13_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage13/floor_13/Floor_13_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_13_t3", "Floor_13_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage13/floor_13/Floor_13_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_13_t4", "Floor_13_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage13/floor_13/Floor_13_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_13_t5", "Floor_13_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage13/floor_13/Floor_13_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_14_t0", "Floor_14_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage14/floor_14/Floor_14_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_14_t1", "Floor_14_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage14/floor_14/Floor_14_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_14_t2", "Floor_14_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage14/floor_14/Floor_14_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_14_t3", "Floor_14_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage14/floor_14/Floor_14_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_14_t4", "Floor_14_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage14/floor_14/Floor_14_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_14_t5", "Floor_14_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage14/floor_14/Floor_14_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_15_t0", "Floor_15_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage15/floor_15/Floor_15_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_15_t1", "Floor_15_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage15/floor_15/Floor_15_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_15_t2", "Floor_15_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage15/floor_15/Floor_15_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_15_t3", "Floor_15_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage15/floor_15/Floor_15_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_15_t4", "Floor_15_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage15/floor_15/Floor_15_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_15_t5", "Floor_15_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage15/floor_15/Floor_15_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_16_t0", "Floor_16_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage16/floor_16/Floor_16_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_16_t1", "Floor_16_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage16/floor_16/Floor_16_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_16_t2", "Floor_16_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage16/floor_16/Floor_16_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_16_t3", "Floor_16_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage16/floor_16/Floor_16_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_16_t4", "Floor_16_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage16/floor_16/Floor_16_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_16_t5", "Floor_16_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage16/floor_16/Floor_16_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_17_t0", "Floor_17_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage17/floor_17/Floor_17_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_17_t1", "Floor_17_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage17/floor_17/Floor_17_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_17_t2", "Floor_17_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage17/floor_17/Floor_17_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_17_t3", "Floor_17_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage17/floor_17/Floor_17_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_17_t4", "Floor_17_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage17/floor_17/Floor_17_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_17_t5", "Floor_17_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage17/floor_17/Floor_17_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_18_t0", "Floor_18_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage18/floor_18/Floor_18_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_18_t1", "Floor_18_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage18/floor_18/Floor_18_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_18_t2", "Floor_18_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage18/floor_18/Floor_18_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_18_t3", "Floor_18_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage18/floor_18/Floor_18_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_18_t4", "Floor_18_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage18/floor_18/Floor_18_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_18_t5", "Floor_18_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage18/floor_18/Floor_18_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_19_t0", "Floor_19_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage19/floor_19/Floor_19_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_19_t1", "Floor_19_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage19/floor_19/Floor_19_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_19_t2", "Floor_19_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage19/floor_19/Floor_19_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_19_t3", "Floor_19_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage19/floor_19/Floor_19_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_19_t4", "Floor_19_t4", 1024, 256, new Rect( 0f, 0f, 1024f, 256f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage19/floor_19/Floor_19_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_19_t5", "Floor_19_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage19/floor_19/Floor_19_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_20_t0", "Floor_20_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage20/floor_20/Floor_20_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_20_t1", "Floor_20_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage20/floor_20/Floor_20_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_20_t2", "Floor_20_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage20/floor_20/Floor_20_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_20_t3", "Floor_20_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage20/floor_20/Floor_20_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_20_t4", "Floor_20_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage20/floor_20/Floor_20_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_20_t5", "Floor_20_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage20/floor_20/Floor_20_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_21_t0", "Floor_21_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage21/floor_21/Floor_21_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_21_t1", "Floor_21_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage21/floor_21/Floor_21_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_21_t2", "Floor_21_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage21/floor_21/Floor_21_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_21_t3", "Floor_21_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage21/floor_21/Floor_21_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_21_t4", "Floor_21_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage21/floor_21/Floor_21_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_21_t5", "Floor_21_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage21/floor_21/Floor_21_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_22_t0", "Floor_22_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage22/floor_22/Floor_22_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_22_t1", "Floor_22_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage22/floor_22/Floor_22_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_22_t2", "Floor_22_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage22/floor_22/Floor_22_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_22_t3", "Floor_22_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage22/floor_22/Floor_22_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_22_t4", "Floor_22_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage22/floor_22/Floor_22_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_22_t5", "Floor_22_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage22/floor_22/Floor_22_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_23_t0", "Floor_23_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage23/floor_23/Floor_23_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_23_t1", "Floor_23_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage23/floor_23/Floor_23_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_23_t2", "Floor_23_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage23/floor_23/Floor_23_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_23_t3", "Floor_23_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage23/floor_23/Floor_23_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_23_t4", "Floor_23_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage23/floor_23/Floor_23_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_23_t5", "Floor_23_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage23/floor_23/Floor_23_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_24_t0", "Floor_24_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage24/floor_24/Floor_24_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_24_t1", "Floor_24_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage24/floor_24/Floor_24_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_24_t2", "Floor_24_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage24/floor_24/Floor_24_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_24_t3", "Floor_24_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage24/floor_24/Floor_24_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_24_t4", "Floor_24_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage24/floor_24/Floor_24_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_24_t5", "Floor_24_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage24/floor_24/Floor_24_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_25_t0", "Floor_25_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage25/floor_25/Floor_25_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_25_t1", "Floor_25_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage25/floor_25/Floor_25_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_25_t2", "Floor_25_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage25/floor_25/Floor_25_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_25_t3", "Floor_25_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage25/floor_25/Floor_25_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_25_t4", "Floor_25_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage25/floor_25/Floor_25_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_25_t5", "Floor_25_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage25/floor_25/Floor_25_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_26_t0", "Floor_26_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage26/floor_26/Floor_26_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_26_t1", "Floor_26_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage26/floor_26/Floor_26_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_26_t2", "Floor_26_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage26/floor_26/Floor_26_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_26_t3", "Floor_26_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage26/floor_26/Floor_26_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_26_t4", "Floor_26_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage26/floor_26/Floor_26_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_26_t5", "Floor_26_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage26/floor_26/Floor_26_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_27_t0", "Floor_27_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage27/floor_27/Floor_27_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_27_t1", "Floor_27_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage27/floor_27/Floor_27_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_27_t2", "Floor_27_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage27/floor_27/Floor_27_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_27_t3", "Floor_27_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage27/floor_27/Floor_27_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_27_t4", "Floor_27_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage27/floor_27/Floor_27_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_27_t5", "Floor_27_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage27/floor_27/Floor_27_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_28_t0", "Floor_28_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage28/floor_28/Floor_28_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_28_t1", "Floor_28_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage28/floor_28/Floor_28_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_28_t2", "Floor_28_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage28/floor_28/Floor_28_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_28_t3", "Floor_28_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage28/floor_28/Floor_28_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_28_t4", "Floor_28_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage28/floor_28/Floor_28_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_28_t5", "Floor_28_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage28/floor_28/Floor_28_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_29_t0", "Floor_29_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage29/floor_29/Floor_29_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_29_t1", "Floor_29_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage29/floor_29/Floor_29_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_29_t2", "Floor_29_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage29/floor_29/Floor_29_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_29_t3", "Floor_29_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage29/floor_29/Floor_29_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_29_t4", "Floor_29_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage29/floor_29/Floor_29_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_29_t5", "Floor_29_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage29/floor_29/Floor_29_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_30_t0", "Floor_30_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage30/floor_30/Floor_30_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_30_t1", "Floor_30_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage30/floor_30/Floor_30_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_30_t2", "Floor_30_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage30/floor_30/Floor_30_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_30_t3", "Floor_30_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage30/floor_30/Floor_30_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_30_t4", "Floor_30_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage30/floor_30/Floor_30_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_30_t5", "Floor_30_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage30/floor_30/Floor_30_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_31_t0", "Floor_31_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage31/floor_31/Floor_31_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_31_t1", "Floor_31_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage31/floor_31/Floor_31_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_31_t2", "Floor_31_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage31/floor_31/Floor_31_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_31_t3", "Floor_31_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage31/floor_31/Floor_31_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_31_t4", "Floor_31_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage31/floor_31/Floor_31_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_31_t5", "Floor_31_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage31/floor_31/Floor_31_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_32_t0", "Floor_32_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage32/floor_32/Floor_32_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_32_t1", "Floor_32_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage32/floor_32/Floor_32_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_32_t2", "Floor_32_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage32/floor_32/Floor_32_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_32_t3", "Floor_32_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage32/floor_32/Floor_32_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_32_t4", "Floor_32_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage32/floor_32/Floor_32_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_32_t5", "Floor_32_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage32/floor_32/Floor_32_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_32_t6", "Floor_32_t6", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage32/floor_32/Floor_32_t6", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_32_t7", "Floor_32_t7", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage32/floor_32/Floor_32_t7", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_33_t0", "Floor_33_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage33/floor_33/Floor_33_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_33_t1", "Floor_33_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage33/floor_33/Floor_33_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_33_t2", "Floor_33_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage33/floor_33/Floor_33_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_33_t3", "Floor_33_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage33/floor_33/Floor_33_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_33_t4", "Floor_33_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage33/floor_33/Floor_33_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_33_t5", "Floor_33_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage33/floor_33/Floor_33_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_34_t0", "Floor_34_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage34/floor_34/Floor_34_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_34_t1", "Floor_34_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage34/floor_34/Floor_34_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_34_t2", "Floor_34_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage34/floor_34/Floor_34_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_34_t3", "Floor_34_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage34/floor_34/Floor_34_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_34_t4", "Floor_34_t4", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage34/floor_34/Floor_34_t4", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_34_t5", "Floor_34_t5", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage34/floor_34/Floor_34_t5", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_35_t0", "Floor_35_t0", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage35/floor_35/Floor_35_t0", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_35_t1", "Floor_35_t1", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage35/floor_35/Floor_35_t1", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_35_t2", "Floor_35_t2", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage35/floor_35/Floor_35_t2", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Floor_35_t3", "Floor_35_t3", 1024, 512, new Rect( 0f, 0f, 1024f, 512f ), new Vector2( 0f, 1f ), 2f, "texture/stage/stage35/floor_35/Floor_35_t3", new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "EANM_01_20", "EANM_01_t0", 1024, 256, new Rect( 1f, 5f, 642f, 250f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "EANM_02_14", "EANM_02_t0", 1024, 512, new Rect( 1f, 165f, 642f, 346f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Eanm_01_3", "Eanm_01_t0", 64, 128, new Rect( 1f, 31f, 60f, 96f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Eanm_02_7", "Eanm_02_t0", 512, 512, new Rect( 1f, 11f, 498f, 500f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Eanm_03_18", "Eanm_03_t0", 1024, 512, new Rect( 1f, 127f, 560f, 384f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Eanm_04_0", "Eanm_04_t0", 512, 256, new Rect( 1f, 15f, 300f, 240f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Eanm_07_4", "Eanm_07_t0", 1024, 512, new Rect( 1f, 109f, 642f, 402f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Eanm_12_6", "Eanm_12_t0", 256, 256, new Rect( 1f, 63f, 180f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Eanm_14_2", "Eanm_14_t0", 128, 128, new Rect( 0f, 67f, 91f, 61f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t0", 512, 1024, new Rect( 1f, 413f, 244f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t1", 512, 1024, new Rect( 269f, 821f, 240f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t2", 512, 1024, new Rect( 243f, 7f, 238f, 200f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t3", 512, 1024, new Rect( 1f, 821f, 246.92387f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t4", 512, 1024, new Rect( 1f, 617f, 246f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t5", 512, 1024, new Rect( 1f, 209f, 240f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t6", 512, 1024, new Rect( 247f, 413f, 244f, 200f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t7", 512, 1024, new Rect( 1f, 5f, 240f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t8", 512, 1024, new Rect( 243f, 209f, 240f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE01", "FACE01_t9", 512, 1024, new Rect( 249f, 615f, 244f, 204f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t0", 1024, 1024, new Rect( 237f, 635f, 230f, 190f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t1", 1024, 1024, new Rect( 482f, 637f, 220.92387f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t10", 1024, 1024, new Rect( 1f, 847f, 240f, 176f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t11", 1024, 1024, new Rect( 243f, 827f, 218.92387f, 196f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t2", 1024, 1024, new Rect( 717f, 833f, 230.92387f, 190f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t3", 1024, 1024, new Rect( 714f, 446.0761f, 218.92387f, 190.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t4", 1024, 1024, new Rect( 1f, 655f, 222.92387f, 190f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t5", 1024, 1024, new Rect( 231f, 441f, 216.92387f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t6", 1024, 1024, new Rect( 1f, 461f, 228f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t7", 1024, 1024, new Rect( 482f, 831f, 218.92387f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t8", 1024, 1024, new Rect( 717f, 639f, 220.92387f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE02", "FACE02_t9", 1024, 1024, new Rect( 469f, 443f, 224f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE03", "FACE03_t0", 512, 1024, new Rect( 1f, 831f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE03", "FACE03_t1", 512, 1024, new Rect( 249f, 831f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE03", "FACE03_t2", 512, 1024, new Rect( 1f, 637f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE03", "FACE03_t3", 512, 1024, new Rect( 249f, 637f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE03", "FACE03_t4", 512, 1024, new Rect( 249f, 455f, 246f, 180f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE03", "FACE03_t5", 512, 1024, new Rect( 249f, 273f, 246f, 180f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE03", "FACE03_t6", 512, 1024, new Rect( 1f, 443f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE03", "FACE03_t7", 512, 1024, new Rect( 1f, 249f, 216.92387f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE04", "FACE04_t0", 512, 1024, new Rect( 249f, 211f, 246f, 200f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE04", "FACE04_t1", 512, 1024, new Rect( 249f, 821f, 246f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE04", "FACE04_t3", 512, 1024, new Rect( 1f, 819f, 246f, 204f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE04", "FACE04_t4", 512, 1024, new Rect( 1f, 615f, 246f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE04", "FACE04_t5", 512, 1024, new Rect( 1f, 209f, 246f, 200f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE04", "FACE04_t6", 512, 1024, new Rect( 249f, 413f, 246f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "FACE04", "FACE04_t7", 512, 1024, new Rect( 1f, 411f, 246f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face05", "Face05_t0", 512, 512, new Rect( 1f, 309f, 246f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face05", "Face05_t1", 512, 512, new Rect( 249f, 309f, 244f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face05", "Face05_t2", 512, 512, new Rect( 1f, 105f, 244f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face06", "Face06_t0", 256, 1024, new Rect( 1f, 651f, 246f, 184f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face06", "Face06_t1", 256, 1024, new Rect( 1f, 89f, 240f, 184f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face06", "Face06_t2", 256, 1024, new Rect( 1f, 463f, 240f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face06", "Face06_t3", 256, 1024, new Rect( 1f, 837f, 246f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face06", "Face06_t4", 256, 1024, new Rect( 1f, 275f, 240f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face07", "Face07_t0", 512, 512, new Rect( 1f, 319f, 228.92387f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face07", "Face07_t1", 512, 512, new Rect( 242f, 319f, 228.92387f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face07", "Face07_t2", 512, 512, new Rect( 1f, 125f, 228.92387f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face07", "Face07_t3", 512, 512, new Rect( 242f, 125f, 238f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face08", "Face08_t1", 512, 1024, new Rect( 206f, 414f, 190.92387f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face08", "Face08_t3", 512, 1024, new Rect( 1f, 616.0761f, 198.92387f, 202.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face08", "Face08_t4", 512, 1024, new Rect( 1f, 821f, 206.92387f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face08", "Face08_t5", 512, 1024, new Rect( 229f, 823f, 198.92387f, 200f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face09", "Face09_t0", 512, 512, new Rect( 1f, 319f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face09", "Face09_t1", 512, 512, new Rect( 249f, 319f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face09", "Face09_t2", 512, 512, new Rect( 1f, 129f, 246f, 188f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face09", "Face09_t3", 512, 512, new Rect( 249f, 129f, 246f, 188f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face10", "Face10_t0", 256, 1024, new Rect( 1f, 657f, 246f, 180f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face10", "Face10_t1", 256, 1024, new Rect( 1f, 479f, 246f, 176f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face10", "Face10_t2", 256, 1024, new Rect( 1f, 301f, 246f, 176f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face10", "Face10_t3", 256, 1024, new Rect( 1f, 839f, 246f, 184f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face11", "Face11_t0", 512, 512, new Rect( 1f, 329f, 230.92387f, 182f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face11", "Face11_t1", 512, 512, new Rect( 239f, 332.0761f, 236f, 178.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face11", "Face11_t2", 512, 512, new Rect( 239f, 152f, 236f, 178f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face15", "Face15_t0", 256, 512, new Rect( 1f, 328.0761f, 138.92387f, 182.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face16", "Face16_t1", 512, 512, new Rect( 1f, 113f, 234f, 196f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face17", "Face17_t0", 512, 512, new Rect( 1f, 110f, 232f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face17", "Face17_t1", 512, 512, new Rect( 240f, 313f, 222.92387f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face17", "Face17_t2", 512, 512, new Rect( 1f, 310.0761f, 226.92387f, 200.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face17", "Face17_t3", 512, 512, new Rect( 240f, 113f, 222.92387f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face19", "Face19_t0", 256, 512, new Rect( 1f, 127f, 196.92387f, 190f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face19", "Face19_t1", 256, 512, new Rect( 1f, 319f, 236f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face20", "Face20_t0", 512, 512, new Rect( 1f, 125f, 234f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face20", "Face20_t1", 512, 512, new Rect( 1f, 319f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face22", "Face22_t0", 512, 512, new Rect( 237f, 313f, 216.92387f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face22", "Face22_t1", 512, 512, new Rect( 1f, 315f, 220.92387f, 196f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face22", "Face22_t2", 512, 512, new Rect( 1f, 117f, 212.92387f, 196f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face23", "Face23_t0", 512, 512, new Rect( 225f, 263f, 216.92387f, 248f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face23", "Face23_t1", 512, 512, new Rect( 1f, 261f, 222f, 250f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face23", "Face23_t2", 512, 512, new Rect( 1f, 10.07612f, 224f, 248.92387f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face24", "Face24_t0", 512, 1024, new Rect( 1f, 467f, 210.92387f, 180f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face24", "Face24_t1", 512, 1024, new Rect( 213f, 279f, 202.92387f, 184f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face24", "Face24_t2", 512, 1024, new Rect( 218f, 465f, 202.92387f, 182f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face24", "Face24_t3", 512, 1024, new Rect( 1f, 649f, 210.92387f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face24", "Face24_t4", 512, 1024, new Rect( 1f, 283f, 202.92387f, 182f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face24", "Face24_t5", 512, 1024, new Rect( 220f, 649f, 210.92387f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face24", "Face24_t6", 512, 1024, new Rect( 1f, 837f, 210.92387f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face24", "Face24_t7", 512, 1024, new Rect( 221f, 837f, 210.92387f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face25", "Face25_t0", 512, 512, new Rect( 1f, 33f, 192f, 236f ), new Vector2( 0.5f, 0f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face25", "Face25_t1", 512, 512, new Rect( 196f, 37f, 203.92387f, 232f ), new Vector2( 0.5050904f, 0f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face25", "Face25_t2", 512, 512, new Rect( 229f, 279f, 198f, 232f ), new Vector2( 0.5f, 0f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face25", "Face25_t3", 512, 512, new Rect( 1f, 271f, 226f, 240f ), new Vector2( 0.5f, 0f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face26", "Face26_t0", 256, 256, new Rect( 1f, 63f, 240f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face27", "Face27_t0", 512, 512, new Rect( 1f, 271f, 228f, 240f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face27", "Face27_t1", 512, 512, new Rect( 231f, 271f, 224f, 240f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face27", "Face27_t2", 512, 512, new Rect( 1f, 29f, 228f, 240f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face28", "Face28_t0", 256, 256, new Rect( 1f, 55f, 232f, 200f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face30", "Face30_t1", 512, 512, new Rect( 240f, 321f, 218.92387f, 190f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face31", "Face31_t0", 256, 512, new Rect( 1f, 103f, 176f, 204f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face32", "Face32_t0", 512, 512, new Rect( 1f, 320.0761f, 236f, 190.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face32", "Face32_t1", 512, 512, new Rect( 239f, 319f, 230f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face32", "Face32_t2", 512, 512, new Rect( 1f, 126f, 230f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face33", "Face33_t0", 1024, 256, new Rect( 1f, 57f, 266f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face33", "Face33_t1", 1024, 256, new Rect( 269f, 57f, 266f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face33", "Face33_t2", 1024, 256, new Rect( 537f, 58.076122f, 266f, 196.92387f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face34", "Face34_t0", 512, 1024, new Rect( 217f, 614.0761f, 202.92387f, 202.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face34", "Face34_t1", 512, 1024, new Rect( 1f, 409.0761f, 202.92387f, 202.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face34", "Face34_t2", 512, 1024, new Rect( 239f, 819f, 198.92387f, 204f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face34", "Face34_t3", 512, 1024, new Rect( 1f, 614.0761f, 200.92387f, 204.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face34", "Face34_t4", 512, 1024, new Rect( 1f, 821f, 232.92387f, 202f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face35", "Face35_t1", 512, 1024, new Rect( 242f, 474f, 224f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face35", "Face35_t2", 512, 1024, new Rect( 275f, 662.0761f, 224.92387f, 184.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face35", "Face35_t3", 512, 1024, new Rect( 1f, 647f, 226.92387f, 184f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face35", "Face35_t4", 512, 1024, new Rect( 275f, 849f, 226.92387f, 174f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face36", "Face36_t0", 256, 1024, new Rect( 1f, 59f, 246f, 184f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face36", "Face36_t1", 256, 1024, new Rect( 1f, 631f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face36", "Face36_t2", 256, 1024, new Rect( 1f, 437f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face36", "Face36_t3", 256, 1024, new Rect( 1f, 245f, 246f, 190f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face36", "Face36_t4", 256, 1024, new Rect( 1f, 825f, 246f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face37", "Face37_t0", 512, 512, new Rect( 1f, 313f, 246f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face37", "Face37_t1", 512, 512, new Rect( 249f, 319f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face37", "Face37_t2", 512, 512, new Rect( 249f, 131f, 216f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face38", "Face38_t0", 256, 1024, new Rect( 1f, 25f, 240f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face38", "Face38_t1", 256, 1024, new Rect( 1f, 419f, 246f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face38", "Face38_t2", 256, 1024, new Rect( 1f, 219f, 246f, 198f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face39", "Face39_t0", 512, 512, new Rect( 227f, 312.0761f, 210.92387f, 198.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face39", "Face39_t1", 512, 512, new Rect( 1f, 315f, 216.92387f, 196f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face40", "Face40_t1", 256, 512, new Rect( 1f, 321f, 246f, 190f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face41", "Face41_t0", 256, 512, new Rect( 1f, 319f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face41", "Face41_t1", 256, 512, new Rect( 1f, 125f, 246f, 192f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face43", "Face43_t0", 256, 256, new Rect( 1f, 78.07612f, 202.92387f, 176.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face44", "Face44_t0", 256, 256, new Rect( 1f, 76.07612f, 192.92387f, 178.92387f ), new Vector2( 0f, 1.0000001f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face45", "Face45_t0", 256, 256, new Rect( 1f, 69f, 240f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face49", "Face49_t0", 256, 512, new Rect( 1f, 311f, 246f, 200f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face49", "Face49_t1", 256, 512, new Rect( 1f, 109f, 246f, 200f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Face51", "Face51_t0", 256, 256, new Rect( 1f, 69f, 246f, 186f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "HelpWindow", "HelpWindow_0", 512, 512, new Rect( 0.29289323f, 260f, 339.7071f, 252f ), new Vector2( 0.49956894f, 0.5f ), 2f, null, new Vector4( 158.7071f, 26f, 170.99998f, 226f ) );
            CheckStaticProbe( "MSG_BOX", "MSG_BOX_t0", 1024, 128, new Rect( 1f, 3f, 632f, 124f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            CheckStaticProbe( "Title", "Title_t0", 1024, 512, new Rect( 1f, 130.07613f, 642f, 380.9239f ), new Vector2( 0f, 1f ), 2f, null, new Vector4( 0f, 0f, 0f, 0f ) );
            string[] excluded = { "105man", "Eanm_11_5", "map00-1_unapproved", "FLOOR_00", "FACE01_extra" };
            for ( int i = 0; i < excluded.Length; i++ )
            {
                Texture2D texture = new Texture2D( 16, 16 );
                texture.name = excluded[ i ];
                Sprite legacy = Sprite.Create( texture, new Rect( 0, 0, 16, 16 ), Vector2.zero, 2.0f );
                legacy.name = excluded[ i ] + "_t0";
                if ( Resolve( legacy ) != legacy ||
                    Resolve( legacy, "Texture/Stage/Stage00/FLOOR_00/FLOOR_00_t0" ) != legacy ||
                    !Mathf.Approximately( GetUIScale( legacy, 0.73f ), 0.73f ) )
                {
                    throw new InvalidOperationException( "Excluded dynamic or unknown Sprite was replaced: " + excluded[ i ] );
                }
                UnityEngine.Object.Destroy( legacy );
                UnityEngine.Object.Destroy( texture );
            }
            string[,] excludedUI = { { "HelpWindow", "HelpWindow_extra" }, { "MSG_BOX", "MSG_BOX_extra" }, { "map00-1", "map00-1_unapproved" } };
            for ( int i = 0; i < excludedUI.GetLength( 0 ); i++ )
            {
                Texture2D texture = new Texture2D( 16, 16 );
                texture.name = excludedUI[ i, 0 ];
                Sprite legacy = Sprite.Create( texture, new Rect( 0, 0, 16, 16 ), Vector2.zero, 2.0f );
                legacy.name = excludedUI[ i, 1 ];
                if ( Resolve( legacy ) != legacy || !Mathf.Approximately( GetUIScale( legacy, 0.73f ), 0.73f ) )
                    throw new InvalidOperationException( "Unselected UI Sprite was replaced: " + legacy.name );
                UnityEngine.Object.Destroy( legacy );
                UnityEngine.Object.Destroy( texture );
            }
            string[] excludedFloors = { "Floor_03_t6", "Floor_04_t9" };
            for ( int i = 0; i < excludedFloors.Length; i++ )
            {
                Texture2D texture = new Texture2D( 16, 16 ); texture.name = excludedFloors[i];
                Sprite legacy = Sprite.Create( texture, new Rect( 0, 0, 16, 16 ), Vector2.zero, 2 ); legacy.name = excludedFloors[i];
                if ( Resolve( legacy, "Texture/Stage/Stage03/Floor_03/" + legacy.name ) != legacy ||
                    !Mathf.Approximately( GetUIScale( legacy, 0.73f ), 0.73f ) )
                    throw new InvalidOperationException( "Unapproved terrain replaced or rescaled: " + legacy.name );
                UnityEngine.Object.Destroy( legacy ); UnityEngine.Object.Destroy( texture );
            }
            Texture2D wrongFloorTexture = new Texture2D( 1024, 512 ); wrongFloorTexture.name = "Floor_03_t0";
            Sprite wrongFloor = Sprite.Create( wrongFloorTexture, new Rect( 0, 0, 1023, 512 ), new Vector2( 0, 1 ), 2 ); wrongFloor.name = "Floor_03_t0";
            CheckRequiredFailure( () => { Resolve( wrongFloor, "Texture/Stage/Stage03/Floor_03/Floor_03_t0" ); } );
            UnityEngine.Object.Destroy( wrongFloor ); UnityEngine.Object.Destroy( wrongFloorTexture );
            Texture2D wrongTexture = new Texture2D( 512, 1024 );
            wrongTexture.name = "FACE01";
            Sprite wrongGeometry = Sprite.Create( wrongTexture, new Rect( 1, 413, 245, 202 ), new Vector2( 0, 1 ), 2.0f );
            wrongGeometry.name = "FACE01_t0";
            CheckRequiredFailure( () => { Resolve( wrongGeometry ); } );
            UnityEngine.Object.Destroy( wrongGeometry );
            UnityEngine.Object.Destroy( wrongTexture );
            Texture2D borderTexture = new Texture2D( 512, 512 );
            borderTexture.name = "HelpWindow";
            Sprite wrongBorder = Sprite.Create( borderTexture, new Rect( 0.29289323f, 260, 339.7071f, 252 ), new Vector2( 0.49956894f, 0.5f ), 2.0f );
            wrongBorder.name = "HelpWindow_0";
            CheckRequiredFailure( () => { Resolve( wrongBorder ); } );
            UnityEngine.Object.Destroy( wrongBorder );
            UnityEngine.Object.Destroy( borderTexture );
            CheckTitlePanelImage();
            int atlasProbes = CheckBuiltAtlasSprites();
            Debug.Log( "SWORDSMAN_STATIC_MVP_SMOKE passed mode=static" +
                " statics=1819 omissions=1355 portraitSlices=165 uiSlices=431 shopBackdrops=12 campBackdrops=33 propSlices=9 fixedTerrainSlices=216 probes=472 excluded=10 geometryRejected=3 atlasProbes=" + atlasProbes );
            Application.Quit( 0 );
        }
        catch ( Exception exception )
        {
            Debug.LogError( "SWORDSMAN_STATIC_MVP_SMOKE failed: " + exception.Message );
            Application.Quit( 1 );
        }
    }

    static void CheckRequiredFailure( Action action )
    {
        try { action(); }
        catch ( InvalidOperationException exception )
        {
            if ( exception.Message.StartsWith( "HD_REQUIRED:", StringComparison.Ordinal ) ) return;
            throw;
        }
        throw new InvalidOperationException( "Invalid required HD resource was accepted." );
    }

    static void CheckTitlePanelImage()
    {
        foreach ( UnityEngine.UI.Image image in UnityEngine.Object.FindObjectsOfType<UnityEngine.UI.Image>( true ) )
        {
            Sprite sprite = image.sprite;
            if ( sprite == null || sprite.name != "Title_t1" ) continue;
            if ( !Mathf.Approximately( sprite.pixelsPerUnit, 4f ) )
                throw new InvalidOperationException( "Title_t1 was not bound at the expected resolution." );
            return;
        }
        throw new InvalidOperationException( "Title_t1 is missing from the title scene." );
    }

    static int CheckBuiltAtlasSprites()
    {
        int checkedCount = 0;
        foreach ( AtlasSpec[] specs in new[] { PortraitAtlasSpecs, UIAtlasSpecs } )
        {
            foreach ( AtlasSpec spec in specs )
            {
                Sprite[] sprites = bundle.LoadAssetWithSubAssets<Sprite>( spec.bundlePath );
                if ( sprites.Length != spec.expectedSpriteCount )
                    throw new InvalidOperationException( "Incorrect runtime atlas slice count: " + spec.textureName );
                var legacyTexture = new Texture2D( sprites[ 0 ].texture.width / 2, sprites[ 0 ].texture.height / 2 );
                legacyTexture.name = spec.textureName;
                foreach ( Sprite hd in sprites )
                {
                    Vector2 pivot = new Vector2( hd.pivot.x / hd.rect.width, hd.pivot.y / hd.rect.height );
                    Sprite legacy = Sprite.Create( legacyTexture, new Rect( 0, 0, hd.rect.width / 2, hd.rect.height / 2 ),
                        pivot, hd.pixelsPerUnit / 2, 0, SpriteMeshType.FullRect, hd.border / 2 );
                    legacy.name = hd.name;
                    if ( Resolve( legacy ) != hd || Resolve( legacy, "texture/" + spec.textureName ) != hd ||
                        !Mathf.Approximately( GetUIScale( hd, 0.5f ), 1.0f / hd.pixelsPerUnit ) )
                        throw new InvalidOperationException( "Runtime atlas slice replacement mismatch: " + hd.name );
                    UnityEngine.Object.Destroy( legacy );
                    checkedCount++;
                }
                UnityEngine.Object.Destroy( legacyTexture );
            }
        }
        if ( checkedCount != 650 + ApprovedOmissionCount )
            throw new InvalidOperationException( "Incorrect runtime atlas probe count: " + checkedCount );
        return checkedCount;
    }

    static void CheckStaticProbe( string textureName, string spriteName, int width, int height, Rect rect, Vector2 pivot, float ppu, string resourcePath, Vector4? border = null )
    {
        Texture2D texture = new Texture2D( width, height );
        texture.name = textureName;
        Sprite legacy = Sprite.Create( texture, rect, pivot, ppu, 0, SpriteMeshType.FullRect, border ?? Vector4.zero );
        legacy.name = spriteName;
        Sprite resolved = resourcePath == null ? Resolve( legacy ) : Resolve( legacy, resourcePath );
        if ( resolved == legacy || resolved.name != legacy.name || !HasMatchingWorldGeometry( legacy, resolved ) )
        {
            throw new InvalidOperationException( "Static Sprite replacement mismatch: " + spriteName );
        }
        if ( ( resourcePath == null || IsApprovedFixedTerrain( resolved ) ) && !Mathf.Approximately( GetUIScale( resolved, 0.5f ), 1.0f / resolved.pixelsPerUnit ) )
        {
            throw new InvalidOperationException( "Dialogue UI scale mismatch: " + spriteName );
        }
        UnityEngine.Object.Destroy( legacy );
        UnityEngine.Object.Destroy( texture );
    }
#endif

    static void LogActivationOnce( string group )
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if ( activationLogs.Add( group ) )
        {
            Debug.Log( "SWORDSMAN_HD " + group + " active." );
        }
#endif
    }

    static InvalidOperationException RequiredResourceFailure( string message )
    {
        return new InvalidOperationException( "HD_REQUIRED: " + message );
    }
}
