using RestSharp.Deserializers;

namespace BrokerService.Models
{
    public class TickerReponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Rank { get; set; }
        [DeserializeAs(Name = "price_usd")]
        public string PriceUsd { get; set; }
        [DeserializeAs(Name = "price_btc")]
        public string PriceBtc { get; set; }
        [DeserializeAs(Name = "24h_volume_usd")]
        public string VolumeUsd24H { get; set; }
        [DeserializeAs(Name = "market_cap_usd")]
        public string MarketCapUsd { get; set; }
        [DeserializeAs(Name = "available_supply")]
        public string AvailableSupply { get; set; }
        [DeserializeAs(Name = "total_supply")]
        public string TotalSupply { get; set; }
        [DeserializeAs(Name = "max_supply")]
        public string MaxSupply { get; set; }
        [DeserializeAs(Name = "percent_change_1h")]
        public string PercentChange1H { get; set; }
        [DeserializeAs(Name = "percent_change_24h")]
        public string PercentChange24H { get; set; }
        [DeserializeAs(Name = "percent_change_7d")]
        public string PercentChange7D { get; set; }
        [DeserializeAs(Name = "last_updated")]
        public string LastUpdated { get; set; }
    }
}