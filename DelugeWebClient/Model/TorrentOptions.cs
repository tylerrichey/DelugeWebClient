using System;
using Newtonsoft.Json;

namespace Deluge.Model
{
    public class TorrentOptions
    {
        [JsonProperty(PropertyName = "move_completed_path")]
        public String MoveCompletedPath { get; set; }
    }
}
