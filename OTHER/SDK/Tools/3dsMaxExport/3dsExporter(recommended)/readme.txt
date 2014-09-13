OGRE 3D Studio Max Exporter (Adapted for NeoAxis Engine)
http://www.ogre3d.org
http://www.neoaxisgroup.com

----
To install the 3ds exporter, do the following:

1. If you have a previous version of the exporter, delete all your previous 
   files and remove any existing toolbars / buttons before installing the new 
   one. 
2. Copy the folders "OgreXMLConverter" and "scripts" from folder 
   "To 3dsMax Directory" into your 3DSMax folder. (with overwrite).
3. Open your 3DSMAX and run MAXScript. (choose MAXScript/Run Script...)
4. Go to folder 3DSMAX\scripts\orge\macros and run "ogreToolbar.mcr"
5. After you run the "ogreToolbar.mcr" there is nothing appears on screen yet. 
   You just need to go to /Customize/Customize User Interface... 
   Choose Ogre Tools from Category. Under the list there will be 
   (maybe 3 icons) "Ogre Exporter" icon shows up.
6. Create a new tab with the name "Orge Exporter" (or whatever you want) and 
   drag the icon to the tab.
7. That's it. You are able to run the exporter. Everything you need is in that 
   little icon. 

----
How to use the 3DS exporter

Please read 'install.txt' for information on how to install the exporter.

Features of the exporter:
exports meshes
exports bones, vertex assignements and bones animation, using the skin modifier
exports .material files, however you are better to use an OGRE .material script 
to define them, since you can then use the full OGRE material properties.

----
How to use it

1. In the 'Ogre Tools' toolbar, click the 'Export to OGRE' button
2. Click the 'Select A Mesh' button and pick the mesh you wish to export
3. Tick the'Export Mesh' checkbox if you want to export the geometry of the object
4. Tick the 'Export Skeleton' box if you want to export the skeleton and 
   animations. Specify the name you want to give the animation, the range and duration.
5. Specify an output file name which will be used as a base for all exported files
6. Click 'Export'

----
