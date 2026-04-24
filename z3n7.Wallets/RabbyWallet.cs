using System;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
namespace z3nIO.Wallets
{

    public class RabbyWallet : IWallet
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly Logger _log;
        private readonly string _extId = "acmacodkjbdgmoleebolmdjonilkdbch";
        private readonly string _fileName;
        private readonly string _key;
        private readonly string _pass;

        public RabbyWallet(IZennoPosterProjectModel project, Instance instance, Logger log = null, string key = null, string fileName = "Rabby-Wallet-Chrome-Web-Store.crx")
        {
            _project = project;
            _instance = instance;
            _fileName = fileName;

            _key = KeyLoad(key);
            _pass = project.HWPass();
            _log = log;
        }
        
        private string KeyLoad(string key)
        {
            if (string.IsNullOrEmpty(key)) key = "key";

            switch (key)
            {
                case "key":
                    key = _project.DbKey("evm");
                    break;
                case "seed":
                    key = _project.DbKey("seed");
                    break;
                default:
                    return key;
            }
            if (string.IsNullOrEmpty(key)) _project.warn("keyIsEmpy", true); 

            return key;
        }

        public void Launch()
        {
            
            var em = _instance.UseFullMouseEmulation;
            _instance.UseFullMouseEmulation = true;

            _log?.Send($"Launching Rabby wallet with file {_fileName}");
            var ext = new ChromeExt(_project, _instance);
            
            ext.Switch(_extId);
            if (ext.Install(_extId, _fileName))
                Import();
            else
                Unlock();

            _instance.CloseExtraTabs();
            _instance.UseFullMouseEmulation = em;
        }

        public void Import()
        {
            _log?.Send("Importing Rabby wallet with private key");
            var key = _key;
            var password = _pass;

            try
            {
                _instance.HeClick(("button", "innertext", "I\\ already\\ have\\ an\\ address", "regexp", 0), deadline:60);
                _instance.HeClick(("img", "src", $"chrome-extension://{_extId}/generated/svgs/d5409491e847b490e71191a99ddade8b.svg", "regexp", 0));
                _instance.HeSet(("privateKey", "id"), key);
                _instance.HeClick(("button", "innertext", "Confirm", "regexp", 0));
                _instance.HeSet(("password", "id"), password);
                _instance.HeSet(("confirmPassword", "id"), password);
                _instance.HeClick(("button", "innertext", "Confirm", "regexp", 0));
                _instance.HeClick(("button", "innertext", "Get\\ Started", "regexp", 0));
                _log?.Send("Successfully imported Rabby wallet");
            }
            catch (Exception ex)
            {
                _log?.Send($"Failed to import Rabby wallet: {ex.Message}");
                throw;
            }
        }

        public void Unlock()
        {
            _log?.Send("Unlocking Rabby wallet");
            var password = _pass;

            _instance.UseFullMouseEmulation = true;

            while (_instance.ActiveTab.URL == $"chrome-extension://{_extId}/offscreen.html")
            {
                _log?.Send("Closing offscreen tab and retrying unlock");
                _instance.ActiveTab.Close();
                _instance.ActiveTab.Navigate($"chrome-extension://{_extId}/index.html#/unlock", "");
            }

            try
            {
                _instance.HeSet(("password", "id"), password);
                _instance.HeClick(("button", "innertext", "Unlock", "regexp", 0));
                _log?.Send("Wallet unlocked successfully");
            }
            catch (Exception ex)
            {
                _log?.Send($"Failed to unlock Rabby wallet: {ex.Message}");
                throw;
            }
        }
    }

}

