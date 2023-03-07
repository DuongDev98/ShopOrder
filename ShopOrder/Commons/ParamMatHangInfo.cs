namespace ShopOrder.Commons
{
    public class ParamMatHangInfo
    {
        public ParamMatHangInfo()
        {
            page = 1;
            loaiMatHang = -1;
            search = "";
            dangId = new string[] { };
            mauId = new string[] { };
            nhomId = new string[] { };
            phanLoaiId = new string[] { };
            shopId = new string[] { };
            sizeId = new string[] { };
            thoiGianId = new string[] { };
        }

        public int page { set; get; }
        public int loaiMatHang { set; get; }
        public string search { set; get; }
        public string[] dangId { set; get; }
        public string[] mauId { set; get; }
        public string[] nhomId { set; get; }
        public string[] phanLoaiId { set; get; }
        public string[] shopId { set; get; }
        public string[] sizeId { set; get; }
        public string[] thoiGianId { set; get; }
    }
}