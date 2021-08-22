# ARDraw-MM4ME
This Project is about AR Draw + Measure Experience in UNITY 3D with the below-mentioned features and Adding that to the MM4ME android project as a library:

* Drawing over the recognized plane (Horizontal or Vertical Plane).
* Line width adjuster.
* Video Recording while drawing.
* Multicolor options for lines.
* One or multiple fingers touch draw ability.

## Video Samples:
Video Folder: https://drive.google.com/drive/folders/1s6zhEX3iMFHdTDHKqXPMzgBqAieI8OBt

Video Recorded with self made screen recording feature : [Link](https://drive.google.com/file/d/1H1HVTivEdfG-CBdwIdChvrEZ-qpkfYSr/view?usp=sharing)



https://user-images.githubusercontent.com/56843128/130333406-571e42ba-922d-499b-b065-934f4c26b62a.mp4




## About Project:

### Scenes:
There are major 2 scenes in the project:
* ARDraw-ScreenRecording&ColorPicker : With ScreenRecording & Color picker feature.
* ARDraw : Basic ARDraw Feature.

    ![Screenshot 2021-08-21 235907](https://user-images.githubusercontent.com/56843128/130331768-6755fd97-f93d-4cc7-a4d2-208bf5951f00.png)
    
### Scripts:
There are 5 Major Scripts written for the project and other from helping Plugin/Library:

* ARDrawManager   :  Responsible for drawing the lines when eligible to draw on one or multi touch.
* ARExperienceManager   :  Responsible for interactiong between multiple scripts about when ios eligible to draw and when not.
* ARLine :  Carry the property and role to add & remove point to draw line when touching the screen to the line renderer.
* ColorPicking :  Responsible for the color picker feature adn update the color to the line material.
* UIController :  Responsible for Screen Recording for click event on start and stop Recording button and calls AndroidUtils class functions.


## How to setup the project on your local machine:
* Make sure you have [UNITY 3D](https://unity3d.com/get-unity/download) and [Android Studio](https://developer.android.com/studio?gclsrc=ds&gclsrc=ds&gclid=CKLO17GTuvICFQOVjgod_mED4A) installed on your machine.
* Clone [this](https://github.com/srvsingh1962/ARDraw-MM4ME) and [Mapmint4ME](https://github.com/srvsingh1962/MapMint4ME) Repository at your desired same folder.

   ![Screenshot 2021-08-18 134911](https://user-images.githubusercontent.com/56843128/129863906-dc952055-d623-4beb-ac2e-e1f61cd1a901.png)
* Now you can open the UNITY project and make sure Android pletform is selected and as we are building this project as library make sure the Export project is selected and Project setting> Player >  Other Setting > Configuration > Scripting Backend is IL2CPP.

   ![Screenshot 2021-08-18 135250](https://user-images.githubusercontent.com/56843128/129864725-e51b710b-10c9-4f22-904c-b3f3dde6abc5.png)

* After you Export the project in ARDraw-MM4ME > ARDraw > AndroidBuild folder open the MM4ME Android project with Android Studio and Sync the project with Gradle files.


Note: In the Android Build folder in UNITY project you can see one folder name as unityLibrary and that is the folder we are using as library in our MM4ME project. Make sure the folder are at the right places and below given link is valid in your project to sync the project with gradle files.

   ![Capture](https://user-images.githubusercontent.com/56843128/130348426-62126bf9-6408-407b-a2bc-d1e6930f0c14.PNG)


Happy Coding :)
