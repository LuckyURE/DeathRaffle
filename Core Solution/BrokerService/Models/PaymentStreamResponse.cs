//using System.Globalization;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
//
//namespace BrokerService.Models
//{
//    public class PaymentStreamResponse
//    {
//    
//        public partial class Welcome
//    {
//        [JsonProperty("_links")]
//        public Links Links { get; set; }
//    }
//
//    public partial class Links
//    {
//        [JsonProperty("self")]
//        public Effects Self { get; set; }
//
//        [JsonProperty("transaction")]
//        public Effects Transaction { get; set; }
//
//        [JsonProperty("effects")]
//        public Effects Effects { get; set; }
//
//        [JsonProperty("succeeds")]
//        public Effects Succeeds { get; set; }
//
//        [JsonProperty("precedes")]
//        public Effects Precedes { get; set; }
//
//        [JsonProperty("id")]
//        public string Id { get; set; }
//
//        [JsonProperty("paging_token")]
//        public string PagingToken { get; set; }
//
//        [JsonProperty("source_account")]
//        public string SourceAccount { get; set; }
//
//        [JsonProperty("type")]
//        public string Type { get; set; }
//
//        [JsonProperty("type_i")]
//        public long TypeI { get; set; }
//
//        [JsonProperty("created_at")]
//        public System.DateTimeOffset CreatedAt { get; set; }
//
//        [JsonProperty("transaction_hash")]
//        public string TransactionHash { get; set; }
//
//        [JsonProperty("asset_type")]
//        public string AssetType { get; set; }
//
//        [JsonProperty("from")]
//        public string From { get; set; }
//
//        [JsonProperty("to")]
//        public string To { get; set; }
//
//        [JsonProperty("amount")]
//        public string Amount { get; set; }
//    }
//
//    public partial class Effects
//    {
//        [JsonProperty("href")]
//        public string Href { get; set; }
//    }
//
//    public partial class Welcome
//    {
//        public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, QuickType.Converter.Settings);
//    }
//
//    internal class Converter
//    {
//        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
//        {
//            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
//            DateParseHandling = DateParseHandling.None,
//            Converters = { 
//                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
//            },
//        };
//    }
//    }
//}