/**************************************************************************************************************************
**
**	Function: Search for appointments.  This function has been ported to a SP to facilitate the searching of custom attributes 
**			  where native support for the XML column type is non-existent
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spSearchAppointments]
(
	@FacilityId int = 0,
	@PatientId int = 0,
	@CriteriaId int = 2,
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
**	Name: dbo.spSearchAppointments
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
		SELECT	a.Id AS AppointmentId,
				p.Id as PatientId,
				ISNULL(e.Id, 0) as EncounterId,
				p.FirstName,
				p.Surname,
				f.FacilityName,
				CONVERT(VARCHAR(10), a.AppointmentDate, 101) AS AppointmentDate,
				a.Reason,
				CASE WHEN a.Cancelled = 1 THEN 'Cancelled' WHEN e.Id IS NOT NULL THEN 'Appointment met' WHEN e.Id IS NULL AND DATEADD(dd, 3, a.AppointmentDate) >= GETDATE() THEN 'Appointment' WHEN e.Id IS NULL AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 0 THEN 'MISSED' WHEN e.Id IS NULL AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 1 THEN 'Did Not Arrive' END AS AppointmentStatus
			FROM Appointment a
				INNER JOIN Patient p ON a.Patient_Id = p.Id
				INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
				INNER JOIN Facility f ON pf.Facility_Id = f.Id
				LEFT JOIN Encounter e ON e.Patient_Id = p.Id AND e.EncounterDate >= DATEADD(dd, 1, a.AppointmentDate) AND e.EncounterDate <= DATEADD(dd, 5, a.AppointmentDate)
			CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y)
			WHERE a.Archived = 0 AND p.Archived = 0 
				AND ((@CriteriaId = 3 AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 0 AND e.Id IS NULL) OR (@CriteriaId = 4 AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 1 AND e.Id IS NULL) OR (@CriteriaId = 2 AND 1 = 1) OR (@CriteriaId = 5 AND e.Id IS NOT NULL) OR (@CriteriaId = 6 AND a.Cancelled = 0))
				AND ((@FacilityId > 0 AND pf.Facility_Id = @FacilityId) OR (@FacilityId = 0 AND 1 = 1))
				AND ((@PatientId > 0 AND p.Id = @PatientId) OR (@PatientId = 0 AND 1 = 1))
				AND ((@FirstName IS NOT NULL AND p.FirstName LIKE @FirstName) OR (@FirstName IS NULL AND 1 = 1))
				AND ((@LastName IS NOT NULL AND p.Surname LIKE @LastName) OR (@LastName IS NULL AND 1 = 1))
				AND ((@SearchFrom IS NOT NULL AND a.AppointmentDate >= @SearchFrom) OR (@SearchFrom IS NULL AND 1 = 1))
				AND ((@SearchTo IS NOT NULL AND a.AppointmentDate <= @SearchTo) OR (@SearchTo IS NULL AND 1 = 1))
				AND ((@CustomValue IS NOT NULL AND X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = @CustomAttributeKey AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)')  LIKE @CustomValue) OR (@CustomValue IS NULL AND 1 = 1))
	END 
	ELSE BEGIN
		SELECT	a.Id AS AppointmentId,
				p.Id as PatientId,
				ISNULL(e.Id, 0) as EncounterId,
				p.FirstName,
				p.Surname,
				f.FacilityName,
				CONVERT(VARCHAR(10), a.AppointmentDate, 101) AS AppointmentDate,
				a.Reason,
				CASE WHEN a.Cancelled = 1 THEN 'Cancelled' WHEN e.Id IS NOT NULL THEN 'Appointment met' WHEN e.Id IS NULL AND DATEADD(dd, 3, a.AppointmentDate) >= GETDATE() THEN 'Appointment' WHEN e.Id IS NULL AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 0 THEN 'MISSED' WHEN e.Id IS NULL AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 1 THEN 'Did Not Arrive' END AS AppointmentStatus
			FROM Appointment a
				INNER JOIN Patient p ON a.Patient_Id = p.Id
				INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
				INNER JOIN Facility f ON pf.Facility_Id = f.Id
				LEFT JOIN Encounter e ON e.Patient_Id = p.Id AND e.EncounterDate >= DATEADD(dd, 1, a.AppointmentDate) AND e.EncounterDate <= DATEADD(dd, 5, a.AppointmentDate)
			WHERE a.Archived = 0 AND p.Archived = 0 
				AND ((@CriteriaId = 3 AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 0 AND e.Id IS NULL) OR (@CriteriaId = 4 AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 1 AND e.Id IS NULL) OR (@CriteriaId = 2 AND 1 = 1) OR (@CriteriaId = 5 AND e.Id IS NOT NULL) OR (@CriteriaId = 6 AND a.Cancelled = 0))
				AND ((@FacilityId > 0 AND pf.Facility_Id = @FacilityId) OR (@FacilityId = 0 AND 1 = 1))
				AND ((@PatientId > 0 AND p.Id = @PatientId) OR (@PatientId = 0 AND 1 = 1))
				AND ((@FirstName IS NOT NULL AND p.FirstName LIKE @FirstName) OR (@FirstName IS NULL AND 1 = 1))
				AND ((@LastName IS NOT NULL AND p.Surname LIKE @LastName) OR (@LastName IS NULL AND 1 = 1))
				AND ((@SearchFrom IS NOT NULL AND a.AppointmentDate >= @SearchFrom) OR (@SearchFrom IS NULL AND 1 = 1))
				AND ((@SearchTo IS NOT NULL AND a.AppointmentDate <= @SearchTo) OR (@SearchTo IS NULL AND 1 = 1))
	END

END