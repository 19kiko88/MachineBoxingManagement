# Machine Boxing Management_v1.3.6

CAE 儲位管理


## 專案說明
- MachineBoxingManagement.Web=>主要專案(Angular + WebAPI)
- MachineBoxingManagement.WinForm=>舊專案
- MachineBoxingManagement.Service=>商業邏輯
## 前端使用套件
- ng-bootstrap：
    - nav分頁頁籤功能
- Angular Material：
    - mat-table
    - mat-checkbox(全選)
## Javascript API
- postMessage：主要視窗<=>Popup視窗溝通(ex:主視窗關閉時，通知popup關閉)

## Update List
- v1.3.6
    - user return bug fix_20220929：
        - fix table header 2.更新暫存區狀態，不為查詢條件的料號也要更新 
        - 輸入P/N後機台選項不reset回預設值
- v1.3.5
    - 修改取出saveChange()寫法(pipe:switchMap + tap) 
    - 變更順序為先變更暫存區狀態後取出，因為先取出會把機台移出(splice)array導致更新機台暫存狀態undefinded出錯
- v1.3.3
    - 修正取出儲存變更，沒有異動時不做更新
- v1.3.1。
    - 更新裝箱維護批次儲存寫法，改用promise chain
    - 新增console log紀錄
    - 新增loading遮罩
    - 取出維護查詢新增主畫面loading遮罩
