--[[
	ENTITY EVENTS
		- OnInteract (activator)
		- OnDeath (prejudicial)
		
	ALL INTERACTABLE ENTITY FUNCTIONS HAVE BEEN EXPOSED TO LUA
--]]

function OnInit(instance)
	instance.OnInteract:Add(use)
end 

function use(prejudicial)
	local status = prejudicial.Intellect:ObjectiveStatus("mission02");
	if status <> -1 then
		prejudicial.Intellect:SetObjectiveStatus("mission02", status + 1);
		PrintText("You fixed another broken part.");
	end
end 