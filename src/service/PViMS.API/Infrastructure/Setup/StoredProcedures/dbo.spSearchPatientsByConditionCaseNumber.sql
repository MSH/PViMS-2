/**************************************************************************************************************************
**
**	Function: Search for patients by concomitant condition.  This function has been ported to a SP to facilitate the searching of custom attributes 
**			  where native support for the XML column type is non-existent
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spSearchPatientsByConditionCaseNumber]
(
	@CaseNumber varchar(50) = NULL
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spSearchPatientsByConditionCaseNumber
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
**	Date: 6 April 2021
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

	SELECT	p.Id AS PatientId, 
			p.FirstName, 
			p.Surname, 
			f.FacilityName, 
			ISNULL(CONVERT(VARCHAR(10), p.DateOfBirth, 101), '') AS DateOfBirth,
			CAST(ISNULL(FLOOR(DATEDIFF(DAY, p.DateofBirth, GETDATE()) / 365.25), 0) AS VARCHAR) AS Age,
			ISNULL(CONVERT(VARCHAR(10), (SELECT MAX(EncounterDate) FROM Encounter e WHERE e.Patient_Id = p.Id), 101), '') AS LatestEncounterDate
		FROM Patient p
			INNER JOIN PatientCondition pc ON p.Id = pc.Patient_Id
			INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
			INNER JOIN Facility f ON pf.Facility_Id = f.Id
		WHERE p.Archived = 0
			AND ((@CaseNumber IS NOT NULL AND pc.CaseNumber = @CaseNumber) OR (@CaseNumber IS NULL AND 1 = 1))
	ORDER BY p.Id desc
END