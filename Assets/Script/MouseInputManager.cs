using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseInputManager : MonoBehaviour
{
    public static MouseInputManager instance;

    [DllImport("Dll1")]
    private static extern bool init();

    [DllImport("Dll1")]
    private static extern bool kill();

    [DllImport("Dll1")]
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

    [SerializeField]
    GameObject prefabCursor;
    Canvas canvas;

    [SerializeField]
    Canvas canvas2;

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
        public int playerID;
        public float sensitivity;
    }

    Dictionary<int, MousePointer> pointersByDeviceId = new Dictionary<int, MousePointer>();
    int nextPlayerId = 1;
    int miceCount = 0;

    void Start()
    {
        Cursor.visible = false;
        canvas = GetComponent<Canvas>();
        bool res = init();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        kill();
        clearCursorsAndDevices();
        yield return new WaitForEndOfFrame();
        init();
    }

    int addCursor(int deviceId)
    {
        if (miceCount > 1)
        {
            Debug.LogError("Nombre de souris max atteinte");
            return -1;
        }

        if (!isInit)
        {
            Debug.LogError("Not initialized");
            return -1;
        }

        MousePointer mp = null;
        pointersByDeviceId.TryGetValue(deviceId, out mp);
        if (mp != null)
        {
            Debug.LogError("This device already has a cursor");
            return -1;
        }

        Debug.Log("Adding DeviceID " + deviceId);
        mp = new MousePointer();
        mp.playerID = nextPlayerId++;
        pointersByDeviceId[deviceId] = mp;
        mp.position = new Vector3(0, 0, 0);

        if (miceCount == 0)
        {
            mp.obj = Instantiate(prefabCursor, canvas.transform);
        }

        if (miceCount == 1)
        {
            mp.obj = Instantiate(prefabCursor, canvas2.transform);
        }
        
        ++miceCount;
        return mp.playerID;
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
                    float dx = ev.x * defaultMiceSensitivity;
                    float dy = ev.y * defaultMiceSensitivity;
                    pointer.obj.GetComponent<Image>().enabled = true;

                    pointer.obj.transform.position += new Vector3(dx, -dy, 0);

                    if (Mathf.Abs(dx) > accelerationThreshold) dx *= accelerationMultiplier;
                    if (Mathf.Abs(dy) > accelerationThreshold) dy *= accelerationMultiplier;

                    RectTransform rect = pointer.obj.transform.parent.GetComponent<RectTransform>();
                    pointer.obj.transform.position = new Vector3(
                        Mathf.Clamp(pointer.obj.transform.position.x, 0, rect.rect.width - screenBorderPixels),
                        Mathf.Clamp(pointer.obj.transform.position.y, 0, rect.rect.height - screenBorderPixels),
                        pointer.obj.transform.position.z);

                    if ((int)ev.press == 1)
                    {
                        Vector3 position = pointer.obj.transform.position;
                        Ray ray = Camera.main.ScreenPointToRay(new Vector3(position.x,position.y, 0.01f));
                        foreach (RaycastHit hit in Physics.RaycastAll(ray))
                        {
                            //Raycast monde
                        }

                        PointerEventData point = new PointerEventData(GetComponent<EventSystem>());
                        point.position = position;

                        List<RaycastResult> results = new List<RaycastResult>();

                        GetComponent<GraphicRaycaster>().Raycast(point, results);

                        foreach(RaycastResult result in results)
                        {
                            //Raycast UI
                            if (result.gameObject != pointer.obj)
                                Debug.Log(result.gameObject);
                        }
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

    void OnApplicationQuit()
    {
        kill();
    }
}
