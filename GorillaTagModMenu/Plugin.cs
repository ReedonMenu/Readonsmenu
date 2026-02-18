using System.Collections.Generic;
using BepInEx;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTagModMenu;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.readonsmenu.gorillatag.modmenu";
    public const string PluginName = "Readons Gorilla Tag Menu";
    public const string PluginVersion = "1.1.0";

    private enum MenuAction
    {
        ToggleNoclip,
        ToggleSpeed,
        ToggleKickGun,
        IncreaseSpeed,
        DecreaseSpeed
    }

    private readonly Dictionary<Collider, MenuAction> vrButtonActions = new();
    private Rect windowRect = new(20f, 20f, 320f, 220f);

    private bool showMenu;
    private bool noclipEnabled;
    private bool speedBoostEnabled;
    private bool kickGunEnabled;

    private bool wasPrimaryButtonPressed;
    private bool wasTriggerPressed;
    private bool triggerPressedThisFrame;

    private float moveSpeed = 9f;

    private GameObject menuRoot;
    private Transform buttonContainer;

    private void Update()
    {
        UpdateVrTriggerState();
        HandleVrMenuToggle();
        HandleVrMenuInteraction();

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            showMenu = !showMenu;
            SetVrMenuActive(showMenu);
        }

        if (showMenu && menuRoot != null)
        {
            UpdateVrButtonLabels();
        }

        HandleNoclip();
        HandleSpeedBoost();
        HandleKickGun();
    }

    private void OnGUI()
    {
        if (!showMenu)
        {
            return;
        }

        windowRect = GUI.Window(7331, windowRect, DrawWindow, $"{PluginName} v{PluginVersion}");
    }

    private void DrawWindow(int id)
    {
        GUILayout.BeginVertical();
        noclipEnabled = GUILayout.Toggle(noclipEnabled, "Noclip");
        speedBoostEnabled = GUILayout.Toggle(speedBoostEnabled, "Speed Boost");
        kickGunEnabled = GUILayout.Toggle(kickGunEnabled, "Kick Gun");

        GUILayout.Space(10f);
        GUILayout.Label($"Move Speed: {moveSpeed:F1}");
        moveSpeed = GUILayout.HorizontalSlider(moveSpeed, 5f, 18f);

        GUILayout.Space(12f);
        GUILayout.Label("Insert or Right A Button = Toggle menu");
        GUILayout.Label("VR Trigger = Press menu buttons / Kick Gun trigger");
        GUILayout.EndVertical();

        GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
    }

    private void UpdateVrTriggerState()
    {
        triggerPressedThisFrame = false;

        var rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!rightHand.isValid)
        {
            wasTriggerPressed = false;
            return;
        }

        rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out var triggerPressed);
        triggerPressedThisFrame = triggerPressed && !wasTriggerPressed;
        wasTriggerPressed = triggerPressed;
    }

    private void HandleVrMenuToggle()
    {
        var rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!rightHand.isValid)
        {
            return;
        }

        rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out var primaryButtonPressed);

        if (primaryButtonPressed && !wasPrimaryButtonPressed)
        {
            showMenu = !showMenu;
            EnsureVrMenu();
            PositionMenuInFrontOfPlayer();
            SetVrMenuActive(showMenu);
        }

        wasPrimaryButtonPressed = primaryButtonPressed;
    }

    private void HandleVrMenuInteraction()
    {
        if (!showMenu)
        {
            return;
        }

        var rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!rightHand.isValid || !triggerPressedThisFrame)
        {
            return;
        }

        if (rightHand.TryGetFeatureValue(CommonUsages.devicePosition, out var position) &&
            rightHand.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation))
        {
            var ray = new Ray(position, rotation * Vector3.forward);
            if (Physics.Raycast(ray, out var hit, 6f) && vrButtonActions.TryGetValue(hit.collider, out var action))
            {
                ApplyMenuAction(action);
            }
        }
    }

    private void ApplyMenuAction(MenuAction action)
    {
        switch (action)
        {
            case MenuAction.ToggleNoclip:
                noclipEnabled = !noclipEnabled;
                break;
            case MenuAction.ToggleSpeed:
                speedBoostEnabled = !speedBoostEnabled;
                break;
            case MenuAction.ToggleKickGun:
                kickGunEnabled = !kickGunEnabled;
                break;
            case MenuAction.IncreaseSpeed:
                moveSpeed = Mathf.Clamp(moveSpeed + 1f, 5f, 18f);
                break;
            case MenuAction.DecreaseSpeed:
                moveSpeed = Mathf.Clamp(moveSpeed - 1f, 5f, 18f);
                break;
        }

        UpdateVrButtonLabels();
    }

    private void EnsureVrMenu()
    {
        if (menuRoot != null)
        {
            return;
        }

        menuRoot = new GameObject("ReadonsVrMenu");
        var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.transform.SetParent(menuRoot.transform, false);
        panel.transform.localScale = new Vector3(0.3f, 0.4f, 0.01f);
        panel.GetComponent<Renderer>().material.color = new Color(0.06f, 0.06f, 0.06f, 0.85f);

        buttonContainer = new GameObject("Buttons").transform;
        buttonContainer.SetParent(menuRoot.transform, false);

        CreateVrButton(new Vector3(0f, 0.13f, -0.01f), MenuAction.ToggleNoclip, "Noclip: OFF");
        CreateVrButton(new Vector3(0f, 0.05f, -0.01f), MenuAction.ToggleSpeed, "Speed: OFF");
        CreateVrButton(new Vector3(0f, -0.03f, -0.01f), MenuAction.ToggleKickGun, "Kick Gun: OFF");
        CreateVrButton(new Vector3(-0.08f, -0.11f, -0.01f), MenuAction.DecreaseSpeed, "Speed -");
        CreateVrButton(new Vector3(0.08f, -0.11f, -0.01f), MenuAction.IncreaseSpeed, "Speed +");

        SetVrMenuActive(false);
    }

    private void CreateVrButton(Vector3 localPosition, MenuAction action, string text)
    {
        var button = GameObject.CreatePrimitive(PrimitiveType.Cube);
        button.transform.SetParent(buttonContainer, false);
        button.transform.localPosition = localPosition;
        button.transform.localScale = new Vector3(0.22f, 0.06f, 0.02f);
        button.GetComponent<Renderer>().material.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        var textObj = new GameObject("Label");
        textObj.transform.SetParent(button.transform, false);
        textObj.transform.localPosition = new Vector3(0f, 0f, -0.013f);

        var textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 60;
        textMesh.characterSize = 0.003f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.white;

        if (button.TryGetComponent<Collider>(out var collider))
        {
            vrButtonActions[collider] = action;
        }
    }

    private void UpdateVrButtonLabels()
    {
        if (buttonContainer == null)
        {
            return;
        }

        for (var i = 0; i < buttonContainer.childCount; i++)
        {
            var button = buttonContainer.GetChild(i);
            var label = button.GetComponentInChildren<TextMesh>();
            var collider = button.GetComponent<Collider>();
            if (label == null || collider == null || !vrButtonActions.TryGetValue(collider, out var action))
            {
                continue;
            }

            switch (action)
            {
                case MenuAction.ToggleNoclip:
                    label.text = $"Noclip: {(noclipEnabled ? "ON" : "OFF")}";
                    break;
                case MenuAction.ToggleSpeed:
                    label.text = $"Speed: {(speedBoostEnabled ? "ON" : "OFF")}";
                    break;
                case MenuAction.ToggleKickGun:
                    label.text = $"Kick Gun: {(kickGunEnabled ? "ON" : "OFF")}";
                    break;
                case MenuAction.IncreaseSpeed:
                    label.text = $"Speed + ({moveSpeed:F1})";
                    break;
                case MenuAction.DecreaseSpeed:
                    label.text = $"Speed - ({moveSpeed:F1})";
                    break;
            }
        }
    }

    private void PositionMenuInFrontOfPlayer()
    {
        if (Camera.main == null || menuRoot == null)
        {
            return;
        }

        var cameraTransform = Camera.main.transform;
        var flatForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        if (flatForward.sqrMagnitude <= 0.001f)
        {
            flatForward = cameraTransform.forward;
        }

        menuRoot.transform.position = cameraTransform.position + flatForward * 0.6f;
        menuRoot.transform.rotation = Quaternion.LookRotation(-flatForward, Vector3.up);
    }

    private void SetVrMenuActive(bool active)
    {
        EnsureVrMenu();
        menuRoot.SetActive(active);
    }

    private void HandleNoclip()
    {
        if (Camera.main == null)
        {
            return;
        }

        var localPlayer = Camera.main.transform.root;
        var colliders = localPlayer.GetComponentsInChildren<Collider>(false);
        foreach (var col in colliders)
        {
            col.enabled = !noclipEnabled;
        }
    }

    private void HandleSpeedBoost()
    {
        if (!speedBoostEnabled || Camera.main == null)
        {
            return;
        }

        var transformRef = Camera.main.transform;
        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += transformRef.forward;
        if (Input.GetKey(KeyCode.S)) direction -= transformRef.forward;
        if (Input.GetKey(KeyCode.D)) direction += transformRef.right;
        if (Input.GetKey(KeyCode.A)) direction -= transformRef.right;

        direction.y = 0f;
        if (direction.sqrMagnitude > 0f)
        {
            transformRef.root.position += direction.normalized * (moveSpeed * Time.deltaTime);
        }
    }

    private void HandleKickGun()
    {
        if (!kickGunEnabled || Camera.main == null)
        {
            return;
        }

        var mouseTrigger = Input.GetMouseButtonDown(0);
        var vrTrigger = triggerPressedThisFrame;
        if (!mouseTrigger && !vrTrigger)
        {
            return;
        }

        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (!Physics.Raycast(ray, out var hitInfo, 500f))
        {
            return;
        }

        var targetView = hitInfo.collider.GetComponentInParent<PhotonView>();
        if (targetView?.Owner == null)
        {
            return;
        }

        TryKickPlayer(targetView.Owner);
    }

    private void TryKickPlayer(Player target)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Logger.LogWarning("Kick Gun requires Master Client.");
            return;
        }

        if (target.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Logger.LogInfo("Refusing to kick local player.");
            return;
        }

        PhotonNetwork.CloseConnection(target);
        Logger.LogInfo($"Kick attempted for {target.NickName} (#{target.ActorNumber}).");
    }
}
