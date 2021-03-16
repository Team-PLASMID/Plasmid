# Plasmid

## Structure
This is a multi-platform MonoGame program with targets for Android and PC.
The Visual Studio solution consists of 3 separate projects:
* Plasmid_Core
* Plasmid_Android
* Plasmid_PC

The bulk of the code is in the core project, and the Android and PC projects only contain code that's platform-specific.
This setup allows us to build the project for both Android and PC while minimizing the amount of code we have to write twice.

The project is primarily intended for Android, but setting up like this gives us flexibility in case we want to have a PC version down the road. It also could make it conveniennt to test certain things in a PC environment instead of needing to transfer files and run on a Android device.

To build and run the game, you'll have to select either the Android or PC project depending on the platform you want to use.

## Monogame overview

By default, a MonoGame project has a Game1.cs and a Program.cs.

Game1.cs contains the code for the actual primary game loop, and Program.cs is just a main() that makes a Game1 object and runs it. Since our project is multi-platform, the Game1 class is in Plasmid_Core and it's shared. Both Plasmid_Android and Plasmid_PC have their own main() since they get started differently (For Android it's Activity1.cs and it's got a bunch of extra configuration stuff compared to the PC version)

The Game1 class has all of the basics MonoGame needs to run a game.

### Initialize()
### LoadContent()
### Update()
### Draw()
