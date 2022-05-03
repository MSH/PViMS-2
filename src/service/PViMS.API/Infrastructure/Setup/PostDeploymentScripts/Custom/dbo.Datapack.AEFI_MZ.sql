DECLARE @Id int
SELECT @Id = Id   FROM Dataset  WHERE DatasetName = 'Spontaneous Report'

IF(@Id IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

/**************************************************
DATASET
**************************************************/
DECLARE @dsid int
DECLARE @dscid int
DECLARE @fid int
DECLARE @deid int
DECLARE @dceid int
INSERT [dbo].[Dataset] ([DatasetName], [Active], [InitialiseProcess], [RulesProcess], [Help], [Created], [LastUpdated], [ContextType_Id], [CreatedBy_Id], [UpdatedBy_Id], [IsSystem])
	VALUES ('Spontaneous Report', 1, '', '', 'FICHA DE NOTIFICACAO DE EVENTOS ADVERSOS POS - VACINACAO (EaPV)', GETDATE(), GETDATE(), 4, 1, 1, 1)
set @dsid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Dados do Pacient
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Dados do Paciente', 1, @dsid, 'Dados do Paciente', 'Por favor, insira algumas informa��es sobre a pessoa que teve a rea��o adversa') 
set @dscid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 5, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('C�digo de notifica��o', @fid, 1, '', '', 0, 'DCAF80CF-7033-437E-A64E-6519D0652080') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (1, @dscid, @deid, 0, 0, 'C�digo de notifica��o', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Morada distrito', @fid, 1, '', '', 0, '226BBDFC-FA9C-4A4E-854F-69BED5643083') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (1, @dscid, @deid, 0, 0, 'Morada distrito', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Localidade', @fid, 1, '', '', 0, '231EB572-D2EB-4256-8030-DB22D18B2B74') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (1, @dscid, @deid, 0, 0, 'Localidade', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Avenida', @fid, 1, '', '', 0, '2AA178F4-00E3-4136-A2AE-247588C5790B') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (1, @dscid, @deid, 0, 0, 'Avenida', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Bairro', @fid, 1, '', '', 0, 'AC68B6DC-8BDA-4196-9BB5-47F813A167B7') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (1, @dscid, @deid, 0, 0, 'Bairro', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('1', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('2', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('3', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('4', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Quarterai', @fid, 1, '', '', 0, 'F5CF2C23-BD6C-465C-BA95-F18DDEA8523F') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (6, @dscid, @deid, 1, 0, 'Quarterai', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('N�mero de contato do paciente', @fid, 1, '', '', 0, '42B30798-711E-4803-940F-2CC012ED3751') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (1, @dscid, @deid, 0, 0, 'Contacto (cel./tel.) (+258)', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Macho', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('F�mea', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Sexo', @fid, 1, '', '', 0, 'E061D363-534E-4EA4-B6E5-F1C531931B12') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (8, @dscid, @deid, 0, 0, 'Sexo', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Data nascimento', @fid, 1, '', '', 0, '0D704069-5C50-4085-8FE1-355BB64EF196') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (4, @dscid, @deid, 1, 0, 'Data nascimento', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('< 1 Ano', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('1 a 5 Anos', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('> 5 Ano', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Faixa et�ria', @fid, 1, '', '', 0, '3DE2964B-3E01-425E-9A48-0262D4781CF8') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (8, @dscid, @deid, 0, 0, 'Faixa et�ria', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('> 18 Anos', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('- 60 Anos', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('> 60 Anos', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Ou 18 anos', @fid, 1, '', '', 0, '88364F7F-C80C-464B-B6FE-A537E4CFF071') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (8, @dscid, @deid, 0, 0, 'Ou 18 anos', '') 
set @dceid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Dados do Notificador
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Dados do Notificador', 2, @dsid, 'Dados do Notificador', 'Insira informa��es sobre a pessoa que relata a rea��o') 
set @dscid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 60, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Nome do notificador', @fid, 1, '', '', 0, '926A07E1-8B83-41CA-8949-739717924AD9') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (91, @dscid, @deid, 1, 0, 'Nome ou iniciais da pessoa que relata as informa��es', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('M�dica', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Farmac�utica', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Consumidor ou outro profissional n�o-sa�de', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Outro profissional de sa�de', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Advogada', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Categoria profissional', @fid, 1, '', '', 0, '79ABA1EC-3979-4BA2-80AF-1F817C8243B9') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (94, @dscid, @deid, 1, 0, 'Categoria profissional', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Institui��o', @fid, 1, '', '', 0, 'A517AAE8-76BD-41F0-8FEE-BD45FCE4EBC8') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (98, @dscid, @deid, 1, 0, 'Institui��o', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('N�mero de contato do notificador', @fid, 1, '', '', 0, '1AC02BD6-5C24-4A37-9742-C6B868ED985D') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (93, @dscid, @deid, 0, 0, 'Contacto (cel./tel.) (+258)', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Endere�o de e-mail do notificador', @fid, 1, '', '', 0, 'FFDA770F-DADE-4F6E-B39A-D5E929AEDE2E') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (93, @dscid, @deid, 0, 0, 'Endere�o de e-mail do notificador', '') 
set @dceid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Informa��es de vacina��o
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Informa��es de vacina��o', 2, @dsid, 'Informa��es de vacina��o', 'Por favor, insira informa��es sobre a vacina que voc� suspeita ter causado a rea��o e sobre outros produtos tomados.') 
set @dscid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Informa��es de vacina��o', @fid, 1, '', '', 0, '712CA632-0CD0-4418-9176-FB0B95AEE8A1') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Informa��es de vacina��o', '') 
set @dceid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (1, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Nome comercial da vacina (incluindo fabricante)', @fid, @deid, '', '', 0, 1, 'Nome comercial da vacina (incluindo fabricante)', '') 

		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Data da vacina��o', @fid, @deid, '', '', 0, 8, 'Data da vacina��o', '') 
			
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, 0, 23, 0, '', NULL, '', 0, 4) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Hora de vacina��o', @fid, @deid, '', '', 0, 3, 'Hora de vacina��o', 'Por favor, insira a hora') 

		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, 0, 59, 0, '', NULL, '', 0, 4) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Tempo de minutos de vacina��o', @fid, @deid, '', '', 0, 3, 'Tempo de minutos de vacina��o', 'Por favor, insira a hora') 

		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Primeira dose', 1, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Segunda dose', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Terceira dose', 0, 0, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('N�mero da dose', @fid, @deid, '', '', 0, 6, 'N�mero da dose', '') 

		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Prazo de validade do vacinacao', @fid, @deid, '', '', 0, 9, 'Prazo de validade / No de lote', '') 

		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (1, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Nome do Diluente', @fid, @deid, '', '', 0, 1, 'Nome do Diluente', '') 

		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder, FriendlyName, Help)
			VALUES ('Prazo de validade do diluente', @fid, @deid, '', '', 0, 9, 'Prazo de validade / No de lote', '') 

/**************************************************
CATEGORY Eventos adversos
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Eventos adversos', 3, @dsid, 'Eventos adversos', 'Insira informa��es sobre o que aconteceu e como foi tratado') 
set @dscid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Rea��o local', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Convuls�es', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Abcesso', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Prurido', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('S�ndrome choque toxico', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('V�mito', 0, 1, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Trombocitopenia', 0, 1, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Anafilaxia', 0, 1, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Febre >=38', 0, 1, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Outro', 0, 1, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Dores nas articula��o', 0, 1, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Pr�xima', 0, 1, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Eventos adversos', @fid, 1, '', '', 0, '1A222998-1768-43F2-AF1B-CB17B4E55E1E') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (91, @dscid, @deid, 0, 0, 'Alguma dessas rea��es aconteceu?', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Data da rea��o relatada', @fid, 1, '', '', 0, '49553CB5-8CEB-49BE-BEAA-FCE4E439EE9E') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (85, @dscid, @deid, 0, 0, 'Data da rea��o relatada', 'Data que o evento foi notificado ao servi�o de saude') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 500, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Descri��o da rea��o', @fid, 1, '', '', 0, 'ACD938A4-76D1-44CE-A070-2B8DF0FE9E0F') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (84, @dscid, @deid, 0, 0, 'Descri��o da rea��o', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Risco de vida', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Motivou hospitaliza��o', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Evento m�dico importante', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Malforma��o cong�nita', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Morte', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Gravidade', @fid, 1, '', '', 0, '302C07C9-B0E0-46AB-9EF8-5D5C2F756BF1') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (91, @dscid, @deid, 0, 0, 'Gravidade', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Em recupera��o', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Recuperou', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Recuperou com sequelas', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('N�o recuperou ainda', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Desconhecido', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Morte', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Resultado do evento', @fid, 1, '', '', 0, '976F6C53-78F2-4007-8F39-54057E554EEB') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (95, @dscid, @deid, 0, 0, 'Qual foi o resultado da rea��o?', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Data da morte', @fid, 1, '', '', 0, '8B15C037-9C92-4AD4-A8F4-6C4042D40D9D') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (97, @dscid, @deid, 0, 0, 'Data da morte', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Sim', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('N�o', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Desconhecido', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Autopsia', @fid, 1, '', '', 0, '7430B81E-FC47-479A-8F8F-D516063B60B8') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (95, @dscid, @deid, 0, 0, 'Autopsia', '') 
set @dceid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 500, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid])
	VALUES ('Hist�ria m�dica', @fid, 1, '', '', 0, '24DE20BE-AB13-487B-B679-D62D6FEE8814') 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (94, @dscid, @deid, 0, 0, 'Hist�ria m�dica', 'Hist�ria de reac��o similar ou outras alergias, medi��o concomitante e outras informa��es relevantes. Pode usar uma folha adicional se necess�rios') 
set @dceid = (SELECT @@IDENTITY)

SELECT ds.DatasetName, dc.DatasetCategoryName, de.ElementName FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
where ds.Id = @dsid
	
--ROLLBACK TRAN A1
COMMIT TRAN A1

