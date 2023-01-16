--Mặt hàng
--1
create table DNHOMHANG (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
--2
create table DPHANLOAI (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
--3
create table DSIZE (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
--4
create table DDANG (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
--5
create table DMAU (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
--6
create table DTHOIGIANDAT (
	ID varchar(36) primary key,
	NAME nvarchar(255)
);
--7
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
	NOIBAT int,
	HANGSALE int,
	BANAMKHO int,
	TIMECREATED datetime not null,
);
--8
create table DSIZECHITIET (
	ID varchar(36) primary key,
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
	DSIZEID varchar(36) foreign key references DSIZE(ID),
);
--9
create table DMAUCHITIET (
	ID varchar(36) primary key,
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
	DMAUID varchar(36) foreign key references DMAU(ID),
);
--10
create table DANH (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
);

--đơn hàng
--11
create table DNHANVIEN (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DIENTHOAI nvarchar(255),
	DIACHI nvarchar(255),
	NOTE nvarchar(255),
	LOAITAIKHOAN int,
);
--12
create table SUSER (
	ID varchar(36) primary key,
	USERNAME nvarchar(255),
	PASSWORD nvarchar(255),
	DNHANVIENID varchar(36) foreign key references DNHANVIEN(ID),
	ISADMIN int,
);
--13
create table DNHAXE (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DIENTHOAI nvarchar(255),
	BIENSO nvarchar(255),
	GIOXECHAY nvarchar(255),
	BENDO nvarchar(255),
	XEOM int
);
--14
create table DNHOMKHACHHANG (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	LOAIGIA int,
);
--15
create table DKHACHHANG (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DIENTHOAI nvarchar(255),
	TINHTHANH nvarchar(255),
	QUANHUYEN nvarchar(255),
	PHUONGXA nvarchar(255),
	DIACHI nvarchar(255),
	NOTE nvarchar(255),
	USERNAME nvarchar(255) not null,
	PASSWORD nvarchar(255) not null,
	DNHOMKHACHHANGID varchar(36) foreign key references DNHOMKHACHHANG(ID),
	LOAIVANCHUYEN int,
	DNHAXEID varchar(36) foreign key references DNHAXE(ID)
);
--16
create table DTRANGTHAIDON (
	ID varchar(36) primary key,
	NAME nvarchar(255),
);
--17
create table TDONHANG (
	ID varchar(36) primary key,
	NGAY datetime not null,
	NAME nvarchar(255),
	TIENHANG bigint,
	PHIVANCUYEN nvarchar(255),
	TONGCONG bigint,
	TINHTHANH nvarchar(255),
	QUANHUYEN nvarchar(255),
	PHUONGXA nvarchar(255),
	DIACHI nvarchar(255),
	DKHACHHANGID varchar(36) foreign key references DKHACHHANG(ID),
	LOAIGIA int,
	LOAIVANCHUYEN int,
	DNHAXEID varchar(36) foreign key references DNHAXE(ID),
	LABEL_GHTK nvarchar(255),
	DNHANVIENID varchar(36) foreign key references DNHANVIEN(ID),
	DTRANGTHAIDONID varchar(36) foreign key references DTRANGTHAIDON(ID),
);
--18
create table TDONHANGCHITIET (
	ID varchar(36) primary key,
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
	DSIZEID varchar(36) foreign key references DSIZE(ID),
	DMAUID varchar(36) foreign key references DMAU(ID),
	TDONHANGID varchar(36) foreign key references TDONHANG(ID),
	DONGIA bigint,
	SOLUONG bigint,
	THANHTIEN bigint,
	NOTE nvarchar(255),
)

insert into SUSER(ID, USERNAME, PASSWORD, ISADMIN)
values ('anhduong-823d-4502-bea8-67068da44fff', 'admin', '019113EB7DF662AAB056F68E4382BF87', 30)