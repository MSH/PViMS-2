/**************************************************************************************************************************
**
**	Function: Search for patients using an identifier.  This function has been ported to a SP to facilitate the searching of custom attributes 
**			  where native support for the XML column type is non-existent
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spSearchPatientsByIdentifier]
(
	@SearchTerm varchar(MAX) = NULL
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spSearchPatientsByIdentifier
**	Desc: 
**
**	This template can be customized:
**              
**	Return values:
** 
**	Called by:   
**              
**	Parameters:
**	Input				Output
**	----------			-----------
**
**	Auth: S Krog
**	Date: 17 September 2021
**  Current: v1
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:			Version		Author:		Description:
**	--------		--------	-------		-------------------------------------------
**
***************************************************************************************************************************/
BEGIN
	SET NOCOUNT ON

	SET @SearchTerm = '%' + @SearchTerm + '%'

	SELECT	p.Id AS PatientId, 
			p.FirstName, 
			p.Surname, 
			f.FacilityName, 
			ISNULL(CONVERT(VARCHAR(10), p.DateOfBirth, 101), '') AS DateOfBirth,
			CAST(ISNULL(FLOOR(DATEDIFF(DAY, p.DateofBirth, GETDATE()) / 365.25), 0) AS VARCHAR) AS Age,
			ISNULL(CONVERT(VARCHAR(10), (SELECT MAX(EncounterDate) FROM Encounter e WHERE e.Patient_Id = p.Id), 101), '') AS LatestEncounterDate
		FROM Patient p
			INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
			INNER JOIN Facility f ON pf.Facility_Id = f.Id
		CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y)
		WHERE p.Archived = 0
			AND (
					EXISTS(SELECT Id FROM PatientCondition pc WHERE pc.Patient_Id = p.id AND pc.CaseNumber LIKE @SearchTerm)
					OR (X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = 'Medical Record Number' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)') LIKE @SearchTerm)
					OR (X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = 'Patient Identity Number' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)')  LIKE @SearchTerm)
				)
	ORDER BY p.Id desc

END