OGRE COMMAND-LINE UTILITIES
===========================

This archive contains a few prebuilt command-line tools for manipulating your
media. For further info, visit http://www.ogre3d.org

OgreXMLConverter
----------------
Converts between the binary and XML formats for .mesh and .skeleton. Will also 
allow you to generate LOD information if you are converting to the binary 
format. This tool is necessary to convert from the XML to OGRE's native runtime
format if your exporter produces XML. You can find the XML Schema for the .mesh
and .skeleton formats in the Ogre source under Tools/XMLConverter/docs.

Usage: 

OgreXMLConverter [-i] [-e] [-t] [-l lodlevels] [-d loddist]
                [[-p lodpercent][-f lodnumtris]] sourcefile [destfile]
-i             = interactive mode - prompt for options
(The next 6 options are only applicable when converting XML to Mesh)
-l lodlevels   = number of LOD levels
-d loddist     = distance increment to reduce LOD
-p lodpercent  = Percentage triangle reduction amount per LOD
-f lodnumtris  = Fixed vertex reduction per LOD
-e             = DO NOT generate edge lists, ie disable stencil shadows
-t             = Generate tangent-space vectors (for normal mapping)
sourcefile     = name of file to convert
destfile       = optional name of file to write to. If you don't
                 specify this OGRE works it out through the extension
                 and the XML contents if the source is XML. For example
                 test.mesh becomes test.xml, test.xml becomes test.mesh
                 if the XML document root is <mesh> etc.

Because the default behaviour is to convert binary to XML and vice versa, you 
can simply drag files onto this converter and it will convert between the 2 
formats, although you will not be able to use it to generate LOD levels this
way.

