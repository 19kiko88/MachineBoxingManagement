export abstract class LocalStorageKey
{  
  public static readonly isTakeInModalOpen: string = "isTakeInModalOpen";//裝箱維護modal是否開啟。-1：切換分頁，強制關閉 0:關閉 1:已經開啟
  public static readonly isTakeOutModalOpen: string = "isTakeOutModalOpen";//取出維護modal是否開啟。-1：切換分頁，強制關閉 0:關閉 1:已經開啟
  public static readonly tempListData: string = "tempListData"/*temp_list_data*/; //裝箱維護暫存機台List
  public static readonly themeType: string = "themeType"/*theme_type*/; //主題配色。0:淺色 1:深色
  public static readonly jwt: string = "jwt"; //JWT
  public static readonly myFavorite: string = "myFavorite"/*my_favorite*/; //取出維護modal的暫存List
  public static readonly takeOutModalQueryCondition: string = "takeOutModalQueryCondition";//取出維護查詢條件
  public static readonly uuid: string = "uuid";//uuid用來檢核是否重複開啟MBM
  public static readonly operators: string = "operators";//MBM操作者
  public static readonly bufferAreas: string = "bufferAreas";//存放暫存區機台
}
