--[[
	Mission 01 - Find out who's been stealing from Anson
	
	-All mission files must have the two variables declared as below
--]]

function OnInit(instance)
	instance.OnStatusChanged:Add(StatusChanged)
end 

function StatusChanged(value)
end 

function GetExp()
	return 500; 
end

function GetDBIndex()
	return 1;
end