export interface boxOutQueryCondition {
  pn?: string
  model?: string
  take_in_dt_s?: string
  take_in_dt_e?: string
  take_out_dt_s?: string
  take_out_dt_e?: string
  locations?: number[]
  options?: number[]
  styles?: number[]
  statuses?: number[]
  buffer_areas?: number[]
  //not_bufferArea?: boolean
}
