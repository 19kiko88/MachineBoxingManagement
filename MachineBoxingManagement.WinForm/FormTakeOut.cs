using MachineBoxingManagement.Services;
using MachineBoxingManagement.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MachineBoxingManagement.WinForm
{
    public partial class FormTakeOut : Form
    {
        private readonly BoxOutService _boxOutService;
        private readonly ExcelService _excelService;
        private readonly int _function;

        private List<MachineBoxingInfoView> _data = new List<MachineBoxingInfoView>();
        private bool unsave = false;

        public FormTakeOut()
        {
            InitializeComponent();
            //_data = new List<MachineBoxingInfoView>();
            dataGridView1.Width = this.Width - 80;
            dataGridView1.Height = this.Height - 170;
        }

        public FormTakeOut(List<MachineBoxingInfoView> data, BoxOutService boxOutService, ExcelService excelService, int function = 1)
        {
            _boxOutService = boxOutService;
            _excelService = excelService;
            _function = function;
            InitializeComponent();
            InitDatagridViewColumn();
            //_data = data;
            SetupData(data);
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[1].Frozen = true;
        }

        private void FormTakeOut_Load(object sender, EventArgs e)
        {

            switch (_function)
            {
                case 1:
                    buttonTakeout.Text = "取出機台";
                    break;
                case 0:
                default:
                    buttonTakeout.Text = "儲存變更";
                    break;
            }

            this.MouseLeave += (s, ev) =>
            {
                if (unsave && MessageBox.Show("尚有修改項目未儲存，是否儲存？", "請確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaveTempProcess();
                }
                unsave = false;
            };

            this.SizeChanged += (s, ev) =>
            {
                dataGridView1.Width = this.Width - 80;
                dataGridView1.Height = this.Height - 170;
                resizeDgvColumns();
            };

            buttonCloseOut.Click += (s, ev) =>
            {               
                this.Close();
            };

            buttonTakeout.Click += (s, ev) =>
            {
                if (MessageBox.Show("確定要進行操作？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    switch (_function)
                    {
                        case 1:
                            TakoutProcess();
                            break;
                        case 0:
                        default:
                            SaveTempProcess();
                            break;
                    }
                }
            };

            dataGridView1.CellValueChanged += (s, ev) =>
            {
                switch (_function)
                {
                    case 1:
                        break;
                    case 0:
                    default:
                        unsave = true;
                        break;
                }
                
                if (dataGridView1.Columns[ev.ColumnIndex].HeaderText == "第幾層")
                {
                    UnifyStackLevel(ev.RowIndex);
                }
            };

            dataGridView1.RowPostPaint += (s, ev) =>
            {
                using (SolidBrush b = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
                {
                    ev.Graphics.DrawString((ev.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentUICulture),
                        dataGridView1.DefaultCellStyle.Font, b, ev.RowBounds.Location.X + 10,
                        ev.RowBounds.Location.Y + 4);
                }

                //color
                var mid = Convert.ToInt32(dataGridView1.Rows[ev.RowIndex].Cells["ID"].Value);
                if (Form1.boxingFullIds.Contains(mid))
                    dataGridView1.Rows[ev.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                else if (ev.RowIndex == dataGridView1.Rows.Count - 1)
                    dataGridView1.Rows[ev.RowIndex].DefaultCellStyle.BackColor = Color.LightBlue;
                else
                    dataGridView1.Rows[ev.RowIndex].DefaultCellStyle.BackColor = Color.White;
            };

        }

        private void UnifyStackLevel(int rowindex)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                var boxname = row.Cells["箱名"].Value.ToString();
                var boxserial = row.Cells["箱號"].Value.ToString();
                var location = row.Cells["地點"].Value.ToString();
                if (boxname == dataGridView1.Rows[rowindex].Cells["箱名"].Value.ToString() &&
                    boxserial == dataGridView1.Rows[rowindex].Cells["箱號"].Value.ToString() &&
                    location == dataGridView1.Rows[rowindex].Cells["地點"].Value.ToString())
                {
                    row.Cells["第幾層"].Value = dataGridView1.Rows[rowindex].Cells["第幾層"].Value;
                }
            }
        }

        private void UnifyStackLevel(string nloc, string nboxname, string nserial, string nstacklvl)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                var boxname = row.Cells["箱名"].Value.ToString();
                var boxserial = row.Cells["箱號"].Value.ToString();
                var location = row.Cells["地點"].Value.ToString();
                if (boxname == nboxname &&
                    boxserial == nserial &&
                    location == nloc)
                {
                    row.Cells["第幾層"].Value = nstacklvl;
                }
            }
        }

        private void SaveTempProcess()
        {
            var cols = new List<Tuple<int, DataGridViewRow>>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                var mid = Convert.ToInt32(row.Cells["ID"].Value);
                var stack = row.Cells["第幾層"].Value.ToString();
                var serial = row.Cells["箱號"].Value.ToString();
                var machine = Form1.machineBoxingInfos.Where(a => a.Id == mid).FirstOrDefault();
                var targetData = _data.Where(a => a.ID == mid).FirstOrDefault();
                if (Convert.ToBoolean(row.Cells["移除?"].Value))
                {
                    cols.Add(Tuple.Create(mid, row));
                    //check
                    Form1.machineBoxingInfos.Remove(machine);
                    _data.Remove(targetData);
                }
                else
                {
                    if (int.TryParse(stack, out int lvl) && lvl >= 1 && lvl <= 5)
                    {
                        machine.StackLevel = lvl;
                        targetData.StackLevel = lvl;
                    }
                    if (int.TryParse(serial, out int sil) && sil >= 1)
                    {
                        machine.BoxingSerial = sil;
                        targetData.BoxingSerial = sil;
                    }
                }
            }

            foreach (var row in cols.Select(a => a.Item2).ToList())
            {
                dataGridView1.Rows.Remove(row);
            }
            unsave = false;
        }

        private void TakoutProcess()
        {
            var cols = new List<Tuple<int, DataGridViewRow>>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells["取出?"].Value))
                {
                    var mid = Convert.ToInt32(row.Cells["ID"].Value);
                    cols.Add(Tuple.Create(mid, row));
                }
            }
            var outResult = _boxOutService.TakeoutMachines(cols.Select(a => a.Item1).ToList(), Environment.UserName, out string outErrorMsg);

            if (!string.IsNullOrEmpty(outErrorMsg))
            {
                MessageBox.Show($"異常，請再試一次：{outErrorMsg}");
            }
            else
            {
                MessageBox.Show($"完成操作。");
            }

            foreach (var row in cols.Where(a => outResult.Contains(a.Item1)).ToList())
            {
                _data.Remove(_data.Where(a => a.ID == row.Item1).FirstOrDefault());
                dataGridView1.Rows.Remove(row.Item2);
            }
        }

        private void resizeDgvColumns()
        {
            dataGridView1.Columns[0].Width = Math.Max(dataGridView1.Width * 50 / 1060, 50);
            dataGridView1.Columns[1].Width = Math.Max(dataGridView1.Width * 130 / 1060, 130);
            dataGridView1.Columns[6].Width = Math.Max(dataGridView1.Width * 250 / 1060, 250);
            dataGridView1.Columns[5].Width = Math.Max(dataGridView1.Width * 80 / 1060, 80);
            dataGridView1.Columns[7].Width = Math.Max(dataGridView1.Width * 100 / 1060, 100);
            dataGridView1.Columns[8].Width = Math.Max(dataGridView1.Width * 80 / 1060, 80);
            dataGridView1.Columns[9].Width = Math.Max(dataGridView1.Width * 80 / 1060, 80);
            dataGridView1.Columns[2].Width = Math.Max(dataGridView1.Width * 100 / 1060, 100);
            dataGridView1.Columns[3].Width = Math.Max(dataGridView1.Width * 50 / 1060, 50);
            dataGridView1.Columns[4].Width = Math.Max(dataGridView1.Width * 80 / 1060, 80);
            dataGridView1.Columns[10].Width = Math.Max(dataGridView1.Width * 80 / 1060, 80);
            dataGridView1.Columns[11].Width = Math.Max(dataGridView1.Width * 80 / 1060, 80);
        }

        public void ResetData()
        {
            dataGridView1.Rows.Clear();
            _data = new List<MachineBoxingInfoView>();

        }

        public void SetupData(List<MachineBoxingInfoView> data)
        {
            var curIds = new List<int>();

            foreach (DataGridViewRow dvr in dataGridView1.Rows)
            {
                curIds.Add(Convert.ToInt32(dvr.Cells["ID"].Value));
            }
            foreach (var d in data)
            {
                if (!curIds.Contains(d.ID))
                {
                    dataGridView1.Rows.Add(
                        d.Takeout,
                        d.PartNumber,
                        d.BoxingName,
                        d.BoxingSerial,
                        d.StackLevel,
                        d.Model,
                        d.Description,
                        d.InStockDate,
                        d.Location,
                        d.BoxingOption,
                        d.Operator,
                        d.OperationTime,
                        d.ID
                        );
                    switch (_function)
                    {
                        case 1:
                            break;
                        case 0:
                        default:
                            UnifyStackLevel(d.Location, d.BoxingName, d.BoxingSerial.ToString(), d.StackLevel.ToString());
                            break;
                    }
                    
                    _data.Add(new MachineBoxingInfoView
                    {
                        ID = d.ID,
                        BoxingName = d.BoxingName,
                        BoxingOption = d.BoxingOption,
                        BoxingSerial = d.BoxingSerial,
                        Description = d.Description,
                        Model = d.Model,
                        InStockDate = d.InStockDate,
                        Location = d.Location,
                        Operator = d.Operator,
                        OperationTime = d.OperationTime,
                        SSN = d.SSN,
                        StackLevel = d.StackLevel,
                        Status = "666",
                        PartNumber = d.PartNumber,
                        Takeout = d.Takeout,
                        Takeouter = "",
                        TakeoutTime = (DateTime?)null
                    });
                }
                    
            }

            switch (_function)
            {
                case 1:
                    break;
                case 0:
                default:
                    SaveTempProcess();
                    break;
            }

            //focus on last row
            if (dataGridView1.Rows.Count > 0)
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
            
                
        }


        private void InitDatagridViewColumn()
        {
            var totext = "";
            switch (_function)
            {
                case 1:
                    totext = "取出?";
                    break;
                case 0:
                default:
                    totext = "移除?";
                    break;
            }


            DataGridViewCheckBoxColumn dt0 = new DataGridViewCheckBoxColumn();
            dt0.HeaderText = totext;
            dt0.Name = totext;
            dataGridView1.Columns.Insert(0, dt0);
            dt0.Width = 50;

            DataGridViewTextBoxColumn dt1 = new DataGridViewTextBoxColumn();
            dt1.HeaderText = "料號";
            dt1.Name = "料號";
            dataGridView1.Columns.Insert(1, dt1);
            dt1.Width = 130;
            dt1.ReadOnly = true;

            DataGridViewTextBoxColumn dt7 = new DataGridViewTextBoxColumn();
            dt7.HeaderText = "箱名";
            dt7.Name = "箱名";
            dataGridView1.Columns.Insert(2, dt7);
            dt7.Width = 100;
            dt7.ReadOnly = true;

            DataGridViewTextBoxColumn dt12 = new DataGridViewTextBoxColumn();
            dt12.HeaderText = "箱號";
            dt12.Name = "箱號";
            dataGridView1.Columns.Insert(3, dt12);
            dt12.Width = 50;

            DataGridViewTextBoxColumn dt8 = new DataGridViewTextBoxColumn();
            dt8.HeaderText = "第幾層";
            dt8.Name = "第幾層";
            dataGridView1.Columns.Insert(4, dt8);
            dt8.Width = 80;

            DataGridViewTextBoxColumn dt3 = new DataGridViewTextBoxColumn();
            dt3.HeaderText = "機種";
            dt3.Name = "機種";
            dataGridView1.Columns.Insert(5, dt3);
            dt3.Width = 80;
            dt3.ReadOnly = true;

            DataGridViewTextBoxColumn dt2 = new DataGridViewTextBoxColumn();
            dt2.HeaderText = "品規";
            dt2.Name = "品規";
            dataGridView1.Columns.Insert(6, dt2);
            dt2.Width = 250;
            dt2.ReadOnly = true;

            DataGridViewTextBoxColumn dt4 = new DataGridViewTextBoxColumn();
            dt4.HeaderText = "入庫日期";
            dt4.Name = "入庫日期";
            dataGridView1.Columns.Insert(7, dt4);
            dt4.Width = 100;
            dt4.ReadOnly = true;

            DataGridViewTextBoxColumn dt5 = new DataGridViewTextBoxColumn();
            dt5.HeaderText = "地點";
            dt5.Name = "地點";
            dataGridView1.Columns.Insert(8, dt5);
            dt5.Width = 80;
            dt5.ReadOnly = true;

            DataGridViewTextBoxColumn dt6 = new DataGridViewTextBoxColumn();
            dt6.HeaderText = "選項";
            dt6.Name = "選項";
            dataGridView1.Columns.Insert(9, dt6);
            dt6.Width = 80;
            dt6.ReadOnly = true;

            DataGridViewTextBoxColumn dt9 = new DataGridViewTextBoxColumn();
            dt9.HeaderText = "操作者";
            dt9.Name = "操作者";
            dataGridView1.Columns.Insert(10, dt9);
            dt9.Width = 80;
            dt9.ReadOnly = true;

            DataGridViewTextBoxColumn dt10 = new DataGridViewTextBoxColumn();
            dt10.HeaderText = "操作日期";
            dt10.Name = "操作日期";
            dataGridView1.Columns.Insert(11, dt10);
            dt10.Width = 80;
            dt10.ReadOnly = true;

            DataGridViewTextBoxColumn dt11 = new DataGridViewTextBoxColumn();
            dt11.HeaderText = "ID";
            dt11.Name = "ID";
            dataGridView1.Columns.Insert(12, dt11);
            dt11.Width = 0;
            dt11.ReadOnly = true;

        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            var excelBytes = _excelService.ExportExcelReport(_data.OrderBy(a => a.Location).ThenBy(a => a.BoxingName).ThenBy(a => a.BoxingSerial).ToList());
            if (!excelBytes.Success)
            {
                MessageBox.Show(excelBytes.Message);
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";
                saveFileDialog.FileName = $"機台資訊_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    System.Threading.Thread.Sleep(1000);
                    System.IO.File.WriteAllBytes(saveFileDialog.FileName, excelBytes.Content);
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                }
            }
        }

    }
}
