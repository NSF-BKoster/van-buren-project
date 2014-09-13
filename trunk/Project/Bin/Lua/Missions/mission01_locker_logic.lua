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
	if prejudicial.Intellect:ObjectiveStatus("mission01") == 0 then
		GetParentEntity():AddNewObject("StimPak");
		prejudicial.Intellect:SetObjectiveStatus("mission01", 1);
		PrintText("You found one of Anson's drugs. Looks like Marienne has some explaining to do.");
	end
end 