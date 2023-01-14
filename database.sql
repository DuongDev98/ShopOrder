--Mặt hàng
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
	NOIBAT int,
	HANGSALE int,
	BANAMKHO int,
	TIMECREATED datetime not null,
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

--đơn hàng
create table DNHOMKHACHHANG (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	LOAIGIA int,
);
create table DKHACHHANG (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DIENTHOAI bigint,
	TINHTHANH nvarchar(255),
	QUANHUYEN nvarchar(255),
	PHUONGXA nvarchar(255),
	DIACHI nvarchar(255),
	NOTE nvarchar(255),
	TAIKHOAN nvarchar(255) not null,
	MATKHAU nvarchar(255) not null,
	DNHOMKHACHHANGID varchar(36) foreign key references DNHOMKHACHHANG(ID),
);
create table DNHANVIEN (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DIENTHOAI bigint,
	DIACHI nvarchar(255),
	NOTE nvarchar(255),
);
create table SUSER (
	ID varchar(36) primary key,
	USERNAME nvarchar(255),
	PASSWORD nvarchar(255),
	DNHANVIENID varchar(36) foreign key references DNHANVIEN(ID),
);
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
);
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