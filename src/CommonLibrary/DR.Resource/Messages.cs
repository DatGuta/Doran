namespace DR.Resource {

    public static class Messages {

        public static class Merchant {

            public static class Register {
                public const string Request_Invalid = "Lỗi! Vui lòng kiểm tra lại thông tin.";
                public const string Merchant_Existed = "Cửa hàng đã tồn tại.";
            }

            public static class GetApiKey {
                public const string Merchant_NotFound = "Cửa hàng không tồn tại.";
            }

            public static class GenerateApiKey {
                public const string Merchant_NotFound = "Cửa hàng không tồn tại.";
                public const string Merchant_Expired = "Cửa hàng không tồn tại.";
            }

            public static class Get {
                public const string Merchant_NotFound = "Cửa hàng không tồn tại.";
                public const string Merchant_Inactive = "Cửa hàng không hoạt động.";
                public const string Merchant_Expired = "Cửa hàng không tồn tại.";
            }
        }

        public static class Auth {

            public static class Login {
                public const string Merchant_NotFound = "Cửa hàng không tồn tại.";
                public const string Merchant_Inactive = "Cửa hàng không hoạt động.";
                public const string Merchant_Expired = "Cửa hàng đã hết hạn.";

                public const string User_NotFound = "Người dùng không tồn tại.";
                public const string User_Inactive = "Người dùng không hoạt động.";
                public const string User_IncorrectPassword = "Sai mật khẩu.";

                public const string User_NoPermission = "Bạn không có quyền đăng nhập vào hệ thống. Vui lòng liên hệ quản trị viên để được hổ trợ.";
                public const string User_IncorrectPinCode = "Mã PIN không chính xác";
            }
        }

        public static class User {

            public static class CreateOrUpdate {
                public const string Role_NotFound = "Phân quyền không tồn tại.";
                public const string User_Existed = "Người dùng đã tồn tại.";
                public const string User_NotFound = "Người dùng không tồn tại.";
                public const string User_NotInactive = "Không thể dừng hoạt động với người quản trị.";
                public const string User_NameIsRequire = "Tên người dùng không được để trống.";
            }

            public static class ChangePinCode {
                public const string User_NotFound = "Người dùng không tồn tại.";
                public const string User_IncorrentOldPinCode = "Sai mã PIN.";
                public const string User_IncorrentOldPassword = "Sai mật khẩu.";
            }

            public static class ChangePassword {
                public const string User_NotFound = "Người dùng không tồn tại.";
                public const string User_IncorrentOldPassword = "Sai mật khẩu.";
            }

            public static class ResetPinCode {
                public const string User_NotFound = "Người dùng không tồn tại.";
            }

            public static class ResetPassword {
                public const string User_NotFound = "Người dùng không tồn tại.";
            }

            public static class Delete {
                public const string User_NotFound = "Người dùng không tồn tại.";
                public const string User_NotDelete = "Chỉ xóa được người dùng ở trạng thái không cho phép hoạt động.";
            }
        }

        public static class Product {

            public static class CreateOrUpdate {
                public const string Product_Existed = "Sản phẩm đã tồn tại.";
                public const string Product_NotFound = "Sản phẩm không tồn tại.";
            }

            public static class Delete {
                public const string Product_NotFound = "Sản phẩm không tồn tại.";
                public const string Product_ExistStore = "Không thể xóa sản phẩm đang được bán ở các cửa hàng.";
                public const string Product_ExistWarehouse = "Chỉ xóa được sản phẩm với trạng thái dừng bán ở các kho và tồn kho của sản phẩm tại các kho bằng 0.";
            }
        }

        public static class Image {
            public const string Image_Error = "Đã có lỗi xảy ra khi lưu hình ảnh.";
        }

        public static class Supplier {

            public static class CreateOrUpdate {
                public const string Supplier_Existed = "Nhà cung cấp đã tồn tại.";
                public const string Supplier_NotFound = "Nhà cung cấp không tồn tại.";
            }

            public static class Delete {
                public const string Supplier_NotFound = "Nhà cung cấp không tồn tại.";
                public const string Supplier_OnUsing = "Không thể xóa nhà cung cấp khi đang được sử dụng.";
            }
        }

        public static class Brand {

            public static class CreateOrUpdate {
                public const string Brand_Existed = "Thương hiệu đã tồn tại.";
                public const string Brand_NotFound = "Thương hiệu không tồn tại.";
            }

            public static class Delete {
                public const string Brand_NotFound = "Thương hiệu không tồn tại.";
                public const string Brand_OnUsing = "Không thể xóa thương hiệu khi đang được sử dụng.";
            }
        }

        public static class Customer {

            public static class CreateOrUpdate {
                public const string Customer_Existed = "Khách hàng đã tồn tại.";
                public const string Customer_NotFound = "Khách hàng không tồn tại.";
            }

            public static class Import {
                public const string Customer_NotRead = "Không thể đọc được tập dữ liệu.";
                public const string Customer_NameNotNull = "Tên khách hàng không được để trống.";
                public const string Customer_CodeNotNull = "Mã khách hàng không được để trống.";
                public const string Customer_IsExit = "Mã khách hàng đã tồn tại.";
            }

            public static class Delete {
                public const string Customer_NotFound = "Khách hàng không tồn tại.";
                public const string Customer_DeleteSuccessed = "Xóa khách hàng thành công.";
                public const string Customer_DeleteFail = "Xóa khách hàng thất bại.";
            }
        }

        public static class CustomerGroup {

            public static class CreateOrUpdate {
                public const string Customer_NotFound = "Nhóm Khách hàng không tồn tại.";
            }

            public static class Delete {
                public const string Customer_NotFound = "Nhóm Khách hàng không tồn tại.";
                public const string Customer_DeleteSuccessed = "Xóa nhóm khách hàng thành công.";
                public const string Customer_DeleteFail = "Vui lòng xóa khách hàng khỏi nhóm khách hàng trước khi xóa.";
            }
        }

        public static class CustomerDebt {
            public const string Customer_NotFound = "Khách hàng không tồn tại.";
            public const string Debt_Invalid = "Nợ gốc không hợp lệ.";

            public static class Debt {
                public const string OriginalDebt_InProgress = "Bạn không thể cài đặt nợ gốc cho khách hàng này vì quá trình cài đặt nợ gốc đang diễn ra.";
                public const string OriginalDebt_Error = "Bạn không thể cài đặt nợ gốc cho khách hàng này.";
            }

            public static class Pay {
                public const string Value_Invalid = "Số tiền không hợp lệ.";
                public const string PaymentMethod_NotFound = "Không tìm thấy phương thức thanh toán.";
                public const string Payment_InProgress = "Bạn không thể tiến hành thanh toán vì quá trình thanh toán đang diễn ra.";
            }
        }

        public static class Role {

            public static class CreateOrUpdate {
                public const string Role_Existed = "Phân quyền đã tồn tại.";
                public const string Role_NotFound = "Phân quyền không tồn tại.";
            }

            public static class Delete {
                public const string Role_NotFound = "Phân quyền không tồn tại.";
                public const string Role_NotDelete = "Chỉ xóa được phân quyền khi không được sử dụng bởi bất kỳ người dùng nào.";
            }
        }

        public static class Warehouse {

            public static class CreateOrUpdate {
                public const string Warehouse_Existed = "Kho đã tồn tại.";
                public const string Warehouse_NotFound = "Kho không tồn tại.";
                public const string Warehouse_NotEditType = "Không thể sửa loại kho khi đã tạo đơn hàng.";
            }

            public static class Delete {
                public const string Warehouse_Notfound = "Kho hàng không tồn tại.";
                public const string Warehouse_Active = "Chỉ xoá được kho với trạng thái dừng hoạt động.";
                public const string Warehouse_NotDelete = "Chỉ xoá được kho khi không có sản phẩm nào đang được bán tại kho và tồn kho của tất cả sản phẩm bằng 0.";
            }
        }

        public static class Store {

            public static class CreateOrUpdate {
                public const string Store_Existed = "Cửa hàng đã tồn tại.";
                public const string Store_NotFound = "Cửa hàng không tồn tại.";
            }

            public static class Delete {
                public const string Store_Notfound = "Cửa hàng không tồn tại.";
                public const string Store_NotDelete = "Chỉ xoá được cửa hàng với trạng thái dừng hoạt động và không có sản phẩm nào được bán trong cửa hàng.";
            }
        }

        public static class WarehouseImport {

            public static class CreateOrUpdate {
                public const string WarehouseImport_Supplier_NotFound = "Nhà cung cấp không hợp lệ.";
                public const string WarehouseImport_Warehouse_NotFound = "Kho không hợp lệ.";
                public const string WarehouseImport_Items_Empty = "Danh sách sản phẩm trong phiếu nhập không được để trống.";
                public const string WarehouseImport_CreateWithVoidType = "Phiếu nhập không thể được tạo với trạng thái [Đã xoá].";
                public const string WarehouseImport_Existed = "Phiếu nhập hàng đã tồn tại.";
                public const string WarehouseImport_NotFound = "Phiếu nhập hàng không tồn tại.";
                public const string WarehouseImport_NotChange = "Bạn không thể thay đổi phiếu nhập hàng.";
                public const string WarehouseImport_NoLessThan = "Ngày nhận không được nhỏ hơn ngày giao hàng.";
            }
        }

        public static class WarehouseExport {

            public static class CreateOrUpdate {
                public const string WarehouseExport_Order_NotFound = "Đơn hàng không hợp lệ.";
                public const string WarehouseExport_Warehouse_NotFound = "Kho không hợp lệ.";
                public const string WarehouseExport_Items_Empty = "Danh sách sản phẩm trong phiếu xuất kho không được để trống.";
                public const string WarehouseExport_CreateWithVoidType = "Phiếu xuất kho không thể được tạo với trạng thái [Đã xoá].";
                public const string WarehouseExport_Existed = "Phiếu xuất kho đã tồn tại.";
                public const string WarehouseExport_NotFound = "Phiếu xuất kho không tồn tại.";
                public const string WarehouseExport_NotChange = "Bạn không thể thay đổi phiếu xuất kho.";
            }
        }

        public static class WarehouseAdjustment {

            public static class CreateOrUpdate {
                public const string WarehouseAdjustment_Warehouse_NotFound = "Kho không hợp lệ.";
                public const string WarehouseAdjustment_Items_Empty = "Danh sách sản phẩm trong phiếu kiểm kho không được để trống.";
                public const string WarehouseAdjustment_Existed = "Phiếu kiểm kho đã tồn tại.";
                public const string WarehouseAdjustment_NotFound = "Phiếu kiểm kho không tồn tại.";
            }

            public static class Delete {
                public const string WarehouseAdjustment_NotFound = "Phiếu kiểm kho không tồn tại.";
            }

            public static class Adjust {
                public const string WarehouseAdjustment_NotFound = "Phiếu kiểm kho không tồn tại.";
                public const string WarehouseAdjustment_Adjusted = "Phiếu kiểm kho đã được điều chỉnh.";
            }
        }

        public static class WarehouseTransfer {

            public static class CreateOrUpdate {
                public const string WarehouseTransfer_FromWarehouse_NotFound = "Kho xuất không hợp lệ.";
                public const string WarehouseTransfer_ToWarehouse_NotFound = "Kho đến không hợp lệ.";
                public const string WarehouseTransfer_Items_Empty = "Danh sách sản phẩm trong phiếu chuyển kho không được để trống.";
                public const string WarehouseTransfer_CreateWithVoidType = "Phiếu chuyển kho không thể được tạo với trạng thái [Đã xoá].";
                public const string WarehouseTransfer_Existed = "Phiếu chuyển kho đã tồn tại.";
                public const string WarehouseTransfer_NotFound = "Phiếu chuyển kho không tồn tại.";
                public const string WarehouseTransfer_NotChange = "Bạn không thể thay đổi phiếu chuyển kho.";
                public const string WarehouseTransfer_ExportedDate_GreaterThan_ImportedDate = "Ngày xuất kho không thể lớn hơn ngày nhập kho";
            }
        }

        public static class Order {

            public static class Create {
                public const string Order_NoItems = "Đơn hàng không được để trống.";
                public const string Warehouse_NotFound = "Không tìm thấy kho.";
            }

            public static class Update {
                public const string Order_NotUpdate = "Không thể cập nhật đơn hàng.";
                public const string Order_NoItems = "Đơn hàng không được để trống.";
                public const string Order_NotFound = "Không tìm thấy đơn hàng.";
                public const string Order_InvalidStatus = "Không thể cập nhận đơn hàng với trạng thái {0}.";
                public const string Order_InvalidQuantity = "Bạn không thể cập nhật đơn hàng với số lượng sản phẩm nhỏ hơn số lượng đã xuất.";
            }

            public static class Void {
                public const string Order_NotFound = "Không tìm thấy đơn hàng.";
                public const string Order_InvalidStatus = "Không thể cập nhận đơn hàng với trạng thái {0}.";
            }

            public static class CreateOrUpdateOrderPayment {
                public const string Model_Invalid_OrderId = "Đơn hàng không hợp lệ, vui lòng kiểm tra lại.";
                public const string Model_Invalid_PaymentMethodId = "Phương thức thanh toán không được để trống.";
                public const string Model_Invalid_Value = "Số tiền thanh toán không được nhỏ hơn 0.";
                public const string Payment_InProgress = "Bạn không thể tiến hành thanh toán vì quá trình thanh toán đang diễn ra.";
                public const string Order_NotFound = "Không tìm thấy đơn hàng.";
                public const string Order_NotPayment = "Bạn không thể tiến hành thanh toán với đơn hàng với trạng thái {0}.";
                public const string PaymentMethod_NotFound = "Không tìm thấy phương thức thanh toán.";
                public const string PaymentMethod_NotPaymentOver = "Số tiền thanh toán vượt quá số còn lại.";
            }

            public static class Export {
                public const string Template_NotFound = "Không tìm thấy mẫu đơn hàng.";
                public const string Order_NotFound = "Không tìm thấy đơn hàng.";
                public const string Order_NotItems = "Bạn không thể xuất đơn hàng không có sản phẩm nào.";
            }

            public static class Refund {
                public const string Refund_Order_NotFound = "Không tìm thấy đơn hàng này.";
                public const string Refund_Warehouse_NotFound = "Không tìm thấy kho này.";
                public const string Refund_NotFound = "Phiếu trả hàng này không tồn tại.";
                public const string Refund_Existed = "Phiếu trả hàng này đã tồn tại.";
                public const string Refund_CreateWithType = "Không thể trả hàng với trạng thái đơn hàng là {0}.";
                public const string Refund_HasPayment = "Không thể sửa phiếu trả hàng khi đã tồn tại phiếu chi.";
                public const string Refund_RefundGtExported = "Không thể sửa phiếu trả hàng khi đã tồn tại phiếu chi.";
            }
        }

        public static class Category {

            public static class CreateOrUpdate {
                public const string Category_Exited = "Danh mục sản phẩm đã tồn tại.";
                public const string Category_NotFound = "Danh mục sản phẩm không tồn tại.";
            }

            public static class Delete {
                public const string Category_NotFound = "Danh mục sản phẩm không tồn tại.";
                public const string Category_DeleteFail = "Vui lòng xóa sản phẩm khỏi danh mục trước khi xóa.";
            }
        }

        public static class ProductReport {
            public const string ProductReport_NotFound = "Danh sách sản phẩm không tồn tại.";
        }

        public static class PaymentMethod {
            public const string PaymentMethod_NotFound = "Phương thức thanh toán không tồn tại.";
            public const string PaymentMethod_Exited = "Mã phương thức thanh toán đã tồn tại.";
            public const string PaymentMethod_NotDelete = "Phương thức thanh toán đang được đặt làm mặc định.";
        }

        public static class File {
            public const string File_NotFound = "Tệp dữ liệu không tồn tại.";
            public const string File_Error = "Đã có lỗi xảy ra khi lưu tệp.";
        }

        public static class Receipt {

            public static class CreateOrUpdate {
                public const string Receipt_Existed = "Mã phiếu thu đã tồn tại.";
                public const string Receipt_NotFound = "Mã phiếu thu không tồn tại.";
                public const string Receipt_ValueInvalid = "Thu tiền không hợp lệ.";
                public const string Customer_Required = "Khách hàng không được để trống.";
                public const string PaymentMethod_Required = "Phương thức thanh toán không được để trống.";
            }

            public static class Delete {
                public const string Receipt_NotFound = "Mã phiếu thu không tồn tại.";
                public const string CustomerDebt_NotFound = "Tài khoản khách hàng không tồn tại.";
            }
        }

        public static class Payment {

            public static class CreateOrUpdate {
                public const string Payment_Existed = "Mã phiếu chi đã tồn tại.";
                public const string Payment_NotFound = "Mã phiếu chi không tồn tại.";
                public const string Customer_Required = "Khách hàng không được để trống.";
                public const string PaymentMethod_Required = "Phương thức thanh toán không được để trống.";
                public const string Payment_ValueInvalid = "Tiền chi không hợp lệ.";
                public const string CustomerDebt_NotFound = "Tài khoản khách hàng không tồn tại.";
            }

            public static class Get {
                public const string CustomerId_NotNull = "Mã khách hàng không được để trống";
                public const string OrderId_NotNull = "Mã đơn hàng không được để trống";
            }

            public static class Delete {
                public const string Payment_NotFound = "Mã phiếu chi không tồn tại.";
                public const string CustomerDebt_NotFound = "Tài khoản khách hàng không tồn tại.";
            }
        }

        public static class Sync {
            public const string Session_NotFound = "Phiên đồng bộ không tồn tại.";
            public const string Session_StepInvalid = "Bước đồng bộ không hợp lệ.";
        }

        public static class WarehouseExportOther {

            public static class CreateOrUpdate {
                public const string WarehouseExportOther_Existed = "Mã phiếu xuất khác đã tồn tại";
                public const string WarehouseExport_NotFound = "Kho xuất không tồn tại";
            }

            public static class Delete {
                public const string WarehouseExportOther_NotFound = "Phiếu xuất khác không tồn tại";
            }

            public static class Get {
                public const string WarehouseExportOther_IdNotNull = "Id phiếu xuất khác không được để trống";
            }
        }
    }
}
