# ![iw4-zaf](iw4-zone-asset-finder/cardicon_heartbeatsensor.png) IW4 Zone Asset Finder

This small CLI tool is a very useful ally in building patches for existing zones that are missing assets, or finding which zone to include in your zone_source to obtain a specific asset.

## How to use
You can download a release in the Releases section or build one yourself.
Once you have a **.exe**, put it at the root folder of iw4x. This tool needs the latest version of **iw4x** (zonebuilder) to work.
You can then launch the program.

## Useful commands

* HELP => Displays a list of all available commands
* **UPDATE** => This is an important command, and the first one you should fire before doing anything else. This function generates a dependency graph of all assets, used by every map zone (coop and mp). It's a slow but necessary operation, which will then make finding assets between zones almost instantaneous. This process will load and list the assets of each appropriate zonefiles and build a dependency graph from it. You only need to fire it once, and then the graph will be stored in iw4x folder.
* SEARCH => Searches for an asset by name, with a lose text search (imprecise names are allowed). Example:
```
> SEARCH bushtree

For search term "bushtree" :
Found in the following zones:

image:~foliage_pacific_bushtree01_b~598da624
    mp_afghan
    mp_arkaden_sh
    mp_awp_arena
    mp_battlecity_mountains
    mp_battlecity_mountains_td
    mp_boneyard
```
* FIND => Finds an asset by exact type and name. The format is: `type`:`name`. Example:
```
> FIND xmodel:com_barrel_metal

For asset "xmodel:com_barrel_metal" :
Found in the following zones:

xmodel:com_barrel_metal
    mp_strike_sh
    mp_strike
```

* BUILDREQ => This command will build an optimized zone_source.csv with a minimal set of imports for every asset you give it. For example, if you give it three models, present in multiple maps, it will generate a zone source containing these models & the minimal set of maps required to build this source. The format is: `"type:name type:name type:name ..."` (enclosed with a " character). Example:
```
> BUILDREQ "xmodel:com_barrel_metal xmodel:com_barrel_shard1 material:mc/mtl_barrel_tan"

The following dependencies are suggested: mp_strike(2 assets), mp_highrise(1 assets),

Source requirements:
require,mp_strike
require,mp_highrise

xmodel,com_barrel_metal
xmodel,com_barrel_shard1
material,mc/mtl_barrel_tan
``` 

* IWDFILES => Lists the IWD files (iwi + streamed sounds) used by a map, and outputs them in `mapname_iwd.txt`. 
