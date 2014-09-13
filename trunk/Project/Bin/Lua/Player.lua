--[[---------------------------------------------------------
    VAN BUREN PROJECT // PLAYER PROFILE
-----------------------------------------------------------]]
dofile "Lua\\GenericUnit.lua"

--[[---------------------------------------------------------
   Desc: PLAYER ONLY // SKILLS CODE
-----------------------------------------------------------]]


function firstaid(source, entity)
	entity:Die();
end

function lockpick(source, entity)

	
	
end

function repair(source, entity)
end

function science(source, entity)
end

function sneak(source, entity)
end

function steal(source, entity)
	source:ExecuteCommand("createWindow inventory");
end

function Traps(source, entity)
end


--[[---------------------------------------------------------
   Desc: DEFAULT SKILLS // COMBAT
-----------------------------------------------------------]]
function firearms()
	return 5 + (4 * GetCharStat("agility")) 
end

function melee()
	return 55 + ((GetCharStat("strength")+GetCharStat("agility")) /2) 
end

function unarmed()
	return 40 + ((GetCharStat("strength")+GetCharStat("agility")) /2)
end


--[[---------------------------------------------------------
   Desc: DEFAULT SKILLS // PASSIVE
-----------------------------------------------------------]]
function charisma()
	return 20 + (2*GetCharStat("charisma"))
end

function barter()
	return 20 + (2*GetCharStat("charisma"))
end

function speech()
	return 25 + (2*GetCharStat("charisma")) 
end
