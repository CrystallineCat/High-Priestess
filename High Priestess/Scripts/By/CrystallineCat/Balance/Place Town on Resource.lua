Millennia.patchBeforeMapGeneration(function (context)
    context.entityInfos
        :select(function (which) return which:hasTag("BonusTile") end)
        :forEach(function (each) each:addTag("AllowTown") end)
end)
