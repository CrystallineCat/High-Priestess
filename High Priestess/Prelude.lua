Millennia = {
	mod = _MILLENNIA.mod,
    setDevConfig =
        function (values)
            for key, value in pairs(values) do
                _DEV_CONFIG(key, tostring(value))
            end
        end,
    setGameValues = 
        function (values)
            Millennia.patch("APlayer", "LoadGameValues", function (self)
                for key, value in pairs(values) do
                    self:SetGameValue(key, value)
                end
            end)
        end,
	patchIn =
		function (assembly, type, method, callback)
			_MILLENNIA.patch(assembly, type, method, callback)
		end,
	patch =
		function (type, method, callback)
			_MILLENNIA.patch("Assembly-CSharp", "CPrompt." .. type, method, callback)
		end,    
    patchNewGame = 
        function (callback)
            Millennia.patch("AGame", "DoPregamePrep", function (self, args)
                if args.newGame then
                    callback({
                        primitive = self,
                        entityInfos = Millennia.entityInfos():all(),
                    })
                end
            end)
        end,
    patchBeforeMapGeneration = 
        function (callback)
            Millennia.patch("AMapController", "SetupHexGrid", function (self)
                callback({
                    primitive = self,
                    entityInfos = Millennia.entityInfos():all(),
                })
            end)
        end,
	typeOf =
		function (assembly, name)
			return _MILLENNIA.typeOf(assembly, name)
		end,  
    entityInfos =
        function ()
            return {
                _INFOS = _ENTITIES.Instance.EntityInfo,
                _wrap =
                    function (info)
                        return {
                            _INFO = info,
                            id = info.ID,
                            addTag =
                                function (self, tag)
                                    if not self:hasTag(tag) then
                                        self._INFO.Tags.Add(tag)
                                        print("Added tag " .. tag .. " to " .. self.id)
                                    end
                                    return self
                                end,
                            hasTag =
                                function (self, tag)
                                    -- Calling self._INFO.HasTag() seems to fail...
                                    return self._INFO.Tags.Contains(tag)
                                end,
                            hasData =
                                function (self, data)
                                    return self._INFO.HasStartingData(data)
                                end,
                        }
                    end,
                all = 
                    function (self)
                        all = {}
                        for entry in self._INFOS do
                            table.insert(all, self._wrap(entry.Value))
                        end
                        return collection(all)
                    end,
            }
        end,
}

function collection(myTable)
    return {
        myTable = myTable,
        forEach = function (self, callback)
            for n, each in ipairs(self.myTable) do
                callback(each, n)
            end
            return self
        end,
        map = function (self, callback)
            new = {}
            for n, each in ipairs(self.myTable) do
                table.insert(new, callback(each, n))
            end
            return collection(new)
        end,
        select = function (self, predicate)
            found = {}
            for n, each in ipairs(self.myTable) do
                if predicate(each, n) then
                    table.insert(found, each)
                end
            end
            return collection(found)
        end,
        reject = function (self, predicate)
            return self:select(function (each, n) return not predicate(each, n) end)
        end,
    }
end

Millennia.patch("AUIManager", "GetBuildText", function (_, _, version)
	return version .. " (" .. Millennia.mod.name .. " " .. Millennia.mod.version .. ")"
end)
