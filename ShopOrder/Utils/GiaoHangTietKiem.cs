using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace ShopOrder.Utils
{
    public class GiaoHangTietKiem
    {
        public bool isDeveloper = true;
        private const string urlDeveloper = "https://services-staging.ghtklab.com";
        private const string tokenDeveloper = "35042506a888ba0154938b31c49994663E945275";
        private const string urlProduction = "https://services.giaohangtietkiem.vn";
        private const string tokenProduction = "";

        //Lấy danh sách địa chỉ hàng
        public ResponseGhtk listPick()
        {
            string _url = "/services/shipment/list_pick_add";
            string dataJson = httpRequest((isDeveloper ? urlDeveloper : urlProduction) + _url, "", "GET");
            //dataJson = File.ReadAllText("D:/data.txt");
            if (dataJson == "") return null;
            return parseJson<ResponseGhtk>(dataJson);
        }

        //Huỷ đơn hàng
        public ResponseGhtk orderCancel(string label_id)
        {
            string _url = "/services/shipment/cancel/" + label_id;
            string dataJson = httpRequest((isDeveloper ? urlDeveloper : urlProduction) + _url, "", "POST");
            if (dataJson == "") return null;
            return parseJson<ResponseGhtk>(dataJson);
        }

        //Trạng thái đơn hàng
        public ResponseGhtk orderInfo(string label_id)
        {
            string _url = "/services/shipment/v2/" + label_id;
            string dataJson = httpRequest((isDeveloper ? urlDeveloper : urlProduction) + _url, "", "GET");
            if (dataJson == "") return null;
            return parseJson<ResponseGhtk>(dataJson);
        }

        //Tính phí vận chuyển
        public ResponseGhtk calculateFee(CalFeeRequest param)
        {
            string _url = "/services/shipment/fee?";
            _url += "pick_province" + param.pick_province;
            _url += "&pick_district" + param.pick_district;
            _url += "&province" + param.province;
            _url += "&district" + param.district;
            _url += "&address" + param.address;
            _url += "&weight" + param.weight;
            _url += "&value" + param.value;
            _url += "&transport" + param.transport;
            string dataJson = httpRequest((isDeveloper ? urlDeveloper : urlProduction) + _url, "", "GET");
            if (dataJson == "") return null;
            return parseJson<ResponseGhtk>(dataJson);
        }

        //Đăng đơn hàng
        public ResponseGhtk orderAdd(OrderAddReqquest param)
        {
            string dataJson = httpRequest("/services/shipment/order/?ver=1.5", JsonConvert.SerializeObject(param), "POST");
            if (dataJson == "")
            {
                return null;
            }
            else
            {
                return parseJson<ResponseGhtk>(dataJson);
            }
        }

        public string httpRequest(string _url, string data, string method)
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create((isDeveloper ? urlDeveloper : urlProduction) + _url);
            http.Headers.Add("token", tokenDeveloper);
            http.ContentType = "application/json; charset=utf-8";
            http.ProtocolVersion = HttpVersion.Version11;
            http.Method = method;
            if (data != "")
            {
                using (StreamWriter writer = new StreamWriter(http.GetRequestStream()))
                {
                    writer.Write(data);
                }
            }

            try
            {
                using (StreamReader reader = new StreamReader(http.GetResponse().GetResponseStream()))
                {
                    data = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                data = "";
            }
            return data;
        }

        private T parseJson<T>(string data)
        {
            JsonSerializer s = new JsonSerializer();
            return s.Deserialize<T>(new JsonTextReader(new StringReader(data)));
        }
    }

    [Serializable]
    public class Product
    {
        public string product_code { set; get; }
        public string name { set; get; }
        public decimal weight { set; get; }
        public int quantity { set; get; }
        public int price { set; get; }
    }

    [Serializable]
    public class Order
    {
        public string id { set; get; }
        public string pick_name { set; get; }
        public string pick_address { set; get; }
        public string pick_province { set; get; }
        public string pick_district { set; get; }
        public string pick_ward { set; get; }
        public string pick_tel { set; get; }
        public string pick_email { set; get; }
        //
        public string tel { set; get; }
        public string name { set; get; }
        public string address { set; get; }
        public string email { set; get; }
        public string province { set; get; }
        public string district { set; get; }
        public string ward { set; get; }
        public string hamlet { set; get; }
        //
        public string return_name { set; get; }
        public string return_address { set; get; }
        public string return_province { set; get; }
        public string return_district { set; get; }
        public string return_ward { set; get; }
        public string return_tel { set; get; }
        public string return_email { set; get; }
        //
        public string is_freeship { set; get; }
        public string pick_date { set; get; }
        public decimal pick_money { set; get; }
        public string note { set; get; }
        public decimal value { set; get; }
        public string transport { set; get; }
        //Road - fly
        public string label_id { set; get; }
        public string partner_id { set; get; }
        public string status { set; get; }
        public string status_text { set; get; }
        public string created { set; get; }
        public string modified { set; get; }
        public string message { set; get; }
        public string deliver_date { set; get; }
        public string customer_fullname { set; get; }
        public string customer_tel { set; get; }
        public string storage_day { set; get; }
        public string ship_money { set; get; }
        public string insurance { set; get; }
        public decimal weight { set; get; }
    }

    //
    public class OrderAddReqquest
    {
        public List<Product> products { set; get; }
        public Order order { set; get; }
    }

    public class ResponseGhtk
    {
        public bool success { set; get; }
        public string message { set; get; }
        public OrderResponse order { set; get; }
        public ErrorResponse error { set; get; }
        public Fee fee { set; get; }
        public List<Data> data { set; get; }
    }

    public class OrderResponse
    {
        public string partner_id { set; get; }
        public string label { set; get; }
        public string area { set; get; }
        public string fee { set; get; }
        public string insurance_fee { set; get; }
        public string estimated_pick_time { set; get; }
        public string estimated_deliver_time { set; get; }
        public List<Product> products { set; get; }
        public decimal status_id { set; get; }
    }

    public class ErrorResponse
    {
        public string code { set; get; }
        public string partner_id { set; get; }
        public string ghtk_label { set; get; }
        public string created { set; get; }
        public decimal status { set; get; }
    }

    public class CalFeeRequest
    {
        public string pick_province { set; get; }
        public string pick_district { set; get; }
        public string province { set; get; }
        public string district { set; get; }
        public string address { set; get; }
        public decimal weight { set; get; }
        public decimal value { set; get; }
        public string transport { set; get; }
    }

    public class Fee
    {
        public string name { set; get; }
        public decimal fee { set; get; }
        public decimal insurance_fee { set; get; }
        public string delivery_type { set; get; }
        public decimal a { set; get; }
        public string dt { set; get; }
        public bool delivery { set; get; }
    }

    public class Data
    {
        public string pick_address_id { set; get; }
        public string address { set; get; }
        public string pick_tel { set; get; }
        public string pick_name { set; get; }
    }
}