create table DNHOMHANG (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
create table DPHANLOAI (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
create table DSIZE (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
create table DDANG (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
create table DMAU (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
create table DTHOIGIANDAT (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
create table DMATHANG (
	ID varchar(36) primary key,
	CODE nvarchar(255),
	NAME nvarchar(255),
	DNHOMHANGID varchar(36) foreign key references DNHOMHANG(ID),
	DPHANLOAIID varchar(36) foreign key references DPHANLOAI(ID),
	DDANGID varchar(36) foreign key references DDANG(ID),
	DTHOIGIANDATID varchar(36) foreign key references DTHOIGIANDAT(ID),
	GIABAN bigint,
	GIABAN2 bigint,
	GIABAN3 bigint,
	GIABAN4 bigint,
	GIABAN5 bigint,
	GIABAN6 bigint,
	GIABAN7 bigint,
	GIABAN8 bigint,
	HANGMOIVE int,
	HANGSAPVE int,
	HANGSALE int,
	BANAMKHO int
);
create table DSIZECHITIET (
	ID varchar(36) primary key,
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
	DSIZEID varchar(36) foreign key references DSIZE(ID),
);
create table DMAUCHITIET (
	ID varchar(36) primary key,
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
	DMAUID varchar(36) foreign key references DMAU(ID),
);
create table DANH (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
);