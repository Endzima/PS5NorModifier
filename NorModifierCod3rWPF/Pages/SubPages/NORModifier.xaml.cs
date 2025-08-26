using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Microsoft.Win32;
using System.Xml;
using NorModifierCod3rWPF.Class;


namespace NorModifierCod3rWPF.Pages.SubPages
{
    /// <summary>
    /// Interaction logic for NORModifier.xaml
    /// </summary>
    public partial class NORModifier : Page
    {
        long offsetOne = 0x1c7010;
        long offsetTwo = 0x1c7030;
        long WiFiMacOffset = 0x1C73C0;
        string? WiFiMacValue = null;
        long LANMacOffset = 0x1C4020;
        string? LANMacValue = null;
        string? offsetOneValue = null;
        string? offsetTwoValue = null;
        long serialOffset = 0x1c7210;
        string? serialValue = null;
        long variantOffset = 0x1c7226;
        string? variantValue = null;
        long moboSerialOffset = 0x1C7200;
        string? moboSerialValue = null;

        public NORModifier()
        {
            InitializeComponent();
            CheckStringStorage();
        }

        private void CheckStringStorage()
        {
            // A function to check if a path has already been imported for the NOR in an existing app session so people don't have to import their BIOS when they switch back to this page from UART or About.

            if (StringStorage.NORPath != null)
            {
                Path.Text = StringStorage.NORPath;
                SetValues(StringStorage.NORPath);
            }
            else
            {
                Path.Text = "";
            }
        }

        private void DonateToCod3r(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenDonos();
            }
        }

        private void browseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialogBox = new OpenFileDialog();
            fileDialogBox.Title = "Open NOR BIN File";
            fileDialogBox.Filter = "PS5 BIN Files|*.bin";

            if (fileDialogBox.ShowDialog() == true)
            {
                if (fileDialogBox.CheckFileExists == false)
                {
                    showMsg("Error", "The file you selected could not be found. Please check the file exists and is a valid BIN file.");
                }
                else
                {
                    if (!fileDialogBox.SafeFileName.EndsWith(".bin"))
                    {
                        //showMsg("The file you selected is not a valid. Please ensure the file you are choosing is a correct BIN file and try again.");
                    }
                    else
                    {
                        string BiosPath = fileDialogBox.FileName;
                        // Set values in XAML
                        SetValues(BiosPath);
                    }
                }
            }
        }

        private void SetValues(string BiosPath)
        {
            StringStorage.NORPath = BiosPath;
            Path.Text = "";
            // Get the path selected and print it into the path box
            string selectedPath = BiosPath;
            Path.Text = selectedPath;

            // Get file length and show in bytes and MB
            long length = new System.IO.FileInfo(selectedPath).Length;
            fileSizeText.Text = length.ToString() + " bytes (" + length / 1024 / 1024 + "MB)";

            #region Extract PS5 Version

            try
            {
                BinaryReader reader = new BinaryReader(new FileStream(BiosPath, FileMode.Open));
                //Set the position of the reader
                reader.BaseStream.Position = offsetOne;
                //Read the offset
                offsetOneValue = BitConverter.ToString(reader.ReadBytes(12)).Replace("-", null);
                reader.Close();
            }
            catch
            {
                // Obviously this value is invalid, so null the value and move on
                offsetOneValue = null;
            }

            try
            {
                BinaryReader reader = new BinaryReader(new FileStream(BiosPath, FileMode.Open));
                //Set the position of the reader
                reader.BaseStream.Position = offsetOne;
                //Read the offset
                offsetTwoValue = BitConverter.ToString(reader.ReadBytes(12)).Replace("-", null);
                reader.Close();
            }
            catch
            {
                // Obviously this value is invalid, so null the value and move on
                offsetTwoValue = null;
            }


            if (offsetOneValue?.Contains("22020101") ?? false)
            {
                modelInfo.Text = "Disc Edition";
            }
            else
            {
                if (offsetTwoValue?.Contains("22030101") ?? false)
                {
                    modelInfo.Text = "Digital Edition";
                }
                else
                {
                    modelInfo.Text = "Unknown";
                }
            }

            #endregion

            #region Extract Motherboard Serial Number

            try
            {
                BinaryReader reader = new BinaryReader(new FileStream(BiosPath, FileMode.Open));
                //Set the position of the reader
                reader.BaseStream.Position = moboSerialOffset;
                //Read the offset
                moboSerialValue = BitConverter.ToString(reader.ReadBytes(16)).Replace("-", null);
                reader.Close();
            }
            catch
            {
                // Obviously this value is invalid, so null the value and move on
                moboSerialValue = null;
            }



            if (moboSerialValue != null)
            {
                moboSerialInfo.Text = NORFuncs.HexStringToString(moboSerialValue);
            }
            else
            {
                moboSerialInfo.Text = "Unknown";
            }

            #endregion

            #region Extract Board Serial Number

            try
            {
                BinaryReader reader = new BinaryReader(new FileStream(BiosPath, FileMode.Open));
                //Set the position of the reader
                reader.BaseStream.Position = serialOffset;
                //Read the offset
                serialValue = BitConverter.ToString(reader.ReadBytes(17)).Replace("-", null);
                reader.Close();
            }
            catch
            {
                // Obviously this value is invalid, so null the value and move on
                serialValue = null;
            }



            if (serialValue != null)
            {
                serialNumber.Text = NORFuncs.HexStringToString(serialValue);
                serialNumberTextbox.Text = NORFuncs.HexStringToString(serialValue);

            }
            else
            {
                serialNumber.Text = "Unknown";
            }

            #endregion

            #region Extract WiFi Mac Address

            try
            {
                BinaryReader reader = new BinaryReader(new FileStream(BiosPath, FileMode.Open));
                //Set the position of the reader
                reader.BaseStream.Position = WiFiMacOffset;
                //Read the offset
                WiFiMacValue = BitConverter.ToString(reader.ReadBytes(6));
                reader.Close();
            }
            catch
            {
                // Obviously this value is invalid, so null the value and move on
                WiFiMacValue = null;
            }

            if (WiFiMacValue != null)
            {
                macAddressInfo.Text = WiFiMacValue;
            }
            else
            {
                macAddressInfo.Text = "Unknown";
            }

            #endregion

            #region Extract LAN Mac Address

            try
            {
                BinaryReader reader = new BinaryReader(new FileStream(BiosPath, FileMode.Open));
                //Set the position of the reader
                reader.BaseStream.Position = LANMacOffset;
                //Read the offset
                LANMacValue = BitConverter.ToString(reader.ReadBytes(6));
                reader.Close();
            }
            catch
            {
                // Obviously this value is invalid, so null the value and move on
                LANMacValue = null;
            }

            if (LANMacValue != null)
            {
                LANMacAddressInfo.Text = LANMacValue;
            }
            else
            {
                LANMacAddressInfo.Text = "Unknown";
            }

            #endregion

            #region Extract Board Variant

            try
            {
                BinaryReader reader = new BinaryReader(new FileStream(BiosPath, FileMode.Open));
                //Set the position of the reader
                reader.BaseStream.Position = variantOffset;
                //Read the offset
                variantValue = BitConverter.ToString(reader.ReadBytes(19)).Replace("-", null).Replace("FF", null);
                reader.Close();
            }
            catch
            {
                // Obviously this value is invalid, so null the value and move on
                variantValue = null;
            }

            if (variantValue != null)
            {
                boardVariant.Text = NORFuncs.HexStringToString(variantValue);
                boardVariantComboBox.SelectedItem = DigitalSelection;
            }
            else
            {
                boardVariant.Text = "Unknown";
            }

            boardVariant.Text += boardVariant.Text switch
            {
                _ when boardVariant.Text.EndsWith("00A") || boardVariant.Text.EndsWith("00B") => " - Japan",
                _ when boardVariant.Text.EndsWith("01A") || boardVariant.Text.EndsWith("01B") || boardVariant.Text.EndsWith("15A") || boardVariant.Text.EndsWith("15B") => " - US, Canada, (North America)",
                _ when boardVariant.Text.EndsWith("02A") || boardVariant.Text.EndsWith("02B") => " - Australia / New Zealand, (Oceania)",
                _ when boardVariant.Text.EndsWith("03A") || boardVariant.Text.EndsWith("03B") => " - United Kingdom / Ireland",
                _ when boardVariant.Text.EndsWith("04A") || boardVariant.Text.EndsWith("04B") => " - Europe / Middle East / Africa",
                _ when boardVariant.Text.EndsWith("05A") || boardVariant.Text.EndsWith("05B") => " - South Korea",
                _ when boardVariant.Text.EndsWith("06A") || boardVariant.Text.EndsWith("06B") => " - Southeast Asia / Hong Kong",
                _ when boardVariant.Text.EndsWith("07A") || boardVariant.Text.EndsWith("07B") => " - Taiwan",
                _ when boardVariant.Text.EndsWith("08A") || boardVariant.Text.EndsWith("08B") => " - Russia, Ukraine, India, Central Asia",
                _ when boardVariant.Text.EndsWith("09A") || boardVariant.Text.EndsWith("09B") => " - Mainland China",
                _ when boardVariant.Text.EndsWith("11A") || boardVariant.Text.EndsWith("11B") || boardVariant.Text.EndsWith("14A") || boardVariant.Text.EndsWith("14B") => " - Mexico, Central America, South America",
                _ when boardVariant.Text.EndsWith("16A") || boardVariant.Text.EndsWith("16B") => " - Europe / Middle East / Africa",
                _ when boardVariant.Text.EndsWith("18A") || boardVariant.Text.EndsWith("18B") => " - Singapore, Korea, Asia", _ => " - Unknown Region"
            };
            #endregion
        }

        private void SaveBios(object sender, RoutedEventArgs e)
        {
            if (Path.Text == "")
            {
                showMsg("Warning", "Please select a valid BIOS file first before attempting to modify your BIOS...");
                return;
            }

            string fileNameToLookFor = "";
            bool errorShownAlready = false;

            if (modelInfo.Text == "" || modelInfo.Text == "...")
            {
                // No valid BIN file seems to have been selected
                showMsg("Warning", "Please select a valid BIOS file first...");
                errorShownAlready = true;
            }
            else
            {
                if (boardModelComboBox.Text == "")
                {
                    showMsg("Warning", "Please select a valid board model before saving new BIOS information!");
                    errorShownAlready = true;
                }
                else
                {
                    if (boardVariantComboBox.Text == "")
                    {
                        showMsg("Warning", "Please select a valid board variant before saving new BIOS information!");
                        errorShownAlready = true;
                    }
                    else
                    {
                        SaveFileDialog saveBox = new SaveFileDialog();
                        saveBox.Title = "Save NOR BIN File";
                        saveBox.Filter = "PS5 BIN Files|*.bin";

                        if (saveBox.ShowDialog() == true)
                        {
                            // First create a copy of the old BIOS file
                            byte[] existingFile = File.ReadAllBytes(StringStorage.NORPath);
                            string newFile = saveBox.FileName;

                            File.WriteAllBytes(newFile, existingFile);

                            fileNameToLookFor = saveBox.FileName;

                            #region Set the new model info
                            if (modelInfo.Text == "Disc Edition")
                            {
                                try
                                {

                                    if (boardVariantComboBox.Text == "Digital Edition")

                                    {

                                        byte[] find = NORFuncs.ConvertHexStringToByteArray(Regex.Replace("22020101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                        byte[] replace = NORFuncs.ConvertHexStringToByteArray(Regex.Replace("22030101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                        if (find.Length != replace.Length)
                                        {
                                            showMsg("Error", "The length of the old hex value does not match the length of the new hex value!");
                                            errorShownAlready = true;
                                        }
                                        byte[] bytes = File.ReadAllBytes(newFile);
                                        foreach (int index in NORFuncs.PatternAt(bytes, find))
                                        {
                                            for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                                            {
                                                bytes[i] = replace[replaceIndex];
                                            }
                                            File.WriteAllBytes(newFile, bytes);
                                        }
                                    }

                                }
                                catch
                                {
                                    showMsg("Error", "An error occurred while saving your BIOS file");
                                    errorShownAlready = true;
                                }
                            }
                            else
                            {
                                if (modelInfo.Text == "Digital Edition")
                                {
                                    try
                                    {

                                        if (boardVariantComboBox.Text == "Disc Edition")
                                        {

                                            byte[] find = NORFuncs.ConvertHexStringToByteArray(Regex.Replace("22030101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                            byte[] replace = NORFuncs.ConvertHexStringToByteArray(Regex.Replace("22020101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                            if (find.Length != replace.Length)
                                            {
                                                showMsg("Error", "The length of the old hex value does not match the length of the new hex value!");
                                                errorShownAlready = true;
                                            }
                                            byte[] bytes = File.ReadAllBytes(newFile);
                                            foreach (int index in NORFuncs.PatternAt(bytes, find))
                                            {
                                                for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                                                {
                                                    bytes[i] = replace[replaceIndex];
                                                }
                                                File.WriteAllBytes(newFile, bytes);
                                            }
                                        }

                                    }
                                    catch
                                    {
                                        showMsg("Error", "An error occurred while saving your BIOS file");
                                        errorShownAlready = true;
                                    }
                                }
                            }
                            #endregion

                            #region Set the new board variant

                            try
                            {
                                byte[] oldVariant = Encoding.UTF8.GetBytes(boardVariant.Text);
                                string oldVariantHex = Convert.ToHexString(oldVariant);

                                byte[] newVariantSelection = Encoding.UTF8.GetBytes(boardVariantComboBox.SelectedItem as string);
                                string newVariantHex = Convert.ToHexString(newVariantSelection);
                                byte[] find = NORFuncs.ConvertHexStringToByteArray(Regex.Replace(oldVariantHex, "0x|[ ,]", string.Empty).Normalize().Trim());
                                byte[] replace = NORFuncs.ConvertHexStringToByteArray(Regex.Replace(newVariantHex, "0x|[ ,]", string.Empty).Normalize().Trim());

                                byte[] bytes = File.ReadAllBytes(newFile);
                                foreach (int index in NORFuncs.PatternAt(bytes, find))
                                {
                                    for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                                    {
                                        bytes[i] = replace[replaceIndex];
                                    }
                                    File.WriteAllBytes(newFile, bytes);
                                }

                            }
                            catch (System.ArgumentException ex)
                            {
                                showMsg("Error", ex.Message.ToString());
                                errorShownAlready = true;
                            }

                            #endregion

                            #region Change Serial Number

                            try
                            {
                                byte[] oldSerial = Encoding.UTF8.GetBytes(serialNumber.Text);
                                string oldSerialHex = Convert.ToHexString(oldSerial);

                                byte[] newSerial = Encoding.UTF8.GetBytes(serialNumberTextbox.Text);
                                string newSerialHex = Convert.ToHexString(newSerial);
                                byte[] find = NORFuncs.ConvertHexStringToByteArray(Regex.Replace(oldSerialHex, "0x|[ ,]", string.Empty).Normalize().Trim());
                                byte[] replace = NORFuncs.ConvertHexStringToByteArray(Regex.Replace(newSerialHex, "0x|[ ,]", string.Empty).Normalize().Trim());

                                byte[] bytes = File.ReadAllBytes(newFile);
                                foreach (int index in NORFuncs.PatternAt(bytes, find))
                                {
                                    for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                                    {
                                        bytes[i] = replace[replaceIndex];
                                    }
                                    File.WriteAllBytes(newFile, bytes);
                                }

                            }
                            catch (System.ArgumentException ex)
                            {
                                showMsg("Error", ex.Message.ToString());
                                errorShownAlready = true;
                            }

                            #endregion
                        }
                        else
                        {
                            showMsg("Notice", "Save operation cancelled!");
                            errorShownAlready = true;
                        }
                    }
                }
            }

            if (File.Exists(fileNameToLookFor) && errorShownAlready == false)
            {
                // Reset everything and show message
                ResetAppFields();
                showMsg("Success!", "A new BIOS file was successfully created. It's highly recommended to load the new BIOS file to verify the information you entered before is correct prior to installing the BIOS onto your motherboard. Remember this software was forked by dogmountaindew and created by TheCod3r with nothing but love. Why not show some love back by dropping a small donation to TheCod3r to say thanks ;).");
            }
        }

        private void showMsg(string header, string errordescription)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ShowMessage(header, errordescription);
            }
        }

        private void ResetAppFields()
        {
            // Null all values for a new BIOS file to be added later to avoid any sort of mixup.
            serialNumber.Text = null;
            serialNumberTextbox.Text = null;
            boardVariant.Text = null;
            boardVariantComboBox.Text = null;
            modelInfo.Text = null;
            macAddressInfo.Text = null;
            LANMacAddressInfo.Text = null;
            moboSerialInfo.Text = null;
            fileSizeText.Text = null;
            Path.Text = null;
            boardModelComboBox.Text = null;
            boardVariantComboBox.Text = null;
        }
    }
}
