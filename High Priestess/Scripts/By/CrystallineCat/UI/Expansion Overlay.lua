TMP_Text = Millennia.typeOf("Unity.TextMeshPro", "TMPro.TMP_Text")

Millennia.patch("ARegionOverlayLocation", "UpdateOverlayGameObjects", function (self, args)
	perTilePerTurn = args.forCity:GetExpansionPointsPerTile()
	ratePerTurn = perTilePerTurn / args.forCity:ComputeExpansionCost(self.Location, args.forCity.PlayerNum)

	if self.ExpansionArrows:ContainsKey(args.forCity) then
		self.ExpansionArrows[args.forCity]:GetComponentInChildren(TMP_Text).text = string.format("%.1f%%", ratePerTurn * 100)
	end

	city = self.Location:GetStrongestCityInfluence(args.forCity.PlayerNum)

	if city == nil then return end

	perTilePerTurn = city:GetExpansionPointsPerTile()
	rateDone = self.ExpansionProgress / self.TotalExpansionCost
	
	if perTilePerTurn == 0 then
		turnsText = "never"
	else
		turns = math.ceil((self.TotalExpansionCost - self.ExpansionProgress) / perTilePerTurn)
		if turns == 1 then
			turnsText = "imminent"
		else
			turnsText = string.format("%d turns", turns)
		end
	end
	
	self.ExpansionCostText.text = turnsText .. "\n" .. string.format("%d%%", rateDone * 100)
end)
