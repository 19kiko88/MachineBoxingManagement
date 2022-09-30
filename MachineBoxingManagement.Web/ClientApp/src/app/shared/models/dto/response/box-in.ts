export interface PartNumber_Model_Desc {
  serial_No: number;
  id: number;
  part_Number: string;
  ssn: string;
  boxing_Location_Id: number;
  boxing_Location_Cn: string;
  boxing_Option_Id: number;
  boxing_Option_Cn: string;
  boxing_Series: string;
  boxing_Serial: number;
  turtle_Level: number;
  status_Id: number;
  operator: string;
  operate_Time: string;
  takeout_Operator: string;
  takeout_Operate_Time: string;
  desc: string;
  insDate?: string;
  mbResale?: null;
  model: string;
  tearDown?: boolean;
  toOA?: boolean;
  is_Buffer_Area: boolean;
  is_Favorite?: boolean;
}

export interface BoxinProcessingData {
  userName?: string;
  saveTemp: boolean;
  partNumber: string;
  location: number;
  machineOption: number;
  machineStyle: number;
  boxSeries: string;
  boxSerial: number;
  ssn: string;
  pnDatas: PartNumber_Model_Desc[];
}
