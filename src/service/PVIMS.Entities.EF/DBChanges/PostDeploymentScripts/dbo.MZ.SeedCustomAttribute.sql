/**************************************************************************************************************************
**
**	Function: SEED CUSTOM ATTRIBUTES : Mozambique Implementation
**
***************************************************************************************************************************/

INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'Patient', 2, N'Custom', N'Patient File Number', 1, 50, NULL, NULL, 0, 0, 1, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'Patient', 3, N'Custom', N'Gender', 1, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'Patient', 2, N'Custom', N'Address', 1, 100, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'Patient', 2, N'Custom', N'Patient Contact Number', 0, 15, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'Patient', 2, N'Custom', N'Patient Identity Number', 1, 20, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'Patient', 3, N'Custom', N'Identity Type', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientCondition', 3, N'Custom', N'Condition Ongoing', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientLabTest', 2, N'Lab Test', N'Comments', 0, 255, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 3, N'Custom', N'Route', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 3, N'Custom', N'Frequency in days per week', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 3, N'Custom', N'Still On Medication', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 2, N'Custom', N'Indication', 0, 50, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 3, N'Custom', N'Type of Indication', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 3, N'Custom', N'Reason For Stopping', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 3, N'Custom', N'Clinician action taken with regard to medicine if related to AE', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 2, N'Custom', N'Batch Number', 0, 50, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 3, N'Custom', N'Effect OF Dechallenge (D) & Rechallenge (R)', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) 
	VALUES (N'PatientMedication', 2, N'Custom', N'Comments', 0, 100, NULL, NULL, 0, 0, 0, NULL)


--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (32, N'Patient', 3, N'Custom', N'Marital Status', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (33, N'Patient', 3, N'Custom', N'Employment Status', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (34, N'Patient', 2, N'Custom', N'Occupation', 0, 50, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (35, N'Patient', 3, N'Custom', N'Language', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (37, N'Patient', 2, N'Custom', N'Address Line 2', 0, 100, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (38, N'Patient', 2, N'Custom', N'City', 0, 50, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (39, N'Patient', 2, N'Custom', N'State', 0, 50, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (40, N'Patient', 2, N'Custom', N'Postal Code', 0, 10, NULL, NULL, 0, 0, 0, NULL)
--



--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (1, N'PatientClinicalEvent', 2, N'Custom', N'SAE Number', 0, 50, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (2, N'PatientClinicalEvent', 3, N'Custom', N'Outcome', 0, NULL, NULL, NULL, 0, 0, 0, N'For fatal outcomes, please ensure all conditions are updated to reflect the relevant condition outcome')
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (3, N'PatientClinicalEvent', 3, N'Custom', N'Intensity (Severity)', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (4, N'PatientClinicalEvent', 3, N'Custom', N'Severity Grading Scale', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (5, N'PatientClinicalEvent', 3, N'Custom', N'Severity Grade', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (6, N'PatientClinicalEvent', 3, N'Custom', N'Is the adverse event serious?', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (7, N'PatientClinicalEvent', 3, N'Custom', N'Seriousness', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (8, N'PatientClinicalEvent', 4, N'Custom', N'Admission Date', 0, NULL, NULL, NULL, 0, 1, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (9, N'PatientClinicalEvent', 4, N'Custom', N'Discharge Date', 0, NULL, NULL, NULL, 0, 1, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (10, N'PatientClinicalEvent', 4, N'Custom', N'Date of Death', 0, NULL, NULL, NULL, 0, 1, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (11, N'PatientClinicalEvent', 3, N'Custom', N'Autopsy Done', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (12, N'PatientClinicalEvent', 3, N'Custom', N'Was the AE attributed to one or more drugs?', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (13, N'PatientClinicalEvent', 3, N'Custom', N'Was the event reported to national PV?', 0, NULL, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (14, N'PatientClinicalEvent', 2, N'Custom', N'Reported By', 0, 50, NULL, NULL, 0, 0, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (15, N'PatientClinicalEvent', 4, N'Custom', N'Date of Report', 0, NULL, NULL, NULL, 0, 1, 0, NULL)
--INSERT [dbo].[CustomAttributeConfiguration] ([ExtendableTypeName], [CustomAttributeType], [Category], [AttributeKey], [IsRequired], [StringMaxLength], [NumericMinValue], [NumericMaxValue], [FutureDateOnly], [PastDateOnly], [IsSearchable], [AttributeDetail]) VALUES (16, N'PatientClinicalEvent', 2, N'Custom', N'Comments', 0, 100, NULL, NULL, 0, 0, 0, NULL)

INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES (N'Gender', N'', N'')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES (N'Gender', N'M', N'Male')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES (N'Gender', N'F', N'Female')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES (N'Identity Type', N'', N'')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES (N'Identity Type', N'NI', N'National identity')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES (N'Identity Type', N'PN', N'Passport number')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES (N'Identity Type', N'WPN', N'Work permit number')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES (N'Condition Ongoing', N'', N'')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES (N'Condition Ongoing', N'Yes', N'Yes')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) 
	VALUES ( N'Condition Ongoing', N'No', N'No')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'0', N'')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'1', N'Auricular (otic)')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'2', N'Buccal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'3', N'Cutaneous')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'4', N'Dental')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'5', N'Endocervical')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'6', N'Endosinusial')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'7', N'Endotracheal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'8', N'Epidural')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'9', N'Extra-amniotic')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'10', N'Hemodialysis')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'11', N'Intra corpus cavernosum')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'12', N'Intra-amniotic')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'13', N'Intra-arterial')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'14', N'Intra-articular')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'15', N'Intra-uterine')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'16', N'Intracardiac')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'17', N'Intracavernous')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'18', N'Intracerebral')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'19', N'Intracervical')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'20', N'Intracisternal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'21', N'Intracorneal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'22', N'Intracoronary')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'23', N'Intradermal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'24', N'Intradiscal (intraspinal)')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'25', N'Intrahepatic')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'26', N'Intralesional')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'27', N'Intralymphatic')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'28', N'Intramedullar (bone marrow)')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'29', N'Intrameningeal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'30', N'Intramuscular')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'31', N'Intraocular')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'32', N'Intrapericardial')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'33', N'Intraperitoneal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'34', N'Intrapleural')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'35', N'Intrasynovial')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'36', N'Intratumor')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'37', N'Intrathecal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'38', N'Intrathoracic')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'39', N'Intratracheal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'40', N'Intravenous bolus')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'41', N'Intravenous drip')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'42', N'Intravenous (not otherwise specified)')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'43', N'Intravesical')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'44', N'Iontophoresis')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'45', N'Nasal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'46', N'Occlusive dressing technique')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'47', N'Ophthalmic')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'48', N'Oral')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'49', N'Oropharingeal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'50', N'Other')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'51', N'Parenteral 051')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'52', N'Periarticular')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'53', N'Perineural')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'54', N'Rectal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'55', N'Respiratory (inhalation)')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'56', N'Retrobulbar')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'57', N'Sunconjunctival')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'58', N'Subcutaneous')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'59', N'Subdermal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'60', N'Sublingual')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'61', N'Topical')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'62', N'Transdermal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'63', N'Transmammary')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'64', N'Transplacental')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'65', N'Unknown')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'66', N'Urethral')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Route', N'67', N'Vaginal')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Frequency in days per week', N'0', N'')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Frequency in days per week', N'1', N'1')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Frequency in days per week', N'2', N'2')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Frequency in days per week', N'3', N'3')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Frequency in days per week', N'4', N'4')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Frequency in days per week', N'5', N'5')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Frequency in days per week', N'6', N'6')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Frequency in days per week', N'7', N'7')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Still On Medication', N'0', N'')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Still On Medication', N'1', N'Yes')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Still On Medication', N'2', N'No')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Type of Indication', N'0', N'')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Type of Indication', N'1', N'Primary')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Type of Indication', N'2', N'Pre-existing condition')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Type of Indication', N'3', N'Treat AE')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'0', N'')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'1', N'Adverse Event')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'2', N'Cost')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'3', N'Course Completed')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'4', N'Cured')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'5', N'Lost To Follow-up')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'6', N'Medicine Out of Stock')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'7', N'No Longer Needed')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'8', N'Patient Died')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'9', N'Patient Withdrew Consent')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'10', N'Planned Medication Change')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'11', N'Poor Adherence')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'12', N'Pregnancy')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'13', N'Treatment Failure')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Reason For Stopping', N'14', N'Not Applicable')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Clinician action taken with regard to medicine if related to AE', N'0', N'')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Clinician action taken with regard to medicine if related to AE', N'1', N'Dose not changed')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Clinician action taken with regard to medicine if related to AE', N'2', N'Dose reduced')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Clinician action taken with regard to medicine if related to AE', N'3', N'Drug interrupted')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Clinician action taken with regard to medicine if related to AE', N'4', N'Drug withdrawn')
INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Clinician action taken with regard to medicine if related to AE', N'5', N'Not applicable')

--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Outcome', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Outcome', N'1', N'Resolved')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Outcome', N'2', N'Resolved with sequelae')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Outcome', N'3', N'Fatal')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (N'Outcome', N'4', N'Resolving')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (6, N'Outcome', N'5', N'Not resolved')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (7, N'Outcome', N'6', N'Unknown')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (8, N'Intensity (Severity)', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (9, N'Intensity (Severity)', N'1', N'Mild')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (10, N'Intensity (Severity)', N'2', N'Moderate')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (11, N'Intensity (Severity)', N'3', N'Severe')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (12, N'Severity Grading Scale', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (13, N'Severity Grading Scale', N'1', N'WHO Scale')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (14, N'Severity Grading Scale', N'2', N'Clinician�s judgement')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (15, N'Severity Grading Scale', N'3', N'CTCAE grading system')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (16, N'Severity Grading Scale', N'4', N'DAIDS AE Grading Table')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (17, N'Severity Grading Scale', N'5', N'Other')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (18, N'Severity Grade', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (19, N'Severity Grade', N'1', N'Grade 1')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (20, N'Severity Grade', N'2', N'Grade 2')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (21, N'Severity Grade', N'3', N'Grade 3')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (22, N'Severity Grade', N'4', N'Grade 4')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (23, N'Severity Grade', N'5', N'Grade 5')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (24, N'Is the adverse event serious?', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (25, N'Is the adverse event serious?', N'1', N'Yes')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (26, N'Is the adverse event serious?', N'2', N'No')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (27, N'Is the adverse event serious?', N'3', N'Unknown')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (28, N'Seriousness', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (29, N'Seriousness', N'1', N'A congenital anomaly or birth defect')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (30, N'Seriousness', N'2', N'Persistent or significant disability or incapacity')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (31, N'Seriousness', N'3', N'Death')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (32, N'Seriousness', N'4', N'Initial or prolonged hospitalization')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (33, N'Seriousness', N'5', N'Life threatening')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (34, N'Seriousness', N'6', N'A medically important event')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (35, N'Effect OF Dechallenge (D) & Rechallenge (R)', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (36, N'Effect OF Dechallenge (D) & Rechallenge (R)', N'1', N'Not Applicable')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (37, N'Effect OF Dechallenge (D) & Rechallenge (R)', N'2', N'D - AE improved/ resolved when medicine dose reduced/interrupted /withdrawn')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (38, N'Effect OF Dechallenge (D) & Rechallenge (R)', N'3', N'D - AE did not improve/ resolve when medicine dose reduced/interrupted /withdrawn')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (39, N'Effect OF Dechallenge (D) & Rechallenge (R)', N'4', N'D - Unknown')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (40, N'Effect OF Dechallenge (D) & Rechallenge (R)', N'5', N'R - patient not re-exposed to the medicine')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (41, N'Effect OF Dechallenge (D) & Rechallenge (R)', N'6', N'R - AE recurred on medicine re-administration/dose increase')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (42, N'Effect OF Dechallenge (D) & Rechallenge (R)', N'7', N'R - AE did not recur on medicine re-administration/dose increase')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (43, N'Effect OF Dechallenge (D) & Rechallenge (R)', N'8', N'R - Unknown')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (44, N'Was the AE attributed to one or more drugs?', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (45, N'Was the AE attributed to one or more drugs?', N'1', N'Yes')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (46, N'Was the AE attributed to one or more drugs?', N'2', N'No')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (47, N'Was the AE attributed to one or more drugs?', N'3', N'Unknown')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (48, N'Was the event reported to national PV?', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (49, N'Was the event reported to national PV?', N'1', N'Yes')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (50, N'Was the event reported to national PV?', N'2', N'No')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (51, N'Was the event reported to national PV?', N'3', N'Unknown')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (80, N'Marital Status', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (81, N'Marital Status', N'1', N'Single')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (82, N'Marital Status', N'2', N'Married')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (83, N'Marital Status', N'3', N'Divorced')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (84, N'Marital Status', N'4', N'Widowed')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (85, N'Marital Status', N'5', N'Legally Seperated')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (86, N'Employment Status', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (87, N'Employment Status', N'1', N'Employed')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (88, N'Employment Status', N'2', N'Unemployed')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (89, N'Employment Status', N'3', N'Student')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (90, N'Employment Status', N'4', N'N/A')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (91, N'Language', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (92, N'Language', N'1', N'English')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (93, N'Language', N'2', N'French')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (180, N'Autopsy Done', N'0', N'')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (181, N'Autopsy Done', N'1', N'Yes')
--INSERT [dbo].[SelectionDataItem] ([AttributeKey], [SelectionKey], [Value]) VALUES (182, N'Autopsy Done', N'2', N'No')

