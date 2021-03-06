﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Deluge.Test
{
    [TestClass]
    public class Utils_Test
    {
        private class TestClass
        {
            [JsonProperty(PropertyName = "propa")]
            public int propa { get; set; }

            [JsonProperty(PropertyName = "propb")]
            public int propb { get; set; }
        }


        [TestMethod]
        public void GetAllJsonPropertyFromType_Test()
        {
            var propNames = Utils.GetAllJsonPropertyFromType(typeof(TestClass));
            CollectionAssert.AreEqual(new string[] { "propa", "propb" }, propNames);
        }
    }
}
