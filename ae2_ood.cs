using System; // General import for System namespace that allows us to use core functionalities such as Console and collections
using System.Collections.Generic; // important allowing us to use lists (List<T> )

class Program
{
    static void Main()
    {
        HomeSystem homeSystem = HomeSystem.GetInstance; //here we get the singleton instance of the Homesystem Class

        Notifications logger = new Notifications(); //created an instance of the Notifications class in order to make it an Observer
        homeSystem.AddObserver(logger); //added the instance of notifications the HomeSystem list of observers


        //These 4 lines below create an instance for each unique device
        Lights livingRoomLight = new Lights("Living Room Light", 100);
        Thermostat mainThermostat = new Thermostat("Main Thermostat", 22.5f);
        DoorLock frontDoor = new DoorLock("Front Door");
        SecurityCamera drivewayCamera = new SecurityCamera("Driveway Camera");

        
        //Here we simply add the devices to the HomeSystem device list (4 lines below)
        homeSystem.AddDevice(livingRoomLight);
        homeSystem.AddDevice(mainThermostat);
        homeSystem.AddDevice(frontDoor);
        homeSystem.AddDevice(drivewayCamera);

        //Below lines perform actions for each device (method names indicate the action --e.g (SetTemperature sets the temperature of the Thermostat))
        livingRoomLight.Toggle();
        mainThermostat.SetTemperature(20.0f);
        frontDoor.Toggle();
        drivewayCamera.StartRecording();
    }
}

//This interface must be implemented by all observers to receive notifications
//Serves as a "promise" that any class implementing this interface will have the Update method
public interface IObserver
{
    void Update(string notification); 
}

// Notifications class that implements the IObserver interface to notify/print out the notifications
// As stated above, it is implementing the Update method
public class Notifications : IObserver
{
    public void Update(string notification)
    {
        Console.WriteLine($"[Notification]: {notification}");
    }
}

//Here we create the Homesystem class that implements the singleton design - thus we have one instance of this class 
public class HomeSystem
{
    // It is 
    private static HomeSystem? Instance; // Private static instance, again, to ensure only one instance of HomeSystem exists (utilized in GetInstance )

    private List<IObserver> Observers = new List<IObserver>(); // List to hold all observer instances.
    private List<HomeObject> Devices = new List<HomeObject>();  // List to hold all devices added to the home system.

    private HomeSystem() { } // Added a private constructor to prevent instantiation outside this class.

    //public static property to get the instance of the homesystem
    public static HomeSystem GetInstance
    {
        get
        {
            if (Instance == null) // quick check if the instance doesnt exist (if not it will create a new one)
            {
                Instance = new HomeSystem();
            }
            return Instance;
        }
    }
    // method to simply add an observer to the Homesystem

    public void AddObserver(IObserver observer)
    {
        Observers.Add(observer);
    }
    // same as the above, but this time removes an observer from the Homesystem 
    public void RemoveObserver(IObserver observer)
    {
        Observers.Remove(observer);
    }
    // This notifies all the registered observes with a message 
    public void NotifyObservers(string notification)
    {
        foreach (var observer in Observers)
        {
            observer.Update(notification);
        }
    }
    // Adds a device to the homesystem and calls the NotifyObservers method that will notify the observers
    public void AddDevice(HomeObject device)
    {
        Devices.Add(device);
        NotifyObservers($"Device {device.GetName()} has been added.");
    }

    // This was implmented in order to remove a device from the HomeSystem and notify observers
    public void RemoveDevice(HomeObject device)
    {
        Devices.Remove(device);
        NotifyObservers($"Device {device.GetName()} has been removed.");
    }

    // This fetches all the devices in the Homesystem
    public List<HomeObject> GetDevices()
    {
        return Devices; //thus the output is a list
    }
}

// ILL TAKE IT FRFOM HERE AND BELLOW, 
// BATHROOM BREAK 

// HomeObject class will be used as a base (super) class for all the devices in the house
// The devices will, 
public abstract class HomeObject
{

    // Getter for Name, useed to fetch the name of the device when needed
    private string Name { get; }

    // Constructor to assign the name to the private property
    protected HomeObject(string name)
    {
        Name = name;
    }

    // Method to get the name of the device, public version of the private property, 
    // to be used to fetch the name of the device outside the class
    public string GetName()
    {
        return Name;
    }

    // Abstract method to be implemented by all the devices, like a promise that all the devices will have this method.
    public abstract string Notify();
}


public interface IToggle
{
    void Toggle();
}

// First of the devices, this as the following inherits from HomeObject and implements the IToggle interface
// It'll be used to control the lights in the house, changing the brightness and turning them on and off
// The class has a (encapsulated) private property to store the brightness of the lights and a boolean to store the state of the lights
public class Lights : HomeObject, IToggle
{

    // Private properties that contain the brightness and the state of the lights
    // Get and set methods, to be used to get and set the values of the properties
    private int Brightness { get; set; }
    private bool IsTurnedOn { get; set; }

    // Constructor initites deaful values, and takes in a name and brightness value, ex. ("Living Room Light", 80)
    public Lights(string name, int brightness) : base(name)
    {
        Brightness = brightness;
        IsTurnedOn = false;
    }

    // No GetBrightness method, as the brightness is a private property, and is acessed by other methods using the private get and set methods

    // Method to set the brightness of the lights, with a constraint of it being between 0 and 100
    public void SetBrightness(int brightness)
{
    if (brightness < 0 || brightness > 100)
    {
        // If not between 0 and 100, throw an exception
        throw new ArgumentOutOfRangeException(nameof(brightness), "Brightness must be between 0 and 100.");
    }
    Brightness = brightness;
    Console.WriteLine($"{GetName()} brightness set to {Brightness}%.");
}

    // Toggle the lights on and off
    // If the lights on -> off, if off -> on
    public void Toggle()
    {
        IsTurnedOn = !IsTurnedOn;
        // Notify the observers about the state of the lights
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} is now {(IsTurnedOn ? "On" : "Off")}.");
    }

    // Notify method, returning the Name, state and brightness of the lights
    public override string Notify()
    {
        return $"{GetName()} is {(IsTurnedOn ? "On" : "Off")} with brightness {Brightness}%.";
    }
}

// Thermostat used to set set temperature and turn on and off
// Thermostat Class Inheriting from HomeObject and implementing IToggle Interface
public class Thermostat : HomeObject, IToggle
{
    private float Temperature { get; set; }
    private bool IsTurnedOn { get; set; }

    public Thermostat(string name, float temperature) : base(name)
    {
        Temperature = temperature;
        IsTurnedOn = false;
    }

    public void SetTemperature(float temperature)
    {
    if (temperature < 10.0f || temperature > 30.0f)
    {
        throw new ArgumentOutOfRangeException(nameof(temperature), "Temperature must be between 10.0°C and 30.0°C.");
    }
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

// DoorLock that can be locked or unlocked, based on it it can be toggled and give notifications to the observers
// DoorLock Class Inheriting from HomeObject and implementing IToggle Interface
// Private properties store the states of the door lock, and public methods to fetch and set the states
public class DoorLock : HomeObject, IToggle
{
    
    // Private Property to store the state of the door lock
    private bool IsLocked { get; set; }

    // Constructor to initialize the door lock with a name
    // Inputs name of the door lock (e.g., "Front Door") 
    // Default state of the door lock (Locked)
    public DoorLock(string name) : base(name)
    {
        IsLocked = true;
    }

    // Method to fetch the lock state of the door
    // Outputs the lock state of the door (true if locked else, false).
    public bool GetIsLocked()
    {
        return IsLocked;
    }

    // Method to toggle the lock state of the door, on -> off and off -> on
    public void Toggle()
    {
        IsLocked = !IsLocked;
        // Notify the observers about the lock state of the door
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} is now {(IsLocked ? "Locked" : "Unlocked")}.");
    }

    // Notify method, that retuerns the Name and the lock state of the door
    public override string Notify()
    {
        return $"{GetName()} is {(IsLocked ? "Locked" : "Unlocked")}.";
    }
}

// Security Camera that can be recording or not, on top of that can be live or not, independent of each other
// Security Camera Class Inheriting from HomeObject
// Encapsulating with private properties to store the states of the camera, and public methods to fetch and set the states
public class SecurityCamera : HomeObject
{

    // Private Properties to store the states of the camera
    private bool IsRecording { get; set; }
    private bool IsLive { get; set; }

    // Constructor to initialize the camera with a name
    // Inputs string name - The name of the camera (e.g., "Garden Camera").
    // States (IsRecording and IsLive) are private and initialized here.
    public SecurityCamera(string name) : base(name)
    {
        IsRecording = false;
        IsLive = false;
    }

    // This method is used to fetch the recording state of the camera.
    // Outputs the recording state of the camera (true if recording else, false).
    public bool GetIsRecording()
    {
        return IsRecording;
    }

    // Method to start recording based on the !IsRecording state
    // If the camera is not recording, it will start recording and notify the observers.
    public void StartRecording()
    {
    if (!IsRecording)
    {
        // If the camera is not recording, start recording and print a message
        // Interacts with the HomeSystem to notify the observers that the camera is now recording
        IsRecording = true;
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} is now recording.");
    }
    else
    {
        // If the camera is already recording, print / log a message
        Console.WriteLine($"{GetName()} is already recording.");
    }
    }

    
    // Method to set the live state of the camera
    // Inputs the live state of the camera (true if live else, false).
    public void SetLive(bool live)
    {
        IsLive = live;
        // Notify the observers about the live state of the camera
        // Interacts with the HomeSystem to notify the observers of the live state
        HomeSystem.GetInstance.NotifyObservers($"{GetName()} is now {(IsLive ? "Live" : "Not Live")}.");
    }

    // Notify method, returning Name and the recording state of the camera
    // Outputs the notification message for the camera.
    public override string Notify()
    {
        return $"{GetName()} is {(IsRecording ? "Recording" : "Not Recording")}.";
    }
}
