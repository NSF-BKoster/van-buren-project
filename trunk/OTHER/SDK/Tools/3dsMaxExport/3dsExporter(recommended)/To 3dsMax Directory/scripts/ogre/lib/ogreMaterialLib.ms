
--!!!!!betauser
function getEngineLocalDirectory outName =
(
	directoryName = getFilenamePath outName;

	index = findString directoryName "Data/";
	if (index == undefined) then
	(
		index = findString directoryName "Data\\";
	)
	if (index == undefined) then
	(
		index = findString directoryName "data/";
	)
	if (index == undefined) then
	(
		index = findString directoryName "data\\";
	)
	
	if (index == undefined) then
	(
		return "";
	)

	index = index + 5;	
	return substring directoryName index -1;
)

-- write into "outfile" the selected OgreMaterial
-- precondition: iskindof material OgreMAterial == true.
-- Use export OgreMaterial.
function writeOgreMaterial material outFile engineLocalDirectory =
(
	local t, matname, mat;

	mat = material ;
	
	if (mat != undefined) then 
	(
		matname = material.name ;	
		format "material %\n" matname to:outFile ;
		format "{\n" to:outFile ;
		format "\ttechnique\n" to:outFile ;
		format "\t{\n" to:outFile ;
		format "\t\tpass\n" to:outFile ;
		format "\t\t{\n" to:outFile ;
		format("\t\t\tambient % % %\n") (mat.ambient.r/255.0) (mat.ambient.g/255.0) (mat.ambient.b/255.0) to:outFile ;
	    format("\t\t\tdiffuse % % %\n") (mat.diffuse.r/255.0) (mat.diffuse.g/255.0) (mat.diffuse.b/255.0) to:outFile ;
    	format("\t\t\tspecular % % % %\n") (mat.specular.r/255.0) (mat.specular.g/255.0) (mat.specular.b/255.0) (mat.specularLevel*100.0) to:outFile ;
		format("\t\t\temissive % % %\n") (mat.selfIllumColor.r/255.0) (mat.selfIllumColor.g/255.0) (mat.selfIllumColor.b/255.0) to:outFile ;
		)
		if ((mat.diffusemap != undefined) and (iskindof mat.diffusemap Bitmaptexture)) then
		(	
			t = filterstring mat.diffusemap.filename "\\" ;
			format "\t\t\ttexture_unit\n" to:outFile ;
			format "\t\t\t{\n" to:outFile ;

--!!!!!!betauser
			format "\t\t\t\ttexture %% \n" engineLocalDirectory t[arrayLength t] to:outFile ;
--			format "\t\t\t\ttexture % \n" prefix t[arrayLength t] to:outFile ;
			format "\t\t\t}\n" to:outFile ;
		)
		format "\t\t}\n" to:outFile ;
		format "\t}\n" to:outFile ;
		format "}\n" to:outFile ;
)

function exportOgreMaterial material outName =
(
    local outFile ;
	if (classof material == OgreMaterial or classof material == ogreMaterial) then
	(
	    outFile = createfile outName ;
		
--!!!!betauser		
		engineLocalDirectory = getEngineLocalDirectory outName;
		
		writeOgreMaterial material outFile engineLocalDirectory;
		close outFile ;
		messageBox "Exporting material successful !" ;
	)
	else
	(
		messageBox "You have to choose an OgreMaterial." ;
	)
)

---------------------------------------------------------------
-- Writes a standard material in a material script
-- prefix will be before material name. (prefix+name)
-- used for multi material
---------------------------------------------------------------
function writeStandardMaterial material prefix outFile engineLocalDirectory =
(
	local t ;
	matname = replaceSpaces material.name ;
	-- writes header	
	format "material %\n" (prefix+matname) to:outFile ;
	format "{\n" to:outFile ;
	format "\ttechnique\n" to:outFile ;
	format "\t{\n" to:outFile ;
	format "\t\tpass\n" to:outFile ;
	format "\t\t{\n" to:outFile ;

--	format("            ambient % % %\n") (material.ambient.r/255.0) (material.ambient.g/255.0) (material.ambient.b/255.0) to:outFile ;
--    format("            diffuse % % %\n") (material.diffuse.r/255.0) (material.diffuse.g/255.0) (material.diffuse.b/255.0) to:outFile ;
--    format("            specular % % % %\n") (material.specular.r/255.0) (material.specular.g/255.0) (material.specular.b/255.0) (material.specularLevel*100.0) to:outFile ;
--	format("            emissive % % %\n") (material.selfIllumColor.r/255.0) (material.selfIllumColor.g/255.0) (material.selfIllumColor.b/255.0) to:outFile ;
	-- tests if there is (or not) a diffuse map.
	if ((material.diffusemap != undefined) and (iskindof material.diffusemap Bitmaptexture)) then
	(	
		t = filterstring material.diffusemap.filename "\\" ;
		format "\t\t\ttexture_unit\n" to:outFile ;
		format "\t\t\t{\n" to:outFile ;
		
--!!!!!!betauser
		format "\t\t\t\ttexture %% \n" engineLocalDirectory t[arrayLength t] to:outFile ;
--		format "\t\t\t\ttexture % \n" t[arrayLength t] to:outFile ;
		format "\t\t\t}\n" to:outFile ;
	)
	format "\t\t}\n" to:outFile ;
	format "\t}\n" to:outFile ;
	format "}\n" to:outFile ;

)

---------------------------------------------------------------
-- Exports a standard material
-- PRE: material is a standard material
-- Use exportMaterial function.
---------------------------------------------------------------
function exportStandardMaterial material outFile engineLocalDirectory =
(
	local sub,i,matname ;
	matname = replaceSpaces material.name ;
	writeStandardMaterial material "" outFile engineLocalDirectory;
)


---------------------------------------------------------------
-- Exports a multi material
-- PRE: material is a multi material
-- Use exportMaterial function.
---------------------------------------------------------------
function exportMultiMaterial material outFile engineLocalDirectory =
(
	local sub,i,matname ;

	matname = replaceSpaces material.name ;
	
	i=0 ;
	for sub in material.materiallist do (
		i+=1 ;
		writeStandardMaterial sub (material.name+"/") outFile engineLocalDirectory;
		format "\n" to:outFile ;
	)
)



---------------------------------------------------------------
-- Exports a material
-- call the fonction linked to the material type
---------------------------------------------------------------
function exportMaterial material outname =
(
	engineLocalDirectory = getEngineLocalDirectory outName;

	local outFile ;
	if (not g_MAX) then
	(
		outFile = createfile outName ;
	)
	else
	(
		ClearListener();
		if (g_MAX_use_listener) then
			format("<ogrestartfile>%</ogrestartfile><ogrestartdata>\n") (outName);
		outFile = listener;
	)
		
	if (material != undefined and classof material == MultiMaterial) then
	(
		exportMultiMaterial material outFile engineLocalDirectory;
	)
	else 
	(
		if (material != undefined and classof material == StandardMaterial) then
		(
			exportStandardMaterial material outFile engineLocalDirectory;
		)
		else
		(
			if (material != undefined and (classof material == OgreMaterial or classof material == ogreMaterial)) then
			(
				writeOgreMaterial material outFile engineLocalDirectory;
			)
			else
			(
				messageBox "You need to choose a Standard Material, MultiMaterial or an OgreMaterial." ;
				return 0;
			)
		)
	)
	
	if (not g_MAX) then
	(
	    close outFile ;
	)
	else
	(
		if (g_MAX_use_listener) then
			format("</ogrestartdata>\n") to: outFile;
	)
	
	messageBox "Exporting material successful !" ;
)


---------------------------------------------------------------
-- Exports all the materials used in the scene
---------------------------------------------------------------
function exportAllSceneMaterials outname =
(
	engineLocalDirectory = getEngineLocalDirectory outName;

	local outFile ;
	if (not g_MAX) then
	(
		outFile = createfile outName ;
	)
	else
	(
		ClearListener();
		if (g_MAX_use_listener) then
			format("<ogrestartfile>%</ogrestartfile><ogrestartdata>\n") (outName);
		outFile = listener;
	)
	
	for material in sceneMaterials do
	(			
		if (material!=undefined and classof material == MultiMaterial) then
			exportMultiMaterial material outFile engineLocalDirectory;
		else (
			if (material!=undefined and classof material == StandardMaterial) then
				exportStandardMaterial material outFile engineLocalDirectory;
		)
		format "\n" to:outFile ;
	)		

	if (not g_MAX) then
	(
	    close outFile ;
	)
	else
	(
		if (g_MAX_use_listener) then
			format("</ogrestartdata>\n") to: outFile;
	)

	messageBox "Exporting all materials in the scene successful !" ;
)

