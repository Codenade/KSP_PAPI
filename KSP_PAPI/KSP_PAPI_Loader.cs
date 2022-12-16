//#define POSITIONING // define for in game keybinds to bring the papi into position
//#define LIGHTSADJUST // define for in game keybinds to change different properties of the attached lights

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KSP_PAPI
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, true)]
    public class KSP_PAPI_Loader : MonoBehaviour
    {
        // TODO: not urgent: do some cleanup
        AssetBundle assets;
        GameObject prefab;
        GameObject ksc_papi_09;
        GameObject ksc_papi_27;
        Mesh mesh;
        Texture2D texture2D;
        Shader shader;
        Material material;
#if POSITIONING || LIGHTSADJUST
        Dictionary<string, KeyBinding> keyBindings;
#endif
        Vector3 ksc_papi_27_position;
        Quaternion ksc_papi_27_rotation;
        Vector3 ksc_papi_09_position;
        Quaternion ksc_papi_09_rotation;

        public void Awake()
        {
            Log(MethodBase.GetCurrentMethod());

            Log("Loading asset bundle \'" + Assembly.GetExecutingAssembly().Location.Replace(Path.GetFileName(Assembly.GetExecutingAssembly().Location), "ksp_papi.assetbundle") + "\'");
            assets = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace(Path.GetFileName(Assembly.GetExecutingAssembly().Location), "ksp_papi.assetbundle"));

            Log("Loading \'Assets/KSP_PAPI/KSP_PAPI.fbx\'");
            mesh = assets.LoadAsset<Mesh>("Assets/KSP_PAPI/KSP_PAPI.fbx");
            Log("Loading \'Assets/KSP_PAPI/KSP_PAPI.png\'");
            texture2D = assets.LoadAsset<Texture2D>("Assets/KSP_PAPI/KSP_PAPI.png");
            Log("Loading \'Assets/KSP_PAPI/KSP_PAPI.shader\'");
            shader = assets.LoadAsset<Shader>("Assets/KSP_PAPI/KSP_PAPI.shader");
            Log("Loading \'Assets/KSP_PAPI/KSP_PAPI.mat\'");
            material = assets.LoadAsset<Material>("Assets/KSP_PAPI/KSP_PAPI.mat");
            Log("Loading \'Assets/KSP_PAPI/KSP_PAPI.prefab\'");
            prefab = assets.LoadAsset<GameObject>("Assets/KSP_PAPI/KSP_PAPI.prefab");
            Log("Finished loading assets");

#if POSITIONING || LIGHTSADJUST           
            keyBindings = new Dictionary<string, KeyBinding>();
#endif
            ksc_papi_27_position = new Vector3(62, 0, 1014);
            ksc_papi_27_rotation = Quaternion.Euler(0, 90, 0);
            ksc_papi_09_position = new Vector3(-62, 0, -1014);
            ksc_papi_09_rotation = Quaternion.Euler(0, -90, 0);
        }

        public void Start()
        {
            Log(MethodBase.GetCurrentMethod());

            DontDestroyOnLoad(this);

#if POSITIONING
            keyBindings.Add("mod_x", new KeyBinding(KeyCode.J));
            keyBindings.Add("mod_y", new KeyBinding(KeyCode.K));
            keyBindings.Add("mod_z", new KeyBinding(KeyCode.L));
            keyBindings.Add("mod_n", new KeyBinding(KeyCode.I));
            keyBindings.Add("mod_r", new KeyBinding(KeyCode.O));
            keyBindings.Add("mod_c", new KeyBinding(KeyCode.H));
            keyBindings.Add("mod_rst", new KeyBinding(KeyCode.P));
#endif
#if LIGHTSADJUST
            keyBindings.Add("light_size+", new KeyBinding(KeyCode.J));
            keyBindings.Add("light_size-", new KeyBinding(KeyCode.K));
            keyBindings.Add("light_show", new KeyBinding(KeyCode.H));
#endif
        }

        public void Update()
        {
            if (ksc_papi_27 == null)
            {
                CreatePAPI(ref ksc_papi_27, GameObject.Find("runway_collider"), ksc_papi_27_position, ksc_papi_27_rotation);
            }
            if (ksc_papi_09 == null)
            {
                CreatePAPI(ref ksc_papi_09, GameObject.Find("runway_collider"), ksc_papi_09_position, ksc_papi_09_rotation);
            }
        }

        public void FixedUpdate()
        {
#if POSITIONING
            else
            {
                Vector3 a = new Vector3(keyBindings["mod_x"].GetKey(true) ? 1 : 0, keyBindings["mod_y"].GetKey(true) ? 1 : 0, keyBindings["mod_z"].GetKey(true) ? 1 : 0);
                if (a != Vector3.zero)
                {
                    if (keyBindings["mod_r"].GetKey(true))
                    {
                        if (keyBindings["mod_n"].GetKey(true))
                        {
                            defaultRotation = Quaternion.Euler(defaultRotation.eulerAngles - a);
                        }
                        else
                        {
                            defaultRotation = Quaternion.Euler(defaultRotation.eulerAngles + a);
                        }
                    }
                    else if (keyBindings["mod_n"].GetKey(true))
                    {
                        adjustment -= a;
                    }
                    else
                    {
                        adjustment += a;
                    }
                }
                if (keyBindings["mod_c"].GetKeyDown(true))
                {
                    Log($"----- testPapi properties -----{Environment.NewLine}" +
                        $"transform.position = {testPapi.transform.position}{Environment.NewLine}" +
                        $"transform.localPosition = {testPapi.transform.localPosition}{Environment.NewLine}" +
                        $"transform.rotation.eulerAngles = {testPapi.transform.rotation.eulerAngles}{Environment.NewLine}" +
                        $"transform.localRotation.eulerAngles = {testPapi.transform.localRotation.eulerAngles}{Environment.NewLine}" +
                        $"-------------------------------");
                }
                if (keyBindings["mod_rst"].GetKeyDown(true))
                {
                    adjustment = new Vector3(48, 0, 1014);
                    defaultRotation = Quaternion.Euler(0, 0, 0);
                }
            }
#endif
#if LIGHTSADJUST
            if (keyBindings["light_size+"].GetKey(true) && testPapi != null)
            {
                testPapi.GetComponent<KSP_PAPI>().LightSize += 0.05f;
            }
            else if (keyBindings["light_size-"].GetKey(true) && testPapi != null)
            {
                testPapi.GetComponent<KSP_PAPI>().LightSize -= 0.05f;
            }
            else if (keyBindings["light_show"].GetKeyDown(true))
            {
                Log(testPapi.GetComponent<KSP_PAPI>().LightSize);
            }
#endif
        }

        void CreatePAPI(ref GameObject papi, GameObject parent, Vector3 position, Quaternion rotation)
        {
            Log(MethodBase.GetCurrentMethod());
            if (parent != null)
            {
                papi = Instantiate(prefab);
                papi.AddComponent<KSP_PAPI>();
                papi.transform.parent = parent.transform;
                papi.transform.localPosition = position;
                papi.transform.localRotation = rotation;
                Log("Spawned new PAPI");
            }
            else
            {
                LogError("No parent set: CreatePAPI()");
            }
        }

        void Log(object o)
        {
            Debug.Log("[KSP_PAPI]: " + o);
        }

        void LogError(object o)
        {
            Debug.LogError("[KSP_PAPI]: (ERROR) -> " + o);
        }
    }
}
