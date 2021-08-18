# ARDraw-MM4ME
This Project is about AR Draw + Measure Experience in UNITY 3D with the below-mentioned features and Adding that to the MM4ME android project as a library:

* Drawing over the recognized plane (Horizontal or Vertical Plane).
* Line width adjuster.
* Video Recording while drawing.
* Multicolor options for lines.
* One or multiple fingers touch draw ability.

## How to setup the project on your local machine:
* Make sure you have [UNITY 3D](https://unity3d.com/get-unity/download) and [Android Studio](https://developer.android.com/studio?gclsrc=ds&gclsrc=ds&gclid=CKLO17GTuvICFQOVjgod_mED4A) installed on your machine.
* Clone this and [Mapmint4ME](https://github.com/srvsingh1962/MapMint4ME) Repository at your desired same folder.

   ![Screenshot 2021-08-18 134911](https://user-images.githubusercontent.com/56843128/129863906-dc952055-d623-4beb-ac2e-e1f61cd1a901.png)
* Now you can open the UNITY project and make sure Android pletform is selected and as we are building this project as library make sure the Export project is selected and Project setting> Player >  Other Setting > Configuration > Scripting Backend is IL2CPP.

   ![Screenshot 2021-08-18 135250](https://user-images.githubusercontent.com/56843128/129864725-e51b710b-10c9-4f22-904c-b3f3dde6abc5.png)

* After you Export the project in ARDraw-MM4ME > ARDraw > AndroidBuild folder open the Android project with Android Studio and Sync the project with Gradle files.

Happy Coding :)
