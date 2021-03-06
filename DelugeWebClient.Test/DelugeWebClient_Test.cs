﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Deluge.Model;

namespace Deluge.Test
{
    [TestClass]
    public class DelugeWebClient_Test
    {
        public string DelugeUrl { get; }
        public string DelugePassword { get; }

        public DelugeWebClient_Test()
        {
            //todo: fix
            string[] l = File.ReadAllLines(@"D:\Dev\SilverCard.Deluge\delugeurl.txt");

            DelugeUrl = l[0];
            DelugePassword = l[1];
        }

        [TestMethod]
        public async Task GetConfig_Test()
        {
            using DelugeWebClient client = new DelugeWebClient(DelugeUrl);
            await client.LoginAsync(DelugePassword);
            var r = await client.GetConfigAsync();
            await client.LogoutAsync();
        }

        [TestMethod]
        public async Task GetSessionStatus_Test()
        {
            using DelugeWebClient client = new DelugeWebClient(DelugeUrl);
            await client.LoginAsync(DelugePassword);
            var r = await client.GetSessionStatusAsync();
            await client.LogoutAsync();

        }

        [TestMethod]
        public async Task GetTorrentsStatusAsync_Test()
        {
            using DelugeWebClient client = new DelugeWebClient(DelugeUrl);
            await client.LoginAsync(DelugePassword);
            var r = await client.GetTorrentsStatusAsync();
            await client.LogoutAsync();
        }

        [TestMethod]
        public async Task AddRemoveTorrentMagnet_Test()
        {
            using DelugeWebClient client = new DelugeWebClient(DelugeUrl);
            await client.LoginAsync(DelugePassword);
            var torrentId = await client.AddTorrentMagnetAsync("magnet:?xt=urn:btih:30987c19cf0eae3cf47766f387c621fa78a58ab9&dn=debian-9.2.1-amd64-netinst.iso", new TorrentOptions() { MoveCompletedPath = "/etc/linux-iso" });
            Thread.Sleep(1000);
            var r2 = await client.RemoveTorrentAsync(torrentId, true);
            Assert.IsTrue(r2);
            await client.LogoutAsync();
        }
    }
}
