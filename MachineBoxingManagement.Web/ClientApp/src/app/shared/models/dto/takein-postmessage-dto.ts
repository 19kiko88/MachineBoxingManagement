import { PartNumber_Model_Desc } from "./response/box-in";

export interface ITakeInPostMessageDto
{
  inputUserName?: string;
  inputData?: PartNumber_Model_Desc;
  isParentClose?: boolean;
}

