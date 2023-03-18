using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.ViewModels;

namespace FoodDlvAPI.Interfaces
{
    public interface ICartRepository
    {
        /// <summary>
        /// 回傳此顧客的購物車是否存在
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        bool IsExists(int memberId, int storeId);

        /// <summary>
        /// 建立該會員對應商店專屬的空的購物車
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        CartDTO CreateNewCart(int memberId, int storeId);

        /// <summary>
        /// 讀取目前購物車的必要內容
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        CartDTO Load(int memberId, int storeId);

        /// <summary>
        /// 添加購物車明細, 包含各項驗證功能
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="request"></param>
        void AddDetail(CartDTO cart, CartDTO request);
        
        /// <summary>
        /// 清空購物車, 並刪除Database中的購物車主檔與明細檔
        /// </summary>
        /// <param name="memberId"></param>
        void EmptyCart(int memberId, int storeId);

        /// <summary>
        /// 監聽"加入購物車", 觸發商品客製化識別號碼的選擇器
        /// </summary>
        /// <returns></returns>
        int IdentifyNumSelector(long cartId);

        /// <summary>
        /// 讀取目前購物車的詳細內容
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        List<CartDTO> GetCartInfos(int memberId);

        /// <summary>
        /// 移除購物車指定明細
        /// </summary>
        /// <param name="identifyNum"></param>
        void RemoveDetail(int identifyNum);

        /// <summary>
        /// 結帳時確認購物車內容
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        CartDTO CheckOutCart(long cartId);
    }
}
