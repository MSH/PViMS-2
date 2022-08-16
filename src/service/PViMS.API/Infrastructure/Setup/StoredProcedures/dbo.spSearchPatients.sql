/**************************************************************************************************************************
**
**	Function: Search for patients.  This function has been ported to a SP to facilitate the searching of custom attributes 
**			  where native support for the XML column type is non-existent
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spSearchPatients]
(
	@UserId int = 0,
	@FacilityId int = 0,
	@PatientId int = 0,
	@FirstName varchar(30) = NULL,
	@LastName varchar(30) = NULL,
	@CaseNumber varchar(50) = NULL,
	@DateOfBirth date = NULL,
	@CustomAttributeKey varchar(MAX) = NULL,
	@CustomPath varchar(25) = NULL,
	@CustomValue varchar(MAX) = NULL
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spSearchPatients
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
**	2021-10-20		2			SIK			Cater for facilityID of 0 - check user permissions
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
				AND ((@FacilityId > 0 AND pf.Facility_Id = @FacilityId) OR (@FacilityId = 0 AND pf.Facility_Id IN (SELECT f.Id FROM UserFacility uf INNER JOIN Facility f ON uf.Facility_Id = f.Id WHERE uf.User_Id = @UserId)))
				AND ((@PatientId > 0 AND p.Id = @PatientId) OR (@PatientId = 0 AND 1 = 1))
				AND ((@FirstName IS NOT NULL AND p.FirstName LIKE @FirstName) OR (@FirstName IS NULL AND 1 = 1))
				AND ((@LastName IS NOT NULL AND p.Surname LIKE @LastName) OR (@LastName IS NULL AND 1 = 1))
				AND ((@CaseNumber IS NOT NULL AND EXISTS(SELECT Id FROM PatientCondition pc WHERE pc.Patient_Id = p.id AND pc.CaseNumber = @CaseNumber)) OR (@CaseNumber IS NULL AND 1 = 1))
				AND ((@DateOfBirth IS NOT NULL AND p.DateOfBirth = @DateOfBirth) OR (@DateOfBirth IS NULL AND 1 = 1))
				AND ((@CustomValue IS NOT NULL AND X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = @CustomAttributeKey AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)')  LIKE @CustomValue) OR (@CustomValue IS NULL AND 1 = 1))
		ORDER BY p.Id desc
	END 
	ELSE BEGIN
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
			WHERE p.Archived = 0
				AND ((@FacilityId > 0 AND pf.Facility_Id = @FacilityId) OR (@FacilityId = 0 AND pf.Facility_Id IN (SELECT f.Id FROM UserFacility uf INNER JOIN Facility f ON uf.Facility_Id = f.Id WHERE uf.User_Id = @UserId)))
				AND ((@PatientId > 0 AND p.Id = @PatientId) OR (@PatientId = 0 AND 1 = 1))
				AND ((@FirstName IS NOT NULL AND p.FirstName LIKE @FirstName) OR (@FirstName IS NULL AND 1 = 1))
				AND ((@LastName IS NOT NULL AND p.Surname LIKE @LastName) OR (@LastName IS NULL AND 1 = 1))
				AND ((@CaseNumber IS NOT NULL AND EXISTS(SELECT Id FROM PatientCondition pc WHERE pc.Patient_Id = p.id AND pc.CaseNumber = @CaseNumber)) OR (@CaseNumber IS NULL AND 1 = 1))
				AND ((@DateOfBirth IS NOT NULL AND p.DateOfBirth = @DateOfBirth) OR (@DateOfBirth IS NULL AND 1 = 1))
		ORDER BY p.Id desc
	END

END