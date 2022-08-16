/**************************************************************************************************************************
**
**	Function: Search for encounters.  This function has been ported to a SP to facilitate the searching of custom attributes 
**			  where native support for the XML column type is non-existent
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spSearchEncounters]
(
	@FacilityId int = 0,
	@PatientId int = 0,
	@FirstName varchar(30) = NULL,
	@LastName varchar(30) = NULL,
	@SearchFrom date = NULL,
	@SearchTo date = NULL,
	@CustomAttributeKey varchar(MAX) = NULL,
	@CustomPath varchar(25) = NULL,
	@CustomValue varchar(MAX) = NULL
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spSearchEncounters
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
**	Date: 13 May 2020
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

	IF (@FirstName IS NOT NULL) BEGIN
		SET @FirstName = '%' + @FirstName + '%'
	END
	IF (@LastName IS NOT NULL) BEGIN
		SET @LastName = '%' + @LastName + '%'
	END
	IF (@CustomValue IS NOT NULL) BEGIN
		SET @CustomValue = '%' + @CustomValue + '%'
	END
	SET @CustomAttributeKey = RTRIM(@CustomAttributeKey)
	SET @CustomPath = RTRIM(@CustomPath)

	IF (@CustomValue IS NOT NULL) BEGIN
		SELECT	e.Id AS EncounterId,
				p.Id as PatientId,
				p.FirstName,
				p.Surname,
				f.FacilityName,
				et.Description AS EncounterType,
				CONVERT(VARCHAR(10), e.EncounterDate, 101) AS EncounterDate
			FROM Encounter e
				INNER JOIN Patient p ON e.Patient_Id = p.Id
				INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
				INNER JOIN Facility f ON pf.Facility_Id = f.Id
				INNER JOIN EncounterType et ON e.EncounterType_Id = et.Id
			CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y)
			WHERE e.Archived = 0 AND p.Archived = 0 
				AND ((@FacilityId > 0 AND pf.Facility_Id = @FacilityId) OR (@FacilityId = 0 AND 1 = 1))
				AND ((@PatientId > 0 AND p.Id = @PatientId) OR (@PatientId = 0 AND 1 = 1))
				AND ((@FirstName IS NOT NULL AND p.FirstName LIKE @FirstName) OR (@FirstName IS NULL AND 1 = 1))
				AND ((@LastName IS NOT NULL AND p.Surname LIKE @LastName) OR (@LastName IS NULL AND 1 = 1))
				AND ((@SearchFrom IS NOT NULL AND e.EncounterDate >= @SearchFrom) OR (@SearchFrom IS NULL AND 1 = 1))
				AND ((@SearchTo IS NOT NULL AND e.EncounterDate <= @SearchTo) OR (@SearchTo IS NULL AND 1 = 1))
				AND ((@CustomValue IS NOT NULL AND X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = @CustomAttributeKey AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)')  LIKE @CustomValue) OR (@CustomValue IS NULL AND 1 = 1))
	END 
	ELSE BEGIN
		SELECT	e.Id AS EncounterId,
				p.Id as PatientId,
				p.FirstName,
				p.Surname,
				f.FacilityName,
				et.Description AS EncounterType,
				CONVERT(VARCHAR(10), e.EncounterDate, 101) AS EncounterDate
			FROM Encounter e
				INNER JOIN Patient p ON e.Patient_Id = p.Id
				INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
				INNER JOIN Facility f ON pf.Facility_Id = f.Id
				INNER JOIN EncounterType et ON e.EncounterType_Id = et.Id
			WHERE e.Archived = 0 AND p.Archived = 0 
				AND ((@FacilityId > 0 AND pf.Facility_Id = @FacilityId) OR (@FacilityId = 0 AND 1 = 1))
				AND ((@PatientId > 0 AND p.Id = @PatientId) OR (@PatientId = 0 AND 1 = 1))
				AND ((@FirstName IS NOT NULL AND p.FirstName LIKE @FirstName) OR (@FirstName IS NULL AND 1 = 1))
				AND ((@LastName IS NOT NULL AND p.Surname LIKE @LastName) OR (@LastName IS NULL AND 1 = 1))
				AND ((@SearchFrom IS NOT NULL AND e.EncounterDate >= @SearchFrom) OR (@SearchFrom IS NULL AND 1 = 1))
				AND ((@SearchTo IS NOT NULL AND e.EncounterDate <= @SearchTo) OR (@SearchTo IS NULL AND 1 = 1))
	END

END