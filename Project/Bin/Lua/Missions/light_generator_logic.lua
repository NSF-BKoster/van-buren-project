function OnInit(instance)
	instance.OnInteract:Add(use);
	instance.OnStatusChanged:Add(statusChanged);
end 

function use(prejudicial)
	if  IsDisabled() == false then 
		local light = GetEntityByName("Light_1");
		local isOn = GetProperty(light,"AllowDynamicLighting");
		if isOn then isOn = false else isOn = true end
		
		SetProperty( light, "AllowDynamicLighting", isOn );
	end
end 

function statusChanged(status)
	UnlockAchievement("test");
end