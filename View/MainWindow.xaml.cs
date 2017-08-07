﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using FullSerializer;
using GalaSoft.MvvmLight.Ioc;
using LZ4;
using Microsoft.Win32;
using MMSaveEditor.ViewModel;

namespace MMSaveEditor.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private fsSerializer serializer;

        private string _openFilePath;
        private static readonly int saveFileVersion = 4;
        private SaveFileInfo _currentSaveInfo;

        public static MainWindow Instance;

        public string VersionString
        {
            get
            {
                return string.Format("Motorsport Manager Save Editor v{0}", Assembly.GetExecutingAssembly()
                    .GetName()
                    .Version);
            }
        }

        public enum TabPage
        {
            TeamPrincipal,
            Driver,
            Team, Game
        }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            serializer = CreateAndConfigureSerializer();
        }

        private static fsSerializer CreateAndConfigureSerializer()
        {
            return new fsSerializer { Config = { DefaultMemberSerialization = fsMemberSerialization.OptOut, SerializeAttributes = new Type[1] { typeof(fsPropertyAttribute) }, IgnoreSerializeAttributes = new Type[2] { typeof(NonSerializedAttribute), typeof(fsIgnoreAttribute) }, SerializeEnumsAsInteger = true, EnablePropertySerialization = false } };
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Save games (*.sav)|*.*";
            openFileDialog.InitialDirectory = Path.Combine(App.Instance.LocalLowFolderPath, "Playsport Games\\Motorsport Manager\\Cloud\\Saves");

            if (openFileDialog.ShowDialog() == true)
            {
                LoadFile(openFileDialog.FileName, serializer, out _currentSaveInfo);
                _openFilePath = openFileDialog.FileName;
                SetupViewModels();
            }
        }

        private static void SetupViewModels()
        {
            var playerVM = SimpleIoc.Default.GetInstance<PlayerViewModel>();
            playerVM.SetModel(Game.Instance.player);
            var teamVM = SimpleIoc.Default.GetInstance<TeamViewModel>();
            teamVM.SetModel(null);
            var gameVM = SimpleIoc.Default.GetInstance<GameViewModel>();
            gameVM.SetModels(Game.Instance.time);
            var principleVM = SimpleIoc.Default.GetInstance<TeamPrincipalViewModel>();
            principleVM.SetList(Game.Instance.teamPrincipalManager.GetEntityList());
            var driverVM = SimpleIoc.Default.GetInstance<DriverViewModel>();
            driverVM.SetList(Game.Instance.driverManager.GetEntityList());
            var engineerVM = SimpleIoc.Default.GetInstance<EngineerViewModel>();
            engineerVM.SetList(Game.Instance.engineerManager.GetEntityList());
            var mechanicVM = SimpleIoc.Default.GetInstance<MechanicViewModel>();
            mechanicVM.SetList(Game.Instance.mechanicManager.GetEntityList());
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_openFilePath))
            {
                SaveFile(_openFilePath, serializer, _currentSaveInfo);
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Save games (*.sav)|*.*";
            saveFileDialog.InitialDirectory = Path.Combine(App.Instance.LocalLowFolderPath, "Playsport Games\\Motorsport Manager\\Cloud\\Saves");
            saveFileDialog.DefaultExt = "sav";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveFile(saveFileDialog.FileName, serializer, _currentSaveInfo);
                _openFilePath = saveFileDialog.FileName;
            }
        }

        public static void SaveFile(string openFilePath, fsSerializer serializer, SaveFileInfo saveFileInfo)
        {
            try
            {
                fsData data1;
                fsResult fsResult1 = serializer.TrySerialize(saveFileInfo, out data1);
                if (fsResult1.Failed)
                    throw new Exception(string.Format("Failed to serialise SaveFileInfo: {0}", fsResult1.FormattedMessages));
                string s1 = fsJsonPrinter.CompressedJson(data1);
                fsData data2;
                fsResult fsResult2 = serializer.TrySerialize(Game.Instance, out data2);
                if (fsResult2.Failed)
                    throw new Exception(string.Format("Failed to serialise Game: {0}",
                        fsResult2.FormattedMessages));
                string s2 = fsJsonPrinter.CompressedJson(data2);
                byte[] bytes1 = Encoding.UTF8.GetBytes(s1);
                byte[] bytes2 = Encoding.UTF8.GetBytes(s2);
                Debug.Assert(bytes1.Length < 268435456 && bytes2.Length < 268435456, "Uh-oh. Ben has underestimated how large save files might get, and we're about to save a file so large it will be detected as corrupt when loading. Best increase the limit!", null);
                byte[] buffer1 = LZ4Codec.Encode(bytes1, 0, bytes1.Length);
                byte[] buffer2 = LZ4Codec.Encode(bytes2, 0, bytes2.Length);
                FileInfo fileInfo = new FileInfo(openFilePath);
                using (FileStream fileStream = File.Create(fileInfo.FullName))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        binaryWriter.Write(1932684653);
                        binaryWriter.Write(saveFileVersion);
                        binaryWriter.Write(buffer1.Length);
                        binaryWriter.Write(bytes1.Length);
                        binaryWriter.Write(buffer2.Length);
                        binaryWriter.Write(bytes2.Length);
                        binaryWriter.Write(buffer1);
                        binaryWriter.Write(buffer2);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void LoadFile(string fileName, fsSerializer serializer, out SaveFileInfo saveFileInfo)
        {
            using (FileStream fileStream = File.Open(fileName, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    if (binaryReader.ReadInt32() != 1932684653)
                    {
                        MessageBoxResult result = MessageBox.Show("Save file is not a valid save file for this game", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        saveFileInfo = null;
                        return;
                    }
                    int num1 = binaryReader.ReadInt32();
                    if (num1 < saveFileVersion)
                    {
                        MessageBoxResult result = MessageBox.Show("Save file is an old format, and no upgrade path exists - must be from an old unsupported development version", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        saveFileInfo = null;
                        return;
                    }
                    if (num1 > saveFileVersion)
                    {
                        MessageBoxResult result = MessageBox.Show("Save file version is newer than the editor expected. If the game has been updated recently you may need to wait for an update to the editor. Check the forums for updates.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        saveFileInfo = null;
                        return;
                    }
                    int headerCount = binaryReader.ReadInt32();
                    int headerOutputLength = binaryReader.ReadInt32();
                    int gameDataCount = binaryReader.ReadInt32();
                    int gameDataOutputLength = binaryReader.ReadInt32();
                    if (headerOutputLength > 268435456)
                    {
                        MessageBoxResult result = MessageBox.Show("Save file header size is apparently way too big - file has either been tampered with or become corrupt", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        saveFileInfo = null;
                        return;
                    }
                    if (gameDataOutputLength > 268435456)
                    {
                        MessageBoxResult result = MessageBox.Show("Save file game data size is apparently way too big - file has either been tampered with or become corrupt", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        saveFileInfo = null;
                        return;
                    }

                    //
                    // Load header SaveFileInfo
                    //
                    fsData headerData;
                    string jsonHead = Encoding.UTF8.GetString(LZ4Codec.Decode(binaryReader.ReadBytes(headerCount), 0, headerCount, headerOutputLength));
#if DEBUG
                    File.WriteAllText(@"saveFileJSONHead.txt", jsonHead);
#endif
                    fsResult fsHeaderResult1 = fsJsonParser.Parse(jsonHead, out headerData);
                    if (fsHeaderResult1.Failed)
                    {
                        MessageBoxResult result = MessageBox.Show(string.Format("Error reported whilst parsing serialized SaveFileInfo string: {0}", fsHeaderResult1.FormattedMessages), "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        saveFileInfo = null;
                        return;
                    }

                    saveFileInfo = null;
                    fsResult fsHeaderResult2 = serializer.TryDeserialize(headerData, ref saveFileInfo);
                    if (fsHeaderResult2.Failed)
                    {
                        MessageBoxResult result = MessageBox.Show(string.Format("Error reported whilst deserializing SaveFileInfo: {0}", fsHeaderResult1.FormattedMessages), "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        saveFileInfo = null;
                        return;
                    }
                    try
                    {
                        FileInfo fileInfo = new FileInfo(fileName);
                        saveFileInfo.fileInfo = fileInfo;
                    }
                    catch (Exception ex)
                    {
                        MessageBoxResult result = MessageBox.Show(string.Format("Could not create FileInfo for {0}. Check that the editor has permissions to access this file.", fileName), "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        saveFileInfo = null;
                        return;
                    }


                    //
                    // Load main save data
                    // 
                    Game targetGame = null;
                    fsData gameData;
                    try
                    {
                        string json = Encoding.UTF8.GetString(LZ4Codec.Decode(binaryReader.ReadBytes(gameDataCount), 0, gameDataCount, gameDataOutputLength));
#if DEBUG
                        File.WriteAllText(@"saveFileJSON.txt", json);
#endif
                        //SaveData saveData = JsonConvert.DeserializeObject<SaveData>( json );
                        //string formattedJSON = JsonConvert.SerializeObject( parsedJson, Formatting.Indented );


                        fsResult fsResult1 = fsJsonParser.Parse(json, out gameData);
                        if (fsResult1.Failed)
                        {
                            MessageBoxResult result = MessageBox.Show(string.Format("Error reported whilst parsing serialized Game data string: {0}", fsResult1.FormattedMessages), "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxResult result = MessageBox.Show(string.Format("Exception thrown whilst parsing serialized Game data string: {0}", fileName), "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    fsResult fsResult2 = new fsResult();
                    try
                    {
                        fsResult2 = serializer.TryDeserialize(gameData, ref targetGame);
                        if (fsResult2.Failed)
                        {
                            MessageBoxResult result = MessageBox.Show(string.Format("Error reported whilst deserializing Game data: {0}", fsResult2.FormattedMessages), "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxResult result = MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);

                        //foreach (object rawMessage in fsResult2.RawMessages)
                        //    Console.Write(rawMessage);

                        //Application.Current.Shutdown();
                        return;
                    }
                    //foreach (object rawMessage in fsResult2.RawMessages)
                    //  Console.Write(rawMessage);
                }
            }
        }

        private void TeamPage_OnListBoxUpdated(object sender, Team e)
        {
            var teamVM = SimpleIoc.Default.GetInstance<TeamViewModel>();
            teamVM.SetModel(e);
        }

        private void TeamPrinciplePage_OnListBoxUpdated(object sender, Person e)
        {
            var vm = SimpleIoc.Default.GetInstance<TeamPrincipalViewModel>();
            vm.SetModel(e as TeamPrincipal);
        }

        private void DriverPage_OnListBoxUpdated(object sender, Person e)
        {
            var vm = SimpleIoc.Default.GetInstance<DriverViewModel>();
            vm.SetModel(e as Driver);
        }

        private void EngineerPage_OnListBoxUpdated(object sender, Person e)
        {
            var vm = SimpleIoc.Default.GetInstance<EngineerViewModel>();
            vm.SetModel(e as Engineer);
        }

        private void MechanicPage_OnListBoxUpdated(object sender, Person e)
        {
            var vm = SimpleIoc.Default.GetInstance<MechanicViewModel>();
            vm.SetModel(e as Mechanic);
        }

        private void Hyperlink_RequestNavigate(object sender,
                                       RequestNavigateEventArgs e)
        {

            Process.Start(e.Uri.ToString());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void SwitchToTab(TabPage driver)
        {
            TabItem chosenTab = null;
            switch (driver)
            {
                case TabPage.TeamPrincipal:
                    break;
                case TabPage.Driver:
                    foreach (TabItem tabControlItem in tabControl.Items)
                    {
                        if (tabControlItem.Name.Equals("DriversTabItem"))
                        {
                            chosenTab = tabControlItem;
                            break;
                        }
                    }
                    break;
                case TabPage.Team:
                    break;
                case TabPage.Game:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(driver), driver, null);
            }
            if (chosenTab != null)
            {
                tabControl.SelectedItem = chosenTab;
            }
        }
    }
}
