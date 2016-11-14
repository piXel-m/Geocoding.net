using System.Collections.Generic;
using System.IO;
using Geocoding.MapQuest;
using Xunit;

using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Geocoding.Tests
{
    public class MapQuestGeocoderTest : GeocoderTest
	{
		protected override IGeocoder CreateGeocoder()
		{
			var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
			string k = config["AppSettings:mapQuestKey"];
			return new MapQuestGeocoder(k);
		}

		[Theory]
		[InlineData("Wilshire & Bundy, Los Angeles")]
		[InlineData("Fried St & 2nd St, Gretna, LA 70053")]
		public override void CanGeocodeWithSpecialCharacters(string address)
		{
			base.CanGeocodeWithSpecialCharacters(address);
		}

        [Theory]
        [InlineData("Frankfurt")]
        public void CanGeocodeWithBoundingBox(string address)
        {
            var mapquest = (geocoder as MapQuestGeocoder);
            mapquest.BoundingBox = new[]
            {
                52.4270f,14.3584f,
                52.2365f, 14.7018f
            };

            Address[] addresses = mapquest.Geocode(address).ToArray();
            addresses[0].AssertFrankfurtOder();

            mapquest.BoundingBox = null;
        }
	}
}