using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    public static class VnPayLibrary
    {
        public static string CreatePaymentUrl(IConfiguration config, HttpContext context, string orderId, decimal amount)
        {
            var vnpayConfig = config.GetSection("VnPaySettings");
            var tmnCode = vnpayConfig["TmnCode"];
            var hashSecret = vnpayConfig["HashSecret"];
            var baseUrl = vnpayConfig["BaseUrl"];
            var returnUrl = vnpayConfig["ReturnUrl"];
            var version = vnpayConfig["Version"];

            var pay = new VnPayRequest();
            pay.AddRequestData("vnp_Version", version);
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", tmnCode);
            pay.AddRequestData("vnp_Amount", ((long)amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {orderId}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);
            pay.AddRequestData("vnp_TxnRef", orderId.ToString());

            string paymentUrl = pay.CreateRequestUrl(baseUrl, hashSecret);
            return paymentUrl;
        }

        public static VnPayResponse GetFullResponseData(IQueryCollection collection, string hashSecret)
        {
            var response = new VnPayResponse();
            foreach (var (key, value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    response.AddResponseData(key, value);
                }
            }

            // Lấy các giá trị từ response và gán vào thuộc tính
            response.vnp_TxnRef = response.GetResponseData("vnp_TxnRef");
            response.vnp_ResponseCode = response.GetResponseData("vnp_ResponseCode");
            response.vnp_TransactionStatus = response.GetResponseData("vnp_TransactionStatus");
            response.vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;

            bool checkSignature = response.ValidateSignature(hashSecret);

            if (!checkSignature)
            {
                return null;
            }
            return response;
        }

        private static string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    }
                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();
                }
            }
            catch (Exception)
            {
                ipAddress = "127.0.0.1";
            }
            return ipAddress;
        }

        // <<< CHUYỂN HÀM NÀY VÀO BÊN TRONG LỚP VnPayLibrary >>>
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }

    public class VnPayRequest
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayComparer());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();
            foreach (var (key, value) in _requestData)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
                }
            }
            string queryString = data.ToString();

            baseUrl += "?" + queryString;
            string signData = queryString.Remove(data.Length - 1, 1);
            string vnp_SecureHash = VnPayLibrary.HmacSHA512(hashSecret, signData); // Gọi hàm từ VnPayLibrary
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            return baseUrl;
        }
    }

    public class VnPayResponse
    {
        private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayComparer());

        // <<< THÊM CÁC THUỘC TÍNH CÒN THIẾU >>>
        public string vnp_TxnRef { get; set; }
        public string vnp_ResponseCode { get; set; }
        public string vnp_TransactionStatus { get; set; }
        public string vnp_SecureHash { get; set; }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            _responseData.TryGetValue(key, out string value);
            return value;
        }

        public bool ValidateSignature(string secretKey)
        {
            var data = new StringBuilder();
            foreach (var (key, value) in _responseData)
            {
                if (!string.IsNullOrEmpty(value) && key != "vnp_SecureHash")
                {
                    data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
                }
            }
            string checkData = data.ToString().Remove(data.Length - 1, 1);
            string checkSignature = VnPayLibrary.HmacSHA512(secretKey, checkData); // Gọi hàm từ VnPayLibrary
            return checkSignature.Equals(this.vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class VnPayComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return string.Compare(x, y, StringComparison.Ordinal);
        }
    }
}