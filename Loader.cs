using BepInEx;
using J0kerMenuTemp.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace J0kerMenuTemp.Loading
{
    [BepInPlugin("com.J0kerModZ.J0kerMenuTemp", "J0ker Menu Temp", "1.0.0")] // You Can Change This To Your Info
    public class Loader : BaseUnityPlugin
    {
        private static GameObject IfActiveLoad;
        private static bool loaded = false;

        private void Update()
        {
            IfActiveLoad = GameObject.Find("Gameplay Scripts");
            if (IfActiveLoad != null && IfActiveLoad.activeInHierarchy && !loaded)
            {
                GameObject load = new GameObject("J0ker Loader");
                load.AddComponent<Client>();
                loaded = true;
            }
        }
    }
}
