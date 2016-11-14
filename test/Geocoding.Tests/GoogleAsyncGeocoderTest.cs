﻿using Geocoding.Google;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Geocoding.Tests
{
    public class GoogleAsyncGeocoderTest : AsyncGeocoderTest
    {
        GoogleGeocoder geoCoder;

        protected override IAsyncGeocoder CreateAsyncGeocoder()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string apiKey = config["AppSettings:googleApiKey"];

            if (String.IsNullOrEmpty(apiKey))
            {
                geoCoder = new GoogleGeocoder();
            }
            else
            {
                geoCoder = new GoogleGeocoder(apiKey);
            }

            return geoCoder;
        }

        [Theory]
        [InlineData("United States", GoogleAddressType.Country)]
        [InlineData("Illinois, US", GoogleAddressType.AdministrativeAreaLevel1)]
        [InlineData("New York, New York", GoogleAddressType.Locality)]
        [InlineData("90210, US", GoogleAddressType.PostalCode)]
        [InlineData("1600 pennsylvania ave washington dc", GoogleAddressType.StreetAddress)]
        public void CanParseAddressTypes(string address, GoogleAddressType type)
        {
            geoCoder.GeocodeAsync(address).ContinueWith(task =>
            {
                GoogleAddress[] addresses = task.Result.ToArray();
                Assert.Equal(type, addresses[0].Type);
            });
        }
    }
}