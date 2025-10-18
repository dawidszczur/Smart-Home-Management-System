HomeSystem homeSystem = HomeSystem.GetInstance;

Notifications logger = new Notifications();
homeSystem.AddObserver(logger);

Lights livingRoomLight = new Lights("Living Room Light", 100);
Thermostat mainThermostat = new Thermostat("Main Thermostat", 22.5f);
DoorLock frontDoor = new DoorLock("Front Door");
SecurityCamera drivewayCamera = new SecurityCamera("Driveway Camera");

homeSystem.AddDevice(livingRoomLight);
homeSystem.AddDevice(mainThermostat);
homeSystem.AddDevice(frontDoor);
homeSystem.AddDevice(drivewayCamera);

livingRoomLight.Toggle();
mainThermostat.SetTemperature(20.0f);
frontDoor.Toggle();
drivewayCamera.StartRecording();

public interface IObserver
{
    void Update(string notification);
}

public class Notifications : IObserver {
    public void Update(string notification) {
        Console.WriteLine($"[Notification]: {notification}");
    }
}

public class HomeSystem {
    private static HomeSystem Instance;
    private List<IObserver> Observers = new List<IObserver>();
    private List<HomeObject> Devices = new List<HomeObject>();

    private HomeSystem() { }

    public static HomeSystem GetInstance
    {
        get
        {
            if (Instance == null)
            {
                Instance = new HomeSystem();
            }
            return Instance;
        }
    }

    public void AddObserver(IObserver observer)
    {
        Observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        Observers.Remove(observer);
    }

    public void NotifyObservers(string notification)
    {
        foreach (var observer in Observers)
        {
            observer.Update(notification);
        }
    }

    public void AddDevice(HomeObject device)
    {
        Devices.Add(device);
        NotifyObservers($"Device {device.GetName()} has been added.");
    }

    public void RemoveDevice(HomeObject device)
    {
        Devices.Remove(device);
        NotifyObservers($"Device {device.GetName()} has been removed.");
    }

    public List<HomeObject> GetDevices()
    {
        return Devices;
    }
}

public abstract class HomeObject
{
    private string Name { get; }

    protected HomeObject(string name)
    {
        Name = name;
    }

    public string GetName()
    {
        return Name;
    }

    public abstract string Notify();
}

public interface IToggle
{
    void Toggle();
}

public class Lights : HomeObject, IToggle
{
    private int Brightness { get; set; }
    private bool IsTurnedOn { get; set; }

    public Lights(string name, int brightness) : base(name)
    {
        Brightness = brightness;
        IsTurnedOn = false;
    }

    public int GetBrightness()
    {
        return Brightness;
    }

    public void SetBrightness(int brightness)
    {
        Brightness = brightness;
        Console.WriteLine($"{GetName()} brightness set to {Brightness}.");
    }

    public void Toggle()
    {
        IsTurnedOn = !IsTurnedOn;
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} is now {(IsTurnedOn ? "On" : "Off")}.");
    }

    public override string Notify()
    {
        return $"{GetName()} is {(IsTurnedOn ? "On" : "Off")}.";
    }
}

public class Thermostat : HomeObject, IToggle
{
    private float Temperature { get; set; }
    private bool IsTurnedOn { get; set; }

    public Thermostat(string name, float temperature) : base(name)
    {
        Temperature = temperature;
        IsTurnedOn = false;
    }

    public float GetTemperature()
    {
        return Temperature;
    }

    public void SetTemperature(float temperature)
    {
        Temperature = temperature;
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} temperature set to {Temperature}°C.");
    }

    public void Toggle()
    {
        IsTurnedOn = !IsTurnedOn;
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} is now {(IsTurnedOn ? "On" : "Off")}.");
    }

    public override string Notify()
    {
        return $"{GetName()} is {(IsTurnedOn ? "On" : "Off")} at {Temperature}°C.";
    }
}

public class DoorLock : HomeObject, IToggle
{
    private bool IsLocked { get; set; }

    public DoorLock(string name) : base(name)
    {
        IsLocked = true;
    }

    public bool GetIsLocked()
    {
        return IsLocked;
    }

    public void Toggle()
    {
        IsLocked = !IsLocked;
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} is now {(IsLocked ? "Locked" : "Unlocked")}.");
    }

    public override string Notify()
    {
        return $"{GetName()} is {(IsLocked ? "Locked" : "Unlocked")}.";
    }
}

public class SecurityCamera : HomeObject
{
    private bool IsRecording { get; set; }
    private bool IsLive { get; set; }

    public SecurityCamera(string name) : base(name)
    {
        IsRecording = false;
        IsLive = false;
    }

    public bool GetIsRecording()
    {
        return IsRecording;
    }

    public void StartRecording()
    {
        IsRecording = true;
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} is now recording.");
    }

    public void SetLive(bool live)
    {
        IsLive = live;
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} is now {(IsLive ? "Live" : "Not Live")}.");
    }

    public override string Notify()
    {
        return $"{GetName()} is {(IsRecording ? "Recording" : "Not Recording")}.";
    }
}
