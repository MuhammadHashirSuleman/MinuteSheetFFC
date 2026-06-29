-- ============================================================================
-- MINUTE SHEET / GENERAL APPROVAL WORKFLOW
-- SEED / DUMMY DATA - Microsoft SQL Server
-- ============================================================================
-- Run AFTER Schema.sql.
-- All employee data sourced from: FFC_Orbit_Employee_Template_Anonymized.xlsx
-- All values are synthetic/fake for prototype testing only.
-- ============================================================================

USE [MinuteSheetDB];
GO

-- ============================================================================
-- 1. DEPARTMENTS (6 departments from Excel)
-- ============================================================================
INSERT INTO dbo.Departments (DepartmentId, Name, ShortName, IsActive) VALUES
(14002001, 'Information & Communication Technology', 'ICT', 1),
(14002002, 'Finance',                                'FIN', 1),
(14002003, 'Human Capital',                          'HCM', 1),
(14002004, 'Procurement',                            'PRC', 1),
(14002005, 'Operations',                             'OPS', 1),
(14002006, 'Maintenance',                            'MNT', 1);
GO

-- ============================================================================
-- 2. EMPLOYEES (44 records - exact copy from Excel template)
-- ============================================================================
-- Columns: PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus,
--          FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
--          Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
--          DepartmentId, DepartmentName, DepartmentShort,
--          EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
--          PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
--          CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance
-- ============================================================================

-- -------------------------------------------------------
-- DEPT: ICT (14002001) - Normal hierarchy chain
-- Chain: Employee -> Manager 001 -> Senior Manager 001 -> General Manager 001 -> Head of Function 001 (top)
-- -------------------------------------------------------
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00010101','OLD10101','Employee 001','employee001@training.local','03XX-0000001','00000-0000001-0',1,0,'Guardian 001','1991-01-02','2021-06-02','2022-06-02','2051-01-02',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000001,NULL,
 14002001,'Information & Communication Technology','ICT',
 'M','AH',14000001,'Information & Communication Technology','ICT',
 9001,'Head Office',9101,'Head Office',
 900001001,'ICT Cost Center 001','00010110',1,1),

('00010102','OLD10102','Employee 002','employee002@training.local','03XX-0000002','00000-0000002-0',2,0,'Guardian 002','1992-01-03','2022-06-03','2023-06-03','2052-01-03',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000002,NULL,
 14002001,'Information & Communication Technology','ICT',
 'M','AH',14000001,'Information & Communication Technology','ICT',
 9001,'Head Office',9101,'Head Office',
 900001002,'ICT Cost Center 002','00010110',1,2),

('00010110','OLD10110','Manager 001','employee003@training.local','03XX-0000003','00000-0000003-0',1,1,'Guardian 003','1993-01-04','2023-06-04','2020-06-04','2053-01-04',NULL,
 'Manager','MGR','Manager',50000002,14000003,NULL,
 14002001,'Information & Communication Technology','ICT',
 'M','AG',14000001,'Information & Communication Technology','ICT',
 9001,'Head Office',9101,'Head Office',
 900001003,'ICT Cost Center 003','00010120',1,3),

('00010120','OLD10120','Senior Manager 001','employee004@training.local','03XX-0000004','00000-0000004-0',2,0,'Guardian 004','1994-01-05','2020-06-05','2021-06-05','2054-01-05',NULL,
 'Senior Manager','SM','Senior Manager',50000003,14000004,NULL,
 14002001,'Information & Communication Technology','ICT',
 'M','AF',14000001,'Information & Communication Technology','ICT',
 9001,'Head Office',9101,'Head Office',
 900001004,'ICT Cost Center 004','00010130',1,4),

('00010130','OLD10130','General Manager 001','employee005@training.local','03XX-0000005','00000-0000005-0',1,0,'Guardian 005','1995-01-06','2021-06-06','2022-06-06','2055-01-06',NULL,
 'General Manager','GM','General Manager',50000004,14000005,NULL,
 14002001,'Information & Communication Technology','ICT',
 'M','AE',14000001,'Information & Communication Technology','ICT',
 9001,'Head Office',9101,'Head Office',
 900001005,'ICT Cost Center 005','00010140',1,5),

('00010140','OLD10140','Head of Function 001','employee006@training.local','03XX-0000006','00000-0000006-0',2,1,'Guardian 006','1996-01-07','2022-06-07','2023-06-07','2056-01-07',NULL,
 'Head of Function','HOF','Head of Function',50000005,14000006,NULL,
 14002001,'Information & Communication Technology','ICT',
 'M','AD',14000001,'Information & Communication Technology','ICT',
 9001,'Head Office',9101,'Head Office',
 900001006,'ICT Cost Center 006',NULL,1,6);  -- TOP LEVEL: no manager
GO

-- -------------------------------------------------------
-- DEPT: Finance (14002002) - Normal hierarchy chain
-- -------------------------------------------------------
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00010201','OLD10201','Employee 007','employee007@training.local','03XX-0000007','00000-0000007-0',1,0,'Guardian 007','1997-01-08','2023-06-08','2020-06-08','2057-01-08',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000007,NULL,
 14002002,'Finance','FIN',
 'M','AH',14000002,'Finance','FIN',
 9001,'Head Office',9101,'Head Office',
 900002007,'FIN Cost Center 007','00010210',1,7),

('00010202','OLD10202','Employee 008','employee008@training.local','03XX-0000008','00000-0000008-0',2,0,'Guardian 008','1998-01-09','2020-06-09','2021-06-09','2058-01-09',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000008,NULL,
 14002002,'Finance','FIN',
 'M','AH',14000002,'Finance','FIN',
 9001,'Head Office',9101,'Head Office',
 900002008,'FIN Cost Center 008','00010210',1,8),

('00010210','OLD10210','Manager 002','employee009@training.local','03XX-0000009','00000-0000009-0',1,1,'Guardian 009','1999-01-10','2021-06-10','2022-06-10','2059-01-10',NULL,
 'Manager','MGR','Manager',50000002,14000009,NULL,
 14002002,'Finance','FIN',
 'M','AG',14000002,'Finance','FIN',
 9001,'Head Office',9101,'Head Office',
 900002009,'FIN Cost Center 009','00010220',1,9),

('00010220','OLD10220','Senior Manager 002','employee010@training.local','03XX-0000010','00000-0000010-0',2,0,'Guardian 010','1990-01-11','2022-06-11','2023-06-11','2050-01-11',NULL,
 'Senior Manager','SM','Senior Manager',50000003,14000010,NULL,
 14002002,'Finance','FIN',
 'M','AF',14000002,'Finance','FIN',
 9001,'Head Office',9101,'Head Office',
 900002010,'FIN Cost Center 010','00010230',1,10),

('00010230','OLD10230','General Manager 002','employee011@training.local','03XX-0000011','00000-0000011-0',1,0,'Guardian 011','1991-01-12','2023-06-12','2020-06-12','2051-01-12',NULL,
 'General Manager','GM','General Manager',50000004,14000011,NULL,
 14002002,'Finance','FIN',
 'M','AE',14000002,'Finance','FIN',
 9001,'Head Office',9101,'Head Office',
 900002011,'FIN Cost Center 011','00010240',1,0),

('00010240','OLD10240','Head of Function 002','employee012@training.local','03XX-0000012','00000-0000012-0',2,1,'Guardian 012','1992-01-13','2020-06-13','2021-06-13','2052-01-13',NULL,
 'Head of Function','HOF','Head of Function',50000005,14000012,NULL,
 14002002,'Finance','FIN',
 'M','AD',14000002,'Finance','FIN',
 9001,'Head Office',9101,'Head Office',
 900002012,'FIN Cost Center 012',NULL,1,1);  -- TOP LEVEL
GO

-- -------------------------------------------------------
-- DEPT: Human Capital (14002003)
-- -------------------------------------------------------
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00010301','OLD10301','Employee 013','employee013@training.local','03XX-0000013','00000-0000013-0',1,0,'Guardian 013','1993-01-14','2021-06-14','2022-06-14','2053-01-14',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000013,NULL,
 14002003,'Human Capital','HCM',
 'M','AH',14000003,'Human Capital','HCM',
 9001,'Head Office',9101,'Head Office',
 900003013,'HCM Cost Center 013','00010310',1,2),

('00010302','OLD10302','Employee 014','employee014@training.local','03XX-0000014','00000-0000014-0',2,0,'Guardian 014','1994-01-15','2022-06-15','2023-06-15','2054-01-15',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000014,NULL,
 14002003,'Human Capital','HCM',
 'M','AH',14000003,'Human Capital','HCM',
 9001,'Head Office',9101,'Head Office',
 900003014,'HCM Cost Center 014','00010310',1,3),

('00010310','OLD10310','Manager 003','employee015@training.local','03XX-0000015','00000-0000015-0',1,1,'Guardian 015','1995-01-16','2023-06-16','2020-06-16','2055-01-16',NULL,
 'Manager','MGR','Manager',50000002,14000015,NULL,
 14002003,'Human Capital','HCM',
 'M','AG',14000003,'Human Capital','HCM',
 9001,'Head Office',9101,'Head Office',
 900003015,'HCM Cost Center 015','00010320',1,4),

('00010320','OLD10320','Senior Manager 003','employee016@training.local','03XX-0000016','00000-0000016-0',2,0,'Guardian 016','1996-01-17','2020-06-17','2021-06-17','2056-01-17',NULL,
 'Senior Manager','SM','Senior Manager',50000003,14000016,NULL,
 14002003,'Human Capital','HCM',
 'M','AF',14000003,'Human Capital','HCM',
 9001,'Head Office',9101,'Head Office',
 900003016,'HCM Cost Center 016','00010330',1,5),

('00010330','OLD10330','General Manager 003','employee017@training.local','03XX-0000017','00000-0000017-0',1,0,'Guardian 017','1997-01-18','2021-06-18','2022-06-18','2057-01-18',NULL,
 'General Manager','GM','General Manager',50000004,14000017,NULL,
 14002003,'Human Capital','HCM',
 'M','AE',14000003,'Human Capital','HCM',
 9001,'Head Office',9101,'Head Office',
 900003017,'HCM Cost Center 017','00010340',1,6),

('00010340','OLD10340','Head of Function 003','employee018@training.local','03XX-0000018','00000-0000018-0',2,1,'Guardian 018','1998-01-19','2022-06-19','2023-06-19','2058-01-19',NULL,
 'Head of Function','HOF','Head of Function',50000005,14000018,NULL,
 14002003,'Human Capital','HCM',
 'M','AD',14000003,'Human Capital','HCM',
 9001,'Head Office',9101,'Head Office',
 900003018,'HCM Cost Center 018',NULL,1,7);  -- TOP LEVEL
GO

-- -------------------------------------------------------
-- DEPT: Procurement (14002004)
-- -------------------------------------------------------
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00010401','OLD10401','Employee 019','employee019@training.local','03XX-0000019','00000-0000019-0',1,0,'Guardian 019','1999-01-20','2023-06-20','2020-06-20','2059-01-20',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000019,NULL,
 14002004,'Procurement','PRC',
 'M','AH',14000004,'Commercial','COM',
 9002,'Commercial Area',9201,'Central Office',
 900004019,'PRC Cost Center 019','00010410',1,8),

('00010402','OLD10402','Employee 020','employee020@training.local','03XX-0000020','00000-0000020-0',2,0,'Guardian 020','1990-01-21','2020-06-01','2021-06-01','2050-01-21',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000020,NULL,
 14002004,'Procurement','PRC',
 'M','AH',14000004,'Commercial','COM',
 9002,'Commercial Area',9201,'Central Office',
 900004020,'PRC Cost Center 020','00010410',1,9),

('00010410','OLD10410','Manager 004','employee021@training.local','03XX-0000021','00000-0000021-0',1,1,'Guardian 021','1991-01-22','2021-06-02','2022-06-02','2051-01-22',NULL,
 'Manager','MGR','Manager',50000002,14000021,NULL,
 14002004,'Procurement','PRC',
 'M','AG',14000004,'Commercial','COM',
 9002,'Commercial Area',9201,'Central Office',
 900004021,'PRC Cost Center 021','00010420',1,10),

('00010420','OLD10420','Senior Manager 004','employee022@training.local','03XX-0000022','00000-0000022-0',2,0,'Guardian 022','1992-01-23','2022-06-03','2023-06-03','2052-01-23',NULL,
 'Senior Manager','SM','Senior Manager',50000003,14000022,NULL,
 14002004,'Procurement','PRC',
 'M','AF',14000004,'Commercial','COM',
 9002,'Commercial Area',9201,'Central Office',
 900004022,'PRC Cost Center 022','00010430',1,0),

('00010430','OLD10430','General Manager 004','employee023@training.local','03XX-0000023','00000-0000023-0',1,0,'Guardian 023','1993-01-24','2023-06-04','2020-06-04','2053-01-24',NULL,
 'General Manager','GM','General Manager',50000004,14000023,NULL,
 14002004,'Procurement','PRC',
 'M','AE',14000004,'Commercial','COM',
 9002,'Commercial Area',9201,'Central Office',
 900004023,'PRC Cost Center 023','00010440',1,1),

('00010440','OLD10440','Head of Function 004','employee024@training.local','03XX-0000024','00000-0000024-0',2,1,'Guardian 024','1994-01-25','2020-06-05','2021-06-05','2054-01-25',NULL,
 'Head of Function','HOF','Head of Function',50000005,14000024,NULL,
 14002004,'Procurement','PRC',
 'M','AD',14000004,'Commercial','COM',
 9002,'Commercial Area',9201,'Central Office',
 900004024,'PRC Cost Center 024',NULL,1,2);  -- TOP LEVEL
GO

-- -------------------------------------------------------
-- DEPT: Operations (14002005)
-- -------------------------------------------------------
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00010501','OLD10501','Employee 025','employee025@training.local','03XX-0000025','00000-0000025-0',1,0,'Guardian 025','1995-01-26','2021-06-06','2022-06-06','2055-01-26',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000025,NULL,
 14002005,'Operations','OPS',
 'M','AH',14000005,'Technical','TECH',
 9003,'Plant Area',9301,'Plant Site',
 900005025,'OPS Cost Center 025','00010510',1,3),

('00010502','OLD10502','Employee 026','employee026@training.local','03XX-0000026','00000-0000026-0',2,0,'Guardian 026','1996-01-27','2022-06-07','2023-06-07','2056-01-27',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000026,NULL,
 14002005,'Operations','OPS',
 'M','AH',14000005,'Technical','TECH',
 9003,'Plant Area',9301,'Plant Site',
 900005026,'OPS Cost Center 026','00010510',1,4),

('00010510','OLD10510','Manager 005','employee027@training.local','03XX-0000027','00000-0000027-0',1,1,'Guardian 027','1997-01-28','2023-06-08','2020-06-08','2057-01-28',NULL,
 'Manager','MGR','Manager',50000002,14000027,NULL,
 14002005,'Operations','OPS',
 'M','AG',14000005,'Technical','TECH',
 9003,'Plant Area',9301,'Plant Site',
 900005027,'OPS Cost Center 027','00010520',1,5),

('00010520','OLD10520','Senior Manager 005','employee028@training.local','03XX-0000028','00000-0000028-0',2,0,'Guardian 028','1998-01-01','2020-06-09','2021-06-09','2058-01-01',NULL,
 'Senior Manager','SM','Senior Manager',50000003,14000028,NULL,
 14002005,'Operations','OPS',
 'M','AF',14000005,'Technical','TECH',
 9003,'Plant Area',9301,'Plant Site',
 900005028,'OPS Cost Center 028','00010530',1,6),

('00010530','OLD10530','General Manager 005','employee029@training.local','03XX-0000029','00000-0000029-0',1,0,'Guardian 029','1999-01-02','2021-06-10','2022-06-10','2059-01-02',NULL,
 'General Manager','GM','General Manager',50000004,14000029,NULL,
 14002005,'Operations','OPS',
 'M','AE',14000005,'Technical','TECH',
 9003,'Plant Area',9301,'Plant Site',
 900005029,'OPS Cost Center 029','00010540',1,7),

('00010540','OLD10540','Head of Function 005','employee030@training.local','03XX-0000030','00000-0000030-0',2,1,'Guardian 030','1990-01-03','2022-06-11','2023-06-11','2050-01-03',NULL,
 'Head of Function','HOF','Head of Function',50000005,14000030,NULL,
 14002005,'Operations','OPS',
 'M','AD',14000005,'Technical','TECH',
 9003,'Plant Area',9301,'Plant Site',
 900005030,'OPS Cost Center 030',NULL,1,8);  -- TOP LEVEL
GO

-- -------------------------------------------------------
-- DEPT: Maintenance (14002006)
-- -------------------------------------------------------
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00010601','OLD10601','Employee 031','employee031@training.local','03XX-0000031','00000-0000031-0',1,0,'Guardian 031','1991-01-04','2023-06-12','2020-06-12','2051-01-04',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000031,NULL,
 14002006,'Maintenance','MNT',
 'M','AH',14000006,'Technical','TECH',
 9003,'Plant Area',9302,'Maintenance Site',
 900006031,'MNT Cost Center 031','00010610',1,9),

('00010602','OLD10602','Employee 032','employee032@training.local','03XX-0000032','00000-0000032-0',2,0,'Guardian 032','1992-01-05','2020-06-13','2021-06-13','2052-01-05',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000032,NULL,
 14002006,'Maintenance','MNT',
 'M','AH',14000006,'Technical','TECH',
 9003,'Plant Area',9302,'Maintenance Site',
 900006032,'MNT Cost Center 032','00010610',1,10),

('00010610','OLD10610','Manager 006','employee033@training.local','03XX-0000033','00000-0000033-0',1,1,'Guardian 033','1993-01-06','2021-06-14','2022-06-14','2053-01-06',NULL,
 'Manager','MGR','Manager',50000002,14000033,NULL,
 14002006,'Maintenance','MNT',
 'M','AG',14000006,'Technical','TECH',
 9003,'Plant Area',9302,'Maintenance Site',
 900006033,'MNT Cost Center 033','00010620',1,0),

('00010620','OLD10620','Senior Manager 006','employee034@training.local','03XX-0000034','00000-0000034-0',2,0,'Guardian 034','1994-01-07','2022-06-15','2023-06-15','2054-01-07',NULL,
 'Senior Manager','SM','Senior Manager',50000003,14000034,NULL,
 14002006,'Maintenance','MNT',
 'M','AF',14000006,'Technical','TECH',
 9003,'Plant Area',9302,'Maintenance Site',
 900006034,'MNT Cost Center 034','00010630',1,1),

('00010630','OLD10630','General Manager 006','employee035@training.local','03XX-0000035','00000-0000035-0',1,0,'Guardian 035','1995-01-08','2023-06-16','2020-06-16','2055-01-08',NULL,
 'General Manager','GM','General Manager',50000004,14000035,NULL,
 14002006,'Maintenance','MNT',
 'M','AE',14000006,'Technical','TECH',
 9003,'Plant Area',9302,'Maintenance Site',
 900006035,'MNT Cost Center 035','00010640',1,2),

('00010640','OLD10640','Head of Function 006','employee036@training.local','03XX-0000036','00000-0000036-0',2,1,'Guardian 036','1996-01-09','2020-06-17','2021-06-17','2056-01-09',NULL,
 'Head of Function','HOF','Head of Function',50000005,14000036,NULL,
 14002006,'Maintenance','MNT',
 'M','AD',14000006,'Technical','TECH',
 9003,'Plant Area',9302,'Maintenance Site',
 900006036,'MNT Cost Center 036',NULL,1,3);  -- TOP LEVEL
GO

-- ============================================================================
-- EDGE CASE TEST EMPLOYEES (from Excel rows 39-45)
-- ============================================================================

-- EDGE CASE 1: MISSING MANAGER
-- PNo 00090001, ManagerPNo = 00999999 (does not exist in Employees table)
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00090001','OLD90001','Employee Missing Manager','employee037@training.local','03XX-0000037','00000-0000037-0',1,0,'Guardian 037','1997-01-10','2021-06-18','2022-06-18','2057-01-10',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000037,NULL,
 14002001,'Information & Communication Technology','ICT',
 'M','AH',14000001,'Information & Communication Technology','ICT',
 9001,'Head Office',9101,'Head Office',
 900001037,'ICT Cost Center 037','00999999',1,4);
GO

-- EDGE CASE 2: INACTIVE MANAGER
-- PNo 00090020 is inactive (LeavingDate = 2025-12-31, IsActive = FALSE)
-- PNo 00090021 reports to this inactive manager
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00090020','OLD90020','Inactive Manager','employee038@training.local','03XX-0000038','00000-0000038-0',2,0,'Guardian 038','1998-01-11','2022-06-19','2023-06-19','2058-01-11','2025-12-31',
 'Manager','MGR','Manager',50000002,14000038,NULL,
 14002002,'Finance','FIN',
 'M','AG',14000002,'Finance','FIN',
 9001,'Head Office',9101,'Head Office',
 900002038,'FIN Cost Center 038','00010240',0,5),  -- IsActive = 0 (FALSE)

('00090021','OLD90021','Employee With Inactive Manager','employee039@training.local','03XX-0000039','00000-0000039-0',1,1,'Guardian 039','1999-01-12','2023-06-20','2020-06-20','2059-01-12',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000039,NULL,
 14002002,'Finance','FIN',
 'M','AH',14000002,'Finance','FIN',
 9001,'Head Office',9101,'Head Office',
 900002039,'FIN Cost Center 039','00090020',1,6);  -- Reports to inactive 00090020
GO

-- EDGE CASE 3: SELF MANAGER
-- PNo 00090030, ManagerPNo = 00090030 (points to itself)
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00090030','OLD90030','Employee Self Manager','employee040@training.local','03XX-0000040','00000-0000040-0',2,0,'Guardian 040','1990-01-13','2020-06-01','2021-06-01','2050-01-13',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000040,NULL,
 14002003,'Human Capital','HCM',
 'M','AH',14000003,'Human Capital','HCM',
 9001,'Head Office',9101,'Head Office',
 900003040,'HCM Cost Center 040','00090030',1,7);  -- SELF MANAGER
GO

-- EDGE CASE 4: CIRCULAR HIERARCHY (A -> B -> C -> A)
INSERT INTO dbo.Employees
(PNo, OldPNo, Name, Email, Phone, CNIC, Gender, MaritalStatus, FatherName, DOB, HireDate, LastPromotionDate, RetirementDate, LeavingDate,
 Designation, DesignationShort, JobDescription, JobKey, PositionId, Grade,
 DepartmentId, DepartmentName, DepartmentShort,
 EmployeeGroup, EmployeeCategory, GroupId, GroupDesc, GroupShort,
 PAreaId, PAreaDesc, PSAreaId, PSAreaDesc,
 CostCenter, CostCenterDesc, ManagerPNo, IsActive, AnnualLeaveBalance)
VALUES
('00090040','OLD90040','Circular Employee A','employee041@training.local','03XX-0000041','00000-0000041-0',1,0,'Guardian 041','1991-01-14','2021-06-02','2022-06-02','2051-01-14',NULL,
 'Assistant Manager','AM','Assistant Manager',50000001,14000041,NULL,
 14002004,'Procurement','PRC',
 'M','AH',14000004,'Commercial','COM',
 9002,'Commercial Area',9201,'Central Office',
 900004041,'PRC Cost Center 041','00090041',1,8),   -- A reports to B

('00090041','OLD90041','Circular Employee B','employee042@training.local','03XX-0000042','00000-0000042-0',2,1,'Guardian 042','1992-01-15','2022-06-03','2023-06-03','2052-01-15',NULL,
 'Manager','MGR','Manager',50000002,14000042,NULL,
 14002004,'Procurement','PRC',
 'M','AG',14000004,'Commercial','COM',
 9002,'Commercial Area',9201,'Central Office',
 900004042,'PRC Cost Center 042','00090042',1,9),   -- B reports to C

('00090042','OLD90042','Circular Employee C','employee043@training.local','03XX-0000043','00000-0000043-0',1,0,'Guardian 043','1993-01-16','2023-06-04','2020-06-04','2053-01-16',NULL,
 'Senior Manager','SM','Senior Manager',50000003,14000043,NULL,
 14002004,'Procurement','PRC',
 'M','AF',14000004,'Commercial','COM',
 9002,'Commercial Area',9201,'Central Office',
 900004043,'PRC Cost Center 043','00090040',1,10);  -- C reports to A -> CIRCULAR
GO


-- ============================================================================
-- 3. ROLES
-- ============================================================================
INSERT INTO dbo.Roles (Name, Description) VALUES
('Admin',           'System administrator with full access to all features and configurations'),
('Employee',        'Regular employee who can create requests and view their own data'),
('Manager',         'Manager who can review and approve requests from direct reports'),
('FinanceReviewer', 'Finance department reviewer for financial request types'),
('HRAdmin',         'HR administrator with access to employee directory management'),
('Auditor',         'Read-only access to all requests, history, and exceptions for audit purposes');
GO

-- ============================================================================
-- 4. USERS (Login accounts linked to employees)
-- ============================================================================
-- Password hash is BCrypt of "Password123!" for all dummy users.
-- BCrypt hash: $2a$11$K4YBroMCEqGPfMGQvHxK5OVMaBFPFGFCoSyGajghfFQCGMaH84Mvu

DECLARE @PH NVARCHAR(500) = '$2a$11$K4YBroMCEqGPfMGQvHxK5OVMaBFPFGFCoSyGajghfFQCGMaH84Mvu';

INSERT INTO dbo.Users (Username, Email, PasswordHash, EmployeePNo, IsActive) VALUES
-- ICT Department
('employee001',         'employee001@training.local',   @PH, '00010101', 1),   -- 1
('employee002',         'employee002@training.local',   @PH, '00010102', 1),   -- 2
('manager001',          'employee003@training.local',   @PH, '00010110', 1),   -- 3
('srmgr001',            'employee004@training.local',   @PH, '00010120', 1),   -- 4
('gm001',               'employee005@training.local',   @PH, '00010130', 1),   -- 5
('hof001',              'employee006@training.local',   @PH, '00010140', 1),   -- 6
-- Finance Department
('employee007',         'employee007@training.local',   @PH, '00010201', 1),   -- 7
('employee008',         'employee008@training.local',   @PH, '00010202', 1),   -- 8
('manager002',          'employee009@training.local',   @PH, '00010210', 1),   -- 9
('srmgr002',            'employee010@training.local',   @PH, '00010220', 1),   -- 10
('gm002',               'employee011@training.local',   @PH, '00010230', 1),   -- 11
('hof002',              'employee012@training.local',   @PH, '00010240', 1),   -- 12
-- Human Capital Department
('employee013',         'employee013@training.local',   @PH, '00010301', 1),   -- 13
('employee014',         'employee014@training.local',   @PH, '00010302', 1),   -- 14
('manager003',          'employee015@training.local',   @PH, '00010310', 1),   -- 15
('srmgr003',            'employee016@training.local',   @PH, '00010320', 1),   -- 16
('gm003',               'employee017@training.local',   @PH, '00010330', 1),   -- 17
('hof003',              'employee018@training.local',   @PH, '00010340', 1),   -- 18
-- Procurement Department
('employee019',         'employee019@training.local',   @PH, '00010401', 1),   -- 19
('employee020',         'employee020@training.local',   @PH, '00010402', 1),   -- 20
('manager004',          'employee021@training.local',   @PH, '00010410', 1),   -- 21
('srmgr004',            'employee022@training.local',   @PH, '00010420', 1),   -- 22
('gm004',               'employee023@training.local',   @PH, '00010430', 1),   -- 23
('hof004',              'employee024@training.local',   @PH, '00010440', 1),   -- 24
-- Operations Department
('employee025',         'employee025@training.local',   @PH, '00010501', 1),   -- 25
('employee026',         'employee026@training.local',   @PH, '00010502', 1),   -- 26
('manager005',          'employee027@training.local',   @PH, '00010510', 1),   -- 27
('srmgr005',            'employee028@training.local',   @PH, '00010520', 1),   -- 28
('gm005',               'employee029@training.local',   @PH, '00010530', 1),   -- 29
('hof005',              'employee030@training.local',   @PH, '00010540', 1),   -- 30
-- Maintenance Department
('employee031',         'employee031@training.local',   @PH, '00010601', 1),   -- 31
('employee032',         'employee032@training.local',   @PH, '00010602', 1),   -- 32
('manager006',          'employee033@training.local',   @PH, '00010610', 1),   -- 33
('srmgr006',            'employee034@training.local',   @PH, '00010620', 1),   -- 34
('gm006',               'employee035@training.local',   @PH, '00010630', 1),   -- 35
('hof006',              'employee036@training.local',   @PH, '00010640', 1),   -- 36
-- Edge case employees
('missing.mgr',         'employee037@training.local',   @PH, '00090001', 1),   -- 37
('inactive.mgr',        'employee038@training.local',   @PH, '00090020', 1),   -- 38
('inactive.mgr.rpt',    'employee039@training.local',   @PH, '00090021', 1),   -- 39
('self.mgr',            'employee040@training.local',   @PH, '00090030', 1),   -- 40
('circular.a',          'employee041@training.local',   @PH, '00090040', 1),   -- 41
('circular.b',          'employee042@training.local',   @PH, '00090041', 1),   -- 42
('circular.c',          'employee043@training.local',   @PH, '00090042', 1),   -- 43
-- System admin (no employee link)
('admin.system',        'admin.system@training.local',  @PH, NULL,       1);   -- 44
GO

-- ============================================================================
-- 5. USER-ROLE ASSIGNMENTS
-- ============================================================================

-- System admin
INSERT INTO dbo.UserRoles (UserId, RoleId) VALUES (44, 1);  -- admin.system -> Admin

-- All AM/Officer-level employees -> Employee role (RoleId=2)
INSERT INTO dbo.UserRoles (UserId, RoleId) VALUES
(1,2),(2,2),(7,2),(8,2),(13,2),(14,2),(19,2),(20,2),(25,2),(26,2),(31,2),(32,2),  -- normal employees
(37,2),(38,2),(39,2),(40,2),(41,2),(42,2),(43,2);  -- edge case employees

-- All Managers -> Employee + Manager roles (RoleId=2,3)
INSERT INTO dbo.UserRoles (UserId, RoleId) VALUES
(3,2),(3,3),    -- Manager 001
(9,2),(9,3),    -- Manager 002
(15,2),(15,3),  -- Manager 003
(21,2),(21,3),  -- Manager 004
(27,2),(27,3),  -- Manager 005
(33,2),(33,3);  -- Manager 006

-- All Senior Managers -> Employee + Manager roles
INSERT INTO dbo.UserRoles (UserId, RoleId) VALUES
(4,2),(4,3),    -- Senior Manager 001
(10,2),(10,3),  -- Senior Manager 002
(16,2),(16,3),  -- Senior Manager 003
(22,2),(22,3),  -- Senior Manager 004
(28,2),(28,3),  -- Senior Manager 005
(34,2),(34,3);  -- Senior Manager 006

-- All General Managers -> Employee + Manager roles
INSERT INTO dbo.UserRoles (UserId, RoleId) VALUES
(5,2),(5,3),    -- GM 001
(11,2),(11,3),  -- GM 002
(17,2),(17,3),  -- GM 003
(23,2),(23,3),  -- GM 004
(29,2),(29,3),  -- GM 005
(35,2),(35,3);  -- GM 006

-- All Heads of Function -> Employee + Manager roles
INSERT INTO dbo.UserRoles (UserId, RoleId) VALUES
(6,2),(6,3),    -- HOF 001
(12,2),(12,3),  -- HOF 002
(18,2),(18,3),  -- HOF 003
(24,2),(24,3),  -- HOF 004
(30,2),(30,3),  -- HOF 005
(36,2),(36,3);  -- HOF 006

-- Finance Senior Manager 002 -> also FinanceReviewer (RoleId=4)
INSERT INTO dbo.UserRoles (UserId, RoleId) VALUES (10, 4);

-- HCM Manager 003 -> also HRAdmin (RoleId=5)
INSERT INTO dbo.UserRoles (UserId, RoleId) VALUES (15, 5);

GO

-- ============================================================================
-- 6. REQUEST TYPES
-- ============================================================================
INSERT INTO dbo.RequestTypes (Code, Name, Description, IsActive) VALUES
('FIN',     'Financial',              'Requests involving financial expenditure, budget allocation, or procurement.', 1),
('NONFIN',  'Non-Financial',          'General requests not involving direct financial expenditure.',                 1),
('HR',      'Human Resources',        'HR-related requests such as policy changes, transfers, or benefits.',         1),
('IT',      'Information Technology',  'IT infrastructure, software, hardware, or system access requests.',           1),
('PROC',    'Procurement',            'Procurement and vendor-related requests.',                                    1),
('ADMIN',   'Administrative',         'General administrative and facilities requests.',                             1);
GO

-- ============================================================================
-- 7. WORKFLOW RULES
-- ============================================================================
INSERT INTO dbo.WorkflowRules (RequestTypeId, BudgetFrom, BudgetTo, RequiredManagerLevels, RequiresFinanceReview, FallbackBehavior, FinalAction, IsActive) VALUES
-- Non-Financial: 2 levels, no finance review
(2, 0.00, NULL,       2, 0, 'WARN', 'APPROVE', 1),
-- Financial: tiered by budget
(1, 0.00,      100000.00, 2, 1, 'WARN', 'APPROVE', 1),
(1, 100000.01, 500000.00, 3, 1, 'WARN', 'APPROVE', 1),
(1, 500000.01, NULL,      4, 1, 'WARN', 'APPROVE', 1),
-- HR: 2 levels
(3, 0.00, NULL, 2, 0, 'WARN', 'APPROVE', 1),
-- IT: tiered
(4, 0.00,   50000.00, 2, 0, 'WARN', 'APPROVE', 1),
(4, 50000.01, NULL,   3, 1, 'WARN', 'APPROVE', 1),
-- Procurement: tiered
(5, 0.00,      200000.00, 2, 1, 'WARN', 'APPROVE', 1),
(5, 200000.01, NULL,      3, 1, 'WARN', 'APPROVE', 1),
-- Administrative: 1 level
(6, 0.00, NULL, 1, 0, 'WARN', 'APPROVE', 1);
GO

-- ============================================================================
-- 8. SAMPLE MINUTE SHEET REQUESTS (6 requests in various states)
-- ============================================================================

-- Request 1: APPROVED (Financial, Dynamic, full lifecycle)
INSERT INTO dbo.MinuteSheetRequests
    (ReferenceNumber, Subject, Body, RequestTypeId, EstimatedBudget, Priority, IsConfidential, WorkflowMode, Status, RequesterPNo, CurrentActionerPNo, CurrentStageOrder, SubmittedAt, CompletedAt)
VALUES
('MS-2026-00001',
 'Procurement of 50 Laptops for ICT Department',
 '<p>This is to request approval for the procurement of <strong>50 laptops</strong> for the ICT department. The current equipment is outdated and affecting team productivity.</p><p><strong>Justification:</strong></p><ul><li>Current laptops are 5+ years old</li><li>Frequent hardware failures causing downtime</li><li>New software requirements exceed current specifications</li></ul><p><strong>Estimated Cost:</strong> PKR 250,000</p><p><strong>Vendor:</strong> To be selected via competitive bidding</p><p><strong>Expected Delivery:</strong> Within 45 days of approval</p>',
 1, 250000.00, 'HIGH', 0, 'DYNAMIC', 'APPROVED',
 '00010101', NULL, NULL, '2026-06-15 09:30:00', '2026-06-20 16:45:00');

-- Request 2: IN_REVIEW (Non-Financial, Dynamic, at stage 2)
INSERT INTO dbo.MinuteSheetRequests
    (ReferenceNumber, Subject, Body, RequestTypeId, EstimatedBudget, Priority, IsConfidential, WorkflowMode, Status, RequesterPNo, CurrentActionerPNo, CurrentStageOrder, SubmittedAt, CompletedAt)
VALUES
('MS-2026-00002',
 'Request for Team Building Activity Approval',
 '<p>Requesting approval for an <strong>annual team building activity</strong> for the ICT department.</p><p>Proposed activities include outdoor exercises and workshops to improve team collaboration. The event is planned for Q3 2026.</p><p>No direct financial expenditure required as it will use the existing departmental welfare budget.</p>',
 2, NULL, 'NORMAL', 0, 'DYNAMIC', 'IN_REVIEW',
 '00010102', '00010120', 2, '2026-06-22 11:00:00', NULL);

-- Request 3: DRAFT (Financial, not yet submitted)
INSERT INTO dbo.MinuteSheetRequests
    (ReferenceNumber, Subject, Body, RequestTypeId, EstimatedBudget, Priority, IsConfidential, WorkflowMode, Status, RequesterPNo, CurrentActionerPNo, CurrentStageOrder, SubmittedAt, CompletedAt)
VALUES
('MS-2026-00003',
 'Annual Software License Renewal',
 '<p>Draft request for renewal of annual software licenses including:</p><ul><li>Microsoft 365 Enterprise - 200 licenses</li><li>Adobe Creative Cloud - 15 licenses</li><li>Jira/Confluence - 50 licenses</li></ul><p>Budget estimate pending vendor quotations.</p>',
 1, 450000.00, 'NORMAL', 0, 'DYNAMIC', 'DRAFT',
 '00010101', NULL, NULL, NULL, NULL);

-- Request 4: REJECTED (Financial, rejected at stage 2)
INSERT INTO dbo.MinuteSheetRequests
    (ReferenceNumber, Subject, Body, RequestTypeId, EstimatedBudget, Priority, IsConfidential, WorkflowMode, Status, RequesterPNo, CurrentActionerPNo, CurrentStageOrder, SubmittedAt, CompletedAt)
VALUES
('MS-2026-00004',
 'Purchase of Luxury Office Furniture',
 '<p>Request to purchase premium office furniture for the Finance department.</p><p>Items include executive desks, ergonomic chairs, and meeting room furnishing.</p><p><strong>Budget:</strong> PKR 800,000</p>',
 1, 800000.00, 'LOW', 0, 'DYNAMIC', 'REJECTED',
 '00010201', NULL, NULL, '2026-06-10 14:20:00', '2026-06-13 10:30:00');

-- Request 5: RETURNED (HR type, returned to initiator)
INSERT INTO dbo.MinuteSheetRequests
    (ReferenceNumber, Subject, Body, RequestTypeId, EstimatedBudget, Priority, IsConfidential, WorkflowMode, Status, RequesterPNo, CurrentActionerPNo, CurrentStageOrder, SubmittedAt, CompletedAt)
VALUES
('MS-2026-00005',
 'Employee Transfer Request - Cross Department',
 '<p>Request to transfer <strong>2 employees</strong> from Operations to ICT department.</p><p>The transfer is proposed to address staffing needs in the ICT development team.</p><p><em>Note: Employee consent forms need to be attached.</em></p>',
 3, NULL, 'HIGH', 1, 'HYBRID', 'RETURNED',
 '00010301', '00010301', NULL, '2026-06-18 08:45:00', NULL);

-- Request 6: IN_REVIEW (IT type, just submitted, at stage 1)
INSERT INTO dbo.MinuteSheetRequests
    (ReferenceNumber, Subject, Body, RequestTypeId, EstimatedBudget, Priority, IsConfidential, WorkflowMode, Status, RequesterPNo, CurrentActionerPNo, CurrentStageOrder, SubmittedAt, CompletedAt)
VALUES
('MS-2026-00006',
 'Network Infrastructure Upgrade - Server Room',
 '<p>Urgent request for server room network infrastructure upgrade.</p><p><strong>Scope:</strong></p><ul><li>Replace aging network switches (10+ years old)</li><li>Install redundant fiber links</li><li>Upgrade firewall appliances</li></ul><p><strong>Estimated Budget:</strong> PKR 75,000</p><p><strong>Impact:</strong> Current infrastructure is causing intermittent outages affecting all departments.</p>',
 4, 75000.00, 'URGENT', 0, 'DYNAMIC', 'IN_REVIEW',
 '00010101', '00010110', 1, '2026-06-28 10:15:00', NULL);
GO

-- ============================================================================
-- 9. MINUTE SHEET ATTACHMENTS
-- ============================================================================
INSERT INTO dbo.MinuteSheetAttachments (MinuteSheetId, FileName, FileType, FileSize, StoragePath, UploadedByPNo) VALUES
(1, 'Laptop_Quotation_VendorA.pdf',   'application/pdf',  245760,  '/uploads/2026/06/laptop_quote_a.pdf',  '00010101'),
(1, 'Laptop_Quotation_VendorB.pdf',   'application/pdf',  198400,  '/uploads/2026/06/laptop_quote_b.pdf',  '00010101'),
(1, 'IT_Equipment_Specs.xlsx',         'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 89600, '/uploads/2026/06/it_specs.xlsx', '00010101'),
(4, 'Furniture_Catalog.pdf',           'application/pdf',  512000,  '/uploads/2026/06/furniture_catalog.pdf', '00010201'),
(6, 'Network_Assessment_Report.pdf',   'application/pdf',  327680,  '/uploads/2026/06/network_assessment.pdf', '00010101'),
(6, 'Server_Room_Layout.png',          'image/png',        153600,  '/uploads/2026/06/server_room.png',     '00010101');
GO

-- ============================================================================
-- 10. WORKFLOW STAGES
-- ============================================================================

-- Request 1 (APPROVED): Financial 250K -> 3 levels + finance review
-- Chain: Manager 001 -> Senior Manager 001 -> Sr Mgr 002 (Finance Review) -> General Manager 001
INSERT INTO dbo.WorkflowStages (MinuteSheetId, StageOrder, ActionerPNo, ActionerName, ActionerDesignation, ActionType, Status, Action, Remarks, ActionedAt, Source) VALUES
(1, 1, '00010110', 'Manager 001',        'Manager',        'REVIEW',          'COMPLETED', 'REVIEWED',  'Quotations verified. Equipment specifications are appropriate. Forwarding to senior management.', '2026-06-16 14:30:00', 'DYNAMIC'),
(1, 2, '00010120', 'Senior Manager 001', 'Senior Manager', 'REVIEW',          'COMPLETED', 'REVIEWED',  'Budget is within departmental allocation. Technical justification is sound. Recommending approval.', '2026-06-18 11:15:00', 'DYNAMIC'),
(1, 3, '00010220', 'Senior Manager 002', 'Senior Manager', 'FINANCE_REVIEW',  'COMPLETED', 'REVIEWED',  'Financial review complete. Budget allocation confirmed. Vendor bidding process should follow standard procurement policy.', '2026-06-19 09:45:00', 'DYNAMIC'),
(1, 4, '00010130', 'General Manager 001','General Manager','APPROVE',          'COMPLETED', 'APPROVED',  'Approved. Proceed with competitive bidding as per procurement policy.', '2026-06-20 16:45:00', 'DYNAMIC');

-- Request 2 (IN_REVIEW): Non-Financial -> 2 levels, at stage 2
INSERT INTO dbo.WorkflowStages (MinuteSheetId, StageOrder, ActionerPNo, ActionerName, ActionerDesignation, ActionType, Status, Action, Remarks, ActionedAt, Source) VALUES
(2, 1, '00010110', 'Manager 001',        'Manager',        'REVIEW',  'COMPLETED', 'REVIEWED',  'Team building is a good initiative. Ensure safety measures for outdoor activities.', '2026-06-24 10:30:00', 'DYNAMIC'),
(2, 2, '00010120', 'Senior Manager 001', 'Senior Manager', 'APPROVE', 'ACTIVE',    NULL,        NULL, NULL, 'DYNAMIC');

-- Request 4 (REJECTED): Financial 800K -> 4 levels, rejected at stage 2
INSERT INTO dbo.WorkflowStages (MinuteSheetId, StageOrder, ActionerPNo, ActionerName, ActionerDesignation, ActionType, Status, Action, Remarks, ActionedAt, Source) VALUES
(4, 1, '00010210', 'Manager 002',        'Manager',        'REVIEW',          'COMPLETED', 'REVIEWED',  'Reviewed. Budget seems excessive for furniture. Forwarding.', '2026-06-11 15:00:00', 'DYNAMIC'),
(4, 2, '00010220', 'Senior Manager 002', 'Senior Manager', 'REVIEW',          'COMPLETED', 'REJECTED',  'REJECTED: Budget of PKR 800,000 for office furniture is not justified. Please revise with standard options.', '2026-06-13 10:30:00', 'DYNAMIC'),
(4, 3, '00010220', 'Senior Manager 002', 'Senior Manager', 'FINANCE_REVIEW',  'SKIPPED',   NULL,        NULL, NULL, 'DYNAMIC'),
(4, 4, '00010230', 'General Manager 002','General Manager','APPROVE',          'SKIPPED',   NULL,        NULL, NULL, 'DYNAMIC');

-- Request 5 (RETURNED): HR -> 2 levels, returned at stage 1
INSERT INTO dbo.WorkflowStages (MinuteSheetId, StageOrder, ActionerPNo, ActionerName, ActionerDesignation, ActionType, Status, Action, Remarks, ActionedAt, Source) VALUES
(5, 1, '00010310', 'Manager 003',        'Manager',        'REVIEW',  'COMPLETED', 'RETURNED',  'RETURNED: Employee consent forms are missing. Please attach signed consent forms before resubmitting.', '2026-06-19 16:00:00', 'HYBRID'),
(5, 2, '00010320', 'Senior Manager 003', 'Senior Manager', 'APPROVE', 'PENDING',   NULL,        NULL, NULL, 'HYBRID');

-- Request 6 (IN_REVIEW): IT 75K -> 2 levels, at stage 1
INSERT INTO dbo.WorkflowStages (MinuteSheetId, StageOrder, ActionerPNo, ActionerName, ActionerDesignation, ActionType, Status, Action, Remarks, ActionedAt, Source) VALUES
(6, 1, '00010110', 'Manager 001',        'Manager',        'REVIEW',  'ACTIVE',    NULL, NULL, NULL, 'DYNAMIC'),
(6, 2, '00010120', 'Senior Manager 001', 'Senior Manager', 'APPROVE', 'PENDING',   NULL, NULL, NULL, 'DYNAMIC');
GO

-- ============================================================================
-- 11. WORKFLOW HISTORY (audit trail)
-- ============================================================================

-- Request 1 (full lifecycle)
INSERT INTO dbo.WorkflowHistory (MinuteSheetId, ActionerPNo, ActionerName, Action, PreviousStatus, NewStatus, Remarks, StageOrder, Timestamp) VALUES
(1, '00010101', 'Employee 001',       'CREATE',  'DRAFT',     'DRAFT',     'Request created.',                                                   NULL, '2026-06-14 16:00:00'),
(1, '00010101', 'Employee 001',       'SUBMIT',  'DRAFT',     'IN_REVIEW', 'Request submitted. Dynamic route generated with 4 stages.',           NULL, '2026-06-15 09:30:00'),
(1, '00010110', 'Manager 001',        'REVIEW',  'IN_REVIEW', 'IN_REVIEW', 'Quotations verified. Equipment specifications are appropriate.',      1,    '2026-06-16 14:30:00'),
(1, '00010120', 'Senior Manager 001', 'REVIEW',  'IN_REVIEW', 'IN_REVIEW', 'Budget within allocation. Recommending approval.',                    2,    '2026-06-18 11:15:00'),
(1, '00010220', 'Senior Manager 002', 'REVIEW',  'IN_REVIEW', 'IN_REVIEW', 'Financial review complete. Budget allocation confirmed.',             3,    '2026-06-19 09:45:00'),
(1, '00010130', 'General Manager 001','APPROVE', 'IN_REVIEW', 'APPROVED',  'Approved. Proceed with competitive bidding.',                         4,    '2026-06-20 16:45:00');

-- Request 2 (in review)
INSERT INTO dbo.WorkflowHistory (MinuteSheetId, ActionerPNo, ActionerName, Action, PreviousStatus, NewStatus, Remarks, StageOrder, Timestamp) VALUES
(2, '00010102', 'Employee 002',       'CREATE',  'DRAFT',     'DRAFT',     'Request created.',                                                   NULL, '2026-06-21 14:30:00'),
(2, '00010102', 'Employee 002',       'SUBMIT',  'DRAFT',     'IN_REVIEW', 'Request submitted. Dynamic route generated with 2 stages.',           NULL, '2026-06-22 11:00:00'),
(2, '00010110', 'Manager 001',        'REVIEW',  'IN_REVIEW', 'IN_REVIEW', 'Team building approved. Ensure safety measures.',                     1,    '2026-06-24 10:30:00');

-- Request 3 (draft)
INSERT INTO dbo.WorkflowHistory (MinuteSheetId, ActionerPNo, ActionerName, Action, PreviousStatus, NewStatus, Remarks, StageOrder, Timestamp) VALUES
(3, '00010101', 'Employee 001',       'CREATE',  'DRAFT',     'DRAFT',     'Request created. Pending vendor quotations.',                         NULL, '2026-06-25 09:15:00');

-- Request 4 (rejected)
INSERT INTO dbo.WorkflowHistory (MinuteSheetId, ActionerPNo, ActionerName, Action, PreviousStatus, NewStatus, Remarks, StageOrder, Timestamp) VALUES
(4, '00010201', 'Employee 007',       'CREATE',  'DRAFT',     'DRAFT',     'Request created.',                                                   NULL, '2026-06-09 11:00:00'),
(4, '00010201', 'Employee 007',       'SUBMIT',  'DRAFT',     'IN_REVIEW', 'Request submitted. Dynamic route generated with 4 stages.',           NULL, '2026-06-10 14:20:00'),
(4, '00010210', 'Manager 002',        'REVIEW',  'IN_REVIEW', 'IN_REVIEW', 'Budget seems excessive. Forwarding.',                                 1,    '2026-06-11 15:00:00'),
(4, '00010220', 'Senior Manager 002', 'REJECT',  'IN_REVIEW', 'REJECTED',  'Budget of PKR 800,000 not justified. Revise with standard options.',  2,    '2026-06-13 10:30:00');

-- Request 5 (returned)
INSERT INTO dbo.WorkflowHistory (MinuteSheetId, ActionerPNo, ActionerName, Action, PreviousStatus, NewStatus, Remarks, StageOrder, Timestamp) VALUES
(5, '00010301', 'Employee 013',       'CREATE',  'DRAFT',     'DRAFT',     'Request created. Marked confidential.',                               NULL, '2026-06-17 10:00:00'),
(5, '00010301', 'Employee 013',       'SUBMIT',  'DRAFT',     'IN_REVIEW', 'Request submitted. Hybrid route with 2 stages.',                      NULL, '2026-06-18 08:45:00'),
(5, '00010310', 'Manager 003',        'RETURN',  'IN_REVIEW', 'RETURNED',  'Employee consent forms missing. Attach before resubmitting.',          1,    '2026-06-19 16:00:00');

-- Request 6 (just submitted)
INSERT INTO dbo.WorkflowHistory (MinuteSheetId, ActionerPNo, ActionerName, Action, PreviousStatus, NewStatus, Remarks, StageOrder, Timestamp) VALUES
(6, '00010101', 'Employee 001',       'CREATE',  'DRAFT',     'DRAFT',     'Urgent request created for network infrastructure.',                  NULL, '2026-06-27 16:30:00'),
(6, '00010101', 'Employee 001',       'SUBMIT',  'DRAFT',     'IN_REVIEW', 'Request submitted. Dynamic route generated with 2 stages.',           NULL, '2026-06-28 10:15:00');
GO

-- ============================================================================
-- 12. WORKFLOW EXCEPTIONS (pre-seeded for edge case employees)
-- ============================================================================
INSERT INTO dbo.WorkflowExceptions (MinuteSheetId, EmployeePNo, ExceptionType, Description, Severity, IsResolved) VALUES
(NULL, '00090001', 'MISSING_MANAGER',     'Employee 00090001 (Employee Missing Manager) has ManagerPNo 00999999 which does not exist in the employee directory.',                    'ERROR',    0),
(NULL, '00090030', 'SELF_MANAGER',        'Employee 00090030 (Employee Self Manager) has ManagerPNo set to their own PNo 00090030. This creates an invalid self-referencing hierarchy.', 'CRITICAL', 0),
(NULL, '00090040', 'CIRCULAR_HIERARCHY',  'Circular hierarchy detected: 00090040 -> 00090041 -> 00090042 -> 00090040. This creates an infinite loop in the approval chain.',        'CRITICAL', 0),
(NULL, '00090021', 'INACTIVE_MANAGER',    'Employee 00090021 reports to 00090020 (Inactive Manager) who is marked as inactive (left on 2025-12-31).',                                'WARNING',  0),
(NULL, '00010601', 'INSUFFICIENT_LEVELS', 'Employee 00010601 chain: Manager 006 -> SM 006 -> GM 006 -> HOF 006 (top). Financial requests requiring 4+ levels will exhaust the chain at 4 levels.', 'WARNING', 0);
GO

-- ============================================================================
-- 13. AI ANALYSIS RESULTS (sample analyses)
-- ============================================================================
INSERT INTO dbo.AiAnalysisResults
    (MinuteSheetId, Summary, DetectedBudget, Impact, Beneficiaries, Urgency, RiskLevel,
     SuggestedCategory, SuggestedSubject, MissingInformation, RiskFlags, SuggestedRoute,
     ReviewerChecklist, SuggestedWorkflowMode, SuggestedLevels, ModelUsed, RawResponse)
VALUES
(1,
 'This request proposes the procurement of 50 laptops for the ICT department to replace outdated equipment that is over 5 years old. The current hardware is causing frequent failures and cannot support new software requirements. The estimated budget is PKR 250,000. Two vendor quotations have been attached. Delivery is expected within 45 days of approval.',
 250000.00,
 'Improves equipment availability, reduces downtime, and enables new software adoption across the ICT department.',
 '["ICT Department", "Development Team", "End Users"]',
 'High',
 'Medium',
 'Financial',
 'Procurement of IT Equipment - ICT Department Laptop Replacement',
 '["Expected completion/delivery timeline could be more specific", "Warranty terms not mentioned in request body", "Disposal plan for old equipment not addressed"]',
 '["Budget exceeds PKR 100,000 - requires 3+ approval levels and finance review", "Procurement via competitive bidding should be verified against procurement policy"]',
 '["Manager (Review)", "Senior Manager (Review)", "Finance Reviewer (Finance Review)", "General Manager (Approve)"]',
 '["Verify budget against annual IT capital expenditure allocation", "Confirm competitive bidding process compliance", "Check if similar procurement was done in the last 12 months", "Verify technical specifications meet department requirements", "Ensure proper asset tagging and inventory plan for new equipment"]',
 'DYNAMIC', 3, 'mock-ai-v1',
 '{"summary":"This request proposes procurement of 50 laptops...","detectedBudget":250000,"impact":"Improves availability and reduces downtime.","beneficiaries":["ICT Department","Development Team","End Users"],"urgency":"High","riskLevel":"Medium","suggestedCategory":"Financial","missingInformation":["Expected completion timeline","Warranty terms","Disposal plan"],"suggestedRoute":["Manager","Senior Manager","Finance Reviewer","General Manager"]}');

INSERT INTO dbo.AiAnalysisResults
    (MinuteSheetId, Summary, DetectedBudget, Impact, Beneficiaries, Urgency, RiskLevel,
     SuggestedCategory, SuggestedSubject, MissingInformation, RiskFlags, SuggestedRoute,
     ReviewerChecklist, SuggestedWorkflowMode, SuggestedLevels, ModelUsed)
VALUES
(6,
 'Urgent request for server room network infrastructure upgrade. The scope includes replacing aging network switches (10+ years old), installing redundant fiber links, and upgrading firewall appliances. The estimated budget is PKR 75,000. Current infrastructure is causing intermittent outages affecting all departments.',
 75000.00,
 'Resolves network outages affecting all departments. Improves network reliability and security.',
 '["All Departments", "IT Infrastructure Team"]',
 'Critical',
 'Medium',
 'IT',
 'Urgent Network Infrastructure Upgrade - Server Room Switches and Firewall',
 '["Specific models/brands of replacement equipment not mentioned", "Downtime plan during installation not provided", "Backup connectivity plan not addressed"]',
 '["Marked as URGENT - verify urgency justification", "Infrastructure change affecting all departments requires change management review"]',
 '["Manager (Review)", "Senior Manager (Approve)"]',
 '["Verify urgency claim against recent outage logs", "Confirm replacement equipment compatibility", "Review installation timeline and downtime impact", "Check if maintenance contract covers failing equipment"]',
 'DYNAMIC', 2, 'mock-ai-v1');
GO


-- ============================================================================
-- 14. VERIFICATION QUERIES
-- ============================================================================

PRINT '';
PRINT '====================================================================';
PRINT 'SEED DATA VERIFICATION';
PRINT '====================================================================';

PRINT '';
PRINT '--- Table Row Counts ---';
SELECT 'Departments'            AS TableName, COUNT(*) AS RowCount FROM dbo.Departments
UNION ALL SELECT 'Employees',            COUNT(*) FROM dbo.Employees
UNION ALL SELECT 'Roles',                COUNT(*) FROM dbo.Roles
UNION ALL SELECT 'Users',                COUNT(*) FROM dbo.Users
UNION ALL SELECT 'UserRoles',            COUNT(*) FROM dbo.UserRoles
UNION ALL SELECT 'RequestTypes',         COUNT(*) FROM dbo.RequestTypes
UNION ALL SELECT 'WorkflowRules',        COUNT(*) FROM dbo.WorkflowRules
UNION ALL SELECT 'MinuteSheetRequests',  COUNT(*) FROM dbo.MinuteSheetRequests
UNION ALL SELECT 'MinuteSheetAttachments', COUNT(*) FROM dbo.MinuteSheetAttachments
UNION ALL SELECT 'WorkflowStages',       COUNT(*) FROM dbo.WorkflowStages
UNION ALL SELECT 'WorkflowHistory',      COUNT(*) FROM dbo.WorkflowHistory
UNION ALL SELECT 'WorkflowExceptions',   COUNT(*) FROM dbo.WorkflowExceptions
UNION ALL SELECT 'AiAnalysisResults',    COUNT(*) FROM dbo.AiAnalysisResults
ORDER BY TableName;

PRINT '';
PRINT '--- Employee Hierarchy (ICT Department 14002001) ---';
SELECT e.PNo, e.Name, e.Designation, e.ManagerPNo, m.Name AS ManagerName, e.IsActive
FROM dbo.Employees e
LEFT JOIN dbo.Employees m ON e.ManagerPNo = m.PNo
WHERE e.DepartmentId = 14002001
ORDER BY e.PNo;

PRINT '';
PRINT '--- Edge Case Employees ---';
SELECT e.PNo, e.Name, e.ManagerPNo,
    CASE
        WHEN e.ManagerPNo = e.PNo THEN 'SELF_MANAGER'
        WHEN e.IsActive = 0 THEN 'INACTIVE'
        WHEN NOT EXISTS (SELECT 1 FROM dbo.Employees m WHERE m.PNo = e.ManagerPNo) AND e.ManagerPNo IS NOT NULL THEN 'MISSING_MANAGER'
        ELSE 'NORMAL'
    END AS EdgeCase
FROM dbo.Employees e
WHERE e.PNo IN ('00090001','00090020','00090021','00090030','00090040','00090041','00090042');

PRINT '';
PRINT '--- Circular Hierarchy Test ---';
SELECT 'A->B' AS Link, a.PNo, a.Name, a.ManagerPNo FROM dbo.Employees a WHERE a.PNo = '00090040'
UNION ALL
SELECT 'B->C', b.PNo, b.Name, b.ManagerPNo FROM dbo.Employees b WHERE b.PNo = '00090041'
UNION ALL
SELECT 'C->A', c.PNo, c.Name, c.ManagerPNo FROM dbo.Employees c WHERE c.PNo = '00090042';

PRINT '';
PRINT '--- Minute Sheet Request Summary ---';
SELECT ms.Id, ms.ReferenceNumber, ms.Status, ms.WorkflowMode, ms.EstimatedBudget,
       rt.Code AS RequestType, ms.RequesterPNo, ms.CurrentActionerPNo,
       (SELECT COUNT(*) FROM dbo.WorkflowStages ws WHERE ws.MinuteSheetId = ms.Id) AS StageCount,
       (SELECT COUNT(*) FROM dbo.WorkflowHistory wh WHERE wh.MinuteSheetId = ms.Id) AS HistoryCount
FROM dbo.MinuteSheetRequests ms
JOIN dbo.RequestTypes rt ON ms.RequestTypeId = rt.Id
ORDER BY ms.Id;

PRINT '';
PRINT '--- Users with Roles ---';
SELECT u.Id, u.Username, u.EmployeePNo, e.Name AS EmployeeName,
       STRING_AGG(r.Name, ', ') AS Roles
FROM dbo.Users u
LEFT JOIN dbo.Employees e ON u.EmployeePNo = e.PNo
LEFT JOIN dbo.UserRoles ur ON u.Id = ur.UserId
LEFT JOIN dbo.Roles r ON ur.RoleId = r.Id
GROUP BY u.Id, u.Username, u.EmployeePNo, e.Name
ORDER BY u.Id;

PRINT '';
PRINT '====================================================================';
PRINT 'Seed data complete. All verification queries executed.';
PRINT '====================================================================';
GO
