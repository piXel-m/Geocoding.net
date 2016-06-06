﻿using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Geocoding.MapQuest
{
    /// <summary>
    /// Geo-code request object
    /// <see cref="http://open.mapquestapi.com/geocoding/"/>
    /// </summary>
    public abstract class BaseRequest
    {
        protected BaseRequest(string key) //output only, no need for default ctor
        {
            Key = key;
        }

        [JsonIgnore]
        private string key;

        /// <summary>
        /// A REQUIRED unique key to authorize use of the Routing Service.
        /// <see cref="http://developer.mapquest.com/"/>
        /// </summary>
        [JsonIgnore]
        public virtual string Key
        {
            get { return key; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("An application key is required for MapQuest");

                key = value;
            }
        }

        /// <summary>
        /// Defaults to json
        /// </summary>
        [JsonIgnore]
        public virtual DataFormat InputFormat { get; private set; }

        /// <summary>
        /// Defaults to json
        /// </summary>
        [JsonIgnore]
        public virtual DataFormat OutputFormat { get; private set; }

        [JsonIgnore]
        private RequestOptions op = new RequestOptions();

        /// <summary>
        /// Optional settings
        /// </summary>
        [JsonProperty("options")]
        public virtual RequestOptions Options
        {
            get { return op; }
            protected set
            {
                if (value == null)
                    throw new ArgumentNullException("Options");

                op = value;
            }
        }

        /// <summary>
        /// if true, use Open Street Map, else use commercial map
        /// </summary>
        public virtual bool UseOSM { get; set; }

        /// <summary>
        /// We are using v1 of MapQuest OSM API
        /// </summary>
        protected virtual string BaseRequestPath
        {
            get
            {
                if (UseOSM)
                    return @"http://open.mapquestapi.com/geocoding/v1/";
                else
                    return @"http://www.mapquestapi.com/geocoding/v1/";
            }
        }

        /// <summary>
        /// The full path for the request
        /// </summary>
        [JsonIgnore]
        public virtual Uri RequestUri
        {
            get
            {
                var sb = new StringBuilder(BaseRequestPath);
                sb.Append(RequestAction);
                sb.Append("?");
                //no need to escape this key, it is already escaped by MapQuest at generation
                //http://www.mapquest.com/v1/searchahead/items?boundingBox=52.621392697207625,13.684844970703125,51.91462637924619,15.088348388671873&clientid=yogi&cookieid=myCookie&limit=7&query=Fra&token=yogisa0

                sb.AppendFormat("key={0}&", Key);
                if (BoundingBox != null)
                    sb.AppendFormat("boundingBox={0}&", string.Join(",", BoundingBox));

                if (InputFormat != DataFormat.json)
                    sb.AppendFormat("inFormat={0}&", InputFormat);

                if (OutputFormat != DataFormat.json)
                    sb.AppendFormat("outFormat={0}&", OutputFormat);

                sb.Length--;
                return new Uri(sb.ToString());
            }
        }

        [JsonIgnore]
        public float[] BoundingBox { get; set; }

        [JsonIgnore]
        public abstract string RequestAction { get; }

        [JsonIgnore]
        private string _verb = "POST";

        /// <summary>
        /// Default request verb is POST for security and large batch payloads
        /// </summary>
        [JsonIgnore]
        public virtual string RequestVerb
        {
            get { return _verb; }
            protected set { _verb = string.IsNullOrWhiteSpace(value) ? "POST" : value.Trim().ToUpper(); }
        }

        /// <summary>
        /// Request body if request verb is applicable (POST, PUT, etc)
        /// </summary>
        [JsonIgnore]
        public virtual string RequestBody
        {
            get
            {
                return this.ToJSON();
            }
        }

        public override string ToString()
        {
            return this.RequestBody;
        }
    }
}