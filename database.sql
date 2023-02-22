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
create table DNHANVIEN (
	ID varchar(36) primary key,
	CODE nvarchar(255),
	NAME nvarchar(255),
	DIENTHOAI nvarchar(255),
	DIACHI nvarchar(255),
	NOTE nvarchar(255),
	LOAITAIKHOAN int,
	CODEMATHANG nvarchar(255),
);
--8
create table DMATHANG (
	ID varchar(36) primary key,
	CODE nvarchar(255),
	NAME nvarchar(255),
	DNHOMHANGID varchar(36) foreign key references DNHOMHANG(ID),
	DPHANLOAIID varchar(36) foreign key references DPHANLOAI(ID),
	DDANGID varchar(36) foreign key references DDANG(ID),
	DTHOIGIANDATID varchar(36) foreign key references DTHOIGIANDAT(ID),
	DNHANVIENID varchar(36) foreign key references DNHANVIEN(ID),
	GIANHAP bigint,
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
	KHOILUONG decimal(18, 2),
	DAI decimal(18, 2),
	RONG decimal(18, 2),
	CAO decimal(18, 2),
	TIMECREATED datetime not null,
);
--9
create table DSIZECHITIET (
	ID varchar(36) primary key,
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
	DSIZEID varchar(36) foreign key references DSIZE(ID),
);
--10
create table DMAUCHITIET (
	ID varchar(36) primary key,
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
	DMAUID varchar(36) foreign key references DMAU(ID),
);

--đơn hàng
--11
create table SUSER (
	ID varchar(36) primary key,
	USERNAME nvarchar(255),
	PASSWORD nvarchar(255),
	DNHANVIENID varchar(36) foreign key references DNHANVIEN(ID),
	ISADMIN int,
);
--12
create table DNHAXE (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DIENTHOAI nvarchar(255),
	BIENSO nvarchar(255),
	GIOXECHAY nvarchar(255),
	BENDO nvarchar(255),
	XEOM int
);
--13
create table DNHOMKHACHHANG (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	LOAIGIA int,
);
--14
create table DKHACHHANG (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DIENTHOAI nvarchar(255),
	TINHTHANH nvarchar(255),
	QUANHUYEN nvarchar(255),
	PHUONGXA nvarchar(255),
	DIACHI nvarchar(255),
	EMAIL nvarchar(255),
	NOTE nvarchar(255),
	USERNAME nvarchar(255) not null,
	PASSWORD nvarchar(255) not null,
	DNHOMKHACHHANGID varchar(36) foreign key references DNHOMKHACHHANG(ID),
	LOAIVANCHUYEN int,
	DNHAXEID varchar(36) foreign key references DNHAXE(ID)
);
--15
create table DTRANGTHAIDON (
	ID varchar(36) primary key,
	NAME nvarchar(255),
);
--16
create table TDONHANG (
	ID varchar(36) primary key,
	TIMECREATED datetime not null,
	LOAI int not null,
	NGAY date not null,
	NAME nvarchar(255),
	TIENHANG bigint not null,
	PHIVANCHUYEN bigint not null,
	TONGCONG bigint not null,
	TIENTHANHTOAN bigint not null,
	DATHANHTOAN int not null,
	TINHTHANH nvarchar(255),
	QUANHUYEN nvarchar(255),
	PHUONGXA nvarchar(255),
	DIACHI nvarchar(255),
	DKHACHHANGID varchar(36) foreign key references DKHACHHANG(ID),
	LOAIGIA int,
	LOAIVANCHUYEN int,
	DNHAXEID varchar(36) foreign key references DNHAXE(ID),
	DNHANVIENID varchar(36) foreign key references DNHANVIEN(ID),
	TMPCODE varchar(36),
	NOTE nvarchar(max),
);
--17
create table TGIAOHANG (
	ID varchar(36) primary key,
	TIMECREATED datetime not null,
	NAME nvarchar(255),
	NGAY date not null,
	DKHACHHANGID varchar(36) foreign key references DKHACHHANG(ID),
	DNHANVIENID varchar(36) foreign key references DNHANVIEN(ID),
	TINHTHANH nvarchar(255),
	QUANHUYEN nvarchar(255),
	PHUONGXA nvarchar(255),
	DIACHI nvarchar(255),
	LOAIVANCHUYEN int,
	PHIVANCHUYEN bigint,
	DNHAXEID varchar(36) foreign key references DNHAXE(ID),
	LABEL_GHTK nvarchar(255),
	NOTE nvarchar(max),
)
--18
create table DANH (
	ID varchar(36) primary key,
	NAME nvarchar(255),
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
	TGIAOHANGID varchar(36) foreign key references TGIAOHANG(ID),
);
--19
create table TDONHANGCHITIET (
	ID varchar(36) primary key,
	DMATHANGID varchar(36) foreign key references DMATHANG(ID),
	DSIZEID varchar(36) foreign key references DSIZE(ID),
	DMAUID varchar(36) foreign key references DMAU(ID),
	TDONHANGID varchar(36) foreign key references TDONHANG(ID),
	TGIAOHANGID varchar(36) foreign key references TGIAOHANG(ID),
	DKHACHHANGID varchar(36) foreign key references DKHACHHANG(ID),
	DTRANGTHAIDONID varchar(36) foreign key references DTRANGTHAIDON(ID),
	DNHANVIENID varchar(36) foreign key references DNHANVIEN(ID),
	DONGIA bigint not null,
	SOLUONG bigint not null,
	THANHTIEN bigint not null,
	SLNHAN int,
	NOTE nvarchar(max),
)
--20
create table TLUUVET (
	ID varchar(36) primary key,
	TIMECREATED datetime not null,
	TDONHANGID varchar(36) foreign key references TDONHANG(ID),
	TDONHANGCHITIETID varchar(36) foreign key references TDONHANGCHITIET(ID),
	NOTE nvarchar(max),
)
--21
create table SCONFIG (
	ID varchar(36) primary key,
	THONGBAO nvarchar(max),
	NOIDUNGCHANTRAN nvarchar(max),
	THONGTIN nvarchar(max),
	TTSAUTHANHTOANID varchar(36) foreign key references DTRANGTHAIDON(ID),
	TTTAODIEUPHOIID varchar(36) foreign key references DTRANGTHAIDON(ID),
	TTXACNHANDANHANID varchar(36) foreign key references DTRANGTHAIDON(ID),
	TTDANHANID varchar(36) foreign key references DTRANGTHAIDON(ID),
	PICK_NAME nvarchar(255),
	PICK_PROVINCE nvarchar(255),
	PICK_DISTRICT nvarchar(255),
	PICK_WARD nvarchar(255),
	PICK_ADDRESS nvarchar(255),
	PICK_TEL nvarchar(255),
	PICK_EMAIL nvarchar(255),
)

insert into SUSER(ID, USERNAME, PASSWORD, ISADMIN)
values ('anhduong-823d-4502-bea8-67068da44fff', 'admin', '019113EB7DF662AAB056F68E4382BF87', 30)

delete from TLUUVET
delete from TDONHANGCHITIET
delete from TDONHANG
delete from DANH where coalesce(TGIAOHANGID, '') <> ''
delete from TGIAOHANG

delete from TLUUVET where not exists (select * from TDONHANGCHITIET where TDONHANGCHITIET.TDONHANGID = TLUUVET.TDONHANGID)
delete from tdonhang where not exists (select * from TDONHANGCHITIET where TDONHANGID = TDONHANG.ID)