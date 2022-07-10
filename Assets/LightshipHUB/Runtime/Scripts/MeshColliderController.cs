using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_EDITOR)
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using Niantic.ARDK.AR;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Extensions.Meshing;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine.EventSystems;

namespace Niantic.ARDK.Templates 
{
    
    public class MeshColliderController : MonoBehaviour 
    {
        [HideInInspector]
        public ObjectHolderController OHcontroller;
        private bool isEggSpawn = true;

        private string[] allowTapOverCanvas = { "TapToPlaceCanvas", "PowerUpCanvas", };
        void Update() 
        {
            if (PlatformAgnosticInput.touchCount <= 0) { return; }
    
            var touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase == TouchPhase.Began) 
            {
                var isTouchOverUI = CheckTouchUI(touch.position);
                if (!isTouchOverUI)
                {
                    GameObject obj = Instantiate(OHcontroller.ObjectHolder, this.transform);
                    obj.SetActive(true);


                    Vector3 entrancePoint = OHcontroller.Camera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, OHcontroller.Camera.nearClipPlane));
                    Rigidbody rb = obj.GetComponent<Rigidbody>();
                    rb.velocity = new Vector3(0f, 0f, 0f);
                    rb.angularVelocity = new Vector3(0f, 0f, 0f);

                    obj.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
                    obj.transform.position = entrancePoint;

                    float force = 200.0f;
                    rb.AddForce(OHcontroller.Camera.transform.forward * force);

                    if (isEggSpawn)
                    {
                        isEggSpawn = false;
                        GameManager.instance.EggLaid();
                        var egg = obj.transform.GetChild(1).transform.GetChild(0).gameObject.AddComponent<EggManager>();
                        egg.Snake = GameManager.instance.SnakeManager;
                        egg.CallSnakeGeneration();
                    }
                    else if(GamePlayManager.instance.IsPowerUpAppleActivated)
                    {
                        Debug.Log("Apples From power Up");
                        GamePlayManager.instance.PowerUP();
                        GamePlayManager.instance.IsPowerUpAppleActivated = false;
                    }
                }
                else
                {
                    Debug.Log("Tap Over UI");
                }

            }
        }

        private bool CheckTouchUI(Vector2 touchPosition)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(touchPosition.x, touchPosition.y);

            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, result);

            if (result.Count > 0)
            {
                for (int i = 0; i < allowTapOverCanvas.Length; i++)
                {
                    if (result[0].gameObject.transform.parent.gameObject.name == allowTapOverCanvas[i])
                    {
                        return false;
                    }
                }

            }
     
            return result.Count > 0;
        }
    }
}
