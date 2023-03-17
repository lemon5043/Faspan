import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr"; //signalr使用

//Member用
//(Member)結帳後通知商家用
async function NotifyTheStore(storeId, orderId) {
  try {
    const targetRole = "store";
    const storeIdToInt = parseInt(storeId)
    const connection = new HubConnectionBuilder()
      .withUrl("https://localhost:7093/OrderHub")
      .configureLogging(LogLevel.Information)
      .build();
    await connection.start();
    await connection.invoke("JoinGroup", { Id: storeIdToInt, Role: targetRole }); //連上StoreGroup
    await connection.invoke("NewOrder", storeIdToInt, orderId); //傳送訂單
    await connection.invoke("LeaveGroup", { Id: storeIdToInt, Role: targetRole }); //離開StoreGroup

  } catch (e) {
    console.log(e)
  }
}

//(Member)結帳後開啟HubGroup 等待通知用
async function LinkStart(memberId) {
  try {
    const role = "member"
    const connection = new HubConnectionBuilder()
      .withUrl("https://localhost:7093/OrderHub")
      .configureLogging(LogLevel.Information)
      .build();
    //監聽由server傳來的OrderId
    connection.on("OrderArrive", async (OrderId) => {
      //
      //todo 這裡填入收到訂單通知後要執行的邏輯
      //
    });
    await connection.start();
    await connection.invoke("JoinGroup", { Id: parseInt(memberId), Role: role });
  } catch (e) {
    console.log(e);
  }
}

//(Member)離開HubGroup
async function LeaveGrop(memberId) {
  try {
    const role = "member"
    const connection = new HubConnectionBuilder()
      .withUrl("https://localhost:7093/OrderHub")
      .configureLogging(LogLevel.Information)
      .build();

    await connection.start();
    await connection.invoke("LeaveGroup", { Id: parseInt(memberId), Role: role });
  } catch (e) {
    console.log(e);
  }
}

//store用
//(store)完成餐點後通知外送員用
async function AssignOrder(driverId, orderId) {
  try {
    const driverIdToInt = parseInt(driverId)
    const targetRole = "driver";
    const connection = new HubConnectionBuilder()
      .withUrl("https://localhost:7093/OrderHub")
      .configureLogging(LogLevel.Information)
      .build();

    await connection.start();
    await connection.invoke("JoinGroup", { Id: driverIdToInt, Role: targetRole }); //連上driverGroup
    await connection.invoke("AssignOrder", driverIdToInt, orderId); //傳送訂單
    await connection.invoke("LeaveGroup", { Id: driverIdToInt, Role: targetRole }); //離開driverGroup

  } catch (e) {
    console.log(e)
  }
}

//(store)營業時間內(自動?)開啟HubGroup 等待通知用
async function JoinGroup(storeId) {
  try {
    const role = "store"
    const connection = new HubConnectionBuilder()
      .withUrl("https://localhost:7093/OrderHub")
      .configureLogging(LogLevel.Information)
      .build();
    //監聽由server傳來的OrderId
    connection.on("NewOrder", async (OrderId) => {
      //
      //todo 這裡填入收到訂單通知後要執行的邏輯
      //
    });
    await connection.start();
    await connection.invoke("JoinGroup", { Id: parseInt(storeId), Role: role });
  } catch (e) {
    console.log(e);
  }
}

//(Member)營業時間外(自動?)離開HubGroup
async function LeaveGropS(storeId) {
  try {
    const role = "store"
    const connection = new HubConnectionBuilder()
      .withUrl("https://localhost:7093/OrderHub")
      .configureLogging(LogLevel.Information)
      .build();
    await connection.start();
    await connection.invoke("LeaveGroup", { Id: parseInt(storeId), Role: role });
  } catch (e) {
    console.log(e);
  }
}