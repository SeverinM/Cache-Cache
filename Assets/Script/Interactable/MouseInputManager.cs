using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseInputManager : MonoBehaviour
{

    public enum MouseButton
    {
        NONE = 0,
        LEFT_BUTTON,
        RIGHT_BUTTON,
        MIDDLE_BUTTON,
    }

    enum ActionType
    {
        RELEASED = 1,
        PRESSED,
        MOVEMENT
    }

    public static MouseInputManager instance;

    [DllImport("RawInput")]
    private static extern bool init();

    [DllImport("RawInput")]
    private static extern bool kill();

    [DllImport("RawInput")]
    private static extern IntPtr poll();

    public const byte RE_DEVICE_CONNECT = 0;
    public const byte RE_MOUSE = 2;
    public const byte RE_DEVICE_DISCONNECT = 1;
    public string getEventName(byte id)
    {
        switch (id)
        {
            case RE_DEVICE_CONNECT: return "RE_DEVICE_CONNECT";
            case RE_DEVICE_DISCONNECT: return "RE_DEVICE_DISCONNECT";
            case RE_MOUSE: return "RE_MOUSE";
        }
        return "UNKNOWN(" + id + ")";
    }

    public float defaultMiceSensitivity = 1f;
    public float accelerationThreshold = 40;
    public float accelerationMultiplier = 2;
    public int screenBorderPixels = 16;

    int idFirstMoving = 0;
    int activeMice = 0;

    [SerializeField]
    GameObject prefabCursor;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    Canvas canvas2;

    [SerializeField]
    Camera camera1;

    [SerializeField]
    Camera camera2;

    [SerializeField]
    Sprite defaultSprite;

    [SerializeField]
    Sprite holdSprite;

    [StructLayout(LayoutKind.Sequential)]
    public struct RawInputEvent
    {
        public int devHandle;
        public int x, y, wheel;
        public byte press;
        public byte release;
        public byte type;
    }

    public class MousePointer
    {
        public GameObject obj;
        public Vector2 position;
        public int deviceID;
        public float sensitivity;
        public Vector2 delta;
        public List<Interactable> pointing = new List<Interactable>();
        public Camera cam;
        public Canvas canv;
        public List<Interactable> holding = new List<Interactable>();
        public Vector3 lastCollisionPoint;
    }

    Dictionary<int, MousePointer> pointersByDeviceId = new Dictionary<int, MousePointer>();
    int nextPlayerId = 1;
    int miceCount = 0;

    void Start()
    {
        Cursor.visible = Debug.isDebugBuild;
        bool res = init();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        kill();
        yield return new WaitForEndOfFrame();
        clearCursorsAndDevices();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        init();
    }

    void addCursor(int deviceId)
    {
        if (pointersByDeviceId.ContainsKey(deviceId)) return;

        if (!isInit)
        {
            Debug.LogError("Not initialized");
            return;
        }

        MousePointer mp = null;
        pointersByDeviceId.TryGetValue(deviceId, out mp);
        if (mp != null)
        {
            Debug.LogError("This device already has a cursor");
            return;
        }

        mp = new MousePointer();
        mp.deviceID = deviceId;
        pointersByDeviceId[deviceId] = mp;
        mp.position = new Vector3(0, 0, 0);
        mp.sensitivity = defaultMiceSensitivity;

        mp.obj = Instantiate(prefabCursor,miceCount == 0 ? canvas.transform : canvas2.transform);
        mp.cam = (miceCount == 0 ? camera1 : camera2);
        mp.canv = (miceCount == 0 ? canvas : canvas2);

        
        ++miceCount;
    }

    void deleteCursor(int deviceId)
    {
        --miceCount;
        var mp = pointersByDeviceId[deviceId];
        pointersByDeviceId.Remove(mp.deviceID);
        Destroy(mp.obj);
    }


    void clearCursorsAndDevices()
    {
        pointersByDeviceId.Clear();
        nextPlayerId = 1;
        miceCount = 0;
    }

    // Update is called once per frame
    int lastEvents = 0;
    bool isInit = true;
    void Update()
    {
        IntPtr data = poll();
        int numEvents = Marshal.ReadInt32(data);
        if (numEvents > 0) lastEvents = numEvents;
        for (int i = 0; i < numEvents; ++i)
        {
            var ev = new RawInputEvent();
            long offset = data.ToInt64() + sizeof(int) + i * Marshal.SizeOf(ev);
            ev.devHandle = Marshal.ReadInt32(new IntPtr(offset + 0));
            ev.x = Marshal.ReadInt32(new IntPtr(offset + 4));
            ev.y = Marshal.ReadInt32(new IntPtr(offset + 8));
            ev.wheel = Marshal.ReadInt32(new IntPtr(offset + 12));
            ev.press = Marshal.ReadByte(new IntPtr(offset + 16));
            ev.release = Marshal.ReadByte(new IntPtr(offset + 17));
            ev.type = Marshal.ReadByte(new IntPtr(offset + 18));

            if (ev.type == RE_DEVICE_CONNECT) addCursor(ev.devHandle);
            else if (ev.type == RE_DEVICE_DISCONNECT) deleteCursor(ev.devHandle);
            else if (ev.type == RE_MOUSE)
            {
                MousePointer pointer = null;
                if (pointersByDeviceId.TryGetValue(ev.devHandle, out pointer))
                {
                    float dx = ev.x * pointer.sensitivity;
                    float dy = ev.y * pointer.sensitivity;
                    pointer.delta = new Vector2(dx, dy);

                    //Invisible while not moving

                    pointer.obj.transform.position += new Vector3(dx, -dy, 0);

                    if (Mathf.Abs(dx) > accelerationThreshold) dx *= accelerationMultiplier;
                    if (Mathf.Abs(dy) > accelerationThreshold) dy *= accelerationMultiplier;

                    RectTransform rect = pointer.obj.transform.parent.GetComponent<RectTransform>();
                    pointer.obj.transform.position = new Vector3(
                        Mathf.Clamp(pointer.obj.transform.position.x, 0, rect.rect.width - screenBorderPixels),
                        Mathf.Clamp(pointer.obj.transform.position.y, 0, rect.rect.height - screenBorderPixels),
                        pointer.obj.transform.position.z);

                    if ((int)ev.press != 0)
                    {
                        Interaction(pointer, (MouseButton) ev.press , ActionType.PRESSED);
                    }

                    if ((int)ev.release != 0)
                    {
                        Interaction(pointer, (MouseButton)ev.release, ActionType.RELEASED);
                    }

                    if (dx != 0 || dy != 0)
                    {
                        Interaction(pointer, MouseButton.NONE, ActionType.MOVEMENT);
                    }
                }
                else
                {
                    Debug.Log("Unknown device found");
                    addCursor(ev.devHandle);
                }
            }
        }
        Marshal.FreeCoTaskMem(data);
    }

    void Interaction(MousePointer pointer , MouseButton btn , ActionType act)
    {
        //Ne se passe plus rien sur le click droit
        if (btn == MouseButton.RIGHT_BUTTON) return;

        if (act.Equals(ActionType.PRESSED))
        {
            pointer.obj.GetComponent<Image>().enabled = true;
            Manager.GetInstance().PlaySound(pointer, "Play_click_mouse");
        }

        Dictionary<Interactable, Vector3> output = new Dictionary<Interactable, Vector3>();
        Vector3 position = pointer.obj.transform.position;
        position += new Vector3(-pointer.obj.GetComponent<RectTransform>().rect.width * 0.166f, pointer.obj.GetComponent<RectTransform>().rect.width * 0.5f, 0);
        Ray ray = pointer.cam.ScreenPointToRay(new Vector3(position.x, position.y, 0.01f));

        foreach (RaycastHit hit in Physics.RaycastAll(ray).OrderBy(x => Vector3.Distance(x.point, pointer.cam.transform.position)))
        {
            Interactable inter = hit.collider.GetComponent<Interactable>();
            if (inter)
            {
                output[inter] = hit.point;
                if (inter.Block) break;
            }
        }

        PointerEventData point = new PointerEventData(GetComponent<EventSystem>());
        point.position = position;

        //IMPORTANT
#if !UNITY_EDITOR
        if (pointer.cam == camera2)
        {
            point.position += new Vector2(camera1.pixelWidth, 0);
        }
#endif

        //Same but for UI
        List<RaycastResult> results = new List<RaycastResult>();
        pointer.canv.GetComponent<GraphicRaycaster>().Raycast(point, results);

        foreach(RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Interactable>())
            {
                output[result.gameObject.GetComponent<Interactable>()] = Vector3.zero;
            }
        }


        Interpret(btn, act, output, pointer);
    }

    void Interpret(MouseButton mouseBtn , ActionType act , Dictionary<Interactable, Vector3> allGob, MousePointer mouse)
    {
        if (allGob.Keys.Where(x => x.IsHandCursor()).Count() > 0)
            mouse.obj.GetComponent<Image>().sprite = holdSprite;
        else
            mouse.obj.GetComponent<Image>().sprite = defaultSprite;

        if (act.Equals(ActionType.PRESSED))
        {
            allGob.Keys.ToList().ForEach(x =>
            {
                if (x.CanInteract)
                {
                    mouse.lastCollisionPoint = allGob[x];
                    x.MouseDown(mouseBtn, mouse);
                    if (x.Echo)
                    {
                        x.Echo.MouseDown(mouseBtn, mouse, x);
                    }
                }
            });
            mouse.holding = allGob.Keys.ToList();
        }

        if (act.Equals(ActionType.RELEASED))
        {
            mouse.holding.ForEach(x =>
            {
                if (x.CanInteract)
                {
                    if (mouse.holding.Contains(x))
                    {
                        x.MouseUp(mouseBtn, mouse);
                        if (x.Echo)
                        {
                            x.Echo.MouseUp(mouseBtn, mouse, x);
                        }
                    }
                }
            });
            mouse.holding.Clear();
        }

        if (act.Equals(ActionType.MOVEMENT))
        {
            //Just entering
            foreach(Interactable inter in allGob.Keys)
            {
                if (!mouse.pointing.Contains(inter))
                {
                    if (inter.CanInteract)
                    {
                        inter.MouseEnter(MouseButton.NONE, mouse);
                        if (inter.Echo)
                        {
                            inter.Echo.MouseEnter(MouseButton.NONE, mouse, inter);
                        }
                    }                    
                }
            }

            //Just exiting
            foreach(Interactable inter in mouse.pointing)
            {
                if(!allGob.Keys.ToList().Contains(inter))
                {
                    if (inter.CanInteract)
                    {
                        inter.MouseLeave(MouseButton.NONE, mouse);
                        if (inter.Echo)
                        {
                            inter.Echo.MouseLeave(MouseButton.NONE, mouse, inter);
                        }
                    }      
                }
            }

            mouse.holding.ForEach(x =>
            {
                if (x.CanInteract)
                {
                    x.MouseMove(mouseBtn, mouse);
                    if (x.Echo)
                    {
                        x.Echo.MouseMove(mouseBtn, mouse, x);
                    }
                }               
            });
        }
    }

    void OnApplicationQuit()
    {
        kill();
    }
}
