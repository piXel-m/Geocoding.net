using System.IO;
using Microsoft.Extensions.Configuration;

namespace Geocoding.Tests
{
    public class MapQuestBatchGeocoderTest : BatchGeocoderTest
	{
		protected override IBatchGeocoder CreateBatchGeocoder()
		{
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string k = config["AppSettings:mapQuestKey"];
			return new MapQuest.MapQuestGeocoder(k);
		}
	}
}
