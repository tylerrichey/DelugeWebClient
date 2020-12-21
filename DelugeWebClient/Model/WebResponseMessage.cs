using Newtonsoft.Json;
using System;

namespace Deluge.Model
{
    internal class WebResponseMessage<T> 
    {      

        [JsonProperty(PropertyName = "id")]
        public int ResponseId { get; set; }

        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }

        [JsonProperty(PropertyName = "error")]
        public WebResponseError Error { get; set; }
    }
}
