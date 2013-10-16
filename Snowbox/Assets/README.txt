*******************************************************************************
 How to test the game client
*******************************************************************************

The quickest way to try the game is by just running a client in the editor
and connecting to UnityPark's official servers, like this:

1. Open the Snowbox project in the Unity editor. 

2. In the project view, open the scene ClientWorld1 in the Scenes folder.

3. Press the play button in the Unity Editor.

4. Press "Play on UnityPark" to connect to the official server.

*******************************************************************************
 How to build just one server world
*******************************************************************************

The easiest way to setup a local game is by building and running one server 
and then starting a client in the editor. Here's how to do exactly that:

1. Open the Snowbox project in the Unity editor. 

2. Choose File -> Build Settings...

3. In the Build Settings window, check the scene Scenes/ServerWorld1.unity and
   uncheck all the others.

4. Select Windows or Mac standalone.

5. Press "Build & Run".

6. Name it SnowboxServer1 and save it in a new folder, for example Build. 

7. If you get a security question from your OS firewall or anti-virus then
   just give the server access to open the UDP port it needs.

8. When the server's window appears you can freely control the floating camera.
   To make sure the server is running properly you can check its log for
   "Server successfully started on port 7101".
   (On windows the log is at Snowbox\Build\SnowboxServer1_Data\output_log.txt)

9. Go back to the Build Settings window in the Unity editor.
   Make sure the standalone platform is still selected otherwise switch to it.

10. uncheck the scene Scenes/ServerWorld1.unity and instead check the
    Scenes/ClientWorld1.unity and Scenes/ClientWorld2.unity scenes and
    close the window.

11. In the project view, open the scene ClientWorld1 in the Scenes folder.

12. Press the play button in the Unity Editor.

13. When you see a ingame GUI window with a "Advanced" button. Press it.

14. A new GUI window should appear with a "LAN" selection button. Press it.

15. Now there should be at least one item in the list with your local IP and
    level "World1". Press its "Join" button.
    (On windows you can check your local IP by pressing the Windows key + R
    and then typing "cmd" and when the command window is up type "ipconfig")

16. When you have connected, you will see your avatar and be able to move it
    with the cursor keys, jump with space, fire & look with the mouse and
    chat with enter.

17. Note that the portal will not be active because you need to start a second
    server running the scene ServerWorld2. The portal, when active, is used to
    transfer players between the two servers and their different levels.


*******************************************************************************
 How to build a client and run several players on one or more computers
*******************************************************************************

If you want to play with more then one client, just build and run the game with
the two client scenes checked in the build settings and do steps 13-16 in the
above guide for each client.


*******************************************************************************
 How to start the game with two servers and activate the portal
*******************************************************************************

You can have two servers on the same machine, one running scene ServerWorld1
and the other scene ServerWorld2. With that setup, the portal will be active
and when a player goes into the portal, the avatar will be transferred to the
other server.

The servers automatically connect to each other with the P2P feature in uLink
and can be started in any order. If you want to see the configuration, look at
the two uLink components in the game object portal1, in the scene ServerWorld1.

The fastest way to test two local servers is just to build and run them.
Another way is to start them both in one editor each. When you run them both on
the same machine they will be connected to each other automatically with the
default P2P configuration. If you want them on two different machines you have
to change the IP address they will try to connect to, via P2P. When they are
connected to each other, the portal will be active and can be used by clients.


*******************************************************************************
 How to run the server and the client in two different editors
*******************************************************************************

The best way to see whats happening in the game is to run the server and
the player in separate editors. Here's how to do exactly that:

1. Close all Unity editors.

2. Create a new copy of the Snowbox project folder. Name it SnowboxClient.

3. Open the original Snowbox project in the Unity editor.

4. Choose Edit -> Preferences. In the "Preferences" window make sure
   "Show Project Wizard at Startup" is checked and close the window.
   
5. Choose File -> Build Settings. In the Build Settings window make sure
   the standalone platform is selected otherwise switch to it.

6. In the project view, open the scene ServerWorld1 in the Scenes folder.

7. Press the play button in the Unity Editor.

8. Open a second instance of the Unity editor:

   On Windows, simply double-click the Unity icon again and when the Project
   Wizard windows appears, open the new SnowboxClient project.
   
   On Mac, a second Unity instance can be opened in the terminal like this:
   /Applications/Unity/Unity.app/Contents/MacOS/Unity -projectPath "SnowboxClient/"

9. Choose File -> Build Settings. In the Build Settings window, check the
   scenes Scenes/ClientWorld1.unity and Scenes/ClientWorld2.unity.
   Uncheck all other scenes.

10. In the Build Settings window, also make sure the standalone platform is
    selected otherwise switch to it, before closing the window.

11. In the project view, open the scene ClientWorld1 in the Scenes folder.

12. Press the play button in the Unity Editor.

13. When you see a ingame GUI window with a "Advanced" button. Press it.

14. A new GUI window should appear with a "LAN" selection button. Press it.

15. Now there should be atleast one item in the list with your local IP and
    level "World1". Press its "Join" button.
    (On windows you can check your local IP by pressing the Windows Key + R
    and then typing "cmd" and when the command window is up type "ipconfig")

16. When you have connected, you will see your avatar and be able to move it
    with the cursor keys, jump with space, fire & look with the mouse and
    chat with enter.

17. If you switch to the previous Unity editor instance you should also see
    your avatar there.
