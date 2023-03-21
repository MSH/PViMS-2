/**************************************************************************************************************************
**
**	Function: SEED DIMENSIONS
**
***************************************************************************************************************************/

-- prevent set or regional settings from interfering with 
-- interpretation of dates / literals

SET DATEFIRST 7;
SET DATEFORMAT mdy;
SET LANGUAGE US_ENGLISH;

-- this is just a holding table for intermediate callations:

CREATE TABLE dbo.DateDimension
(
  [date]       DATE PRIMARY KEY, 
  [day]        AS DATEPART(DAY,      [date]),
  [month]      AS DATEPART(MONTH,    [date]),
  FirstOfMonth AS CONVERT(DATE, DATEADD(MONTH, DATEDIFF(MONTH, 0, [date]), 0)),
  [MonthName]  AS DATENAME(MONTH,    [date]),
  [week]       AS DATEPART(WEEK,     [date]),
  [ISOweek]    AS DATEPART(ISO_WEEK, [date]),
  [DayOfWeek]  AS DATEPART(WEEKDAY,  [date]),
  [quarter]    AS DATEPART(QUARTER,  [date]),
  [year]       AS DATEPART(YEAR,     [date]),
  FirstOfYear  AS CONVERT(DATE, DATEADD(YEAR,  DATEDIFF(YEAR,  0, [date]), 0)),
  Style112     AS CONVERT(CHAR(8),   [date], 112),
  Style101     AS CONVERT(CHAR(10),  [date], 101)
)

INSERT dbo.DateDimension([date]) 
SELECT d
FROM
(
  SELECT d = DATEADD(DAY, rn - 1, DATEADD(yy, -3, GETDATE()))
  FROM 
  (
    SELECT TOP (DATEDIFF(DAY, DATEADD(yy, -3, GETDATE()), DATEADD(yy, 3, GETDATE()))) 
      rn = ROW_NUMBER() OVER (ORDER BY s1.[object_id])
    FROM sys.all_objects AS s1
    CROSS JOIN sys.all_objects AS s2
    -- on my system this would support > 5 million days
    ORDER BY s1.[object_id]
  ) AS x
) AS y;
SELECT * FROM dbo.DateDimension