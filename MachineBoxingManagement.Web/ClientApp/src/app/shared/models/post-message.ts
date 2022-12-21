import { PartNumber_Model_Desc } from "./dto/response/box-in";

export interface IPostMessage {
  inputUserName?: string;
  boxinInputData?: PartNumber_Model_Desc;
  boxoutInputDatas?: PartNumber_Model_Desc[];
  queryMachines?: boolean;
  isLoading?: boolean;
  resetOperator?: boolean;
  refreshMain?: boolean;
}

