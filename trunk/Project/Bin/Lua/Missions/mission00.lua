--[[
	Mission 00 - Escape Cell Block 13
	
	-All mission files must have the two variables declared as below
--]]

local inst;
local own;
local handler;

function OnInit(instance, owner)
	if instance:GetStatus() == 0 then
		inst = instance;
		own = owner;
		handler = owner.OnMapTransition:Add(mapTrans);
	end
end 

function mapTrans()
	inst:SetStatus(1);
	own.OnMapTransition:Remove(handler);
end 

function GetExp()
	return 300; 
end

function GetDBIndex()
	return 2;
end
