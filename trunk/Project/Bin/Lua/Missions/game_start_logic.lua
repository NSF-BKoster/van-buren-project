--[[
	GAME START LOGIC - GIVE MANE MISSION AND RESET SPAWN LOGIC TO NULL
	
--]]

function OnInit(instance)
	instance.OnSpawnUnit:Add(UnitSpawn)
end 

function UnitSpawn(unit)
	if unit:IsPlayerUnit() then
		unit.Intellect:AttemptAddObjective("mission00");
		SetProperty(unit, "ScriptLogic", "");
	end
end 