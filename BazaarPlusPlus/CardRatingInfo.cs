using Newtonsoft.Json;

namespace BazaarPlusPlus
{
	public class CardRatingInfo
	{
		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("tier")]
		public string Tier { get; set; } = string.Empty;

		[JsonProperty("unduel_id")]
		public string UnDuelId { get; set; } = string.Empty;

		// CDN 图片 GUID，可能与游戏内部 TemplateId 对应
		[JsonProperty("img_guid")]
		public string ImgGuid { get; set; } = string.Empty;
	}
}
