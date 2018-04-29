## Instructions for Time Machine App for Hololens

### How to run the program:
- Open the file in Unity. File->Build settings-> Select “App” folder 
(related settings please see Hololens tutorial: https://docs.microsoft.com/en-us/windows/mixed-reality/holograms-101e)
Go to “App” folder and open .sln file with Visual Studio. 
- Then click on remote machine to deploy on Hololens. (related settings please see https://docs.microsoft.com/en-us/windows/mixed-reality/holograms-210)
- Note that if new stuffs are added in Unity, i.e. anything in Unity scene is changed, the app should be built again. Otherwise if there’s scripts changing only, just deploy again.
- If previous steps don’t work, try to open a new Unity project and import the TimeMachine.unity package. 

### App instructions:
- The room would be gradually covered by green mesh, and there is also a menu in front of you and always face towards you.
- If the menu bothers your view, click on “menu” button, and the menu would stay at where it was. Click again to resume.  
- Look around the room, and let the green mesh cover the part you want to save. That is, if a wall is not covered by green mesh, it will not be saved later on.
- Click on “save” button and the cursor might stop for one second, it’s normal. And a new brick would appear below the save button, with current time as file name.
- Click on the file button which just appeared, and the saved room model shows.
- Click on the file button again to turn off the room model, and green mesh would come back.
- Move something in the room and look at the stuff you moved to let the green mesh change. Once the green mesh nicely meets the changed surface, click on “save”.
- A new file shows. You can turn those file on and off by clicking on them. And the color of the clicked file corresponds to color of the room model. (Color repeats if more than 5 files) 
- Only when all the files are turned off and the green mesh comes back then you can save a new room model.
- On top of the menu, click on “mini” to display the room model as a miniature. Click on “normal” to go back to normal mode.
- Where to find the file: Connect the Hololens device portal from PC (i.e. enter the IP address on web browser.) FileExplorer->LocalAppData->TimeMachine->RoamingState

### How to understand the program:   
- The basis resource of this project is from this discussion: https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/188
- It explains how to save spatial understanding mesh and also provides simple code. And the code is based on “Spatial Mapping” and “Spatial Understanding” modules, which are included in HoloToolkit (Microsoft official toolkit for Hololens).
- The way to understand Unity and C# is that, Unity is the front end, and C# is the back end. For example, you can create a cube within Unity and attach a C# script as a component to  the cube, and edit the C# script to describe actions of the cube. In this project, when the “save” button is clicked, the script “ClickCommands.cs” attached on the button would detect the selection and do corresponding action, which in this case is to change the button’s name to “Cube_save_clicked”.
- The “Update()” function in each C# script would be called every frame. Thus once the script “RoomSaver.cs” knows that the save button is clicked, it will run the function “saveroom()” to save the mesh.
- And similar concept also works for RoomLoader


