--[[---------------------------------------------------------
     VAN BUREN PROJECT // GENERIC UNIT PROFILE
-----------------------------------------------------------]]

--[[---------------------------------------------------------
   Basic formula + character gains more maximum hit points for every level equal to 3 + EN/2 rounded down
-----------------------------------------------------------]]
function GetHP(object)
	return 25 + (object:GetCharStat("strength") + (2 * object:GetCharStat("endurance"))) + (object:GetCharStat("level") * (3+math.floor(object:GetCharStat("endurance") / 2)))
end

--[[---------------------------------------------------------
-----------------------------------------------------------]]
function GetAP(object)
	return 5 + (math.floor(object:GetCharStat("agility") / 2))
end


--[[---------------------------------------------------------
-----------------------------------------------------------]]
function GetCarryWeight(object)
	return 25 + 25 * object:GetCharStat("strength")
end


--[[---------------------------------------------------------
-----------------------------------------------------------]]
function PoisonResistance(object)
	return 5 * object:GetCharStat("endurance")
end


--[[---------------------------------------------------------
-----------------------------------------------------------]]
function GetSequence(object)
	return 2 * object:GetCharStat("perception")
end


--[[---------------------------------------------------------
-----------------------------------------------------------]]
function GetCriticalChance(object)
	return object:GetCharStat("luck")
end


--[[---------------------------------------------------------
-----------------------------------------------------------]]
function GetHealingRate(object)
	local var = (1/3) * object:GetCharStat("endurance")
	
	if var < 1 then return 1 else return var
	end
end


--[[---------------------------------------------------------
-----------------------------------------------------------]]
function GetSkillRate(object)
	return (object:GetCharStat("intelligence") * 2) + 5
end


