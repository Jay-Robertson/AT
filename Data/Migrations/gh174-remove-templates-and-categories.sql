-- remove 'glovebag' template
declare @templateId as Int = (select Id from Template where Name = 'Glove bag')
delete from TemplatesAssets where Template_Id = @templateId
delete from Template where Id = @templateId

/* Delete Item categories: Airless Equipment, Duct Equip / Materials, Flooring Equip Accessories,
Monitoring Equip., Power Tools, Power Tools Material, Fastners Materials, Misc Equip, Misc Material,
Dye. */

update Assets set Category_Id = null
from Assets a
join AssetCategory c on c.Id = a.Category_Id
where c.Name in (
	'AIRLESS EQUIPMENT',
	'DUCT EQUIP/MATERIALS',
	'FLOORING EQUIPMENT ACCESSORIES',
	'MONITORING EQUIPMENT',
	'POWER TOOLS',
	'POWER TOOL MATERIAL',
	'FASTENER MATERIAL',
	'MISC EQUIPMENT',
	'MISCELLANEOUS MATERIALS',
	'DYE')

delete from AssetCategory where Name in (
	'AIRLESS EQUIPMENT',
	'DUCT EQUIP/MATERIALS',
	'FLOORING EQUIPMENT ACCESSORIES',
	'MONITORING EQUIPMENT',
	'POWER TOOLS',
	'POWER TOOL MATERIAL',
	'FASTENER MATERIAL',
	'MISC EQUIPMENT',
	'MISCELLANEOUS MATERIALS',
	'DYE')