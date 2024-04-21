Millennia.patchNewGame(function (context)
    context.entityInfos
        :select(function (which) return which:hasData("UpgradeLine-TradePost") end)
        :forEach(function (each) each:addTag("BuildRequirementTag-Forest") end)
    
    -- This works, but the trade post doesn't provide any resource...
end)
