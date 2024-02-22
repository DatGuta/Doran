using DR.Common.Hashers;
using DR.Constant.Enums;
using DR.Database.Models;
using DR.Helper;

namespace DR.ManageApi.Application.Handlers.CustomerRpHandlers.Queries;

public class ListCustomerRpQuery : PaginatedRequest<ListCustomerRpData> {
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool ShowAll { get; set; }
    public List<string> CustomerIds { get; set; } = new();
    public bool IsAll { get; set; }
}

public class ListCustomerRpData : PaginatedList<CustomerRpDto> { }

internal class ListCustomerRpHandler(IServiceProvider serviceProvider) : BaseHandler<ListCustomerRpQuery, ListCustomerRpData>(serviceProvider) {

    public override async Task<ListCustomerRpData> Handle(ListCustomerRpQuery request, CancellationToken cancellationToken) {
        request.CustomerIds ??= [];

        var queryCustomerTracking = db.CustomerTrackings.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.IsUpdate && !o.IsDelete)
            .Where(o => request.StartDate <= o.Date && o.Date <= request.EndDate);

        var queryCustomer = db.Customers.AsNoTracking()
            .OrderBy(o => o.Code)
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
            .WhereIf(request.CustomerIds.Count > 0, o => request.CustomerIds.Contains(o.Id))
            .WhereFunc(!string.IsNullOrWhiteSpace(request.SearchText), q => {
                var searchText = StringHelper.UnsignedUnicode(request.SearchText!);
                var searchPhone = PhoneHasher.Encrypt(searchText);
                return q.Where(o => o.Code.Contains(searchText!.ToUpper())
                    || o.SearchName.Contains(searchText)
                    || o.CustomerPhones != null && o.CustomerPhones.Any(x => x.SearchPhone == searchPhone || x.SearchLastPhone == searchPhone));
            })
            .WhereFunc(!request.ShowAll, q => {
                var trackingCustomerIds = queryCustomerTracking.Select(o => o.CustomerId).Distinct();
                return q.Where(o => trackingCustomerIds.Contains(o.Id));
            });

        int count = await queryCustomer.CountIf(request.IsCount, o => o.Id, cancellationToken);

        if (!request.IsAll) {
            queryCustomer = queryCustomer.Skip(request.Skip).Take(request.Take);
        }

        var items = await queryCustomer.Select(o => new CustomerRpDto {
            Id = o.Id,
            Code = o.Code,
            Name = o.Name,
        }).ToListAsync(cancellationToken);

        var trackings = await queryCustomerTracking.Join(queryCustomer, t => t.CustomerId, c => c.Id, (t, c) => t)
            .ToListAsync(cancellationToken);
        var cusIdsHasTracking = trackings.Select(o => o.CustomerId).Distinct().ToList();

        var lastTrackings = new List<CustomerTracking>();
        if (items.Count > cusIdsHasTracking.Count && request.StartDate.HasValue) {
            lastTrackings = await GetLastTracking(request.MerchantId, request.StartDate.Value,
                queryCustomer, queryCustomerTracking, cancellationToken);
        }

        var debtDocTyeps = new List<ECustomerDocType> { ECustomerDocType.Order, ECustomerDocType.OrderRefund };
        var balanceDocTypes = new List<ECustomerDocType> { ECustomerDocType.Receipt, ECustomerDocType.PaymentRefund, ECustomerDocType.PaymentStandard };

        foreach (var item in items) {
            var cusTrackings = trackings.FindAll(o => o.CustomerId == item.Id);
            if (cusTrackings.Count > 0) {
                var firstTracking = cusTrackings.OrderBy(o => o.Date).First();

                item.OpeningDebt = firstTracking.DebtBefore;
                item.OpeningBalance = firstTracking.BalanceBefore;

                item.CurrentDebt = cusTrackings.FindAll(o => debtDocTyeps.Contains(o.DocumentType)).Sum(o => o.Debt);
                item.CurrentBalance = cusTrackings.FindAll(o => balanceDocTypes.Contains(o.DocumentType)).Sum(o => o.Balance);
            } else {
                var lastTracking = lastTrackings.Find(o => o.CustomerId == item.Id);
                if (lastTracking != null) {
                    item.OpeningDebt = lastTracking.DebtAfter;
                    item.OpeningBalance = lastTracking.BalanceAfter;

                    item.CurrentDebt = 0;
                    item.CurrentBalance = 0;
                }
            }

            item.EndDebt = item.OpeningDebt + item.CurrentDebt - item.CurrentBalance;
            item.EndBalance = item.OpeningBalance + item.CurrentBalance - item.CurrentDebt;

            if (item.OpeningDebt < 0) item.OpeningDebt = 0;
            if (item.OpeningBalance < 0) item.OpeningBalance = 0;
            if (item.CurrentDebt < 0) item.CurrentDebt = 0;
            if (item.CurrentBalance < 0) item.CurrentBalance = 0;
            if (item.EndDebt < 0) item.EndDebt = 0;
            if (item.EndBalance < 0) item.EndBalance = 0;
        }

        return new() {
            Count = count,
            Items = items,
        };
    }

    private async Task<List<CustomerTracking>> GetLastTracking(string merchantId, DateTimeOffset startDate,
        IQueryable<Customer> queryCustomer, IQueryable<CustomerTracking> queryCustomerTracking, CancellationToken cancellationToken) {
        var trackingCustomerIds = queryCustomerTracking.Select(o => o.CustomerId).Distinct();
        var queryCustomerNoTracking = queryCustomer.Where(o => !trackingCustomerIds.Contains(o.Id));

        var queryLastTracking = db.CustomerTrackings.AsNoTracking()
            .Where(o => o.MerchantId == merchantId && o.IsUpdate && !o.IsDelete && o.Date < startDate);

        var maxDate = queryLastTracking.Join(queryCustomerNoTracking, t => t.CustomerId, c => c.Id, (t, c) => t)
            .GroupBy(o => o.CustomerId)
            .Select(o => new {
                CustomerId = o.Key,
                Date = o.Max(x => x.Date),
            });

        return await queryLastTracking.Join(maxDate, t => new { t.CustomerId, t.Date }, d => new { d.CustomerId, d.Date }, (t, d) => t)
            .ToListAsync(cancellationToken);
    }

    private async Task<ListCustomerRpData> HandleOld(ListCustomerRpQuery request, CancellationToken cancellationToken) {
        var handleStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Export, EOrderStatus.Exported, EOrderStatus.Ticket };
        request.CustomerIds ??= new();

        // Trong kỳ: Lọc khách hàng từ 2 điều kiện trên
        var customers = db.Customers.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
            .WhereIf(!request.ShowAll, x => x.Orders != null && x.Orders.Any(o => o.CreatedDate >= request.StartDate && o.CreatedDate <= request.EndDate))
            .WhereIf(request.CustomerIds.Count > 0, o => request.CustomerIds.Contains(o.Id))
            .WhereFunc(!string.IsNullOrWhiteSpace(request.SearchText), q => {
                var searchText = StringHelper.UnsignedUnicode(request.SearchText!);
                var searchPhone = PhoneHasher.Encrypt(searchText);
                return q.Where(o => o.Code.Contains(searchText!.ToUpper())
                    || o.SearchName.Contains(searchText)
                    || o.CustomerPhones != null && o.CustomerPhones.Any(x => x.SearchPhone == searchPhone || x.SearchLastPhone == searchPhone));
            });

        var customersCount = await customers.CountIf(request.IsCount, o => o.Id, cancellationToken);
        var pagedCustomers = await customers.OrderBy(o => o.Code)
            .WhereFunc(!request.IsAll, p => p.Skip(request.PageIndex * request.PageSize).Take(request.PageSize))
            .Select(o => new CustomerRpDto {
                Id = o.Id,
                Code = o.Code,
                Name = o.Name,
            }).ToListAsync(cancellationToken);
        var customerIds = pagedCustomers.Select(o => o.Id).ToList();

        #region Công nợ khách hàng

        // Lấy Công nợ khách hàng
        var queryOrders = db.Orders.AsNoTracking()
                              .Where(o => o.MerchantId == request.MerchantId
                                          && customerIds.Contains(o.CustomerId ?? "")
                                          && o.TotalBill > decimal.Zero //&& o.Remaining > decimal.Zero
                                          && handleStatus.Contains(o.Status) /*&& handlePaymentStatus.Contains(o.PaymentStatus)*/);
        // Đầu kỳ
        // Danh sách đơn hàng
        var customerOpeningDebts = await queryOrders.Where(o => o.CreatedDate < request.StartDate)
                                                    .GroupBy(o => o.CustomerId!)
                                                    .ToDictionaryAsync(k => k.Key, v => v.Sum(o => o.TotalBill), cancellationToken);
        // Trong kỳ
        // Danh sách đơn hàng
        var customerDebts = await queryOrders.Where(o => o.CreatedDate >= request.StartDate && o.CreatedDate <= request.EndDate)
                                             .GroupBy(o => o.CustomerId!)
                                             .ToDictionaryAsync(k => k.Key, v => v.Sum(o => o.TotalBill), cancellationToken);

        var queryReturnItems = await (from oad in db.OrderActionDetails.AsNoTracking()
                                      join oa in db.OrderActions.AsNoTracking() on oad.OrderActionId equals oa.Id
                                      join o in db.Orders.AsNoTracking() on oa.OrderId equals o.Id
                                      where o.MerchantId == request.MerchantId
                                          && customerIds.Contains(o.CustomerId ?? "")
                                          && o.TotalBill > decimal.Zero
                                          && handleStatus.Contains(o.Status)
                                          && oa.Type == EOrderAction.Refund
                                          && !oa.IsDelete
                                          && oa.Date <= request.EndDate
                                      group new { o.CustomerId, oa.Date, ReturnAmount = oad.Quantity * oad.Price }
                                          by new { o.CustomerId, Range = oa.Date < request.StartDate ? 0 : 1 }
                               into grouped
                                      select new {
                                          grouped.Key.CustomerId,
                                          grouped.Key.Range,
                                          ReturnAmount = grouped.Sum(x => x.ReturnAmount)
                                      })
                                .ToDictionaryAsync(k => new { k.CustomerId, k.Range }, v => v.ReturnAmount, cancellationToken);

        #endregion Công nợ khách hàng

        #region Phiếu thu

        // Phiếu thu: Khách hàng thanh toán tiền hàng
        var queryAccountReceipts = db.Receipts.AsNoTracking()
                                       .Where(p => p.MerchantId == request.MerchantId && !p.IsDelete)
                                       .Where(o => customerIds.Contains(o.CustomerId));
        // Đầu kỳ
        // Thanh toán đơn hàng
        var accountOpeningReceipts = await queryAccountReceipts
                                           .Where(o => o.ReceiptDate < request.StartDate)
                                           .Select(r => new {
                                               r.CustomerId,
                                               ReceiptValue = r.Value,
                                               ReceiptOrderValue = r.ReceiptDetails == null ? 0m : r.ReceiptDetails.Sum(rd => rd.Value)
                                           })
                                           .GroupBy(o => o.CustomerId)
                                           .ToDictionaryAsync(k => k.Key, v => new {
                                               ReceiptValue = v.Sum(o => o.ReceiptValue),
                                               ReceiptOrderValue = v.Sum(o => o.ReceiptOrderValue)
                                           }, cancellationToken);
        // Trong kỳ
        // Thanh toán đơn hàng
        var accountReceipts = await queryAccountReceipts
                                    .Where(o => o.ReceiptDate >= request.StartDate && o.ReceiptDate <= request.EndDate)
                                    .Select(r => new {
                                        r.CustomerId,
                                        ReceiptValue = r.Value,
                                        ReceiptOrderValue = r.ReceiptDetails == null ? 0m : r.ReceiptDetails.Sum(rd => rd.Value)
                                    })
                                    .GroupBy(o => o.CustomerId)
                                    .ToDictionaryAsync(k => k.Key, v => new {
                                        ReceiptValue = v.Sum(o => o.ReceiptValue),
                                        ReceiptOrderValue = v.Sum(o => o.ReceiptOrderValue)
                                    }, cancellationToken);

        #endregion Phiếu thu

        #region Phiếu chi

        // Phiếu chi:
        // - Trả hàng + Cửa hàng chuyển tiền vào tài khoản KH
        // - Hoàn tiền cho khách hàng
        var queryAccountPayments = db.Payments.AsNoTracking()
                                       .Where(p => p.MerchantId == request.MerchantId && !p.IsDelete)
                                       .Where(o => customerIds.Contains(o.CustomerId));

        // Đầu kỳ
        // Thanh toán trả hàng
        // var accountOpeningStandardPayments = await queryAccountPayments
        //                                            .Where(o => o.Type == EPayment.Standard && o.CreatedDate < request.StartDate)
        //                                            .GroupBy(o => o.CustomerId)
        //                                            .ToDictionaryAsync(k => k.Key, v => v.Sum(o => o.Value), cancellationToken);
        // Hoàn tiền khách hàng
        var accountOpeningRefundPayments = await queryAccountPayments
                                                 .Where(o => o.Type == EPayment.Refund && o.PaymentDate < request.StartDate)
                                                 .GroupBy(o => o.CustomerId)
                                                 .ToDictionaryAsync(k => k.Key, v => v.Sum(o => o.Value), cancellationToken);
        // Trong kỳ
        // Thanh toán trả hàng
        var accountStandardPayments = await queryAccountPayments
                                            .Where(o => o.Type == EPayment.Standard && o.PaymentDate >= request.StartDate && o.PaymentDate <= request.EndDate)
                                            .GroupBy(o => o.CustomerId)
                                            .ToDictionaryAsync(k => k.Key, v => v.Sum(o => o.Value), cancellationToken);
        // Hoàn tiền khách hàng
        var accountRefundPayments = await queryAccountPayments
                                          .Where(o => o.Type == EPayment.Refund && o.PaymentDate >= request.StartDate && o.PaymentDate <= request.EndDate)
                                          .GroupBy(o => o.CustomerId)
                                          .ToDictionaryAsync(k => k.Key, v => v.Sum(o => o.Value), cancellationToken);

        #endregion Phiếu chi

        foreach (var customer in pagedCustomers) {

            #region Đầu kỳ

            // Nợ đầu kỳ
            var openingOrderValue = customerOpeningDebts.GetValueOrDefault(customer.Id);

            // Nếu có trả hàng thì sẽ phải trừ thêm số tiền trả hàng
            var openingReturnValue = queryReturnItems.GetValueOrDefault(new { CustomerId = customer.Id, Range = 0 });
            openingOrderValue -= openingReturnValue;

            var openingRefundPayment = accountOpeningRefundPayments.GetValueOrDefault(customer.Id);
            openingOrderValue -= openingRefundPayment;

            // Có đầu kỳ
            var openingReceipt = accountOpeningReceipts.GetValueOrDefault(customer.Id)?.ReceiptValue ?? 0m;

            // Có trừ Nợ
            var openingValue = openingReceipt - openingOrderValue;
            if (openingValue >= 0m) {
                // Khi số TIỀN THANH TOÁN trừ hết cho số TIỀN NỢ => Khách hàng này đã thanh toán hết và có thể đang thanh toán dư.
                customer.OpeningDebt = 0m;
                customer.OpeningBalance = openingValue;
            } else {
                // Khi số TIỀN THANH TOÁN trừ KHÔNG hết cho số TIỀN NỢ => Khách hàng này còn nợ.
                customer.OpeningDebt = Math.Abs(openingValue);
                customer.OpeningBalance = 0m;
            }

            #endregion Đầu kỳ

            #region Trong kỳ

            // Nợ trong kỳ
            var currentOrderValue = customerDebts.GetValueOrDefault(customer.Id);
            customer.CurrentDebt = currentOrderValue;

            // Nếu có trả hàng thì sẽ phải trừ thêm số tiền trả hàng
            var currentReturnValue = queryReturnItems.GetValueOrDefault(new { CustomerId = customer.Id, Range = 1 });
            customer.CurrentDebt -= currentReturnValue;

            // Có trong kỳ
            var currentReceipt = accountReceipts.GetValueOrDefault(customer.Id);
            customer.CurrentBalance = currentReceipt?.ReceiptValue ?? 0m;

            var currentRefundPayment = accountRefundPayments.GetValueOrDefault(customer.Id);
            customer.CurrentBalance -= currentRefundPayment;

            #endregion Trong kỳ

            #region Cuối kỳ

            var totalDebt = customer.OpeningDebt + customer.CurrentDebt;
            var totalBalance = customer.OpeningBalance + customer.CurrentBalance;

            // Có trừ Nợ
            var endValue = totalBalance - totalDebt;
            if (endValue >= 0m) {
                customer.EndDebt = 0m;
                customer.EndBalance = endValue;
            } else {
                customer.EndDebt = Math.Abs(endValue);
                customer.EndBalance = 0m;
            }

            #endregion Cuối kỳ

            var standardPayment = accountStandardPayments.GetValueOrDefault(customer.Id, decimal.Zero);
            //customer.HasPayment = currentReturnValue - standardPayment > 0;
        }

        return new() {
            Items = pagedCustomers,
            Count = customersCount,
        };
    }
}
