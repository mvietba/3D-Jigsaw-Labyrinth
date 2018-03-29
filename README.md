# 3D-Jigsaw-Labyrinth
This is a solution to the 3rd assignment from the course **CS-E4002 Virtual and Augmented Reality**,  Aalto University.

Student: **Viet Ba, Mai**.

Mail: <firstname_without_space> . <last_name> @ aalto.fi

Developed with Unity3D version 2017.3.1f1 Personal on Windows 10.

## Free Unity Assets used:
* VR setup: SteamVR
* Labyrinth generator: DungeonTools

## Jigsaw Pieces
Materials for cubes created using Blender.

##Gameplay
The player collects pieces of a 3D jigsaw spread around a labyrinth. The jigsaw can be solved only once all pieces are collected.
###Labyrinth
* Locomotion: movement in the direction in which a player's head is facing, active when left controller's trigger button is pressed.
* Speed: increased by pressing right controller's grip button, decreased by pressing left controller's grip button.
* Interaction: touch a jigsaw piece with the right controller and press its trigger button to collect the object.
* Collect all: press a touch pad to collect all pieces immediately.

###Jigsaw
Once all pieces are collected, the player is teleported to the starting position and the pieces are places around the player.
* Locomotion: walking, head-steering disabled.	
* Interaction: modified version of _ControllerGrabObject.cs_ script by Ray Wenderlich (https://www.raywenderlich.com/149239/htc-vive-tutorial-unity)
* Object teleportation (optional): modified version of _LaserPointer.cs_ script by Ray Wenderlich (https://www.raywenderlich.com/149239/htc-vive-tutorial-unity). The player can choose to enable a laser pointer which allows teleporting a jigsaw piece, not yet snapped, to the controller, by pressing a touchpad. The laser pointer turns green when pointed at a jigsaw puzzle piece and red otherwise. This option is useful in small or furnished rooms. To grab an object, by default the user must walk up to it close enough to be able to touch it with a controller.
* Reference piece: first object interacted with after collecting all pieces becomes a reference piece. Other pieces are then "snapped" to their correct placements, relatively to it.
* Snapping: if a piece is released within close to its correct placement, it is "snapped" to the correct placement and releases white particles.
* Snapping mechanics: for each piece there is a corresponding empty holder (transparent cube mesh) with the same name. All holders are placed in a container. Firstly, the container moves together with its children to match the position and rotation of the corresponding holder to the reference piece. Afterwards, if a not yet snapped piece is released within a small positional and rotational threshold from the corresponding holder, its position and rotation are set to match the holder's and a fixed joint is created with the container's Rigidbody.
* Jigsaw displacement: allows putting the jigsaw in a comfortable position. Moving a piece that was previously snapped moves all other snapped pieces. After 10 pieces are snapped, displacement is disabled, because the behaviour of pieces becomes unexpected when there are many fixed joints to the container (e.q. jiggling, flying away).

	
<!--- Project's Github repository: https://github.com/mvietba/3D-Jigsaw-Labyrinth --->