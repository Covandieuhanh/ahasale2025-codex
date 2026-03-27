using System.Collections.Generic;

public static class GianHangPosCartView_cl
{
    public sealed class CartLine
    {
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
    }

    public sealed class CartSummaryState
    {
        public decimal TotalVnd { get; set; }
        public decimal TotalRights { get; set; }
        public int TotalCount { get; set; }
        public string TotalVndText { get; set; }
        public string TotalRightsText { get; set; }
        public string TotalCountText { get; set; }
    }

    public static CartSummaryState BuildSummary(IEnumerable<CartLine> lines)
    {
        decimal totalVnd = 0m;
        int totalCount = 0;

        if (lines != null)
        {
            foreach (CartLine line in lines)
            {
                if (line == null)
                    continue;

                int quantity = line.SoLuong < 0 ? 0 : line.SoLuong;
                decimal price = line.GiaBan < 0m ? 0m : line.GiaBan;
                totalVnd += price * quantity;
                totalCount += quantity;
            }
        }

        decimal totalRights = GianHangCheckoutCore_cl.ConvertVndToRights(totalVnd);
        return new CartSummaryState
        {
            TotalVnd = totalVnd,
            TotalRights = totalRights,
            TotalCount = totalCount,
            TotalVndText = totalVnd.ToString("#,##0"),
            TotalRightsText = totalRights.ToString("0.00"),
            TotalCountText = totalCount.ToString("#,##0")
        };
    }
}
