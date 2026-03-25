using UnityEngine;

namespace J0kerMenuTemp.Menu
{
    public class Mods : MonoBehaviour
    {
        #region PreLoaded Mods
        // If you do not want these mods you can remove them!
        public static void Fly()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * 15;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        #region Platforms

        private static GameObject PlatsLeft, PlatsRight;
        private static bool PlatLSpawn, PlatRSpawn;

        public static void PlatL()
        {
            PlatsLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(PlatsLeft.GetComponent<Rigidbody>());
            Object.Destroy(PlatsLeft.GetComponent<BoxCollider>());
            Object.Destroy(PlatsLeft.GetComponent<Renderer>());
            PlatsLeft.transform.localScale = new Vector3(0.25f, 0.3f, 0.25f);

            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.transform.parent = PlatsLeft.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.1f, 1f, 1f);
            gameObject.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            gameObject.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
            gameObject.transform.position = new Vector3(0.02f, 0f, 0f);
        }

        public static void PlatR()
        {
            PlatsRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(PlatsRight.GetComponent<Rigidbody>());
            Object.Destroy(PlatsRight.GetComponent<BoxCollider>());
            Object.Destroy(PlatsRight.GetComponent<Renderer>());

            PlatsRight.transform.localScale = new Vector3(0.25f, 0.3f, 0.25f);
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.transform.parent = PlatsRight.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.1f, 1f, 1f);
            gameObject.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            gameObject.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
            gameObject.transform.position = new Vector3(-0.02f, 0f, 0f);
        }

        public static void Platforms() // This is the mod you will put on the menu
        {
            if (ControllerInputPoller.instance.leftControllerGripFloat > 0.1f && PlatsLeft == null)
            {
                PlatL();
            }
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f && PlatsRight == null)
            {
                PlatR();
            }

            if (ControllerInputPoller.instance.leftControllerGripFloat > 0.1f && PlatsLeft != null && !PlatLSpawn)
            {
                PlatsLeft.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                PlatsLeft.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                PlatLSpawn = true;
            }
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f && PlatsRight != null && !PlatRSpawn)
            {
                PlatsRight.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                PlatsRight.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
                PlatRSpawn = true;
            }

            if (!ControllerInputPoller.instance.leftGrab && PlatsLeft != null)
            {
                GameObject.Destroy(PlatsLeft);
                PlatsLeft = null;
                PlatLSpawn = false;
            }
            if (!ControllerInputPoller.instance.rightGrab && PlatsRight != null)
            {
                GameObject.Destroy(PlatsRight);
                PlatsRight = null;
                PlatRSpawn = false;
            }
        }
        #endregion

        #region Example Gun Code
        // If You Want My Guns You Can Use This

        // Make Sure To Add This
        static GameObject GunSphere;

        static void ExampleGun()
        {
            // Copy From Here
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo);
                GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GunSphere.transform.position = hitinfo.point;
                GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                GunSphere.GetComponent<Renderer>().material.color = Color.white;
                GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                GameObject.Destroy(GunSphere.GetComponent<Collider>());

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    GameObject.Destroy(GunSphere, Time.deltaTime);
                    GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;

                    // Your Code Here!
                }
            }
            if (GunSphere != null)
            {
                GameObject.Destroy(GunSphere, Time.deltaTime);
            }
            // And Stop Here
        }
        #endregion

        #endregion
    }
}
