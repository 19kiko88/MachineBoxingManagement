export interface boxOutQueryCondition {
  pn?: string
  model?: string
  take_in_dt_s?: string
  take_in_dt_e?: string
  take_out_dt_s?: string
  take_out_dt_e?: string
  locations?: number[]
  all_ckb_list_option: boolean
  options?: number[]
  styles?: number[]
  statuses?: number[]
  buffer_areas?: number[]
  favorites? : number[]
  //not_bufferArea?: boolean
}
