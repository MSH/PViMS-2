DECLARE @Id int
SELECT @Id = Id   FROM Dataset  WHERE DatasetName = 'E2B(R2) ICH Report'

SET NOCOUNT ON

BEGIN TRAN A1

	-- delete sub values
	DELETE disv 
		FROM DatasetInstance di
			INNER JOIN DatasetInstanceValue div ON di.Id = div.DatasetInstance_Id 
			INNER JOIN DatasetInstanceSubValue disv ON div.Id = disv.DatasetInstanceValue_Id
			INNER JOIN Dataset d ON di.Dataset_Id = d.Id 
		WHERE d.Id = @Id
	
	-- delete instance values
	DELETE div 
		FROM DatasetInstance di
			INNER JOIN DatasetInstanceValue div ON di.Id = div.DatasetInstance_Id 
			INNER JOIN Dataset d ON di.Dataset_Id = d.Id 
		WHERE d.Id = @Id
	
	-- delete instances
	DELETE di
		FROM DatasetInstance di
			INNER JOIN Dataset d ON di.Dataset_Id = d.Id 
		WHERE d.Id = @Id
	
	-- delete work plan associated to dataset
	DELETE etwp from EncounterTypeWorkPlan etwp inner join WorkPlan wp on etwp.WorkPlan_Id = wp.Id where wp.Dataset_Id = @id
	DELETE wpdc from WorkPlanCareEventDatasetCategory wpdc inner join WorkPlanCareEvent wpce on wpdc.WorkPlanCareEvent_id = wpce.Id inner join WorkPlan wp on wpce.WorkPlan_Id = wp.Id where wp.Dataset_Id = @id
	DELETE wpce from WorkPlanCareEvent wpce inner join WorkPlan wp on wpce.WorkPlan_Id = wp.Id where wp.Dataset_Id = @id
	DELETE WorkPlan where Dataset_Id = @id

	-- delete xml structure
	DELETE dsxa from Dataset ds inner join DatasetXml dsx on ds.DatasetXml_Id = dsx.Id inner join DatasetXmlNode dsxn on dsxn.DatasetXml_Id = dsx.Id inner join DatasetXmlAttribute dsxa on dsxn.Id = dsxa.ParentNode_Id  WHERE ds.Id = @Id 
	DELETE dsxn from Dataset ds inner join DatasetXml dsx on ds.DatasetXml_Id = dsx.Id inner join DatasetXmlNode dsxn on dsxn.DatasetXml_Id = dsx.Id WHERE ds.Id = @Id 

	-- delete mappings	
	DELETE dmv from DatasetMapping dm inner join DatasetMappingValue dmv on dm.Id = dmv.Mapping_Id inner join DatasetCategoryElement dce on dm.DestinationElement_Id = dce.Id inner join DatasetCategory dc on dce.DatasetCategory_Id = dc.Id WHERE dc.Dataset_Id = @Id
	DELETE dms from DatasetMappingSub dms inner join DatasetMapping dm on dms.Mapping_Id = dm.Id inner join DatasetCategoryElement dce on dm.DestinationElement_Id = dce.Id inner join DatasetCategory dc on dce.DatasetCategory_Id = dc.Id WHERE dc.Dataset_Id = @Id
	DELETE dm from DatasetMapping dm inner join DatasetCategoryElement dce on dm.DestinationElement_Id = dce.Id inner join DatasetCategory dc on dce.DatasetCategory_Id = dc.Id WHERE dc.Dataset_Id = @Id

	-- remove dataset elements
	declare @elements table (id int)
	insert into @elements (id) select de.Id from DatasetElement de left join DatasetCategoryElement dce on de.Id = dce.DatasetElement_Id where dce.Id is null and de.ElementName not in ('TerminologyMedDra', 'SuspectDrug1_Naranjo', 'SuspectDrug1_WHO', 'SuspectDrug2_Naranjo', 'SuspectDrug2_WHO', 'SuspectDrug3_Naranjo', 'SuspectDrug3_WHO', 'ConcomitantDrug1_Naranjo', 'ConcomitantDrug1_WHO', 'ConcomitantDrug2_Naranjo', 'ConcomitantDrug2_WHO', 'ConcomitantDrug3_Naranjo', 'ConcomitantDrug3_WHO')
	DELETE DatasetRule WHERE DatasetElement_Id in (select id from @elements)
	DELETE FieldValue where Field_Id in (select Field_Id from DatasetElement where Id in (select id from @elements))
	DELETE ds FROM DatasetElementSub ds INNER JOIN DatasetElement de ON ds.DatasetElement_Id = de.Id where de.Id in (select id from @elements)
	DELETE DatasetElement where Id in (select id from @elements)
	DELETE Field where Id in (select Field_Id from DatasetElement where Id in (select id from @elements))

	-- remove dataset
	DELETE dce from DatasetCategoryElement dce inner join DatasetCategory dc on dce.DatasetCategory_Id = dc.Id WHERE dc.Dataset_Id = @Id
	DELETE dcc from DatasetCategoryCondition dcc inner join DatasetCategory dc on dcc.DatasetCategory_Id = dc.Id WHERE dc.Dataset_Id = @Id
	DELETE DatasetCategory WHERE Dataset_Id = @Id
	DELETE DatasetRule WHERE Dataset_Id = @Id
	DELETE Dataset  WHERE Id = @Id

	-- remove xml dataset
	DELETE dsx from Dataset ds inner join DatasetXml dsx on ds.DatasetXml_Id = dsx.Id WHERE ds.Id = @Id 
	
	--ROLLBACK TRAN A1
COMMIT TRAN A1

