When adding a new ".wav" sound file to this folder, you need to regenerate the ".bnk" file using the "gen-gungeon-audio-bank.py" python script. You need python to run this script so download it from the following link: https://www.python.org/downloads/
If you do not have the script, reference the following page to download it: https://mtgmodders.gitbook.io/etg-modding-guide/sounds/using-custom-sounds

Once you have your new ".bnk" file, you need to add it to your project in visual studio. You do this by clicking include in project on it if it isn't already included, then change it's "Build Action" to "Embedded Resource" to have including in your mod's ".dll" file. Lastly, rebuild your project and your new custom sound should be all set up.

For helping tweaking audio settings in your ".csv" file. Refer to the following link: https://github.com/pcrain/GungeonCraft/blob/master/RawResources/Sounds/sound_info.csv
name is the name of the sound, without the path or .wav extension
volume is actually Gain in decibels
so like, 5.0 is much loader, 10.0 blasts your ears off, -5.0 is much quieter, and -10 is almost inaudible
loop is how many times the sound loops. should usually be 1
channel is always sound for your case
and limit, in theory, limits the amount of instances of a sound that can be playing at once. it should usually be 0 for unlimited
in practice limit doesn't seem to actually work correctly so it does basically nothing