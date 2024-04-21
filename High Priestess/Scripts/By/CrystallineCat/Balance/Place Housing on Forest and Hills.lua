Millennia.patchNewGame(function (context)
    context.entityInfos
        :select(function (which) return which:hasData("UpgradeLine-Housing") end)
        :forEach(function (each) each:addTag("BuildRequirementTag-Forest") end)
        :forEach(function (each) each:addTag("BuildRequirementTag-Hills") end)
        
    context.entityInfos
        :select(function (which) return which:hasData("UpgradeLine-Apartment") end)
        :forEach(function (each) each:addTag("BuildRequirementTag-Hills") end)
end)
