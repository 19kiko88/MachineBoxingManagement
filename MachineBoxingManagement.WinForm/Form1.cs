using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Services;
using MachineBoxingManagement.Services.Models;
using MachineBoxingManagement.WinForm.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniversalLibrary.Models;

namespace MachineBoxingManagement.WinForm
{
    public partial class Form1 : Form
    {

        public static List<int> boxingFullIds = new List<int>();

        internal static List<MachineBoxingInfo> machineBoxingInfos = new List<MachineBoxingInfo>();
        internal static List<BoxingNSInfo> boxingNSInfos = new List<BoxingNSInfo>();

        private readonly List<BoxingLocation> _boxingLocations;
        private readonly List<BoxingOption> _boxingOptions;
        private readonly List<BoxingStyle> _boxingStyles;
        private readonly BoxInService _boxInService;
        private readonly BoxOutService _boxOutService;
        private readonly ExcelService _excelService;

        private readonly string _tempFolder;
        private readonly string _notifyFolder;

        private FormTakeOut formTempView;
        private int tempMachineId = 1;
        private bool noevent = false;
        private DateTime opTime;
        private List<Tuple<string, string, string, bool?, bool?, bool?, DateTime?>> pnModelDescs = new List<Tuple<string, string, string, bool?, bool?, bool?, DateTime?>>();
        private WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();

        private const int maxinumBoxingQuantity = 20;

        public Form1()
        {
            InitializeComponent();
            //initializing

            player.settings.volume = 50;

            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.Text = "PC機台儲位系統" + (ConfigurationManager.ConnectionStrings["CAEDB01"].ConnectionString.ToLower().Contains("-vt") ?
                "(測試版)" : "(正式版)");
            labelPnDesc.Text = "";
            textBoxOperator.Text = Environment.UserName;
            numericUpDownTurtleLevel.Minimum = 1;
            numericUpDownTurtleLevel.Maximum = 5;
            textBoxMIBox.Enabled = false;
            checkBoxTempSave.Checked = true;
            opTime = DateTime.Now;
            _tempFolder = Path.Combine(Application.StartupPath, "temp");
            if (!Directory.Exists(_tempFolder)) Directory.CreateDirectory(_tempFolder);

            //auto clean up old files
            foreach (var file in Directory.GetFiles(_tempFolder).Where(a => File.GetLastWriteTime(a).AddDays(7) < DateTime.Now))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                }
            }

            _notifyFolder = Path.Combine(Application.StartupPath, "content", "Notification");
            if (!Directory.Exists(_notifyFolder)) Directory.CreateDirectory(_notifyFolder);

            comboBoxWarehouse.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMStyle.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMOption.DropDownStyle = ComboBoxStyle.DropDownList;

            _boxInService = new Services.BoxInService(
                String.Format(ConfigurationManager.ConnectionStrings["CAEDB01"].ConnectionString, "Oijf35ri^39i#"),
                ConfigurationManager.ConnectionStrings["CAEService"].ConnectionString);

            _boxOutService = new Services.BoxOutService(
                String.Format(ConfigurationManager.ConnectionStrings["CAEDB01"].ConnectionString, "Oijf35ri^39i#"),
                ConfigurationManager.ConnectionStrings["CAEService"].ConnectionString);

            _excelService = new ExcelService(String.Format(ConfigurationManager.ConnectionStrings["CAEDB01"].ConnectionString, "Oijf35ri^39i#"),
                _boxInService);

            //test
            //_excelService.GenerateBoxingStickers(machineBoxingInfos);

            //default dropdown list
            var blocs = _boxInService.EnumBoxingLocations();
            if (blocs.Success)
                _boxingLocations = blocs.Content;

            var bopts = _boxInService.EnumBoxingOptions();
            if (bopts.Success)
                _boxingOptions = bopts.Content;

            var bstyles = _boxInService.EnumBoxingStyles();
            if (bstyles.Success)
                _boxingStyles = bstyles.Content;

            comboBoxWarehouse.DataSource = _boxingLocations.OrderBy(a => a.Id).Select(a => a.Name).ToList();
            comboBoxMOption.DataSource = _boxingOptions.OrderBy(a => a.Id).Select(a => a.Name).ToList();
            comboBoxMStyle.DataSource = _boxingStyles.OrderBy(a => a.Id).Select(a => a.Name).ToList();

            var count = 0;
            //dynamic controls
            foreach (var w in _boxingLocations)
            {
                CheckBox chkbox = new CheckBox();
                chkbox.Text = w.Name;
                chkbox.Tag = "l_"+ w.Id;
                chkbox.Checked = true;
                chkbox.Location = new Point(label17.Location.X, label17.Location.Y + ++count*30);
                tabPageBoxOut.Controls.Add(chkbox);
            }

            count = 0;
            foreach (var w in _boxingOptions)
            {
                CheckBox chkbox = new CheckBox();
                chkbox.Text = w.Name;
                chkbox.Tag = "o_" + w.Id;
                chkbox.Checked = true;
                chkbox.Location = new Point(label16.Location.X, label17.Location.Y + ++count * 30);
                tabPageBoxOut.Controls.Add(chkbox);
            }

            count = 0;
            foreach (var w in _boxingStyles)
            {
                CheckBox chkbox = new CheckBox();
                chkbox.Text = w.Name;
                chkbox.Tag = "s_" + w.Id;
                chkbox.Checked = true;
                chkbox.Location = new Point(label15.Location.X, label17.Location.Y + ++count * 30);
                tabPageBoxOut.Controls.Add(chkbox);
            }

            //events

            checkBoxTopMost.CheckedChanged += (sender, args) =>
            {
                this.TopMost = checkBoxTopMost.Checked;
            };

            textBoxOperator.Leave += (sender, args) =>
            {
                if (!noevent)
                {
                    textBoxPN.Focus();
                }
            };

            textBoxOperator.KeyUp += (sender, args) =>
            {
                if (!noevent)
                {
                    if (args.KeyCode == Keys.Enter)
                    {
                        textBoxPN.Focus();
                    }
                }

            };

            textBoxPN.Leave += (sender, args) =>
            {
                if (!noevent && !string.IsNullOrEmpty(textBoxPN.Text.Trim()))
                {
                    ProcessingPN();
                    if (checkBoxTempSave.Checked)
                    {
                        
                        TempSave();
                        ShowTempSaveForm();
                        labelPnDesc.Text = "";
                    }
                }
                
            };

            textBoxPN.KeyUp += (sender, args) =>
            {
                if (!noevent && !string.IsNullOrEmpty(textBoxPN.Text.Trim()))
                {
                    if (args.KeyCode == Keys.Enter)
                    {
                        
                        ProcessingPN();
                        if (checkBoxTempSave.Checked)
                        {
                            TempSave();
                            ShowTempSaveForm();
                            labelPnDesc.Text = "";
                        }
                    }
                }
                
            };

            comboBoxMStyle.SelectedValueChanged += (sender, args) =>
            {
                //if (comboBoxMOption.SelectedValue != null && comboBoxMOption.SelectedValue.ToString() == "海運" &&
                //(comboBoxMStyle.SelectedValue.ToString() != "預設" &&
                //comboBoxMStyle.SelectedValue.ToString() != "待燒OA"))
                //{
                //    MessageBox.Show("海運機台只能選擇預設或待燒OA。");
                //    comboBoxMStyle.SelectedItem = "預設";
                //    return;
                //}
                if (!noevent && !string.IsNullOrEmpty(textBoxPN.Text.Trim()))
                {
                    ProcessingPN();
                    labelPnDesc.Text = "";
                }
                
            };

            comboBoxMOption.SelectedValueChanged += (sender, args) =>
            {
                //if (comboBoxMOption.SelectedValue != null && comboBoxMOption.SelectedValue.ToString() == "海運" &&
                //(comboBoxMStyle.SelectedValue.ToString() != "預設" &&
                //comboBoxMStyle.SelectedValue.ToString() != "待燒OA"))
                //{
                //    comboBoxMStyle.SelectedItem = "預設";
                //}
                if (!noevent && !string.IsNullOrEmpty(textBoxPN.Text.Trim()))
                {
                    ProcessingPN();
                    labelPnDesc.Text = "";
                }

            };


            textBoxSeries.Leave += (sender, args) =>
            {
                if (!noevent && !string.IsNullOrEmpty(textBoxSeries.Text.Trim()))
                {
                    var mib = GetMIB(textBoxSeries.Text, out string errMsg, boxingLocation: comboBoxWarehouse.Text);
                    if (mib.Item1 > 0)
                    {
                        numericUpDownBoxNumber.Minimum = mib.Item1;
                        numericUpDownBoxNumber.Value = mib.Item1;
                        textBoxMIBox.Text = (mib.Item2).ToString();
                        numericUpDownTurtleLevel.Value = mib.Item3;
                    }
                    else
                    {
                        numericUpDownBoxNumber.Minimum = 1;
                        numericUpDownBoxNumber.Value = 1;
                        textBoxMIBox.Text = "1";
                        numericUpDownTurtleLevel.Value = mib.Item3;
                    }
                }

            };

            textBoxSeries.KeyUp += (sender, args) =>
            {
                if (!noevent && !string.IsNullOrEmpty(textBoxSeries.Text.Trim()))
                {
                    if (args.KeyCode == Keys.Enter)
                    {
                        var mib = GetMIB(textBoxSeries.Text, out string errMsg, boxingLocation: comboBoxWarehouse.Text);
                        if (mib.Item1 > 0)
                        {
                            numericUpDownBoxNumber.Minimum = mib.Item1;
                            numericUpDownBoxNumber.Value = mib.Item1;
                            textBoxMIBox.Text = (mib.Item2).ToString();
                            numericUpDownTurtleLevel.Value = mib.Item3;
                        }
                        else
                        {
                            numericUpDownBoxNumber.Minimum = 1;
                            numericUpDownBoxNumber.Value = 1;
                            textBoxMIBox.Text = "1";
                            numericUpDownTurtleLevel.Value = mib.Item3;
                        }
                    }
                }

            };

            

            numericUpDownBoxNumber.ValueChanged += (sender, args) =>
            {
                if (!noevent)
                {
                    var mib = GetMIB(textBoxSeries.Text, out string errMsg, boxingLocation: comboBoxWarehouse.Text);
                    if (mib.Item1 > 0)
                    {
                        if (numericUpDownBoxNumber.Value > mib.Item1)
                        {
                            textBoxMIBox.Text = "1";
                            numericUpDownTurtleLevel.Value = 1;
                        }
                        else
                        {
                            textBoxMIBox.Text = (mib.Item2).ToString();
                            numericUpDownTurtleLevel.Value = mib.Item3;
                        }
                    }
                }
                
            };

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelVersion.Text = $"VER:{Application.ProductVersion}";
            //Services.BoxInService bis = new Services.BoxInService(
            //    "Server=tp-caevm-vt02.corpnet.asus;Port=5432;Database=CAEDB01;User Id=MsmAdmin;Password=Oijf35ri^39i#;Timeout=60;Command Timeout = 600;",
            //    "http://cae.corpnet.asus/CAEServices/CAEService.svc");

            //var qqq = bis.GetBoxingNames("90NB0CY2-T00041");
            //var ggg = bis.GetNextBoxingSerial(qqq.Content.First().Id);

        }

        private void TempSave()
        {
            if (string.IsNullOrEmpty(textBoxPN.Text))
            {
                //error
                MessageBox.Show("P/N為空白值，無法暫存!");
            }
            else
            {


                var boxingLocid = _boxingLocations.Where(a => a.Name == comboBoxWarehouse.Text).Select(a => a.Id).FirstOrDefault();
                var boxingoptid = _boxingOptions.Where(a => a.Name == comboBoxMOption.Text).Select(a => a.Id).FirstOrDefault();

                var tempid = tempMachineId++;
                machineBoxingInfos.Add(new MachineBoxingInfo
                {
                    Id = tempid,
                    PartNumber = textBoxPN.Text.Trim().ToUpper(),
                    Ssn = textBoxSSN.Text,
                    BoxingLocationId = boxingLocid,
                    BoxingOptionId = boxingoptid,
                    BoxingName = textBoxSeries.Text,
                    BoxingSerial = (int)numericUpDownBoxNumber.Value,
                    StackLevel = (int)numericUpDownTurtleLevel.Value,
                    StatusId = 666,
                    Operator = textBoxOperator.Text,
                    OperateTime = opTime
                });

                if (Convert.ToInt32(textBoxMIBox.Text) == maxinumBoxingQuantity)
                {
                    //full
                    boxingFullIds.Add(tempid);
                    Task.Run(() => PlaySound("滿箱音效.mp3"));
                }
                else
                {
                    //now clear everytime
                    boxingFullIds.Clear();
                }

                ResetMachineInfo();

            }
        }

        private void ResetMachineInfo()
        {
            //reset
            noevent = true;
            labelPnDesc.Text = "";
            textBoxPN.Text = "";
            textBoxSSN.Text = "";
            textBoxModel.Text = "";
            textBoxMIBox.Text = "";
            textBoxSeries.Text = "";
            comboBoxMStyle.Text = "";
            numericUpDownBoxNumber.Minimum = 1;
            numericUpDownBoxNumber.Value = 1;
            numericUpDownTurtleLevel.Value = 1;
            textBoxPN.Focus();
            noevent = false;
        }

        private Tuple<int, int, int> GetMachineInBoxCount(string boxingname, int boxinglocationid)
        {

            var countDic = machineBoxingInfos.Where(a => a.BoxingName == boxingname && a.BoxingLocationId == boxinglocationid).GroupBy(a => a.BoxingSerial).ToDictionary(a => a.Key, a => a.Count()).OrderByDescending(a => a.Key);

            var turtleLvl = machineBoxingInfos.Where(a => a.BoxingName == boxingname && a.BoxingLocationId == boxinglocationid && a.BoxingSerial == countDic.FirstOrDefault().Key).Select(a => a.StackLevel).FirstOrDefault();

            return Tuple.Create(countDic.FirstOrDefault().Key, countDic.FirstOrDefault().Value, turtleLvl ?? 1);// 1:箱號 2:裝箱數量 3:烏龜車層數


        }

        private Tuple<int, int, int> GetMIB(string boxingName, out string errMsg, string boxingLocation)
        {
            errMsg = "";
            Tuple<int, int, int> result;
            var blid = _boxingLocations.Where(b => b.Name == boxingLocation).Select(b => b.Id).FirstOrDefault();

            //in memory
            var mib = GetMachineInBoxCount(boxingName, blid);
            var inMemoryData = boxingNSInfos.Where(a => a.BoxingName == boxingName && a.BoxingLocation == boxingLocation).FirstOrDefault();

            if (mib.Item1 == 0)
            {
                //get db count
                var bnumberResult = _boxInService.GetBoxingSerialInfo(boxingName, blid);
                if (!bnumberResult.Success)
                {
                    errMsg = $"取箱號失敗，訊息:{bnumberResult.Message}";
                    return Tuple.Create(-1, 1, 1);
                }
                else
                {

                    if (bnumberResult.Content.Item2 + 1 > maxinumBoxingQuantity)
                    {
                        result = Tuple.Create(bnumberResult.Content.Item1 + 1, 1, 1);
                    }
                    else
                    {
                        result = Tuple.Create(bnumberResult.Content.Item1 == 0 ? 1 : bnumberResult.Content.Item1, bnumberResult.Content.Item2 + 1, bnumberResult.Content.Item3);
                    }

                    
                    if (inMemoryData == null)
                    {
                        boxingNSInfos.Add(new BoxingNSInfo
                        {
                            BoxingName = boxingName,
                            BoxingLocation = boxingLocation,
                            BoxingSerial = result.Item1,
                            Quantity = result.Item2 - 1,
                            StackLvl = result.Item3
                        });
                    }
                    else
                    {
                        inMemoryData.BoxingSerial = result.Item1;
                        inMemoryData.Quantity = result.Item2 - 1;
                        inMemoryData.StackLvl = result.Item3;
                    }
                }
            }
            else
            {

                if (mib.Item2 + (mib.Item1 == inMemoryData.BoxingSerial ? inMemoryData.Quantity : 0) + 1 > maxinumBoxingQuantity)
                {
                    result = Tuple.Create(mib.Item1 + 1, 1, 1);
                }
                else
                {
                    result = Tuple.Create(mib.Item1, mib.Item2 + (mib.Item1 == inMemoryData.BoxingSerial ? inMemoryData.Quantity : 0) + 1, mib.Item3);
                }
            }

            return result;

        }

        private void ProcessingPN()
        {
            if (!string.IsNullOrEmpty(textBoxPN.Text))
            {
                noevent = true;
                var desc = "";
                var model = "";
                DateTime? insDate;
                bool? toOa = null;
                bool? tearDown = null;
                bool? mbResale = null;

                var pnfinded = pnModelDescs.Where(a => a.Item1.ToUpper() == textBoxPN.Text.Trim().ToUpper()).FirstOrDefault();

                if (pnfinded != null)
                {
                    desc = pnfinded.Item2;
                    model = pnfinded.Item3;
                    toOa = pnfinded.Item4;
                    tearDown = pnfinded.Item5;
                    mbResale = pnfinded.Item6;
                    insDate = pnfinded.Item7;
                }
                else
                {

                    var instockInfo = _boxOutService.GetInstockTime(new List<string> { textBoxPN.Text }, out string instockErr).FirstOrDefault();

                    if (!string.IsNullOrEmpty(instockErr) || !DateTime.TryParse(instockInfo.Item2, out DateTime isd))
                    {
                        Task.Run(() => PlaySound("錯誤音效.mp3"));
                        textBoxPN.Text = "";
                        MessageBox.Show($"無法取得料號入庫日資訊，請再重試看看，訊息:{instockErr}");
                        noevent = false;
                        return;
                    }

                    desc = instockInfo.Item3;
                    model = instockInfo.Item3.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries).GetValue(1).ToString().Split('/').GetValue(0).ToString().Split('-').GetValue(0).ToString();
                    insDate = isd;

                    switch (comboBoxMOption.Text)
                    {
                        case "一般NPI機台":
                        case String a when a.StartsWith("海運"):
                            var pnXstatus = _boxInService.GetPnXStatus(new List<string> { textBoxPN.Text });

                            if (!pnXstatus.Success)
                            {
                                Task.Run(() => PlaySound("錯誤音效.mp3"));
                                textBoxPN.Text = "";
                                MessageBox.Show($"無法取得料號OA相關訊息，請再重試看看，訊息:{pnXstatus.Message}");
                                noevent = false;
                                return;
                            }

                            toOa = pnXstatus.Content.First().ToOA;
                            tearDown = pnXstatus.Content.First().TearDown;
                            mbResale = pnXstatus.Content.First().MbResale;

                            if (toOa == null || mbResale == null)
                            {
                                Task.Run(() => PlaySound("錯誤音效.mp3"));
                                textBoxPN.Text = "";
                                MessageBox.Show($"料號尚未登入OA或拆解相關訊息，請先上傳再繼續，訊息:{pnXstatus.Message}");
                                noevent = false;
                                return;
                            }
                            if (!pnModelDescs.Any(b => b.Item1.ToUpper() == textBoxPN.Text.Trim().ToUpper()))
                            {
                                pnModelDescs.Add(Tuple.Create(textBoxPN.Text.Trim().ToUpper(),
                                    desc,
                                    model,
                                    toOa,
                                    tearDown,
                                    mbResale,
                                    insDate));
                            }

                            break;
                        default:

                            break;
                    }

                }

                if (toOa != null && toOa.Value)
                {
                    if (insDate.Value.AddYears(1) < DateTime.Now)
                    {
                        //已過一年可轉OA
                        comboBoxMStyle.SelectedItem = "待燒OA";
                    }
                }
                else if (tearDown != null && tearDown.Value)
                {
                    if (mbResale.Value)
                    {
                        comboBoxMStyle.SelectedItem = "可拆解可轉賣";
                    }
                    else
                    {
                        comboBoxMStyle.SelectedItem = "可拆解不可轉賣";
                    }
                }

                var bnameResult = _boxInService.GetBoxingName(textBoxPN.Text, model, comboBoxMOption.Text, comboBoxMStyle.Text, insDate.Value);

                if (!bnameResult.Success)
                {
                    Task.Run(() => PlaySound("錯誤音效.mp3"));
                    textBoxPN.Text = "";
                    MessageBox.Show($"P/N內容可能錯誤，訊息:{bnameResult.Message}");
                    noevent = false;
                    return;
                }

                textBoxSeries.Text = bnameResult.Content;
                textBoxModel.Text = model;
                labelPnDesc.Text = desc;

                var mib = GetMIB(textBoxSeries.Text, out string errMsg, comboBoxWarehouse.Text);
                if (mib.Item1 == -1)
                {
                    Task.Run(() => PlaySound("錯誤音效.mp3"));
                    textBoxPN.Text = "";
                    MessageBox.Show(errMsg);
                }
                else
                {
                    
                    numericUpDownBoxNumber.Minimum = mib.Item1;
                    numericUpDownBoxNumber.Value = mib.Item1;
                    textBoxMIBox.Text = (mib.Item2).ToString();
                    numericUpDownTurtleLevel.Value = mib.Item3;
                    
                }
                noevent = false;
            }
        }

        private void buttonBatchSave_Click(object sender, EventArgs e)
        {
            if (machineBoxingInfos.Count == 0)
            {
                MessageBox.Show("尚未刷入任一機台，無法批次儲存!");
                return;
            }
            //excel
            var usedBoxes = machineBoxingInfos.Select(a => Tuple.Create(
                a.BoxingLocationId, a.BoxingName, a.BoxingSerial
                )).Distinct()
                .Select(a => new MachineUniBoxingProp
                {
                    BoxingLocationID = a.Item1,
                    BoxingName = a.Item2,
                    BoxingSerial = a.Item3
                }).ToList();

            var dbBoxes = _boxInService.GetDBBoxes(usedBoxes);

            if (dbBoxes.Success == false)
            {
                //無法抓取DB箱資訊
            }
            else
            {
                dbBoxes.Content.AddRange(machineBoxingInfos);
            }

            var excelResult = _excelService.GenerateBoxingStickers(dbBoxes.Content, pnModelDescs.Select(a => Tuple.Create(a.Item1, a.Item2, a.Item3, a.Item7)).ToList());
            
            if (excelResult.Success)
            {
                //SaveFileDialog saveFileDialog = new SaveFileDialog();
                //saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";
                //saveFileDialog.FileName = $"外箱貼紙_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var xlsxFileName = $"外箱貼紙_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                System.Threading.Thread.Sleep(1000);
                excelResult.Content.SaveAs(Path.Combine(_tempFolder , xlsxFileName));

                System.Diagnostics.Process.Start(Path.Combine(_tempFolder, xlsxFileName));

                //if (saveFileDialog.ShowDialog() == DialogResult.OK)
                //{

                //    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                //}

            }

            if (!excelResult.Success)
            {
                if (MessageBox.Show(excelResult.Message + ",\n是否依然上傳至資料庫？", "無法產生Excel", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    excelResult.Success = true;
                }
            }

            if (excelResult.Success)
            {
                var saveResult = _boxInService.SaveBoxingInfos(machineBoxingInfos);
                if (!saveResult.Success || saveResult.Content == 0)
                {
                    MessageBox.Show($"上傳失敗，原因:{saveResult.Message}");
                }
                else
                {
                    MessageBox.Show("上傳完成！");

                    //clear
                    machineBoxingInfos = new List<MachineBoxingInfo>();
                    boxingNSInfos = new List<BoxingNSInfo>();
                    boxingFullIds = new List<int>();

                    opTime = DateTime.Now;
                    ResetMachineInfo();
                    if (formTempView is null || formTempView.IsDisposed)
                    { }
                    else
                        formTempView.ResetData();
                }
            }

    }

        private void buttonQuery_Click(object sender, EventArgs e)
        {
            //test
            //_boxOutService.QueryMachines(new Services.Models.QueryRule
            //{
            //    PartNumber = "90NB0BK1-T00020",
            //    Model = "T101HA"
            //}, errMsg: out string errMsg);

            if (textBoxPNQuery.Text.Trim() == "" && textBoxModelQuery.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入查詢條件");
                return;
            }

            var checkedTags = new List<string>();
            foreach (var ctrl in tabPageBoxOut.Controls)
            {
                if (ctrl is CheckBox)
                {
                    var chkbox = ctrl as CheckBox;
                    if (chkbox.Checked) checkedTags.Add(chkbox.Tag.ToString());
                }
            }


            var qresult = _boxOutService.QueryMachines(new Services.Models.QueryRule
            {
                PartNumber = textBoxPNQuery.Text.ToUpper().Trim(),
                Model = textBoxModelQuery.Text.Trim().ToUpper(),
                LocationIds = checkedTags.Where(a => a.StartsWith("l_")).Select(a => Convert.ToInt32(a.Split('_').GetValue(1).ToString())).ToList(),
                OptionIds = checkedTags.Where(a => a.StartsWith("o_")).Select(a => Convert.ToInt32(a.Split('_').GetValue(1).ToString())).ToList(),
                StyleIds = checkedTags.Where(a => a.StartsWith("s_")).Select(a => Convert.ToInt32(a.Split('_').GetValue(1).ToString())).ToList()
            }, errMsg: out string errMsg);

            if (!string.IsNullOrEmpty(errMsg))
            {
                MessageBox.Show("錯誤：" + errMsg);
            }
            else
            {
                FormTakeOut fto = new FormTakeOut(qresult, _boxOutService, _excelService);
                fto.Show();
            }

            textBoxModelQuery.Text = string.Empty;
            textBoxPNQuery.Text = string.Empty; 

        }

        private void buttonTempModify_Click(object sender, EventArgs e)
        {
            ShowTempSaveForm();
            
        }


        private void buttonTempSave_Click(object sender, EventArgs e)
        {
            TempSave();
            ShowTempSaveForm();
        }

        private void ShowTempSaveForm()
        {
            var machViews = _boxOutService.QueryMachines(new Services.Models.QueryRule(), out string erMsg, machineBoxingInfos);
            if (formTempView is null || formTempView.IsDisposed)
            {
                formTempView = new FormTakeOut(machViews, _boxOutService, _excelService, function: 0);
                formTempView.Show();
            }
            else
                formTempView.SetupData(machViews);
        }

        private void PlaySound(string name)
        {
            player.URL = Path.Combine(_notifyFolder, name);
            try
            {
                player.controls.play();
            }
            catch (Exception err)
            {
            }
            
        }

    }
}
