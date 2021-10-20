using System;
using System.IO;
using System.Reflection;
using PluginInterface;
using System.Collections;
using System.Threading;
using System.Xml.Serialization;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace OTACS
{


    /// <summary>
    /// Summary description for PluginServices.
    /// </summary>
    public class PluginServices : IPluginHost   //<--- Notice how it inherits IPluginHost interface!
    {
        /// <summary>
        /// Constructor of the Class
        /// </summary>
        public PluginServices()
        {
        }

        private ArrayList pluginList = new ArrayList();
        private ArrayList configList = new ArrayList();
        private AvailablePlugins colAvailablePlugins = new AvailablePlugins();
        private AvailablePlugin thermalPlugin = null;
        //private ArrayList pluginConnections = new ArrayList();
        private long cycleID = 0;
        private bool isPlaying = false;
        private bool[] processed;
        private Configuration curConfiguration = new Configuration();
        private int uniquePluginID = 1;

        public event LongDelegate CycleIDChanged;
        public event VoidDelegate ThermalPluginErrorOccured;
        private Assembly pluginAssembly;

        /// <summary>
        /// Unique Plugin ID
        /// </summary>
        public int UniquePluginID
        {
            get { return uniquePluginID; }
            set { uniquePluginID = value; }
        }

        /// <summary>
        /// Thermal Source Plugin
        /// </summary>
        public AvailablePlugin ThermalPlugin
        {
            get { return thermalPlugin; }
            set { thermalPlugin = value; }
        }

        /// <summary>
        /// Plugins List
        /// </summary>
        public ArrayList PluginList
        {
            get { return pluginList; }
            set { pluginList = value; }
        }

        /// <summary>
        /// Configurations List
        /// </summary>
        public ArrayList ConfigList
        {
            get { return configList; }
            set { configList = value; }
        }



        /// <summary>
        /// A Collection of all Plugins Found and Loaded by the FindPlugins() Method
        /// </summary>
        public AvailablePlugins AvailablePlugins
        {
            get { return colAvailablePlugins; }
            set { colAvailablePlugins = value; }
        }

        public Configuration CurConfiguration
        {
            get { return curConfiguration; }
            set { curConfiguration = value; }
        }

        /// <summary>
        /// Current CycleID
        /// </summary>
        public long CycleID
        {
            get { return cycleID; }
            set { cycleID = value; }
        }

        /// <summary>
        /// Current cycle state
        /// </summary>
        public bool IsPlaying
        {
            get { return isPlaying; }
            set { isPlaying = value; }
        }



        /// <summary>
        /// Searches the passed Path for Plugins
        /// </summary>
        /// <param name="Path">Directory to search for Plugins in</param>
        public void PopulatePlugins(string Path)
        {
            //First empty the collection, we're reloading them all
            PluginList.Clear();
            
            //Go through all the files in the plugin directory
            foreach (string fileOn in Directory.GetFiles(Path))
            {
                FileInfo file = new FileInfo(fileOn);

                //Preliminary check, must be .dll
                if (file.Extension.Equals(".dll"))
                {
                    //Add the 'plugin'
                    if (file.Name.Contains("avcodec-53") || file.Name.Contains("swscale-2") || file.Name.Contains("swresample-0") ||
    file.Name.Contains("postproc-52") || file.Name.Contains("avformat-53") || file.Name.Contains("avdevice-53") ||
    file.Name.Contains("avutil-51") || file.Name.Contains("avfilter-2"))
                    {
                        continue;
                    }
                    string pluginName = GetPluginName(fileOn);
                    if (pluginName != null)
                    {
                        PluginPathName foundPlugin = new PluginPathName();
                        foundPlugin.Path = fileOn;
                        foundPlugin.Name = pluginName;
                        PluginList.Add(foundPlugin);
                    }
                }
            }
        }


        /// <summary>
        /// Searches the passed Path for Configurations
        /// </summary>
        /// <param name="Path">Directory to search for Plugins in</param>
        public void PopulateConfigs(string Path)
        {
            //First empty the collection, we're reloading them all
            ConfigList.Clear();

            //Go through all the files in the plugin directory
            foreach (string fileOn in Directory.GetFiles(Path))
            {
                FileInfo file = new FileInfo(fileOn);

                //Preliminary check, must be .dll
                if (file.Extension.Equals(".XML"))
                {
                    ConfigList.Add(fileOn);
                }
            }
        }
        /// <summary>
        /// Creates Plugin assemblies
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private string GetPluginName(string FileName)
        {
            try
            {
                //Create a new assembly from the plugin file we're adding..
                Assembly pluginAssembly = Assembly.LoadFrom(FileName);
                //Next we'll loop through all the Types found in the assembly
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (pluginType.IsPublic) //Only look at public types
                    {
                        if (!pluginType.IsAbstract)  //Only look at non-abstract types
                        {
                            //Gets a type object of the interface we need the plugins to match
                            Type typeInterface = pluginType.GetInterface("PluginInterface.IPlugin", true);
                            //Make sure the interface we want to use actually exists

                            if (typeInterface != null)
                            {
                                string retStr = ((IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()))).Name;
                                typeInterface = null;
                                pluginAssembly = null;
                                return retStr;
                            }

                            typeInterface = null; //Mr. Clean
                        }
                    }
                }
                pluginAssembly = null; //more cleanup
            }
            catch (ReflectionTypeLoadException ex)
            {
                string errorMessage = ex.ToString();
                MessageBox.Show("Something went wrong while reading the plugins. " + errorMessage);
            }
            catch (Exception ex2)
            {
                MessageBox.Show("Something went wrong while reading the plugins. " + ex2.Message);
                //if (ex2.Message.Contains("avcodec-53") || ex2.Message.Contains("swscale-2") || ex2.Message.Contains("swresample-0") ||
                //    ex2.Message.Contains("postproc-52") || ex2.Message.Contains("avformat-53") || ex2.Message.Contains("avdevice-53") ||
                //    ex2.Message.Contains("avutil-51") || ex2.Message.Contains("Tau"))
                //{
                //    // do nothing
                //}
                //else
                //{
                //    MessageBox.Show("Something went wrong while reading the plugins. " + ex2.Message);
                //}
            }
                
            return null;
        }

        public string GetAssemblyPath(string pluginName)
        {
            foreach (PluginPathName ppn in PluginList)
            {
                if (ppn.Name == pluginName)
                {
                    return ppn.Path;
                }
            }
            return null;
        }


        /// <summary>
        /// Unloads and Closes all AvailablePlugins
        /// </summary>
        public void ClosePlugins()
        {
            foreach (AvailablePlugin pluginOn in colAvailablePlugins)
            {
                //Close all plugin instances
                //We call the plugins Dispose sub first incase it has to do 
                //Its own cleanup stuff
                pluginOn.Instance.Dispose();

                //After we give the plugin a chance to tidy up, get rid of it
                pluginOn.Instance = null;
            }

            //Finally, clear our collection of available plugins
            colAvailablePlugins.Clear();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return pluginAssembly;
        }

        public void AddPlugin(string FileName, int uniqueID, string serialFilename)
        {
            //Create a new assembly from the plugin file we're adding..
            pluginAssembly = Assembly.LoadFrom(FileName);

            //Next we'll loop through all the Types found in the assembly
            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (pluginType.IsPublic) //Only look at public types
                {
                    if (!pluginType.IsAbstract)  //Only look at non-abstract types
                    {
                        //Gets a type object of the interface we need the plugins to match
                        Type typeInterface = pluginType.GetInterface("PluginInterface.IPlugin", true);

                        //Make sure the interface we want to use actually exists
                        if (typeInterface != null)
                        {
                            //Create a new available plugin since the type implements the IPlugin interface
                            AvailablePlugin newPlugin = new AvailablePlugin();

                            //Set the filename where we found it
                            newPlugin.AssemblyPath = FileName;

                            if (File.Exists(serialFilename))
                            {
                                FileStream flStream = new FileStream(serialFilename, FileMode.Open, FileAccess.Read);
                                try
                                {
                                    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                                    BinaryFormatter binFormatter = new BinaryFormatter();

                                    newPlugin.Instance = (IPlugin)binFormatter.Deserialize(flStream);
                                }
                                finally
                                {
                                    AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                                    flStream.Close();
                                }

                            }
                            else
                            {
                                //Create a new instance and store the instance in the collection for later use
                                //We could change this later on to not load an instance.. we have 2 options
                                //1- Make one instance, and use it whenever we need it.. it's always there
                                //2- Don't make an instance, and instead make an instance whenever we use it, then close it
                                //For now we'll just make an instance of all the plugins
                                newPlugin.Instance = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
                            }

                            //Set the Plugin's host to this class which inherited IPluginHost
                            newPlugin.Instance.Host = this;

                            //Call the initialization sub of the plugin
                            newPlugin.Instance.Initialize();

                            newPlugin.Instance.MyID = uniqueID;

                            //Add the new plugin to our collection here
                            this.colAvailablePlugins.Add(newPlugin);

                            //cleanup a bit
                            newPlugin = null;
                        }

                        typeInterface = null; //Mr. Clean			
                    }
                }
            }

            pluginAssembly = null; //more cleanup
        }



        /// <summary>
        /// Handles the data output from the output pin of a plugin
        /// </summary>
        /// <param name="pin">pin on which to send the output</param>
        /// <param name="Plugin">plugin that called the SendData</param>
        public void SendData(IPin pin, IPinData data, IPlugin Plugin)
        {
            if (pin.Connected)
            {
                for (int i = 0; i < CurConfiguration.PluginConnections.Count; i++)
                {
                    Connection con = (Connection)CurConfiguration.PluginConnections[i];
                    if ((con.outPlugin.Instance.Name == Plugin.Name) && (con.outPlugin.Instance.MyID == Plugin.MyID) && (con.outPin.Name == pin.Name))
                    {
                        /*
                        SendDataStruct send = new SendDataStruct();
                        send.SendData = data;
                        send.SendIndex = i;
                        SendDataList.Add(send);
                         */

                        /*
                        curSendData = data;
                        Thread t = new Thread(InitiateProcess);
                        t.Start(i);             
                        */

                        ArrayList param = new ArrayList();
                        param.Add(data);
                        param.Add(i);
                        Thread t = new Thread(InitiateProcess);
                        t.Start(param);

                        if (IsPlaying)
                        {
                            //con.inPlugin.Instance.Process(con.inPin, data);
                            if (IsCurrentCycleDone())
                            {
                                ContinueCycle();
                            }
                            //break;
                        }
                    }
                }

                /*
                for (int i = 0; i < SendDataList.Count; i++)
                {
                    Thread t = new Thread(InitiateProcess);
                    t.Start(i);
                }
                 */
            }
        }

        public void InitiateProcess(object curParam)
        {
            IPinData senddata = (IPinData)((ArrayList)curParam)[0];
            int Index = (int)((ArrayList)curParam)[1];
            Connection con = (Connection)CurConfiguration.PluginConnections[Index];
            //Hadi  Enclosing the line that throws exception with try and catch, just to let it continue (temporary solution)
            try
            {
                con.inPlugin.Instance.Process(con.inPin, senddata);
            }
            catch(NullReferenceException e)
                {
                    Console.WriteLine(e.Message + e.StackTrace);
                } 
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (OverflowException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void ThermalProcess()
        {
            //try
            //{
            //    if (ThermalPlugin.Instance != null)
            //        ThermalPlugin.Instance.Process(null, null);
            //}
            //catch (Exception)
            //{
            //    ThermalPluginErrorOccured();
            //}

            if (ThermalPlugin != null)
                ThermalPlugin.Instance.Process(null, null);
        }

        /// <summary>
        /// Signals that the critical processing is finished at a critical pin of a plugin
        /// </summary>
        /// <param name="pin">the critical pin on which processing has finished</param>
        /// <param name="Plugin">plugin that called the function</param>
        public void SignalCriticalProcessingIsFinished(IPin pin, IPlugin Plugin)
        {
            if (IsPlaying)
            {
                for (int i = 0; i < CurConfiguration.PluginConnections.Count; i++)
                {
                    Connection con = (Connection)CurConfiguration.PluginConnections[i];
                    if ((con.inPlugin.Instance.Name == Plugin.Name) && (con.inPlugin.Instance.MyID == Plugin.MyID) && (con.inPin.Name == pin.Name) && (con.inPin.Category == PinCategory.Critical))
                    {
                        processed[i] = true;
                        if (IsCurrentCycleDone())
                        {
                            ContinueCycle();
                        }
                        break;
                    }
                }
            }
        }

        public IPin LoadOrCreatePin(string name, PinCategory category, Type[] requiredInterfaces)
        {
            BasePin pin = new BasePin(name, category, requiredInterfaces);
            return pin;
        }

        public void PlayPauseCycle(bool play)
        {
            if (play)
            {
                IsPlaying = true;
                processed = new bool[CurConfiguration.PluginConnections.Count];
                //if (IsCurrentCycleDone())
                //{
                ContinueCycle();
                //}
            }
            else
            {
                IsPlaying = false;
            }

        }

        private void ContinueCycle()
        {
            if (IsPlaying)
            {
                for (int i = 0; i < CurConfiguration.PluginConnections.Count; i++)
                {
                    Connection con = (Connection)CurConfiguration.PluginConnections[i];
                    if (con.inPin.Category == PinCategory.Critical)
                    {
                        processed[i] = false;
                    }
                }
                CycleID++;
                CycleIDChanged(CycleID);

                Thread t = new Thread(ThermalProcess);
                t.Start();
            }
        }

        private bool IsCurrentCycleDone()
        {
            for (int i = 0; i < CurConfiguration.PluginConnections.Count; i++)
            {
                Connection con = (Connection)CurConfiguration.PluginConnections[i];
                if ((con.inPin.Category == PinCategory.Critical) && (processed[i] == false))
                {
                    return false;
                }
            }
            return true;
        }

        private bool Serialize(Configuration config, string fileName)
        {
            String serialFilePath = Path.GetDirectoryName(fileName);
            String configName = Path.GetFileNameWithoutExtension(fileName);
            String newFolder = serialFilePath + "\\" + configName;
            if (Directory.Exists(newFolder))
            {
                Directory.Delete(newFolder, true);
            }
            Directory.CreateDirectory(newFolder);

            for (int i = 0; i < config.PluginParams.Count; i++)
            {
                PluginParameters ppm = (PluginParameters)config.PluginParams[i];
                AvailablePlugin plugin = AvailablePlugins.Find(ppm.pluginName, ppm.pluginID);

                String serialFilename = newFolder + "\\" + ppm.pluginName + "_" + ppm.pluginID + ".serial";
                FileStream flStream = new FileStream(serialFilename,
                    FileMode.Create, FileAccess.Write);

                try
                {
                    BinaryFormatter binFormatter = new BinaryFormatter();
                    binFormatter.Serialize(flStream, plugin.Instance);
                }
                catch
                {
                    flStream.Close();
                    return false;
                }
                flStream.Close();
            }


            return true;

        }

        public bool SaveConfiguration(string fileName)
        {
            //Save customer object to XML file using our ObjectXMLSerializer class...
            try
            {
                Configuration saveConfig = FillInCurrentConfigurationObject();
                ObjectXMLSerializer<Configuration>.Save(saveConfig, fileName);
                if (!Serialize(saveConfig, fileName))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;

            }
        }



        bool IsInPluginParameters(Configuration config, string pluginName, int pluginID)
        {
            for (int i = 0; i < config.PluginParams.Count; i++)
            {
                PluginParameters ppm = (PluginParameters)config.PluginParams[i];
                if (ppm.pluginName == pluginName && ppm.pluginID == pluginID)
                {
                    return true;
                }
            }
            return false;
        }

        Configuration FillInCurrentConfigurationObject()
        {
            Configuration saveConfig = new Configuration();
            for (int i = 0; i < CurConfiguration.PluginConnections.Count; i++)
            {
                ConnectionParameters cpm = new ConnectionParameters();
                Connection conn = (Connection)CurConfiguration.PluginConnections[i];
                cpm.inPluginName = conn.inPlugin.Instance.Name;
                cpm.inPluginID = conn.inPlugin.Instance.MyID;
                cpm.inPinIndex = conn.inputPinIndex;
                cpm.outPluginName = conn.outPlugin.Instance.Name;
                cpm.outPluginID = conn.outPlugin.Instance.MyID;
                cpm.outPinIndex = conn.outputPinIndex;

                cpm.ConnPicBoxLocations = new Point[conn.Junctions.Count];
                for (int j = 0; j < conn.Junctions.Count; j++)
                {
                    cpm.ConnPicBoxLocations[j] = ((System.Windows.Forms.PictureBox)conn.Junctions[j]).Location;
                }

                saveConfig.ConnParams.Add(cpm);

                if (!IsInPluginParameters(saveConfig, conn.inPlugin.Instance.Name, conn.inPlugin.Instance.MyID))
                {
                    PluginParameters ppm = new PluginParameters();
                    ppm.pluginName = conn.inPlugin.Instance.Name;
                    ppm.pluginID = conn.inPlugin.Instance.MyID;
                    ppm.GraphLocation = conn.inPlugin.MyGraph.Location;
                    ppm.GraphSize = conn.inPlugin.MyGraph.Size;
                    ppm.isUIVisible = conn.inPlugin.MyGraph.IsUIVisible;
                    ppm.UILocation = conn.inPlugin.MyForm.Location;

                    ppm.UISize = conn.inPlugin.MyForm.Size;
                    /*
                    if (conn.inPlugin.MyForm.IsDocked)
                    {
                        ppm.UISize = conn.inPlugin.MyForm.Size;
                        ppm.UISize.Height += 22;
                    }
                    else
                    {
                        ppm.UISize = conn.inPlugin.MyForm.Size;
                    }
                    */

                    saveConfig.PluginParams.Add(ppm);
                }

                if (!IsInPluginParameters(saveConfig, conn.outPlugin.Instance.Name, conn.outPlugin.Instance.MyID))
                {
                    PluginParameters ppm = new PluginParameters();
                    ppm.pluginName = conn.outPlugin.Instance.Name;
                    ppm.pluginID = conn.outPlugin.Instance.MyID;
                    ppm.GraphLocation = conn.outPlugin.MyGraph.Location;
                    ppm.GraphSize = conn.outPlugin.MyGraph.Size;
                    ppm.isUIVisible = conn.outPlugin.MyGraph.IsUIVisible;
                    ppm.UILocation = conn.outPlugin.MyForm.Location;

                    ppm.UISize = conn.outPlugin.MyForm.Size;
                    /*
                    if (conn.outPlugin.MyForm.IsDocked)
                    {
                        ppm.UISize = conn.outPlugin.MyForm.Size;
                        ppm.UISize.Height += 22;
                    }
                    else
                    {
                        ppm.UISize = conn.outPlugin.MyForm.Size;
                    }
                    */
                    saveConfig.PluginParams.Add(ppm);
                }
            }
            saveConfig.UniqueID = UniquePluginID;
            return saveConfig;
        }

        public bool LoadConfiguration(string fileName)
        {
            // Load the customer object from the existing XML file (if any)...
            if (File.Exists(fileName) == true)
            {

                // Load the customer object from the XML file using our custom class...
                CurConfiguration = ObjectXMLSerializer<Configuration>.Load(fileName);

                if (CurConfiguration == null)
                {
                    return false;
                }
                else  // Load customer properties into the form...
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }


    }

    public struct PluginPathName
    {
        public string Path;
        public string Name;
    }



    public struct Connection
    {
        public int outputPinIndex;
        public int inputPinIndex;
        public AvailablePlugin outPlugin;
        public BasePin outPin;
        public AvailablePlugin inPlugin;
        public BasePin inPin;
        public ArrayList Junctions;
    }

    public struct ConnectionParameters
    {
        public string outPluginName;
        public int outPluginID;
        public int outPinIndex;
        public string inPluginName;
        public int inPluginID;
        public int inPinIndex;
        public Point[] ConnPicBoxLocations;
    }

    public struct PluginParameters
    {
        public string pluginName;
        public int pluginID;
        public Size GraphSize;
        public Point GraphLocation;
        public Size UISize;
        public Point UILocation;
        public bool isUIVisible;
    }

    [XmlRootAttribute("Configuration", Namespace = "", IsNullable = false)]
    public class Configuration
    {

        public Configuration()
        {
        }

        // Serializes an ArrayList as a "Hobbies" array of XML elements of type string named "Hobby".
        //[XmlArray("PluginConnections"), XmlArrayItem("PluginConnection", typeof(Connection))]
        [XmlIgnoreAttribute()]
        public ArrayList PluginConnections = new System.Collections.ArrayList();

        [XmlArray("ConnParams"), XmlArrayItem("CParam", typeof(ConnectionParameters))]
        public ArrayList ConnParams = new ArrayList();

        [XmlArray("PluginParams"), XmlArrayItem("PParam", typeof(PluginParameters))]
        public ArrayList PluginParams = new ArrayList();

        public int UniqueID;

    }

    public class BasePin : IPin
    {
        // Fields
        private PinCategory category;
        private bool connected;
        private string name;
        private Type[] optionalInterfaces;
        private Type[] requiredInterfaces;


        public BasePin(string setName, PinCategory setCategory, Type[] setRequiredTypes)
        {
            this.category = setCategory;
            this.requiredInterfaces = setRequiredTypes;
            this.name = setName;
            this.connected = false;
        }

        public BasePin(string setName, PinCategory setCategory, Type[] setRequiredTypes, Type[] setOptionalTypes)
        {
            this.category = setCategory;
            this.requiredInterfaces = setRequiredTypes;
            this.optionalInterfaces = setOptionalTypes;
            this.name = setName;
            this.connected = false;
        }

        public Type[] GetOptionalInterfaces()
        {
            return this.optionalInterfaces;
        }

        public Type[] GetRequiredInterfaces()
        {
            return this.requiredInterfaces;
        }

        public void ReplaceOptionalInterfaces(Type[] newInterfaces)
        {
            this.optionalInterfaces = newInterfaces;
        }

        public void ReplaceRequiredInterfaces(Type[] newInterfaces)
        {
            this.requiredInterfaces = newInterfaces;
        }

        public void SetCategory(PinCategory setCategory)
        {
            this.category = setCategory;
        }

        // Properties
        public bool Connected
        {
            get
            {
                return this.connected;
            }
            set
            {
                this.connected = value;
            }
        }

        public PinCategory Category
        {
            get
            {
                return this.category;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }


    //namespace Types
    //{
    /// <summary>
    /// Collection for AvailablePlugin Type
    /// </summary>
    public class AvailablePlugins : System.Collections.CollectionBase
    {
        //A Simple Home-brew class to hold some info about our Available Plugins

        /// <summary>
        /// Add a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToAdd">The Plugin to Add</param>
        public void Add(AvailablePlugin pluginToAdd)
        {
            this.List.Add(pluginToAdd);
        }

        /// <summary>
        /// Remove a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToRemove">The Plugin to Remove</param>
        public void Remove(AvailablePlugin pluginToRemove)
        {
            this.List.Remove(pluginToRemove);
        }

        /// <summary>
        /// Return a Plugin at the index from the collection of Available plugins
        /// </summary>
        /// <param name="pluginToRemove">The Plugin to Remove</param>
        public AvailablePlugin PluginAt(int index)
        {
            return (AvailablePlugin)this.List[index];
        }

        /*
        /// <summary>
        /// Finds a plugin in the available Plugins
        /// </summary>
        /// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
        /// <returns>Available Plugin, or null if the plugin is not found</returns>
        public AvailablePlugin Find(string pluginNameOrPath)
        {
            AvailablePlugin toReturn = null;
			
            //Loop through all the plugins
            foreach (AvailablePlugin pluginOn in this.List)
            {
                //Find the one with the matching name or filename
                if ((pluginOn.Instance.Name.Equals(pluginNameOrPath)) || pluginOn.AssemblyPath.Equals(pluginNameOrPath))
                {
                    toReturn = pluginOn;
                    break;		
                }
            }
            return toReturn;
        }
        */

        /// <summary>
        /// Finds a plugin in the available Plugins
        /// </summary>
        /// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
        /// <returns>Available Plugin, or null if the plugin is not found</returns>
        public AvailablePlugin Find(string pluginName, int pluginID)
        {
            AvailablePlugin toReturn = null;

            //Loop through all the plugins
            foreach (AvailablePlugin pluginOn in this.List)
            {
                //Find the one with the matching name or filename
                if ((pluginOn.Instance.Name.Equals(pluginName)) && (pluginOn.Instance.MyID == pluginID))
                {
                    toReturn = pluginOn;
                    break;
                }
            }
            return toReturn;
        }
    }

    /// <summary>
    /// Data Class for Available Plugin.  Holds and instance of the loaded Plugin, as well as the Plugin's Assembly Path
    /// </summary>
    public class AvailablePlugin
    {
        //This is the actual AvailablePlugin object.. 
        //Holds an instance of the plugin to access
        //ALso holds assembly path... not really necessary
        private IPlugin myInstance = null;
        private string myAssemblyPath = "";
        private PluginUIForm myForm = new PluginUIForm();
        private PluginGraphForm myGraph = null;

        public IPlugin Instance
        {
            get { return myInstance; }
            set
            {
                myInstance = value;
                //MyGraph = new PluginGraphForm(this);
            }
        }
        public string AssemblyPath
        {
            get { return myAssemblyPath; }
            set { myAssemblyPath = value; }
        }

        public PluginUIForm MyForm
        {
            get { return myForm; }
            set { myForm = value; }
        }

        public PluginGraphForm MyGraph
        {
            get { return myGraph; }
            set { myGraph = value; }
        }
    }

}

