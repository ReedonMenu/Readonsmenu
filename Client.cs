using System.Collections.Generic;
using TMPro;
using UnityEngine;
using HarmonyLib;

namespace Reedon.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate", MethodType.Normal)]
    public class Client : MonoBehaviour
    {
        #region Shader/Text
        static Shader MenuShader = Shader.Find("GorillaTag/UberShader");
        static TMP_FontAsset Font = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdtext").GetComponent<TextMeshPro>().font;
        #endregion

        #region Menu Var
        static GameObject MenuPrefab, MenuPointer;
        static bool MenuActive = false;

        // 1st One Is The Normal Button Color, 2nd Is The Color When The Button Is Enabled, 3rd Is The Menus Color, 4th Is The Color Of The Menu Pointer
        static Color MenuButtonColor = Color.white, MenuButtonColorEnabled = Color.gray,  MenuColor = Color.black, MenuPointerColor = Color.white;

        static string MenuName = "J0ker Menu Temp";
        #endregion

        #region Buttons Var
        static List<string> buttonTexts = new List<string>
        {
            "Fly [B]",
            "Platforms",
            "PlaceHolder",
            "PlaceHolder",
            "PlaceHolder",
            "PlaceHolder",
            "PlaceHolder",
            "PlaceHolder",
        };
        public static List<bool> buttonFlags = new List<bool>();
        public static List<GameObject> ButtonObjects = new List<GameObject>();
        static bool ButtonsCreated;
        #endregion

        #region Page var
        static int currentPage = 0, buttonsPerPage = 4;
        static float pageSwitchCooldown = 0.5f, lastPageSwitchTime = 0f;
        #endregion

        public void Start()
        {
            Panel();
            Pointer();
            UpdateButtonVisibility();
        }

        public void Update()
        {
            if (ControllerInputPoller.instance.leftControllerSecondaryButton)
            {
                MenuActive = true;
            }
            else
            {
                MenuActive = false;
            }

            MenuPrefab.SetActive(MenuActive);
            MenuPointer.SetActive(MenuActive);

            Vector3 offset = new Vector3(0.1f, 0f, 0.1f);
            MenuPrefab.transform.parent = GorillaLocomotion.Player.Instance.leftHandFollower;

            MenuPrefab.transform.localPosition = offset;
            MenuPrefab.transform.localRotation = Quaternion.Euler(-90f, 180f, 0f);

            ButtonsActive();

            if (MenuActive && ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
            {
                NextPage();
            }

            if (MenuActive && ControllerInputPoller.instance.leftControllerIndexFloat > 0.1f)
            {
                BackPage();
            }

            // Scale The Menu With The Player (DONT DO 0.5)
            if (GorillaTagger.Instance.offlineVRRig.gameObject.transform.localScale == new Vector3(3f, 3f, 3f))
            {
                MenuPrefab.transform.localScale = new Vector3(0.06f, 1.1f, 1.1f);
            }
            else if (GorillaTagger.Instance.offlineVRRig.gameObject.transform.localScale == new Vector3(1f, 1f, 1f))
            {
                MenuPrefab.transform.localScale = new Vector3(0.02f, 0.3f, 0.3f);
            }
        }

        static void ButtonsActive()
        {
            #region Color
            for (int i = 0; i < buttonFlags.Count; i++)
            {
                if (!buttonFlags[i])
                {
                    GameObject button = ButtonObjects[i];
                    button.GetComponent<Renderer>().material.color = MenuButtonColor;
                }
                else
                {
                    GameObject button = ButtonObjects[i];
                    button.GetComponent<Renderer>().material.color = MenuButtonColorEnabled;
                }
            }
            #endregion

            if (buttonFlags[0]) Mods.Fly();

            if (buttonFlags[1]) Mods.Platforms();
        }

        static void Panel()
        {
            MenuPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(MenuPrefab.GetComponent<BoxCollider>());

            MenuPrefab.name = "Mod Menu";
            MenuPrefab.GetComponent<Renderer>().material.shader = MenuShader;
            MenuPrefab.GetComponent<Renderer>().material.color = MenuColor;

            GameObject textObject = new GameObject("MenuText");
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();

            textMesh.text = $"{MenuName}\n====================";
            textMesh.fontSize = 8;
            textMesh.font = Font;
            textMesh.color = Color.white;
            textMesh.fontStyle = FontStyles.Italic;
            textMesh.alignment = TextAlignmentOptions.Center;

            textObject.transform.SetParent(MenuPrefab.transform);
            textObject.transform.localPosition = new Vector3(-0.6f, 0.40f, 0f);
            textObject.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            textObject.transform.localScale = Vector3.one * 0.1f;

            CreateButtons(buttonTexts.Count);
        }

        static void Pointer()
        {
            MenuPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MenuPointer.name = "Menu Pointer";
            MenuPointer.tag = "Pointer";
            MenuPointer.GetComponent<Renderer>().material.shader = MenuShader;
            MenuPointer.GetComponent<Renderer>().material.color = MenuPointerColor;

            SphereCollider pointerCollider = MenuPointer.AddComponent<SphereCollider>();
            pointerCollider.isTrigger = true;
            pointerCollider.radius = 0.05f;

            MenuPointer.transform.parent = GorillaLocomotion.Player.Instance.rightControllerTransform;
            MenuPointer.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            MenuPointer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }

        static void CreateButtons(int numberOfButtons)
        {
            if (!ButtonsCreated)
            {
                float buttonHeight = 0.1f;
                float buttonSpacing = 0.08f;

                for (int i = 0; i < numberOfButtons; i++)
                {
                    GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    button.name = "Button " + (i + 1);
                    button.tag = "Button";
                    button.AddComponent<BoxCollider>().isTrigger = true;
                    button.GetComponent<Renderer>().material.shader = MenuShader;
                    button.GetComponent<Renderer>().material.color = MenuButtonColor;

                    button.transform.parent = MenuPrefab.transform;
                    button.transform.localScale = new Vector3(0.5f, 0.1f, 0.8f);
                    button.transform.localPosition = new Vector3(-0.6f, 0.2f - ((i % buttonsPerPage) * (buttonHeight + buttonSpacing)), 0f);

                    GameObject buttonTextObject = new GameObject("ButtonText");
                    TextMeshPro buttonText = buttonTextObject.AddComponent<TextMeshPro>();

                    buttonText.text = buttonTexts[i];
                    buttonText.fontSize = 8;
                    buttonText.font = Font;
                    buttonText.color = Color.black;
                    buttonText.alignment = TextAlignmentOptions.Center;

                    buttonTextObject.transform.SetParent(button.transform);
                    buttonTextObject.transform.localPosition = new Vector3(-0.6f, 0f, 0f);
                    buttonTextObject.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    buttonTextObject.transform.localScale = new Vector3(0.06f, 0.5f, 1f);

                    button.AddComponent<ButtonTrigger>();
                    ButtonObjects.Add(button);

                    ButtonTrigger buttonTrigger = button.GetComponent<ButtonTrigger>();
                    buttonTrigger.SetButtonIndex(i);

                    buttonFlags.Add(false);
                }
                ButtonsCreated = true;
            }
        }

        static void UpdateButtonVisibility()
        {
            for (int i = 0; i < ButtonObjects.Count; i++)
            {
                bool onCurrentPage = i / buttonsPerPage == currentPage;
                ButtonObjects[i].SetActive(onCurrentPage);
            }
        }

        static void NextPage()
        {
            if (Time.time - lastPageSwitchTime >= pageSwitchCooldown)
            {
                currentPage = (currentPage + 1) % Mathf.CeilToInt((float)buttonTexts.Count / buttonsPerPage);
                lastPageSwitchTime = Time.time;
                UpdateButtonVisibility();
            }
        }

        static void BackPage()
        {
            if (Time.time - lastPageSwitchTime >= pageSwitchCooldown)
            {
                if (currentPage > 0)
                {
                    currentPage = (currentPage - 1) % Mathf.CeilToInt((float)buttonTexts.Count / buttonsPerPage);
                }
                else
                {
                    currentPage = Mathf.CeilToInt((float)buttonTexts.Count / buttonsPerPage) - 1;
                }
                lastPageSwitchTime = Time.time;
                UpdateButtonVisibility();
            }
        }
    }

    public class ButtonTrigger : MonoBehaviour
    {
        private int buttonIndex = -1;
        private float cooldownTime = 0.5f;
        private float lastClickTime = 0f;

        public void SetButtonIndex(int index)
        {
            buttonIndex = index;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "Menu Pointer")
            {
                if (Time.time - lastClickTime >= cooldownTime)
                {
                    if (buttonIndex >= 0 && buttonIndex < Client.buttonFlags.Count)
                    {
                        Client.buttonFlags[buttonIndex] = !Client.buttonFlags[buttonIndex];
                    }

                    lastClickTime = Time.time;
                }
            }
        }
    }
}